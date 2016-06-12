using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game unit.
/// 
/// </summary>
public class GameUnit{
	public const int OWNER_NONE = 0;
	public const int VALUE_DEFAULT = 0;

	public int X {
		get;
		set;
	}

	public int Y {
		get;
		set;
	}

	public int OwnerId {
		get;
		set;
	}

	public int Value {
		get;
		set;
	}

	public GameUnit()
	{
		Reset ();
	}

	public void Reset()
	{
		OwnerId = OWNER_NONE; // 中立
		Value = 0;
		X = -1;
		Y = -1;
	}
}
/// <summary>
/// Game state.
/// 对局数据
/// </summary>
public class GameState {
	private GameUnit[,] _units;
	private int _width = 0;
	private int _height = 0;

	public GameState(int width, int height)
	{
		_width = width;
		_height = height;
		_units = new GameUnit[width, height];

		for(int y=0;y<height;y++)
		{
			for(int x=0;x<width;x++)
			{
				_units [x, y] = new GameUnit ();
				_units [x, y].X = x;
				_units [x, y].Y = y;
			}
		}
	}

	public int Width
	{
		get{
			return _width;
		}
	}

	public int Height
	{
		get{
			return _height;
		}
	}

	public GameUnit UnitAt(int x, int y)
	{
		Debug.Assert (x < _width && y < _height);
		var u = _units [x, y];

		if(u==null)
		{
			u = new GameUnit ();
			_units [x, y] = u;
		}
		return u;
	}

	public void Reset()
	{
		for(int y=0;y<_height;y++)
		{
			for(int x=0;x<_width;x++)
			{
				_units [x, y].Reset ();
				_units [x, y].X = x;
				_units [x, y].Y = y;
			}
		}
	}
		
	public IList<GamePlayOperation> ApplyMove(IList<GamePlayOperation> moves)
	{
		// 由该次移动引发的效果
		List<GamePlayOperation> consequencialMoves = new List<GamePlayOperation>();

		for(int i=0;i<moves.Count;i++)
		{
			var op = moves [i];
			var unit = UnitAt (op.X, op.Y);
			switch(op.Op)
			{
			case GamePlayOperation.OpEnum.PLACE:
				unit.OwnerId = op.PlayerId;
				break;
			case GamePlayOperation.OpEnum.ACTION:
				break;
			case GamePlayOperation.OpEnum.TAKEN:
				unit.OwnerId = op.PlayerId;
				break;
			default:
				break;
			}

			calculateConsequencialMoves (op, consequencialMoves);
		}

		return consequencialMoves;
	}

	// 黑白棋算法
	private void calculateConsequencialMoves(GamePlayOperation currentMove, IList<GamePlayOperation> consequencialMoves)
	{
		
		// 上下左右各计算一次
		calculateConsequencialMovesOnDirection(currentMove, 0, consequencialMoves);
		calculateConsequencialMovesOnDirection(currentMove, 1, consequencialMoves);
		calculateConsequencialMovesOnDirection(currentMove, 2, consequencialMoves);
		calculateConsequencialMovesOnDirection(currentMove, 3, consequencialMoves);
	}

	// direction: 上下左右 0123
	private void calculateConsequencialMovesOnDirection(GamePlayOperation currentMove, int direction, IList<GamePlayOperation> consequencialMoves)
	{
		Vector2 step = Vector2.zero;
		switch(direction)
		{
		case 0:
			step.y = 1;
			break;
		case 1:
			step.y = -1;
			break;
		case 2:
			step.x = -1;
			break;
		case 3:
			step.x = 1;
			break;
		default:
			Debug.Assert (true, "Invalid direction: " + direction);
			break;
		}

		Vector2 currentPos = new Vector2 (currentMove.X, currentMove.Y);
		List<GameUnit> opponents = new List<GameUnit>();
		bool hasConsequencials = false;

		while(true)
		{
			currentPos += step;
			if(!isValidGrid((int)currentPos.x, (int)currentPos.y))
			{
				// 超出棋盘范围
				break;
			}

			var u = UnitAt ((int)currentPos.x, (int)currentPos.y);
			if(u.OwnerId == GameUnit.OWNER_NONE)
			{
				// 空白区域，表示没有连成线
				break;
			}

			if(u.OwnerId == currentMove.PlayerId)
			{
				// 还是自己的棋子
				if(opponents.Count>0)
				{
					// 夹击对方棋子
					hasConsequencials = true;
				}
				break;
			}

			// 对方的棋子
			opponents.Add(u);
		}

		if(hasConsequencials)
		{
			for(int i=0;i<opponents.Count;i++)
			{
				GamePlayOperation move = new GamePlayOperation ();
				move.X = opponents [i].X;
				move.Y = opponents [i].Y;
				move.Op = GamePlayOperation.OpEnum.TAKEN;
				move.PlayerId = currentMove.PlayerId;
				consequencialMoves.Add (move);
			}
		}
	}

	bool isValidGrid(int x, int y)
	{
		bool invalid = x < 0 || x >= Width || y < 0 || y >= Height;

		return !invalid;
	}

	/// <summary>
	/// Shoulds the finish.
	/// 对局是否结束
	/// </summary>
	/// <returns><c>true</c>, if finish was shoulded, <c>false</c> otherwise.</returns>
	public bool ShouldFinish()
	{
		bool hasEmptyPosition = false;
		// 只要还有空位就不用结束
		for(int y=0;y<Height;y++)
		{
			for(int x=0;x<Width;x++)
			{
				if(UnitAt(x,y).OwnerId == GameUnit.OWNER_NONE)
				{
					hasEmptyPosition = true;
					break;
				}
			}
			if(hasEmptyPosition)
			{
				break;
			}
		}

		return !hasEmptyPosition;
	}

	public int Score(int playerId)
	{
		int score = 0;
		for(int y=0;y<Height;y++)
		{
			for(int x=0;x<Width;x++)
			{
				if(UnitAt(x,y).OwnerId == playerId)
				{
					score++;
				}
			}
		}

		return score;
	}
}
