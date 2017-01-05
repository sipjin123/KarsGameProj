using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UniRx;

using Common;
using Common.Extensions;
using Common.Utils;

using CColor = Common.Extensions.Color;

namespace Synergy88
{
    public class AudioPlayer : MonoBehaviour
    {
        public static readonly string ERROR = CColor.red.LogHeader("[HEADER]");

        public const string FILE_EXT = ".ogg";
        public const string SFX_PATH = "Audio/SFX/";
        public const string BGM_PATH = "Audio/BGM/";
        
        [SerializeField]
        private BGM playingBGM = BGM.Invalid;

        [SerializeField]
        private AudioSource[] audioSfx;

        [SerializeField]
        private AudioSource audioBgm;
        
        [SerializeField]
        private float masterVolume = 1.0f;
        public float MasterVolume
        {
            get
            {
                return this.masterVolume;
            }
        }

        [SerializeField]
        private bool isSFXEnabled = true;

        [SerializeField]
        private bool isBGMEnabled = true;

        [SerializeField]
        private SFXAudioMap SFXAudioMap;

        [SerializeField]
        private BGMAudioMap BGMAudioMap;

        private AudioSource activeAmbience = null;

        private void Awake()
        {
            Assertion.AssertNotNull(this.audioSfx);
            Assertion.AssertNotNull(this.audioBgm);
        }

        private void OnEnable()
        {
            Factory.Register<AudioPlayer>(this);
        }

        private void OnDisable()
        {
            Factory.Clean<AudioPlayer>();
        }
        
        private void UpdateSFX(bool value)
        {
            this.isSFXEnabled = value;
        }

        private void UpdateBGM(bool value)
        {
            this.isBGMEnabled = value;
            this.audioBgm.mute = !value;
        }
        
        public bool IsSFXplaying(int num)
        {
            return this.audioSfx[num].isPlaying;
        }

        public bool IsBGMPlaying()
        {
            return this.audioBgm.isPlaying;
        }

        public bool IsBGMPlaying(BGM bgm)
        {
            if (this.playingBGM != bgm)
            {
                return false;
            }

            return this.IsBGMPlaying();
        }


        public void PlayBgm(BGM bgm, float volume = 1.0f)
        {
            if (!this.isBGMEnabled)
            {
                return;
            }
            
            Assertion.Assert(BGMAudioMap.ContainsKey(bgm), string.Format("{0} AudioPlayer::PlayBgm Invalid Key! Sfx:{1} Volume:{2}\n", ERROR, bgm, volume));
            Assertion.AssertNotNull(BGMAudioMap[bgm], string.Format("{0} AudioPlayer::PlayBgm Invalid Key! Sfx:{1} Volume:{2}\n", ERROR, bgm, volume));

            if (this.audioBgm.isPlaying)
            {
                this.StartCoroutine(this.FadeOut(this.audioBgm, delegate {
                    this.audioBgm.clip = BGMAudioMap[bgm];
                    this.audioBgm.volume = volume;
                    this.audioBgm.Play();
                    this.playingBGM = bgm;
                }, 0.75f));
            }
            else
            {
                this.audioBgm.clip = BGMAudioMap[bgm];
                this.audioBgm.volume = volume;
                this.audioBgm.Play();
                this.playingBGM = bgm;
            }
        }

        public void PauseBGM()
        {
            this.audioBgm.Pause();
        }

        public void ResumeBGM()
        {
            this.audioBgm.UnPause();
        }

        public void StopBGM()
        {
            this.audioBgm.Stop();
            this.playingBGM = BGM.Invalid;
        }

        public void PlaySFX(SFX sfx, float volume = 1.0f)
        {
            if (!this.isSFXEnabled)
            {
                return;
            }
            
            Assertion.Assert(SFXAudioMap.ContainsKey(sfx), string.Format("{0} AudioPlayer::PlaySFX Invalid Key! Bgm:{1} Volume:{2}\n", ERROR, sfx, volume));
            Assertion.AssertNotNull(SFXAudioMap[sfx], string.Format("{0} AudioPlayer::PlaySFX Invalid Key! Bgm:{1} Volume:{2}\n", ERROR, sfx, volume));

            foreach (AudioSource audio in this.audioSfx)
            {
                if (!audio.isPlaying)
                {
                    audio.clip = SFXAudioMap[sfx];
                    audio.volume = volume;
                    audio.Play();
                    break;
                }
            }
        }

        public void PlaySFXAsAmbience(SFX sfx, float volume = 1.0f)
        {
            if (!this.isSFXEnabled)
            {
                return;
            }
            
            Assertion.Assert(SFXAudioMap.ContainsKey(sfx), string.Format("{0} AudioPlayer::PlaySFXAsAmbience Invalid Key! Bgm:{1} Volume:{2}\n", ERROR, sfx, volume));
            Assertion.AssertNotNull(SFXAudioMap[sfx], string.Format("{0} AudioPlayer::PlaySFXAsAmbience Invalid Key! Bgm:{1} Volume:{2}\n", ERROR, sfx, volume));

            foreach (AudioSource audio in this.audioSfx)
            {
                if (!audio.isPlaying)
                {
                    audio.clip = SFXAudioMap[sfx];
                    audio.volume = volume;
                    audio.Play();
                    activeAmbience = audio;
                    break;
                }
            }
        }

        public void StopCurrentAmbience()
        {
            if (activeAmbience != null && activeAmbience.isPlaying)
            {
                activeAmbience.Stop();
            }
        }

        public void PlaySFXFadeBgm(SFX sfx, float volume = 1.0f)
        {
            /*
            Assertion.Assert(AUDIO_DURATION.ContainsKey(sfx), "AudioPlayer::PlaySFXFadeBgm Invalid sfx for audio duration. sfx:" + sfx + "\n");
            this.StartCoroutine(this.FadeOut(this.audioBgm, delegate {
                this.PlaySFX(sfx, volume);
            }));

            this.StartCoroutine(this.FadeIn(this.audioBgm, null, AUDIO_DURATION[sfx]));
            //*/
        }

        public IEnumerator FadeIn(AudioSource audio, Action action = null, float delay = 0.0f)
        {
            if (delay > 0.0f)
            {
                yield return new WaitForSeconds(delay);
            }

            float duration = 0.15f;
            float timer = 0.0f;
            while (timer <= duration)
            {
                timer += Time.deltaTime;
                audio.volume = (timer / duration);
                yield return null;
            }

            timer = duration;
            audio.volume = (timer / duration);
            yield return null;

            if (action != null)
            {
                action();
            }
        }

        public IEnumerator FadeOut(AudioSource audio, Action action = null, float duration = 0.15f)
        {
            float timer = 0.0f;
            while (timer <= duration)
            {
                timer += Time.deltaTime;
                audio.volume = Mathf.Clamp01(1.0f - (timer / duration));
                yield return null;
            }

            timer = duration;
            audio.volume = Mathf.Clamp01(1.0f - (timer / duration));
            yield return null;

            if (action != null)
            {
                action();
            }
        }
        
    }
}