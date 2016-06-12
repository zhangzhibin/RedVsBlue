using UnityEngine;
using System.Collections;

public class ShakeAction : MonoBehaviour {
	
//	void Awake()
//	{
//		
//	}
//
	// Use this for initialization
	void Start () {
//		Play (1, 1, 1, 1, 0.1f);		
	}
	
	// Update is called once per frame
	void Update () {
		updateState (Time.deltaTime);
	}

	Vector3 _deltaSize = Vector3.zero;
	float _actionTime = 0;
	float _recoverTime = 0;

	Vector3 _currentDelta = Vector3.zero;
	float _playTime = 0;
	Vector3 _recoverSpeed = Vector3.zero;

	bool _isBegin = true;
	int _repeat = 1;
	bool _destroyOnDone = true;
	public void Play(float xDelta, float yDelta, float zDelta, float actionTime, float recoverTime, int repeat = 1, bool destroyOnDone=true)
	{
		_deltaSize.x = xDelta;
		_deltaSize.y = yDelta;
		_deltaSize.z = zDelta;

		_actionTime = actionTime;
		_recoverTime = recoverTime;

		_repeat = repeat;
		if(_repeat<1)
		{
			_repeat = 1;
		}

		_destroyOnDone = destroyOnDone;
		beginAction ();
	}

	public void Stop()
	{
		_isBegin = false;
	}

	protected virtual void updateState(float deltaTime)
	{
		if(!_isBegin || deltaTime<=0f)
		{
			return;
		}

		float shakeTime = 0;
		float recoverTime = 0;
		bool stopAfterAction = false;

		if(_playTime < _actionTime)
		{
			if(_playTime + deltaTime <= _actionTime)
			{
				shakeTime = deltaTime;
			}
			else
			{
				shakeTime = _actionTime - _playTime;
				recoverTime = deltaTime - shakeTime;
			}
		}
		else
		{
			if(_playTime + deltaTime <= _actionTime + _recoverTime)
			{
				recoverTime = deltaTime;
			}
			else
			{
				recoverTime = _actionTime + _recoverTime - _playTime;

				// stop
				stopAfterAction = true;
			}
		}

		if(shakeTime>0)
		{
			shakeAction (shakeTime);
		}

		if(recoverTime>0)
		{
			recoverAction (recoverTime);
		}

		_playTime += deltaTime;

		if(stopAfterAction)
		{
			_repeat--;
			if(_repeat <= 0)
			{
				endAction (_destroyOnDone);
			}
			else
			{
				// start next loop
				endAction(false);
				beginAction();
			}
		}
	}

	void beginAction()
	{
		_isBegin = true;

		_playTime = 0;
		_currentDelta = Vector3.zero;

		_recoverSpeed = Vector3.zero;
	}

	void endAction(bool destroyAction)
	{
		_isBegin = false;
		_recoverSpeed = Vector3.zero;

		transform.localPosition -= _currentDelta;
		_currentDelta = Vector3.zero;

		// remove action
		if(destroyAction)
		{
			Destroy (this);
		}
	}

	// 随机抖动阶段
	void shakeAction(float deltaTime)
	{
		Vector3 shakeSpeed = new Vector3 (
		                    _deltaSize.x * (float)Random.Range (-10, 11) * 0.1f,
							_deltaSize.y * (float)Random.Range (-10, 11) * 0.1f,
							_deltaSize.z * (float)Random.Range (-10, 11) * 0.1f);

		Vector3 shake = shakeSpeed * deltaTime;
		_currentDelta += shake;

		transform.localPosition += shake;
	}

	// 匀速恢复阶段
	void recoverAction(float deltaTime)
	{
		if(_recoverSpeed == Vector3.zero)
		{
			_recoverSpeed = _currentDelta / _recoverTime;
		}	

		Vector3 recover = _recoverSpeed * deltaTime;
		_currentDelta -= recover;

		transform.localPosition -= recover;
	}

	public void StopShake(bool destroyAction)
	{
		endAction (destroyAction);
	}

	public static ShakeAction ShakeObject(GameObject target, float xDelta, float yDelta, float zDelta, 
													  float actionTime, float recoverTime, 
													  int repeat=1, bool destroyOnDone = true)
	{
		var shakeAction = target.AddComponent<ShakeAction> ();
		shakeAction.Play (xDelta, yDelta, zDelta, actionTime, recoverTime, repeat);

		return shakeAction;
	}
}
