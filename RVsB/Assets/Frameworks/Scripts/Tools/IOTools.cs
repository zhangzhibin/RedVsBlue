using UnityEngine;
using System.Collections;
using System.IO;

public static class IOTools{

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}

    /// <summary>
    /// 加载返回文件内容
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="content">文件内容</param>
    /// <returns></returns>
    static public bool LoadTextFromFile(string filePath, out string content)
    {
        content = "";
		var text = Resources.Load (filePath, typeof(TextAsset)) as TextAsset;
		if (text != null) {
			if (text.text.Length <= 0) {
				Debug.LogWarning ("Possible empty file: " + filePath);
                return false;
			} else {
				Debug.Log ("load file: " + filePath);
				Debug.Log ("Content:\n" + text.text);
			}
			content = text.text;
		} else {
			Debug.LogWarning ("Can't load file: " + filePath);
            return false;
		}

        return true;
	}

    /// <summary>
    /// 从配置文件中加载tilemap信息
    /// </summary>
    /// <param name="filePath">TileMap json 文件路径</param>
    /// <param name="tileMap"></param>
    /// <returns></returns>
    static public bool LoadTileMap(string filePath, out TiledMap tileMap)
    {
        tileMap = null;

        //读取地图文件
        string jsonString = null;
        var errCode = IOTools.LoadTextFromFile(filePath, out jsonString);
        if (errCode)
        {
            tileMap = JsonUtility.FromJson<TiledMap>(jsonString);
        }

        return errCode;
    }

	static public bool SaveTextFile(string fileRelativePath, string content)
	{
		// https://msdn.microsoft.com/en-us/library/system.io.file(v=vs.110).aspx
		// https://msdn.microsoft.com/en-us/library/system.io.file.createtext(v=vs.110).aspx

	#if UNITY_WEBPLAYER
		return false;
	#else
		bool success = true;
		StreamWriter fileWriter = null;
		string path = Path.Combine(Application.persistentDataPath, fileRelativePath);

		fileWriter = File.CreateText (path);

		if (fileWriter != null) {
			try {
				fileWriter.WriteLine (content);
			} catch (System.Exception e) {
				Debug.LogErrorFormat ("Write to file [{0}] failed: \n{1}", path, e);
				success = false;
			}

			fileWriter.Close ();
		}
		else
		{
			Debug.LogErrorFormat ("Open/Create file [{0}] failed", path);
			success = false;
		}

		return success;
	#endif
	}

	static public bool ReadTextFile(string fileRelativePath, out string content)
	{
		#if UNITY_WEBPLAYER
		content = "";
		return false;
		#else
		bool success = true;
		content = "";
		string path = Path.Combine(Application.persistentDataPath, fileRelativePath);

		if(!File.Exists(path))
		{
			success = false;
			Debug.LogFormat ("File not exist: {0}", path);

			return success;
		}

		var fileReader = File.OpenText (path);
		Debug.Assert (fileReader != null);

		content = fileReader.ReadToEnd ();

		fileReader.Close ();

		return success;
		#endif
	}

	static public bool CreateFolder(string folderPath)
	{
		string path = Path.Combine(Application.persistentDataPath, folderPath);

		if(!Directory.Exists(path))
		{
			Directory.CreateDirectory (path);
		}

		return true;
	}


}
