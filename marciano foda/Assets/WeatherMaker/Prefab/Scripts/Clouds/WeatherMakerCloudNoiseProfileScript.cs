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
    public enum WeatherMakerCloudNoiseType
    {
        Perlin = 10,
        Simplex = 20,
        Worley = 50,
        PerlinWorley = 100,
        SimplexWorley = 110
    }

    [System.Serializable]
    public class WeatherMakerCloudNoiseParameters
    {
        [Range(1, 6)]
        [Tooltip("Octaves")]
        public int Octaves = 6;

        [Range(0.1f, 64.0f)]
        [Tooltip("Period")]
        public float Period = 6.0f;

        [Range(-1.0f, 3.0f)]
        [Tooltip("Brightness")]
        public float Brightness = 0.0f;

        [Range(0.0f, 8.0f)]
        [Tooltip("Multiplier")]
        public float Multiplier = 1.0f;

        [Range(0.0f, 64.0f)]
        [Tooltip("Power")]
        public float Power = 1.0f;

        [Tooltip("Whether to invert the noise")]
        public bool Invert;

        public Vector4 GetParams1()
        {
            return new Vector4(Octaves, Period, Brightness, Invert ? 1.0f : 0.0f);
        }

        public Vector4 GetParams2()
        {
            return new Vector4(Multiplier, Power, 0.0f, 0.0f);
        }
    }

    [CreateAssetMenu(fileName = "WeatherMakerCloudNoiseProfile_", menuName = "WeatherMaker/Cloud Noise Profile", order = 180)]
    public class WeatherMakerCloudNoiseProfileScript : ScriptableObject
    {
        [Tooltip("Noise type")]
        public WeatherMakerCloudNoiseType NoiseType = WeatherMakerCloudNoiseType.Perlin;

        [Tooltip("Perlin noise parameters")]
        public WeatherMakerCloudNoiseParameters PerlinParameters;

        [Tooltip("Worley noise parameters")]
        public WeatherMakerCloudNoiseParameters WorleyParameters = new WeatherMakerCloudNoiseParameters { Invert = true };

        public void ApplyToMaterial(Material m)
        {
            m.SetInt(WMS._CloudNoiseType, (int)NoiseType);
            m.SetVector(WMS._CloudNoisePerlinParams1, PerlinParameters.GetParams1());
            m.SetVector(WMS._CloudNoisePerlinParams2, PerlinParameters.GetParams2());
            m.SetVector(WMS._CloudNoiseWorleyParams1, WorleyParameters.GetParams1());
            m.SetVector(WMS._CloudNoiseWorleyParams2, WorleyParameters.GetParams2());
        }
    }
}
