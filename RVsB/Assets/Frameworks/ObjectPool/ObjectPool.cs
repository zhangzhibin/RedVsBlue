using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface poolable behaviour.
/// 所有可以进行添加到对象池里的对象都需要实现的接口
/// </summary>
public interface IPoolableBehaviour{
	void OnReleaseToPool();
	void ResetToDefault();
	GameObject GetGameObject();
}

public class SimplePoolableBehaviour : MonoBehaviour, IPoolableBehaviour
{
	public virtual void OnReleaseToPool()
	{
		
	}

	public virtual void ResetToDefault()
	{

	}

	public virtual GameObject GetGameObject()
	{
		return gameObject;
	}

	public virtual void ReturnToPool(float delay)
	{
		StartCoroutine (returnToPoolAfterDelay (delay));
	}

	protected IEnumerator returnToPoolAfterDelay(float delay)
	{
		yield return new WaitForSeconds (delay);

		ObjectPool.ReleaseObject (this);
	}
}

[System.Serializable]
public class PoolObjectSetting
{
	public GameObject Prefab;
	public int InitSize = 0;
	public int MaxSize = 10;

	public override string ToString ()
	{
		return string.Format ("[PoolObjectSetting] Object: [{0}], Size[{1}/{2}]", 
			Prefab.name, InitSize, MaxSize);
	}
}

public class ObjectPool : MonoBehaviour{
	[Tooltip("整个对象池最多保留多少对象")]
	public int MaxSize = 200;

	[SerializeField]
	private int _currentSize = 0;

	public PoolObjectSetting[] PoolSettings;
	private Dictionary<string, PoolObjectSetting> _poolSettings = new Dictionary<string, PoolObjectSetting>();

//	private List<PoolObjectSetting> _dynamicPoolSettings = new List<PoolObjectSetting>();
	private GameObject _dynamicPool = null;

	private Dictionary<string, LinkedList<GameObject>> _pooledObjects = new Dictionary<string, LinkedList<GameObject>>();

	public static ObjectPool Instance
	{
		get{
//			return Singleton<ObjectPool>.Instance;
			return SingletonMonoBehaviour<ObjectPool>.Instance;
		}
	}

	void Awake()
	{
		if(!SingletonMonoBehaviour<ObjectPool>.DestroyExtraObjects (gameObject))
		{
			Init ();
		}
	}

	// 初始化对象池，预创建对象
	public bool Init()
	{
		string poolKey;
		foreach(var poolSetting in PoolSettings)
		{
			Debug.Log (poolSetting);

			_poolSettings.Add (poolSetting.Prefab.name, poolSetting);

			if(poolSetting.Prefab!=null && poolSetting.InitSize>0)
			{
				poolKey = poolSetting.Prefab.name;

				LinkedList<GameObject> objList = getObjectList (poolKey);

				Debug.Assert (objList != null);

				int initCount = poolSetting.InitSize - objList.Count;

				for(int i=0;i<initCount;i++)
				{
					var obj = Instantiate (poolSetting.Prefab) as GameObject;
					obj.name = poolKey; // 去除 clone 后缀

					obj.transform.parent = transform;
					obj.SetActive (false);

					objList.AddLast (obj);

					_currentSize++;
				}
			}
		}
		return true;
	}

	// 清楚对象池
	public void Clear(bool clearAll)
	{
		foreach(var objectListPair in _pooledObjects)
		{
			if(objectListPair.Value!=null)
			{
				// FIXME: 只删掉多余的
				var settings = getPoolObjectSettingByName(objectListPair.Key);
				if(settings == null || clearAll)
				{
					objectListPair.Value.Clear ();
				}
				else
				{
					while(objectListPair.Value.Count > settings.InitSize)
					{
						objectListPair.Value.RemoveFirst ();
					}
				}
			}
		}
	}

	// 根据对象类型名（Prefab名）获取对象列表，如果不存在，则创建空列表
	private LinkedList<GameObject> getObjectList(string name)
	{
		LinkedList<GameObject> objList = null;

		string poolKey = name;

		if(!_pooledObjects.ContainsKey(poolKey))
		{
			_pooledObjects.Add (poolKey, new LinkedList<GameObject> ());
		}

		_pooledObjects.TryGetValue (poolKey, out objList);

		return objList;
	}

	// 获取类型的对象池设定
	private PoolObjectSetting getPoolObjectSettingByName(string name)
	{
		PoolObjectSetting setting = null;
//		if(PoolSettings!=null && PoolSettings.Length>0)
//		{
//			foreach(var ps in PoolSettings)
//			{
//				if(ps.Prefab.name == name)
//				{
//					setting = ps;
//					break;
//				}
//			}
//		}
//
//		if(setting==null)
//		{
//			foreach(var ps in _dynamicPoolSettings)
//			{
//				if(ps.Prefab.name == name)
//				{
//					setting = ps;
//					break;
//				}
//			}
//		}

		_poolSettings.TryGetValue(name, out setting);
		return setting;
	}

	private PoolObjectSetting addPoolObjectSetting(GameObject objType)
	{
		if(_dynamicPool == null)
		{
			_dynamicPool = new GameObject ("DynamicPool");
			_dynamicPool.transform.parent = transform;
			_dynamicPool.transform.localPosition = Vector3.zero;
		}

		var templateObject = Instantiate (objType);
		templateObject.name = objType.name;
		templateObject.SetActive (false);
		templateObject.transform.parent = _dynamicPool.transform;

		PoolObjectSetting setting = new PoolObjectSetting();
		setting.Prefab = templateObject;
		setting.MaxSize = 0;
		setting.InitSize = 0;

//		_dynamicPoolSettings.Add (setting);
		_poolSettings.Add(objType.name, setting);

		return setting;
	}

	// 根据Prefab类型获取对应的对象
	// 如果该对象有对象池设定，则从对象池中获取
	// 否则，直接创建一个新对象
	public GameObject GetPooledObject(GameObject objType, bool addToPool)
	{
		PoolObjectSetting setting = getPoolObjectSettingByName (objType.name);
		if(setting==null)
		{
			if(!addToPool)
			{
				return Instantiate (objType) as GameObject;
			}
			else
			{
				// 动态添加该类型到对象池
				setting = addPoolObjectSetting (objType);
			}
		}

		string poolKey = objType.name;

		GameObject obj = null;

		LinkedList<GameObject> objList = getObjectList (poolKey);

		Debug.AssertFormat (objList != null, "!![ObjectPool] null object list for type: {0}", poolKey);

		if(objList.Count>0)
		{
			do {
				obj = objList.Last.Value;
				objList.RemoveLast ();
				_currentSize--;

				Debug.AssertFormat (obj != null, "!![ObjectPool] null object in pool, type: {0}", poolKey);

				if (obj == null) 
				{
					Debug.LogError ("Null Object in Pool: " + poolKey);
				}

				if(objList.Count<=0)
				{
					break;
				}
			} while(obj == null);
		}

		if(obj!= null)
		{
			// 先激活再修改状态，否则Animator无法使用
			obj.SetActive (true);

			// Detaches the transform from its parent.
			obj.transform.parent = null;
		}
		else
		{
			obj = Instantiate (objType) as GameObject;
			obj.name = objType.name; // 去除 clone 后缀
		}

		return obj;
	}

	// 返回对象到对象池 （从其父节点移除，并设置active为false）
	// 若该对象没有配置对象池或者超出配额，则直接删除
	public bool ReturnToPool(GameObject obj)
	{
		if(obj==null)
		{
			return false;
		}
		obj.SetActive (false);

		// Instantiate 产生的对象，名字自带 (clone) 后缀
		PoolObjectSetting setting = getPoolObjectSettingByName (obj.name);
		if(setting==null)
		{
			Destroy(obj);
			return false;
		}

		bool pooled = false;

		string poolKey = obj.name;

		LinkedList<GameObject> objList = getObjectList (poolKey);

		Debug.AssertFormat (objList != null, "!![ObjectPool] null object list for type: {0}", poolKey);

		if(_currentSize < MaxSize && (setting.MaxSize<=0 || objList.Count<setting.MaxSize))
		{
			obj.transform.parent = gameObject.transform;

			obj.SetActive (false);

//			#if UNITY_EDITOR
//			// 编辑器测试时，检测是否有重复
//			bool exist = objList.Find(obj)!=null;
//			Debug.AssertFormat(!exist, "!![Object: {0}] already exist in pool", obj.name);
//			if(exist)
//			{
//				Debug.Log("ERROR");
//				exist = false;
//			}
//			#endif

			objList.AddLast(obj);

			pooled = true;

			_currentSize++;
		}
		else
		{
			Destroy(obj);

			pooled = false;
		}

		return pooled;
	}

	/// <summary>
	/// Instantiates the object.
	/// 通过对象池实例化一个对象
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="objType">Object type.</param>
	public static GameObject InstantiateObject(GameObject objType, bool addToDynamicPool = false)
	{
		GameObject obj = Instance.GetPooledObject (objType, addToDynamicPool);

		return obj;
	}

	/// <summary>
	/// Releases the object.
	/// 将不需要的对象释放，放回对象池
	/// </summary>
	/// <param name="objInstance">Object instance.</param>
//	public static void ReleaseObject(GameObject obj)
//	{
////		GameObject.Destroy (objInstance);
//		Instance.ReturnToPool(obj);
//	}
//
//	public static void ReleaseObject(MonoBehaviour scriptObject)
//	{
//		Instance.ReturnToPool (scriptObject.gameObject);
//	}

	public static void ReleaseObject(IPoolableBehaviour poolableObject)
	{
		if(poolableObject==null)
		{
			return;
		}

		poolableObject.OnReleaseToPool ();
		Instance.ReturnToPool (poolableObject.GetGameObject ());
	}
}
