using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LanguageEnum
{
	UNKNOWN,
	CHINESE,
	ENGLISH
}

public class GiiLocalizationManager : MonoBehaviour {
	public SystemLanguage _DefaultLanguage;

	public List<SystemLanguage> _SupportLanguages;
	public List<GiiLocalizationConfig.StringResource> _Strings;

	public string GetString(string languageKey, SystemLanguage language)
	{
		return "";
	}
}
