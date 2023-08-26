using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace yourvrexperience.Utils
{
	public class SoundsController : MonoBehaviour
	{
		public const string EventSoundsControllerFadeCompleted = "EventSoundsControllerFadeCompleted";

		public enum ChannelsAudio { Background = 0, FX1, FX2 }

		private static SoundsController _instance;
		public static SoundsController Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType<SoundsController>();
				}
				return _instance;
			}
		}

		public AudioClip[] Sounds;
		public bool EnableSound = false;		

		private AudioSource[] _audioSources;
		private AudioSource _audioBackground;

		private bool _activateFadeOut;
		private bool _activateFadeIn;
		private float _timeToFade = 0;

		private string _currentAudioMelodyPlaying = "";

		public string CurrentAudioMelodyPlaying
		{
			get { return _currentAudioMelodyPlaying; }
		}

		void Awake()
		{
			_audioSources = GetComponents<AudioSource>();
			_audioBackground = _audioSources[(int)ChannelsAudio.Background];
		}

		void Start()
		{
		}

		void OnDestroy()
		{
			_instance = null;
		}

		public void StopAllSounds()
		{
			StopSoundBackground();
			StopSoundsFx();
		}

		public void PlaySoundClipBackground(AudioClip audio, bool loop, float volume, bool is3D = false)
		{
			if (!EnableSound) return;

			ResetFade();

			_currentAudioMelodyPlaying = audio.name;
			_audioBackground.clip = audio;
			_audioBackground.loop = loop;
			_audioBackground.volume = volume;
			_audioBackground.Play();
			_audioBackground.spatialBlend = (is3D?1:0);
		}

		public void PauseSoundBackground()
		{
			_audioBackground.Pause();
		}
		public void ResumeSoundBackground()
		{
			_audioBackground.UnPause();
		}

		public void StopSoundBackground()
		{
			_currentAudioMelodyPlaying = "";
			_audioBackground.clip = null;
			_audioBackground.Stop();
		}

		public void PlaySoundBackground(string audioName, bool loop, float volume)
		{
			for (int i = 0; i < Sounds.Length; i++)
			{
				if (Sounds[i].name == audioName)
				{
					PlaySoundClipBackground(Sounds[i], loop, volume);
				}
			}
		}

		public void PlaySoundClipFx(ChannelsAudio channel, AudioClip audio, bool loop, float volume, bool is3D = false)
		{
			if (!EnableSound) return;
			if ((int)channel >= _audioSources.Length) return;

			ResetFade();

			_audioSources[(int)channel].clip = audio;
			_audioSources[(int)channel].loop = loop;
			_audioSources[(int)channel].volume = volume;
			_audioSources[(int)channel].Play();
			_audioSources[(int)channel].spatialBlend = (is3D?1:0);
		}

		public void StopSoundsFx()
		{
			if ((int)ChannelsAudio.FX1 < _audioSources.Length) 
			{
				_audioSources[(int)ChannelsAudio.FX1].clip = null;
				_audioSources[(int)ChannelsAudio.FX1].Stop();
			}
			if ((int)ChannelsAudio.FX2 < _audioSources.Length) 
			{
				_audioSources[(int)ChannelsAudio.FX2].clip = null;
				_audioSources[(int)ChannelsAudio.FX2].Stop();
			}			
		}

		public void PlaySoundFX(string audioName, bool loop, float volume)
		{
			for (int i = 0; i < Sounds.Length; i++)
			{
				if (Sounds[i].name == audioName)
				{
					PlaySoundClipFx(ChannelsAudio.FX1, Sounds[i], loop, volume);
				}
			}
		}

		public void PlaySoundFX(ChannelsAudio channel, string audioName, bool loop, float volume)
		{
			for (int i = 0; i < Sounds.Length; i++)
			{
				if (Sounds[i].name == audioName)
				{
					PlaySoundClipFx(channel, Sounds[i], loop, volume);
				}
			}
		}

		private void ResetFade()
		{
			_activateFadeOut = false;
			_activateFadeIn = false;
			_timeToFade = 0;
		}

		public void FadeOut(float timeToFade)
		{
			_activateFadeOut = true;
			_timeToFade = timeToFade;
			_audioBackground.volume = 1;
		}
		public void FadeIn(float timeToFade)
		{
			_activateFadeIn = true;
			_timeToFade = timeToFade;
			_audioBackground.volume = 0;
		}

		private void FadeInUpdate()
		{
			if (_activateFadeIn)
			{
				_timeToFade -= Time.deltaTime;
				if (_timeToFade > 0)
				{
					_audioBackground.volume += Time.deltaTime/_timeToFade;
				}
				else
				{
					_audioBackground.volume = 0;
					_activateFadeIn = false;
					SystemEventController.Instance.DispatchSystemEvent(EventSoundsControllerFadeCompleted);
				}
			}
		}

		private void FadeOutUpdate()
		{
			if (_activateFadeOut)
			{
				_timeToFade -= Time.deltaTime;
				if (_timeToFade > 0)
				{
					_audioBackground.volume -= Time.deltaTime/_timeToFade;
				}
				else
				{
					_audioBackground.volume = 0;
					_activateFadeOut = false;
					SystemEventController.Instance.DispatchSystemEvent(EventSoundsControllerFadeCompleted);
				}
			}
		}

		public void PlayRemoteBackground(ChannelsAudio channel, string audioURL, bool loop, float volume, bool is3D = false)
		{
			StartCoroutine(LoadAudioFromServer(audioURL, true, channel, loop, volume, is3D));
		}

		public void PlayRemoteFX(ChannelsAudio channel, string audioURL, bool loop, float volume, bool is3D = false)
		{
			StartCoroutine(LoadAudioFromServer(audioURL, false, channel, loop, volume, is3D));
		}

		private IEnumerator LoadAudioFromServer(string url, bool isBackground, ChannelsAudio channel, bool loop, float volume, bool is3D)
		{
			using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
			{
				yield return www.SendWebRequest();

				if (www.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.LogError("Error while receiving audio clip: " + www.error);
				}
				else
				{
					AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
					if (isBackground)
					{
						PlaySoundClipBackground(audioClip, loop, volume, is3D);
					}
					else
					{
						PlaySoundClipFx(channel, audioClip, loop, volume, is3D);
					}					
				}
			}
		}

		public void Play3DSound(AudioClip audioClip, Vector3 position, float volume, GameObject objectSound = null, bool loop = false)
        {
			if (!EnableSound) return;
            if (audioClip == null) return;

            AudioSource audioSource;
            GameObject soundGameObject = null;
            if (objectSound == null)
            {
                soundGameObject = new GameObject("One Shot Sound");
            }
            else
            {
                soundGameObject = new GameObject("Passenger Object");
                soundGameObject.AddComponent<PassengerObject>();
                soundGameObject.GetComponent<PassengerObject>().MainObject = objectSound;
                soundGameObject.transform.position = objectSound.transform.position;
            }

            soundGameObject.AddComponent<AudioSource>();
            soundGameObject.transform.position = position;
            audioSource = soundGameObject.GetComponent<AudioSource>();

            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.spatialBlend = 1;
            audioSource.loop = loop;

            audioSource.Play();

            if (objectSound == null)
            {
                GameObject.Destroy(soundGameObject, audioSource.clip.length);
            }                
        }

		void Update()
		{
			FadeInUpdate();
			FadeOutUpdate();
		}

	}
}