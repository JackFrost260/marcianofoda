using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.Networking;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerLocationWeatherScript : MonoBehaviour
    {
        [Header("Weather Zone")]
        [Tooltip("The weather zone to control")]
        public WeatherMakerWeatherZoneScript WeatherZoneScript;

        [Header("Parameters")]
        [Tooltip("Update rate in minutes. API will be refreshed every n minutes with new weather.")]
        [Range(15.0f, 60.0f)]
        public float UpdateRateSeconds = 15.0f;

        [Tooltip("The place name to use, overrides lat/lon. Set to null to use the current day/night cycle lat/lon.")]
        public string PlaceName;

        [Tooltip("The latitude to use. Set to -999 to use the current day/night cycle latitude. Set to 999 to auto-detect local position.")]
        public float Latitude = 999.0f;

        [Tooltip("The longitude to use. Set to -999 to use the current day/night cycle longitude. Set to 999 to auto-detect local position.")]
        public float Longitude = 999.0f;

        [Tooltip("Api key for the weather API")]
        public string ApiKey;

        [Header("Location Services")]
        [Tooltip("Whether to auto start and stop the location service. Set to false if you do this elsewhere or don't need location services.")]
        public bool AutoStartStop = true;

        [Tooltip("Accuracy in meters. Ignored if AutoStartStop is false.")]
        [Range(1, 10000)]
        public float AccuracyMeters = 1000;

        [Tooltip("Distance in meters to move before refreshing position. Ignored if AutoStartStop is false.")]
        [Range(1, 10000)]
        public float ThresholdMeters = 1000;

        /// <summary>
        /// The weather location API to use
        /// </summary>
        public IWeatherMakerLocationWeatherApi WeatherApi { get; set; }

        private System.Action<WeatherMakerProfileScript> weatherCallback;

        /// <summary>
        /// Elapsed time - when it goes above UpdateRateSeconds, refreshes weather. Set to 9999.0f to force a refresh.
        /// </summary>
        public float ElapsedTime { get; private set; }

        private void WeatherCallback(WeatherMakerProfileScript data)
        {
            if (WeatherZoneScript != null)
            {
                WeatherZoneScript.SingleProfile = data;
            }
        }

        private void OnEnable()
        {
            if (AutoStartStop)
            {
                Input.location.Start(AccuracyMeters, ThresholdMeters);
            }
            weatherCallback = WeatherCallback;
            if (WeatherApi == null)
            {
                WeatherApi = new WeatherMakerOpenWeatherMapApi();
            }
            ElapsedTime = 9999.0f;
            if (WeatherZoneScript != null)
            {
                WeatherZoneScript.SingleProfile = Resources.Load<WeatherMakerProfileScript>("WeatherMakerProfile_Clear");
                WeatherZoneScript.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (AutoStartStop)
            {
                Input.location.Stop();
            }
        }

        private void Update()
        {
            if (WeatherApi != null && ApiKey != null && WeatherZoneScript != null)
            {
                WeatherZoneScript.gameObject.SetActive(true);
                ElapsedTime += Time.unscaledDeltaTime;
                if (ElapsedTime >= UpdateRateSeconds)
                {
                    ElapsedTime = 0.0f;
                    float lat, lon;
                    if (Latitude < -998.0f || Longitude < -998.0f)
                    {
                        lat = (float)WeatherMakerDayNightCycleManagerScript.Instance.Latitude;
                        lon = (float)WeatherMakerDayNightCycleManagerScript.Instance.Longitude;
                    }
                    else if ((Latitude > 998.0f || Longitude > 998.0f) && Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
                    {
                        lat = Input.location.lastData.latitude;
                        lon = Input.location.lastData.longitude;
                    }
                    else
                    {
                        if (Latitude >= -90.0f && Latitude <= 90.0f && Longitude >= -180.0f && Longitude <= 180.0f)
                        {
                            lat = Latitude;
                            lon = Longitude;
                        }
                        else
                        {
                            lat = (float)WeatherMakerDayNightCycleManagerScript.Instance.Latitude;
                            lon = (float)WeatherMakerDayNightCycleManagerScript.Instance.Longitude;
                        }
                    }
                    WeatherApi.QueryWeather(lat, lon, PlaceName, ApiKey, weatherCallback);
                }
            }
        }
    }

    public interface IWeatherMakerLocationWeatherApi
    {
        void QueryWeather(float lat, float lon, string locationName, string apiKey, System.Action<WeatherMakerProfileScript> callback);
    }

    public class WeatherMakerOpenWeatherMapApi : IWeatherMakerLocationWeatherApi
    {
        // https://openweathermap.org/weather-conditions
        private readonly Dictionary<int, string> idToProfile = new Dictionary<int, string>
        {
            // thunderstorm
            { 200, "WeatherMakerProfile_Storm" },
            { 201, "WeatherMakerProfile_Storm" },
            { 202, "WeatherMakerProfile_Storm" },
            { 210, "WeatherMakerProfile_Storm" },
            { 211, "WeatherMakerProfile_Storm" },
            { 212, "WeatherMakerProfile_Storm" },
            { 221, "WeatherMakerProfile_Storm" },
            { 230, "WeatherMakerProfile_Storm" },
            { 231, "WeatherMakerProfile_Storm" },
            { 232, "WeatherMakerProfile_Storm" },
            { 504, "WeatherMakerProfile_Storm" },
            { 781, "WeatherMakerProfile_Storm" },

            // light rain
            { 300, "WeatherMakerProfile_LightRain" },
            { 310, "WeatherMakerProfile_LightRain" },
            { 313, "WeatherMakerProfile_LightRain" },
            { 321, "WeatherMakerProfile_LightRain" },
            { 500, "WeatherMakerProfile_LightRain" },
            { 520, "WeatherMakerProfile_LightRain" },
            { 531, "WeatherMakerProfile_LightRain" },

            // medium rain
            { 301, "WeatherMakerProfile_MediumRain" },
            { 311, "WeatherMakerProfile_MediumRain" },
            { 501, "WeatherMakerProfile_MediumRain" },
            { 502, "WeatherMakerProfile_MediumRain" },
            { 521, "WeatherMakerProfile_MediumRain" },

            // heavy rain
            { 302, "WeatherMakerProfile_HeavyRain" },
            { 312, "WeatherMakerProfile_HeavyRain" },
            { 314, "WeatherMakerProfile_HeavyRain" },
            { 503, "WeatherMakerProfile_HeavyRain" },
            { 522, "WeatherMakerProfile_HeavyRain" },

            // sleet
            { 611, "WeatherMakerProfile_MediumSleet" },
            { 612, "WeatherMakerProfile_MediumSleet" },
            { 613, "WeatherMakerProfile_HeavySleet" },
            { 615, "WeatherMakerProfile_LightSleet" },
            { 616, "WeatherMakerProfile_MediumSleet" },
            
            // snow
            { 600, "WeatherMakerProfile_LightSnow" },
            { 601, "WeatherMakerProfile_MediumSnow" },
            { 602, "WeatherMakerProfile_HeavySnow" },
            { 620, "WeatherMakerProfile_LightSnow" },
            { 621, "WeatherMakerProfile_LightSnow" },
            { 622, "WeatherMakerProfile_MediumSnow" },

            // clouds
            { 801, "WeatherMakerProfile_LightCloudsScattered" },
            { 802, "WeatherMakerProfile_MdeiumCloudsScattered" },
            { 803, "WeatherMakerProfile_MdeiumHeavyCloudsScattered" },
            { 804, "WeatherMakerProfile_HeavyCloudsScattered" },

            // atmosphere
            { 701, "WeatherMakerProfile_MediumFog" },
            { 711, "WeatherMakerProfile_Smoky" },
            { 721, "WeatherMakerProfile_LightFog" },
            { 731, "WeatherMakerProfile_Sandstorm" },
            { 741, "WeatherMakerProfile_MediumFog" },
            { 751, "WeatherMakerProfile_Sandstorm" },
            { 761, "WeatherMakerProfile_Sandstorm" },
            { 762, "WeatherMakerProfile_Smoky" },
            { 771, "WeatherMakerProfile_Sandstorm" }
        };

        private WeatherMakerProfileScript ParseResult(string text)
        {
            // {"coord":{"lon":139,"lat":35},"weather":[{"id":804,"main":"Clouds","description":"overcast clouds","icon":"04n"}],"base":"stations","main":{"temp":289,"pressure":1012,"humidity":65,"temp_min":287.04,"temp_max":290.37},
            // "wind":{"speed":1.45,"deg":317.931},"clouds":{"all":100},"dt":1557501319,"sys":{"type":3,"id":2019346,"message":0.0066,"country":"JP","sunrise":1557517462,"sunset":1557567389},"id":1851632,"name":"Shuzenji","cod":200}
            Match match = Regex.Match(text, @"""weather"":\[\{""id"":(?<id>[0-9]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                int id = int.Parse(match.Groups["id"].Value);
                string profileName;
                if (idToProfile.TryGetValue(id, out profileName))
                {
                    return Resources.Load<WeatherMakerProfileScript>(profileName);
                }
            }
            else
            {
                Debug.LogWarning("Failed to parse weather api text of " + text);
            }
            return Resources.Load<WeatherMakerProfileScript>("WeatherMakerProfile_Clear");
        }

        public void QueryWeather(float lat, float lon, string locationName, string apiKey, System.Action<WeatherMakerProfileScript> callback)
        {
            string url;
            if (!string.IsNullOrEmpty(locationName))
            {
                url = "https://api.openweathermap.org/data/2.5/weather?q=" + UnityWebRequest.EscapeURL(locationName) + "&APPID=" + apiKey;
            }
            else
            {
                url = "https://api.openweathermap.org/data/2.5/weather?lat=35&lon=139&APPID=" + apiKey;
            }
            UnityWebRequest req = UnityWebRequest.Get(url);
            UnityWebRequestAsyncOperation op = req.SendWebRequest();
            op.completed += (AsyncOperation _op) =>
            {
                callback(ParseResult(req.downloadHandler.text));
            };
        }
    }
}