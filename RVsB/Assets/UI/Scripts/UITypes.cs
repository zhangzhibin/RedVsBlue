using UnityEngine;
using System.Collections;

public enum UITypeEnum
{
	UI_UNKNOWN,
	UI_HOME,
	UI_GAMEPLAY,
	UI_GAMEPAUSE,
	UI_RETRY,
	UI_RESULT
}

public enum UIEventEnum
{
	NONE,
	SHOW,
	SHOW_READY,
	HIDE,
	HIDE_READY
}

public static class UIEvents{
	public static string Event(UITypeEnum UIType, UIEventEnum state)
	{
		return string.Format ("UIEvents.{0}.{1}", UIType, state);
	}
}
