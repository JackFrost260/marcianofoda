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
        private class TriggerEnterEvent
        {
            public Transform Transform;
            public readonly List<WeatherMakerWeatherZoneScript> Stack = new List<WeatherMakerWeatherZoneScript>();
            public bool Popped;
        }

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

        private static readonly Dictionary<Transform, TriggerEnterEvent> activeZones = new Dictionary<Transform, TriggerEnterEvent>();
        private static readonly List<TriggerEnterEvent> pendingTriggerEvents = new List<TriggerEnterEvent>();

        private WeatherMakerProfileScript currentProfile;
        private WeatherMakerProfileScript currentProfileSingle;
        private float secondsRemainingTransition;
        private float secondsRemainingHold;
        private readonly List<Transform> tempGameObjects = new List<Transform>();

        private void Cleanup()
        {
            // cleanup destroyed dictionary entries
            foreach (var key in activeZones.Keys)
            {
                if (key == null)
                {
                    tempGameObjects.Add(key);
                }
            }
            foreach (Transform obj in tempGameObjects)
            {
                activeZones.Remove(obj);
            }
            tempGameObjects.Clear();
        }

        private void ProcessPendingTriggerEvents()
        {
            for (int i = 0; i < pendingTriggerEvents.Count; i++)
            {
                TransitionFrom(pendingTriggerEvents[i]);
            }
            pendingTriggerEvents.Clear();
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

            ProcessPendingTriggerEvents();
            UpdateProfile();
            Cleanup();
        }

        private void OnEnable()
        {
            UpdateProfile();
        }

        private void OnDisable()
        {
        }

        private void OnDestroy()
        {
            CleanupProfile(currentProfile);
        }

        private void OnTriggerEnter(Collider other)
        {
            bool isServer = WeatherMakerScript.Instance != null && WeatherMakerScript.Instance.NetworkConnection.IsServer;
            if (isServer && WeatherMakerScript.IsPlayer(other.transform))
            {
                TriggerEnterEvent triggerEvent;
                if (!activeZones.TryGetValue(other.transform, out triggerEvent))
                {
                    activeZones[other.transform] = triggerEvent = new TriggerEnterEvent();
                }
                if (!triggerEvent.Stack.Contains(this))
                {
                    triggerEvent.Transform = other.transform;
                    WeatherMakerWeatherZoneScript previous = (triggerEvent.Stack.Count == 0 ? null : triggerEvent.Stack[triggerEvent.Stack.Count - 1]);
                    triggerEvent.Stack.Add(this);
                    pendingTriggerEvents.Add(triggerEvent);

                    string newName = (SingleProfile == null ? (ProfileGroup == null ? "None" : ProfileGroup.name) : SingleProfile.name);
                    string oldName = (previous == null ? "None" : (previous.SingleProfile == null ? previous.ProfileGroup.name : previous.SingleProfile.name));
                    Debug.LogFormat("{0} is entering weather zone {1} from zone {2}", other.transform.name, newName, oldName);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (SingleProfile == null && ProfileGroup == null)
            {
                return;
            }

            bool isServer = WeatherMakerScript.Instance != null && WeatherMakerScript.Instance.NetworkConnection.IsServer;
            if (isServer && WeatherMakerScript.IsPlayer(other.transform))
            {
                // denote that we have left a weather zone and pop back to the previous weather zone
                TriggerEnterEvent triggerEvent;
                if (activeZones.TryGetValue(other.transform, out triggerEvent))
                {
                    triggerEvent.Popped = true;
                    pendingTriggerEvents.Add(triggerEvent);
                }
            }
        }

        private void TransitionFrom(TriggerEnterEvent evt)
        {
            // called when player moves from one weather zone to another
            if (evt.Stack.Count == 0 || currentProfile == null)
            {
                return;
            }

            WeatherMakerWeatherZoneScript currentScript, previousScript;
            if (evt.Popped)
            {
                previousScript = evt.Stack[evt.Stack.Count - 1];
                evt.Stack.RemoveAt(evt.Stack.Count - 1);
                evt.Popped = false;

                // if transition out of weather zone to no weather zone, stick with the previous weather script
                currentScript = (evt.Stack.Count == 0 ? previousScript : evt.Stack[evt.Stack.Count - 1]);
            }
            else
            {
                previousScript = (evt.Stack.Count == 1 ? null : evt.Stack[evt.Stack.Count - 2]);
                currentScript = evt.Stack[evt.Stack.Count - 1];
            }

            // if entering a new zone, begin transition to the new zone's current weather
            WeatherMakerProfileScript previousProfile = (previousScript == null ? null : previousScript.currentProfile);
            float transitionDuration = currentProfile.RandomTransitionDuration();

            // pick a new duration for the transition
            string connectionId = WeatherMakerScript.Instance.NetworkConnection.GetConnectionId(evt.Transform);

            // transition to new weather zone
            // if transition from another zone, multiply the transition duration so we get to the new weather zone weather more quickly
            WeatherMakerScript.Instance.RaiseWeatherProfileChanged(previousProfile, currentScript.currentProfile, transitionDuration * (previousScript == null ? 1.0f : TransitionDurationMultiplier),
                secondsRemainingHold, false, (connectionId == null ? null : new string[1] { connectionId }));
        }

        private void NotifyThoseInZoneOfProfileChange(WeatherMakerProfileScript oldProfile, WeatherMakerProfileScript newProfile, float transitionDuration)
        {
            // anyone in this zone gets a new profile
            List<string> connectionIds = null;
            bool foundOne = false;
            foreach (var kv in activeZones)
            {
                // if the active zone is this zone
                if (kv.Value.Stack.Count != 0 && kv.Value.Stack[kv.Value.Stack.Count - 1] == this)
                {
                    foundOne = true;

                    // find a connection id
                    string connectionId = WeatherMakerScript.Instance.NetworkConnection.GetConnectionId(kv.Value.Transform);
                    if (connectionId != null)
                    {
                        // found connection id, add to list
                        connectionIds = (connectionIds ?? new List<string>());
                        connectionIds.Add(connectionId);
                    }
                }
            }

            if (foundOne)
            {
                // send notification that those in this zone need a new weather profile
                WeatherMakerScript.Instance.RaiseWeatherProfileChanged(oldProfile, newProfile, secondsRemainingTransition,
                    secondsRemainingHold, false, (connectionIds == null ? null : connectionIds.ToArray()));
            }
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
