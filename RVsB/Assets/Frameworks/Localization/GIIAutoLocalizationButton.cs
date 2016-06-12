using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class ButtonSprites
{
	public Sprite NormalSprite;
	public Sprite PressedSprite;
	public Sprite DisabledSprite;

	public ButtonSprites()
	{
		NormalSprite = null;
		PressedSprite = null;
		DisabledSprite = null;
	}
}

[System.Serializable]
public class LocalizationButtonConfig : LocalizationResourceConfig<ButtonSprites>
{
	public LocalizationButtonConfig(LanguageEnum lan) : base(lan)
	{

	}
}

public class GIIAutoLocalizationButton : GIIAutoLocalization {
	public Button _button = null;

	[SerializeField]
	public LocalizationButtonConfig[] _localizedTextures = new LocalizationButtonConfig[]{
		new LocalizationButtonConfig(LanguageEnum.ENGLISH),
		new LocalizationButtonConfig(LanguageEnum.CHINESE)
	};

	public override void Awake ()
	{
		base.Awake ();

		if(_button == null)
		{
			_button = GetComponent<Button> ();
		}
	}

	protected override void updateResource (LanguageEnum lan)
	{
		base.updateResource (lan);

		if(_button == null)
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

		ButtonSprites res = null;
		foreach(var r in _localizedTextures)
		{
			if(r.Language == lan)
			{
				res = r.Resource;
				break;
			}
		}

		if(res!=null)
		{
			if (res.NormalSprite != null) 
			{
				_button.image.sprite = res.NormalSprite;
			}

			SpriteState st = new SpriteState ();

			if(res.PressedSprite!=null)
			{
				st.pressedSprite = res.PressedSprite;
			}

			if(res.DisabledSprite!=null)
			{
				st.disabledSprite = res.DisabledSprite;
			}
			_button.spriteState = st;
		}
	}
}
