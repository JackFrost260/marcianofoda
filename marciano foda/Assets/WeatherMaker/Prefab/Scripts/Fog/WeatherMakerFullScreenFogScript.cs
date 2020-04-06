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

using UnityEngine;
using UnityEngine.Rendering;

using System;
using System.Collections.Generic;

namespace DigitalRuby.WeatherMaker
{
    [ExecuteInEditMode]
    public class WeatherMakerFullScreenFogScript : WeatherMakerFogScript<WeatherMakerFullScreenFogProfileScript>
    {
        [Header("Unity fog")]
        [Tooltip("Whether to apply settings to Unity fog (true) or to use Weather Maker full screen volumetric fog (false).")]
        public bool UseUnityFog;

        [Header("Full screen fog - rendering")]
        [Tooltip("Down sample scale.")]
        public WeatherMakerDownsampleScale DownSampleScale = WeatherMakerDownsampleScale.FullResolution;

        [Tooltip("Material to render the fog full screen after it has been calculated")]
        public Material FogFullScreenMaterial;

        [Tooltip("Fog Blur Material.")]
        public Material FogBlurMaterial;

        [Tooltip("Material for temporal reprojection")]
        public Material TemporalReprojectionMaterial;

        [Tooltip("Fog Blur Shader Type.")]
        public BlurShaderType BlurShader;

        [Tooltip("Temporal reprojection size - allows rendering a portion of this effect over a number of frames to spread cost out over time. " +
            "This can introduce rendering artifacts so be on the lookout for that.")]
        public WeatherMakerTemporalReprojectionSize TemporalReprojection = WeatherMakerTemporalReprojectionSize.None;

        [Tooltip("Render fog in this render queue for the command buffer.")]
        public CameraEvent FogRenderQueue = CameraEvent.BeforeForwardAlpha;

        [Tooltip("Whether to render fog in reflection cameras.")]
        public bool AllowReflections = true;

        [Tooltip("Control how fog casts shadow. Larger values cast more shadow.")]
        [Range(0.0f, 0.02f)]
        public float FogShadowStrengthFactor = 0.005f;

        private WeatherMakerFullScreenEffect effect;
        private System.Action<WeatherMakerCommandBuffer> updateShaderPropertiesAction;

        private const string commandBufferName = "WeatherMakerFullScreenFogScript";

        private float shadowMultiplier;
        private float intensityMultiplier;

        private void UpdateUnityFog(Camera camera)
        {
            if (!UseUnityFog)
            {
                return;
            }

            WeatherMakerCelestialObjectScript sun = WeatherMakerLightManagerScript.SunForCamera(camera);
            if (sun == null)
            {
                return;
            }

            RenderSettings.fog = (FogProfile.fogDensity > 0.0001f && FogProfile.FogMode != WeatherMakerFogMode.None);
            Color fogColor = (FogProfile.FogColor * (sun == null || !sun.LightIsOn ? Color.black : sun.GetGradientColor(FogProfile.FogGradientColor)));
            if (WeatherMakerLightManagerScript.Instance != null)
            {
                bool cameraIsPerspective = (camera == null || !camera.orthographic);
                foreach (WeatherMakerLightManagerScript.LightState light in WeatherMakerLightManagerScript.Instance.Lights)
                {
                    if (light.Light.type == LightType.Directional && light.Light != sun.Light)
                    {
                        WeatherMakerCelestialObjectScript obj = light.Light.GetComponent<WeatherMakerCelestialObjectScript>();
                        if (obj != null && obj.OrbitTypeIsPerspective == cameraIsPerspective)
                        {
                            fogColor += (FogProfile.FogColor * (light.Light.color * light.Light.intensity));
                        }
                        break;
                    }
                }
            }
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = FogProfile.fogDensity;
            switch (FogProfile.FogMode)
            {
                case WeatherMakerFogMode.Constant:
                case WeatherMakerFogMode.Linear:
                    RenderSettings.fogMode = FogMode.Linear;
                    break;

                case WeatherMakerFogMode.None:
                    RenderSettings.fogDensity = 0.0f;
                    break;

                case WeatherMakerFogMode.Exponential:
                    RenderSettings.fogMode = FogMode.Exponential;
                    break;

                case WeatherMakerFogMode.ExponentialSquared:
                    RenderSettings.fogMode = FogMode.ExponentialSquared;
                    break;
            }
        }

        private void UpdateFogProperties(Camera camera)
        {
            if (FogProfile == null || WeatherMakerScript.Instance == null || WeatherMakerLightManagerScript.Instance == null)
            {
                return;
            }
            else if (WeatherMakerScript.Instance.PerformanceProfile != null)
            {
                DownSampleScale = WeatherMakerScript.Instance.PerformanceProfile.FogDownsampleScale;
                TemporalReprojection = WeatherMakerScript.Instance.PerformanceProfile.FogTemporalReprojectionSize;
            }

            // reduce shadow strength as the fog blocks out dir lights
            float h;
            float m = Mathf.Pow(FogProfile.MaxFogFactor, 3.0f);
            const float p = 0.005f;

            switch (FogProfile.FogMode)
            {
                case WeatherMakerFogMode.Constant:
                    shadowMultiplier = 1.0f - (Mathf.Min(1.0f, FogProfile.fogDensity * 1.2f * m));
                    intensityMultiplier = 1.0f - (FogProfile.fogDensity * m);
                    break;

                case WeatherMakerFogMode.Linear:
                    h = (FogProfile.FogHeight < Mathf.Epsilon ? 1000.0f : FogProfile.FogHeight) * m;
                    shadowMultiplier = 1.0f - (FogProfile.fogDensity * 16.0f * h * FogShadowStrengthFactor);
                    intensityMultiplier = 1.0f - (FogProfile.fogDensity * 2.0f * h * p);
                    break;

                case WeatherMakerFogMode.Exponential:
                    h = (FogProfile.FogHeight < Mathf.Epsilon ? 1000.0f : FogProfile.FogHeight) * 2.0f * m;
                    shadowMultiplier = 1.0f - (Mathf.Min(1.0f, Mathf.Pow(FogProfile.fogDensity * 32.0f * h * FogShadowStrengthFactor, 0.5f)));
                    intensityMultiplier = 1.0f - (FogProfile.fogDensity * 4.0f * h * p);
                    break;

                case WeatherMakerFogMode.ExponentialSquared:
                    h = (FogProfile.FogHeight < Mathf.Epsilon ? 1000.0f : FogProfile.FogHeight) * 4.0f * m;
                    shadowMultiplier = 1.0f - (Mathf.Min(1.0f, Mathf.Pow(FogProfile.fogDensity * 64.0f * h * FogShadowStrengthFactor, 0.5f)));
                    intensityMultiplier = 1.0f - (FogProfile.fogDensity * 8.0f * h * p);
                    break;

                default:
                    shadowMultiplier = 1.0f;
                    intensityMultiplier = 1.0f;
                    break;
            }

            shadowMultiplier = Mathf.Clamp(shadowMultiplier * 1.3f, 0.2f, 1.0f);
            if (WeatherMakerLightManagerScript.ScreenSpaceShadowMode == BuiltinShaderMode.Disabled || QualitySettings.shadows == ShadowQuality.Disable)
            {
                WeatherMakerLightManagerScript.Instance.DirectionalLightIntensityMultipliers["WeatherMakerFullScreenFogScript"] = Mathf.Clamp(intensityMultiplier, 0.5f, 1.0f);
                WeatherMakerLightManagerScript.Instance.DirectionalLightShadowIntensityMultipliers["WeatherMakerFullScreenFogScript"] = shadowMultiplier;
            }
            else
            {
                WeatherMakerLightManagerScript.Instance.DirectionalLightIntensityMultipliers.Remove("WeatherMakerFullScreenFogScript");
                WeatherMakerLightManagerScript.Instance.DirectionalLightShadowIntensityMultipliers.Remove("WeatherMakerFullScreenFogScript");
            }
            Shader.SetGlobalFloat(WMS._WeatherMakerFogGlobalShadow, shadowMultiplier);
            updateShaderPropertiesAction = (updateShaderPropertiesAction ?? UpdateShaderProperties);
            effect.SetupEffect(FogMaterial, FogFullScreenMaterial, FogBlurMaterial, BlurShader, DownSampleScale,
                (FogProfile.SunShaftSampleCount <= 0 ? WeatherMakerDownsampleScale.Disabled : FogProfile.SunShaftDownSampleScale),
                WeatherMakerDownsampleScale.Disabled, TemporalReprojectionMaterial, TemporalReprojection, updateShaderPropertiesAction,
                (FogProfile.FogDensity > Mathf.Epsilon && FogProfile.FogMode != WeatherMakerFogMode.None && !UseUnityFog));
            UpdateWind();
            UpdateUnityFog(camera);
        }

        private void UpdateShaderProperties(WeatherMakerCommandBuffer b)
        {
            // temp turn off fog lights for reflection camera and save a lot of performance
            bool origFogLights = (WeatherMakerScript.Instance == null || WeatherMakerScript.Instance.PerformanceProfile.EnableFogLights);
            int origFogShafts = (WeatherMakerScript.Instance == null ? FogProfile.SunShaftSampleCount : WeatherMakerScript.Instance.PerformanceProfile.FogFullScreenSunShaftSampleCount);
            bool tempFogLights = origFogLights && (b.CameraType == WeatherMakerCameraType.Normal);
            int tempFogShafts = (b.CameraType == WeatherMakerCameraType.Normal ? origFogShafts : 0);
            float density = FogProfile.fogDensity;

            // TODO: See if this is really necessary anymore
            //if (b.CameraType == WeatherMakerCameraType.Reflection)
            //{
                // HACK: reflection camera hack, density is not correct but this compensates
                // TODO: figure out why fog formula is wrong in reflection camera
                //FogProfile.fogDensity = Mathf.Min(1.0f, density * 100.0f);
            //}

            if (WeatherMakerScript.Instance != null)
            {
                WeatherMakerScript.Instance.PerformanceProfile.EnableFogLights = tempFogLights;
                WeatherMakerScript.Instance.PerformanceProfile.FogFullScreenSunShaftSampleCount = tempFogShafts;
            }
            FogProfile.UpdateMaterialProperties(b.Material, b.Camera, true);
            FogProfile.fogDensity = density;
            if (WeatherMakerScript.Instance != null)
            {
                WeatherMakerScript.Instance.PerformanceProfile.EnableFogLights = origFogLights;
                WeatherMakerScript.Instance.PerformanceProfile.FogFullScreenSunShaftSampleCount = origFogShafts;
            }
        }

        protected override void Awake()
        {
            // create fog profile now, base.Start will also create it but uses a non-full screen profile default
            if (FogProfile == null)
            {
                FogProfile = Resources.Load<WeatherMakerFullScreenFogProfileScript>("WeatherMakerFullScreenFogProfile_Default");
            }

            base.Awake();

            effect = new WeatherMakerFullScreenEffect
            {
                CommandBufferName = commandBufferName,
                DownsampleRenderBufferTextureName = "_MainTex2",
                RenderQueue = FogRenderQueue
            };

            if (Application.isPlaying)
            {
                FogFullScreenMaterial = new Material(FogFullScreenMaterial);
                FogBlurMaterial = new Material(FogBlurMaterial);
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            WeatherMakerScript.EnsureInstance(this, ref instance);
            WeatherMakerCommandBufferManagerScript.Instance.RegisterPreCull(CameraPreCull, this);
            WeatherMakerCommandBufferManagerScript.Instance.RegisterPreRender(CameraPreRender, this);
            WeatherMakerCommandBufferManagerScript.Instance.RegisterPostRender(CameraPostRender, this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (effect != null)
            {
                effect.Dispose();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (effect != null)
            {
                effect.Dispose();
            }
            WeatherMakerCommandBufferManagerScript.Instance.UnregisterPreCull(this);
            WeatherMakerCommandBufferManagerScript.Instance.UnregisterPreRender(this);
            WeatherMakerCommandBufferManagerScript.Instance.UnregisterPostRender(this);
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        protected override void UpdateFogMaterialFromProfile()
        {
            // no need to call base class as we set material properties elsewhere
        }

        private void CameraPreCull(Camera camera)
        {
            if (effect != null && !WeatherMakerScript.ShouldIgnoreCamera(this, camera, !AllowReflections))
            {
                UpdateFogProperties(camera);
                effect.PreCullCamera(camera);
            }
        }

        private void CameraPreRender(Camera camera)
        {
            if (effect != null && !WeatherMakerScript.ShouldIgnoreCamera(this, camera, !AllowReflections))
            {
                effect.PreRenderCamera(camera);
            }
        }

        private void CameraPostRender(Camera camera)
        {
            if (effect != null && !WeatherMakerScript.ShouldIgnoreCamera(this, camera, !AllowReflections))
            {
                effect.PostRenderCamera(camera);
            }
        }

        private static WeatherMakerFullScreenFogScript instance;
        /// <summary>
        /// Shared instance of full screen fog script
        /// </summary>
        public static WeatherMakerFullScreenFogScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance); }
        }
    }
}