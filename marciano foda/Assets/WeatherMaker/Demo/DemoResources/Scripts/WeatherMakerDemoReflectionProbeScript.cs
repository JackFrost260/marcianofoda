using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    [ExecuteInEditMode]
    public class WeatherMakerDemoReflectionProbeScript : MonoBehaviour
    {
        public ReflectionProbe ReflectionProbe;

        private float seconds;

        private void Update()
        {
            seconds += Time.unscaledDeltaTime;
            if (seconds >= (1.0f / 30.0f))
            {
                seconds = 0.0f;
                ReflectionProbe.RenderProbe();
            }
        }
    }
}
