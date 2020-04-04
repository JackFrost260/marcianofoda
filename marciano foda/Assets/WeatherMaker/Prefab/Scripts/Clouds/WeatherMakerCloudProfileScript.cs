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
    public enum WeatherMakerCloudType
    {
        None = 0,
        Light = 10,
        LightScattered = 15,
        LightMedium = 20,
        LightMediumScattered = 25,
        Medium = 30,
        MediumScattered = 35,
        MediumHeavy = 40,
        MediumHeavyScattered = 45,
        HeavyDark = 50,
        HeavyScattered = 55,
        HeavyBright = 60,
        Storm = 70,
        Custom = 250
    }

    [CreateAssetMenu(fileName = "WeatherMakerCloudProfile", menuName = "WeatherMaker/Cloud Profile", order = 40)]
    public class WeatherMakerCloudProfileScript : ScriptableObject
    {
        [Header("Layers")]
        [Tooltip("The first, and lowest cloud layer, null for none")]
        public WeatherMakerCloudLayerProfileScript CloudLayer1;

        [Tooltip("The second, and second lowest cloud layer, null for none")]
        public WeatherMakerCloudLayerProfileScript CloudLayer2;

        [Tooltip("The third, and third lowest cloud layer, null for none")]
        public WeatherMakerCloudLayerProfileScript CloudLayer3;

        [Tooltip("The fourth, and highest cloud layer, null for none")]
        public WeatherMakerCloudLayerProfileScript CloudLayer4;

        [Tooltip("Allow a single layer of volumetric clouds. In the future, more volumetric layers might be supported")]
        public WeatherMakerCloudVolumetricProfileScript CloudLayerVolumetric1;

        [Header("Lighting")]
        [Tooltip("How much to multiply directional light intensities by when clouds are showing. Ignored for volumetric clouds.")]
        [Range(0.0f, 1.0f)]
        public float DirectionalLightIntensityMultiplier = 1.0f;

        [Tooltip("How much to multiply directional light shadow strengths by when clouds are showing. Ignored for volumetric clouds.")]
        [Range(0.0f, 1.0f)]
        public float DirectionalLightShadowStrengthMultiplier = 1.0f;

        [Tooltip("How much clouds affect directional light shadow strength, lower values ensure no reduction. Ignored for volumetric clouds.")]
        [Range(0.0f, 3.0f)]
        public float CloudShadowStrength = 1.0f;

        [Tooltip("How much clouds affect directional light intensity, lower values ensure no reduction. Ignorec for volumetric clouds.")]
        [Range(0.0f, 3.0f)]
        public float CloudLightStrength = 1.0f;

        [Tooltip("Add a global shadow for volumetric clouds. This will cast at a minimum this amount of shadow everywhere. 1 for none, 0 for full shadow.")]
        [Range(0.0f, 1.0f)]
        public float CloudVolumetricShadow = 1.0f;

        [Tooltip("Cloud dither level, helps with night clouds banding")]
        [Range(0.0f, 1.0f)]
        public float CloudDitherLevel = 0.0008f;

        [Header("Weather map (volumetric only)")]
        [Tooltip("Set a custom weather map texture, bypassing the auto-generated weather map")]
        public Texture WeatherMapRenderTextureOverride;

        [Tooltip("Set a custom weather map texture mask, this will mask out all areas of the weather map based on lower alpha values.")]
        public Texture WeatherMapRenderTextureMask;

        [Tooltip("Velocity of weather map mask in uv coordinates (0 - 1)")]
        public Vector2 WeatherMapRenderTextureMaskVelocity;

        [Tooltip("Offset of weather map mask (0 - 1). Velocity is applied automatically but you can set manually as well.")]
        public Vector2 WeatherMapRenderTextureMaskOffset;

        [Tooltip("Clamp for weather map mask offset to ensure that it does not go too far out of bounds.")]
        public Vector2 WeatherMapRenderTextureMaskOffsetClamp = new Vector2(-1.1f, 1.1f);

        [Tooltip("Weather map scale, x,y = noise generation multiplier, z = world scale.")]
        public Vector3 WeatherMapScale = new Vector3(1.0f, 1.0f, 0.00001f);

        [Tooltip("Weather map cloud coverage velocity, xy units per second, z change per second")]
        public Vector3 WeatherMapCloudCoverageVelocity = new Vector3(11.0f, 15.0f, 0.0f);

        [MinMaxSlider(0.01f, 100.0f, "Scale of cloud coverage. Higher values produce smaller clouds.")]
        public RangeOfFloats WeatherMapCloudCoverageScale = new RangeOfFloats(4.0f, 16.0f);

        [MinMaxSlider(-360.0f, 360.0f, "Rotation of cloud coverage. Rotates coverage map around center of weather map.")]
        public RangeOfFloats WeatherMapCloudCoverageRotation;

        [MinMaxSlider(-1.0f, 1.0f, "Cloud coverage adder. Higher values create more cloud coverage.")]
        public RangeOfFloats WeatherMapCloudCoverageAdder = new RangeOfFloats { Minimum = 0.0f, Maximum = 0.0f };

        [MinMaxSlider(0.0f, 16.0f, "Cloud coverage power. Higher values create more firm cloud coverage edges.")]
        public RangeOfFloats WeatherMapCloudCoveragePower = new RangeOfFloats { Minimum = 1.0f, Maximum = 1.0f };

        [Tooltip("Weather map cloud type velocity, xy units per second, z change per second")]
        public Vector3 WeatherMapCloudTypeVelocity = new Vector3(17.0f, 10.0f, 0.0f);

        [MinMaxSlider(0.01f, 100.0f, "Scale of cloud types. Higher values produce more jagged clouds.")]
        public RangeOfFloats WeatherMapCloudTypeScale = new RangeOfFloats(2.0f, 8.0f);

        [MinMaxSlider(-360.0f, 360.0f, "Rotation of cloud type. Rotates cloud type map around center of weather map.")]
        public RangeOfFloats WeatherMapCloudTypeRotation;

        [MinMaxSlider(-1.0f, 1.0f, "Cloud type adder. Higher values create more cloud type.")]
        public RangeOfFloats WeatherMapCloudTypeAdder = new RangeOfFloats { Minimum = 0.0f, Maximum = 0.0f };

        [MinMaxSlider(0.0f, 16.0f, "Cloud type power. Higher values create more firm cloud type edges.")]
        public RangeOfFloats WeatherMapCloudTypePower = new RangeOfFloats { Minimum = 1.0f, Maximum = 1.0f };

        [Header("Planet (volumetric only)")]
        [Tooltip("Cloud height.")]
        [Range(0.0f, 20000.0f)]
        public float CloudHeight = 1500;

        [Tooltip("Cloud height top - clouds extend from CloudHeight to this value.")]
        [Range(100.0f, 10000.0f)]
        public float CloudHeightTop = 4000;

        [Tooltip("Planet radius for sphere cloud mapping. 1200000.0 seems to work well.")]
        public float CloudPlanetRadius = 1200000.0f;

        [Header("Camera")]
        [Tooltip("How much to scale camera position (xz), smaller values will cause the clouds to move less with the camera. 0 for no cloud movement at all.")]
        [Range(0.0f, 1.0f)]
        public float CameraPositionScale = 1.0f;

        private const float scaleReducer = 0.1f;

        /// <summary>
        /// Checks whether clouds are enabled
        /// </summary>
        public bool CloudsEnabled { get; private set; }

        /// <summary>
        /// Sum of cloud cover, max of 1
        /// </summary>
        public float CloudCoverTotal { get; private set; }

        /// <summary>
        /// Sum of cloud density, max of 1
        /// </summary>
        public float CloudDensityTotal { get; private set; }

        /// <summary>
        /// Sum of cloud light absorption, max of 1
        /// </summary>
        public float CloudLightAbsorptionTotal { get; private set; }

        /// <summary>
        /// A value of 0 to 1 that is a guide on how much to block the direct intensity of directional light, i.e. sun light reflecting off of water that makes the nice bright spots right in line of field of view to the sun
        /// </summary>
        public float CloudDirectionalLightDirectBlock { get; private set; }

        /// <summary>
        /// Cloud camera position
        /// </summary>
        public Vector3 CloudCameraPosition { get; private set; }

        /// <summary>
        /// Aurora profile
        /// </summary>
        public WeatherMakerAuroraProfileScript Aurora { get; set; }

        private Vector3 cloudNoiseVelocityAccum1;
        private Vector3 cloudNoiseVelocityAccum2;
        private Vector3 cloudNoiseVelocityAccum3;
        private Vector3 cloudNoiseVelocityAccum4;

        private Vector3 velocityAccumCoverage;
        private Vector3 velocityAccumType;
        internal float additionalCloudRayOffset;

        //unused currently
        //internal Vector3 cloudCoverageOffset;
        //internal Vector3 cloudTypeOffset;

        private static Vector4[] randomVectors;

        private readonly WeatherMakerShaderPropertiesScript shaderProps = new WeatherMakerShaderPropertiesScript();

        private void SetShaderVolumetricCloudShaderProperties(WeatherMakerShaderPropertiesScript props, Texture weatherMap)
        {
            props.SetVector(WMS._WeatherMakerCloudCameraPosition, CloudCameraPosition);
            props.SetTexture(WMS._WeatherMakerWeatherMapTexture, weatherMap);
            props.SetVector(WMS._WeatherMakerWeatherMapScale, WeatherMapScale);
            props.SetFloat(WMS._CloudCoverVolumetric, CloudLayerVolumetric1.CloudCover.LastValue);
            props.SetFloat(WMS._CloudCoverSecondaryVolumetric, CloudLayerVolumetric1.CloudCoverSecondary.LastValue);
            props.SetFloat(WMS._CloudTypeVolumetric, CloudLayerVolumetric1.CloudType.LastValue);
            props.SetFloat(WMS._CloudTypeSecondaryVolumetric, CloudLayerVolumetric1.CloudTypeSecondary.LastValue);
            props.SetFloat(WMS._CloudDensityVolumetric, CloudLayerVolumetric1.CloudDensity.LastValue);
            props.SetFloat(WMS._CloudHeightNoisePowerVolumetric, CloudLayerVolumetric1.CloudHeightNoisePowerVolumetric.LastValue);
            props.SetInt(WMS._CloudDirLightRaySampleCount, CloudLayerVolumetric1.CloudDirLightRaySampleCount);
            props.SetFloat(WMS._CloudPlanetRadiusVolumetric, CloudPlanetRadius);
            props.SetFloat(WMS._CloudNoiseScalarVolumetric, CloudLayerVolumetric1.CloudNoiseScalar.LastValue);
            props.SetTexture(WMS._CloudNoiseShapeVolumetric, CloudLayerVolumetric1.CloudNoiseShape);
            props.SetTexture(WMS._CloudNoiseDetailVolumetric, CloudLayerVolumetric1.CloudNoiseDetail);
            props.SetTexture(WMS._CloudNoiseCurlVolumetric, CloudLayerVolumetric1.CloudNoiseCurl);
            props.SetVector(WMS._CloudShapeAnimationVelocity, CloudLayerVolumetric1.CloudShapeAnimationVelocity);
            props.SetVector(WMS._CloudDetailAnimationVelocity, CloudLayerVolumetric1.CloudDetailAnimationVelocity);
            props.SetVector(WMS._CloudNoiseScaleVolumetric, CloudLayerVolumetric1.CloudNoiseScale);
            props.SetFloat(WMS._CloudNoiseScalarVolumetric, CloudLayerVolumetric1.CloudNoiseScalar.LastValue);
            props.SetFloat(WMS._CloudNoiseDetailPowerVolumetric, CloudLayerVolumetric1.CloudNoiseDetailPower.LastValue);
            props.SetFloat(WMS._CloudShapeNoiseMinVolumetric, CloudLayerVolumetric1.CloudShapeNoiseMin.LastValue);
            props.SetFloat(WMS._CloudShapeNoiseMaxVolumetric, CloudLayerVolumetric1.CloudShapeNoiseMax.LastValue);
            props.SetFloat(WMS._CloudBottomFadeVolumetric, CloudLayerVolumetric1.CloudBottomFade.LastValue);

            // lower cloud level sphere
            // assign global shader so shadow map can see them
            props.SetFloat(WMS._CloudStartVolumetric, CloudHeight);
            props.SetFloat(WMS._CloudStartSquaredVolumetric, CloudHeight * CloudHeight);
            props.SetFloat(WMS._CloudPlanetStartVolumetric, CloudHeight + CloudPlanetRadius);
            props.SetFloat(WMS._CloudPlanetStartSquaredVolumetric, Mathf.Pow(CloudHeight + CloudPlanetRadius, 2.0f));

            // height of top minus bottom cloud layer
            float height = CloudHeightTop - CloudHeight;
            props.SetFloat(WMS._CloudHeightVolumetric, height);
            props.SetFloat(WMS._CloudHeightInverseVolumetric, 1.0f / height);
            height *= height;
            props.SetFloat(WMS._CloudHeightSquaredVolumetric, height);
            props.SetFloat(WMS._CloudHeightSquaredInverseVolumetric, 1.0f / height);

            // upper cloud level sphere
            props.SetFloat(WMS._CloudEndVolumetric, CloudHeightTop);
            height = CloudHeightTop * CloudHeightTop;
            props.SetFloat(WMS._CloudEndSquaredVolumetric, height);
            props.SetFloat(WMS._CloudEndSquaredInverseVolumetric, 1.0f / height);
            props.SetFloat(WMS._CloudPlanetEndVolumetric, CloudHeightTop + CloudPlanetRadius);
            props.SetFloat(WMS._CloudPlanetEndSquaredVolumetric, Mathf.Pow(CloudHeightTop + CloudPlanetRadius, 2.0f));

            props.SetFloat(WMS._CloudPlanetRadiusNegativeVolumetric, -CloudPlanetRadius);
            props.SetFloat(WMS._CloudPlanetRadiusSquaredVolumetric, CloudPlanetRadius * CloudPlanetRadius);

            props.SetVector(WMS._CloudHenyeyGreensteinPhaseVolumetric, CloudLayerVolumetric1.CloudHenyeyGreensteinPhase);
            props.SetFloat(WMS._CloudRayOffsetVolumetric, CloudLayerVolumetric1.CloudRayOffset + additionalCloudRayOffset);
            props.SetFloat(WMS._CloudMinRayYVolumetric, CloudLayerVolumetric1.CloudMinRayY);
            props.SetFloat(WMS._CloudLightStepMultiplierVolumetric, CloudLayerVolumetric1.CloudLightStepMultiplier);

            props.SetVector(WMS._CloudGradientStratus, CloudLayerVolumetric1.CloudGradientStratusVector);
            props.SetVector(WMS._CloudGradientStratoCumulus, CloudLayerVolumetric1.CloudGradientStratoCumulusVector);
            props.SetVector(WMS._CloudGradientCumulus, CloudLayerVolumetric1.CloudGradientCumulusVector);
        }

        public void SetShaderCloudParameters(Material cloudMaterial, ComputeShader cloudProbe, Camera camera, Texture weatherMap)
        {
            if (WeatherMakerDayNightCycleManagerScript.Instance == null || WeatherMakerLightManagerScript.Instance == null)
            {
                return;
            }

            WeatherMakerCelestialObjectScript sun = (camera == null || !camera.orthographic ? WeatherMakerLightManagerScript.Instance.SunPerspective : WeatherMakerLightManagerScript.Instance.SunOrthographic);
            if (sun == null)
            {
                return;
            }

            CloudLayerVolumetric1.CloudGradientStratusVector = WeatherMakerCloudVolumetricProfileScript.CloudHeightGradientToVector4(CloudLayerVolumetric1.CloudGradientStratus);
            CloudLayerVolumetric1.CloudGradientStratoCumulusVector = WeatherMakerCloudVolumetricProfileScript.CloudHeightGradientToVector4(CloudLayerVolumetric1.CloudGradientStratoCumulus);
            CloudLayerVolumetric1.CloudGradientCumulusVector = WeatherMakerCloudVolumetricProfileScript.CloudHeightGradientToVector4(CloudLayerVolumetric1.CloudGradientCumulus);

            shaderProps.Update(null);
            SetShaderVolumetricCloudShaderProperties(shaderProps, weatherMap);
            shaderProps.Update(cloudMaterial);
            SetShaderVolumetricCloudShaderProperties(shaderProps, weatherMap);
            if (cloudProbe != null)
            {
                shaderProps.Update(cloudProbe);
                SetShaderVolumetricCloudShaderProperties(shaderProps, weatherMap);
            }
            

            if (WeatherMakerScript.Instance == null)
            {
                Shader.SetGlobalInt(WMS._CloudVolumetricShadowSampleCount, 8);
            }
            else
            {
                Shader.SetGlobalInt(WMS._CloudVolumetricShadowSampleCount, WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudShadowSampleCount);
            }

            if (CloudsEnabled)
            {
                if (CloudLayerVolumetric1.CloudCover.LastValue > 0.001f)
                {
                    cloudMaterial.SetVector(WMS._CloudNoiseSampleCountVolumetric, (WeatherMakerScript.Instance == null ? CloudLayerVolumetric1.CloudNoiseSampleCount.ToVector2() :
                        WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudSampleCount.ToVector2()));
                    cloudMaterial.SetInt(WMS._CloudRaymarchSkipThreshold, (WeatherMakerScript.Instance == null ? CloudLayerVolumetric1.CloudRaymarchSkipThreshold :
                        WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudRaymarchSkipThreshold));
                    cloudMaterial.SetFloat(WMS._CloudRaymarchMaybeInCloudStepMultiplier, (WeatherMakerScript.Instance == null ? CloudLayerVolumetric1.CloudRaymarchMaybeInCloudStepMultiplier :
                        WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudRaymarchMaybeInCloudStepMultiplier));
                    cloudMaterial.SetFloat(WMS._CloudRaymarchInCloudStepMultiplier, (WeatherMakerScript.Instance == null ? CloudLayerVolumetric1.CloudRaymarchInCloudStepMultiplier :
                        WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudRaymarchInCloudStepMultiplier));
                    cloudMaterial.SetFloat(WMS._CloudRaymarchSkipMultiplier, (WeatherMakerScript.Instance == null ? CloudLayerVolumetric1.CloudRaymarchSkipMultiplier :
                        WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudRaymarchSkipMultiplier));
                    cloudMaterial.SetInt(WMS._CloudRaymarchSkipMultiplierMaxCount, (WeatherMakerScript.Instance == null ? CloudLayerVolumetric1.CloudRaymarchSkipMultiplierMaxCount :
                        WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudRaymarchSkipMultiplierMaxCount));

                    cloudMaterial.SetVector(WMS._CloudNoiseLodVolumetric, (WeatherMakerScript.Instance == null ? CloudLayerVolumetric1.CloudNoiseLod.ToVector2() :
                        WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudLod.ToVector2()));
                    cloudMaterial.SetColor(WMS._CloudColorVolumetric, CloudLayerVolumetric1.CloudColor);
                    if (CloudLayerVolumetric1.LerpCloudGradientColor == null)
                    {
                        Color gradColor = sun.GetGradientColor(CloudLayerVolumetric1.CloudDirLightGradientColor);
                        cloudMaterial.SetColor(WMS._CloudDirColorVolumetric, gradColor);
                    }
                    else
                    {
                        Color oldColor = sun.GetGradientColor(CloudLayerVolumetric1.LerpCloudGradientColor);
                        Color newColor = sun.GetGradientColor(CloudLayerVolumetric1.CloudDirLightGradientColor);
                        Color lerpColor = Color.Lerp(oldColor, newColor, CloudLayerVolumetric1.lerpProgress);
                        cloudMaterial.SetColor(WMS._CloudDirColorVolumetric, lerpColor);
                    }
                    cloudMaterial.SetColor(WMS._CloudEmissionColorVolumetric, CloudLayerVolumetric1.CloudEmissionColor);
                    cloudMaterial.SetFloat(WMS._CloudDirLightMultiplierVolumetric, CloudLayerVolumetric1.CloudDirLightMultiplier);
                    cloudMaterial.SetFloat(WMS._CloudPointSpotLightMultiplierVolumetric, CloudLayerVolumetric1.CloudPointSpotLightMultiplier);
                    cloudMaterial.SetFloat(WMS._CloudAmbientGroundIntensityVolumetric, CloudLayerVolumetric1.CloudAmbientGroundIntensity);
                    cloudMaterial.SetFloat(WMS._CloudAmbientSkyIntensityVolumetric, CloudLayerVolumetric1.CloudAmbientSkyIntensity);
                    cloudMaterial.SetFloat(WMS._CloudBackgroundSkyIntensityVolumetric, CloudLayerVolumetric1.CloudSkyIntensity);
                    cloudMaterial.SetFloat(WMS._CloudAmbientSkyHeightMultiplierVolumetric, CloudLayerVolumetric1.CloudAmbientSkyHeightMultiplier);
                    cloudMaterial.SetFloat(WMS._CloudAmbientGroundHeightMultiplierVolumetric, CloudLayerVolumetric1.CloudAmbientGroundHeightMultiplier);
                    cloudMaterial.SetFloat(WMS._CloudLightAbsorptionVolumetric, CloudLayerVolumetric1.CloudLightAbsorption);
                    cloudMaterial.SetFloat(WMS._CloudDirLightIndirectMultiplierVolumetric, CloudLayerVolumetric1.CloudDirLightIndirectMultiplier);
                    cloudMaterial.SetFloat(WMS._CloudPowderMultiplierVolumetric, CloudLayerVolumetric1.CloudPowderMultiplier.LastValue);
                    cloudMaterial.SetFloat(WMS._CloudOpticalDistanceMultiplierVolumetric, CloudLayerVolumetric1.CloudOpticalDistanceMultiplier);
                    Shader.SetGlobalFloat(WMS._CloudOpticalDistanceMultiplierVolumetric, CloudLayerVolumetric1.CloudOpticalDistanceMultiplier); // shadow map, etc. depend on this variable
                    cloudMaterial.SetFloat(WMS._CloudHorizonFadeMultiplierVolumetric, CloudLayerVolumetric1.CloudHorizonFadeMultiplier);
                    cloudMaterial.SetFloat(WMS._CloudDirLightSampleCount, (WeatherMakerScript.Instance == null ? CloudLayerVolumetric1.CloudDirLightSampleCount :
                        WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudDirLightSampleCount));

                    if (randomVectors == null)
                    {
                        randomVectors = new Vector4[6];
                        for (int i = 0; i < randomVectors.Length; i++)
                        {
                            randomVectors[i] = Random.onUnitSphere;
                        }
                        Shader.SetGlobalVectorArray(WMS._CloudConeRandomVectors, randomVectors);
                    }

                    if (WeatherMakerScript.Instance != null)
                    {
                        cloudMaterial.SetFloat(WMS._CloudMaxRayLengthMultiplierVolumetric, WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudMaxRayLengthMultiplier);
                        Shader.SetGlobalFloat(WMS._CloudMaxRayLengthMultiplierVolumetric, WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudMaxRayLengthMultiplier); // shadow map, etc. depend on this variable
                        cloudMaterial.SetFloat(WMS._CloudRayDitherVolumetric, WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudRayDither);
                        cloudMaterial.SetFloat(WMS._CloudRaymarchMultiplierVolumetric, WeatherMakerScript.Instance.PerformanceProfile.VolumetricCloudRaymarchMultiplier);
                    }
                    else
                    {
                        cloudMaterial.SetFloat(WMS._CloudMaxRayLengthMultiplierVolumetric, CloudLayerVolumetric1.CloudMaxRayLengthMultiplier);
                        Shader.SetGlobalFloat(WMS._CloudMaxRayLengthMultiplierVolumetric, CloudLayerVolumetric1.CloudMaxRayLengthMultiplier); // shadow map, etc. depend on this variable
                        cloudMaterial.SetFloat(WMS._CloudRayDitherVolumetric, CloudLayerVolumetric1.CloudRayDither);
                        cloudMaterial.SetFloat(WMS._CloudRaymarchMultiplierVolumetric, CloudLayerVolumetric1.CloudRaymarchMultiplier);
                    }
                }

                // flat
                float cloudCover1 = 0.0f;
                float cloudCover2 = 0.0f;
                float cloudCover3 = 0.0f;
                float cloudCover4 = 0.0f;
                if (CloudLayerVolumetric1 == null || CloudLayerVolumetric1.CloudCover.Maximum == 0.0f ||
                    (CloudLayerVolumetric1.FlatLayerMask & WeatherMakerVolumetricCloudsFlatLayerMask.One) == WeatherMakerVolumetricCloudsFlatLayerMask.One)
                {
                    cloudCover1 = CloudLayer1.CloudCover;
                }
                if (CloudLayerVolumetric1 == null || CloudLayerVolumetric1.CloudCover.Maximum == 0.0f ||
                    (CloudLayerVolumetric1.FlatLayerMask & WeatherMakerVolumetricCloudsFlatLayerMask.Two) == WeatherMakerVolumetricCloudsFlatLayerMask.Two)
                {
                    cloudCover2 = CloudLayer2.CloudCover;
                }
                if (CloudLayerVolumetric1 == null || CloudLayerVolumetric1.CloudCover.Maximum == 0.0f ||
                    (CloudLayerVolumetric1.FlatLayerMask & WeatherMakerVolumetricCloudsFlatLayerMask.Three) == WeatherMakerVolumetricCloudsFlatLayerMask.Three)
                {
                    cloudCover3 = CloudLayer3.CloudCover;
                }
                if (CloudLayerVolumetric1 == null || CloudLayerVolumetric1.CloudCover.Maximum == 0.0f ||
                    (CloudLayerVolumetric1.FlatLayerMask & WeatherMakerVolumetricCloudsFlatLayerMask.Four) == WeatherMakerVolumetricCloudsFlatLayerMask.Four)
                {
                    cloudCover4 = CloudLayer4.CloudCover;
                }
                cloudMaterial.SetTexture(WMS._CloudNoise1, CloudLayer1.CloudNoise ?? Texture2D.blackTexture);
                cloudMaterial.SetTexture(WMS._CloudNoise2, CloudLayer2.CloudNoise ?? Texture2D.blackTexture);
                cloudMaterial.SetTexture(WMS._CloudNoise3, CloudLayer3.CloudNoise ?? Texture2D.blackTexture);
                cloudMaterial.SetTexture(WMS._CloudNoise4, CloudLayer4.CloudNoise ?? Texture2D.blackTexture);

                WMS.SetColorArray(cloudMaterial, WMS._CloudColor,
                    CloudLayer1.CloudColor * sun.GetGradientColor(CloudLayer1.CloudGradientColor),
                    CloudLayer2.CloudColor * sun.GetGradientColor(CloudLayer2.CloudGradientColor),
                    CloudLayer3.CloudColor * sun.GetGradientColor(CloudLayer3.CloudGradientColor),
                    CloudLayer4.CloudColor * sun.GetGradientColor(CloudLayer4.CloudGradientColor));
                WMS.SetColorArray(cloudMaterial, WMS._CloudEmissionColor,
                    CloudLayer1.CloudEmissionColor,
                    CloudLayer2.CloudEmissionColor,
                    CloudLayer3.CloudEmissionColor,
                    CloudLayer4.CloudEmissionColor);
                WMS.SetFloatArray(cloudMaterial, WMS._CloudAmbientMultiplier,
                    CloudLayer1.CloudAmbientMultiplier,
                    CloudLayer2.CloudAmbientMultiplier,
                    CloudLayer3.CloudAmbientMultiplier,
                    CloudLayer4.CloudAmbientMultiplier);
                WMS.SetFloatArray(cloudMaterial, WMS._CloudScatterMultiplier,
                    CloudLayer1.CloudScatterMultiplier,
                    CloudLayer2.CloudScatterMultiplier,
                    CloudLayer3.CloudScatterMultiplier,
                    CloudLayer4.CloudScatterMultiplier);
                WMS.SetVectorArray(cloudMaterial, WMS._CloudNoiseScale,
                    CloudLayer1.CloudNoiseScale * scaleReducer,
                    CloudLayer2.CloudNoiseScale * scaleReducer,
                    CloudLayer3.CloudNoiseScale * scaleReducer,
                    CloudLayer4.CloudNoiseScale * scaleReducer);
                WMS.SetVectorArray(cloudMaterial, WMS._CloudNoiseMultiplier,
                    CloudLayer1.CloudNoiseMultiplier,
                    CloudLayer2.CloudNoiseMultiplier,
                    CloudLayer3.CloudNoiseMultiplier,
                    CloudLayer4.CloudNoiseMultiplier);
                WMS.SetVectorArray(cloudMaterial, WMS._CloudNoiseVelocity, cloudNoiseVelocityAccum1, cloudNoiseVelocityAccum2, cloudNoiseVelocityAccum3, cloudNoiseVelocityAccum4);

                WMS.SetFloatArrayRotation(cloudMaterial, WMS._CloudNoiseRotation,
                    CloudLayer1.CloudNoiseRotation.LastValue,
                    CloudLayer2.CloudNoiseRotation.LastValue,
                    CloudLayer3.CloudNoiseRotation.LastValue,
                    CloudLayer4.CloudNoiseRotation.LastValue);
                /*
                if (CloudLayer1.CloudNoiseMask != null || CloudLayer2.CloudNoiseMask != null || CloudLayer3.CloudNoiseMask != null || CloudLayer4.CloudNoiseMask != null)
                {
                    cloudMaterial.SetTexture(WMS._CloudNoiseMask1, CloudLayer1.CloudNoiseMask ?? Texture2D.whiteTexture);
                    cloudMaterial.SetTexture(WMS._CloudNoiseMask2, CloudLayer2.CloudNoiseMask ?? Texture2D.whiteTexture);
                    cloudMaterial.SetTexture(WMS._CloudNoiseMask3, CloudLayer3.CloudNoiseMask ?? Texture2D.whiteTexture);
                    cloudMaterial.SetTexture(WMS._CloudNoiseMask4, CloudLayer4.CloudNoiseMask ?? Texture2D.whiteTexture);
                    WeatherMakerShaderIds.SetVectorArray(cloudMaterial, "_CloudNoiseMaskOffset",
                        CloudLayer1.CloudNoiseMaskOffset,
                        CloudLayer2.CloudNoiseMaskOffset,
                        CloudLayer3.CloudNoiseMaskOffset,
                        CloudLayer4.CloudNoiseMaskOffset);
                    WeatherMakerShaderIds.SetVectorArray(cloudMaterial, "_CloudNoiseMaskVelocity", cloudNoiseMaskVelocityAccum1, cloudNoiseMaskVelocityAccum2, cloudNoiseMaskVelocityAccum3, cloudNoiseMaskVelocityAccum4);
                    WeatherMakerShaderIds.SetFloatArray(cloudMaterial, "_CloudNoiseMaskScale",
                        (CloudLayer1.CloudNoiseMask == null ? 0.0f : CloudLayer1.CloudNoiseMaskScale * scaleReducer),
                        (CloudLayer2.CloudNoiseMask == null ? 0.0f : CloudLayer2.CloudNoiseMaskScale * scaleReducer),
                        (CloudLayer3.CloudNoiseMask == null ? 0.0f : CloudLayer3.CloudNoiseMaskScale * scaleReducer),
                        (CloudLayer4.CloudNoiseMask == null ? 0.0f : CloudLayer4.CloudNoiseMaskScale * scaleReducer));
                    WeatherMakerShaderIds.SetFloatArrayRotation(cloudMaterial, "_CloudNoiseMaskRotation",
                        CloudLayer1.CloudNoiseMaskRotation.LastValue,
                        CloudLayer2.CloudNoiseMaskRotation.LastValue,
                        CloudLayer3.CloudNoiseMaskRotation.LastValue,
                        CloudLayer4.CloudNoiseMaskRotation.LastValue);
                }
                */
                WMS.SetFloatArray(cloudMaterial, WMS._CloudHeight,
                    CloudLayer1.CloudHeight,
                    CloudLayer2.CloudHeight,
                    CloudLayer3.CloudHeight,
                    CloudLayer4.CloudHeight);
                WMS.SetFloatArray(cloudMaterial, WMS._CloudCover, cloudCover1, cloudCover2, cloudCover3, cloudCover4);
                WMS.SetFloatArray(cloudMaterial, WMS._CloudDensity,
                    CloudLayer1.CloudDensity,
                    CloudLayer2.CloudDensity,
                    CloudLayer3.CloudDensity,
                    CloudLayer4.CloudDensity);
                WMS.SetFloatArray(cloudMaterial, WMS._CloudLightAbsorption,
                    CloudLayer1.CloudLightAbsorption,
                    CloudLayer2.CloudLightAbsorption,
                    CloudLayer3.CloudLightAbsorption,
                    CloudLayer4.CloudLightAbsorption);
                WMS.SetFloatArray(cloudMaterial, WMS._CloudHorizonFadeMultiplier,
                    CloudLayer1.CloudHorizonFade,
                    CloudLayer2.CloudHorizonFade,
                    CloudLayer3.CloudHorizonFade,
                    CloudLayer4.CloudHorizonFade);
                WMS.SetFloatArray(cloudMaterial, WMS._CloudSharpness,
                    CloudLayer1.CloudSharpness,
                    CloudLayer2.CloudSharpness,
                    CloudLayer3.CloudSharpness,
                    CloudLayer4.CloudSharpness);
                WMS.SetFloatArray(cloudMaterial, WMS._CloudRayOffset,
                    CloudLayer1.CloudRayOffset,
                    CloudLayer2.CloudRayOffset,
                    CloudLayer3.CloudRayOffset,
                    CloudLayer4.CloudRayOffset);

                float cover = CloudCoverTotal * (1.5f - CloudLightAbsorptionTotal);
                float sunIntensityMultiplier = Mathf.Clamp(1.0f - (cover * CloudLightStrength), 0.2f, 1.0f);
                float sunShadowMultiplier = Mathf.Lerp(1.0f, 0.0f, Mathf.Clamp(((CloudDensityTotal + cover) * CloudShadowStrength), 0.0f, 1.0f));
                float sunIntensityMultiplierWithoutLightStrength = Mathf.Clamp(1.0f - (cover * cover * 0.85f), 0.2f, 1.0f);
                float cloudShadowReducer = sunIntensityMultiplierWithoutLightStrength;
                Shader.SetGlobalFloat(WMS._WeatherMakerCloudGlobalShadow2, cloudShadowReducer);
                cloudShadowReducer = Mathf.Min(cloudShadowReducer, Shader.GetGlobalFloat(WMS._WeatherMakerCloudGlobalShadow));

                // if we have volumetric clouds and a sun or moon with shadows and we have a screen space shadow texture, use screen space shadows
                if (CloudLayerVolumetric1.CloudCover.LastValue > 0.001f &&
                    QualitySettings.shadows != ShadowQuality.Disable &&
                    WeatherMakerLightManagerScript.ScreenSpaceShadowMode != UnityEngine.Rendering.BuiltinShaderMode.Disabled &&
                    WeatherMakerLightManagerScript.Instance != null &&
                    ((WeatherMakerLightManagerScript.Instance.SunPerspective != null &&
                    WeatherMakerLightManagerScript.Instance.SunPerspective.LightIsOn &&
                    WeatherMakerLightManagerScript.Instance.SunPerspective.Light.shadows != LightShadows.None) ||
                    (WeatherMakerLightManagerScript.Instance.Moons.Count > 0 &&
                    WeatherMakerLightManagerScript.Instance.Moons[0].LightIsOn &&
                    WeatherMakerLightManagerScript.Instance.Moons[0].Light.shadows != LightShadows.None)))
                {
                    // do not reduce light intensity or shadows, screen space shadows are being used
                    WeatherMakerLightManagerScript.Instance.DirectionalLightIntensityMultipliers.Remove("WeatherMakerFullScreenCloudsScript");
                    WeatherMakerLightManagerScript.Instance.DirectionalLightShadowIntensityMultipliers.Remove("WeatherMakerFullScreenCloudsScript");
                }
                else
                {
                    Shader.SetGlobalFloat(WMS._WeatherMakerCloudGlobalShadow, cloudShadowReducer);

                    // brighten up on orthographic, looks better
                    if (WeatherMakerScript.Instance.MainCamera.orthographic)
                    {
                        sunIntensityMultiplier = Mathf.Min(1.0f, sunIntensityMultiplier * 2.0f);
                    }

                    // we rely on sun intensity and shadow reduction to reduce weather maker effects, we are not getting cloud shadows
                    WeatherMakerLightManagerScript.Instance.DirectionalLightIntensityMultipliers["WeatherMakerFullScreenCloudsScript"] = sunIntensityMultiplier * Mathf.Lerp(1.0f, DirectionalLightIntensityMultiplier, cover);
                    WeatherMakerLightManagerScript.Instance.DirectionalLightShadowIntensityMultipliers["WeatherMakerFullScreenCloudsScript"] = sunShadowMultiplier * Mathf.Lerp(1.0f, DirectionalLightShadowStrengthMultiplier, cover);
                }

                cloudMaterial.SetFloat(WMS._WeatherMakerCloudDitherLevel, CloudDitherLevel);
            }
            else
            {
                WeatherMakerLightManagerScript.Instance.DirectionalLightIntensityMultipliers.Remove("WeatherMakerFullScreenCloudsScript");
                WeatherMakerLightManagerScript.Instance.DirectionalLightShadowIntensityMultipliers.Remove("WeatherMakerFullScreenCloudsScript");
            }
        }

        private void LoadDefaultLayerIfNeeded(ref WeatherMakerCloudLayerProfileScript script)
        {
            if (script == null)
            {
                script = Resources.Load<WeatherMakerCloudLayerProfileScript>("WeatherMakerCloudLayerProfile_None");
            }
        }

        private void LoadDefaultLayerIfNeeded(ref WeatherMakerCloudVolumetricProfileScript script)
        {
            if (script == null)
            {
                script = Resources.Load<WeatherMakerCloudVolumetricProfileScript>("WeatherMakerCloudLayerProfileVolumetric_None");
            }
        }

        private void UpdateWeatherMap(WeatherMakerShaderPropertiesScript props, Texture weatherMap, float weatherMapSeed)
        {
            props.SetTexture(WMS._WeatherMakerWeatherMapTexture, weatherMap);
            props.SetFloat(WMS._WeatherMakerCloudVolumetricShadow, CloudVolumetricShadow);
            props.SetVector(WMS._WeatherMakerWeatherMapScale, WeatherMapScale);
            props.SetFloat(WMS._CloudCoverVolumetric, CloudLayerVolumetric1.CloudCover.LastValue);
            props.SetFloat(WMS._CloudCoverSecondaryVolumetric, CloudLayerVolumetric1.CloudCoverSecondary.LastValue);
            props.SetFloat(WMS._CloudDensityVolumetric, CloudLayerVolumetric1.CloudDensity.LastValue);
            props.SetFloat(WMS._CloudTypeVolumetric, CloudLayerVolumetric1.CloudType.LastValue);
            props.SetFloat(WMS._CloudTypeSecondaryVolumetric, CloudLayerVolumetric1.CloudTypeSecondary.LastValue);
            props.SetVector(WMS._CloudCoverageVelocity, velocityAccumCoverage);
            props.SetVector(WMS._CloudTypeVelocity, velocityAccumType);
            props.SetFloat(WMS._CloudCoverageFrequency, WeatherMapCloudCoverageScale.LastValue);
            props.SetFloat(WMS._CloudTypeFrequency, WeatherMapCloudTypeScale.LastValue);
            float r = WeatherMapCloudCoverageRotation.LastValue * Mathf.Deg2Rad;
            props.SetVector(WMS._CloudCoverageRotation, new Vector2(Mathf.Sin(r), Mathf.Cos(r)));
            r = WeatherMapCloudTypeRotation.LastValue * Mathf.Deg2Rad;
            props.SetVector(WMS._CloudTypeRotation, new Vector2(Mathf.Sin(r), Mathf.Cos(r)));
            props.SetFloat(WMS._CloudCoverageAdder, WeatherMapCloudCoverageAdder.LastValue);
            //weatherMapMaterial.SetVector(WMS._CloudCoverageOffset, cloudCoverageOffset);
            props.SetFloat(WMS._CloudCoveragePower, WeatherMapCloudCoveragePower.LastValue);
            props.SetFloat(WMS._CloudTypeAdder, WeatherMapCloudTypeAdder.LastValue);
            //weatherMapMaterial.SetVector(WMS._CloudTypeOffset, cloudTypeOffset);
            props.SetFloat(WMS._CloudTypePower, WeatherMapCloudTypePower.LastValue);
            props.SetVector(WMS._MaskOffset, WeatherMapRenderTextureMaskOffset);
            props.SetFloat(WMS._WeatherMakerWeatherMapSeed, weatherMapSeed);
            props.SetVector(WMS._WeatherMakerCloudCameraPosition, CloudCameraPosition);
        }

        public void UpdateWeatherMap(Material weatherMapMaterial, Camera camera, ComputeShader cloudProbeShader, RenderTexture weatherMap, float weatherMapSeed)
        {
            if (CloudLayerVolumetric1 == null)
            {
                return;
            }
            if (camera == null)
            {
                camera = Camera.main;
                if (camera == null)
                {
                    return;
                }
            }

            Vector3 cameraPos = camera.transform.position;
            cameraPos.x *= CameraPositionScale;
            cameraPos.z *= CameraPositionScale;
            CloudCameraPosition = cameraPos;
            shaderProps.Update(null);
            UpdateWeatherMap(shaderProps, weatherMap, weatherMapSeed);
            shaderProps.Update(weatherMapMaterial);
            UpdateWeatherMap(shaderProps, weatherMap, weatherMapSeed);
            if (cloudProbeShader != null)
            {
                shaderProps.Update(cloudProbeShader);
                UpdateWeatherMap(shaderProps, weatherMap, weatherMapSeed);
            }
        }

        public void EnsureNonNullLayers()
        {
            LoadDefaultLayerIfNeeded(ref CloudLayer1);
            LoadDefaultLayerIfNeeded(ref CloudLayer2);
            LoadDefaultLayerIfNeeded(ref CloudLayer3);
            LoadDefaultLayerIfNeeded(ref CloudLayer4);
            LoadDefaultLayerIfNeeded(ref CloudLayerVolumetric1);
        }

        public WeatherMakerCloudProfileScript Clone()
        {
            WeatherMakerCloudProfileScript clone = ScriptableObject.Instantiate(this);
            clone.EnsureNonNullLayers();
            clone.CloudLayer1 = ScriptableObject.Instantiate(clone.CloudLayer1);
            clone.CloudLayer2 = ScriptableObject.Instantiate(clone.CloudLayer2);
            clone.CloudLayer3 = ScriptableObject.Instantiate(clone.CloudLayer3);
            clone.CloudLayer4 = ScriptableObject.Instantiate(clone.CloudLayer4);
            clone.CloudLayerVolumetric1 = ScriptableObject.Instantiate(clone.CloudLayerVolumetric1);
            CopyStateTo(clone);
            return clone;
        }

        public void Update()
        {
            EnsureNonNullLayers();
            CloudsEnabled =
            (
                (CloudLayerVolumetric1.CloudColor.a > 0.0f && CloudLayerVolumetric1.CloudCover.LastValue > 0.001f) ||
                (CloudLayer1.CloudNoise != null && CloudLayer1.CloudColor.a > 0.0f && CloudLayer1.CloudCover > 0.0f) ||
                (CloudLayer2.CloudNoise != null && CloudLayer2.CloudColor.a > 0.0f && CloudLayer2.CloudCover > 0.0f) ||
                (CloudLayer3.CloudNoise != null && CloudLayer3.CloudColor.a > 0.0f && CloudLayer3.CloudCover > 0.0f) ||
                (CloudLayer4.CloudNoise != null && CloudLayer4.CloudColor.a > 0.0f && CloudLayer4.CloudCover > 0.0f) ||
                (Aurora != null && Aurora.AuroraEnabled)
            );
            CloudCoverTotal = Mathf.Min(1.0f, (CloudLayer1.CloudCover + CloudLayer2.CloudCover + CloudLayer3.CloudCover + CloudLayer4.CloudCover +
                (CloudLayerVolumetric1.CloudCover.LastValue)));
            CloudDensityTotal = Mathf.Min(1.0f,
                (CloudLayerVolumetric1.CloudCover.LastValue * CloudLayerVolumetric1.CloudDensity.LastValue) +
                (CloudLayer1.CloudCover * CloudLayer1.CloudDensity) +
                (CloudLayer2.CloudCover * CloudLayer2.CloudDensity) +
                (CloudLayer3.CloudCover * CloudLayer1.CloudDensity) +
                (CloudLayer4.CloudCover * CloudLayer4.CloudDensity));
            CloudLightAbsorptionTotal = Mathf.Min(1.0f,
                 (Mathf.Clamp(1.0f - (CloudLayerVolumetric1.CloudCover.LastValue * CloudLayerVolumetric1.CloudDensity.LastValue), 0.0f, 1.0f)) +
                (CloudLayer1.CloudCover * CloudLayer1.CloudLightAbsorption) +
                (CloudLayer2.CloudCover * CloudLayer2.CloudLightAbsorption) +
                (CloudLayer3.CloudCover * CloudLayer3.CloudLightAbsorption) +
                (CloudLayer4.CloudCover * CloudLayer4.CloudLightAbsorption));
            CloudDirectionalLightDirectBlock = Mathf.Min(1.0f, (CloudCoverTotal + CloudDensityTotal) * 1.2f);
            float flatVelocityMultiplier = Time.deltaTime * 0.005f;
            //cloudNoiseMaskVelocityAccum1 += (CloudLayer1.CloudNoiseMaskVelocity * velMult);
            //cloudNoiseMaskVelocityAccum2 += (CloudLayer2.CloudNoiseMaskVelocity * velMult);
            //cloudNoiseMaskVelocityAccum3 += (CloudLayer3.CloudNoiseMaskVelocity * velMult);
            //cloudNoiseMaskVelocityAccum4 += (CloudLayer4.CloudNoiseMaskVelocity * velMult);
            cloudNoiseVelocityAccum1 += (CloudLayer1.CloudNoiseVelocity * flatVelocityMultiplier);
            cloudNoiseVelocityAccum2 += (CloudLayer2.CloudNoiseVelocity * flatVelocityMultiplier);
            cloudNoiseVelocityAccum3 += (CloudLayer3.CloudNoiseVelocity * flatVelocityMultiplier);
            cloudNoiseVelocityAccum4 += (CloudLayer4.CloudNoiseVelocity * flatVelocityMultiplier);
            float velocityScale = Time.deltaTime * 10.0f * WeatherMapScale.z;
            velocityAccumCoverage += (WeatherMapCloudCoverageVelocity * velocityScale);
            velocityAccumType += (WeatherMapCloudTypeVelocity * velocityScale);
            WeatherMapRenderTextureMaskOffset += (WeatherMapRenderTextureMaskVelocity * Time.deltaTime);

            // ensure mask offset does not go to far out of bounds
            WeatherMapRenderTextureMaskOffset.x = Mathf.Clamp(WeatherMapRenderTextureMaskOffset.x, WeatherMapRenderTextureMaskOffsetClamp.x, WeatherMapRenderTextureMaskOffsetClamp.y);
            WeatherMapRenderTextureMaskOffset.y = Mathf.Clamp(WeatherMapRenderTextureMaskOffset.y, WeatherMapRenderTextureMaskOffsetClamp.x, WeatherMapRenderTextureMaskOffsetClamp.y);
        }

        public void CopyStateTo(WeatherMakerCloudProfileScript other)
        {
            other.velocityAccumCoverage = velocityAccumCoverage;
            other.velocityAccumType = velocityAccumType;
            other.cloudNoiseVelocityAccum1 = this.cloudNoiseVelocityAccum1;
            other.cloudNoiseVelocityAccum2 = this.cloudNoiseVelocityAccum2;
            other.cloudNoiseVelocityAccum3 = this.cloudNoiseVelocityAccum3;
            other.cloudNoiseVelocityAccum4 = this.cloudNoiseVelocityAccum4;
            other.CloudCoverTotal = this.CloudCoverTotal;
            other.CloudDensityTotal = this.CloudDensityTotal;
            other.CloudLightAbsorptionTotal = this.CloudLightAbsorptionTotal;
            other.CloudDirectionalLightDirectBlock = this.CloudDirectionalLightDirectBlock;
            other.CloudsEnabled = this.CloudsEnabled;
            other.WeatherMapRenderTextureMaskVelocity = this.WeatherMapRenderTextureMaskVelocity;
            other.WeatherMapRenderTextureMaskOffset = this.WeatherMapRenderTextureMaskOffset;
        }
    }
}
