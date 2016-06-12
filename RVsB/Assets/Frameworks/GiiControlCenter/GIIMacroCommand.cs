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

// 组合命令，由多个Command组合成
public class GIIMacroCommand: IGIICommand{
	private Queue<IGIICommand> _subCommands = null;
	private IGIICommand _currentCommand = null;
	private string _name;
	private bool _needsClosure = false;

	public virtual string Name{
		get{
			return _name;
		}
	}

	public GIIMacroCommand(string name)
	{
		_subCommands = new Queue<IGIICommand>();
		_currentCommand = null;
		_name = name;
		_needsClosure = false;
	}

	public void AddSubCommand(IGIICommand command)
	{
		if(_currentCommand == null)
		{
			_currentCommand = command;
		}
		else
		{
			_subCommands.Enqueue (command);
		}

		if(!_needsClosure && command.NeedsClosure())
		{
			// 只要有一个子指令需要等待，则该操作需要等待
			_needsClosure = true;
		}
	}

	public virtual bool ExecuteInCommandCenter (IGIICommandCenter commandCenter)
	{
		if(_currentCommand == null)
		{
			// 出现这种情况的时候表示当前指令已经执行完
//			Debug.Assert(false, "!! Current Command is finished already !!");
			return true;
		}

		_currentCommand.ExecuteInCommandCenter (commandCenter);
		while(_currentCommand.IsFinished() && _subCommands.Count>0)
		{
			_currentCommand = _subCommands.Dequeue ();
			_currentCommand.ExecuteInCommandCenter (commandCenter);
		}

		if(_currentCommand.IsFinished())
		{
			_currentCommand = null;
			return true;
		}

		return _currentCommand.IsFinished();  // always false
	}

	// 是否需要等待结束事件
	public virtual bool NeedsClosure()
	{
//		if(_currentCommand == null)
//		{
//			// 出现这种情况的时候表示当前指令已经执行完
//			Debug.Assert(false, "!! Current Command is finished already !!");
//			return false;
//		}
//
//		return _currentCommand.NeedsClosure ();
		return _needsClosure;
	}

//	public virtual bool IsClosureEvent(string eventName)
//	{
//		if(_currentCommand == null)
//		{
//			// 出现这种情况的时候表示当前指令已经执行完
//			Debug.Assert(false, "!! Current Command is finished already !!");
//			return true;
//		}
//
//		return _currentCommand.IsClosureEvent (eventName);
//	}

	// 执行结束事件
	// 返回：当前指令是否结束
	public virtual bool OnClosureEvent (string eventName, IGIICommandCenter commandCenter)
	{
		if(_currentCommand == null)
		{
			// 出现这种情况的时候表示当前指令已经执行完
			Debug.Assert(false, "!! Current Command is finished already !!");
			return true;
		}

		if(_currentCommand.OnClosureEvent (eventName, commandCenter))
		{
			// 是当前指令期待的结束符
			_currentCommand = null;

			if(_subCommands.Count>0)
			{
				// 下一个指令
				_currentCommand = _subCommands.Dequeue ();
				ExecuteInCommandCenter (commandCenter);
			}
		}

		return IsFinished ();
	}

	// 操作是否结束
	public virtual bool IsFinished()
	{
		if(_currentCommand == null)
		{
			// 出现这种情况的时候表示当前指令已经执行完
//			Debug.Assert(false, "!! Current Command is finished already !!");
			return true;
		}

		return _currentCommand.IsFinished ();
	}
}
