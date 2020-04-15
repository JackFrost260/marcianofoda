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
using UnityEngine.Rendering;

namespace DigitalRuby.WeatherMaker
{
    [CreateAssetMenu(fileName = "WeatherMakerFogProfile", menuName = "WeatherMaker/Fog Profile", order = 60)]
    public class WeatherMakerFogProfileScript : ScriptableObject
    {
        [Header("Fog appearance")]
        [Tooltip("Fog mode")]
        public WeatherMakerFogMode FogMode = WeatherMakerFogMode.Exponential;

        [Tooltip("Fog density")]
        [Range(0.0f, 1.0f)]
        public float FogDensity = 0.0f;
        internal float fogDensity;

        [Tooltip("Multiply the computed fog factor by this value")]
        [Range(1.0f, 100.0f)]
        public float FogFactorMultiplier = 1.0f;

        [Tooltip("The depth where fog starts (in world space units). Ignored if fog is not full screen or fog has a height limit.")]
        [Range(0.0f, 1000.0f)]
        public float FogStartDepth = 0.0f;

        [Tooltip("The depth where fog ends (in world space units). Ignored if 0 or fog is not full screen or fog has a height limit.")]
        [Range(0.0f, 100000.0f)]
        public float FogEndDepth = 0.0f;

        [Tooltip("Fog color")]
        public Color FogColor = Color.white;

        [Tooltip("Fog emission color, alpha is intensity")]
        public Color FogEmissionColor = Color.black;

        [Tooltip("Fog gradient color based on sun position where center is sun at horizon.")]
        public Gradient FogGradientColor = new Gradient();

        [Tooltip("Fog density gradient based on sun position where center is sun at horizon. Use alpha, color is ignored.")]
        public Gradient FogDensityGradient = new Gradient();

        [Range(0.0f, 10.0f)]
        [Tooltip("Fog light absorption - lower values absorb more light, higher values scatter and intensify light more.")]
        public float FogLightAbsorption = 1.0f;

        [Tooltip("Maximum fog factor, where 1 is the maximum allowed fog.")]
        [Range(0.0f, 1.0f)]
        public float MaxFogFactor = 1.0f;

        [Header("Fog noise")]
        [Tooltip("Fog noise scale. Lower values get less tiling. 0 to disable noise.")]
        [Range(0.0f, 1.0f)]
        public float FogNoiseScale = 0.01f;

        [Tooltip("Controls how the noise value is calculated. Negative values allow areas of no noise, higher values increase the intensity of the noise.")]
        [Range(-1.0f, 1.0f)]
        public float FogNoiseAdder = 0.0f;

        [Tooltip("How much the noise effects the fog.")]
        [Range(0.0f, 10.0f)]
        public float FogNoiseMultiplier = 0.0f;

        [Tooltip("Fog noise velocity, determines how fast the fog moves. Not all fog scripts support 3d velocity, some only support 2d velocity (x and y).")]
        public Vector3 FogNoiseVelocity = new Vector3(0.01f, 0.01f, 0.0f);
        internal Vector3 fogNoiseVelocityAccum;

        [Tooltip("Fog noise rotation speed in radians per second. W is rotation radius.")]
        public Vector4 FogNoiseVelocityRotation;

        [Tooltip("True to have wind affect fog noise velocity, false otherwise. This does require scanning for wind zones, so disable if you see any performance issues.")]
        public bool WindEffectsFogNoiseVelocity;

        [Tooltip("Number of samples to take for 3D fog. If the player will never enter the fog, this can be a lower value. If the player can move through the fog, 40 or higher is better, but will cost some performance.")]
        [Range(1, 128)]
        public int FogNoiseSampleCount = 40;

        [Tooltip("Dithering level. 0 to disable dithering.")]
        [Range(0.0f, 1.0f)]
        public float DitherLevel = 0.005f;

        [Header("Fog shadows (sun only, requires EnableFogLights)")]
        [Tooltip("Number of shadow samples, 0 to disable fog shadows.")]
        [Range(0, 256)]
        public int FogShadowSampleCount = 0;

        [Tooltip("Max ray length for fog shadows. Set to 0 to disable fog shadows.")]
        [Range(0.0f, 20000.0f)]
        public float FogShadowMaxRayLength = 1000.0f;

        [Tooltip("Multiplier for fog shadow lighting. Higher values make brighter light rays.")]
        [Range(0.0f, 32.0f)]
        public float FogShadowMultiplier = 8.0f;

        [Tooltip("Controls how light falls off from the light source. Higher values fall off faster. Setting this to a value that is a power of two is recommended.")]
        [Range(0.0f, 128.0f)]
        public float FogShadowPower = 64.0f;

        [Tooltip("Controls brightness of light in the fog vs in shadow.")]
        [Range(0.0f, 10.0f)]
        public float FogShadowBrightness = 1.0f;

        [Tooltip("Controls how light falls off from the light source. Lower values fall off faster.")]
        [Range(0.0f, 1.0f)]
        public float FogShadowDecay = 0.95f;

        [Tooltip("Fog shadow dither multiplier. Higher values dither more.")]
        [Range(0.0f, 3.0f)]
        public float FogShadowDither = 0.4f;

        [Tooltip("How much cloud shadows effect the fog, set to 0 for indoor fog.")]
        [Range(0.0f, 1.0f)]
        public float FogCloudShadowStrength = 1.0f;

        [Tooltip("Control how fog casts shadow. Larger values cast more shadow.")]
        [Range(0.0f, 0.02f)]
        public float FogShadowStrengthFactor = 0.005f;

        [Tooltip("Magic numbers for fog shadow dithering. Tweak if you don't like the dithering appearance.")]
        public Vector4 FogShadowDitherMagic = new Vector4(0.73f, 1.665f, 1024.0f, 1024.0f);

        [Header("Volume smoothing (ignored for full screen fog)")]
        [Tooltip("Fog edge smooth factor. Ignored for full screen fog.")]
        [Range(0.0f, 1.0f)]
        public float FogEdgeSmoothFactor = 0.25f;

        [Tooltip("Fog height falloff power.")]
        [Range(0.0f, 1.0f)]
        public float FogHeightFalloffPower = 0.75f;

        [Tooltip("Fog edge falloff power. Ignored for full screen fog.")]
        [Range(0.0f, 128.0f)]
        public float FogEdgeFalloffPower = 0.75f;

        /// <summary>
        /// Density of fog for scattering reduction
        /// </summary>
        private float fogScatterReduction = 1.0f;
        public float FogScatterReduction { get { return fogScatterReduction; } }

        /// <summary>
        /// Whether this fog profile will render noise in the fog
        /// </summary>
        public bool HasNoise { get { return FogNoiseScale > 0.0f && FogNoiseMultiplier > 0.0f && WeatherMakerLightManagerScript.NoiseTexture3DInstance != null; } }

        internal Vector3 fogNoisePositionOffset;
        internal float fogNoisePercent = 1.0f;

        /// <summary>
        /// Update a fog material with fog properties from this object
        /// </summary>
        /// <param name="material">Fog material</param>
        /// <param name="camera">Camera</param>
        /// <param name="global">Whether to set global shader properties</param>
        public virtual void UpdateMaterialProperties(Material material, Camera camera, bool global)
        {
            WeatherMakerCelestialObjectScript sun = WeatherMakerLightManagerScript.SunForCamera(camera);
            if (sun == null)
            {
                return;
            }

            bool gamma = (QualitySettings.activeColorSpace == ColorSpace.Gamma);
            float scatterCover = (WeatherMakerFullScreenCloudsScript.Instance != null && WeatherMakerFullScreenCloudsScript.Instance.enabled && WeatherMakerFullScreenCloudsScript.Instance.CloudProfile != null ? WeatherMakerFullScreenCloudsScript.Instance.CloudProfile.CloudCoverTotal : 0.0f);
            scatterCover = Mathf.Pow(scatterCover, 4.0f);
            Color fogEmissionColor = FogEmissionColor * FogEmissionColor.a;
            fogEmissionColor.a = FogEmissionColor.a;
            float fogNoiseSampleCount = (float)(WeatherMakerScript.Instance == null ? FogNoiseSampleCount : WeatherMakerScript.Instance.PerformanceProfile.FogNoiseSampleCount);
            float invFogNoiseSampleCount = 1.0f / fogNoiseSampleCount;
            int hasNoiseInt = (HasNoise ? 1 : 0);
            float ditherLevel = (gamma ? DitherLevel : DitherLevel * 0.5f);
            Color fogDensityColor = sun.GetGradientColor(FogDensityGradient);
            fogDensity = FogDensity * fogDensityColor.a;

            if (!(this is WeatherMakerFullScreenFogProfileScript) || FogMode == WeatherMakerFogMode.None || fogDensity <= 0.0f || MaxFogFactor <= 0.001f)
            {
                fogScatterReduction = 1.0f;
            }
            else if (FogMode == WeatherMakerFogMode.Exponential)
            {
                fogScatterReduction = Mathf.Clamp(1.0f - ((fogDensity + scatterCover) * 0.5f), 0.15f, 1.0f);
            }
            else if (FogMode == WeatherMakerFogMode.Linear)
            {
                fogScatterReduction = Mathf.Clamp((1.0f - ((fogDensity + scatterCover) * 0.25f)), 0.15f, 1.0f);
            }
            else if (FogMode == WeatherMakerFogMode.ExponentialSquared)
            {
                fogScatterReduction = Mathf.Clamp((1.0f - ((fogDensity + scatterCover) * 0.75f)), 0.15f, 1.0f);
            }
            else if (FogMode == WeatherMakerFogMode.Constant)
            {
                fogScatterReduction = Mathf.Clamp(1.0f - (fogDensity + scatterCover), 0.5f, 1.0f);
            }
            material.SetFloat(WMS._WeatherMakerFogDensityScatter, fogScatterReduction);
            material.SetInt(WMS._WeatherMakerFogNoiseEnabled, hasNoiseInt);
            if (global)
            {
                Shader.SetGlobalFloat(WMS._WeatherMakerFogDensityScatter, fogScatterReduction);
                Shader.SetGlobalInt(WMS._WeatherMakerFogNoiseEnabled, hasNoiseInt);
            }
            if (WeatherMakerScript.Instance == null || WeatherMakerScript.Instance.PerformanceProfile.EnableFogLights)
            {
                float fogShadowSampleCount = (float)(WeatherMakerScript.Instance == null ? FogShadowSampleCount : WeatherMakerScript.Instance.PerformanceProfile.FogShadowSampleCount);
                float brightness = Mathf.Min(sun.Light.intensity, Mathf.Pow(sun.Light.intensity, 2.0f));
                if (QualitySettings.shadows != ShadowQuality.Disable &&
                    fogShadowSampleCount > 0 &&
                    sun.Light.intensity > 0.0f &&
                    sun.Light.shadows != LightShadows.None &&
                    FogShadowMaxRayLength > 0.0f &&
                    brightness > 0.0f)
                {
                    float invFogShadowSampleCount = 1.0f / fogShadowSampleCount;
                    float fogShadowBrightness = brightness * FogShadowBrightness;

                    material.SetInt(WMS._WeatherMakerFogVolumetricLightMode, 2);
                    material.SetFloat(WMS._WeatherMakerFogLightShadowSampleCount, fogShadowSampleCount);
                    material.SetFloat(WMS._WeatherMakerFogLightShadowInvSampleCount, invFogShadowSampleCount);
                    material.SetFloat(WMS._WeatherMakerFogLightShadowMaxRayLength, FogShadowMaxRayLength);
                    material.SetFloat(WMS._WeatherMakerFogLightShadowMultiplier, FogShadowMultiplier);
                    material.SetFloat(WMS._WeatherMakerFogLightShadowPower, FogShadowPower);
                    material.SetFloat(WMS._WeatherMakerFogLightShadowBrightness, fogShadowBrightness);
                    material.SetFloat(WMS._WeatherMakerFogLightShadowDecay, FogShadowDecay);
                    material.SetFloat(WMS._WeatherMakerFogLightShadowDither, FogShadowDither);
                    material.SetVector(WMS._WeatherMakerFogLightShadowDitherMagic, FogShadowDitherMagic);
                    material.SetFloat(WMS._WeatherMakerFogCloudShadowStrength, FogCloudShadowStrength);

                    if (global)
                    {
                        Shader.SetGlobalInt(WMS._WeatherMakerFogVolumetricLightMode, 2);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogLightShadowSampleCount, fogShadowSampleCount);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogLightShadowInvSampleCount, invFogShadowSampleCount);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogLightShadowMaxRayLength, FogShadowMaxRayLength);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogLightShadowMultiplier, FogShadowMultiplier);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogLightShadowPower, FogShadowPower);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogLightShadowBrightness, fogShadowBrightness);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogLightShadowDecay, FogShadowDecay);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogLightShadowDither, FogShadowDither);
                        Shader.SetGlobalVector(WMS._WeatherMakerFogLightShadowDitherMagic, FogShadowDitherMagic);
                        Shader.SetGlobalFloat(WMS._WeatherMakerFogCloudShadowStrength, FogCloudShadowStrength);
                    }
                }
                else
                {
                    material.SetInt(WMS._WeatherMakerFogVolumetricLightMode, 1);
                    if (global)
                    {
                        Shader.SetGlobalInt(WMS._WeatherMakerFogVolumetricLightMode, 1);
                    }
                }
            }
            else
            {
                material.SetInt(WMS._WeatherMakerFogVolumetricLightMode, 0);
                if (global)
                {
                    Shader.SetGlobalInt(WMS._WeatherMakerFogVolumetricLightMode, 0);
                }
            }

            Color fogColor = sun.GetGradientColor(FogGradientColor) * FogColor;
            FogEndDepth = Mathf.Max(FogStartDepth, FogEndDepth);
            float endDepth = (FogEndDepth <= FogStartDepth ? FogStartDepth + 5000.0f : FogEndDepth);
            float fogLinearFogFactor = 0.1f * (1.0f / Mathf.Max(0.0001f, (endDepth - FogStartDepth))) * fogDensity * (endDepth - FogStartDepth);
            material.SetFloat(WMS._WeatherMakerFogStartDepth, FogStartDepth);
            material.SetFloat(WMS._WeatherMakerFogEndDepth, endDepth);
            material.SetColor(WMS._WeatherMakerFogColor, fogColor);
            material.SetColor(WMS._WeatherMakerFogEmissionColor, fogEmissionColor);
            material.SetFloat(WMS._WeatherMakerFogHeightFalloffPower, FogHeightFalloffPower);
            material.SetFloat(WMS._WeatherMakerFogLightAbsorption, FogLightAbsorption);
            material.SetFloat(WMS._WeatherMakerFogLinearFogFactor, fogLinearFogFactor);
            material.SetFloat(WMS._WeatherMakerFogNoisePercent, fogNoisePercent);
            material.SetFloat(WMS._WeatherMakerFogNoiseScale, FogNoiseScale);
            material.SetFloat(WMS._WeatherMakerFogNoiseAdder, FogNoiseAdder);
            material.SetFloat(WMS._WeatherMakerFogNoiseMultiplier, FogNoiseMultiplier);
            material.SetVector(WMS._WeatherMakerFogNoiseVelocity, fogNoiseVelocityAccum);
            material.SetFloat(WMS._WeatherMakerFogNoiseSampleCount, fogNoiseSampleCount);
            material.SetFloat(WMS._WeatherMakerFogNoiseSampleCountInverse, invFogNoiseSampleCount);
            material.SetVector(WMS._WeatherMakerFogNoisePositionOffset, fogNoisePositionOffset);
            material.SetFloat(WMS._WeatherMakerFogFactorMax, MaxFogFactor);
            material.SetInt(WMS._WeatherMakerFogMode, (int)FogMode);
            material.SetFloat(WMS._WeatherMakerFogDitherLevel, ditherLevel);
            material.SetFloat(WMS._WeatherMakerFogDensity, fogDensity);
            material.SetFloat(WMS._WeatherMakerFogFactorMultiplier, FogFactorMultiplier);
            if (global)
            {
                Shader.SetGlobalFloat(WMS._WeatherMakerFogStartDepth, FogStartDepth);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogEndDepth, endDepth);
                Shader.SetGlobalColor(WMS._WeatherMakerFogColor, fogColor);
                Shader.SetGlobalColor(WMS._WeatherMakerFogEmissionColor, fogEmissionColor);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogHeightFalloffPower, FogHeightFalloffPower);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogLightAbsorption, FogLightAbsorption);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogLinearFogFactor, fogLinearFogFactor);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogNoisePercent, fogNoisePercent);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogNoiseScale, FogNoiseScale);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogNoiseAdder, FogNoiseAdder);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogNoiseMultiplier, FogNoiseMultiplier);
                Shader.SetGlobalVector(WMS._WeatherMakerFogNoiseVelocity, fogNoiseVelocityAccum);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogNoiseSampleCount, fogNoiseSampleCount);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogNoiseSampleCountInverse, invFogNoiseSampleCount);
                Shader.SetGlobalVector(WMS._WeatherMakerFogNoisePositionOffset, fogNoisePositionOffset);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogFactorMax, MaxFogFactor);
                Shader.SetGlobalInt(WMS._WeatherMakerFogMode, (int)FogMode);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogDitherLevel, ditherLevel);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogDensity, fogDensity);
                Shader.SetGlobalFloat(WMS._WeatherMakerFogFactorMultiplier, FogFactorMultiplier);
                Shader.SetGlobalFloat(WMS._DirectionalLightMultiplier, material.GetFloat(WMS._DirectionalLightMultiplier));
                Shader.SetGlobalFloat(WMS._PointSpotLightMultiplier, material.GetFloat(WMS._PointSpotLightMultiplier));
            }
        }

        public void Update()
        {
            fogNoiseVelocityAccum += (FogNoiseVelocity * Time.deltaTime * 0.005f);

            if (FogNoiseVelocityRotation.x != 0.0f || FogNoiseVelocityRotation.y != 0.0f || FogNoiseVelocityRotation.z != 0.0f)
            {
                fogNoisePositionOffset = new Vector3(FogNoiseVelocityRotation.w, FogNoiseVelocityRotation.w, FogNoiseVelocityRotation.w);
                fogNoisePositionOffset = Quaternion.Euler((Vector3)FogNoiseVelocityRotation * Time.time) * fogNoisePositionOffset;
            }
        }

        public virtual void CopyStateTo(WeatherMakerFogProfileScript profile)
        {
            profile.fogNoiseVelocityAccum = fogNoiseVelocityAccum;
            profile.fogNoisePercent = fogNoisePercent;
        }
    }

    public enum WeatherMakerFogMode
    {
        None,
        Constant,
        Linear,
        Exponential,
        ExponentialSquared
    }
}
