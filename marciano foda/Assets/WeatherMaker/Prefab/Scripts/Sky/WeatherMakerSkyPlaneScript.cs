﻿//
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
    public class WeatherMakerSkyPlaneScript : WeatherMakerPlaneCreatorScript
    {

#if UNITY_EDITOR

        private void OnWillRenderObject()
        {
            if (!Application.isPlaying && Camera.current != null && WeatherMakerLightManagerScript.Instance != null)
            {
                SkyPlaneProfile.UpdateSkyPlane(Camera.current, MeshRenderer.sharedMaterial, gameObject, WeatherMakerLightManagerScript.Instance.SunOrthographic);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (SkyPlaneProfile == null)
            {
                SkyPlaneProfile = Resources.Load<WeatherMakerSkyProfileScript>("WeatherMakerSkyProfile_Procedural");
            }
            if (!Application.isPlaying && Camera.main != null && WeatherMakerLightManagerScript.Instance != null)
            {
                SkyPlaneProfile.UpdateSkyPlane(Camera.main, MeshRenderer.sharedMaterial, gameObject, WeatherMakerLightManagerScript.Instance.SunOrthographic);
            }
        }

#endif

        private void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
            WeatherMakerCommandBufferManagerScript.Instance.RegisterPreCull(CameraPreCull, this);
        }

        private void OnDisable()
        {
        }

        private void OnDestroy()
        {
            WeatherMakerCommandBufferManagerScript.Instance.UnregisterPreCull(this);
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        [Header("Sky plane profile")]
        public WeatherMakerSkyProfileScript SkyPlaneProfile;

        private void CameraPreCull(Camera camera)
        {
            if (!WeatherMakerScript.ShouldIgnoreCamera(this, camera))
            {
                if (SkyPlaneProfile != null && camera != null && isActiveAndEnabled && WeatherMakerLightManagerScript.Instance != null)
                {
                    SkyPlaneProfile.UpdateSkyPlane(camera, MeshRenderer.sharedMaterial, gameObject, WeatherMakerLightManagerScript.Instance.SunOrthographic);
                }
            }
        }

        private static WeatherMakerSkyPlaneScript instance;
        /// <summary>
        /// Shared instance of sky plane script
        /// </summary>
        public static WeatherMakerSkyPlaneScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance); }
        }
    }
}
