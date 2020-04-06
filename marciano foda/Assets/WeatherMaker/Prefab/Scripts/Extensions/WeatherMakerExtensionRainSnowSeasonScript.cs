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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public abstract class WeatherMakerExtensionRainSnowSeasonScript<T> : WeatherMakerExtensionScript<T> where T : UnityEngine.MonoBehaviour
    {
        [System.Flags]
        public enum WeatherMakerRainSnowSeasonScriptFlags
        {
            ControlRain = 1,
            ControlSnow = 2,
            ControlSeason = 4
        }

        [WeatherMaker.EnumFlag]
        [Tooltip("What types of properties to control.")]
        public WeatherMakerRainSnowSeasonScriptFlags ControlFlags = WeatherMakerRainSnowSeasonScriptFlags.ControlRain | WeatherMakerRainSnowSeasonScriptFlags.ControlSnow | WeatherMakerRainSnowSeasonScriptFlags.ControlSeason;

        private float lastRain = -1.0f;
        private float lastSnow = -1.0f;
        private float lastSeason = -1.0f;

        static readonly float[] monthToSeasonLookupNorth = new float[]
        {
            0.33f,
            0.66f,
            0.99f,
            1.34f,
            1.67f,
            2.0f,
            2.33f,
            2.67f,
            3.0f,
            3.33f,
            3.67f,
            4.0f
        };

        static readonly float[] monthToSeasonLookupSouth = new float[]
        {
            2.33f,
            2.66f,
            2.99f,
            3.34f,
            3.67f,
            4.0f,
            0.33f,
            0.67f,
            1.0f,
            1.33f,
            1.67f,
            2.0f
        };

        private void UpdateRain()
        {
            if (WeatherMakerScript.Instance == null || WeatherMakerScript.Instance.PrecipitationManager == null)
            {
                return;
            }

            float rain = WeatherMakerScript.Instance.PrecipitationManager.RainIntensity;
            if (rain != lastRain)
            {
                lastRain = rain;
                rain += (WeatherMakerScript.Instance.PrecipitationManager.SleetIntensity * 0.5f);
                rain += (WeatherMakerScript.Instance.PrecipitationManager.HailIntensity * 0.25f);
                OnUpdateRain(rain);
            }
        }

        private void UpdateSnow()
        {
            if (WeatherMakerScript.Instance == null || WeatherMakerScript.Instance.PrecipitationManager == null)
            {
                return;
            }

            float snow = WeatherMakerScript.Instance.PrecipitationManager.SnowIntensity;
            if (snow != lastSnow)
            {
                lastSnow = snow;
                snow += (WeatherMakerScript.Instance.PrecipitationManager.SleetIntensity * 0.5f);
                snow += (WeatherMakerScript.Instance.PrecipitationManager.HailIntensity * 0.5f);
                OnUpdateSnow(snow);
            }
        }

        private void UpdateSeason()
        {
            if (WeatherMakerDayNightCycleManagerScript.Instance == null)
            {
                return;
            }

            int month = WeatherMakerDayNightCycleManagerScript.Instance.Month - 1;
            if (month < 0 || month > 11)
            {
                return;
            }
            float season;
            if (WeatherMakerDayNightCycleManagerScript.Instance.Latitude > 0.0f)
            {
                season = monthToSeasonLookupNorth[month];
            }
            else
            {
                season = monthToSeasonLookupSouth[month];
            }
            if (season != lastSeason)
            {
                lastSeason = season;
                OnUpdateSeason(season);
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (TypeScript == null)
            {
                return;
            }
            else if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlRain) == WeatherMakerRainSnowSeasonScriptFlags.ControlRain)
            {
                UpdateRain();
            }
            if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlSnow) == WeatherMakerRainSnowSeasonScriptFlags.ControlSnow)
            {
                UpdateSnow();
            }
            if ((ControlFlags & WeatherMakerRainSnowSeasonScriptFlags.ControlSeason) == WeatherMakerRainSnowSeasonScriptFlags.ControlSeason)
            {
                UpdateSeason();
            }
        }

        // rain intensity (0 to 1)
        protected virtual void OnUpdateRain(float rain) { }

        // snow intentity (0 to 1)
        protected virtual void OnUpdateSnow(float snow) { }

        // season: 0-1 winter, 1-2 spring, 2-3 summer, 3-4 fall
        protected virtual void OnUpdateSeason(float season) { }
    }
}
