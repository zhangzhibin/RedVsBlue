using UnityEngine;
using System.Collections;

public class TouchView : MonoBehaviour  
{  
	public Vector2 _Origin;
	public float _Height = 25;
	public float _Width = 200;

	void OnGUI()  
	{  
		var touches = Input.touches;
		int i = 0;
		#if UNITY_EDITOR
		GUI.Box(new Rect(_Origin.x, _Origin.y + (_Height+2) * i, _Width, _Height),
			string.Format("[{4}] #{0} [{1:0},{2:0}] [t:{3:0}ms] ", 
				i, 100, 200, Time.deltaTime * 1000f, 1));
		#endif
//
//		GUI.Box(new Rect(_Origin.x, _Origin.y + (_Height+2) * 1, _Width, _Height),
//			string.Format("#{0} [{1:0.0},{2:0.0}] {3}]", 
//				i, 100, 100, TouchPhase.Stationary));
		
		foreach(var t in touches)
		{
			GUI.Box(new Rect(_Origin.x, _Origin.y + (_Height+2) * i, _Width, _Height),
				string.Format("[{4}] #{0} [{1:0.0},{2:0.0}] [t:{3:F.0}] ", 
					i, t.position.x, t.position.y, t.deltaTime, t.fingerId));
			i++;
		}
	}
}