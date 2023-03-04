using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yourvrexperience.Utils
{
	public class SoundsController : MonoBehaviour
	{
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

		private AudioSource _audioBackground;
		private AudioSource _audioFX;

		void Awake()
		{
			AudioSource[] myAudioSources = GetComponents<AudioSource>();
			_audioBackground = myAudioSources[0];
			_audioFX = myAudioSources[1];
		}

		void Start()
		{
		}

		private void PlaySoundClipBackground(AudioClip audio, bool loop, float volume)
		{
			if (!EnableSound) return;

			_audioBackground.clip = audio;
			_audioBackground.loop = loop;
			_audioBackground.volume = volume;
			_audioBackground.Play();
		}

		public void StopSoundBackground()
		{
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


		private void PlaySoundClipFx(AudioClip audio, bool loop, float volume)
		{
			if (!EnableSound) return;

			_audioFX.clip = audio;
			_audioFX.loop = loop;
			_audioFX.volume = volume;
			_audioFX.Play();
		}

		public void StopSoundFx()
		{
			_audioFX.clip = null;
			_audioFX.Stop();
		}

		public void PlaySoundFX(string audioName, bool loop, float volume)
		{
			for (int i = 0; i < Sounds.Length; i++)
			{
				if (Sounds[i].name == audioName)
				{
					PlaySoundClipFx(Sounds[i], loop, volume);
				}
			}
		}

	}
}