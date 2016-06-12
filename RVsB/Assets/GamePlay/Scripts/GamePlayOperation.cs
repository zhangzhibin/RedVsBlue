using UnityEngine;
using System.Collections;

public struct GamePlayOperation {
	public enum OpEnum{
		NONE,
		PLACE,		// 空白位置下子
		ACTION,		// 对当前棋子进行分裂
		TAKEN		// 消灭对方棋子
	}

	public int X;
	public int Y;
	public OpEnum Op;
	public int PlayerId;
}
