using UnityEngine;
using System.Collections;

public interface IGIIEvent
{
	string Name {
		get;
	}

	object Body {
		get;
		set;
	}
}

/// <summary>
/// GII event data.
/// 消息的数据主体
/// </summary>
public class GIIEvent : IGIIEvent{
	private string _name;
	public virtual string Name{
		get{
			return _name;
		}
		set{
			_name = value;
		}
	}

	private object _body;
	public virtual object Body{
		get{
			return _body;
		}
		set{
			_body = value;
		}
	}

	public GIIEvent(string name, object body)
	{
		_name = name;
		_body = body;
	}

	public GIIEvent(string name)
		:this(name, null)
	{
		
	}

	public static GIIEvent TempEvent(object body)
	{
		return new GIIEvent ("#temp#", body);
	}
}

