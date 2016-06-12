using UnityEngine;
using System.Collections;

public enum QualityLevel
{
	UNKNOWN,
	HIGH,
	LOW
}

public static class QualityEvents{
	public const string QUALITY_CHANGE = "Events.Quality.Change";
}

/// <summary>
/// Quality manager.
/// 画质控制管理，根据画质设定：
/// 1. 关闭或者取消某些效果
/// 2. 替换某些效果
/// </summary>
public class QualityManager : MonoBehaviour {
	public GameObject[] _ObjectsToDiableInLowQuality;
	public GameObject[] _ObjectsToDiableInHighQuality;

	private QualityLevel _currentQualityLevel = QualityLevel.UNKNOWN;

	public static QualityManager Instance
	{
		get{
			return SingletonMonoBehaviour<QualityManager>.Instance;
		}
	}

	void Awake()
	{
		if(!SingletonMonoBehaviour<QualityManager>.DestroyExtraObjects(this))
		{
			
		}
	}
	// Use this for initialization
//	void Start () {
//		updateQualityLevel (_currentQualityLevel);
//	}

	void updateQualityLevel(QualityLevel level)
	{
		if(_ObjectsToDiableInLowQuality != null)
		{
			foreach(var obj in _ObjectsToDiableInLowQuality)
			{
				obj.SetActive (level != QualityLevel.LOW);
			}
		}

		if(_ObjectsToDiableInHighQuality != null)
		{
			foreach(var obj in _ObjectsToDiableInHighQuality)
			{
				obj.SetActive (level != QualityLevel.HIGH);
			}
		}
	}

	public QualityLevel CurrentQualityLevel
	{
		get{
			return _currentQualityLevel;
		}
		set{
			if(_currentQualityLevel!=value)
			{
				_currentQualityLevel = value;
				updateQualityLevel (_currentQualityLevel);

				GIIControlCenter.Instance.FireEvent (QualityEvents.QUALITY_CHANGE, _currentQualityLevel);
			}
		}
	}
}
