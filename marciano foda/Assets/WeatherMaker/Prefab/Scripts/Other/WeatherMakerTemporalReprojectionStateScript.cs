using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerTemporalReprojectionState : System.IDisposable
    {
        /// <summary>
        /// Types of temporal reprojection blend modes
        /// </summary>
        public enum TemporalReprojectionBlendMode
        {
            /// <summary>
            /// Blur smartly with neighbor pixels, this reduces image quality but eliminates artifacts with fast changing graphics or camera
            /// </summary>
            Blur = 0,

            /// <summary>
            /// Render every pixel with a jittered camera frustum. This produces a sharper, higher quality image, but can introduce artifacts
            /// if the graphics or camera move fast. The shader attempts to work-around most of this by checking changing light conditions
            /// and doing neighborhood pixel clamping.
            /// </summary>
            Sharp = 1
        }

        // private int debugCount;
        private int frameIndex;
        private int[] frameNumbers;

        public int ReprojectionSize { get; private set; }
        public int ReprojectionSizeSquared { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public int SubFrameWidth { get; private set; }
        public int SubFrameHeight { get; private set; }
        public int SubFrameNumber { get; private set; }
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }
        public float OffsetXConstant { get; private set; }
        public float OffsetYConstant { get; private set; }
        public float OffsetXConstant2 { get; private set; }
        public float OffsetYConstant2 { get; private set; }
        public float TemporalOffsetX { get; private set; }
        public float TemporalOffsetY { get; private set; }
        public bool NeedsFirstFrameHandling { get; set; }

        /// <summary>
        /// Integrated temporal reprojection combines a full shader with temporal reprojection in an attempt to reduce artifacts, especially at higher
        /// temporal reprojection sizes.
        /// When IntegratedTemporalReprojection is false, temporal reprojection renders in a separate upscaling pass.
        /// </summary>
        public bool IntegratedTemporalReprojection { get; set; }

        public WeatherMakerCommandBuffer CommandBuffer { get; set; }

        public RenderTexture SubFrameTexture { get { return subFrameCurrent; } }
        public RenderTexture PrevFrameTexture { get { return prevFrameCurrent; } }
        public Camera Camera { get; private set; }
        public Material TemporalReprojectionMaterial { get; private set; }
        public TemporalReprojectionBlendMode BlendMode { get; private set; }

        private RenderTexture subFrameCurrent;
        private RenderTexture prevFrameCurrent;

        private readonly Matrix4x4[] projection = new Matrix4x4[2];
        private readonly Matrix4x4[] view = new Matrix4x4[2];
        private readonly Matrix4x4[] inverseProjection = new Matrix4x4[2];
        private readonly Matrix4x4[] inverseProjectionView = new Matrix4x4[2];
        private readonly Matrix4x4[] inverseView = new Matrix4x4[2];
        private readonly Matrix4x4[] prevView = new Matrix4x4[2];
        private readonly Matrix4x4[] prevViewProjection = new Matrix4x4[2];
        private readonly Matrix4x4[] ipivpvp = new Matrix4x4[2];

        private void DisposeRenderTextures()
        {
            WeatherMakerFullScreenEffect.DestroyRenderTexture(ref subFrameCurrent);
            WeatherMakerFullScreenEffect.DestroyRenderTexture(ref prevFrameCurrent);
        }

        private void CreateFrameNumbers()
        {
            int i = 0;
            frameNumbers = new int[ReprojectionSizeSquared];

            for (i = 0; i < ReprojectionSizeSquared; i++)
            {
                frameNumbers[i] = i;
            }

            // generate array of unique random numbers in range in random order, no duplicates
            // https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net
            while (i-- > 0)
            {
                int k = frameNumbers[i];
                int j = (int)(Random.value * 1000.0f) % ReprojectionSizeSquared;
                frameNumbers[i] = frameNumbers[j];
                frameNumbers[j] = k;
            }

            SubFrameNumber = 0;
            frameIndex = 0;
        }

        public WeatherMakerTemporalReprojectionState(Camera camera, Material reprojMaterial, bool integratedTemporalReprojection)
        {
            Camera = camera;
            TemporalReprojectionMaterial = (reprojMaterial == null ? null : (Application.isPlaying ? new Material(reprojMaterial) : reprojMaterial));
            IntegratedTemporalReprojection = integratedTemporalReprojection;
            if (TemporalReprojectionMaterial != null && Application.isPlaying)
            {
                TemporalReprojectionMaterial.name += " (Clone)";
                BlendMode = (TemporalReprojectionBlendMode)TemporalReprojectionMaterial.GetInt(WMS._TemporalReprojection_BlendMode);
            }
        }

        private void ComputeTemporalOffsets()
        {
            // offset value for projection matrix
            XOffset = (SubFrameNumber % ReprojectionSize);
            YOffset = (SubFrameNumber / ReprojectionSize);
            TemporalOffsetX = ((float)XOffset * OffsetXConstant) + OffsetXConstant2;
            TemporalOffsetY = ((float)YOffset * OffsetYConstant) + OffsetYConstant2;
        }

        public void Dispose()
        {
            DisposeRenderTextures();
            if (TemporalReprojectionMaterial != null && TemporalReprojectionMaterial.name.IndexOf("(Clone)") >= 0)
            {
                GameObject.DestroyImmediate(TemporalReprojectionMaterial);
            }
        }

        public void PreCullFrame(Camera camera, WeatherMakerDownsampleScale scale, WeatherMakerTemporalReprojectionSize projSize)
        {
            if (TemporalReprojectionMaterial == null)
            {
                return;
            }
            WeatherMakerCameraType cameraType = WeatherMakerScript.GetCameraType(camera);
            if (cameraType == WeatherMakerCameraType.Other)
            {
                return;
            }
            int projSizeInt = (int)projSize;
            int downsampling = (int)scale;
            int frameWidth = camera.pixelWidth / downsampling;
            int frameHeight = camera.pixelHeight / downsampling;

            // ensure reprojection fits cleanly into frame
            while (frameWidth % projSizeInt != 0) { frameWidth++; }
            while (frameHeight % projSizeInt != 0) { frameHeight++; }

            if (frameWidth != FrameWidth || frameHeight != FrameHeight || ReprojectionSize != projSizeInt)
            {
                DisposeRenderTextures();
                ReprojectionSize = projSizeInt;
                ReprojectionSizeSquared = ReprojectionSize * ReprojectionSize;
                CreateFrameNumbers();
                FrameWidth = frameWidth;
                FrameHeight = frameHeight;
                SubFrameWidth = frameWidth / projSizeInt;
                SubFrameHeight = frameHeight / projSizeInt;

                RenderTextureFormat format = camera.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
                RenderTextureDescriptor descSubFrame = WeatherMakerFullScreenEffect.GetRenderTextureDescriptor(downsampling, projSizeInt, projSizeInt, format, 0, camera);
                subFrameCurrent = WeatherMakerFullScreenEffect.CreateRenderTexture(descSubFrame, FilterMode.Bilinear, TextureWrapMode.Clamp);
                subFrameCurrent.name = "WeatherMakerTemporalReprojectionSubFrame";
                RenderTextureDescriptor descPrevFrame = WeatherMakerFullScreenEffect.GetRenderTextureDescriptor(downsampling, projSizeInt, 1, format, 0, camera);
                prevFrameCurrent = WeatherMakerFullScreenEffect.CreateRenderTexture(descPrevFrame, FilterMode.Bilinear, TextureWrapMode.Clamp);
                prevFrameCurrent.name = "WeatherMakerTemporalReprojectionPrevFrame";

                OffsetXConstant = (1.0f / (float)frameWidth);
                OffsetYConstant = (1.0f / (float)frameHeight);
                OffsetXConstant2 = (-0.5f * (float)(ReprojectionSize - 1) * OffsetXConstant);
                OffsetYConstant2 = (-0.5f * (float)(ReprojectionSize - 1) * OffsetYConstant);
                NeedsFirstFrameHandling = true;
            }

            ComputeTemporalOffsets();
        }

        public void PreRenderFrame(Camera camera, Camera lastNormalCamera)
        {
            if (TemporalReprojectionMaterial == null)
            {
                return;
            }
            WeatherMakerCameraType cameraType = WeatherMakerScript.GetCameraType(camera);
            if (cameraType == WeatherMakerCameraType.Other)
            {
                return;
            }

            lastNormalCamera = (cameraType == WeatherMakerCameraType.Normal ? camera : (lastNormalCamera ?? camera));

            // assign current matrixes, only normal stereo cameras use the stereo methods
            // reflection cameras are rendered one eye at a time without stereo
            if (camera.stereoEnabled)
            {
                view[0] = camera.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                view[1] = camera.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                projection[0] = lastNormalCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                projection[1] = lastNormalCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                inverseView[0] = view[0].inverse;
                inverseView[1] = view[1].inverse;
                inverseProjection[0] = projection[0].inverse;
                inverseProjection[1] = projection[1].inverse;
                inverseProjectionView[0] = inverseProjection[0] * inverseView[0];
                inverseProjectionView[1] = inverseProjection[1] * inverseView[1];
                prevViewProjection[0] = (projection[0] * prevView[0]);
                prevViewProjection[1] = (projection[1] * prevView[1]);
                ipivpvp[0] = projection[0] * prevView[0] * inverseView[0] * inverseProjection[0];
                ipivpvp[1] = projection[1] * prevView[1] * inverseView[1] * inverseProjection[1];
            }
            else
            {
                view[0] = view[1] = camera.worldToCameraMatrix;
                inverseView[0] = inverseView[1] = camera.cameraToWorldMatrix;
                projection[0] = projection[1] = lastNormalCamera.projectionMatrix;
                inverseProjection[0] = inverseProjection[1] = lastNormalCamera.projectionMatrix.inverse;
                inverseProjectionView[0] = inverseProjectionView[1] = inverseView[0] * inverseProjection[0];
                prevViewProjection[0] = prevViewProjection[1] = (projection[0] * prevView[0]);
                ipivpvp[0] = ipivpvp[1] = projection[0] * prevView[0] * inverseView[0] * inverseProjection[0];
            }

            TemporalReprojectionMaterial.SetTexture(WMS._TemporalReprojection_SubFrame, SubFrameTexture);
            TemporalReprojectionMaterial.SetTexture(WMS._TemporalReprojection_PrevFrame, PrevFrameTexture);
            TemporalReprojectionMaterial.SetFloat(WMS._TemporalReprojection_SubPixelSize, ReprojectionSize);
            TemporalReprojectionMaterial.SetFloat(WMS._TemporalReprojection_SubFrameNumber, SubFrameNumber);
            TemporalReprojectionMaterial.SetMatrixArray(WMS._TemporalReprojection_PreviousView, prevView);
            TemporalReprojectionMaterial.SetMatrixArray(WMS._TemporalReprojection_View, view);
            TemporalReprojectionMaterial.SetMatrixArray(WMS._TemporalReprojection_InverseView, inverseView);
            TemporalReprojectionMaterial.SetMatrixArray(WMS._TemporalReprojection_Projection, projection);
            TemporalReprojectionMaterial.SetMatrixArray(WMS._TemporalReprojection_InverseProjection, inverseProjection);
            TemporalReprojectionMaterial.SetMatrixArray(WMS._TemporalReprojection_InverseProjectionView, inverseProjectionView);
            TemporalReprojectionMaterial.SetMatrixArray(WMS._TemporalReprojection_PreviousViewProjection, prevViewProjection);
            TemporalReprojectionMaterial.SetMatrixArray(WMS._TemporalReprojection_ipivpvp, ipivpvp);

            if (CommandBuffer != null && CommandBuffer.Material != null)
            {
                CommandBuffer.Material.SetTexture(WMS._TemporalReprojection_SubFrame, SubFrameTexture);
                CommandBuffer.Material.SetTexture(WMS._TemporalReprojection_PrevFrame, PrevFrameTexture);
                CommandBuffer.Material.SetFloat(WMS._TemporalReprojection_SubPixelSize, ReprojectionSize);
                CommandBuffer.Material.SetFloat(WMS._TemporalReprojection_SubFrameNumber, SubFrameNumber);
                CommandBuffer.Material.SetFloat(WMS._TemporalReprojection_SimilarityMax, TemporalReprojectionMaterial.GetFloat(WMS._TemporalReprojection_SimilarityMax));
                CommandBuffer.Material.SetInt(WMS._TemporalReprojection_BlendMode, (int)BlendMode);
                CommandBuffer.Material.SetMatrixArray(WMS._TemporalReprojection_PreviousView, prevView);
                CommandBuffer.Material.SetMatrixArray(WMS._TemporalReprojection_View, view);
                CommandBuffer.Material.SetMatrixArray(WMS._TemporalReprojection_InverseView, inverseView);
                CommandBuffer.Material.SetMatrixArray(WMS._TemporalReprojection_Projection, projection);
                CommandBuffer.Material.SetMatrixArray(WMS._TemporalReprojection_InverseProjection, inverseProjection);
                CommandBuffer.Material.SetMatrixArray(WMS._TemporalReprojection_InverseProjectionView, inverseProjectionView);
                CommandBuffer.Material.SetMatrixArray(WMS._TemporalReprojection_PreviousViewProjection, prevViewProjection);
                CommandBuffer.Material.SetMatrixArray(WMS._TemporalReprojection_ipivpvp, ipivpvp);
            }
        }

        public void PostRenderFrame(Camera camera)
        {
            if (TemporalReprojectionMaterial == null)
            {
                return;
            }
            WeatherMakerCameraType cameraType = WeatherMakerScript.GetCameraType(camera);
            if (cameraType == WeatherMakerCameraType.Other)
            {
                return;
            }

            prevView[0] = view[0];
            prevView[1] = view[1];

            // move to next temporal reprojection frame
            unchecked
            {
                //if (++debugCount % 60 == 0)
                {
                    if (++frameIndex == frameNumbers.Length)
                    {
                        frameIndex = 0;
                    }
                    SubFrameNumber = frameNumbers[frameIndex];
                }
            }
        }
    }
}
