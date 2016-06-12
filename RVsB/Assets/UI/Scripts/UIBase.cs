using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBase : MonoBehaviour, IGIIEventObserverEx {
	public GameObject _Canvas = null;
	public Animator _Animator = null;

	public UITypeEnum _UIType = UITypeEnum.UI_UNKNOWN;

	// !! Animator里定义的State名必须与以下一致 !!
	public const string SHOW_STATE = "Show";
	public const string HIDE_STATE = "Hide";
	public const string SHOW_READY_STATE = "ShowReady";
	public const string HIDE_READY_STATE = "HideReady";

	private string _showReadyEvent = null;
	private string _hideReadyEvent = null;

	private FuncCallback0 _hideReadyCallback = null;
	private FuncCallback0 _showReadyCallback = null;

	public virtual void Awake()
	{
		BindUIComponents ();
	}

	public virtual void Show(bool playAnimation=true, FuncCallback0 onReadyCallback = null)
	{
		_showReadyCallback = onReadyCallback;
		if(_Canvas!=null)
		{
			_Canvas.SetActive (true);
		}
		gameObject.SetActive (true);

		UpdateData();

		GIIControlCenter.Instance.RegisterEventObserverEx (this); // BUG：直接显示在屏幕上的UI可能因为没有注册消息，导致无法关闭
		if (playAnimation && _Animator != null)
		{
			_Animator.Play (SHOW_STATE);
		}
		else
		{
			GIIControlCenter.Instance.FireEvent (UIEvents.Event (_UIType, UIEventEnum.SHOW_READY));
//			onShowReady ();
		}
	}

	public virtual void Hide(bool playAnimation = true, FuncCallback0 onReadyCallback = null)
	{
		_hideReadyCallback = onReadyCallback;
		if(playAnimation && _Animator!=null)
		{
			_Animator.Play (HIDE_STATE);
		}
		else
		{
			if(_UIType == UITypeEnum.UI_UNKNOWN)
			{
				onHideReady ();
			}
			else
			{
				GIIControlCenter.Instance.FireEvent (UIEvents.Event (_UIType, UIEventEnum.HIDE_READY));
			}
		}
	}

//	public virtual void OnEnable()
//	{
//		GIIControlCenter.Instance.RegisterEventObserverEx (this);
//	}
//
//	public virtual void OnDisable()
//	{
//		GIIControlCenter.Instance.UnregisterEventObserverEx (this);
//	}

	protected virtual void BindUIComponents()
	{

	}

	protected virtual void UpdateData()
	{
		
	}

	protected virtual void onShowReady()
	{
		Debug.LogFormat ("{0}:{1}", _UIType, UIEventEnum.SHOW_READY);

		if(_showReadyCallback != null)
		{
			_showReadyCallback ();
		}
	}

	protected virtual void onHideReady()
	{
		Debug.LogFormat ("{0}:{1}", _UIType, UIEventEnum.HIDE_READY);


//		if(_Animator!=null)
//		{
//			_Animator.Stop ();
//		}

		GIIControlCenter.Instance.UnregisterEventObserverEx (this);

		if(_Canvas!=null)
		{
			_Canvas.SetActive (false);
		}

		gameObject.SetActive (false);

		if(_hideReadyCallback != null)
		{
			_hideReadyCallback ();
		}
	}

	public virtual bool OnEvent(IGIIEvent giiEvent)
	{
		if(giiEvent.Name == _showReadyEvent)
		{
			onShowReady ();
		}
		else if(giiEvent.Name == _hideReadyEvent)
		{
			onHideReady ();
		}
		return true;
	}

	IList<string> _interestedEvents = null;
	protected void initEventList()
	{
		if(_interestedEvents == null)
		{
			_interestedEvents = new List<string> ();

			addEventsToList ();
		}
	}

	protected virtual void addEventsToList()
	{
		_showReadyEvent = UIEvents.Event (_UIType, UIEventEnum.SHOW_READY);
		_hideReadyEvent = UIEvents.Event (_UIType, UIEventEnum.HIDE_READY);

		addInterestedEvent (_showReadyEvent);
		addInterestedEvent (_hideReadyEvent);
	}

	protected void addInterestedEvent(string eventName)
	{
		_interestedEvents.Add (eventName);
	}

	public IList<string> InterestedEvents
	{
		get{
			if(_interestedEvents == null)
			{
				initEventList ();
			}

			return _interestedEvents;
		}
	}
}
