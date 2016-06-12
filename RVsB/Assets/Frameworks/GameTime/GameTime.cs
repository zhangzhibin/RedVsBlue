using UnityEngine;
using System.Collections;

/// <summary>
/// Game time.
/// 对Time 的封装
/// </summary>
public static class GameTime {
	#region 默认 Time 接口
	public static float time
	{
		get{
			return Time.time;
		}
	}

	public static float deltaTime
	{
		get{
			return Time.deltaTime;
		}
	}

	public static float timeScale
	{
		get{
			return Time.timeScale;
		}
		set
		{
			Time.timeScale = value;
		}
	}
	#endregion

	#region 扩展接口，游戏内专用
	// 注意设置这个参数，只能对游戏逻辑的速度进行控制
	// 对于以下无效：Animation 动画， Particle System 粒子系统
	// 如果要对以上进行控制，还是要直接修改 Time.timeScale
	// THINK: 因此，感觉也不是特有用……
	public static float gameTimeScale {
		get;
		set;
	}

	public static float deltaGameTime
	{
		get{
			return deltaTime * gameTimeScale;
		}
	}
	#endregion
}
