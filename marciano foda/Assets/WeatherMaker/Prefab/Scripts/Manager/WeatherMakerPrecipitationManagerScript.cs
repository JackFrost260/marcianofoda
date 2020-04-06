//
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Precipitation manager interface
    /// </summary>
    public interface IPrecipitationManager : IWeatherMakerManager
    {
        /// <summary>
        /// Rain intensity (0 - 1)
        /// </summary>
        float RainIntensity { get; }

        /// <summary>
        /// Snow intensity (0 - 1)
        /// </summary>
        float SnowIntensity { get; }

        /// <summary>
        /// Hail intensity (0 - 1)
        /// </summary>
        float HailIntensity { get; }

        /// <summary>
        /// Sleet intensity (0 - 1)
        /// </summary>
        float SleetIntensity { get; }

        /// <summary>
        /// Custom intensity (0 - 1)
        /// </summary>
        float CustomIntensity { get; }
    }

    /// <summary>
    /// Precipitation manager default implementation
    /// </summary>
    public class WeatherMakerPrecipitationManagerScript : MonoBehaviour, IPrecipitationManager
    {
        private WeatherMakerPrecipitationType precipitation = WeatherMakerPrecipitationType.None;
        [Header("Precipitation")]
        [Tooltip("Current precipitation")]
        public WeatherMakerPrecipitationType Precipitation = WeatherMakerPrecipitationType.None;

        [Tooltip("Intensity of precipitation (0-1)")]
        [Range(0.0f, 1.0f)]
        public float PrecipitationIntensity;

        [Tooltip("How long in seconds to fully change from one precipitation type to another")]
        [Range(0.0f, 300.0f)]
        public float PrecipitationChangeDuration = 4.0f;

        [Tooltip("How long to delay before applying a change in precipitation intensity.")]
        [Range(0.0f, 300.0f)]
        public float PrecipitationChangeDelay = 0.0f;

        [Tooltip("The threshold change in intensity that will cause a cross-fade between precipitation changes. Intensity changes smaller than this value happen quickly.")]
        [Range(0.0f, 0.2f)]
        public float PrecipitationChangeThreshold = 0.1f;

        [Header("Dependencies")]
        [Tooltip("Rain script")]
        public WeatherMakerFallingParticleScript RainScript;

        [Tooltip("Snow script")]
        public WeatherMakerFallingParticleScript SnowScript;

        [Tooltip("Hail script")]
        public WeatherMakerFallingParticleScript HailScript;

        [Tooltip("Sleet script")]
        public WeatherMakerFallingParticleScript SleetScript;

        [Tooltip("Set a custom precipitation script for use with Precipitation = WeatherMakerPrecipitationType.Custom ")]
        public WeatherMakerFallingParticleScript CustomPrecipitationScript;

        /// <summary>
        /// The current precipitation script - use Precipitation to change precipitation
        /// </summary>
        public WeatherMakerFallingParticleScript PrecipitationScript { get; private set; }

        private float lastPrecipitationIntensity = -1.0f;
        private float nextIntensityChangeSeconds = -1.0f;
        private RangeOfFloats nextIntensityChangeRange;
        private RangeOfFloats nextIntensityDurationRange;
        private Color precipitationTintColor = Color.white;
        private Color precipitationMistTintColor = Color.white;
        private Color precipitationSecondaryTintColor = Color.white;
        private bool transitionInProgress;

        private void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
        }

        private void OnDisable()
        {

        }

        private void LateUpdate()
        {
            CheckForPrecipitationChange();
        }

        private void OnDestroy()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        private void TweenPrecipitationScript(WeatherMakerFallingParticleScript script, float end)
        {
            if (PrecipitationChangeDuration < 0.1f)
            {
                script.Intensity = end;
                return;
            }

            float duration = (Mathf.Abs(script.Intensity - end) < PrecipitationChangeThreshold ? 0.0f : PrecipitationChangeDuration);
            FloatTween tween = TweenFactory.Tween("WeatherMakerPrecipitationChange_" + script.gameObject.GetInstanceID(), script.Intensity, end, duration, TweenScaleFunctions.Linear, (t) =>
            {
                //Debug.LogFormat("Precipitation tween key: {0}, value: {1}, prog: {2}, duration: {3}, delay: {4}", t.Key, t.CurrentValue, t.CurrentProgress, duration, PrecipitationChangeDelay);
                script.Intensity = t.CurrentValue;
                transitionInProgress = true;
            }, (t) =>
            {
                //Debug.LogFormat("Precipitation tween key: {0} completed", t.Key);
                transitionInProgress = false;
            });
            tween.Delay = PrecipitationChangeDelay;
            PrecipitationChangeDelay = 0.0f;
        }

        private void ChangePrecipitation(WeatherMakerFallingParticleScript newPrecipitation)
        {
            if (newPrecipitation != PrecipitationScript && PrecipitationScript != null)
            {
                // animate away from the current precipitation
                TweenPrecipitationScript(PrecipitationScript, 0.0f);
                lastPrecipitationIntensity = -1.0f;
            }

            // set new precipitation script
            PrecipitationScript = newPrecipitation;
        }

        private void CheckForPrecipitationChange()
        {
            if (Precipitation != precipitation)
            {
                precipitation = Precipitation;
                switch (precipitation)
                {
                    default:
                        ChangePrecipitation(null);
                        break;

                    case WeatherMakerPrecipitationType.Rain:
                        ChangePrecipitation(RainScript);
                        break;

                    case WeatherMakerPrecipitationType.Snow:
                        ChangePrecipitation(SnowScript);
                        break;

                    case WeatherMakerPrecipitationType.Hail:
                        ChangePrecipitation(HailScript);
                        break;

                    case WeatherMakerPrecipitationType.Sleet:
                        ChangePrecipitation(SleetScript);
                        break;

                    case WeatherMakerPrecipitationType.Custom:
                        ChangePrecipitation(CustomPrecipitationScript);
                        break;
                }
            }

            // pick new intensity if profile says to do so
            if (nextIntensityChangeSeconds > 0.0f && (nextIntensityChangeSeconds -= Time.deltaTime) <= 0.0f)
            {
                if (!transitionInProgress)
                {
                    PrecipitationIntensity = nextIntensityChangeRange.Random();
                }
                nextIntensityChangeSeconds = nextIntensityDurationRange.Random();
            }

            if (PrecipitationScript != null && PrecipitationIntensity != lastPrecipitationIntensity)
            {
                lastPrecipitationIntensity = PrecipitationIntensity;
                TweenPrecipitationScript(PrecipitationScript, PrecipitationIntensity);
                PrecipitationScript.PrecipitationTintColor = precipitationTintColor;
                PrecipitationScript.PrecipitationMistTintColor = precipitationMistTintColor;
                PrecipitationScript.PrecipitationSecondaryTintColor = precipitationSecondaryTintColor;
            }
        }

        public void WeatherProfileChanged(WeatherMakerProfileScript oldProfile, WeatherMakerProfileScript newProfile, float transitionDelay, float transitionDuration)
        {
            Precipitation = newProfile.PrecipitationProfile.PrecipitationType;
            PrecipitationChangeDelay = transitionDelay;
            PrecipitationChangeDuration = transitionDuration;
            PrecipitationIntensity = newProfile.PrecipitationProfile.IntensityRange.Random();
            nextIntensityChangeSeconds = (newProfile.PrecipitationProfile.IntensityRangeDuration.Maximum <= 0.0f ? 0.0f : newProfile.PrecipitationProfile.IntensityRangeDuration.Random());
            nextIntensityChangeRange = newProfile.PrecipitationProfile.IntensityRange;
            nextIntensityDurationRange = newProfile.PrecipitationProfile.IntensityRangeDuration;
            Color oldPrecipitationTintColor = precipitationTintColor;
            Color oldPrecipitationMistTintColor = precipitationMistTintColor;
            Color oldPecipitationSecondaryTintColor = precipitationSecondaryTintColor;
            Color newPrecipitationTintColor = newProfile.PrecipitationProfile.PrecipitationTintColor;
            Color newPrecipitationMistTintColor = newProfile.PrecipitationProfile.PrecipitationMistTintColor;
            Color newPrecipitationSecondaryTintColor = newProfile.PrecipitationProfile.PrecipitationSecondaryTintColor;
            FloatTween tween = TweenFactory.Tween("WeatherMakerPrecipitation_" + GetInstanceID(), 0.0f, 1.0f, transitionDuration, TweenScaleFunctions.Linear, (ITween<float> c) =>
            {
                float progress = c.CurrentValue;
                precipitationTintColor = Color.Lerp(oldPrecipitationTintColor, newPrecipitationTintColor, progress);
                precipitationMistTintColor = Color.Lerp(oldPrecipitationMistTintColor, newPrecipitationMistTintColor, progress);
                precipitationSecondaryTintColor = Color.Lerp(oldPecipitationSecondaryTintColor, newPrecipitationSecondaryTintColor, progress);
            });
            tween.Delay = transitionDelay;
        }

        public void GetSnowIntensityUnity(WeatherMakerOutputParameterFloat value) { value.Value = SnowIntensity; }
        public void GetWetnessIntensityUnity(WeatherMakerOutputParameterFloat value) { value.Value = Mathf.Max(RainIntensity, SleetIntensity); }

        public float RainIntensity { get { return RainScript.Intensity; } }
        public float SnowIntensity { get { return SnowScript.Intensity; } }
        public float HailIntensity { get { return HailScript.Intensity; } }
        public float SleetIntensity { get { return SleetScript.Intensity; } }
        public float CustomIntensity { get { return CustomPrecipitationScript.Intensity; } }

        private static WeatherMakerPrecipitationManagerScript instance;
        /// <summary>
        /// Shared instance of precipitation manager script
        /// </summary>
        public static WeatherMakerPrecipitationManagerScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance); }
        }
    }
}
