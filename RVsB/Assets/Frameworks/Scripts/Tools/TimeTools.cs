using UnityEngine;
using System.Collections;

public static class TimeTools {
	public static string FormatTimeHHMMSS(float time)
	{
		int totalSeconds = (int)time;

		int hours = totalSeconds / 3600;
		int minutes = (totalSeconds % 3600)/60;
		int seconds = totalSeconds % 60;

		return string.Format ("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
	}
}
