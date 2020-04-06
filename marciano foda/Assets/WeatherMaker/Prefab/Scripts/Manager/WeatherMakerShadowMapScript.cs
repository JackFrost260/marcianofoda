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
    [RequireComponent(typeof(Light))]
    [ExecuteInEditMode]
    public class WeatherMakerShadowMapScript : MonoBehaviour
    {
        [Tooltip("The texture name for shaders to access the cascaded shadow map, null/empty for none.")]
        public string ShaderTextureName = "_WeatherMakerShadowMapTexture";

        [Tooltip("Optional material to add cloud shadows to the shadow map, null for no cloud shadows.")]
        public Material CloudShadowMaterial;

        private Light _light;
        private CommandBuffer _commandBufferDepthShadows;
        private CommandBuffer _commandBufferScreenSpaceShadows;

        private void RemoveCommandBuffer(LightEvent evt, ref CommandBuffer commandBuffer)
        {
            if (_light != null && commandBuffer != null)
            {
                // putting these in try/catch as Unity 2018.3 throws weird errors
                try
                {
                    _light.RemoveCommandBuffer(evt, commandBuffer);
                    commandBuffer.Release();
                }
                catch
                {
                    // eat exceptions
                }
                commandBuffer = null;
            }
        }

        private void CleanupCommandBuffers()
        {
            RemoveCommandBuffer(LightEvent.AfterShadowMap, ref _commandBufferDepthShadows);
            RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, ref _commandBufferScreenSpaceShadows);
        }

        private void AddShadowMapCommandBuffer()
        {
            if (_light != null && _light.shadows != LightShadows.None && _commandBufferDepthShadows == null && !string.IsNullOrEmpty(ShaderTextureName))
            {
                _commandBufferDepthShadows = new CommandBuffer { name = "WeatherMakerShadowMapDepthShadowScript_" + gameObject.name };
                //if (CloudShadowMaterial != null)
                {
                    // TODO: update shadow map with cloud shadows, not working, needs more research
                    //_commandBufferDepthShadows.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CurrentActive, CloudShadowMaterial, 1);
                }
                _commandBufferDepthShadows.SetGlobalTexture(ShaderTextureName, BuiltinRenderTextureType.CurrentActive);
                _light.AddCommandBuffer(LightEvent.AfterShadowMap, _commandBufferDepthShadows);
            }
        }

        private void AddScreenSpaceShadowsCommandBuffer()
        {
            if (CloudShadowMaterial != null && _light != null && _light.type == LightType.Directional &&
                _light.shadows != LightShadows.None && WeatherMakerLightManagerScript.Instance != null &&
                WeatherMakerLightManagerScript.ScreenSpaceShadowMode != BuiltinShaderMode.Disabled)
            {
                if (_commandBufferScreenSpaceShadows == null)
                {
                    // copy the screen space shadow texture for re-use later
                    _commandBufferScreenSpaceShadows = new CommandBuffer { name = "WeatherMakerShadowMapScreensSpaceShadowScript_" + gameObject.name };

                    // on XR, we have to set the shader directly in graphics settings due to Unity bugs
                    if (!UnityEngine.XR.XRDevice.isPresent)
                    {
                        _commandBufferScreenSpaceShadows.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive, CloudShadowMaterial, 0);
                    }

                    _commandBufferScreenSpaceShadows.SetGlobalTexture(WeatherMakerLightManagerScript.Instance.ScreenSpaceShadowsRenderTextureName, BuiltinRenderTextureType.CurrentActive);
                    _light.AddCommandBuffer(LightEvent.AfterScreenspaceMask, _commandBufferScreenSpaceShadows);
                }
            }
            else
            {
                // remove shadow command buffer
                RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, ref _commandBufferScreenSpaceShadows);
            }
        }

        private void CreateCommandBuffers()
        {
            CleanupCommandBuffers();
            AddShadowMapCommandBuffer();
            AddScreenSpaceShadowsCommandBuffer();
        }

        internal void Reset()
        {
            CleanupCommandBuffers();
        }

        private void Update()
        {
            if (WeatherMakerScript.Instance == null || !WeatherMakerScript.Instance.PerformanceProfile.EnableVolumetricClouds)
            {
                // remove shadow command buffer
                RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, ref _commandBufferScreenSpaceShadows);
            }
            else
            {
                AddScreenSpaceShadowsCommandBuffer();
            }
        }

        private void LateUpdate()
        {
            // ensure that the any shader using cloud shadows knows the correct cloud shadow parameters
            if (CloudShadowMaterial != null)
            {
                Shader.SetGlobalFloat(WMS._CloudShadowMapAdder, CloudShadowMaterial.GetFloat(WMS._CloudShadowMapAdder));
                Shader.SetGlobalFloat(WMS._CloudShadowMapMultiplier, CloudShadowMaterial.GetFloat(WMS._CloudShadowMapMultiplier));
                Shader.SetGlobalFloat(WMS._CloudShadowMapMinimum, CloudShadowMaterial.GetFloat(WMS._CloudShadowMapMinimum));
                Shader.SetGlobalFloat(WMS._CloudShadowMapMaximum, CloudShadowMaterial.GetFloat(WMS._CloudShadowMapMaximum));
                Shader.SetGlobalFloat(WMS._CloudShadowMapPower, CloudShadowMaterial.GetFloat(WMS._CloudShadowMapPower));
            }
        }

        private void OnEnable()
        {
            _light = GetComponent<Light>();
            CreateCommandBuffers();
        }

        private void OnDisable()
        {
            CleanupCommandBuffers();
        }
    }
}