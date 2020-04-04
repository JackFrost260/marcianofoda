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
    public static class GradientExtensions
    {
        private static readonly List<float> keysTimes = new List<float>();

        public static UnityEngine.Gradient Lerp(this UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return Lerp(a, b, t, false, false);
        }

        private static UnityEngine.Gradient Lerp(this UnityEngine.Gradient a, UnityEngine.Gradient b, float t, bool noAlpha, bool noColor)
        {
            keysTimes.Clear();

            if (a == b || a.Equals(b))
            {
                return b;
            }
            else if (a.alphaKeys.Length == b.alphaKeys.Length && a.colorKeys.Length == b.colorKeys.Length)
            {
                // full compare of all keys, save allocating memory if both gradients are equal
                bool equal = true;
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    if (a.alphaKeys[i].alpha != b.alphaKeys[i].alpha || a.alphaKeys[i].time != b.alphaKeys[i].time ||
                        a.colorKeys[i].color != b.colorKeys[i].color || a.colorKeys[i].time != b.colorKeys[i].time)
                    {
                        equal = false;
                        break;
                    }
                }
                if (equal)
                {
                    return b;
                }
                Gradient gradient = new Gradient();
                GradientColorKey[] clrs = new GradientColorKey[a.colorKeys.Length];
                GradientAlphaKey[] alphas = new GradientAlphaKey[a.colorKeys.Length];
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    clrs[i] = new GradientColorKey(Color.Lerp(a.colorKeys[i].color, b.colorKeys[i].color, t), Mathf.Lerp(a.colorKeys[i].time, b.colorKeys[i].time, t));
                    alphas[i] = new GradientAlphaKey(Mathf.Lerp(a.alphaKeys[i].alpha, b.alphaKeys[i].alpha, t), Mathf.Lerp(a.alphaKeys[i].time, b.alphaKeys[i].time, t));
                }
                gradient.colorKeys = clrs;
                gradient.alphaKeys = alphas;
                return gradient;
            }
            else
            {
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    float k = a.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                    {
                        keysTimes.Add(k);
                    }
                }

                for (int i = 0; i < b.colorKeys.Length; i++)
                {
                    float k = b.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                    {
                        keysTimes.Add(k);
                    }
                }
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    float k = a.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                    {
                        keysTimes.Add(k);
                    }
                }

                for (int i = 0; i < b.alphaKeys.Length; i++)
                {
                    float k = b.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                    {
                        keysTimes.Add(k);
                    }
                }

                GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
                GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

                for (int i = 0; i < keysTimes.Count; i++)
                {
                    float key = keysTimes[i];
                    var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
                    clrs[i] = new GradientColorKey(clr, key);
                    alphas[i] = new GradientAlphaKey(clr.a, key);
                }

                var g = new Gradient();
                g.SetKeys(clrs, alphas);
                return g;
            }
        }
    }
}
