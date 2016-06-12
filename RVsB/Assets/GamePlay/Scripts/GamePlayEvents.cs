using UnityEngine;
using System.Collections;

public class GamePlayEvents{
	// 准备对局
	public const string PrepareGame = "Event.GamePlay.Prepare";
	// 开始对局
	public const string StartGame = "Event.GamePlay.Start";
	// 结束对局
	public const string FinishGame = "Event.GamePlay.Finish";

	// 角色加入游戏
	public const string PlayerJoin = "Event.GamePlay.PlayerJoin";

	// 角色回合开始
	public const string PlayerTurnBegin = "Event.GamePlay.PlayerTurn.Begin";
	// 角色移动指令
	public const string PlayerMove = "Event.GamePlay.PlayerMove";
	// 角色回合结束
	public const string PlayerTurnEnd = "Event.GamePlay.PlayerTrun.End";
}
