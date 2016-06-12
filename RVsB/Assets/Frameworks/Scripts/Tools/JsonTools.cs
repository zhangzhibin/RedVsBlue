using UnityEngine;
using System.Collections;

namespace TDGame{
	public static class JsonTools 
	{
		public static T CreateObjectFromJsonFile<T>(string jsonPath)
		{
			T obj = default(T);
			string jsonString; 
            bool errCode = IOTools.LoadTextFromFile (jsonPath, out jsonString);
			if (jsonString == null || jsonString.Length <= 0) {
				Debug.Log ("Can't or Empty file: " + jsonPath);
			}
			else 
			{
				obj = JsonUtility.FromJson<T> (jsonString);
			}

			if(obj == null)
			{
				Debug.Log ("Failed to Parse json to object: " + jsonPath);	
			}

			return obj;
		}

		public static void OverrideObjectWithJsonFile<T>(string jsonPath, T obj)
		{
			string jsonString;
            bool errCode = IOTools.LoadTextFromFile (jsonPath, out jsonString);
			if (jsonString == null || jsonString.Length <= 0) 
			{
				JsonUtility.FromJsonOverwrite (jsonPath, obj);
			}
		}

//		public static void CreateObjectFromJsonString<T>(string jsonString, T obj)
//		{
//			string jsonString;
//			JsonUtility.FromJsonOverwrite (jsonPath, obj);
//			}
//		}

//		public static T FromJsonFile<T>(this JsonUtility jsonUtils, string jsonPath)
//		{
//			static class 不能作为类型，所以也不能扩展	
//		}
	}
}

