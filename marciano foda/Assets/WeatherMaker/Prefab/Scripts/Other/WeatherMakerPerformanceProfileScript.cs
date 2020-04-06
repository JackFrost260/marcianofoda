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
    [CreateAssetMenu(fileName = "WeatherMakerPerformanceProfile", menuName = "WeatherMaker/Performance Profile", order = 999)]
    public class WeatherMakerPerformanceProfileScript : ScriptableObject
    {
        [Header("Clouds")]
        [Tooltip("Whether to allow volumetric clouds, if false the volumetric cloud member of any cloud profile will be nulled before being set.")]
        public bool EnableVolumetricClouds = true;

        [Tooltip("Downsample scale for clouds")]
        public WeatherMakerDownsampleScale VolumetricCloudDownsampleScale = WeatherMakerDownsampleScale.HalfResolution;

        [Tooltip("Temporal reprojection size for clouds")]
        public WeatherMakerTemporalReprojectionSize VolumetricCloudTemporalReprojectionSize = WeatherMakerTemporalReprojectionSize.TwoByTwo;

        [Tooltip("Downsample scale for cloud post process (dir light rays, etc.)")]
        public WeatherMakerDownsampleScale VolumetricCloudDownsampleScalePostProcess = WeatherMakerDownsampleScale.QuarterResolution;

        [Tooltip("Volumetric cloud weather map size. Higher weather maps allow smoother clouds and movement at the cost of a larger weather map texture.")]
        [Range(128, 4096)]
        public int VolumetricCloudWeatherMapSize = 1024;

        [MinMaxSlider(16, 256, "Volumetric cloud sample count range")]
        public RangeOfIntegers VolumetricCloudSampleCount = new RangeOfIntegers { Minimum = 64, Maximum = 256 };

        [Tooltip("Volumetric cloud max ray length multiplier, accumulates noise over longer distance, especially at horizon")]
        [Range(1.0f, 100.0f)]
        public float VolumetricCloudMaxRayLengthMultiplier = 10.0f;

        [Tooltip("Ray march multiplier. Greater than 1.0 improves performance but can reduce quality.")]
        [Range(0.1f, 10.0f)]
        public float VolumetricCloudRaymarchMultiplier = 2.0f;

        [Tooltip("Dither cloud ray direction to try and avoid banding, 0 for none.")]
        [Range(0.0f, 1.0f)]
        public float VolumetricCloudRayDither = 0.01f;

        [Tooltip("Number of ray marches with no cloud before increasing ray march step. 0 to disable this feature.")]
        [Range(0, 256)]
        public int VolumetricCloudRaymarchSkipThreshold = 0;

        [Tooltip("Reduce cloud raymarch by this amount while possibly in a cloud. Reduce for better cloud details but watch out for artifacts. Ignored if skip threshold is 0.")]
        [Range(0.01f, 1.0f)]
        public float VolumetricCloudRaymarchMaybeInCloudStepMultiplier = 1.0f;

        [Tooltip("Reduce cloud raymarch by this amount while in a cloud. Reduce for better cloud details but watch out for artifacts. Ignored if skip threshold is 0.")]
        [Range(0.01f, 1.0f)]
        public float VolumetricCloudRaymarchInCloudStepMultiplier = 1.0f;

        [Tooltip("Once skip threshold is reached, continually multiply step distance by this value. Ignored if skip threshold is 0.")]
        [Range(1.01f, 2.0f)]
        public float VolumetricCloudRaymarchSkipMultiplier = 1.1f;

        [Tooltip("The max number of times to multiply the raymarch distance, this ensures that the ray march step distance does not become so large that it misses clouds.")]
        [Range(1, 64)]
        public int VolumetricCloudRaymarchSkipMultiplierMaxCount = 16;

        [MinMaxSlider(0.0f, 10.0f, "Volumetric cloud noise lod, used to increase mip level during noise sampling")]
        public RangeOfFloats VolumetricCloudLod = new RangeOfFloats { Minimum = 0.0f, Maximum = 1.0f };

        [Tooltip("Volumetric cloud dir light sample count")]
        [Range(0, 6)]
        public int VolumetricCloudDirLightSampleCount = 5;

        [Tooltip("Sample count for volumetric cloud dir light rays. Set to 0 to disable.")]
        [Range(0, 64)]
        public int VolumetricCloudDirLightRaySampleCount = 16;

        [Tooltip("Number of samples for volumetric cloud shadows")]
        [Range(0, 16)]
        public int VolumetricCloudShadowSampleCount = 8;

        [Tooltip("Whether to allow volumetric cloud reflections.")]
        public bool VolumetricCloudAllowReflections = true;

        [Header("Aurora Borealis / Northern Lights")]
        [Tooltip("Aurora sample count max")]
        [Range(0, 64)]
        public int AuroraSampleCount = 42;

        [Tooltip("Aurora sub-sample count max")]
        [Range(1, 4)]
        public int AuroraSubSampleCount = 4;

        [Header("Precipitation")]
        [Tooltip("Whether to enable precipitation collision. This can impact performance, so turn off if this is a problem.")]
        public bool EnablePrecipitationCollision;

        [Tooltip("Whether per pixel lighting is enabled - currently precipitation particles are the only materials that support this. " +
            "Turn off if you see a performance impact.")]
        public bool EnablePerPixelLighting = true;

        [Header("Fog")]
        [Tooltip("Downsample scale for fog")]
        public WeatherMakerDownsampleScale FogDownsampleScale = WeatherMakerDownsampleScale.HalfResolution;

        [Tooltip("Temporal reprojection size for fog")]
        public WeatherMakerTemporalReprojectionSize FogTemporalReprojectionSize = WeatherMakerTemporalReprojectionSize.TwoByTwo;

        [Tooltip("Whether to enable volumetric fog point/spot lights. Fog always uses directional lights. Disable to improve performance.")]
        public bool EnableFogLights = true;

        [Tooltip("Sample count for full screen fog sun shafts. Set to 0 to disable.")]
        [Range(0, 64)]
        public int FogFullScreenSunShaftSampleCount = 16;

        [Tooltip("Fog noise sample count, 0 to disable fog noise.")]
        [Range(0, 128)]
        public int FogNoiseSampleCount = 40;

        [Tooltip("Fog dir light shadow sample count, 0 to disable fog shadows.")]
        [Range(0, 128)]
        public int FogShadowSampleCount = 0;

        [Header("Reflections")]
        [Tooltip("Whether to ignore reflection probe cameras. For an indoor scene or if you don't care to reflect clouds, fog, etc. set this to true.")]
        public bool IgnoreReflectionProbes;

        [Tooltip("Reflection texture size, bigger textures look better but at a cost of additional rendering time. Set to 127 to disable reflections.")]
        [Range(127, 4096)]
        public int ReflectionTextureSize = 1024;

        [Tooltip("Reflection shadow mode.")]
        public ShadowQuality ReflectionShadows = ShadowQuality.HardOnly;

        [Header("Post Processing")]
        [Tooltip("Whether to enable tonemapping for effects that support it. If you are using post process color grading, consider setting this to false.")]
        public bool EnableTonemap = true;
    }
}
