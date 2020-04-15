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
    public enum WeatherMakerPrecipitationType
    {
        None = 0,
        Rain = 1,
        Snow = 2,
        Sleet = 3,
        Hail = 4,
        Custom = 127
    }

    [CreateAssetMenu(fileName = "WeatherMakerPrecipitationProfile", menuName = "WeatherMaker/Precipitation Profile", order = 25)]
    [System.Serializable]
    public class WeatherMakerPrecipitationProfileScript : WeatherMakerBaseScriptableObjectScript
    {
        [Tooltip("Type of precipitation")]
        public WeatherMakerPrecipitationType PrecipitationType = WeatherMakerPrecipitationType.Rain;

        [MinMaxSlider(0.0f, 1.0f, "Range of intensities")]
        public RangeOfFloats IntensityRange = new RangeOfFloats { Minimum = 0.1f, Maximum = 0.3f };

        [MinMaxSlider(0.0f, 120.0f, "How often a new value from IntensityRange should be chosen")]
        public RangeOfFloats IntensityRangeDuration = new RangeOfFloats { Minimum = 10.0f, Maximum = 60.0f };

        [Tooltip("Tint the precipitation, useful for acid rain or other magical effects.")]
        public Color PrecipitationTintColor = Color.white;

        [Tooltip("Tint the precipitation mist, useful for acid rain or other magical effects.")]
        public Color PrecipitationMistTintColor = Color.white;

        [Tooltip("Tint the precipitation secondary, useful for acid rain or other magical effects.")]
        public Color PrecipitationSecondaryTintColor = Color.white;
    }
}
