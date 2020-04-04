using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_LWRP

using UnityEngine.Rendering.LWRP;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerLWRPRenderFeatureScript : UnityEngine.Rendering.LWRP.ScriptableRendererFeature
    {
        private class ExecuteCommandBuffersPass : ScriptableRenderPass
        {
            private readonly CameraEvent cameraEvent;

            public ExecuteCommandBuffersPass(CameraEvent evt) : base()
            {
                cameraEvent = evt;

                switch (evt)
                {
                    case CameraEvent.AfterDepthNormalsTexture:
                    case CameraEvent.AfterDepthTexture:
                        renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
                        break;

                    case CameraEvent.AfterEverything:
                    case CameraEvent.AfterFinalPass:
                    case CameraEvent.AfterHaloAndLensFlares:
                        renderPassEvent = RenderPassEvent.AfterRendering;
                        break;

                    case CameraEvent.AfterForwardAlpha:
                    case CameraEvent.BeforeFinalPass:
                    case CameraEvent.BeforeHaloAndLensFlares:
                    case CameraEvent.BeforeImageEffects:
                        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
                        break;

                    case CameraEvent.AfterForwardOpaque:
                    case CameraEvent.BeforeImageEffectsOpaque:
                        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
                        break;

                    case CameraEvent.AfterGBuffer:
                        // TODO: Change when LWRP deferred is added
                        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
                        break;

                    case CameraEvent.AfterImageEffects:
                    case CameraEvent.AfterImageEffectsOpaque:
                        renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
                        break;

                    case CameraEvent.AfterLighting:
                    case CameraEvent.AfterReflections:
                        renderPassEvent = RenderPassEvent.AfterRenderingShadows;
                        break;

                    case CameraEvent.AfterSkybox:
                        renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
                        break;

                    case CameraEvent.BeforeDepthNormalsTexture:
                    case CameraEvent.BeforeDepthTexture:
                        renderPassEvent = RenderPassEvent.BeforeRenderingPrepasses;
                        break;

                    case CameraEvent.BeforeForwardAlpha:
                        renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
                        break;

                    case CameraEvent.BeforeForwardOpaque:
                        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
                        break;

                    case CameraEvent.BeforeGBuffer:
                        // TODO: Change when LWRP deferred is added
                        renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
                        break;

                    case CameraEvent.BeforeLighting:
                    case CameraEvent.BeforeReflections:
                        renderPassEvent = RenderPassEvent.BeforeRenderingShadows;
                        break;

                    case CameraEvent.BeforeSkybox:
                        renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
                        break;
                }
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer[] cmds = renderingData.cameraData.camera.GetCommandBuffers(cameraEvent);
                foreach (CommandBuffer cmd in cmds)
                {
                    context.ExecuteCommandBuffer(cmd);
                }
            }
        }

        private readonly List<ExecuteCommandBuffersPass> commandBufferPasses = new List<ExecuteCommandBuffersPass>();

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            foreach (ExecuteCommandBuffersPass pass in commandBufferPasses)
            {
                renderer.EnqueuePass(pass);
            }
        }

        public override void Create()
        {
            foreach (var e in System.Enum.GetValues(typeof(CameraEvent)))
            {
                commandBufferPasses.Add(new ExecuteCommandBuffersPass((CameraEvent)e));
            }
        }
    }
}

#endif
