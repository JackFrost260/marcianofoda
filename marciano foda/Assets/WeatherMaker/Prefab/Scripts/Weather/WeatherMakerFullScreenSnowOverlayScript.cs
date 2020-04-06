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
    /// <summary>
    /// Full screen snow overlay script
    /// </summary>
    [ExecuteInEditMode]
    public class WeatherMakerFullScreenSnowOverlayScript : WeatherMakerFullScreenOverlayScript<WeatherMakerOverlayProfileScript>
    {
        protected override void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
            CommandBufferName = "WeatherMakerFullScreenSnowOverlayScript";
            base.OnEnable();
        }

        protected override void OnDestroy()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        private static WeatherMakerFullScreenSnowOverlayScript instance;
        /// <summary>
        /// Shared instance of full screen overlay script
        /// </summary>
        public static WeatherMakerFullScreenSnowOverlayScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance); }
        }
    }
}
