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
using System.Diagnostics;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Weather maker camera types
    /// </summary>
    public enum WeatherMakerCameraType
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal,

        /// <summary>
        /// Reflection (water, mirror, etc.)
        /// </summary>
        Reflection,

        /// <summary>
        /// Cube map (reflection probe, etc.)
        /// </summary>
        CubeMap,

        /// <summary>
        /// Pre-render or other camera, internal use, should generally be ignored
        /// </summary>
        Other
    }

    /// <summary>
    /// Represents a command buffer
    /// </summary>
    public class WeatherMakerCommandBuffer
    {
        /// <summary>
        /// Camera the command buffer is attached to
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// Render queue for the command buffer
        /// </summary>
        public CameraEvent RenderQueue;

        /// <summary>
        /// The command buffer
        /// </summary>
        public CommandBuffer CommandBuffer;

        /// <summary>
        /// A copy of the original material to render with, will be destroyed when command buffer is removed
        /// </summary>
        public Material Material;

        /// <summary>
        /// Reprojection state or null if none
        /// </summary>
        public WeatherMakerTemporalReprojectionState ReprojectionState;

        /// <summary>
        /// Whether the command buffer is a reflection
        /// </summary>
        public WeatherMakerCameraType CameraType { get; set; }

        /// <summary>
        /// Optional action to update material properties
        /// </summary>
        public System.Action<WeatherMakerCommandBuffer> UpdateMaterial;
    }

    /// <summary>
    /// Command buffer manager
    /// </summary>
    [ExecuteInEditMode]
    public class WeatherMakerCommandBufferManagerScript : MonoBehaviour
    {
        [Tooltip("Material to downsample the depth buffer")]
        public Material DownsampleDepthMaterial;

        /// <summary>
        /// Set this in OnWillRenderObject for the current reflection Vector
        /// </summary>
        public static Vector3? CurrentReflectionPlane;

        /// <summary>
        /// Set to any camera you are calling RenderToCubemap with, null out after the call to RenderToCubemap
        /// </summary>
        public static Camera CubemapCamera;

        private readonly List<WeatherMakerCommandBuffer> commandBuffers = new List<WeatherMakerCommandBuffer>();
        private readonly Matrix4x4[] inverseView = new Matrix4x4[2];
        private readonly Matrix4x4[] inverseProj = new Matrix4x4[2];
        private readonly List<KeyValuePair<System.Action<Camera>, MonoBehaviour>> preCullEvents = new List<KeyValuePair<System.Action<Camera>, MonoBehaviour>>();
        private readonly List<KeyValuePair<System.Action<Camera>, MonoBehaviour>> preRenderEvents = new List<KeyValuePair<System.Action<Camera>, MonoBehaviour>>();
        private readonly List<KeyValuePair<System.Action<Camera>, MonoBehaviour>> postRenderEvents = new List<KeyValuePair<System.Action<Camera>, MonoBehaviour>>();
        private readonly List<Camera> cameraStack = new List<Camera>();

        private const string afterForwardOpaqueCommandBufferName = "WeatherMakerAfterForwardOpaque";
        private const string depthCommandBufferName = "WeatherMakerDepthDownsample";
        //private CommandBuffer afterForwardOpaqueCommandBuffer;
        private CommandBuffer depthCommandBufferDeferred;
        private CommandBuffer depthCommandBufferForward;

#if COPY_FULL_DEPTH_TEXTURE

        private RenderTexture depthBuffer;

#endif

        //public RenderTexture AfterOpaqueBuffer { get; private set; }
        public RenderTexture HalfDepthBuffer { get; private set; }
        public RenderTexture QuarterDepthBuffer { get; private set; }
        public RenderTexture EighthDepthBuffer { get; private set; }
        public RenderTexture SixteenthDepthBuffer { get; private set; }

#if COPY_FULL_DEPTH_TEXTURE

        public RenderTargetIdentifier DepthBufferId { get; private set; }

#endif

        public RenderTargetIdentifier FullDepthBufferId { get; private set; }
        public RenderTargetIdentifier HalfDepthBufferId { get; private set; }
        public RenderTargetIdentifier QuarterDepthBufferId { get; private set; }
        public RenderTargetIdentifier EighthDepthBufferId { get; private set; }
        public RenderTargetIdentifier SixteenthDepthBufferId { get; private set; }

        /// <summary>
        /// Current camera stack count
        /// </summary>
        public static int CameraStack { get { return (Instance == null ? 0 : Instance.cameraStack.Count); } }

        private void UpdateDeferredShadingKeyword(Camera camera, Material material)
        {
            if (camera.actualRenderingPath == RenderingPath.DeferredShading)
            {
                if (!material.IsKeywordEnabled("WEATHER_MAKER_DEFERRED_SHADING"))
                {
                    material.EnableKeyword("WEATHER_MAKER_DEFERRED_SHADING");
                }
            }
            else if (material.IsKeywordEnabled("WEATHER_MAKER_DEFERRED_SHADING"))
            {
                material.DisableKeyword("WEATHER_MAKER_DEFERRED_SHADING");
            }
        }

        private void UpdateCommandBuffersForCamera(Camera camera)
        {
            if (camera == null || !enabled)
            {
                return;
            }

            camera.depthTextureMode |= DepthTextureMode.Depth;
            AttachDepthCommandBuffer(camera);
            CreateAfterForwardOpaqueCommandBuffer(camera);

            foreach (WeatherMakerCommandBuffer commandBuffer in commandBuffers)
            {
                if (commandBuffer == null || commandBuffer.Material == null || camera != commandBuffer.Camera || cameraStack.Count == 0)
                {
                    continue;
                }
                UpdateDeferredShadingKeyword(camera, commandBuffer.Material);
                if (camera == null)
                {
                    continue;
                }

                camera = commandBuffer.Camera;
                Camera baseCamera = cameraStack[0];
                if (camera.stereoEnabled)
                {
                    // see https://github.com/chriscummings100/worldspaceposteffect/blob/master/Assets/WorldSpacePostEffect/WorldSpacePostEffect.cs
                    inverseView[0] = camera.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
                    inverseView[1] = camera.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;

                    // only use base camera projection
                    inverseProj[0] = baseCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left).inverse;
                    inverseProj[1] = baseCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right).inverse;
                }
                else
                {
                    inverseView[0] = inverseView[1] = camera.worldToCameraMatrix.inverse;

                    // only use base camera projection
                    inverseProj[0] = inverseProj[1] = baseCamera.projectionMatrix.inverse;
                }
                Shader.SetGlobalMatrixArray(WMS._WeatherMakerInverseView, inverseView);
                Shader.SetGlobalMatrixArray(WMS._WeatherMakerInverseProj, inverseProj);
                if (commandBuffer.ReprojectionState != null)
                {
                    commandBuffer.ReprojectionState.PreRenderFrame(camera, baseCamera);
                }
                if (commandBuffer.ReprojectionState != null && !commandBuffer.ReprojectionState.NeedsFirstFrameHandling &&
                    commandBuffer.ReprojectionState.BlendMode == WeatherMakerTemporalReprojectionState.TemporalReprojectionBlendMode.Sharp)
                {
                    float xo = commandBuffer.ReprojectionState.TemporalOffsetX;
                    float yo = commandBuffer.ReprojectionState.TemporalOffsetY;
                    Shader.SetGlobalVector(WMS._WeatherMakerTemporalUV, new Vector4(xo, yo, 1.0f, 1.0f));
                }
                else
                {
                    Shader.SetGlobalVector(WMS._WeatherMakerTemporalUV, Vector4.zero);
                }
                if (commandBuffer.CameraType == WeatherMakerCameraType.CubeMap || commandBuffer.Camera == CubemapCamera)
                {
                    Shader.SetGlobalFloat(WMS._WeatherMakerCameraRenderMode, 2.0f);
                }
                else if (commandBuffer.CameraType == WeatherMakerCameraType.Reflection)
                {
                    Shader.SetGlobalFloat(WMS._WeatherMakerCameraRenderMode, 1.0f);
                }
                else
                {
                    Shader.SetGlobalFloat(WMS._WeatherMakerCameraRenderMode, 0.0f);
                }
                if (commandBuffer.UpdateMaterial != null) 
                {
                    commandBuffer.UpdateMaterial(commandBuffer);
                }
            }
        }

        private void CleanupCommandBuffer(WeatherMakerCommandBuffer commandBuffer)
        {
            if (commandBuffer == null)
            {
                return;
            }
            else if (commandBuffer.Material != null && commandBuffer.Material.name.IndexOf("(clone)", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                GameObject.DestroyImmediate(commandBuffer.Material);
            }
            if (commandBuffer.Camera != null)
            {
                commandBuffer.Camera.RemoveCommandBuffer(commandBuffer.RenderQueue, commandBuffer.CommandBuffer);
            }
            if (commandBuffer.CommandBuffer != null)
            {
                commandBuffer.CommandBuffer.Dispose();
            }
        }

        private void CleanupCameras()
        {
            // remove destroyed camera command buffers
            for (int i = commandBuffers.Count - 1; i >= 0; i--)
            {
                if (commandBuffers[i].Camera == null)
                {
                    CleanupCommandBuffer(commandBuffers[i]);
                    commandBuffers.RemoveAt(i);
                }
            }
        }

        private void RemoveAllCommandBuffers()
        {
            for (int i = commandBuffers.Count - 1; i >= 0; i--)
            {
                CleanupCommandBuffer(commandBuffers[i]);
            }
            commandBuffers.Clear();
        }

        private void Update()
        {
            UpdateDepthDownsampler();
        }

        private void LateUpdate()
        {
            CleanupCameras();
        }

        private void OnEnable()
        {
            // use pre-render to give all other pre-cull scripts a chance to set properties, state, etc.
            Camera.onPreCull += CameraPreCull;
            Camera.onPreRender += CameraPreRender;
            Camera.onPostRender += CameraPostRender;
            CleanupDepthTextures();
        }

        private void OnDisable()
        {
            // use pre-render to give all other pre-cull scripts a chance to set properties, state, etc.
            Camera.onPreCull -= CameraPreCull;
            Camera.onPreRender -= CameraPreRender;
            Camera.onPostRender -= CameraPostRender;
            CleanupDepthTextures();
        }

        private bool ListHasScript(List<KeyValuePair<System.Action<Camera>, MonoBehaviour>> list, MonoBehaviour script)
        {
            foreach (KeyValuePair<System.Action<Camera>, MonoBehaviour> item in list)
            {
                if (item.Value == script)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Register for pre cull events. Call from OnEnable.
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="script">Script</param>
        public void RegisterPreCull(System.Action<Camera> action, MonoBehaviour script)
        {
            if (script != null && !ListHasScript(preCullEvents, script))
            {
                preCullEvents.Add(new KeyValuePair<System.Action<Camera>, MonoBehaviour>(action, script));
            }
        }

        /// <summary>
        /// Unregister pre cull events. Call from OnDestroy.
        /// </summary>
        /// <param name="script">Script</param>
        public void UnregisterPreCull(MonoBehaviour script)
        {
            if (script != null)
            {
                for (int i = preCullEvents.Count - 1; i >= 0; i--)
                {
                    if (preCullEvents[i].Value == script)
                    {
                        preCullEvents.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Register pre render events. Call from OnEnable.
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="script">Script</param>
        /// <param name="highPriority">High priority go to front of the list, low to the back</param>
        public void RegisterPreRender(System.Action<Camera> action, MonoBehaviour script, bool highPriority = false)
        {
            if (script != null && !ListHasScript(preRenderEvents, script))
            {
                if (highPriority)
                {
                    preRenderEvents.Add(new KeyValuePair<System.Action<Camera>, MonoBehaviour>(action, script));
                }
                else
                {
                    preRenderEvents.Insert(0, new KeyValuePair<System.Action<Camera>, MonoBehaviour>(action, script));
                }
            }
        }

        /// <summary>
        /// Unregister pre render events. Call from OnDestroy.
        /// </summary>
        /// <param name="script">Script</param>
        public void UnregisterPreRender(MonoBehaviour script)
        {
            if (script != null)
            {
                for (int i = preRenderEvents.Count - 1; i >= 0; i--)
                {
                    if (preRenderEvents[i].Value == script)
                    {
                        preRenderEvents.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Register post render events. Call from OnEnable.
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="script">Script</param>
        public void RegisterPostRender(System.Action<Camera> action, MonoBehaviour script)
        {
            if (script != null && !ListHasScript(postRenderEvents, script))
            {
                postRenderEvents.Add(new KeyValuePair<System.Action<Camera>, MonoBehaviour>(action, script));
            }
        }

        /// <summary>
        /// Unregister post render events. Call from OnDestroy.
        /// </summary>
        /// <param name="script">Script</param>
        public void UnregisterPostRender(MonoBehaviour script)
        {
            if (script != null)
            {
                for (int i = postRenderEvents.Count - 1; i >= 0; i--)
                {
                    if (postRenderEvents[i].Value == script)
                    {
                        postRenderEvents.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Add a command buffer
        /// </summary>
        /// <param name="commandBuffer">Command buffer to add, the CommandBuffer property must have a unique name assigned</param>
        /// <returns>True if added, false if not</returns>
        public bool AddCommandBuffer(WeatherMakerCommandBuffer commandBuffer)
        {
            if (!enabled || commandBuffer == null || string.IsNullOrEmpty(commandBuffer.CommandBuffer.name))
            {
                return false;
            }
            RemoveCommandBuffer(commandBuffer.Camera, commandBuffer.CommandBuffer.name);
            commandBuffers.Add(commandBuffer);
            commandBuffer.Camera.AddCommandBuffer(commandBuffer.RenderQueue, commandBuffer.CommandBuffer);
            return true;
        }

        /// <summary>
        /// Remove a command buffer
        /// </summary>
        /// <param name="commandBuffer">Command buffer to remove</param>
        /// <returns>True if removed, false if not</returns>
        public bool RemoveCommandBuffer(WeatherMakerCommandBuffer commandBuffer)
        {
            if (commandBuffer == null)
            {
                return false;
            }
            int index = commandBuffers.IndexOf(commandBuffer);
            if (index >= 0)
            {
                CleanupCommandBuffer(commandBuffers[index]);
                commandBuffers.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a command buffer
        /// </summary>
        /// <param name="camera">Camera to remove command buffer on</param>
        /// <param name="name">Name of the command buffer to remove</param>
        /// <returns>True if removed, false if not</returns>
        public bool RemoveCommandBuffer(Camera camera, string name)
        {
            if (camera == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            for (int i = 0; i < commandBuffers.Count; i++)
            {
                if (commandBuffers[i].Camera == camera && commandBuffers[i].CommandBuffer.name == name)
                {
                    CleanupCommandBuffer(commandBuffers[i]);
                    commandBuffers.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove all command buffers with a specified name
        /// </summary>
        /// <param name="name">Name of the command buffers to remove</param>
        /// <returns>True if at least one command buffer removed, false otherwise</returns>
        public bool RemoveCommandBuffers(string name)
        {
            bool foundOne = false;
            for (int i = commandBuffers.Count - 1; i >= 0; i--)
            {
                if (commandBuffers[i].CommandBuffer.name == name)
                {
                    CleanupCommandBuffer(commandBuffers[i]);
                    commandBuffers.RemoveAt(i);
                    foundOne = true;
                }
            }
            return foundOne;
        }

        /// <summary>
        /// Checks for existance of a command buffer
        /// </summary>
        /// <param name="commandBuffer">Command buffer to check for</param>
        /// <returns>True if exists, false if not</returns>
        public bool ContainsCommandBuffer(WeatherMakerCommandBuffer commandBuffer)
        {
            if (commandBuffer == null || commandBuffer.Camera == null)
            {
                return false;
            }
            foreach (CommandBuffer cameraCommandBuffer in commandBuffer.Camera.GetCommandBuffers(commandBuffer.RenderQueue))
            {
                if (commandBuffer.CommandBuffer == cameraCommandBuffer)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks for existance of a command buffer by camera and name
        /// </summary>
        /// <param name="camera">Camera to check for</param>
        /// <param name="renderQueue">Camera event to check for</param>
        /// <param name="name">Name to check for</param>
        /// <returns>True if exists, false if not</returns>
        public bool ContainsCommandBuffer(Camera camera, CameraEvent renderQueue, string name)
        {
            if (camera == null || string.IsNullOrEmpty(name))
            {
                return false;
            }
            foreach (CommandBuffer cameraCommandBuffer in camera.GetCommandBuffers(renderQueue))
            {
                if (cameraCommandBuffer.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get a command buffer
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="renderQueue">Render queue</param>
        /// <param name="name">Name</param>
        /// <returns>Found command buffer or null if not found</returns>
        public WeatherMakerCommandBuffer GetCommandBuffer(Camera camera, CameraEvent renderQueue, string name)
        {
            if (camera == null || string.IsNullOrEmpty(name))
            {
                return null;
            }
            foreach (WeatherMakerCommandBuffer buffer in commandBuffers)
            {
                if (buffer.Camera == camera && buffer.RenderQueue == renderQueue && buffer.CommandBuffer.name == name)
                {
                    return buffer;
                }
            }
            return null;
        }

        private void CreateAfterForwardOpaqueCommandBuffer(Camera camera)
        {
            /*
            if (afterForwardOpaqueCommandBuffer != null)
            {
                camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, afterForwardOpaqueCommandBuffer);
                camera.RemoveCommandBuffer(CameraEvent.AfterSkybox, afterForwardOpaqueCommandBuffer);
            }
            if (afterForwardOpaqueCommandBuffer == null || (AfterOpaqueBuffer != null && (AfterOpaqueBuffer.width != camera.pixelWidth || AfterOpaqueBuffer.height != camera.pixelHeight)))
            {
                if (AfterOpaqueBuffer != null)
                {
                    AfterOpaqueBuffer.Release();
                    Destroy(AfterOpaqueBuffer);
                    AfterOpaqueBuffer = WeatherMakerFullScreenEffect.CreateRenderTexture(WeatherMakerFullScreenEffect.GetRenderTextureDescriptor(1, 1, 1, RenderTextureFormat.DefaultHDR, 0, camera));
                }
                afterForwardOpaqueCommandBuffer = new CommandBuffer { name = afterForwardOpaqueCommandBufferName + Time.unscaledDeltaTime };
                afterForwardOpaqueCommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, AfterOpaqueBuffer);
                afterForwardOpaqueCommandBuffer.SetGlobalTexture(WMS._WeatherMakerAfterForwardOpaqueTexture, AfterOpaqueBuffer);
            }
            if (camera.clearFlags == CameraClearFlags.Skybox)
            {
                camera.AddCommandBuffer(CameraEvent.AfterSkybox, afterForwardOpaqueCommandBuffer);
            }
            else
            {
                camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, afterForwardOpaqueCommandBuffer);
            }
            */
        }

        private void CreateAndAddDepthCommandBuffer(Camera camera)
        {
            if (HalfDepthBuffer == null)
            {
                return;
            }

            bool deferred = (camera.actualRenderingPath == RenderingPath.DeferredLighting || camera.actualRenderingPath == RenderingPath.DeferredShading);
            CommandBuffer depthCommandBuffer = (deferred ? depthCommandBufferDeferred : depthCommandBufferForward);

            // create depth downsampler
            if (depthCommandBuffer == null)
            {
                depthCommandBuffer = new CommandBuffer { name = depthCommandBufferName + Time.unscaledTime };
                if (deferred)
                {
                    depthCommandBufferDeferred = depthCommandBuffer;
                    if (UnityEngine.XR.XRDevice.isPresent)
                    {
                        // bug in VR, deferred depth texture not set
                        depthCommandBuffer.SetGlobalTexture(WMS._CameraDepthTexture, BuiltinRenderTextureType.ResolvedDepth);
                    }
                }
                else
                {
                    depthCommandBufferForward = depthCommandBuffer;
                }

#if COPY_FULL_DEPTH_TEXTURE

                depthCommandBuffer.SetGlobalFloat(WMS._DownsampleDepthScale, 1.0f);
                depthCommandBuffer.Blit(HalfDepthBufferId, DepthBufferId, DownsampleDepthMaterial, 0);

#endif

                depthCommandBuffer.SetGlobalFloat(WMS._DownsampleDepthScale, 2.0f);
                depthCommandBuffer.Blit(QuarterDepthBufferId, HalfDepthBufferId, DownsampleDepthMaterial, 1);
                depthCommandBuffer.SetGlobalFloat(WMS._DownsampleDepthScale, 4.0f);
                depthCommandBuffer.Blit(HalfDepthBufferId, QuarterDepthBufferId, DownsampleDepthMaterial, 2);
                depthCommandBuffer.SetGlobalFloat(WMS._DownsampleDepthScale, 8.0f);
                depthCommandBuffer.Blit(QuarterDepthBufferId, EighthDepthBufferId, DownsampleDepthMaterial, 3);
                depthCommandBuffer.SetGlobalFloat(WMS._DownsampleDepthScale, 16.0f);
                depthCommandBuffer.Blit(EighthDepthBufferId, SixteenthDepthBufferId, DownsampleDepthMaterial, 4);
            }

            CommandBuffer[] commandBuffersForward = camera.GetCommandBuffers(CameraEvent.AfterDepthTexture);
            CommandBuffer[] commandBuffersDeferred = camera.GetCommandBuffers(CameraEvent.BeforeReflections);
            bool needsCommandBuffer = true;
            foreach (CommandBuffer buffer in commandBuffersForward)
            {
                if (buffer.name.StartsWith(depthCommandBufferName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (buffer.name != depthCommandBuffer.name)
                    {
                        camera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, buffer);
                    }
                    else
                    {
                        needsCommandBuffer = false;
                    }
                }
            }
            foreach (CommandBuffer buffer in commandBuffersDeferred)
            {
                if (buffer.name.StartsWith(depthCommandBufferName, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (buffer.name != depthCommandBuffer.name)
                    {
                        camera.RemoveCommandBuffer(CameraEvent.BeforeReflections, buffer);
                    }
                    else
                    {
                        needsCommandBuffer = false;
                    }
                }
            }
            if (needsCommandBuffer)
            {
                if (deferred)
                {
                    camera.AddCommandBuffer(CameraEvent.BeforeReflections, depthCommandBuffer);
                }
                else
                {
                    camera.AddCommandBuffer(CameraEvent.AfterDepthTexture, depthCommandBuffer);
                }
            }
        }

        private RenderTargetIdentifier UpdateDepthDownsampler(ref RenderTexture tex, RenderTargetIdentifier id, int scale)
        {
            RenderTextureDescriptor desc = WeatherMakerFullScreenEffect.GetRenderTextureDescriptor(scale, 0, 1, RenderTextureFormat.RFloat);
            if (tex == null || tex.width != desc.width)
            {
                WeatherMakerFullScreenEffect.DestroyRenderTexture(ref tex);
                tex = WeatherMakerFullScreenEffect.CreateRenderTexture(desc, FilterMode.Point, TextureWrapMode.Clamp);
                tex.name = "WeatherMakerDepthTexture_" + scale;
                id = new RenderTargetIdentifier(tex);
                WeatherMakerFullScreenEffect.ReleaseCommandBuffer(ref depthCommandBufferDeferred);
                WeatherMakerFullScreenEffect.ReleaseCommandBuffer(ref depthCommandBufferForward);
            }

            return id;
        }

        private void UpdateDepthDownsampler()
        {
            if (DownsampleDepthMaterial != null)
            {

#if COPY_FULL_DEPTH_TEXTURE

                DepthBufferId = UpdateDepthDownsampler(ref depthBuffer, DepthBufferId, 1);

#endif

                RenderTexture tmp = HalfDepthBuffer;
                HalfDepthBufferId = UpdateDepthDownsampler(ref tmp, HalfDepthBufferId, 2);
                HalfDepthBuffer = tmp;
                tmp = QuarterDepthBuffer;
                QuarterDepthBufferId = UpdateDepthDownsampler(ref tmp, QuarterDepthBufferId, 4);
                QuarterDepthBuffer = tmp;
                tmp = EighthDepthBuffer;
                EighthDepthBufferId = UpdateDepthDownsampler(ref tmp, EighthDepthBufferId, 8);
                EighthDepthBuffer = tmp;
                tmp = SixteenthDepthBuffer;
                SixteenthDepthBufferId = UpdateDepthDownsampler(ref tmp, SixteenthDepthBufferId, 16);
                SixteenthDepthBuffer = tmp;

#if COPY_FULL_DEPTH_TEXTURE

                Shader.SetGlobalTexture(WMS._CameraDepthTextureOne, depthBuffer);

#endif

                Shader.SetGlobalTexture(WMS._CameraDepthTextureHalf, HalfDepthBuffer);
                Shader.SetGlobalTexture(WMS._CameraDepthTextureQuarter, QuarterDepthBuffer);
                Shader.SetGlobalTexture(WMS._CameraDepthTextureEighth, EighthDepthBuffer);
                Shader.SetGlobalTexture(WMS._CameraDepthTextureSixteenth, SixteenthDepthBuffer);
            }
        }

        private void CleanupDepthTextures()
        {
            HalfDepthBuffer = WeatherMakerFullScreenEffect.DestroyRenderTexture(HalfDepthBuffer);
            QuarterDepthBuffer = WeatherMakerFullScreenEffect.DestroyRenderTexture(QuarterDepthBuffer);
            EighthDepthBuffer = WeatherMakerFullScreenEffect.DestroyRenderTexture(EighthDepthBuffer);
            SixteenthDepthBuffer = WeatherMakerFullScreenEffect.DestroyRenderTexture(SixteenthDepthBuffer);
            WeatherMakerFullScreenEffect.ReleaseCommandBuffer(ref depthCommandBufferDeferred);
            WeatherMakerFullScreenEffect.ReleaseCommandBuffer(ref depthCommandBufferForward);
        }

        private void AttachDepthCommandBuffer(Camera camera)
        {
            if (DownsampleDepthMaterial != null && !WeatherMakerScript.ShouldIgnoreCamera(this, camera, false))
            {
                CreateAndAddDepthCommandBuffer(camera);
            }
        }

        private void InvokeEvents(Camera camera, List<KeyValuePair<System.Action<Camera>, MonoBehaviour>> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Value == null)
                {
                    list.RemoveAt(i);
                }
                else if (list[i].Value.enabled)
                {
                    list[i].Key(camera);
                }
            }
        }

        private void CameraPreCull(Camera camera)
        {
            // avoid infinite loop
            if (cameraStack.Contains(camera))
            {
                return;
            }

            cameraStack.Add(camera);
            InvokeEvents(camera, preCullEvents);
        }

        private void CameraPreRender(Camera camera)
        {
            if (!WeatherMakerScript.ShouldIgnoreCamera(this, camera, false))
            {
                UpdateCommandBuffersForCamera(camera);
            }

            InvokeEvents(camera, preRenderEvents);
        }

        private void CameraPostRender(Camera camera)
        {
            cameraStack.Remove(camera);
            InvokeEvents(camera, postRenderEvents);
        }

        private static WeatherMakerCommandBufferManagerScript instance;
        /// <summary>
        /// Shared instance of weather maker manager script
        /// </summary>
        public static WeatherMakerCommandBufferManagerScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance<WeatherMakerCommandBufferManagerScript>(ref instance, true); }
        }
    }
}