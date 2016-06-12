using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSUI : MonoBehaviour {
	private float frameCounter = 0;
	private float frameTime = 0;

	public Text FPSText;

	public  float updateInterval = 0.5F;

//	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
//	private float timeleft; // Left time for current interval

	private float frameStartTime;

	public static FPSUI Instance{
		get{
			return SingletonMonoBehaviour<FPSUI>.Instance;
		}
	}

	void Awake()
	{
		if(!SingletonMonoBehaviour<FPSUI>.DestroyExtraObjects (this))
		{
			frameCounter = 0;
		}
	}

	void Start()
	{
		if( !FPSText )
		{
			Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
			enabled = false;
			return;
		}
//		timeleft = updateInterval;  
	}

	// Update is called once per frame
	void Update () {
		updateFPS ();
	}

	void updateFPS2()
	{
		frameCounter++;

		if(frameCounter>0 && frameCounter %50==0)
		{
			float fps = 0;
			var deltaTime = Time.time - frameTime;

			if(deltaTime<=0)
			{
				fps = 0;
			}
			else
			{
				fps = frameCounter / (Time.time - frameTime);	
			}

			frameCounter = 0;

			setFPSValue (fps);
		}

		if(frameCounter==0)
		{
			frameTime = Time.time;
		}
	}

	void updateFPS()
	{
//		timeleft -= Time.deltaTime;
//		accum += Time.timeScale/Time.deltaTime;
		if(frames == 0)
		{
			frameStartTime = Time.realtimeSinceStartup;
		}

		++frames;

		float passedTime = Time.realtimeSinceStartup - frameStartTime;

		if(passedTime>=updateInterval)
		{
			float fps = (float)frames / passedTime;	
			setFPSValue (fps);

			frames = 0;
		}

//		// Interval ended - update GUI text and start new interval
//		if( timeleft <= 0.0 )
//		{
//			// display two fractional digits (f2 format)
//			float fps = accum/frames;
//
//			setFPSValue (fps);
//
//			//	DebugConsole.Log(format,level);
//			timeleft = updateInterval;
//			accum = 0.0F;
//			frames = 0;
//		}
	}

	void setFPSValue(float fps)
	{
		FPSText.text = string.Format ("FPS:{0:F1}", fps);
		if(fps>=50f)
		{
			FPSText.color = Color.green;
		}
		else if(fps>=40f)
		{
			FPSText.color = Color.yellow;
		}
		else
		{
			FPSText.color = Color.red;
		}
	}
}
