using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Chessboard. 
/// 棋盘
/// </summary>
public class Chessboard : MonoBehaviour {
	// 棋盘配置
	[System.Serializable]
	public struct Config
	{
		public int Width;
		public int Height;
	}

	// 棋子定义
	public GameObject _ChessUnitPrefab;

	// 棋子的容器
	public GameObject _ChessUnitContainer;

	public Config _CurrentConfig;
	public ChessUnit[,] _checkUnits = null;

	// 棋盘的实际尺寸
	public float _BoardWidth = 10f;
	public float _BoardHeight = 10f;
	public float _Spacing = 0.1f;
	private float _unitSize = 0f;
	private Vector2 _boardOrigin; // 棋盘左下角坐标

	void Start()
	{
//		Init ();
	}

//	public void Init()
//	{
//		initBoard (_CurrentConfig);
//	}

	public void ResetBoard()
	{
		if(_checkUnits!=null)
		{
			for (int y = 0; y < _CurrentConfig.Height; y++) 
			{
				for (int x = 0; x < _CurrentConfig.Width; x++) 
				{
					var u = UnitAt (x, y);
					Destroy (u.gameObject);
					_checkUnits [x, y] = null;
				}
			}
		}
	}

	public void InitBoard(Config boardConfig)
	{
		ResetBoard ();

		_CurrentConfig = boardConfig;

		_unitSize = (_BoardWidth - _Spacing * (boardConfig.Width + 1f)) / (float)boardConfig.Width;

		_boardOrigin.x = -_BoardWidth * 0.5f;
		_boardOrigin.y = -_BoardHeight * 0.5f;

		_checkUnits = new ChessUnit[boardConfig.Width, boardConfig.Height];
		for(int y=0;y<boardConfig.Height;y++)
		{
			for(int x=0;x<boardConfig.Width;x++)
			{
				var unit = initUnit (_boardOrigin, x, y, _unitSize, _Spacing);
				_checkUnits [x, y] = unit;

				unit.transform.parent = _ChessUnitContainer.transform;
				unit.name = string.Format ("chess_({0},{1})", x, y);
			}
		}

		_ChessUnitContainer.name = string.Format ("AllChesses_({0},{1})", boardConfig.Width, boardConfig.Height);
	}

	private ChessUnit initUnit(Vector2 origin, int x, int y, float unitSize, float spacing)
	{
		var unitObject = Instantiate (_ChessUnitPrefab);

		Vector2 pos = new Vector2 ();
		pos.x = origin.x + unitSize * (x + 0.5f) + spacing * (x + 1);
		pos.y = origin.y + unitSize * (y + 0.5f) + spacing * (y + 1);

		unitObject.transform.localPosition = pos;
		unitObject.transform.localScale = Vector2.one * unitSize;
			
		ChessUnit unit = unitObject.GetComponent<ChessUnit> ();
		unit.X = x;
		unit.Y = y;
		unit.OwnerId = GameUnit.OWNER_NONE;

		return unit;
	}

	public ChessUnit UnitAt(int x, int y)
	{
		Debug.Assert (x >= 0 && x < _CurrentConfig.Width && y >= 0 && y < _CurrentConfig.Height);
		ChessUnit u = null;
		if(x >= 0 && x < _CurrentConfig.Width && y >= 0 && y < _CurrentConfig.Height)
		{
			u = _checkUnits [x, y];
		}

		return u;
	}

//	// 
//	public GamePlayOperation ConvertToOperation (Vector2 worldPosition) 
//	{
//		GamePlayOperation op = new GamePlayOperation();
////		op.op = GamePlayOperation.OpEnum.NONE;
////
//
//
//		return op;
//	}

	public void ApplyMove(IList<GamePlayOperation> moves)
	{
		if(moves!=null)
		{
			for(int i=0;i<moves.Count;i++)
			{
				var op = moves [i];
				var unit = UnitAt (op.X, op.Y);
				if(unit != null)
				{
					switch(op.Op)
					{
					case GamePlayOperation.OpEnum.PLACE:
					case GamePlayOperation.OpEnum.TAKEN:
						unit.OwnerId = op.PlayerId;
						break;
					case GamePlayOperation.OpEnum.ACTION:
						break;
					default:
						break;
					}
				}
			}
		}
	}
}
