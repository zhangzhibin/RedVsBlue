using UnityEngine;
using System.Collections;

public class SingletonMonoBehaviour<T> where T : MonoBehaviour
{
	protected static T instance;

	/**
      Returns the instance of this singleton.
   */
	public static T Instance
	{
		get
		{
			if(instance == null)
			{
				instance = (T) GameObject.FindObjectOfType(typeof(T));

				if (instance == null)
				{
//					Debug.LogError("An instance of " + typeof(T) + 
//						" is needed in the scene, but there is none.");
				}
			}
			return instance;
		}
	}

	public static bool DestroyExtraObjects(GameObject obj)
	{
		bool shouldDestroy = false;
		if(obj!=Instance.gameObject)
		{
			GameObject.Destroy (obj);
			shouldDestroy = true;
		}
		else
		{
			GameObject.DontDestroyOnLoad (obj);
		}

		return shouldDestroy;
	}

	public static bool DestroyExtraObjects(MonoBehaviour scriptObj)
	{
		return DestroyExtraObjects (scriptObj.gameObject);
	}
}

public interface ISingleton
{
	bool Init();
}

public class Singleton<T> where T : class, ISingleton
{
	protected static T instance;

	/**
      Returns the instance of this singleton.
   */
	public static T Instance
	{
		get
		{
			if(instance == null)
			{
				instance = System.Activator.CreateInstance(typeof(T)) as T;
				if(!instance.Init ())
				{
					// 初始化失败，返回 null
					Debug.LogAssertion("Singleton object init failed: " + typeof(T).Name);
					instance = null;
				}
			}

			return instance;
		}
	}

	public static void Destroy(){
		if (instance != null) {
			instance = null;
		}
	}
}
