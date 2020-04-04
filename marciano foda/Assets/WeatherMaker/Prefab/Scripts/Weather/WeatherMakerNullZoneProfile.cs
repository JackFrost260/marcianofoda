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

namespace DigitalRuby.WeatherMaker
{
    [System.Flags]
    public enum NullZoneRenderMask
    {
        Precipitation = 1,
        Fog = 2,
        Overlay = 4,
        Water = 8
    }

    [System.Serializable]
    public sealed class WeatherMakerZullZoneState
    {
        [Tooltip("Whether the state is enabled.")]
        public bool Enabled;

        [WeatherMaker.EnumFlag(Tooltip = "Null zone render mask.")]
        public NullZoneRenderMask RenderMask = (NullZoneRenderMask)0;

        [Tooltip("Null zone fade. Lower values fade more near the edges of the null zone. Set to max value to disable fade. " +
            "If less than 0, overlay in the zone will be allowed but will fade out as camera gets closer to the zone using abs(fade).")]
        [Range(-0.99f, 100.0f)]
        public float Fade = 0.2f;
    }

    /// <summary>
    /// Null zone profile
    /// </summary>
    [CreateAssetMenu(fileName = "WeatherMakerNullZoneProfile", menuName = "WeatherMaker/Null Zone Profile", order = 23)]
    public sealed class WeatherMakerNullZoneProfile : ScriptableObject
    {

#if UNITY_EDITOR

#pragma warning disable 0414

        [SerializeField]
        [TextArea(1, 6)]
        [Tooltip("Description to describe what this profile is doing")]
        private string Description = null;

#pragma warning restore 0414

#endif

        [Tooltip("The default render state. Used if the player is not in the null zone or the entered state is not enabled.")]
        public WeatherMakerZullZoneState DefaultState;

        [Tooltip("The render mask to use if the player enters the null zone. A player is any object with an enabled audio listener.")]
        public WeatherMakerZullZoneState EnteredState;
    }
}
