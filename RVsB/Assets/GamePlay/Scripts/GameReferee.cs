using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game referee.
/// 对局角色：裁判
/// 责任：准备对局，控制对局进程，判定玩家的移动，判定胜负，结束对局
/// </summary>
public class GameReferee : MonoBehaviour, IGameReferee {
	[SerializeField]
	private Chessboard _board;
	public Chessboard CurrentChessBoard
	{
		get{
			return _board;
		}
	}

	#region 角色操作
	private IGamePlayer _currentPlayer = null;
	private int _currentPlayerIndex = -1;
	private List<IGamePlayer> _allPlayers = new List<IGamePlayer> ();

//	public void Start()
//	{
//		
//	}

	public IGamePlayer GetPlayer(int playerId)
	{
		IGamePlayer p = null;
		for(int i=0;i<_allPlayers.Count;i++)
		{
			if(_allPlayers[i].PlayerId == playerId)
			{
				p = _allPlayers [i];
				break;
			}
		}

		return p;
	}

	public void AddPlayer(IGamePlayer player)
	{
		player.PlayerId = _allPlayers.Count + 1;
		Debug.LogFormat ("Add Player: {0}", player.PlayerId);

		_allPlayers.Add (player);

		GIIControlCenter.Instance.FireEvent (GamePlayEvents.PlayerJoin, player);
	}

	public void BeginPlayerTurn()
	{
		var player = PickNextPlayer ();
		Debug.Assert (player != null);

		GIIControlCenter.Instance.FireEvent (GamePlayEvents.PlayerTurnBegin, player);
		player.BeginTurn (this);
	}

	public void OnPlayerTurn()
	{
		
	}

	public void EndPlayerTurn(IGamePlayer currentPlayer)
	{
		// 处理当前玩家的移动状态
		ApplyMove(_lastMoves);
		_lastMoves.Clear ();

		GIIControlCenter.Instance.FireEvent (GamePlayEvents.PlayerTurnEnd, currentPlayer);

		// 检测是否对局结束
		if(ShouldFinishGame())
		{
			FinishGame ();
		}
		else
		{
			// 不满足结束条件，下一玩家移动
//			var player = PickNextPlayer ();
//			player.BeginTurn (this);
			BeginPlayerTurn();
		}
	}

	public IGamePlayer PickNextPlayer()
	{
		if(_currentPlayerIndex<0)
		{
			_currentPlayerIndex = 0;
		}
		else
		{
			_currentPlayerIndex++;
			if(_currentPlayerIndex >= _allPlayers.Count)
			{
				_currentPlayerIndex = 0;
			}
		}

		_currentPlayer = _allPlayers [_currentPlayerIndex];

		return _currentPlayer;
	}

	public IGamePlayer CurrentPlayer
	{
		get{
			return _currentPlayer;
		}
	}
	#endregion

	#region 游戏状态（数值）
	GameState _currentGame = null;
	public GameState CurrentGameState {
		get{
			return _currentGame;
		}
	}
	#endregion

	#region 游戏全局进程
	public void PrepareGame(int width, int height)
	{
		_currentGame = new GameState (width, height);

		Chessboard.Config boardConfig = new Chessboard.Config ();
		boardConfig.Width = width;
		boardConfig.Height = height;
		_board.InitBoard (boardConfig);

		// update gameboard
		GIIControlCenter.Instance.FireEvent(GamePlayEvents.PrepareGame);
	}

	public void StartGame()
	{
		Debug.Assert (_allPlayers.Count >= 2); // 2个以上对手才能对局
		GIIControlCenter.Instance.FireEvent(GamePlayEvents.StartGame);

		BeginPlayerTurn ();
	}

	public void FinishGame()
	{
		GIIControlCenter.Instance.FireEvent (GamePlayEvents.FinishGame, CurrentGameState);
	}

	public bool ShouldFinishGame()
	{
		return CurrentGameState.ShouldFinish();
	}

	public void ClearGame()
	{
		_allPlayers.Clear ();
		_currentPlayerIndex = -1;
		_currentPlayer = null;
	}
	#endregion

	#region 角色移动
	private List<GamePlayOperation> _lastMoves = new List<GamePlayOperation>();
	public bool IsValidMove (GamePlayOperation op)
	{
		var unit = CurrentGameState.UnitAt (op.X, op.Y);
		if (unit.OwnerId == GameUnit.OWNER_NONE)
		{
			// 空白位置
			op.Op = GamePlayOperation.OpEnum.PLACE;

			return true;
		}
		else if(unit.OwnerId == op.PlayerId)
		{
			// 当前角色所拥有的棋子
		}
			
		return false;
	}

	public bool PlayMove(GamePlayOperation op)
	{
		_lastMoves.Add (op);
		return true;
	}

	public bool ApplyMove(IList<GamePlayOperation> moves)
	{
		CurrentChessBoard.ApplyMove(moves);
		var consequencialMoves = CurrentGameState.ApplyMove (moves);

		while(consequencialMoves!=null && consequencialMoves.Count>0)
		{
			CurrentChessBoard.ApplyMove(consequencialMoves);
			consequencialMoves = CurrentGameState.ApplyMove (consequencialMoves);
		}

		return true;
	}
	#endregion
}
