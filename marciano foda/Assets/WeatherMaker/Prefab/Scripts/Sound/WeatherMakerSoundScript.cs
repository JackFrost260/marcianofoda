﻿//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store at https://www.assetstore.unity3d.com/en/#!/content/60955?aid=1011lGnL. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

// #define ENABLE_DEBUG_LOG_WEATHER_MAKER_SOUNDS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    [CreateAssetMenu(fileName = "WeatherMakerSound", menuName = "WeatherMaker/Sound", order = 110)]
    public class WeatherMakerSoundScript : WeatherMakerBaseScriptableObjectScript
    {
        private float timeToNextPlay;
        private float timeRemainingToPlay;

        [Tooltip("A name to help you keep track of the ambient sound")]
        public string Name;

        [Tooltip("The audio clip to play (one is chosen at random)")]
        public AudioClip[] AudioClips;

        [Tooltip("Whether to loop the audio clip, all clips must be looping or not looping.")]
        public bool Looping;

        [Tooltip("The times of day the sound can play. In order to play HoursOfDay must be empty or must also match.")]
        [EnumFlag]
        public WeatherMakerTimeOfDayCategory TimesOfDay;

        [Tooltip("The hours in a day the sound can play, or null/empty to just use TimesOfDay.")]
        public List<RangeOfFloats> HoursOfDay;

        [MinMaxSlider(0.0f, 120.0f, "How long in seconds to wait in between playing the sound. Set to 0 min and max to have no wait and a continous sound.")]
        public RangeOfFloats IntervalRange;

        [MinMaxSlider(0.0f, 300.0f, "How long in seconds to play the sound, for non-looped AudioSource this is ignored, and the entire sound is played.")]
        public RangeOfFloats DurationRange;

        [MinMaxSlider(0.0f, 1.0f, "Range of volume for the ambient sound, a new value is chosen each time the sound is played.")]
        public RangeOfFloats VolumeRange = new RangeOfFloats { Minimum = 1.0f, Maximum = 1.0f };

        [MinMaxSlider(0.0f, 60.0f, "How long in seconds to fade in and out for looping audio sources.")]
        public RangeOfFloats FadeDuration = new RangeOfFloats { Minimum = 5.0f, Maximum = 15.0f };

        /// <summary>
        /// Wrapper for AudioSource to help with looping
        /// </summary>
        public WeatherMakerLoopingAudioSource AudioSourceLoop { get; private set; }

        /// <summary>
        /// Whether the sound can play - this is usually true unless something like an audio zone is used and the player is not in the zone.
        /// </summary>
        public bool CanPlay { get; set; }

        /// <summary>
        /// Parent game object to put sounds in
        /// </summary>
        public GameObject Parent { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            CanPlay = true;

#if UNITY_EDITOR

            if (!Application.isPlaying)
            {
                return;
            }

#endif

            if (Parent != null)
            {
                AudioSource source = Parent.AddComponent<AudioSource>();
                source.loop = Looping;
                source.playOnAwake = false;
                source.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
                AudioSourceLoop = new WeatherMakerLoopingAudioSource(source, 0.0f, 0.0f, Looping);
            }
        }

        public void Stop(float fadeOutSeconds = 0.0f, bool destroyOnStopped = false)
        {
            if (AudioSourceLoop != null && AudioSourceLoop.AudioSource != null)
            {

#if ENABLE_DEBUG_LOG_WEATHER_MAKER_SOUNDS

                Debug.LogFormat("Weather Maker stopping sound {0}", Name);

#endif

                AudioSourceLoop.Stop(fadeOutSeconds, destroyOnStopped);
                timeToNextPlay = 0.0f;
            }
        }

        public override void Update()
        {
            if (AudioSourceLoop == null || AudioSourceLoop.AudioSource == null)
            {
                return;
            }
            else if (AudioSourceLoop.AudioSource.isPlaying)
            {
                // see if we need to stop
                if (!AudioSourceLoop.Stopping && ((timeRemainingToPlay -= Time.deltaTime) < 0.0f && IntervalRange.Minimum > 0.0f) || !CanStartSound())
                {
                    Stop();
                }
            }
            // check if it is the right time of day to play the ambient sound
            else if (CanStartSound())
            {
                if (timeToNextPlay <= 0.0f && IntervalRange.Minimum > 0.0f)
                {
                    CalculateNextPlay();
                }
                // check if it is time to play
                else if ((timeToNextPlay -= Time.deltaTime) < 0.0f)
                {
                    timeToNextPlay = 0.0f;
                    timeRemainingToPlay = (!Looping || DurationRange.Maximum <= 0.0f ? float.MaxValue : DurationRange.Random());
                    float startFade = Looping ? FadeDuration.Random() : 0.0f;
                    float endFade = Looping ? FadeDuration.Random() : 0.0f;
                    AudioSourceLoop.SetFade(startFade, endFade);
                    AudioSourceLoop.AudioSource.clip = AudioClips[UnityEngine.Random.Range(0, AudioClips.Length)];
                    AudioSourceLoop.Play(VolumeRange.Random());

#if ENABLE_DEBUG_LOG_WEATHER_MAKER_SOUNDS

                    if (Looping)
                    {
                        Debug.LogFormat("Weather Maker playing sound {0} for {1} seconds", Name, (timeRemainingToPlay == float.MaxValue ? "Infinite" : timeRemainingToPlay.ToString("0.00")));
                    }
                    else
                    {
                        Debug.LogFormat("Weather Maker playing sound {0} for {1:0.00} seconds", Name, AudioClip.length);
                    }

#endif

                }
            }
            else
            {
                timeToNextPlay = 0.0f;
            }
            AudioSourceLoop.VolumeModifier = WeatherMakerAudioManagerScript.CachedAmbientVolumeModifier;
            AudioSourceLoop.Update();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Resume();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Pause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (AudioSourceLoop != null && AudioSourceLoop.AudioSource != null)
            {
                GameObject.Destroy(AudioSourceLoop.AudioSource);
            }
        }

        private void Pause()
        {
            if (AudioSourceLoop != null)
            {
                AudioSourceLoop.Pause();
            }
        }

        private void Resume()
        {
            if (AudioSourceLoop != null && CanPlay)
            {
                AudioSourceLoop.Resume();
            }
        }

        private bool CanStartSound()
        {
            if (!Disabled && CanPlay && AudioClips != null && AudioClips.Length != 0 && AudioSourceLoop != null && AudioSourceLoop.AudioSource != null && !AudioSourceLoop.DestroyOnStopped &&
                (HoursOfDay.Count != 0 || (WeatherMakerDayNightCycleManagerScript.Instance != null && (int)(WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDayCategory & TimesOfDay) != 0)))
            {
                if (HoursOfDay.Count == 0)
                {
                    return true;
                }
                foreach (RangeOfFloats hours in HoursOfDay)
                {
                    if (WeatherMakerDayNightCycleManagerScript.Instance != null &&
                        WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDayTimespan.TotalHours >= hours.Minimum &&
                        WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDayTimespan.TotalHours <= hours.Maximum)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CalculateNextPlay(float delay = 0.0f)
        {
            timeToNextPlay = IntervalRange.Random() + delay;

#if ENABLE_DEBUG_LOG_WEATHER_MAKER_SOUNDS

            Debug.LogFormat("Weather Maker sound {0} will play in {1:0.00} seconds", Name, timeToNextPlay);

#endif

        }
    }
}