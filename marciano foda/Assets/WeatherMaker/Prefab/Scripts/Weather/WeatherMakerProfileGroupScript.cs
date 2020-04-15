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
    [System.Serializable]
    public class WeatherMakerProfileAndWeight
    {
        [Header("Profile and Weight")]
        [Tooltip("Profile name")]
        public string Name;

        [Tooltip("Profile settings")]
        public WeatherMakerProfileScript Profile;

        [Tooltip("Weight")]
        [Range(0.0f, 1000.0f)]
        public int Weight;

        [Header("Profile Overrides")]
        [Tooltip("Override cloud profile (set to null to not override)")]
        public WeatherMakerCloudProfileScript CloudProfile;

        [Tooltip("Override sky profile (set to null to not override)")]
        public WeatherMakerSkyProfileScript SkyProfile;

        [Tooltip("Override aurora profile (set to null to not override)")]
        public WeatherMakerAuroraProfileScript AuroraProfile;

        [Tooltip("Override precipitation profile (set to null to not override)")]
        public WeatherMakerPrecipitationProfileScript PrecipitationProfile;

        [Tooltip("Override fog profile (set to null to not override)")]
        public WeatherMakerFullScreenFogProfileScript FogProfile;

        [Tooltip("Override wind profile (set to null to not override)")]
        public WeatherMakerWindProfileScript WindProfile;

        [Tooltip("Override lightning profile (set to null to not override)")]
        public WeatherMakerLightningProfileScript LightningProfile;

        [Tooltip("Override sound profile (set null to not override)")]
        public WeatherMakerSoundProfileScript SoundProfile;

        [MinMaxSlider(0.0f, 1000.0f, "(Seconds) Override random duration for profile to transition in, set to 0 to use the profile group transition duration.")]
        public RangeOfFloats TransitionDuration = new RangeOfFloats { Minimum = 0, Maximum = 0 };

        [MinMaxSlider(0.0f, 1000.0f, "(Seconds) Override random duration for profile to hold before transition to another profile, set to 0 to use the profile group transition duration.")]
        public RangeOfFloats HoldDuration = new RangeOfFloats { Minimum = 0, Maximum = 0 };

        /// <summary>
        /// Get a profile, adding in any overrides and cloning if needed
        /// </summary>
        /// <returns></returns>
        public WeatherMakerProfileScript GetProfile()
        {
            return WeatherMakerProfileGroupScript.OverrideProfile(Profile, CloudProfile, SkyProfile, AuroraProfile, PrecipitationProfile, FogProfile, WindProfile, LightningProfile, SoundProfile, TransitionDuration, HoldDuration);
        }
    }

    [CreateAssetMenu(fileName = "WeatherMakerProfileGroup", menuName = "WeatherMaker/Weather Profile Group", order = 20)]
    public class WeatherMakerProfileGroupScript : WeatherMakerBaseScriptableObjectScript, IComparer<WeatherMakerProfileAndWeight>
    {
        [Tooltip("Profiles. A random profile is chosen using the weight of all profiles.")]
        public WeatherMakerProfileAndWeight[] Profiles;

        private static readonly List<WeatherMakerProfileAndWeight> sortedProfiles = new List<WeatherMakerProfileAndWeight>();

        [Header("Profile Overrides")]
        [Tooltip("Override cloud profile (set to null to not override)")]
        public WeatherMakerCloudProfileScript CloudProfile;

        [Tooltip("Override sky profile (set to null to not override)")]
        public WeatherMakerSkyProfileScript SkyProfile;

        [Tooltip("Override aurora profile (set to null to not override)")]
        public WeatherMakerAuroraProfileScript AuroraProfile;

        [Tooltip("Override precipitation profile (set to null to not override)")]
        public WeatherMakerPrecipitationProfileScript PrecipitationProfile;

        [Tooltip("Override fog profile (set to null to not override)")]
        public WeatherMakerFullScreenFogProfileScript FogProfile;

        [Tooltip("Override wind profile (set to null to not override)")]
        public WeatherMakerWindProfileScript WindProfile;

        [Tooltip("Override lightning profile (set to null to not override)")]
        public WeatherMakerLightningProfileScript LightningProfile;

        [Tooltip("Override sound profile (set to null to not override)")]
        public WeatherMakerSoundProfileScript SoundProfile;

        [MinMaxSlider(0.0f, 1000.0f, "(Seconds) Override random duration for profiles to transition in, set to 0 to use the weather zone transition duration.")]
        public RangeOfFloats TransitionDuration = new RangeOfFloats { Minimum = 0, Maximum = 0 };

        [MinMaxSlider(0.0f, 1000.0f, "(Seconds) Override random duration for profiles to hold before transition to another profile, set to 0 to use the weather zone hold duration.")]
        public RangeOfFloats HoldDuration = new RangeOfFloats { Minimum = 0, Maximum = 0 };

        /// <summary>
        /// Pick a random profile based on weights of the profiles
        /// </summary>
        /// <returns>Random weighted profile or null if no profiles setup</returns>
        public WeatherMakerProfileScript PickWeightedProfile()
        {
            if (Profiles == null || Profiles.Length == 0)
            {
                return null;
            }

            int maxWeight = 0;

            // ensure profiles are sorted by weight ascending
            sortedProfiles.Clear();
            sortedProfiles.AddRange(Profiles);
            sortedProfiles.Sort(this);

            // get the max weight (sum of all weights)
            foreach (WeatherMakerProfileAndWeight w in sortedProfiles)
            {
                if (!w.Profile.Disabled)
                {
                    maxWeight += w.Weight;
                }
            }

            // pick a weight that we need to exceed to pick a profile
            int randomWeight = UnityEngine.Random.Range(0, maxWeight);

            // interate all profiles, as we sum up the weight and it crosses are random weight, we have our profile
            foreach (WeatherMakerProfileAndWeight profile in sortedProfiles)
            {
                if (!profile.Profile.Disabled)
                {
                    if (randomWeight < profile.Weight)
                    {
                        WeatherMakerProfileScript individualProfile = profile.GetProfile();
                        return OverrideProfile(individualProfile, CloudProfile, SkyProfile, AuroraProfile, PrecipitationProfile, FogProfile, WindProfile, LightningProfile, SoundProfile, TransitionDuration, HoldDuration);
                    }
                    randomWeight -= profile.Weight;
                }
            }

            // shouldn't get here
            Debug.LogError("Error in PickWeightedProfile algorithm, should not return null here");
            return null;
        }

        private static void AssignOverride<T>(ref T field, T fieldOverrideIfNotNull)
        {
            field = (fieldOverrideIfNotNull == null ? field : fieldOverrideIfNotNull);
        }

        public static WeatherMakerProfileScript OverrideProfile
        (
            WeatherMakerProfileScript profile,
            WeatherMakerCloudProfileScript cloudProfile,
            WeatherMakerSkyProfileScript skyProfile,
            WeatherMakerAuroraProfileScript auroraProfile,
            WeatherMakerPrecipitationProfileScript precipitationProfile,
            WeatherMakerFullScreenFogProfileScript fogProfile,
            WeatherMakerWindProfileScript windProfile,
            WeatherMakerLightningProfileScript lightningProfile,
            WeatherMakerSoundProfileScript soundProfile,
            RangeOfFloats transitionDuration,
            RangeOfFloats holdDuration
        )
        {
            if (profile == null)
            {
                Debug.LogError("Null profile in Weather Maker profile OverrideProfile, this is not allowed");
                return null;
            }

            // check for overrides, if so clone the profile
            else if (cloudProfile != null || skyProfile != null || auroraProfile != null || precipitationProfile != null || fogProfile != null ||
                windProfile != null || lightningProfile != null || soundProfile != null || transitionDuration.Maximum > 0.0f || holdDuration.Maximum > 0.0f)
            {
                if (profile.name.IndexOf("(clone)", System.StringComparison.OrdinalIgnoreCase) < 0)
                {
                    profile = ScriptableObject.Instantiate(profile);
                }
                AssignOverride(ref profile.CloudProfile, cloudProfile);
                AssignOverride(ref profile.SkyProfile, skyProfile);
                AssignOverride(ref profile.AuroraProfile, auroraProfile);
                AssignOverride(ref profile.PrecipitationProfile, precipitationProfile);
                AssignOverride(ref profile.FogProfile, fogProfile);
                AssignOverride(ref profile.WindProfile, windProfile);
                AssignOverride(ref profile.LightningProfile, lightningProfile);
                AssignOverride(ref profile.SoundProfile, soundProfile);
                if (transitionDuration.Maximum > 0.0f)
                {
                    profile.TransitionDuration = transitionDuration;
                }
                if (holdDuration.Maximum > 0.0f)
                {
                    profile.HoldDuration = holdDuration;
                }
            }

            return profile;
        }

        int IComparer<WeatherMakerProfileAndWeight>.Compare(WeatherMakerProfileAndWeight x, WeatherMakerProfileAndWeight y)
        {
            return x.Weight.CompareTo(y.Weight);
        }
    }
}
