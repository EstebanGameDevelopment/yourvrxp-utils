using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yourvrexperience.Utils
{
    public class SoundFader : MonoBehaviour
    {
		private bool _activateFadeOut;
		private bool _activateFadeIn;
		private float _timeToFade = 0;
		private float _originalTimeToFade = 0;
		private int _iteration = 0;

		private AudioSource _audioSource;

		public static void CreateSoundFader(AudioSource audioSource, bool isFadeIn, float timeToFade)
        {
			GameObject soundFader = new GameObject();
			soundFader.name = "SOUND_FADER";
			SoundFader fader = soundFader.AddComponent<SoundFader>();
			fader.Init(audioSource);
			if (isFadeIn)
            {
				fader.FadeIn(timeToFade);
			}
			else
            {
				fader.FadeOut(timeToFade);
			}
		}

		public void Init(AudioSource audioSource)
		{
			_audioSource = audioSource;
			_activateFadeOut = false;
			_activateFadeIn = false;
			_timeToFade = 0;
		}

        private void OnDestroy()
        {
			_audioSource = null;
		}

        public void FadeOut(float timeToFade)
		{
			_activateFadeOut = true;
			_timeToFade = timeToFade;
			_originalTimeToFade = _timeToFade;
			// _audioSource.volume = 1;
		}
		public void FadeIn(float timeToFade)
		{
			_activateFadeIn = true;
			_timeToFade = timeToFade;
			_originalTimeToFade = _timeToFade;
			_audioSource.volume = 0;
		}

		private void FadeInUpdate()
		{
			if (_activateFadeIn)
			{
				if ((_audioSource != null) && (_audioSource.clip != null))
				{
					_timeToFade -= Time.fixedDeltaTime;
					if (_timeToFade > 0)
					{
						_audioSource.volume = (_originalTimeToFade - _timeToFade) / _originalTimeToFade;
					}
					else
					{
						_audioSource.volume = 1;
						_activateFadeIn = false;
						_audioSource = null;
						SystemEventController.Instance.DispatchSystemEvent(SoundsController.EventSoundsControllerFadeCompleted);
						GameObject.Destroy(this.gameObject);
					}
				}
				else
                {
					_audioSource = null;
					GameObject.Destroy(this.gameObject);
				}
			}
		}

		private void FadeOutUpdate()
		{
			if (_activateFadeOut)
			{
				if ((_audioSource != null) && (_audioSource.clip != null))
                {
					if (_audioSource.clip.length - _audioSource.time < _originalTimeToFade)
					{
						if (_iteration == 0)
						{
							_audioSource.volume = 1;
						}
						_iteration++;
						_timeToFade -= Time.fixedDeltaTime;
						if (_timeToFade > 0)
						{
							_audioSource.volume = (_timeToFade / _originalTimeToFade);
						}
						else
						{
							_audioSource.volume = 0;
							_activateFadeOut = false;
							_audioSource = null;
							SystemEventController.Instance.DispatchSystemEvent(SoundsController.EventSoundsControllerFadeCompleted);
							GameObject.Destroy(this.gameObject);
						}
					}
				}
				else
                {
					_audioSource = null;
					GameObject.Destroy(this.gameObject);
				}
			}
		}

		void FixedUpdate()
		{
			FadeInUpdate();
			FadeOutUpdate();
		}
	}
}
