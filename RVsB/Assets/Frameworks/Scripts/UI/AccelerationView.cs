using UnityEngine;
using System.Collections;

public class AccelerationView : MonoBehaviour  
{  
//
//	// Use this for initialization  
//	void Start()  
//	{  
//
//	}  
//
//	// Update is called once per frame  
//	void Update()  
//	{  
//	}  

	void OnGUI()  
	{  
		GUI.Box(new Rect(5, 5, 100, 20), string.Format("x:{0:0.000}", Input.acceleration.x));  
		GUI.Box(new Rect(5, 30, 100, 20), string.Format("y:{0:0.000}", Input.acceleration.y));  
		GUI.Box(new Rect(5, 55, 100, 20), string.Format("z:{0:0.000}", Input.acceleration.z)); 

		Vector3 normal = Vector3.down;
		var rotation = Quaternion.FromToRotation (normal, Input.acceleration).eulerAngles;

		GUI.Box(new Rect(5, 80, 100, 20), string.Format("RX:{0:0.0}", rotation.x)); 
		GUI.Box(new Rect(5, 105, 100, 20), string.Format("RY:{0:0.0}", rotation.y)); 
		GUI.Box(new Rect(5, 130, 100, 20), string.Format("RZ:{0:0.0}", rotation.z)); 
	}  
}  