using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

//[CustomEditor(typeof(TextAsset))]
//[MenuItem("Tools/Create Custom Font from BMP font")]

//public class BMPFontPlugin : Editor {
//	bool isBMPFont = false;
//
//	private Editor _editor;
//
//	void Awake()
//	{
//		if(_editor == null)
//		{
//			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//			foreach (var a in assemblies)
//			{
//				var type = a.GetType("UnityEditor.TextAssetInspector");
//				if (type != null)
//				{
//					_editor = Editor.CreateEditor(target, type);
//					break;
//				}
//			}
//		}
//
//		string path = AssetDatabase.GetAssetPath (target);
//		if(path.LastIndexOf(".fnt") == path.Length - 4)
//		{
//			isBMPFont = true;
//		}
//
//		if(isBMPFont)
//		{
//			GUI.enabled = true;
//		}
//	}
//}
