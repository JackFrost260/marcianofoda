using System;
using UnityEngine;

using DigitalRuby.WeatherMaker;

#if PLAYMAKER_PRESENT

namespace HutongGames.PlayMaker.Actions
{
    public abstract class WeatherMakerFsmStateAction : FsmStateAction
    {
        [Tooltip("Whether to run this action every frame")]
        public bool RunEveryFrame;

        protected abstract void DoUpdate();

        public override void OnEnter()
        {
            DoUpdate();
            if (!RunEveryFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoUpdate();
        }
    }

    [ActionCategory("WeatherMaker")]
    [Tooltip("Get the current date and time")]
    public class GetDateTime : WeatherMakerFsmStateAction
    {
        [Tooltip("Year in local time")]
        public FsmInt Year;

        [Tooltip("Month in local time")]
        public FsmInt Month;

        [Tooltip("Day in local time")]
        public FsmInt Day;

        [Tooltip("Hour in local time")]
        public FsmInt Hour;

        [Tooltip("Minute in local time")]
        public FsmInt Minute;

        [Tooltip("Second in local time")]
        public FsmInt Second;

        protected override void DoUpdate()
        {
            if (WeatherMakerDayNightCycleManagerScript.Instance == null || WeatherMakerDayNightCycleManagerScript.Instance.DayNightProfile == null)
            {
                return;
            }

            DateTime dt = WeatherMakerDayNightCycleManagerScript.Instance.DateTime;
            Year = dt.Year;
            Month = dt.Month;
            Day = dt.Day;
            Hour = dt.Hour;
            Minute = dt.Minute;
            Second = dt.Second;
        }
    }

    [ActionCategory("WeatherMaker")]
    [Tooltip("Get current weather profile name")]
    public class GetWeatherProfile : WeatherMakerFsmStateAction
    {
        public FsmString WeatherProfileName;

        protected override void DoUpdate()
        {
            if (WeatherMakerScript.Instance == null)
            {
                return;
            }

            WeatherProfileName = (WeatherMakerScript.Instance.LastLocalProfile == null ? "None" : WeatherMakerScript.Instance.LastLocalProfile.name);
        }
    }

    [ActionCategory("WeatherMaker")]
    [Tooltip("Set the current weather profile by name")]
    public class SetWeatherProfile : WeatherMakerFsmStateAction
    {
        [Tooltip("Set to 'None' to remove the weather profile and go back to random weather")]
        [RequiredField]
        public FsmString WeatherProfileName;

        protected override void DoUpdate()
        {
            if (WeatherMakerScript.Instance == null || WeatherProfileName == null || string.IsNullOrEmpty(WeatherProfileName.Value))
            {
                return;
            }

            WeatherMakerWeatherZoneScript zone = WeatherMakerScript.Instance.gameObject.GetComponentInChildren<WeatherMakerWeatherZoneScript>();
            if (zone != null)
            {
                zone.SingleProfile = (WeatherProfileName.Value.Equals("none", StringComparison.OrdinalIgnoreCase) ? null : Resources.Load<WeatherMakerProfileScript>(WeatherProfileName.Value));
                zone.gameObject.SetActive(true);
            }
        }
    }

    [ActionCategory("WeatherMaker")]
    [Tooltip("Set day/night cycle time to the current system time")]
    public class SetDateTimeSystem : WeatherMakerFsmStateAction
    {
        protected override void DoUpdate()
        {
            if (WeatherMakerDayNightCycleManagerScript.Instance == null || WeatherMakerDayNightCycleManagerScript.Instance.DayNightProfile == null)
            {
                return;
            }

            DateTime utcNow = DateTime.UtcNow;
            WeatherMakerDayNightCycleManagerScript.Instance.DateTime = utcNow;
            if (RunEveryFrame && WeatherMakerDayNightCycleManagerScript.Instance.DayNightProfile != null)
            {
                WeatherMakerDayNightCycleManagerScript.Instance.DayNightProfile.Speed = WeatherMakerDayNightCycleManagerScript.Instance.DayNightProfile.NightSpeed = 0.0f;
            }
        }
    }

    [ActionCategory("WeatherMaker")]
    [Tooltip("Set the current day/night cycle date and time")]
    public class SetTimeDate : WeatherMakerFsmStateAction
    {
        public bool SetYear;
        [Tooltip("Year in local time")]
        public FsmInt Year;
        
        public bool SetMonth;
        [Tooltip("Month in local time")]
        [HasFloatSlider(1, 12)]
        public FsmInt Month;

        public bool SetDay;
        [Tooltip("Day in local time")]
        [HasFloatSlider(1, 31)]
        public FsmInt Day;

        public bool SetHour;
        [Tooltip("Hour in local time")]
        [HasFloatSlider(0, 23)]
        public FsmInt Hour;

        public bool SetMinute;        
        [Tooltip("Minute in local time")]
        [HasFloatSlider(0, 59)]
        public FsmInt Minute;

        public bool SetSecond;
        [Tooltip("Second in local time")]
        [HasFloatSlider(0, 59)]
        public FsmInt Second;

        protected override void DoUpdate()
        {
            if (WeatherMakerDayNightCycleManagerScript.Instance == null || WeatherMakerDayNightCycleManagerScript.Instance.DayNightProfile == null)
            {
                return;
            }
            DateTime dt = WeatherMakerDayNightCycleManagerScript.Instance.DateTime;
            int year = (SetYear ? Year.Value : dt.Year);
            int month = (SetMonth ? Month.Value : dt.Month);
            int day = (SetDay ? Day.Value : dt.Day);
            int hour = (SetHour ? Hour.Value : dt.Hour);
            int minute = (SetMinute ? Minute.Value : dt.Minute);
            int second = (SetSecond ? Second.Value : dt.Second);
            dt = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
            WeatherMakerDayNightCycleManagerScript.Instance.DateTime = dt;
        }
    }
}

#endif
