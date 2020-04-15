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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Weather zones simply determine what weather profile should be used. Depending
    /// on whether the collider is intersected by a camera will determine what profile
    /// the camera sees. TODO: Add multi-camera and network support.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Weather Maker/Weather/Weather Zone", 0)]
    public class WeatherMakerWeatherZoneScript : MonoBehaviour
    {
        [Header("Weather Zone - Profile Group (Multiple Profiles)")]
        [Tooltip("Ignored if SingleProfile is not null, otherwise this is the weather profile group " +
            "for this weather zone. The collider (which must be a trigger) is used to determine whether " +
            "this profile group is activated.")]
        public WeatherMakerProfileGroupScript ProfileGroup;

        [Header("Weather Zone - Single Profile")]
        [Tooltip("A single weather profile for this weather zone. If null, ProfileGroup is used. " +
            "If you are using SingleProfile, you must manually change this field to change the weather. " +
            "If you want automatic weather, use ProfileGroup instead. The collider (which must be a " +
            "trigger) is used to determine whether this profile is activated.")]
        public WeatherMakerProfileScript SingleProfile;

        [Header("Weather Zone - Transition Duration Multiplier")]
        [Tooltip("The transition duration multiplier when entering this weather zone from another zone. " +
            "This helps speed up the transition to the new weather zone, such as coming from a blizzard to " +
            "a more temperate weather zone.")]
        [Range(0.0f, 1.0f)]
        public float TransitionDurationMultiplier = 0.5f;

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

        [MinMaxSlider(0.0f, 1000.0f, "(Seconds) override random duration for profiles to transition in (set to 0 to not override)")]
        public RangeOfFloats TransitionDuration = new RangeOfFloats { Minimum = 0, Maximum = 0 };

        [MinMaxSlider(0.0f, 1000.0f, "(Seconds) override random duration for profiles to hold before transition to another profile (set to 0 to not override)")]
        public RangeOfFloats HoldDuration = new RangeOfFloats { Minimum = 0, Maximum = 0 };

        private static readonly Dictionary<WeatherMakerWeatherZoneScript, List<Transform>> zonesAndPlayers = new Dictionary<WeatherMakerWeatherZoneScript, List<Transform>>();
        private static readonly Dictionary<Transform, List<WeatherMakerWeatherZoneScript>> playersAndZones = new Dictionary<Transform, List<WeatherMakerWeatherZoneScript>>();

        private WeatherMakerProfileScript currentProfile;
        private WeatherMakerProfileScript currentProfileSingle;
        private float secondsRemainingTransition;
        private float secondsRemainingHold;

        private void Cleanup()
        {
            // clear all dictionary entries, trigger events will re-fire when enable is called
            zonesAndPlayers.Clear();
            playersAndZones.Clear();
        }

        private void Awake()
        {
            
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        private void LateUpdate()
        {
            if (SingleProfile == null && ProfileGroup == null || WeatherMakerScript.Instance == null)
            {
                return;
            }

            UpdateProfile();
        }

        private void OnEnable()
        {
            UpdateProfile();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void OnDestroy()
        {
            CleanupProfile(currentProfile);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (SingleProfile == null && ProfileGroup == null)
            {
                return;
            }

            bool isServer = WeatherMakerScript.Instance != null && WeatherMakerScript.Instance.NetworkConnection.IsServer;
            if (isServer && gameObject.activeInHierarchy && enabled && WeatherMakerScript.IsPlayer(other.transform))
            {
                // ensure we have player zones and zone players lists
                List<WeatherMakerWeatherZoneScript> playerZones;
                if (!playersAndZones.TryGetValue(other.transform, out playerZones))
                {
                    playersAndZones[other.transform] = playerZones = new List<WeatherMakerWeatherZoneScript>();
                }

                if (playerZones.Contains(this))
                {
                    // already in the zone, don't re-process, sometimes OnTriggerEnter is called twice in a row
                    return;
                }

                List<Transform> zonePlayers;
                if (!zonesAndPlayers.TryGetValue(this, out zonePlayers))
                {
                    zonesAndPlayers[this] = zonePlayers = new List<Transform>();
                }

                // remove player from their previous zone
                WeatherMakerWeatherZoneScript prevZone = null;
                if (playerZones.Count != 0)
                {
                    // remove player from previous weather zone
                    prevZone = playerZones[playerZones.Count - 1];
                    List<Transform> prevZonePlayers;
                    if (zonesAndPlayers.TryGetValue(prevZone, out prevZonePlayers))
                    {
                        prevZonePlayers.Remove(other.transform);
                    }
                }

                // see if we have a previous zone, if so we have a previous profile to transition from
                WeatherMakerProfileScript previousProfile = (playerZones.Count == 0 ? null : playerZones[playerZones.Count - 1].currentProfile);
                float transitionDuration = (previousProfile == null ? 0.001f : currentProfile.RandomTransitionDuration());

                // add zone to the player zone stack
                playerZones.Add(this);
                
                // add player to the zone player list
                zonePlayers.Add(other.transform);

                if (WeatherMakerScript.IsLocalPlayer(other.transform))
                {
                    WeatherMakerScript.Instance.LastLocalProfile = currentProfile;
                    if (prevZone != this)
                    {
                        WeatherMakerScript.Instance.WeatherZoneChanged.Invoke(this);
                    }
                }

                // pick a new duration for the transition
                string connectionId = WeatherMakerScript.Instance.NetworkConnection.GetConnectionId(other.transform);

                // transition to new weather zone
                // if transition from another zone, multiply the transition duration so we get to the new weather zone weather more quickly
                WeatherMakerScript.Instance.RaiseWeatherProfileChanged(previousProfile, currentProfile, transitionDuration * (previousProfile == null ? 1.0f : TransitionDurationMultiplier),
                    secondsRemainingHold, false, (connectionId == null ? null : new string[1] { connectionId }));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (SingleProfile == null && ProfileGroup == null)
            {
                return;
            }

            bool isServer = WeatherMakerScript.Instance != null && WeatherMakerScript.Instance.NetworkConnection.IsServer;
            if (isServer && gameObject.activeInHierarchy && enabled && WeatherMakerScript.IsPlayer(other.transform))
            {
                List<WeatherMakerWeatherZoneScript> playerZones;
                if (!playersAndZones.TryGetValue(other.transform, out playerZones))
                {
                    playersAndZones[other.transform] = playerZones = new List<WeatherMakerWeatherZoneScript>();
                }

                if (!playerZones.Contains(this))
                {
                    // not in this zone, exit out - sometimes OnTriggerExit is called twice in a row
                    return;
                }

                List<Transform> zonePlayers;
                if (!zonesAndPlayers.TryGetValue(this, out zonePlayers))
                {
                    zonesAndPlayers[this] = zonePlayers = new List<Transform>();
                }

                // if entering a new zone, begin transition to the new zone's current weather
                WeatherMakerProfileScript previousProfile = (playerZones.Count == 0 ? null : playerZones[playerZones.Count - 1].currentProfile);
                WeatherMakerProfileScript newProfile = null;

                // remove zone from the player zone stack
                playerZones.Remove(this);

                // remove player from the zone player list
                zonePlayers.Remove(other.transform);

                float transitionDuration = 0.0f;

                // add the player to the previous weather zone if any
                WeatherMakerWeatherZoneScript newZone = null;
                if (playerZones.Count != 0)
                {
                    newZone = playerZones[playerZones.Count - 1];
                    if (!zonesAndPlayers.TryGetValue(newZone, out zonePlayers))
                    {
                        zonePlayers = new List<Transform>();
                    }
                    zonePlayers.Add(other.transform);

                    // pick a new duration for the transition
                    if (newZone.currentProfile != null)
                    {
                        transitionDuration = newZone.currentProfile.RandomTransitionDuration();
                        newProfile = newZone.currentProfile;
                    }
                }
                else
                {
                    Debug.LogError("Exited weather zone into no more zones, please add a global weather zone as a catch all zone");
                    return;
                }

                if (WeatherMakerScript.IsLocalPlayer(other.transform))
                {
                    WeatherMakerScript.Instance.LastLocalProfile = newProfile;
                    WeatherMakerScript.Instance.WeatherZoneChanged.Invoke(newZone);
                }

                string connectionId = WeatherMakerScript.Instance.NetworkConnection.GetConnectionId(other.transform);

                // transition to new weather zone
                // if transition from another zone, multiply the transition duration so we get to the new weather zone weather more quickly
                WeatherMakerScript.Instance.RaiseWeatherProfileChanged(previousProfile, newProfile, transitionDuration * (previousProfile == null ? 1.0f : TransitionDurationMultiplier),
                    secondsRemainingHold, false, (connectionId == null ? null : new string[1] { connectionId }));
            }
        }

        private bool PruneNulls<T>(List<T> list) where T : class
        {
            if (list == null)
            {
                return false;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                }
            }

            return (list.Count != 0);
        }

        private void NotifyThoseInZoneOfProfileChange(WeatherMakerProfileScript oldProfile, WeatherMakerProfileScript newProfile, float transitionDuration)
        {
            // anyone in this zone gets a new profile
            List<Transform> playersInZone;
            if (!zonesAndPlayers.TryGetValue(this, out playersInZone) || !PruneNulls(playersInZone))
            {
                // no players in zone, we are done
                return;
            }
            List<string> connectionIds = new List<string>();
            bool hasLocalPlayer = false;
            foreach (Transform player in playersInZone)
            {
                hasLocalPlayer = WeatherMakerScript.IsLocalPlayer(player);
                connectionIds.Add(WeatherMakerScript.Instance.NetworkConnection.GetConnectionId(player));
            }

            if (hasLocalPlayer)
            {
                WeatherMakerScript.Instance.LastLocalProfile = newProfile;
            }

            // send notification that those in this zone need a new weather profile
            WeatherMakerScript.Instance.RaiseWeatherProfileChanged(oldProfile, newProfile, secondsRemainingTransition,
                secondsRemainingHold, false, connectionIds.ToArray());
        }

        private void CleanupProfile(WeatherMakerProfileScript profile)
        {
            if (profile != null && profile.name.IndexOf("(clone)", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                GameObject.Destroy(profile);
            }
        }

        private void UpdateProfile()
        {
            // clients will get the transition sent to them via network extension
            if (WeatherMakerScript.Instance == null || !WeatherMakerScript.Instance.NetworkConnection.IsServer || (SingleProfile == null && ProfileGroup == null))
            {
                return;
            }
            else if (SingleProfile == null)
            {
                currentProfileSingle = null;
                if (secondsRemainingTransition <= 0.0f)
                {
                    if ((secondsRemainingHold -= Time.deltaTime) <= 0.0f)
                    {
                        // setup new transition
                        WeatherMakerProfileScript oldProfile = currentProfile;
                        currentProfile = ProfileGroup.PickWeightedProfile();
                        RangeOfFloats duration = TransitionDuration;
                        currentProfile = WeatherMakerProfileGroupScript.OverrideProfile(currentProfile, CloudProfile, SkyProfile, AuroraProfile,
                            PrecipitationProfile, FogProfile, WindProfile, LightningProfile, SoundProfile, duration, HoldDuration);
                        secondsRemainingTransition = currentProfile.RandomTransitionDuration();
                        secondsRemainingHold = currentProfile.RandomHoldDuration();
                        NotifyThoseInZoneOfProfileChange(oldProfile, currentProfile, secondsRemainingTransition);
                        CleanupProfile(oldProfile);
                    }
                }
                else
                {
                    // else let the transition continue
                    secondsRemainingTransition -= Time.deltaTime;
                }
            }
            else if (SingleProfile != currentProfileSingle)
            {
                currentProfileSingle = SingleProfile;
                WeatherMakerProfileScript oldProfile = currentProfile;
                RangeOfFloats duration = TransitionDuration;
                currentProfile = WeatherMakerProfileGroupScript.OverrideProfile(SingleProfile, CloudProfile, SkyProfile, AuroraProfile,
                    PrecipitationProfile, FogProfile, WindProfile, LightningProfile, SoundProfile, duration, HoldDuration);
                secondsRemainingTransition = currentProfile.RandomTransitionDuration();
                NotifyThoseInZoneOfProfileChange(oldProfile, SingleProfile, secondsRemainingTransition);
                CleanupProfile(oldProfile);
            }
        }
    }
}
