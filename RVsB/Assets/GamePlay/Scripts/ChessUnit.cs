using UnityEngine;
using System.Collections;

// 棋子
public class ChessUnit : MonoBehaviour {
//	private int _x;
//	private int _y;
	[SerializeField]
	private SpriteRenderer _display;

	public int X {
		get;
		set;
	}

	public int Y {
		get;
		set;
	}

	private int _ownerId;
	public int OwnerId {
		get{
			return _ownerId;
		}
		set{
			if(_ownerId!=value)
			{
				changeOwner (value);
				_ownerId = value;
			}
		}
	}

	public override string ToString()
	{
		return string.Format ("ChessUnit[{2}]({0},{1})", X, Y, OwnerId);
	}

	void changeOwner(int newOwnerId)
	{
		switch(newOwnerId)
		{
		case 1:
			_display.color = Color.red;
			break;
		case 2:
			_display.color = Color.blue;
			break;
		case GameUnit.OWNER_NONE:
		default:
			_display.color = Color.white;
			break;
		}
	}
}
