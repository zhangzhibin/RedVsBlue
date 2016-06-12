using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GII 全局控制中心
/// 管理 1. 消息（Event）
/// 2. 操作请求（Command）
/// </summary>
public class GIIControlCenter: IGIICommandCenter {
	private readonly object _eventObserversLock = new object();
	private readonly object _eventObserversOpLock = new object();
	private Dictionary<string, List<IGIIEventObserver>> _eventObservers = null;

	private readonly object _commandLock = new object();
	private Dictionary<string, GIICommandHandler> _allCommands = null;

	private int _inProcessingEvents = 0; 	// 处理事件中
	private int _inProcessingCommands = 0; 	// 处理操作中

	private struct EventObserverPair
	{
		public string _EventName;
		public IGIIEventObserver _Observer;

		public EventObserverPair(string e, IGIIEventObserver o)
		{
			_EventName = e;
			_Observer = o;
		}
	}

	private Queue<EventObserverPair> _unregisterRequests = new Queue<EventObserverPair> ();
	private Queue<EventObserverPair> _registerRequests = new Queue<EventObserverPair> ();

	private Queue<GIIEvent> _pendingEvents = null;

	private struct EventPair
	{
		public int _CommandLevel;
		public GIIEvent _Event;

		public EventPair(GIIEvent e, int l)
		{
			_Event = e;
			_CommandLevel = l;
		}
	}

	private List<Queue<GIIEvent>> _pendingCommandEvents = new List<Queue<GIIEvent>>();

//	// Command 的结束标记
//	private Stack<string> _commandClosure = new Stack<string> ();
//	// Command 队列
//	private Queue<GIIEvent> _pendingCommands = new Queue<GIIEvent>();

	// 当前执行中的命令
//	private IGIICommand _currentCommand = null;

	// 未执行完的命令栈（后执行的命令在上方）
	private Stack<IGIICommand> _pendingCommands = new Stack<IGIICommand>();

	private static GIIControlCenter _instance = null;
	protected static readonly object _staticSyncRoot = new object();
	public static GIIControlCenter Instance{
		get{
			lock(_staticSyncRoot)
			{
				if (_instance == null) {
					_instance = new GIIControlCenter ();
					_instance.Init ();
				}
			}

			return _instance;
		}
		private set{ }
	}

	public GIIControlCenter()
	{
		if (_eventObservers == null) {
			_eventObservers = new Dictionary<string, List<IGIIEventObserver>> ();
		}

		if(_allCommands == null)
		{
			_allCommands = new Dictionary<string, GIICommandHandler> ();
		}
	}

	public void Init(){
//		lock(_eventObserversLock)
//		{
//
//		}
	}

	List<IGIIEventObserver> getEventObserverList(string eventName){
		List<IGIIEventObserver> observers = null;
		lock (_eventObserversLock) 
		{
			if (_eventObservers.ContainsKey (eventName)) {
				observers = _eventObservers [eventName];
			} else {
				observers = new List<IGIIEventObserver> ();
				_eventObservers.Add (eventName, observers);
			}
		}
		return observers;
	}

	public void RegisterEventObserver(string eventName, IGIIEventObserver observer){
		lock(_eventObserversOpLock)
		{
			if(_inProcessingEvents<=0)
			{
				// 没有处理中的消息，则立即处理
				var observerList = getEventObserverList (eventName);
				if(!observerList.Contains(observer))
				{
					observerList.Add (observer);
				}
			}
			else
			{
				_registerRequests.Enqueue (new EventObserverPair (eventName, observer));
			}
		}
	}

	public void UnregisterEventObserver(string eventName, IGIIEventObserver observer){
		lock (_eventObserversOpLock) 
		{
			if(_inProcessingEvents<=0)
			{
				// 没有处理中的消息，则立即处理
				var observerList = getEventObserverList (eventName);
				observerList.Remove (observer);
			}
			else
			{
				_unregisterRequests.Enqueue (new EventObserverPair (eventName, observer));
			}
		}
	}

	public void RegisterEventObserverEx(IGIIEventObserverEx observer)
	{
		var events = observer.InterestedEvents;
		foreach(var e in events)
		{
			if(e == null || e.Length<=0)
			{
				continue;
			}
			RegisterEventObserver (e, observer);
		}
	}

	public void UnregisterEventObserverEx(IGIIEventObserverEx observer)
	{
		var events = observer.InterestedEvents;
		foreach(var e in events)
		{
			if(e == null || e.Length<=0)
			{
				continue;
			}

			UnregisterEventObserver (e, observer);
		}
	}
		
	private void processEvent(string eventName, object eventBody, int cmdLevel)
	{
		lock (_eventObserversOpLock)
		{
			if(!_eventObservers.ContainsKey(eventName))
			{
				// no observers
				return;
			}

			// 由当前Command或者上级Command触发的消息，延迟到当前Command结束后处理
			if(_inProcessingCommands>0 && _inProcessingCommands >= cmdLevel)
			{
				addEventToQueue (new GIIEvent (eventName, eventBody), _inProcessingCommands);
				return;
			}

			// 正式处理该事件
			_inProcessingEvents ++;

//			#if UNITY_EDITOR
//			Debug.LogFormat("[{0}] => Fired, @{1}", eventName, cmdLevel);
//			#endif

			bool passedDown = false;
			var observerList = getEventObserverList (eventName);
			foreach(var observer in observerList)
			{
				passedDown = observer.OnEvent (new GIIEvent(eventName, eventBody));
				if(!passedDown)
				{
					break;
				}
			}

			_inProcessingEvents--;

			// 由该消息产生的注销/注册请求也延迟处理
			processPendingRequests();

			// 处理完的事件可能触发：1. 结束当前指令，2. 执行新指令
			runPendingCommandsOnEvent (eventName);

			sendPendingEvents (cmdLevel);
		}
	}

	public void FireEvent(string eventName, object eventBody)
	{
		processEvent (eventName, eventBody, _inProcessingCommands);
	}

	public void FireEvent(string eventName)
	{
		FireEvent (eventName, null);
	}

	// command center
	public void ExecuteCommand(string commandName, object commandBody)
	{
		ExecuteCommand (new GIIEvent (commandName, commandBody));
	}

	public void ExecuteCommand(string commandName)
	{
		ExecuteCommand (commandName, null);
	}

	// 立即执行指定命令
	public void ExecuteCommand(GIIEvent command)
	{
		lock(_commandLock)
		{
			if(_allCommands.ContainsKey(command.Name))
			{
				var handler = _allCommands [command.Name];
				if(handler!=null)
				{
					var cmd = handler (command);


					if(!cmd.NeedsClosure())
					{
//						// 立即执行，并且结束 ！！（所以对于 MacroCommand，如果有一个操作需要等待，则整个操作都不立即执行）
//						cmd.ExecuteInCommandCenter(this);
//
//						// 处理由该Command产生的消息
//						sendPendingEvents (_inProcessingCommands + 1);

						processCommand(cmd);
					}
					else
					{
						// 延后执行
						_pendingCommands.Push (cmd);
					}
				}
			}
			else
			{
				// not registered
				Debug.LogWarningFormat("Command [{0}] has no handler", command.Name);
			}

			runPendingCommands ();
		}
	}

	void processCommand(IGIICommand cmd)
	{
		// 立即执行，并且结束 ！！（所以对于 MacroCommand，如果有一个操作需要等待，则整个操作都不立即执行）
		cmd.ExecuteInCommandCenter(this);

		if(!cmd.IsFinished())
		{
			_pendingCommands.Push (cmd);
		}

		// 处理由该Command产生的消息
		sendPendingEvents (_inProcessingCommands + 1);
	}

	// 以下仅供 IGIICommand调用
	public void BeginProcessingCommand(string cmdName)
	{
		_inProcessingCommands++;

		#if UNITY_EDITOR
		Debug.LogFormat("[BeginProcessingCommand][{1}] => {0}", _inProcessingCommands, cmdName);
		#endif
	}

	public void EndProcessingCommand(string cmdName)
	{
		_inProcessingCommands--;

		#if UNITY_EDITOR
		Debug.LogFormat("[EndProcessingCommand][{1}] => {0}", _inProcessingCommands, cmdName);
		#endif
	}

//	// 立即执行指定命令，并等待结束消息
//	public void ExecuteCommandWithClosure(GIIEvent command, string closure)
//	{
//		
//	}
//
//	public void ExecuteCommandWait(GIIEvent command)
//	{
//		
//	}

	public void RegisterCommand(string commandName, GIICommandHandler handler)
	{
		lock(_commandLock)
		{
			if(_allCommands.ContainsKey(commandName))
			{
				Debug.LogWarningFormat("Command [{0}] handler changed", commandName);
			}
			_allCommands[commandName] = handler;
		}
	}

	public void UnregisterCommand(string commandName)
	{
		lock(_commandLock)
		{
			#if UNITY_EDITOR	
			if(!_allCommands.ContainsKey(commandName))
			{
				Debug.LogWarningFormat("Command [{0}] has no handler to remove", commandName);
			}
			#endif

			_allCommands.Remove (commandName);
		}
	}

	void processPendingRequests()
	{
		if(_inProcessingEvents>0)
		{
			return;
		}

		while(_unregisterRequests.Count>0)
		{
			var r = _unregisterRequests.Dequeue ();
			UnregisterEventObserver (r._EventName, r._Observer);
		}

		while(_registerRequests.Count>0)
		{
			var r = _registerRequests.Dequeue ();
			RegisterEventObserver (r._EventName, r._Observer);
		}
	}

	void sendPendingEvents(int cmdLevel)
	{
		Queue<GIIEvent> q = null;
		for(int i = _pendingCommandEvents.Count-1;i>=cmdLevel;i--)
		{
			q = _pendingCommandEvents [i];
			while(q.Count>0)
			{
				var e = q.Dequeue ();
				processEvent(e.Name, e.Body, i);
			}
		}
	}

	void runPendingCommands()
	{
		if (_pendingCommands.Count <= 0) 
		{
			return;
		}

		var cmd = _pendingCommands.Pop ();
		processCommand(cmd);

		while(cmd.IsFinished() && _pendingCommands.Count>0)
		{
			cmd = _pendingCommands.Pop ();
			cmd.ExecuteInCommandCenter (this);
		}

		if(cmd!=null && !cmd.IsFinished())
		{
			// 没结束的命令放回等待队列
			_pendingCommands.Push (cmd);
		}
	}

	void runPendingCommandsOnEvent(string closureEvent)
	{
		if(_pendingCommands.Count<=0)
		{
			return;
		}

		var cmd = _pendingCommands.Pop ();
		if(cmd.OnClosureEvent(closureEvent, this)) // ???
		{
			// 结束最后一个指令
			cmd = null;

			// 执行剩余指令
			runPendingCommands ();
		}
		else
		{
			// 不是最后一个指令期待的结束符，或者指令仍未结束
			_pendingCommands.Push(cmd);
		}
	}

	void addEventToQueue(GIIEvent e, int cmdLevel)
	{
		Queue<GIIEvent> q = null;
		if(_pendingCommandEvents.Count < cmdLevel+1)
		{
			// 扩容
			for(int i=_pendingCommandEvents.Count;i<cmdLevel+1;i++)
			{
				_pendingCommandEvents.Add (new Queue<GIIEvent> ());
			}
		}

		q = _pendingCommandEvents [cmdLevel];
		q.Enqueue (e);
	}
}
