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
using System.Collections;

namespace DigitalRuby.WeatherMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class WeatherMakerSkySphereScript : WeatherMakerSphereCreatorScript
    {
        [Header("Sky sphere profile")]
        public WeatherMakerSkyProfileScript SkySphereProfile;
        private readonly WeatherMakerMaterialCopy skySphereMaterial = new WeatherMakerMaterialCopy();

        private void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
            if (WeatherMakerCommandBufferManagerScript.Instance != null)
            {
                WeatherMakerCommandBufferManagerScript.Instance.RegisterPreCull(CameraPreCull, this);
                WeatherMakerCommandBufferManagerScript.Instance.RegisterPreRender(CameraPreRender, this);
                WeatherMakerCommandBufferManagerScript.Instance.RegisterPostRender(CameraPostRender, this);
            }
            skySphereMaterial.Update(MeshRenderer.sharedMaterial);
        }

        private void OnDisable()
        {
            skySphereMaterial.Dispose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (WeatherMakerCommandBufferManagerScript.Instance != null)
            {
                WeatherMakerCommandBufferManagerScript.Instance.UnregisterPreCull(this);
                WeatherMakerCommandBufferManagerScript.Instance.UnregisterPreRender(this);
                WeatherMakerCommandBufferManagerScript.Instance.UnregisterPostRender(this);
            }
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (SkySphereProfile != null && WeatherMakerDayNightCycleManagerScript.Instance != null &&
                (SkySphereProfile.SkyMode == WeatherMakeSkyMode.ProceduralPreethamStyle || SkySphereProfile.SkyMode == WeatherMakeSkyMode.ProceduralUnityStyle) &&
                WeatherMakerDayNightCycleManagerScript.Instance.DayNightProfile != null && SkySphereProfile.RotateAxis != Vector3.zero)
            {
                float seconds = WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay;
                transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(0.0f, 360.0f, seconds / WeatherMakerDayNightCycleProfileScript.SecondsPerDay), SkySphereProfile.RotateAxis);
            }
        }

        private void CameraPreCull(Camera camera)
        {
            if (WeatherMakerScript.ShouldIgnoreCamera(this, camera) || camera.orthographic)
            {
                return;
            }

            if (SkySphereProfile != null && camera != null && isActiveAndEnabled && WeatherMakerLightManagerScript.Instance != null)
            {
                SkySphereProfile.UpdateSkySphere(camera, skySphereMaterial, gameObject, WeatherMakerLightManagerScript.Instance.SunPerspective);
            }
        }

        private void CameraPreRender(Camera camera)
        {
            skySphereMaterial.Update(MeshRenderer.sharedMaterial);
            MeshRenderer.sharedMaterial = skySphereMaterial;
        }

        private void CameraPostRender(Camera camera)
        {
            MeshRenderer.sharedMaterial = skySphereMaterial.Original;
        }

#if UNITY_EDITOR

        protected override void OnWillRenderObject()
        {
            base.OnWillRenderObject();

            if (!Application.isPlaying && Camera.current != null && SkySphereProfile != null && WeatherMakerLightManagerScript.Instance != null)
            {
                SkySphereProfile.UpdateSkySphere(Camera.current, skySphereMaterial, gameObject, WeatherMakerLightManagerScript.Instance.SunPerspective);
            }
        }

        protected override void Start()
        {
            base.Start();

            if (SkySphereProfile == null)
            {
                SkySphereProfile = Resources.Load<WeatherMakerSkyProfileScript>("WeatherMakerSkyProfile_Procedural");
            }
        }

#endif

        private static WeatherMakerSkySphereScript instance;
        /// <summary>
        /// Shared instance of sky sphere script
        /// </summary>
        public static WeatherMakerSkySphereScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance); }
        }
    }
}