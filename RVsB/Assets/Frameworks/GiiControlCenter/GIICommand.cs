using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GII command.
/// 命令与事件类似，不同在于
/// Event（事件）主要用于数据的变化和特定事件发生
/// Command（命令）专用于操作请求
/// </summary>
//public class GIICommand{
//
//}

public delegate IGIICommand GIICommandHandler(IGIIEvent commandEvent);

public interface IGIICommandCenter
{
	// 以下仅供 IGIICommand调用
	void BeginProcessingCommand(string cmdName);
	void EndProcessingCommand (string cmdName);
}

public interface IGIICommand
{
	// 执行操作
	bool ExecuteInCommandCenter (IGIICommandCenter commandCenter);

	// 是否需要等待结束事件
	bool NeedsClosure();

//	// 检测结束事件
//	bool IsClosureEvent (string eventName);

	// 执行结束事件
	bool OnClosureEvent (string eventName, IGIICommandCenter commandCenter);

	// 操作是否结束
	bool IsFinished();

	string Name {
		get;
	}
}


public class GIISimpleCommand: IGIICommand{
	
	private FuncCallback0 _execution = null;
	private string _closure = null;
	private bool _closed = false;
	private string _name;

	public virtual string Name{
		get{
			return _name;
		}
	}

	public GIISimpleCommand(string name, FuncCallback0 fn = null, string closure = null)
	{
		_name = name;
		_execution = fn;
		_closure = closure;
		_closed = false;
	}

	public FuncCallback0 Execution
	{
		set{
			_execution = value;
		}
	}

	public string Closure
	{
		set{
			_closure = value;
		}
	}

	public virtual bool ExecuteInCommandCenter (IGIICommandCenter commandCenter)
	{
		if(_execution!=null)
		{
			// TODO: GIIControlCenter作为外部参数传入更合适
			commandCenter.BeginProcessingCommand (Name);

			#if UNITY_EDITOR
			Debug.LogFormat("[{0}] => Execute", _name);
			#endif

			_execution ();
			commandCenter.EndProcessingCommand (Name);
			// 只执行一次
			_execution = null;
		}

		if(NeedsClosure())
		{
			_closed = false;

			#if UNITY_EDITOR
			Debug.LogFormat("[{0}] => Waiting for closure: {1}", _name, _closure);
			#endif
		}
		else
		{
			_closed = true;

			#if UNITY_EDITOR
			Debug.LogFormat("[{0}] => Finish", Name, _closure);
			#endif
		}

		return _closed;
	}

	// 是否需要等待结束事件
	public virtual bool NeedsClosure()
	{
		return (_closure != null);
	}

	private bool isClosureEvent(string eventName)
	{
		if(_closure == null || _closure.Length<=0)
		{
			return true;
		}

		return (_closure == eventName);
	}

	// 执行结束事件
	public virtual bool OnClosureEvent (string eventName, IGIICommandCenter commandCenter)
	{
		if(isClosureEvent(eventName))
		{
			#if UNITY_EDITOR
			Debug.LogFormat("[{0}] => OnClosureEvent, finish", Name);
			#endif

			_closed = true;
		}

		return _closed;
	}

	// 操作是否结束
	public virtual bool IsFinished()
	{
		return _closed;
	}
}
