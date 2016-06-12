using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class LocalizationSpriteConfig : LocalizationResourceConfig<Sprite>
{
	public LocalizationSpriteConfig(LanguageEnum lan, Sprite res = null) : base(lan, res)
	{

	}
}

public class GIIAutoLocalizationImage : GIIAutoLocalization {
	public Image _image = null;

	[SerializeField]
	public LocalizationSpriteConfig[] _localizedTextures = new LocalizationSpriteConfig[]{
		new LocalizationSpriteConfig(LanguageEnum.ENGLISH),
		new LocalizationSpriteConfig(LanguageEnum.CHINESE)
	};

	public override void Awake ()
	{
		base.Awake ();

		if(_image == null)
		{
			_image = GetComponent<Image> ();
		}
	}

	protected override void updateResource (LanguageEnum lan)
	{
		base.updateResource (lan);

		if(_image == null)
		{
			return;
		}

		if(lan == LanguageEnum.UNKNOWN)
		{
			return;
		}

		if(_localizedTextures == null)
		{
			return;
		}

		Sprite sp = null;
		foreach(var r in _localizedTextures)
		{
			if(r.Language == lan)
			{
				sp = r.Resource;
				break;
			}
		}

		if(sp!=null)
		{
			_image.sprite = sp;
		}
	}
}
