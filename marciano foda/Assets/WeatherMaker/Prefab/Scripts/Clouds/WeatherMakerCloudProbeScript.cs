using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    [ExecuteInEditMode]
    public class WeatherMakerCloudProbeScript : MonoBehaviour
    {
        [Tooltip("The target to probe. Defaults to this script transform.")]
        public Transform ProbeTarget;

        private void OnEnable()
        {
            if (ProbeTarget == null)
            {
                ProbeTarget = transform;
            }
            if (WeatherMakerFullScreenCloudsScript.Instance != null && ProbeTarget != null)
            {
                WeatherMakerFullScreenCloudsScript.Instance.RequestCloudProbe(ProbeTarget, true);
            }
        }

        private void OnDisable()
        {
            if (WeatherMakerFullScreenCloudsScript.Instance != null && ProbeTarget != null)
            {
                WeatherMakerFullScreenCloudsScript.Instance.RequestCloudProbe(ProbeTarget, false);
            }
        }

#if UNITY_EDITOR

        private static Texture2D blackTexture;
        private void OnDrawGizmos()
        {
            if (WeatherMakerFullScreenCloudsScript.Instance != null && ProbeTarget != null)
            {
                // https://gamedev.stackexchange.com/questions/120960/how-can-i-debug-draw-different-shapes-in-unity
                float density = WeatherMakerFullScreenCloudsScript.Instance.GetCloudDensity(Camera.current ?? Camera.main, ProbeTarget);
                float d = Mathf.Min(1.0f, density * 5.0f);
                UnityEditor.Handles.color = new Color(d, d, d, 1.0f);
                UnityEditor.Handles.SphereHandleCap(0, ProbeTarget.position, Quaternion.identity, 16.0f, EventType.Repaint);
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = textStyle.active.textColor = Color.white;
                if (blackTexture == null)
                {
                    blackTexture = new Texture2D(1, 1);
                    blackTexture.SetPixel(0, 0, Color.black);
                    blackTexture.Apply();
                }
                textStyle.normal.background = textStyle.active.background = blackTexture;
                UnityEditor.Handles.Label(ProbeTarget.position, "Cloud: " + density.ToString("0.000"), textStyle);
            }
        }

#endif

    }
}
