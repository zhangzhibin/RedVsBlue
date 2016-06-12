using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameConfigEvents{
	public const string ENABLE_SOUND = "Event.GameConfig.Enable.Sound";
	public const string ENABLE_BEST_QUALITY = "Event.GameConfig.Enable.BestQuality";
	public const string PREFERED_LANGUAGE_CHANGE = "Event.GameConfig.Prefered.Language.Change";
}

public static class GameConfigs 
{
	public const string KEY_SOUND_ON = "CONFIG.SOUND.ON";
	public const string KEY_GUIDE_DONE = "CONFIG.GUIDE.DONE";
	public const string KEY_BOOST_ALPHA = "CONFIG.BOOST.ALPHA";
	public const string KEY_PREFERED_LANGUAGE = "CONFIG.PREFERED.LANGUAGE";

	public static void Init()
	{
		EnableSound = PlayerPrefs.GetInt (KEY_SOUND_ON, 1) > 0;
		_guideDone = PlayerPrefs.GetInt (KEY_GUIDE_DONE, 0) > 0;

		initLanguage ();
	}

	public static class Tag
	{
		public const string PLAYER = "Player";
		public const string OBSTACLE = "Obstacle";
		public const string LOOT = "Loot";
		public const string BOOST = "Boost";
		public const string OBSTACLE_TRIGGER = "ObstacleTrigger";
		public const string MAZE_CELL = "MazeCell";
	}

	public static bool DebugMode {
		get;
		set;
	}

	private static bool _sound = true;
	public static bool EnableSound{
		get{
			return _sound;
		}

		set{
			if(_sound!=value)
			{
				_sound = value;
				PlayerPrefs.SetInt (KEY_SOUND_ON, _sound ? 1 : 0);
				PlayerPrefs.Save ();
			}

			AudioController.Instance.EnableSound  =_sound;
			GIIControlCenter.Instance.FireEvent (GameConfigEvents.ENABLE_SOUND, _sound);
		}
	}

	private static LanguageEnum _language = LanguageEnum.UNKNOWN;
	public static LanguageEnum Language
	{
		get{
			return _language;
		}
		set
		{
			if(_language != value)
			{
				_language = value;

				if(_language!=LanguageEnum.UNKNOWN)
				{
					PlayerPrefs.SetInt (KEY_PREFERED_LANGUAGE, (int)_language);
					PlayerPrefs.Save ();

					GIIControlCenter.Instance.FireEvent (GameConfigEvents.PREFERED_LANGUAGE_CHANGE, _language);
				}
			}	
		}
	}

	private static void initLanguage()
	{
//		PlayerPrefs.DeleteKey (KEY_PREFERED_LANGUAGE);
//		PlayerPrefs.Save ();
		LanguageEnum lan = (LanguageEnum)PlayerPrefs.GetInt (KEY_PREFERED_LANGUAGE, (int)LanguageEnum.UNKNOWN);
		if(lan == LanguageEnum.UNKNOWN)
		{
			// 未知
			if(Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
			{
				// 简体中文
				_language = LanguageEnum.CHINESE;
			}
			else
			{
				_language = LanguageEnum.ENGLISH;
			}
			GIIControlCenter.Instance.FireEvent (GameConfigEvents.PREFERED_LANGUAGE_CHANGE, _language);
		}
	}

	public static void ResetLanguage()
	{
		_language = LanguageEnum.UNKNOWN;
		PlayerPrefs.DeleteKey (KEY_PREFERED_LANGUAGE);
		PlayerPrefs.Save ();

		initLanguage ();
	}

	private static bool _guideDone = false;
	public static bool GuideDone {
		get{
			return _guideDone;
		}
		set{
			_guideDone = value;

			PlayerPrefs.SetInt (KEY_GUIDE_DONE, _guideDone ? 1 : 0);
			PlayerPrefs.Save ();
		}
	}

	private static bool _bestQuality = false;
	public static bool BestQuality{
		get{
			return _bestQuality;
		}
		set{
			_bestQuality = value;
			GIIControlCenter.Instance.FireEvent (GameConfigEvents.ENABLE_BEST_QUALITY, _bestQuality);
		}
	}

	public static bool LowQuality
	{
		get{
			return QualityManager.Instance.CurrentQualityLevel == QualityLevel.LOW;
		}
		set{
			if(value)
			{
				QualityManager.Instance.CurrentQualityLevel = QualityLevel.LOW;
			}
			else
			{
				QualityManager.Instance.CurrentQualityLevel = QualityLevel.HIGH;
			}
		}
	}
}
