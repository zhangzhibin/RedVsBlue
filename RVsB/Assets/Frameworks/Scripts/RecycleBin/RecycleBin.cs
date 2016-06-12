using UnityEngine;
using System.Collections;

/// <summary>
/// Recycle bin.
/// 临时对象回收站，将Scene里在运行不需要用到的对象扔到这里，启动时将自动删除
/// </summary>
public class RecycleBin : MonoBehaviour {
	public GameObject[] TempObjects;

	// Use this for initialization
	void Start () {
		clean ();
	}

	void clean()
	{
		if(TempObjects!=null && TempObjects.Length>0)
		{
			foreach(var tempObj in TempObjects)
			{
				DestroyImmediate (tempObj);
			}
		}

		Destroy (gameObject);
	}
}
