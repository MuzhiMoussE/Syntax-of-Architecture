using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Frame
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _bgmSource;
        private AudioSource _sfxSource;
        
        private float _fadeDuration = 2f;
        private Coroutine _fadeCoroutine;
        
        public void Init(AudioSource bgmSource, AudioSource sfxSource, float fadeDuration)
        {
            _bgmSource = bgmSource;
            _sfxSource = sfxSource;
            _fadeDuration = fadeDuration;
        }


        
        public void PlayBGM(AudioClip clip, float targetVolume)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            
            _fadeCoroutine = StartCoroutine(FadeOutAndSwitchBGM(clip,targetVolume));
        }

        private IEnumerator FadeOutAndSwitchBGM(AudioClip nextBGM,float targetVolume)
        {
            yield return LerpVolume(_bgmSource,0.0f);

            _bgmSource.Stop();

            // 切换BGM音频文件
            _bgmSource.clip = nextBGM;

            _bgmSource.Play();
            
            yield return LerpVolume(_bgmSource, targetVolume);
        }
        
        private IEnumerator LerpVolume(AudioSource source, float targetVolume)
        {
            var startVolume = source.volume;

            var currentTime = 0.0f;
            while (currentTime < _fadeDuration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / _fadeDuration);
                yield return null;
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            _sfxSource.Stop();
            _sfxSource.clip = clip;
            _sfxSource.Play();
        }

        public bool IsPlayingSFX(AudioClip interactingAudioClip)
        {
            return _sfxSource.clip == interactingAudioClip && _sfxSource.isPlaying;
        }
    }
}
