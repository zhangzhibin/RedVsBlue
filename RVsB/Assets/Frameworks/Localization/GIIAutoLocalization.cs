using UnityEngine;
using System.Collections;



[System.Serializable]
public class LocalizationResourceConfig<RESOURCE_TYPE>
{
	public LanguageEnum Language;
	public RESOURCE_TYPE Resource;

//	public LocalizationResourceConfig()
//	{
//		Language = LanguageEnum.UNKNOWN;
//		Resource = default(RESOURCE_TYPE);
//	}

	public LocalizationResourceConfig(LanguageEnum lan, RESOURCE_TYPE res = default(RESOURCE_TYPE))
	{
		Language = lan;
		Resource = res;
	}
}

public class GIIAutoLocalization : MonoBehaviour, IGIIEventObserver {
	[SerializeField]
	protected LanguageEnum _currentLanguage = LanguageEnum.UNKNOWN;
	public virtual void Awake()
	{

	}

	void OnEnable()
	{
		onLanguageChange (GameConfigs.Language);

		GIIControlCenter.Instance.RegisterEventObserver (GameConfigEvents.PREFERED_LANGUAGE_CHANGE, this);

	}

	void OnDisable()
	{
		GIIControlCenter.Instance.UnregisterEventObserver (GameConfigEvents.PREFERED_LANGUAGE_CHANGE, this);
	}

	private void onLanguageChange(LanguageEnum lan)
	{
		if(lan == LanguageEnum.UNKNOWN)
		{
			return;
		}

		if(lan == _currentLanguage)
		{
			return;
		}

		_currentLanguage = lan;

		updateResource (lan);
	}

	public virtual bool OnEvent (IGIIEvent eventData)
	{
		if(eventData.Name == GameConfigEvents.PREFERED_LANGUAGE_CHANGE)
		{
			onLanguageChange ((LanguageEnum)eventData.Body);
		}

		return true;
	}

	// 所有的子类需要自行处理本方法	
	protected virtual void updateResource(LanguageEnum lan)
	{
		Debug.Assert (lan != LanguageEnum.UNKNOWN);
	}
}
