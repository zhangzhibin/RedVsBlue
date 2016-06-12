using UnityEngine;
using System.Collections;

public enum RENDER_QUEUE{
	DEFAULT = 0,
	BACKGROUND = 1000,
	GEOMETRY = 2000,
	ALPHA_TEST = 2450,
	TRANSPARENT = 3000,
	OVERLAY = 4000
}

public class ManualRendererQueue : MonoBehaviour
{
	public RENDER_QUEUE _RenderQueue = RENDER_QUEUE.DEFAULT;
	public int _RenderQueueOffset=0;
	// Use this for initialization
	void Start () {
		if(_RenderQueue != RENDER_QUEUE.DEFAULT)
		{
			SetRenderer();
		}        
    }

    public void SetRenderer()
    {
        Renderer AllRenderers = GetComponent<Renderer>();
		AllRenderers.sharedMaterial.renderQueue = (int)_RenderQueue + _RenderQueueOffset;
//		foreach (Material m in AllRenderers.sharedMaterial) 
//		{
//			m.renderQueue = (int)_RenderQueue + _RenderQueueOffset;
//		}
    }
}
