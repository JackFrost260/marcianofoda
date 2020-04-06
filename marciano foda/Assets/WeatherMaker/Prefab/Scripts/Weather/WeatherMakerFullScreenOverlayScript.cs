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
    [ExecuteInEditMode]
    public class WeatherMakerFullScreenOverlayScript<TProfile> : MonoBehaviour where TProfile : WeatherMakerOverlayProfileScriptBase
    {
        [Header("Overlay - profile")]
        [Tooltip("Overlay profile")]
        public TProfile OverlayProfile;

        [Header("Overlay - rendering")]
        [Tooltip("Down sample scale.")]
        public WeatherMakerDownsampleScale DownSampleScale = WeatherMakerDownsampleScale.FullResolution;

        [Tooltip("Material that renders the overlay effect")]
        public Material OverlayMaterial;

        [Tooltip("Material to render the overlay full screen alpha in a second pass if needed")]
        public Material OverlayAlphaMaterial;

        [Tooltip("Overlay blur Material.")]
        public Material OverlayBlurMaterial;

        [Tooltip("Overlay blur Shader Type.")]
        public BlurShaderType BlurShader;

        [Tooltip("Render overlay in this render queue for the command buffer.")]
        public CameraEvent OverlayRenderQueue = CameraEvent.BeforeImageEffectsOpaque;

        [Tooltip("External intensity function")]
        public WeatherMakerOutputParameterEventFloat ExternalIntensityFunction;

        [Tooltip("Whether to render overlay in reflection cameras.")]
        public bool AllowReflections = true;

        public WeatherMakerFullScreenEffect Effect { get; private set; }

        private readonly WeatherMakerOutputParameterFloat param = new WeatherMakerOutputParameterFloat();
        private System.Action<WeatherMakerCommandBuffer> updateShaderPropertiesAction;

        protected string CommandBufferName = "WeatherMakerFullScreenOverlayScript";

        private void CleanupEffect()
        {
            if (Effect != null)
            {
                Effect.Dispose();
                Effect = null;
            }
        }

        private void UpdateEffectProperties()
        {
            if (Effect == null) 
            {
                Effect = new WeatherMakerFullScreenEffect
                {
                    CommandBufferName = this.CommandBufferName,
                    DownsampleRenderBufferTextureName = "_MainTex2",
                    RenderQueue = OverlayRenderQueue
                };
            }
            SetupEffect(Effect);
            bool showOverlay = (OverlayProfile != null && !OverlayProfile.Disabled && (OverlayProfile.OverlayIntensity > 0.0001f || OverlayProfile.AutoIntensityMultiplier != 0.0f || OverlayProfile.OverlayMinimumIntensity > 0.0001f) && OverlayProfile.OverlayColor.a > 0.0f);
            if (showOverlay)
            {
                OverlayProfile.Update();
            }
            updateShaderPropertiesAction = (updateShaderPropertiesAction ?? UpdateShaderProperties);
            Effect.SetupEffect(OverlayMaterial, OverlayAlphaMaterial, OverlayBlurMaterial, BlurShader, DownSampleScale, WeatherMakerDownsampleScale.Disabled, WeatherMakerDownsampleScale.Disabled,
                null, WeatherMakerTemporalReprojectionSize.None, updateShaderPropertiesAction, showOverlay);
        }

        private float ExternalIntensityFunctionImpl()
        {
            ExternalIntensityFunction.Invoke(param);
            return param.Value;
        }

        protected virtual void SetupEffect(WeatherMakerFullScreenEffect effect)
        {
        }

        protected virtual void UpdateShaderProperties(WeatherMakerCommandBuffer b)
        {
            if (OverlayProfile != null && !OverlayProfile.Disabled)
            {
                if (ExternalIntensityFunction == null)
                {
                    OverlayProfile.ExternalIntensityFunction = null;
                }
                else
                {
                    OverlayProfile.ExternalIntensityFunction = ExternalIntensityFunctionImpl;
                }
                OverlayProfile.UpdateMaterial(b.Material);
            }
        }

        protected virtual void Update()
        {
        }

        private void LateUpdate()
        {
            UpdateEffectProperties();
        }

        protected virtual void OnEnable()
        {
            // clone profile to prevent accidental modification
            if (Application.isPlaying && OverlayProfile != null)
            {
                OverlayProfile = ScriptableObject.Instantiate(OverlayProfile) as TProfile;
            }
            CleanupEffect();
            WeatherMakerCommandBufferManagerScript.Instance.RegisterPreCull(CameraPreCull, this);
            WeatherMakerCommandBufferManagerScript.Instance.RegisterPreRender(CameraPreRender, this);
            WeatherMakerCommandBufferManagerScript.Instance.RegisterPostRender(CameraPostRender, this);
        }

        private void OnDisable()
        {
            CleanupEffect();
        }

        protected virtual void OnDestroy()
        {
            CleanupEffect();
            WeatherMakerCommandBufferManagerScript.Instance.UnregisterPreCull(this);
            WeatherMakerCommandBufferManagerScript.Instance.UnregisterPreRender(this);
            WeatherMakerCommandBufferManagerScript.Instance.UnregisterPostRender(this);
        }

        protected virtual void CameraPreCull(Camera camera)
        {
            if (Effect != null && !WeatherMakerScript.ShouldIgnoreCamera(this, camera, !AllowReflections))
            {
                if (Effect.Enabled && (camera.actualRenderingPath == RenderingPath.Forward || camera.actualRenderingPath == RenderingPath.VertexLit))
                {

#if UNITY_EDITOR

                    if (WeatherMakerScript.GetCameraType(camera) == WeatherMakerCameraType.Normal)
                    {
                        Debug.LogWarning("Full screen overlay works best with deferred shading");
                    }

#endif

                    camera.depthTextureMode |= DepthTextureMode.DepthNormals;
                }
                else
                {
                    camera.depthTextureMode &= (~DepthTextureMode.DepthNormals);
                }
                Effect.PreCullCamera(camera);
            }
        }

        protected virtual void CameraPreRender(Camera camera)
        {
            if (Effect != null && !WeatherMakerScript.ShouldIgnoreCamera(this, camera, !AllowReflections))
            {
                Effect.PreRenderCamera(camera);
            }
        }

        protected virtual void CameraPostRender(Camera camera)
        {
            if (Effect != null && !WeatherMakerScript.ShouldIgnoreCamera(this, camera, !AllowReflections))
            {
                Effect.PostRenderCamera(camera);
            }
        }
    }
}
