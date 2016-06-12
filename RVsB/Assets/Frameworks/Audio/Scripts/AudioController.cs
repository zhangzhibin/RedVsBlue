using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 实现方式参考：http://unity.clockstone.com/assetStore-audioToolkit.html

[System.Serializable]
public struct SoundConfig
{
	public SoundEnum SoundId;
	public AudioClip Clip;
}

[System.Serializable]
public struct MusicConfig
{
	public MusicEnum MusicId;
	public AudioClip Clip;
}

/// <summary>
/// Audio controller.
/// 音频的总控制
/// 1. 管理音频文件和配置
/// 2. 管理正在播放的音乐和音效
/// 3. 控制音频的开启与关闭
/// 4. pooling
/// </summary>
public class AudioController : MonoBehaviour {
//	public static string DefaultMediatorName = "Mediator." + typeof(AudioController).Name;

	// 预定义好的音频对象
	public GameObject PredefinedAudioObject;

	// 背景音乐的配置
	public MusicConfig[] MusicConfigs;

	// 音效的配置
	public SoundConfig[] SoundConfigs;

	// 背景音乐缓存
	private TDAudioSource BGMSource;

	// 音效缓存
	private Dictionary<SoundEnum, List<TDAudioSource>> soundSources = new Dictionary<SoundEnum, List<TDAudioSource>>(); 

	public AudioController()
	{
//		MediatorName = DefaultMediatorName;
//
//		if(TheUserPreferenceProxy!=null)
//		{
//			EnableSound = TheUserPreferenceProxy.SoundOn;
//		}
//		else
//		{
		EnableSound = true;
//		}
	}

	public static AudioController Instance
	{
		get{
			return SingletonMonoBehaviour<AudioController>.Instance;
		}
	}

//	// 玩家配置
//	protected UserPreferenceProxy userPreferenceProxy;
//	protected virtual UserPreferenceProxy TheUserPreferenceProxy
//	{
//		get{
//			if(userPreferenceProxy==null)
//			{
//				userPreferenceProxy = ApplicationFacade.RetrieveProxy (UserPreferenceProxy.defaultProxyName) as UserPreferenceProxy;
//			}
//			return userPreferenceProxy;
//		}
//	}

	// Use this for initialization
	void Awake () 
	{
		SingletonMonoBehaviour<AudioController>.DestroyExtraObjects (gameObject);

		// 启动时 创建背景音乐的声源
		if(BGMSource==null)
		{
			var bgmObj = Instantiate(PredefinedAudioObject);

			bgmObj.transform.parent = transform;

			BGMSource = bgmObj.GetComponent<TDAudioSource> ();
		}
	}

	public void Init()
	{
//		// 读取配置
//		if(TheUserPreferenceProxy!=null)
//		{
//			EnableSound = TheUserPreferenceProxy.SoundOn;
//		}
	}

	public AudioClip GetAudioClip(SoundEnum soundId)
	{
		if(soundId==SoundEnum.NONE)
		{
			return null;
		}

		AudioClip audioClip = null;
		foreach(var soundConfig in SoundConfigs)
		{
			if(soundConfig.SoundId==soundId)
			{
				audioClip = soundConfig.Clip;
				break;
			}
		}

		return audioClip;
	}

	public AudioClip GetAudioClip(MusicEnum musicId)
	{
		if(musicId==MusicEnum.NONE)
		{
			return null;
		}

		AudioClip audioClip = null;
		foreach(var musicConfig in MusicConfigs)
		{
			if(musicConfig.MusicId==musicId)
			{
				audioClip = musicConfig.Clip;
				break;
			}
		}

		return audioClip;
	}

	// 查找空闲的声源
	// TODO: 现在声源根据播放的声音进行分组缓存，是否可以考虑不根据声音分组，而是共用一个缓存池
	private TDAudioSource getFreeSoundSource(SoundEnum soundId)
	{
		if(!soundSources.ContainsKey(soundId))
		{
			soundSources.Add (soundId, new List<TDAudioSource> ());
		}

		List<TDAudioSource> audioSourceList = null;
		soundSources.TryGetValue (soundId, out audioSourceList);

		// 查找空闲声源
		TDAudioSource audio = null;
		foreach(var _audio in audioSourceList)
		{
			if(!_audio.IsPlaying)
			{
				audio = _audio;
				break;
			}
		}

		if(audio==null)
		{
			// 如果没找到空闲，则添加一个新的
			var audioObject = Instantiate (PredefinedAudioObject);
			audioObject.transform.parent = transform;
			audio = audioObject.GetComponent<TDAudioSource> ();

			audio.SoundId = soundId;

			audioSourceList.Add (audio);
		}

		return audio;
	}

	// 操作 
	private bool _enableSound;
	public bool EnableSound
	{
		get{
			return _enableSound;
		}
		set{
			if(_enableSound!=value)
			{
				_enableSound = value;
				if(_enableSound)
				{
					if(BGMSource!=null)
					{
						// 
						BGMSource.Play ();
					}
				}
				else
				{
					if(BGMSource!=null)
					{
						BGMSource.Stop ();
					}
				}
			}
		}
	}

	public void Pause()
	{
		AudioListener.pause = true;
	}

	public void Resume()
	{
		AudioListener.pause = false;
	}

	public void PlaySound(SoundEnum sound, bool loop = false)
	{
		if(!EnableSound)
		{
			return;
		}

		var audioSource = getFreeSoundSource (sound);
		if(audioSource!=null)
		{
			audioSource.Play (loop?1:0);
		}
	}

	public void PlayMusic(MusicEnum music, bool loop = true)
	{
		if(!EnableSound)
		{
			return;
		}

		if(BGMSource==null)
		{
			var bgmObj = Instantiate (PredefinedAudioObject);
//			GameObject.DontDestroyOnLoad (bgmObj);

			bgmObj.transform.parent = transform;

			BGMSource = bgmObj.GetComponent<TDAudioSource> ();
		}

		if(BGMSource!=null)
		{
			BGMSource.MusicId = music;
			BGMSource.Play (loop?1:0);
		}
	}

	public void StopMuisc()
	{
		if(BGMSource!=null)
		{
			BGMSource.Stop ();
		}
	}

	public void PauseMusic(bool v)
	{
		if(BGMSource!=null)
		{
			BGMSource.Pause (v);
		}
	}

//	// 事件处理 [开始]
//	public override IList<string> ListNotificationInterests()
//	{
//		return new string[] {
//			UserPreferenceEvents.EVENT_SOUND_ON_UPDATE	
//		};
//	}
//
//	public override void HandleNotification(PureMVC.Interfaces.INotification notification)
//	{
//		switch(notification.Name)
//		{
//		case UserPreferenceEvents.EVENT_SOUND_ON_UPDATE:
//			OnSoundOnUpdate ((bool)notification.Body);	
//			break;
//		}
//	}

	public void OnSoundOnUpdate(bool v)
	{
		EnableSound = v;
	}
	// 事件处理 [结束]

}
