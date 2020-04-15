﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using UnityEngine;

#if MIRROR_NETWORKING_PRESENT

using Mirror;

#endif

namespace DigitalRuby.WeatherMaker
{

#if MIRROR_NETWORKING_PRESENT

    [RequireComponent(typeof(NetworkIdentity))]

#endif

    [AddComponentMenu("Weather Maker/Extensions/Weather Maker Mirror Network Script", 1)]
    public class WeatherMakerMirrorNetworkScript :

#if MIRROR_NETWORKING_PRESENT

        NetworkBehaviour

#else

        MonoBehaviour

#endif

        , IWeatherMakerNetworkConnection
    {

#if MIRROR_NETWORKING_PRESENT

        private static WeatherMakerMirrorNetworkScript instance;
        public static WeatherMakerMirrorNetworkScript Instance { get { return WeatherMakerScript.FindOrCreateInstance(ref instance, true); } }

        public bool IsServer => (netIdentity != null && isServer);
        public bool IsClient => (netIdentity != null && isClient);
        public bool IsConnected => (netId != 0 && netIdentity != null && (isServer || isClient));

        private System.DateTime lastTimeOfDay;

#else

        public bool IsServer { get { return true; } }
        public bool IsClient { get { return true; } }
        public bool IsConnected { get { return false; } }

#endif

        private void Cleanup()
        {

#if MIRROR_NETWORKING_PRESENT

            if (WeatherMakerScript.Instance != null)
            {
                WeatherMakerScript.Instance.WeatherProfileChangedEvent -= WeatherProfileChanged;
                WeatherMakerScript.Instance.HasHadWeatherTransition = false;
                lastTimeOfDay = System.DateTime.MinValue;
            }

#endif

        }

        private void WeatherMakerInit()
        {

#if MIRROR_NETWORKING_PRESENT

            if (WeatherMakerScript.Instance != null)
            {
                WeatherMakerScript.Instance.NetworkConnection = this;
            }

#endif

        }

        private void WeatherMakerUpdate()
        {

#if MIRROR_NETWORKING_PRESENT

            if (IsConnected && IsServer && WeatherMakerDayNightCycleManagerScript.Instance != null)
            {
                WeatherMakerDayNightCycleManagerScript d = WeatherMakerDayNightCycleManagerScript.Instance;
                System.DateTime dt = new System.DateTime(d.Year, d.Month, d.Day, d.TimeOfDayTimespan.Hours, d.TimeOfDayTimespan.Minutes, d.TimeOfDayTimespan.Seconds);
                if (dt != lastTimeOfDay)
                {
                    lastTimeOfDay = dt;
                    RpcSetTimeOfDay(d.Year, d.Month, d.Day, d.TimeOfDay);
                }
            }

#endif

        }

        private void Awake()
        {
            WeatherMakerInit();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void LateUpdate()
        {


#if MIRROR_NETWORKING_PRESENT

            WeatherMakerUpdate();

#endif

        }

#if MIRROR_NETWORKING_PRESENT

        public override void OnNetworkDestroy()
        {
            base.OnNetworkDestroy();
            Cleanup();
        }

        [Command]
        private void CmdNewClient()
        {
            WeatherMakerDayNightCycleManagerScript d = WeatherMakerDayNightCycleManagerScript.Instance;
            if (d != null)
            {
                TargetRpcSetTimeOfDay(connectionToClient, d.Year, d.Month, d.Day, d.TimeOfDay);
            }
        }

        [ClientRpc]
        private void RpcSetTimeOfDay(int year, int month, int day, float timeOfDay)
        {
            if (netId != 0 && isClient && WeatherMakerDayNightCycleManagerScript.Instance != null)
            {
                WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay = timeOfDay;
            }
        }

        [TargetRpc]
        internal void TargetRpcSetTimeOfDay(NetworkConnection conn, int year, int month, int day, float timeOfDay)
        {
            if (netId != 0 && isClient && WeatherMakerDayNightCycleManagerScript.Instance != null)
            {
                WeatherMakerDayNightCycleManagerScript.Instance.TimeOfDay = timeOfDay;
            }
        }

        [TargetRpc]
        private void TargetRpcWeatherProfileChanged(NetworkConnection conn, string oldProfileName, string oldProfileJson, string newProfileName, string newProfileJson, float transitionDuration)
        {
            if (netId != 0 && isClient && WeatherMakerScript.Instance != null)
            {
                // notify any listeners of the change - hold duration is -1.0 meaning the server will send another profile when it is ready (hold duration unknown to client)
                WeatherMakerProfileScript oldProfileFromJson = null;
                WeatherMakerProfileScript newProfileFromJson = null;
                if (oldProfileName != null && oldProfileJson != null)
                {
                    oldProfileFromJson = GameObject.Instantiate(Resources.Load<WeatherMakerProfileScript>(oldProfileName));
                    JsonUtility.FromJsonOverwrite(oldProfileJson, oldProfileFromJson);
                }
                if (newProfileName != null && newProfileJson != null)
                {
                    newProfileFromJson = GameObject.Instantiate(Resources.Load<WeatherMakerProfileScript>(newProfileName));
                    JsonUtility.FromJsonOverwrite(newProfileJson, newProfileFromJson);
                }
                WeatherMakerScript.Instance.RaiseWeatherProfileChanged(oldProfileFromJson, newProfileFromJson, transitionDuration, -1.0f, true, null);
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            WeatherMakerScript.Instance.WeatherProfileChangedEvent -= WeatherProfileChanged;
            WeatherMakerScript.Instance.WeatherProfileChangedEvent += WeatherProfileChanged;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            if (!IsServer)
            {
                if (WeatherMakerScript.Instance != null)
                {
                    WeatherMakerScript.Instance.HasHadWeatherTransition = false;
                }
                if (WeatherMakerDayNightCycleManagerScript.Instance != null)
                {
                    // disable day night speed, the server will be syncing the day / night
                    WeatherMakerDayNightCycleManagerScript.Instance.Speed = WeatherMakerDayNightCycleManagerScript.Instance.NightSpeed = 0.0f;

                    // request day night from server
                    CmdNewClient();
                }
            }
        }

#endif

        public string GetConnectionId(Transform obj)
        {
			
#if MIRROR_NETWORKING_PRESENT

            if (obj != null)
            {
                NetworkIdentity id = obj.GetComponentInChildren<NetworkIdentity>();
                if (id == null && obj.transform.parent != null)
                {
                    id = obj.parent.GetComponent<NetworkIdentity>();
                }
                if (id == null || (id.connectionToServer == null && id.connectionToClient == null))
                {
                    return null;
                }
                else if (id.connectionToClient != null)
                {
                    return id.connectionToClient.connectionId.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    return id.connectionToServer.connectionId.ToString(CultureInfo.InvariantCulture);
                }
            }

#endif

            return "0";
        }

        private void WeatherProfileChanged(WeatherMakerProfileScript oldProfile, WeatherMakerProfileScript newProfile, float transitionDuration, string[] connectionIds)
        {

#if MIRROR_NETWORKING_PRESENT

            if (connectionIds == null || connectionIds.Length == 0)
            {
                return;
            }

            // send the profile change to clients
            else if (netId != 0 && isServer)
            {
                string oldProfileName = (oldProfile == null ? null : oldProfile.name.Replace("(Clone)", string.Empty).Trim());
                string newProfileName = (newProfile == null ? null : newProfile.name.Replace("(Clone)", string.Empty).Trim());
                string oldProfileJson = (oldProfile == null ? null : JsonUtility.ToJson(oldProfile));
                string newProfileJson = (newProfile == null ? null : JsonUtility.ToJson(newProfile));
                Dictionary<int, NetworkConnection> connections = NetworkServer.connections;
                foreach (string connectionId in connectionIds)
                {
                    NetworkConnection conn;
                    int connectionIdInt;
                    if (int.TryParse(connectionId, out connectionIdInt) && connections.TryGetValue(connectionIdInt, out conn))
                    {
                        TargetRpcWeatherProfileChanged(conn, oldProfileName, oldProfileJson, newProfileName, newProfileJson, transitionDuration);
                    }
                }
            }

#endif

        }
    }
}
