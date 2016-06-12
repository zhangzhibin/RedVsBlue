using UnityEngine;
using System.Collections;

[System.Serializable]
public struct RenderQueueConfig
{
	public RENDER_QUEUE _RenderQueue;
	public int _RenderQueueOffset;

	public int GetRenderQueue()
	{
		return (int)_RenderQueue + _RenderQueueOffset;
	}
}

[System.Serializable]
public class MaterialConfig
{
	public Material _Materail;
	public RenderQueueConfig _RenderQueueConfig;
}

public class MaterialsManager : MonoBehaviour {
	public MaterialConfig[] _MaterialConfigs;
	void Awake()
	{
		updateMaterails ();
	}

	void updateMaterails()
	{
		if(_MaterialConfigs==null)
		{
			return;
		}

		foreach(var mc in _MaterialConfigs)
		{
			if(mc._RenderQueueConfig._RenderQueue!=RENDER_QUEUE.DEFAULT)
			{
				mc._Materail.renderQueue = (int)mc._RenderQueueConfig.GetRenderQueue ();
			}
		}
	}
}
