using UnityEngine;
using System.Collections;

/// <summary>
/// Interface game referee.
/// 游戏裁判
/// 处理游戏规则
/// </summary>
public interface IGameReferee  {
	GameState CurrentGameState{
		get;
	}

	Chessboard CurrentChessBoard { get;}

	bool IsValidMove (GamePlayOperation op);
	bool PlayMove(GamePlayOperation op);
//	void EndMove ();

	void EndPlayerTurn (IGamePlayer currentPlayer);
}
