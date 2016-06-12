using UnityEngine;
using System.Collections;
using UnityEditor;

//[CustomEditor(typeof(CustomFontCreator))]
//[CanEditMultipleObjects]
public class CustomFontCreatorPlugin {
	[MenuItem("Tools/创建Unity字体")][CanEditMultipleObjects]
	public static void OnCreateBMPFont()
	{
		const string fntExtension = ".fnt";

		var textObject = Selection.activeObject as TextAsset;
		if(textObject!=null)
		{
			var path = AssetDatabase.GetAssetPath (textObject);
			if (path.LastIndexOf (fntExtension) == path.Length - fntExtension.Length) 
			{
				CreateBMPFont (textObject);
			}
			else 
			{
				// not a bmp font
				Debug.LogFormat ("Object is not a fnt file: {0}", path);
			}
		}
		else
		{
			Debug.LogFormat ("Selected Object is not a fnt file: {0}", textObject.name);
		}
	}

//	public override void OnInspectorGUI ()
//	{
//		if(GUILayout.Button("Create Custom Font"))
//		{
//			CustomFontCreator creator = target as CustomFontCreator;
//			Debug.Log ("Create Custom Font: " + target.name);
//
//			if(creator._FntSettings == null)
//			{
//				Debug.LogError ("No fnt file configured");
//				return;
//			}
//
//			CreateBMPFont (creator._FntSettings);
//		}
//
//		base.OnInspectorGUI ();
//	}

	public static void CreateBMPFont(TextAsset fntSetting)
	{
//		TextAsset fntSetting = creator._FntSettings;

		// 公共路径
		string path = AssetDatabase.GetAssetPath (fntSetting);

		var idx = path.LastIndexOf ("/");
		Debug.Assert (idx >= 0);

		string basePath = path.Substring (0, idx);
		Debug.Log ("Path: " + basePath);

		// 公共文件名：去掉后缀后的名字
		var fileName = path.Substring (idx+1);
		var extIdx = fileName.LastIndexOf (".");
		if(extIdx>=0)
		{
			fileName = fileName.Substring (0, extIdx);	
		}

		Debug.Log ("File Name: " + fileName);
		Debug.Assert (basePath.Length > 0 && fileName.Length > 0);

		BMFont bmfont = getBMFontInfo (fntSetting);
		if(bmfont==null)
		{
			Debug.Log ("Invalid .fnt file");
			return;
		}

		var texturePath = basePath + "/" + bmfont.spriteName + ".png";
		Debug.Log ("Texture Path: " + texturePath);

		var fntTexture = AssetDatabase.LoadAssetAtPath<Texture> (texturePath);
		if(fntTexture == null)
		{
			Debug.LogError ("Can't find texture at target path");
			return;
		}

		string materialPath = basePath + "/" + fileName + "M.mat";
		Material fntMaterial = AssetDatabase.LoadAssetAtPath<Material> (materialPath);
		if(fntMaterial == null)
		{
			fntMaterial = createFontMaterial (materialPath, fntTexture);
//			EditorUtility.SetDirty(fntMaterial);
		}

		string fntPath = basePath + "/" + fileName + "U.fontsettings";
		Font fnt = AssetDatabase.LoadAssetAtPath<Font> (fntPath);
		if(fnt == null)
		{
			fnt = createCustomFnt (fntPath, fntMaterial);
		}

		var charInfos = retriveCharInfos (bmfont);
		fnt.characterInfo = charInfos;

		// FIXME: #BUG# 关闭Unity后，自定义字体数据消失 (charInfos)
		EditorUtility.SetDirty(fntMaterial);
		EditorUtility.SetDirty(fnt);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
	}

	public static BMFont getBMFontInfo(TextAsset fntSettings)
	{
		BMFont bmFont = new BMFont ();

		// 读取fnt字体信息
		BMFontReader.Load(bmFont, fntSettings.name, fntSettings.bytes);

		return bmFont;
	}

	public static Material createFontMaterial(string path, Texture fntTexture)
	{
		Shader textShader = Shader.Find ("GUI/Text Shader");
		Debug.Assert (textShader != null);

		Material fntMaterial = new Material (textShader);

//		fntMaterial.shader = textShader;
		fntMaterial.SetTexture ("_MainTex", fntTexture);

		AssetDatabase.CreateAsset (fntMaterial, path);
		if(AssetDatabase.Contains(fntMaterial))
		{
			Debug.Log ("Font created: " + path);
		}

		return fntMaterial;
	}

	public static Font createCustomFnt(string path, Material fntMaterial)
	{
		Font fnt = new Font ();

		fnt.material = fntMaterial;

		AssetDatabase.CreateAsset (fnt, path);

		if(AssetDatabase.Contains(fnt))
		{
			Debug.Log ("Font created: " + path);
		}
		return fnt;
	}

	public static CharacterInfo[] retriveCharInfos(BMFont bmFont)
	{
//		BMFont bmFont = new BMFont ();
//
//		// 读取fnt字体信息
//		BMFontReader.Load(bmFont, fntSettings.name, fntSettings.bytes);

		// 创建Unity自定义字体信息
		CharacterInfo[] characterInfo = new CharacterInfo[bmFont.glyphs.Count];
		for (int i = 0; i < bmFont.glyphs.Count; i++)
		{
			BMGlyph bmInfo = bmFont.glyphs[i];
			CharacterInfo info = new CharacterInfo();
			info.index = bmInfo.index;
			//             float x = (float)bmInfo.x / (float)mbFont.texWidth;
			//             float y = 1 - (float)bmInfo.y / (float)mbFont.texHeight;
			// info.uvBottomLeft = new Vector2(x, y);
//			info.uv.x = (float)bmInfo.x / (float)bmFont.texWidth;
//			info.uv.y = 1 - (float)bmInfo.y / (float)bmFont.texHeight;
//			info.uv.width = (float)bmInfo.width / (float)bmFont.texWidth;
//			info.uv.height = -1f * ((float)bmInfo.height )/ (float)bmFont.texHeight;
//			info.vert.x = (float)bmInfo.offsetX; 
//			info.vert.y = 0f;
//			info.vert.width = (float)bmInfo.width;
//			info.vert.height = (float)bmInfo.height;
//			info.width = (float)bmInfo.advance;

			// 左上
			info.uvTopLeft = new Vector2 ((float)bmInfo.x / (float)bmFont.texWidth, 1.0f - (float)bmInfo.y / (float)bmFont.texHeight);

			// 右下
			info.uvBottomRight = new Vector2 ((float)(bmInfo.width + bmInfo.x) / (float)bmFont.texWidth, 
												1.0f - ((float)(bmInfo.y + bmInfo.height))/ (float)bmFont.texHeight);

			info.minX = bmInfo.offsetX; 
			info.minY = bmInfo.offsetY;
			info.maxX = info.minX + bmInfo.width;
			info.maxY = info.maxY + bmInfo.height;
			info.advance = bmInfo.advance;

//			info.uvBottomLeft.x = (float)bmInfo.x / (float)bmFont.texWidth;
//			info.uvBottomLeft.y = 1 - (float)bmInfo.y / (float)bmFont.texHeight;
//			info.uv.width = (float)bmInfo.width / (float)bmFont.texWidth;
//			info.uv.height = -1f * ((float)bmInfo.height )/ (float)bmFont.texHeight;
//			info.vert.x = (float)bmInfo.offsetX; 
//			info.vert.y = 0f;
//			info.vert.width = (float)bmInfo.width;
//			info.vert.height = (float)bmInfo.height;
//			info.width = (float)bmInfo.advance;

			characterInfo[i] = info;
		}

//		_TargetFnt.characterInfo = characterInfo;

		return characterInfo;
	}
}
