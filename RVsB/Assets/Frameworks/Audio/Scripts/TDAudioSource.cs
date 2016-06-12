using UnityEngine;
using System.Collections;

public class TDAudioSource : MonoBehaviour {
	private AudioClip _audioClip;
	public AudioClip TheAudioClip
	{
		get{
			return _audioClip;
		}
		set{
			_audioClip = value;
		}
	}

	private SoundEnum _soundId;
	public SoundEnum SoundId
	{
		get{
			return _soundId;
		}	
		set{
			Debug.AssertFormat (MusicId == MusicEnum.NONE, "Music is not none: {0}", MusicId);

			if(_soundId!=value)
			{
				_soundId = value;
				TheAudioClip = AudioController.Instance.GetAudioClip (value);
			}

		}
	}

	private MusicEnum _musicId;
	public MusicEnum MusicId
	{
		get{
			return _musicId;
		}
		set{
			Debug.AssertFormat (SoundId == SoundEnum.NONE, "Sound is not none: {0}", MusicId);
			if(_musicId!=value)
			{
				_musicId = value;
				TheAudioClip = AudioController.Instance.GetAudioClip (value);
			}
		}
	}

	public bool IsMusic
	{
		get{
			return (SoundId == SoundEnum.NONE && MusicId != MusicEnum.NONE);
		}
	}

	public bool IsSound
	{
		get{
			return (SoundId != SoundEnum.NONE && MusicId == MusicEnum.NONE);
		}
	}

	private AudioSource _audioSource;
	private AudioSource theAudioSource
	{
		get{
			if(_audioSource==null)
			{
				_audioSource = GetComponent<AudioSource> ();
				if(_audioSource==null)
				{
					_audioSource = gameObject.AddComponent<AudioSource> ();
				}

//				if(TheAudioClip!=_audioSource.clip)
//				{
//					_audioSource.clip = TheAudioClip;
//				}
				_audioSource.loop = _loop;
			}
			return _audioSource;
		}
	}

	private bool _loop;
	public bool Loop
	{
		get{
			return _loop;
		}
		set{
			_loop = value;
			if(theAudioSource!=null)
			{
				theAudioSource.loop = _loop;
			}
		}
	}

	public bool IsPlaying
	{
		get{
			if(theAudioSource!=null)
			{
				return theAudioSource.isPlaying;
			}
			return false;
		}
	}

	public void Play()
	{
		Play (-1);
	}

	public void Play(int loop)
	{
		if(theAudioSource!=null)
		{
			if(loop==0)
			{
				Loop = false;
			}
			else if(loop>0)
			{
				Loop = true;
			}

			theAudioSource.clip = TheAudioClip;

			if(!AudioController.Instance.EnableSound)
			{
				return;
			}
			else
			{
				theAudioSource.Play ();
			}
		}
	}

	public void Stop()
	{
		if(theAudioSource!=null && theAudioSource.isPlaying)
		{
			theAudioSource.Stop ();
		}
	}

	public void Pause(bool v)
	{
		if(theAudioSource!=null)
		{
			if(v)
			{
				theAudioSource.Pause();
			}
			else
			{
				theAudioSource.UnPause ();
			}
		}
	}

	public void Mute(bool mute)
	{
		if(theAudioSource!=null)
		{
			theAudioSource.mute = mute;
		}
	}
}
