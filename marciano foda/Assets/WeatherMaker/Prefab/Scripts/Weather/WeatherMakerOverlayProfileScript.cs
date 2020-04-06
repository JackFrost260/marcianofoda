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
    [CreateAssetMenu(fileName = "WeatherMakerOverlayProfile", menuName = "WeatherMaker/Overlay Profile", order = 80)]
    public class WeatherMakerOverlayProfileScript : WeatherMakerOverlayProfileScriptBase
    {

    }

    public class WeatherMakerOverlayProfileScriptBase : WeatherMakerBaseScriptableObjectScript
    {
        [Header("Overlay - appearance")]
        [Tooltip("Overlay intensity")]
        [Range(0.0f, 1.0f)]
        public float OverlayIntensity;

        [Tooltip("Overlay reflection intensity. Not all overlay support this.")]
        [Range(0.0f, 1.0f)]
        public float OverlayReflectionIntensity;

        [Tooltip("Overlay minimum intensity, regardless of other factors, overlay intensity will not drop below this value.")]
        [Range(0.0f, 1.0f)]
        public float OverlayMinimumIntensity;

        [Tooltip("Overlay how fast it accumulates - higher values accumulate faster based on external intensity, 0 for no auto accumulation. Negative for melting/reducing effect. 0.001 seems to be a good value.")]
        [Range(-1.0f, 1.0f)]
        public float AutoIntensityMultiplier = 0.0f;
        private float autoIntensityAccum;

        /// <summary>
        /// Overlay function to get external intensity, such as from the snow script, null for none
        /// </summary>
        public System.Func<float> ExternalIntensityFunction { get; set; }

        [Tooltip("Overlay overlay intensity as y normal moves away from 1. Lower values cause the overlay to appear more on normals with lower y values.")]
        [Range(-1.0f, 1.0f)]
        public float OverlayNormalReducer = 0.5f;

        [Tooltip("Overlay texture, null for default")]
        public Texture2D OverlayTexture;

        [Tooltip("Overlay scale")]
        [Range(0.0f, 1.0f)]
        public float OverlayScale = 0.005f;

        [Tooltip("Overlay offset - initial uv offset")]
        public Vector2 OverlayOffset;

        [Tooltip("Overlay velocity (moves texture UV)")]
        public Vector2 OverlayVelocity;
        private Vector2 overlayVelocityAccum;

        [Tooltip("Overlay color")]
        public Color OverlayColor = Color.white;

        [Tooltip("Overlay specular color")]
        public Color OverlaySpecularColor = Color.white;

        [Tooltip("Overlay specular intensity")]
        [Range(0.0f, 64.0f)]
        public float OverlaySpecularIntensity = 2.0f;

        [Tooltip("Overlay specular power, reduces specular area but increases intensity as this value increases")]
        [Range(0.0f, 64.0f)]
        public float OverlaySpecularPower = 4.0f;

        [Header("Overlay - noise")]
        [Tooltip("Overlay noise texture, null for none")]
        public Texture2D OverlayNoiseTexture;

        [Tooltip("Overlay noise multiplier")]
        [Range(0.0f, 20.0f)]
        public float OverlayNoiseMultiplier = 8.0f;

        [Tooltip("Overlay noise power, shader noise calculation is raised to this power")]
        [Range(0.0f, 16.0f)]
        public float OverlayNoisePower = 1.0f;

        [Tooltip("Overlay noise adder")]
        [Range(-6.0f, 6.0f)]
        public float OverlayNoiseAdder;

        [Tooltip("One minus overlay intensity to this power is subtracted from OverlayNoiseAdder, " +
            "used to easily make gaps in the snow, which automatically reduce as overlay intensity increases.")]
        [Range(0.0f, 1.0f)]
        public float OverlayNoiseAdderIntensityPower = 0.5f;
        [Tooltip("Overlay noise scale")]
        [Range(0.0f, 1.0f)]
        public float OverlayNoiseScale = 0.0025f;

        [Tooltip("Overlay noise offset")]
        public Vector2 OverlayNoiseOffset;

        [Tooltip("Overlay noise velocity")]
        public Vector2 OverlayNoiseVelocity;
        private Vector2 overlayNoiseVelocityAccum;

        [Header("Overlay - min height")]
        [Tooltip("Minimum height to show overlay at")]
        [Range(0.0f, 1000.0f)]
        public float OverlayMinHeight = 0.0f;

        [Tooltip("Overlay noise multiplier")]
        [Range(0.0f, 3.0f)]
        public float OverlayMinHeightNoiseMultiplier = 0.5f;

        [Tooltip("Overlay min height falloff multiplier")]
        [Range(0.0f, 3.0f)]
        public float OverlayMinHeightFalloffMultiplier = 0.5f;

        [Tooltip("Overlay min height falloff power")]
        [Range(0.0f, 128.0f)]
        public float OverlayMinHeightFalloffPower = 16.0f;

        [Tooltip("Overlay height noise texture, null for none")]
        public Texture2D OverlayNoiseHeightTexture;

        [Tooltip("Overlay noise adder if using noise texture to vary min height, ignored if not using noise texture")]
        [Range(-5.0f, 5.0f)]
        public float OverlayMinHeightNoiseAdder = 2.0f;

        [Tooltip("Overlay noise scale for min height variance, ignored if not using noise texture")]
        [Range(0.0f, 1.0f)]
        public float OverlayMinHeightNoiseScale = 0.02f;

        [Tooltip("Overlay noise offset for min height variance, ignored if not using noise texture")]
        public Vector2 OverlayMinHeightNoiseOffset;

        [Tooltip("Overlay noise velocity for min height variance, ignored if not using noise texture")]
        public Vector2 OverlayMinHeightNoiseVelocity;
        private Vector2 overlayMinHeightVelocityAccum;

        private float finalIntensity;

        public virtual void UpdateMaterial(Material overlayMaterial)
        {
            if (overlayMaterial == null)
            {
                return;
            }

            overlayMaterial.SetFloat(WMS._OverlayIntensity, finalIntensity);
            overlayMaterial.SetFloat(WMS._OverlayReflectionIntensity, OverlayReflectionIntensity);
            overlayMaterial.SetFloat(WMS._OverlayNormalReducer, OverlayNormalReducer);
            overlayMaterial.SetTexture(WMS._OverlayTexture, OverlayTexture);
            if (OverlayNoiseTexture != null && OverlayNoiseMultiplier > 0.0f)
            {
                overlayMaterial.SetTexture(WMS._OverlayNoiseTexture, OverlayNoiseTexture);
                overlayMaterial.SetFloat(WMS._OverlayNoiseMultiplier, OverlayNoiseMultiplier);
                overlayMaterial.SetFloat(WMS._OverlayNoisePower, OverlayNoisePower);
                float overlayIntensityPower = Mathf.Pow(OverlayIntensity, OverlayNoiseAdderIntensityPower);
                overlayMaterial.SetFloat(WMS._OverlayNoiseAdder, Mathf.Max(-6.0f, OverlayNoiseAdder - (1.0f - overlayIntensityPower)));
                overlayMaterial.SetFloat(WMS._OverlayNoiseScale, OverlayNoiseScale);
                overlayMaterial.SetVector(WMS._OverlayNoiseOffset, OverlayNoiseOffset);
                overlayMaterial.SetVector(WMS._OverlayNoiseVelocity, overlayNoiseVelocityAccum);
                overlayMaterial.SetInt(WMS._OverlayNoiseEnabled, 1);
            }
            else
            {
                overlayMaterial.SetInt(WMS._OverlayNoiseEnabled, 0);
            }
            if (OverlayMinHeight > 0.0f)
            {
                overlayMaterial.SetFloat(WMS._OverlayMinHeight, OverlayMinHeight);
                overlayMaterial.SetFloat(WMS._OverlayMinHeightNoiseMultiplier, OverlayMinHeightNoiseMultiplier);
                overlayMaterial.SetFloat(WMS._OverlayMinHeightNoiseAdder, OverlayMinHeightNoiseAdder);
                overlayMaterial.SetFloat(WMS._OverlayMinHeightFalloffMultiplier, OverlayMinHeightFalloffMultiplier);
                overlayMaterial.SetFloat(WMS._OverlayMinHeightFalloffPower, OverlayMinHeightFalloffPower);
                if (OverlayNoiseHeightTexture != null)
                {
                    overlayMaterial.SetInt(WMS._OverlayMinHeightNoiseEnabled, 1);
                    overlayMaterial.SetTexture(WMS._OverlayNoiseHeightTexture, OverlayNoiseHeightTexture);
                    overlayMaterial.SetFloat(WMS._OverlayMinHeightNoiseScale, OverlayMinHeightNoiseScale);
                    overlayMaterial.SetVector(WMS._OverlayMinHeightNoiseOffset, OverlayMinHeightNoiseOffset);
                    overlayMaterial.SetVector(WMS._OverlayMinHeightNoiseVelocity, overlayMinHeightVelocityAccum);
                }
                else
                {
                    overlayMaterial.SetInt(WMS._OverlayMinHeightNoiseEnabled, 0);
                }
            }
            else
            {
                overlayMaterial.SetInt(WMS._OverlayMinHeightNoiseEnabled, 0);
                overlayMaterial.SetFloat(WMS._OverlayMinHeight, 0.0f);
            }
            overlayMaterial.SetFloat(WMS._OverlayScale, OverlayScale);
            overlayMaterial.SetVector(WMS._OverlayOffset, OverlayOffset);
            overlayMaterial.SetVector(WMS._OverlayVelocity, overlayVelocityAccum);
            overlayMaterial.SetColor(WMS._OverlayColor, OverlayColor);
            overlayMaterial.SetColor(WMS._OverlaySpecularColor, OverlaySpecularColor);
            overlayMaterial.SetFloat(WMS._OverlaySpecularIntensity, OverlaySpecularIntensity);
            overlayMaterial.SetFloat(WMS._OverlaySpecularPower, OverlaySpecularPower);
        }

        public override void Update()
        {
            base.Update();

            if (ExternalIntensityFunction != null && AutoIntensityMultiplier != 0.0f)
            {
                if (AutoIntensityMultiplier > 0.0f)
                {
                    autoIntensityAccum = Mathf.Clamp(autoIntensityAccum + (ExternalIntensityFunction() * Time.deltaTime * AutoIntensityMultiplier), 0.0f, 1.0f);
                    OverlayIntensity = autoIntensityAccum;
                }
                else if (AutoIntensityMultiplier < 0.0f)
                {
                    autoIntensityAccum = Mathf.Clamp(autoIntensityAccum + (Time.deltaTime * AutoIntensityMultiplier), 0.0f, 1.0f);
                    OverlayIntensity = autoIntensityAccum;
                }
            }
            else
            {
                autoIntensityAccum = OverlayIntensity;
            }
            overlayVelocityAccum += (OverlayVelocity * Time.deltaTime);
            overlayNoiseVelocityAccum += (OverlayNoiseVelocity * Time.deltaTime);
            overlayMinHeightVelocityAccum += (OverlayMinHeightNoiseVelocity * Time.deltaTime);
            finalIntensity = Mathf.Clamp(autoIntensityAccum * autoIntensityAccum, OverlayMinimumIntensity, 1.0f);
        }
    }
}
