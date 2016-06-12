using UnityEngine;
using System.Collections;

/// <summary>
/// Interface of game player.
/// 比赛玩家的接口
/// </summary>
/// 
public interface IGamePlayer 
{
	int PlayerId{get;set;}
	void BeginTurn (IGameReferee referee);
	void EndTurn(IGameReferee referee);

	string PlayerName{ get;}
}
