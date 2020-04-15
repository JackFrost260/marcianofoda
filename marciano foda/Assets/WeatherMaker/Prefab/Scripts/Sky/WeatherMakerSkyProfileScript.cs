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

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// Sky modes
    /// </summary>
    public enum WeatherMakeSkyMode
    {
        /// <summary>
        /// Textured - day, dawn/dusk and night are all done via textures
        /// </summary>
        Textured = 0,

        /// <summary>
        /// Procedural sky - day and dawn/dusk textures are overlaid on top of procedural sky, night texture is used as is
        /// </summary>
        ProceduralTexturedUnityStyle = 1,

        /// <summary>
        /// Procedural sky - day, dawn/dusk textures are ignored, night texture is used as is
        /// </summary>
        ProceduralUnityStyle = 2,

        /// <summary>
        /// Procedural sky - day and dawn/dusk textures are overlaid on top of procedural sky, night texture is used as is - preetham style
        /// </summary>
        ProceduralTexturedPreethamStyle = 3,

        /// <summary>
        /// Procedural sky - day, dawn/dusk textures are ignored, night texture is used as is - preetham style
        /// </summary>
        ProceduralPreethamStyle = 4
    }

    [CreateAssetMenu(fileName = "WeatherMakerSkyProfile", menuName = "WeatherMaker/Sky Profile", order = 30)]
    [System.Serializable]
    public class WeatherMakerSkyProfileScript : ScriptableObject
    {
        [Header("Sky Rendering")]
        [Tooltip("The sky mode. 'Textured' uses a texture for day, dawn/dusk and night. " +
            "'Procedural textured' combines a procedural sky with the day and dawn/dusk textures using alpha, and uses the night texture as is. " +
            "'Procedural' uses the night texture as is and does everything else procedurally.")]
        public WeatherMakeSkyMode SkyMode = WeatherMakeSkyMode.ProceduralUnityStyle;

        [Range(0.0f, 1.0f)]
        [Tooltip("Dither level")]
        public float DitherLevel = 0.005f;

        [Header("Positioning")]
        [Range(-1.0f, 1.0f)]
        [Tooltip("Offset the sky this amount from the camera y. In 2D, this moves the sky up or down a percentage of screen height.")]
        public float YOffsetMultiplier = 0.0f;

        [Range(0.1f, 1.0f)]
        [Tooltip("Place the sky sphere at this amount of the far clip plane")]
        public float FarClipPlaneMultiplier = 0.8f;

        [Tooltip("Sky gradient. Useful for extra tint of sky and clouds as sun nears the horizon. Center of gradient is sun right at horizon.")]
        public Gradient SkyGradient;

        [Header("Textures")]
        [Tooltip("The daytime texture (ignored for procedural sky mode).")]
        public Texture2D DayTexture;

        [Tooltip("The dawn / dusk texture (ignored for procedural sky mode).")]
        public Texture2D DawnDuskTexture;

        [Tooltip("The night time texture")]
        public Texture2D NightTexture;

        [Header("Night Sky")]
        [Range(0.0f, 1.0f)]
        [Tooltip("Night pixels must have an R, G or B value greater than or equal to this to be visible. Raise this value if you want to hide dimmer elements " +
            "of your night texture or there is a lot of light pollution, i.e. a city.")]
        public float NightVisibilityThreshold = 0.0f;

        [Range(0.0f, 32.0f)]
        [Tooltip("Intensity of night sky. Pixels that don't meet the NightVisibilityThreshold will still be invisible.")]
        public float NightIntensity = 1.0f;

        [Tooltip("Night power. The night multiplier is raised by this power. Set to less than 1.0 to cause the night sky and starts to come in faster.")]
        [Range(0.01f, 4.0f)]
        public float NightPower = 1.0f;

        [Tooltip("How much color in the sky reduces the night sky. The max rgb of the sky is multiplied by this value (with a min of 1.0). (1.0 - this value) * night sky is final night sky value.")]
        [Range(0.0f, 256.0f)]
        public float NightDuskMultiplier = 128.0f;

        [Range(0.0f, 100.0f)]
        [Tooltip("How fast the twinkle pulsates")]
        public float NightTwinkleSpeed = 6.6f;

        [Tooltip("The variance of the night twinkle. The higher the value, the more variance.")]
        [Range(0.0f, 10.0f)]
        public float NightTwinkleVariance = 1.11f;

        [Tooltip("The minimum of the max rgb component for the night pixel to twinkle")]
        [Range(0.0f, 1.0f)]
        public float NightTwinkleMinimum = 0.02f;

        [Tooltip("The amount of randomness in the night sky twinkle")]
        [Range(0.0f, 5.0f)]
        public float NightTwinkleRandomness = 0.87f;

        [Header("Sky Parameters (Advanced)")]
        [Tooltip("Allow rotating on an axis with day/night cycle. Set to all 0 for no rotation. Requires procedural sky mode (non-textured).")]
        public Vector3 RotateAxis;

        [Tooltip("Allows eclipses - beware Unity bug that causes raycast to be very expensive. If you see CPU spike, disable.")]
        public bool CheckForEclipse;

        [Range(0.0f, 1.0f)]
        [Tooltip("Sky camera height (km)")]
        public float SkyCameraHeight = 0.0001f;

        [Tooltip("Sky sphere exposure, decrease/increase brightness")]
        [Range(0.01f, 8.0f)]
        public float SkyExposure = 1.0f;

        [Tooltip("Sky tint color")]
        public Color SkyTintColor = new Color(0.5f, 0.5f, 0.5f);

        [Range(0.0f, 1.0f)]
        [Tooltip("Sky atmosphere mie g, controls glow around the sun, etc.")]
        public float SkyAtmosphereMie = 0.99f;

        [Range(0.0f, 5.0f)]
        [Tooltip("Sky atmosphere thickness")]
        public float SkyAtmosphereThickness = 1.0f;

        [Range(0.0f, 10.0f)]
        [Tooltip("Sky atmosphere turbidity")]
        public float SkyAtmosphereTurbidity = 1.0f;

        [Range(1.0f, 2.0f)]
        [Tooltip("Sky outer radius")]
        public float SkyOuterRadius = 1.025f;

        [Range(1.0f, 2.0f)]
        [Tooltip("Sky inner radius")]
        public float SkyInnerRadius = 1.0f;

        [Range(0.0f, 50.0f)]
        [Tooltip("Sky mie multiplier")]
        public float SkyMieMultiplier = 1.0f;

        [Range(0.0f, 50.0f)]
        [Tooltip("Sky rayleigh multiplier")]
        public float SkyRayleighMultiplier = 1.0f;

        [SingleLine("Light wave lengths (RGB, Tint Range)")]
        public Vector4 LightWaveLengths = new Vector4(0.650f, 0.570f, 0.475f, 0.15f);

        private static readonly RaycastHit[] eclipseHits = new RaycastHit[16];

        private WeatherMakerShaderPropertiesScript shaderProps;

        private static Vector4 GetTotalRayleigh(Vector3 waveLength, float rayleighMultiplier, WeatherMakerCelestialObjectScript sun)
        {
            if (sun == null)
            {
                return Vector4.one;
            }

            // https://github.com/ngokevin/kframe/blob/master/components/sun-sky/shaders/fragment.glsl
            const float n = 1.0003f; // refractive index of air
            const float N = 2.545E25f; // number of molecules per unit volume for air at 288.15K and 1013mb (sea level -45 celsius)
            const float pn = 0.035f;  // depolatization factor for standard air

            float sunPosY = -sun.transform.forward.y;
            float sunfade = 1.0f - Mathf.Clamp(1.0f - Mathf.Exp((sunPosY / 450000.0f)), 0.0f, 1.0f);
            float rayleighCoefficient = rayleighMultiplier - (1.0f * (1.0f - sunfade));
            Vector4 r = new Vector4
            (
                rayleighCoefficient * (((8.0f * Mathf.Pow(Mathf.PI, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f))) * (6.0f + 3.0f * pn)) / ((3.0f * N * Mathf.Pow(waveLength.x, 4.0f)) * (6.0f - 7.0f * pn))),
                rayleighCoefficient * (((8.0f * Mathf.Pow(Mathf.PI, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f))) * (6.0f + 3.0f * pn)) / ((3.0f * N * Mathf.Pow(waveLength.y, 4.0f)) * (6.0f - 7.0f * pn))),
                rayleighCoefficient * (((8.0f * Mathf.Pow(Mathf.PI, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f))) * (6.0f + 3.0f * pn)) / ((3.0f * N * Mathf.Pow(waveLength.z, 4.0f)) * (6.0f - 7.0f * pn))),
                sunfade
            );
            return r;
        }

        // https://github.com/ngokevin/kframe/blob/master/components/sun-sky/shaders/fragment.glsl
        private static readonly Vector3 K = new Vector3(0.686f, 0.678f, 0.666f); // coefficient for primaries
        private static Vector4 GetTotalMie(Vector3 waveLength, float turbidity, float multiplier, WeatherMakerCelestialObjectScript sun)
        {
            const float v = 2.0f; // 4 - 2
            float sunY = -sun.transform.forward.y;
            float mieCoefficient = Mathf.Min(0.99f, 0.005f * multiplier * Mathf.Min(50.0f, 1.0f / Mathf.Pow(Mathf.Max(0.01f, sunY), 0.5f)));
            float c = (0.2f * turbidity) * 10E-18f;
            float d = 0.434f * c * Mathf.PI;
            Vector4 m = new Vector4
            (
               mieCoefficient * (d * Mathf.Pow((2.0f * Mathf.PI) / waveLength.x, v) * K.x),
               mieCoefficient * (d * Mathf.Pow((2.0f * Mathf.PI) / waveLength.y, v) * K.y),
               mieCoefficient * (d * Mathf.Pow((2.0f * Mathf.PI) / waveLength.z, v) * K.z),
               GetTotalSunIntensity(sun)
            );
            return m;
        }

        private static float GetTotalSunIntensity(WeatherMakerCelestialObjectScript sun)
        {
            // earth shadow hack
            const float cutoffAngle = 1.67510731556870734f;// Mathf.PI / 1.95f;
            const float steepness = 1.5f;
            const float EE = 1000.0f;

            Vector3 sunDir = sun.transform.forward;
            float zenithAngleCos = Vector3.Dot(sunDir, Vector3.up);
            return Mathf.Max(0.5f, EE * Mathf.Max(0.0f, 1.0f - Mathf.Exp(-((cutoffAngle - Mathf.Acos(zenithAngleCos)) / steepness))));
        }

        internal static void SetGlobalSkyParameters(Gradient skyGradient, float skyAtmosphereMie, float skyMieMultiplier, float skyRayleighMultiplier, float skyAtmosphereThickness,
            float skyAtmosphereTurbidity, float skyExposure, Color skyTintColor, float skyOuterRadius, float skyInnerRadius, float skyCameraHeight, Vector4 waveLength,
            WeatherMakerCelestialObjectScript sun)
        {
            if (sun == null)
            {
                return;
            }

            // global sky parameters
            float mieG = -skyAtmosphereMie;
            float mieG2 = skyAtmosphereMie * skyAtmosphereMie;
            float mieConstant = 0.001f * skyMieMultiplier;
            float rayleighConstant = 0.0025f * skyRayleighMultiplier;
            rayleighConstant = Mathf.LerpUnclamped(0.0f, rayleighConstant, Mathf.Pow(skyAtmosphereThickness, 2.5f));
            float redColor = waveLength.x;
            float greenColor = waveLength.y;
            float blueColor = waveLength.z;
            float lightRange = waveLength.w;
            float lightWaveLengthRedTint = Mathf.Lerp(redColor - lightRange, redColor + lightRange, 1.0f - skyTintColor.r);
            float lightWaveLengthGreenTint = Mathf.Lerp(greenColor - lightRange, greenColor + lightRange, 1.0f - skyTintColor.g);
            float lightWaveLengthBlueTint = Mathf.Lerp(blueColor - lightRange, blueColor + lightRange, 1.0f - skyTintColor.b);
            float lightWaveLengthRed4 = lightWaveLengthRedTint * lightWaveLengthRedTint * lightWaveLengthRedTint * lightWaveLengthRedTint;
            float lightWaveLengthGreen4 = lightWaveLengthGreenTint * lightWaveLengthGreenTint * lightWaveLengthGreenTint * lightWaveLengthGreenTint;
            float lightWaveLengthBlue4 = lightWaveLengthBlueTint * lightWaveLengthBlueTint * lightWaveLengthBlueTint * lightWaveLengthBlueTint;
            float lightInverseWaveLengthRed4 = 1.0f / lightWaveLengthRed4;
            float lightInverseWaveLengthGreen4 = 1.0f / lightWaveLengthGreen4;
            float lightInverseWaveLengthBlue4 = 1.0f / lightWaveLengthBlue4;
            const float sunBrightnessFactor = 40.0f;
            float sunRed = rayleighConstant * sunBrightnessFactor * lightInverseWaveLengthRed4;
            float sunGreen = rayleighConstant * sunBrightnessFactor * lightInverseWaveLengthGreen4;
            float sunBlue = rayleighConstant * sunBrightnessFactor * lightInverseWaveLengthBlue4;
            float sunIntensity = mieConstant * sunBrightnessFactor;
            float pi4Red = rayleighConstant * 4.0f * skyAtmosphereTurbidity * Mathf.PI * lightInverseWaveLengthRed4;
            float pi4Green = rayleighConstant * 4.0f * skyAtmosphereTurbidity * Mathf.PI * lightInverseWaveLengthGreen4;
            float pi4Blue = rayleighConstant * 4.0f * skyAtmosphereTurbidity * Mathf.PI * lightInverseWaveLengthBlue4;
            float pi4Intensity = mieConstant * 4.0f * skyAtmosphereTurbidity * Mathf.PI;
            float scaleFactor = 1.0f / (skyOuterRadius - 1.0f);
            const float scaleDepth = 0.25f;
            float scaleOverScaleDepth = scaleFactor / scaleDepth;
            if (sun != null)
            {
                Vector4 col =sun.GetGradientColor(skyGradient);
                col *= skyExposure;
                Shader.SetGlobalVector(WMS._WeatherMakerSkyTintColor, col);
                float mieDot = sun.GetSunMieDot();
                Shader.SetGlobalVector(WMS._WeatherMakerSkyMieG, new Vector4(mieG, mieG2, -mieG, mieDot));
            }
            Shader.SetGlobalVector(WMS._WeatherMakerSkyAtmosphereParams, new Vector4(skyAtmosphereThickness, skyAtmosphereTurbidity, skyExposure, 0.0f));
            Shader.SetGlobalVector(WMS._WeatherMakerSkyRadius, new Vector4(skyOuterRadius, skyOuterRadius * skyOuterRadius, skyInnerRadius, skyInnerRadius * skyInnerRadius));
            Shader.SetGlobalVector(WMS._WeatherMakerSkyMie, new Vector4(1.5f * ((1.0f - mieG2) / (2.0f + mieG2)), 1.0f + mieG2, 2.0f + mieG, 0.0f));
            Shader.SetGlobalVector(WMS._WeatherMakerSkyLightScattering, new Vector4(sunRed, sunGreen, sunBlue, sunIntensity));
            Shader.SetGlobalVector(WMS._WeatherMakerSkyLightPIScattering, new Vector4(pi4Red, pi4Green, pi4Blue, pi4Intensity));
            Shader.SetGlobalVector(WMS._WeatherMakerSkyScale, new Vector4(scaleFactor, scaleDepth, scaleOverScaleDepth, skyCameraHeight));

            // reduce wave length for preetham sky formulas
            waveLength *= 1E-6f;
            Shader.SetGlobalVector(WMS._WeatherMakerSkyTotalRayleigh, GetTotalRayleigh(waveLength, skyRayleighMultiplier * skyAtmosphereThickness, sun));
            Shader.SetGlobalVector(WMS._WeatherMakerSkyTotalMie, GetTotalMie(waveLength, skyAtmosphereTurbidity, skyMieMultiplier, sun));
        }

        private void SetShaderSkyParameters(Camera camera, Material material, WeatherMakerCelestialObjectScript sun)
        {
            material.mainTexture = DayTexture;
            material.SetInt(WMS._WeatherMakerSkyRenderType, (int)SkyMode);
            material.SetTexture(WMS._DawnDuskTex, DawnDuskTexture);
            material.SetTexture(WMS._NightTex, NightTexture);

            SetGlobalSkyParameters(SkyGradient, SkyAtmosphereMie, SkyMieMultiplier, SkyRayleighMultiplier, SkyAtmosphereThickness, SkyAtmosphereTurbidity,
                SkyExposure, SkyTintColor, SkyOuterRadius, SkyInnerRadius, SkyCameraHeight + (camera.transform.position.y * 0.0000001f), LightWaveLengths,
                sun);

            bool gamma = (QualitySettings.activeColorSpace == ColorSpace.Gamma);
            float radius = (camera.farClipPlane * FarClipPlaneMultiplier) * 0.95f;
            Shader.SetGlobalFloat(WMS._WeatherMakerSkySphereRadius, radius);
            Shader.SetGlobalFloat(WMS._WeatherMakerSkySphereRadiusSquared, radius * radius);
            Shader.SetGlobalFloat(WMS._WeatherMakerSkyDitherLevel, (gamma ? DitherLevel : DitherLevel * 0.5f));
            Shader.SetGlobalFloat(WMS._WeatherMakerSkyYOffset2D, YOffsetMultiplier);
        }

        private void SetShaderLightParameters(Material material)
        {
            if (WeatherMakerDayNightCycleManagerScript.Instance == null)
            {
                return;
            }

            material.SetFloat(WMS._NightSkyMultiplier, Mathf.Max(1.0f - Mathf.Min(1.0f, SkyAtmosphereThickness), WeatherMakerDayNightCycleManagerScript.Instance.NightMultiplier));
            material.SetFloat(WMS._NightVisibilityThreshold, NightVisibilityThreshold);
            material.SetFloat(WMS._NightIntensity, NightIntensity);
            material.SetFloat(WMS._NightPower, NightPower);
            material.SetFloat(WMS._NightDuskMultiplier, NightDuskMultiplier);
            if (NightTwinkleRandomness > 0.0f || (NightTwinkleVariance > 0.0f && NightTwinkleSpeed > 0.0f))
            {
                material.SetFloat(WMS._NightTwinkleSpeed, NightTwinkleSpeed);
                material.SetFloat(WMS._NightTwinkleVariance, NightTwinkleVariance);
                material.SetFloat(WMS._NightTwinkleMinimum, NightTwinkleMinimum);
                material.SetFloat(WMS._NightTwinkleRandomness, NightTwinkleRandomness);
                material.SetInt(WMS._WeatherMakerSkyEnableNightTwinkle, 1);
            }
            else
            {
                material.SetInt(WMS._WeatherMakerSkyEnableNightTwinkle, 0);
            }
        }

        private void RaycastForEclipse(Camera camera, Material material)
        {
            if (WeatherMakerLightManagerScript.Instance == null)
            {
                return;
            }

            // disable allow eclipses everywhere by default
            float eclipsePower = 0.0f;
            foreach (WeatherMakerCelestialObjectScript moon in WeatherMakerLightManagerScript.Instance.Moons)
            {
                Shader.SetGlobalInt(WMS._WeatherMakerSkyEnableSunEclipse, 0);
                if (moon.Collider != null)
                {
                    moon.Collider.enabled = CheckForEclipse;
                }
            }

            if (CheckForEclipse)
            {
                WeatherMakerCelestialObjectScript sun = WeatherMakerLightManagerScript.SunForCamera(camera);
                if (sun != null)
                {
                    float sunRadius = Mathf.Lerp(0.0f, 1000.0f, Mathf.Pow(sun.Scale, 0.15f));
                    Vector3 origin = camera.transform.position - (sun.transform.forward * camera.farClipPlane * 1.7f);
                    int hitCount = Physics.SphereCastNonAlloc(origin, sunRadius, sun.transform.forward, eclipseHits, camera.farClipPlane);
                    for (int i = 0; i < hitCount; i++)
                    {
                        foreach (WeatherMakerCelestialObjectScript moon in WeatherMakerLightManagerScript.Instance.Moons)
                        {
                            if (moon != null && moon.transform.gameObject == eclipseHits[i].collider.gameObject)
                            {
                                float dot = Mathf.Abs(Vector3.Dot(eclipseHits[i].normal, sun.transform.forward));
                                eclipsePower += Mathf.Pow(dot, 256.0f);
                                Shader.SetGlobalInt(WMS._WeatherMakerSkyEnableSunEclipse, 1);
                                //Debug.LogFormat("Eclipse raycast normal: {0}, dot: {1}, power: {2}", eclipseHits[i].normal, dot, eclipsePower);
                                break;
                            }
                        }
                    }
                }
            }

            if (eclipsePower == 0.0f)
            {
                WeatherMakerLightManagerScript.Instance.DirectionalLightIntensityMultipliers["WeatherMakerSkySphereScriptEclipse"] = 1.0f;
            }
            else
            {
                float eclipseLightReducer = 1.0f - Mathf.Clamp(eclipsePower, 0.0f, 1.0f);
                WeatherMakerLightManagerScript.Instance.DirectionalLightIntensityMultipliers["WeatherMakerSkySphereScriptEclipse"] = eclipseLightReducer;
                material.SetFloat(WMS._NightSkyMultiplier, Mathf.Max(1.0f - Mathf.Min(1.0f, SkyAtmosphereThickness), Mathf.Max(eclipsePower, WeatherMakerDayNightCycleManagerScript.Instance.NightMultiplier)));
            }
        }

        private void SetSkySphereScalesAndPositions(Camera camera, GameObject skySphere)
        {
            // adjust sky sphere position and scale
            float farPlane = camera.farClipPlane * FarClipPlaneMultiplier * 0.9f;
            Vector3 anchor = camera.transform.position;
            float scale = farPlane * 0.5f;
            float finalScale;
            skySphere.transform.localScale = new Vector3(scale, scale, scale);
            skySphere.transform.position = camera.transform.position;

            // move sun back near the far plane and scale appropriately
            foreach (WeatherMakerCelestialObjectScript sun in WeatherMakerLightManagerScript.Instance.Suns)
            {
                if (sun != null && sun.OrbitTypeIsPerspective)
                {
                    Vector3 sunOffset = (sun.transform.forward * ((farPlane * 0.9f) - scale));
                    sun.transform.position = anchor - sunOffset;
                    sun.transform.localScale = new Vector3(scale, scale, scale);
                }
            }

            // move moons back near the far plane and scale appropriately
            foreach (WeatherMakerCelestialObjectScript moon in WeatherMakerLightManagerScript.Instance.Moons)
            {
                if (moon != null && moon.OrbitTypeIsPerspective)
                {
                    scale = farPlane * moon.Scale;
                    finalScale = Mathf.Clamp(Mathf.Abs(moon.transform.forward.y) * 3.0f, 0.8f, 1.0f);
                    finalScale = scale / finalScale;
                    Vector3 moonOffset = (moon.transform.forward * (farPlane - finalScale));
                    moon.transform.position = anchor - moonOffset;
                    moon.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
                }
            }
        }

        private void SetSkyPlaneScalesAndPositions(Camera camera, GameObject skyPlane)
        {
            float orthoHeight = camera.orthographicSize * 2.0f;
            skyPlane.transform.position = camera.transform.position + (Vector3.forward * camera.farClipPlane * 0.99f);
            skyPlane.transform.localScale = new Vector3(orthoHeight * camera.aspect, orthoHeight, 1.0f);
        }

        /// <summary>
        /// Perform updates to the sky sphere based on profile settings
        /// </summary>
        /// <param name="camera">Current camera</param>
        /// <param name="material">Sky sphere material</param>
        /// <param name="skySphere">Sky sphere game object</param>
        /// <param name="sun">The sun</param>
        public void UpdateSkySphere(Camera camera, Material material, GameObject skySphere, WeatherMakerCelestialObjectScript sun)
        {
            if (material == null || WeatherMakerLightManagerScript.Instance == null || sun == null)
            {
                return;
            }
            SetSkySphereScalesAndPositions(camera, skySphere);
            SetShaderLightParameters(material);
            RaycastForEclipse(camera, material);
            SetShaderSkyParameters(camera, material, sun);
            shaderProps = (shaderProps ?? new WeatherMakerShaderPropertiesScript(material));
            shaderProps.Update(material);
            WeatherMakerLightManagerScript.Instance.UpdateShaderVariables(camera, shaderProps, null);
        }

        /// <summary>
        /// Perform updates to the sky plane based on profile settings
        /// </summary>
        /// <param name="camera">Current camera</param>
        /// <param name="material">Sky sphere material</param>
        /// <param name="skyPlane">Sky plane game object</param>
        /// <param name="sun">The sun</param>
        public void UpdateSkyPlane(Camera camera, Material material, GameObject skyPlane, WeatherMakerCelestialObjectScript sun)
        {
            if (material == null || WeatherMakerLightManagerScript.Instance == null || sun == null)
            {
                return;
            }
            SetSkyPlaneScalesAndPositions(camera, skyPlane);
            SetShaderLightParameters(material);
            SetShaderSkyParameters(camera, material, sun);
            shaderProps = (shaderProps ?? new WeatherMakerShaderPropertiesScript(material));
            shaderProps.Update(material);
            WeatherMakerLightManagerScript.Instance.UpdateShaderVariables(camera, shaderProps, null);
        }
    }
}
