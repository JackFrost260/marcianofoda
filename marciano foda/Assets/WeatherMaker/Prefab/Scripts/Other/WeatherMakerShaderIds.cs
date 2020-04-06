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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public static class WMS
    {
        #region Buffers

        private static readonly Color[] tmpColorArray = new Color[4];
        private static readonly float[] tmpFloatArray = new float[4];
        private static readonly float[] tmpFloatArray2 = new float[8];
        private static readonly Vector4[] tmpVectorArray = new Vector4[4];

        #endregion Buffers

        #region Ids

        public static readonly int _AlphaMultiplierAnimation = Shader.PropertyToID("_AlphaMultiplierAnimation");
        public static readonly int _AlphaMultiplierAnimation2 = Shader.PropertyToID("_AlphaMultiplierAnimation2");
        public static readonly int _Blur7 = Shader.PropertyToID("_Blur7");
        public static readonly int _CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
        public static readonly int _CameraDepthTextureEighth = Shader.PropertyToID("_CameraDepthTextureEighth");
        public static readonly int _CameraDepthTextureHalf = Shader.PropertyToID("_CameraDepthTextureHalf");
        public static readonly int _CameraDepthTextureOne = Shader.PropertyToID("_CameraDepthTextureOne");
        public static readonly int _CameraDepthTextureQuarter = Shader.PropertyToID("_CameraDepthTextureQuarter");
        public static readonly int _CameraDepthTextureSixteenth = Shader.PropertyToID("_CameraDepthTextureSixteenth");
        public static readonly int _CloudAmbientGroundIntensityVolumetric = Shader.PropertyToID("_CloudAmbientGroundIntensityVolumetric");
        public static readonly int _CloudAmbientGroundHeightMultiplierVolumetric = Shader.PropertyToID("_CloudAmbientGroundHeightMultiplierVolumetric");
        public static readonly int _CloudAmbientMultiplier = Shader.PropertyToID("_CloudAmbientMultiplier");
        public static readonly int _CloudAmbientSkyHeightMultiplierVolumetric = Shader.PropertyToID("_CloudAmbientSkyHeightMultiplierVolumetric");
        public static readonly int _CloudAmbientSkyIntensityVolumetric = Shader.PropertyToID("_CloudAmbientSkyIntensityVolumetric");
        public static readonly int _CloudBottomFadeVolumetric = Shader.PropertyToID("_CloudBottomFadeVolumetric");
        public static readonly int _CloudColor = Shader.PropertyToID("_CloudColor");
        public static readonly int _CloudColorVolumetric = Shader.PropertyToID("_CloudColorVolumetric");
        public static readonly int _CloudConeRandomVectors = Shader.PropertyToID("_CloudConeRandomVectors");
        public static readonly int _CloudCover = Shader.PropertyToID("_CloudCover");
        public static readonly int _CloudCoverageAdder = Shader.PropertyToID("_CloudCoverageAdder");
        public static readonly int _CloudCoverageFrequency = Shader.PropertyToID("_CloudCoverageFrequency");
        public static readonly int _CloudCoverageOffset = Shader.PropertyToID("_CloudCoverageOffset");
        public static readonly int _CloudCoveragePower = Shader.PropertyToID("_CloudCoveragePower");
        public static readonly int _CloudCoverageRotation = Shader.PropertyToID("_CloudCoverageRotation");
        public static readonly int _CloudCoverageVelocity = Shader.PropertyToID("_CloudCoverageVelocity");
        public static readonly int _CloudCoverVolumetric = Shader.PropertyToID("_CloudCoverVolumetric");
        public static readonly int _CloudCoverSecondaryVolumetric = Shader.PropertyToID("_CloudCoverSecondaryVolumetric");
        public static readonly int _CloudDensity = Shader.PropertyToID("_CloudDensity");
        public static readonly int _CloudDensityVolumetric = Shader.PropertyToID("_CloudDensityVolumetric");
        public static readonly int _CloudDetailAnimationVelocity = Shader.PropertyToID("_CloudDetailAnimationVelocity");
        public static readonly int _CloudNoiseDetailPowerVolumetric = Shader.PropertyToID("_CloudNoiseDetailPowerVolumetric");
        public static readonly int _CloudDirColorVolumetric = Shader.PropertyToID("_CloudDirColorVolumetric");
        public static readonly int _CloudDirLightIndirectMultiplierVolumetric = Shader.PropertyToID("_CloudDirLightIndirectMultiplierVolumetric");
        public static readonly int _CloudDirLightMultiplierVolumetric = Shader.PropertyToID("_CloudDirLightMultiplierVolumetric");
        public static readonly int _CloudDirLightRaySampleCount = Shader.PropertyToID("_CloudDirLightRaySampleCount");
        public static readonly int _CloudDirLightSampleCount = Shader.PropertyToID("_CloudDirLightSampleCount");
        public static readonly int _CloudEmissionColor = Shader.PropertyToID("_CloudEmissionColor");
        public static readonly int _CloudEmissionColorVolumetric = Shader.PropertyToID("_CloudEmissionColorVolumetric");
        public static readonly int _CloudEndSquaredInverseVolumetric = Shader.PropertyToID("_CloudEndSquaredInverseVolumetric");
        public static readonly int _CloudEndSquaredVolumetric = Shader.PropertyToID("_CloudEndSquaredVolumetric");
        public static readonly int _CloudEndVolumetric = Shader.PropertyToID("_CloudEndVolumetric");
        public static readonly int _CloudNoiseFrame = Shader.PropertyToID("_CloudNoiseFrame");
        public static readonly int _CloudGradientCumulus = Shader.PropertyToID("_CloudGradientCumulus");
        public static readonly int _CloudGradientStratoCumulus = Shader.PropertyToID("_CloudGradientStratoCumulus");
        public static readonly int _CloudGradientStratus = Shader.PropertyToID("_CloudGradientStratus");
        public static readonly int _CloudHeight = Shader.PropertyToID("_CloudHeight");
        public static readonly int _CloudHeightInverseVolumetric = Shader.PropertyToID("_CloudHeightInverseVolumetric");
        public static readonly int _CloudHeightNoisePowerVolumetric = Shader.PropertyToID("_CloudHeightNoisePowerVolumetric");
        public static readonly int _CloudHeightSquaredInverseVolumetric = Shader.PropertyToID("_CloudHeightSquaredInverseVolumetric");
        public static readonly int _CloudHeightSquaredVolumetric = Shader.PropertyToID("_CloudHeightSquaredVolumetric");
        public static readonly int _CloudHeightVolumetric = Shader.PropertyToID("_CloudHeightVolumetric");
        public static readonly int _CloudHenyeyGreensteinPhaseVolumetric = Shader.PropertyToID("_CloudHenyeyGreensteinPhaseVolumetric");
        public static readonly int _CloudHorizonFadeMultiplier = Shader.PropertyToID("_CloudHorizonFadeMultiplier");
        public static readonly int _CloudHorizonFadeMultiplierVolumetric = Shader.PropertyToID("_CloudHorizonFadeMultiplierVolumetric");
        public static readonly int _CloudLightAbsorption = Shader.PropertyToID("_CloudLightAbsorption");
        public static readonly int _CloudLightAbsorptionVolumetric = Shader.PropertyToID("_CloudLightAbsorptionVolumetric");
        public static readonly int _CloudLightStepMultiplierVolumetric = Shader.PropertyToID("_CloudLightStepMultiplierVolumetric");
        public static readonly int _CloudMaxRayLengthMultiplierVolumetric = Shader.PropertyToID("_CloudMaxRayLengthMultiplierVolumetric");
        public static readonly int _CloudMinRayYVolumetric = Shader.PropertyToID("_CloudMinRayYVolumetric");
        public static readonly int _CloudNoise1 = Shader.PropertyToID("_CloudNoise1");
        public static readonly int _CloudNoise2 = Shader.PropertyToID("_CloudNoise2");
        public static readonly int _CloudNoise3 = Shader.PropertyToID("_CloudNoise3");
        public static readonly int _CloudNoise4 = Shader.PropertyToID("_CloudNoise4");
        public static readonly int _CloudNoiseCurlVolumetric = Shader.PropertyToID("_CloudNoiseCurlVolumetric");
        public static readonly int _CloudNoiseDetailVolumetric = Shader.PropertyToID("_CloudNoiseDetailVolumetric");
        public static readonly int _CloudNoiseLodVolumetric = Shader.PropertyToID("_CloudNoiseLodVolumetric");
        public static readonly int _CloudNoiseMask1 = Shader.PropertyToID("_CloudNoiseMask1");
        public static readonly int _CloudNoiseMask2 = Shader.PropertyToID("_CloudNoiseMask2");
        public static readonly int _CloudNoiseMask3 = Shader.PropertyToID("_CloudNoiseMask3");
        public static readonly int _CloudNoiseMask4 = Shader.PropertyToID("_CloudNoiseMask4");
        public static readonly int _CloudNoiseMultiplier = Shader.PropertyToID("_CloudNoiseMultiplier");
        public static readonly int _CloudNoiseRotation = Shader.PropertyToID("_CloudNoiseRotation");
        public static readonly int _CloudNoisePerlinParams1 = Shader.PropertyToID("_CloudNoisePerlinParams1");
        public static readonly int _CloudNoisePerlinParams2 = Shader.PropertyToID("_CloudNoisePerlinParams2");
        public static readonly int _CloudNoiseSampleCountVolumetric = Shader.PropertyToID("_CloudNoiseSampleCountVolumetric");
        public static readonly int _CloudNoiseScalarVolumetric = Shader.PropertyToID("_CloudNoiseScalarVolumetric");
        public static readonly int _CloudNoiseScaleVolumetric = Shader.PropertyToID("_CloudNoiseScaleVolumetric");
        public static readonly int _CloudNoiseShapeVolumetric = Shader.PropertyToID("_CloudNoiseShapeVolumetric");
        public static readonly int _CloudNoiseScale = Shader.PropertyToID("_CloudNoiseScale");
        public static readonly int _CloudNoiseType = Shader.PropertyToID("_CloudNoiseType");
        public static readonly int _CloudNoiseVelocity = Shader.PropertyToID("_CloudNoiseVelocity");
        public static readonly int _CloudNoiseWorleyParams1 = Shader.PropertyToID("_CloudNoiseWorleyParams1");
        public static readonly int _CloudNoiseWorleyParams2 = Shader.PropertyToID("_CloudNoiseWorleyParams2");
        public static readonly int _CloudOpticalDistanceMultiplierVolumetric = Shader.PropertyToID("_CloudOpticalDistanceMultiplierVolumetric");
        public static readonly int _CloudPlanetEndSquaredVolumetric = Shader.PropertyToID("_CloudPlanetEndSquaredVolumetric");
        public static readonly int _CloudPlanetEndVolumetric = Shader.PropertyToID("_CloudPlanetEndVolumetric");
        public static readonly int _CloudPlanetRadiusNegativeVolumetric = Shader.PropertyToID("_CloudPlanetRadiusNegativeVolumetric");
        public static readonly int _CloudPlanetRadiusSquaredVolumetric = Shader.PropertyToID("_CloudPlanetRadiusSquaredVolumetric");
        public static readonly int _CloudPlanetRadiusVolumetric = Shader.PropertyToID("_CloudPlanetRadiusVolumetric");
        public static readonly int _CloudPlanetStartSquaredVolumetric = Shader.PropertyToID("_CloudPlanetStartSquaredVolumetric");
        public static readonly int _CloudPlanetStartVolumetric = Shader.PropertyToID("_CloudPlanetStartVolumetric");
        public static readonly int _CloudPointSpotLightMultiplierVolumetric = Shader.PropertyToID("_CloudPointSpotLightMultiplierVolumetric");
        public static readonly int _CloudPowderMultiplierVolumetric = Shader.PropertyToID("_CloudPowderMultiplierVolumetric");
        public static readonly int _CloudRayDitherVolumetric = Shader.PropertyToID("_CloudRayDitherVolumetric");
        public static readonly int _CloudRaymarchMultiplierVolumetric = Shader.PropertyToID("_CloudRaymarchMultiplierVolumetric");
        public static readonly int _CloudRaymarchSkipThreshold = Shader.PropertyToID("_CloudRaymarchSkipThreshold");
        public static readonly int _CloudRaymarchMaybeInCloudStepMultiplier = Shader.PropertyToID("_CloudRaymarchMaybeInCloudStepMultiplier");
        public static readonly int _CloudRaymarchInCloudStepMultiplier = Shader.PropertyToID("_CloudRaymarchInCloudStepMultiplier");
        public static readonly int _CloudRaymarchSkipMultiplier = Shader.PropertyToID("_CloudRaymarchSkipMultiplier");
        public static readonly int _CloudRaymarchSkipMultiplierMaxCount = Shader.PropertyToID("_CloudRaymarchSkipMultiplierMaxCount");
        public static readonly int _CloudRayOffset = Shader.PropertyToID("_CloudRayOffset");
        public static readonly int _CloudRayOffsetVolumetric = Shader.PropertyToID("_CloudRayOffsetVolumetric");
        public static readonly int _CloudScatterMultiplier = Shader.PropertyToID("_CloudScatterMultiplier");
        public static readonly int _CloudShadowMapAdder = Shader.PropertyToID("_CloudShadowMapAdder");
        public static readonly int _CloudShadowMapMaximum = Shader.PropertyToID("_CloudShadowMapMaximum");
        public static readonly int _CloudShadowMapMinimum = Shader.PropertyToID("_CloudShadowMapMinimum");
        public static readonly int _CloudShadowMapMultiplier = Shader.PropertyToID("_CloudShadowMapMultiplier");
        public static readonly int _CloudShadowMapPower = Shader.PropertyToID("_CloudShadowMapPower");
        public static readonly int _CloudShapeAnimationVelocity = Shader.PropertyToID("_CloudShapeAnimationVelocity");
        public static readonly int _CloudShapeNoiseMaxVolumetric = Shader.PropertyToID("_CloudShapeNoiseMaxVolumetric");
        public static readonly int _CloudShapeNoiseMinVolumetric = Shader.PropertyToID("_CloudShapeNoiseMinVolumetric");
        public static readonly int _CloudSharpness = Shader.PropertyToID("_CloudSharpness");
        public static readonly int _CloudBackgroundSkyIntensityVolumetric = Shader.PropertyToID("_CloudBackgroundSkyIntensityVolumetric");
        public static readonly int _CloudStartSquaredVolumetric = Shader.PropertyToID("_CloudStartSquaredVolumetric");
        public static readonly int _CloudStartVolumetric = Shader.PropertyToID("_CloudStartVolumetric");
        public static readonly int _CloudTypeAdder = Shader.PropertyToID("_CloudTypeAdder");
        public static readonly int _CloudTypeFrequency = Shader.PropertyToID("_CloudTypeFrequency");
        public static readonly int _CloudTypeOffset = Shader.PropertyToID("_CloudTypeOffset");
        public static readonly int _CloudTypePower = Shader.PropertyToID("_CloudTypePower");
        public static readonly int _CloudTypeRotation = Shader.PropertyToID("_CloudTypeRotation");
        public static readonly int _CloudTypeVelocity = Shader.PropertyToID("_CloudTypeVelocity");
        public static readonly int _CloudTypeVolumetric = Shader.PropertyToID("_CloudTypeVolumetric");
        public static readonly int _CloudTypeSecondaryVolumetric = Shader.PropertyToID("_CloudTypeSecondaryVolumetric");
        public static readonly int _CloudVolumetricShadowSampleCount = Shader.PropertyToID("_CloudVolumetricShadowSampleCount");
        public static readonly int _Cull = Shader.PropertyToID("_Cull");
        public static readonly int _DawnDuskTex = Shader.PropertyToID("_DawnDuskTex");
        public static readonly int _DownsampleDepthScale = Shader.PropertyToID("_DownsampleDepthScale");
        public static readonly int _DstBlendMode = Shader.PropertyToID("_DstBlendMode");
        public static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
        public static readonly int _MaskOffset = Shader.PropertyToID("_MaskOffset");
        public static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        public static readonly int _MainTex2 = Shader.PropertyToID("_MainTex2");
        public static readonly int _MainTex3 = Shader.PropertyToID("_MainTex3");
        public static readonly int _MainTex4 = Shader.PropertyToID("_MainTex4");
        public static readonly int _MaskTex = Shader.PropertyToID("_MaskTex");
        public static readonly int _WeatherMakerFogFactorMax = Shader.PropertyToID("_WeatherMakerFogFactorMax");
        public static readonly int _NightDuskMultiplier = Shader.PropertyToID("_NightDuskMultiplier");
        public static readonly int _NightIntensity = Shader.PropertyToID("_NightIntensity");
        public static readonly int _NightPower = Shader.PropertyToID("_NightPower");
        public static readonly int _NightSkyMultiplier = Shader.PropertyToID("_NightSkyMultiplier");
        public static readonly int _NightTex = Shader.PropertyToID("_NightTex");
        public static readonly int _NightTexAddColor = Shader.PropertyToID("_NightTexAddColor");
        public static readonly int _NightTexTintColor = Shader.PropertyToID("_NightTexTintColor");
        public static readonly int _NightTwinkleMinimum = Shader.PropertyToID("_NightTwinkleMinimum");
        public static readonly int _NightTwinkleRandomness = Shader.PropertyToID("_NightTwinkleRandomness");
        public static readonly int _NightTwinkleSpeed = Shader.PropertyToID("_NightTwinkleSpeed");
        public static readonly int _NightTwinkleVariance = Shader.PropertyToID("_NightTwinkleVariance");
        public static readonly int _NightVisibilityThreshold = Shader.PropertyToID("_NightVisibilityThreshold");
        public static readonly int _NullZoneCount = Shader.PropertyToID("_NullZoneCount");
        public static readonly int _NullZonesCenter = Shader.PropertyToID("_NullZonesCenter");
        public static readonly int _NullZonesMax = Shader.PropertyToID("_NullZonesMax");
        public static readonly int _NullZonesMin = Shader.PropertyToID("_NullZonesMin");
        public static readonly int _NullZonesParams = Shader.PropertyToID("_NullZonesParams");
        public static readonly int _NullZonesQuaternion = Shader.PropertyToID("_NullZonesQuaternion");
        public static readonly int _OverlayColor = Shader.PropertyToID("_OverlayColor");
        public static readonly int _OverlayIntensity = Shader.PropertyToID("_OverlayIntensity");
        public static readonly int _OverlayReflectionIntensity = Shader.PropertyToID("_OverlayReflectionIntensity");
        public static readonly int _OverlayMinHeight = Shader.PropertyToID("_OverlayMinHeight");
        public static readonly int _OverlayMinHeightFalloffMultiplier = Shader.PropertyToID("_OverlayMinHeightFalloffMultiplier");
        public static readonly int _OverlayMinHeightFalloffPower = Shader.PropertyToID("_OverlayMinHeightFalloffPower");
        public static readonly int _OverlayMinHeightNoiseAdder = Shader.PropertyToID("_OverlayMinHeightNoiseAdder");
        public static readonly int _OverlayMinHeightNoiseEnabled = Shader.PropertyToID("_OverlayMinHeightNoiseEnabled");
        public static readonly int _OverlayMinHeightNoiseMultiplier = Shader.PropertyToID("_OverlayMinHeightNoiseMultiplier");
        public static readonly int _OverlayMinHeightNoiseOffset = Shader.PropertyToID("_OverlayMinHeightNoiseOffset");
        public static readonly int _OverlayMinHeightNoiseScale = Shader.PropertyToID("_OverlayMinHeightNoiseScale");
        public static readonly int _OverlayMinHeightNoiseVelocity = Shader.PropertyToID("_OverlayMinHeightNoiseVelocity");
        public static readonly int _OverlayNoiseAdder = Shader.PropertyToID("_OverlayNoiseAdder");
        public static readonly int _OverlayNoiseEnabled = Shader.PropertyToID("_OverlayNoiseEnabled");
        public static readonly int _OverlayNoiseHeightTexture = Shader.PropertyToID("_OverlayNoiseHeightTexture");
        public static readonly int _OverlayNoiseMultiplier = Shader.PropertyToID("_OverlayNoiseMultiplier");
        public static readonly int _OverlayNoiseOffset = Shader.PropertyToID("_OverlayNoiseOffset");
        public static readonly int _OverlayNoisePower = Shader.PropertyToID("_OverlayNoisePower");
        public static readonly int _OverlayNoiseScale = Shader.PropertyToID("_OverlayNoiseScale");
        public static readonly int _OverlayNoiseTexture = Shader.PropertyToID("_OverlayNoiseTexture");
        public static readonly int _OverlayNoiseVelocity = Shader.PropertyToID("_OverlayNoiseVelocity");
        public static readonly int _OverlayNormalReducer = Shader.PropertyToID("_OverlayNormalReducer");
        public static readonly int _OverlayOffset = Shader.PropertyToID("_OverlayOffset");
        public static readonly int _OverlayReflectionTexture = Shader.PropertyToID("_OverlayReflectionTexture");
        public static readonly int _OverlayScale = Shader.PropertyToID("_OverlayScale");
        public static readonly int _OverlaySpecularColor = Shader.PropertyToID("_OverlaySpecularColor");
        public static readonly int _OverlaySpecularIntensity = Shader.PropertyToID("_OverlaySpecularIntensity");
        public static readonly int _OverlaySpecularPower = Shader.PropertyToID("_OverlaySpecularPower");
        public static readonly int _OverlayTexture = Shader.PropertyToID("_OverlayTexture");
        public static readonly int _OverlayVelocity = Shader.PropertyToID("_OverlayVelocity");
        public static readonly int _ParticleDitherLevel = Shader.PropertyToID("_ParticleDitherLevel");
        public static readonly int _RealTimeCloudNoiseShapeTypes = Shader.PropertyToID("_RealTimeCloudNoiseShapeTypes");
        public static readonly int _RealTimeCloudNoiseShapePerlinParam1 = Shader.PropertyToID("_RealTimeCloudNoiseShapePerlinParam1");
        public static readonly int _RealTimeCloudNoiseShapePerlinParam2 = Shader.PropertyToID("_RealTimeCloudNoiseShapePerlinParam2");
        public static readonly int _RealTimeCloudNoiseShapeWorleyParam1 = Shader.PropertyToID("_RealTimeCloudNoiseShapeWorleyParam1");
        public static readonly int _RealTimeCloudNoiseShapeWorleyParam2 = Shader.PropertyToID("_RealTimeCloudNoiseShapeWorleyParam2");
        public static readonly int _ReflectionIntensity = Shader.PropertyToID("_ReflectionIntensity");
        public static readonly int _Specular = Shader.PropertyToID("_Specular");
        public static readonly int _SrcBlendMode = Shader.PropertyToID("_SrcBlendMode");
        public static readonly int _TemporalReprojection_BlendMode = Shader.PropertyToID("_TemporalReprojection_BlendMode");
        public static readonly int _TemporalReprojection_InverseProjection = Shader.PropertyToID("_TemporalReprojection_InverseProjection");
        public static readonly int _TemporalReprojection_InverseProjectionView = Shader.PropertyToID("_TemporalReprojection_InverseProjectionView");
        public static readonly int _TemporalReprojection_InverseView = Shader.PropertyToID("_TemporalReprojection_InverseView");
        public static readonly int _TemporalReprojection_ipivpvp = Shader.PropertyToID("_TemporalReprojection_ipivpvp");
        public static readonly int _TemporalReprojection_PrevFrame = Shader.PropertyToID("_TemporalReprojection_PrevFrame");
        public static readonly int _TemporalReprojection_PreviousView = Shader.PropertyToID("_TemporalReprojection_PreviousView");
        public static readonly int _TemporalReprojection_PreviousViewProjection = Shader.PropertyToID("_TemporalReprojection_PreviousViewProjection");
        public static readonly int _TemporalReprojection_Projection = Shader.PropertyToID("_TemporalReprojection_Projection");
        public static readonly int _TemporalReprojection_SimilarityMax = Shader.PropertyToID("_TemporalReprojection_SimilarityMax");
        public static readonly int _TemporalReprojection_SubFrame = Shader.PropertyToID("_TemporalReprojection_SubFrame");
        public static readonly int _TemporalReprojection_SubFrameNumber = Shader.PropertyToID("_TemporalReprojection_SubFrameNumber");
        public static readonly int _TemporalReprojection_SubPixelSize = Shader.PropertyToID("_TemporalReprojection_SubPixelSize");
        public static readonly int _TemporalReprojection_View = Shader.PropertyToID("_TemporalReprojection_View");
        public static readonly int _TintColor = Shader.PropertyToID("_TintColor");
        public static readonly int _WaterDepthThreshold = Shader.PropertyToID("_WaterDepthThreshold");
        public static readonly int _WaterFogDensity = Shader.PropertyToID("_WaterFogDensity");
        public static readonly int _WaterReflective = Shader.PropertyToID("_WaterReflective");
        public static readonly int _WaterUnderwater = Shader.PropertyToID("_WaterUnderwater");
        public static readonly int _WaterWave1 = Shader.PropertyToID("_WaterWave1");
        public static readonly int _WaterWave2 = Shader.PropertyToID("_WaterWave2");
        public static readonly int _WaterWave3 = Shader.PropertyToID("_WaterWave3");
        public static readonly int _WaterWave4 = Shader.PropertyToID("_WaterWave4");
        public static readonly int _WaterWave5 = Shader.PropertyToID("_WaterWave5");
        public static readonly int _WaterWave6 = Shader.PropertyToID("_WaterWave6");
        public static readonly int _WaterWave7 = Shader.PropertyToID("_WaterWave7");
        public static readonly int _WaterWave8 = Shader.PropertyToID("_WaterWave8");
        public static readonly int _WaterWave1_Precompute = Shader.PropertyToID("_WaterWave1_Precompute");
        public static readonly int _WaterWave2_Precompute = Shader.PropertyToID("_WaterWave2_Precompute");
        public static readonly int _WaterWave3_Precompute = Shader.PropertyToID("_WaterWave3_Precompute");
        public static readonly int _WaterWave4_Precompute = Shader.PropertyToID("_WaterWave4_Precompute");
        public static readonly int _WaterWave5_Precompute = Shader.PropertyToID("_WaterWave5_Precompute");
        public static readonly int _WaterWave6_Precompute = Shader.PropertyToID("_WaterWave6_Precompute");
        public static readonly int _WaterWave7_Precompute = Shader.PropertyToID("_WaterWave7_Precompute");
        public static readonly int _WaterWave8_Precompute = Shader.PropertyToID("_WaterWave8_Precompute");
        public static readonly int _WaterWave1_Params1 = Shader.PropertyToID("_WaterWave1_Params1");
        public static readonly int _WaterWave2_Params1 = Shader.PropertyToID("_WaterWave2_Params1");
        public static readonly int _WaterWave3_Params1 = Shader.PropertyToID("_WaterWave3_Params1");
        public static readonly int _WaterWave4_Params1 = Shader.PropertyToID("_WaterWave4_Params1");
        public static readonly int _WaterWave5_Params1 = Shader.PropertyToID("_WaterWave5_Params1");
        public static readonly int _WaterWave6_Params1 = Shader.PropertyToID("_WaterWave6_Params1");
        public static readonly int _WaterWave7_Params1 = Shader.PropertyToID("_WaterWave7_Params1");
        public static readonly int _WaterWave8_Params1 = Shader.PropertyToID("_WaterWave8_Params1");
        public static readonly int _WaterWaveMultiplier = Shader.PropertyToID("_WaterWaveMultiplier");
        public static readonly int _WeatherMakerAfterForwardOpaqueTexture = Shader.PropertyToID("_WeatherMakerAfterForwardOpaqueTexture");
        public static readonly int _WeatherMakerAmbientLightColor = Shader.PropertyToID("_WeatherMakerAmbientLightColor");
        public static readonly int _WeatherMakerAmbientLightColorEquator = Shader.PropertyToID("_WeatherMakerAmbientLightColorEquator");
        public static readonly int _WeatherMakerAmbientLightColorGround = Shader.PropertyToID("_WeatherMakerAmbientLightColorGround");
        public static readonly int _WeatherMakerAmbientLightColorSky = Shader.PropertyToID("_WeatherMakerAmbientLightColorSky");
        public static readonly int _WeatherMakerAreaLightAtten = Shader.PropertyToID("_WeatherMakerAreaLightAtten");
        public static readonly int _WeatherMakerAreaLightColor = Shader.PropertyToID("_WeatherMakerAreaLightColor");
        public static readonly int _WeatherMakerAreaLightCount = Shader.PropertyToID("_WeatherMakerAreaLightCount");
        public static readonly int _WeatherMakerAreaLightDirection = Shader.PropertyToID("_WeatherMakerAreaLightDirection");
        public static readonly int _WeatherMakerAreaLightMaxPosition = Shader.PropertyToID("_WeatherMakerAreaLightMaxPosition");
        public static readonly int _WeatherMakerAreaLightMinPosition = Shader.PropertyToID("_WeatherMakerAreaLightMinPosition");
        public static readonly int _WeatherMakerAreaLightPosition = Shader.PropertyToID("_WeatherMakerAreaLightPosition");
        public static readonly int _WeatherMakerAreaLightPositionEnd = Shader.PropertyToID("_WeatherMakerAreaLightPositionEnd");
        public static readonly int _WeatherMakerAreaLightRotation = Shader.PropertyToID("_WeatherMakerAreaLightRotation");
        public static readonly int _WeatherMakerAreaLightVar1 = Shader.PropertyToID("_WeatherMakerAreaLightVar1");
        public static readonly int _WeatherMakerAreaLightViewportPosition = Shader.PropertyToID("_WeatherMakerAreaLightViewportPosition");
        public static readonly int _WeatherMakerAuroraSampleCount = Shader.PropertyToID("_WeatherMakerAuroraSampleCount");
        public static readonly int _WeatherMakerAuroraSubSampleCount = Shader.PropertyToID("_WeatherMakerAuroraSubSampleCount");
        public static readonly int _WeatherMakerAuroraAnimationSpeed = Shader.PropertyToID("_WeatherMakerAuroraAnimationSpeed");
        public static readonly int _WeatherMakerAuroraMarchScale = Shader.PropertyToID("_WeatherMakerAuroraMarchScale");
        public static readonly int _WeatherMakerAuroraScale = Shader.PropertyToID("_WeatherMakerAuroraScale");
        public static readonly int _WeatherMakerAuroraColor = Shader.PropertyToID("_WeatherMakerAuroraColor");
        public static readonly int _WeatherMakerAuroraColorKeys = Shader.PropertyToID("_WeatherMakerAuroraColorKeys");
        public static readonly int _WeatherMakerAuroraIntensity = Shader.PropertyToID("_WeatherMakerAuroraIntensity");
        public static readonly int _WeatherMakerAuroraOctave = Shader.PropertyToID("_WeatherMakerAuroraOctave");
        public static readonly int _WeatherMakerAuroraPower = Shader.PropertyToID("_WeatherMakerAuroraPower");
        public static readonly int _WeatherMakerAuroraHeightFadePower = Shader.PropertyToID("_WeatherMakerAuroraHeightFadePower");
        public static readonly int _WeatherMakerAuroraHeight = Shader.PropertyToID("_WeatherMakerAuroraHeight");
        public static readonly int _WeatherMakerAuroraPlanetRadius = Shader.PropertyToID("_WeatherMakerAuroraPlanetRadius");
        public static readonly int _WeatherMakerAuroraDistanceFade = Shader.PropertyToID("_WeatherMakerAuroraDistanceFade");
        public static readonly int _WeatherMakerAuroraDither = Shader.PropertyToID("_WeatherMakerAuroraDither");
        public static readonly int _WeatherMakerAuroraSeed = Shader.PropertyToID("_WeatherMakerAuroraSeed");
        public static readonly int _WeatherMakerBlueNoiseTexture = Shader.PropertyToID("_WeatherMakerBlueNoiseTexture");
        public static readonly int _WeatherMakerCameraFrustumRays = Shader.PropertyToID("_WeatherMakerCameraFrustumRays");
        public static readonly int _WeatherMakerCameraFrustumRaysTemporal = Shader.PropertyToID("_WeatherMakerCameraFrustumRaysTemporal");
        public static readonly int _WeatherMakerCameraRenderMode = Shader.PropertyToID("_WeatherMakerCameraRenderMode");
        public static readonly int _WeatherMakerCloudCameraPosition = Shader.PropertyToID("_WeatherMakerCloudCameraPosition");
        public static readonly int _WeatherMakerCloudDitherLevel = Shader.PropertyToID("_WeatherMakerCloudDitherLevel");
        public static readonly int _WeatherMakerCloudGlobalShadow = Shader.PropertyToID("_WeatherMakerCloudGlobalShadow");
        public static readonly int _WeatherMakerCloudGlobalShadow2 = Shader.PropertyToID("_WeatherMakerCloudGlobalShadow2");
        public static readonly int _WeatherMakerCloudVolumetricShadow = Shader.PropertyToID("_WeatherMakerCloudVolumetricShadow");
        public static readonly int _WeatherMakerCameraCubemap = Shader.PropertyToID("_WeatherMakerCameraCubemap");
        public static readonly int _WeatherMakerDawnDuskMultiplier = Shader.PropertyToID("_WeatherMakerDawnDuskMultiplier");
        public static readonly int _WeatherMakerDayMultiplier = Shader.PropertyToID("_WeatherMakerDayMultiplier");
        public static readonly int _WeatherMakerDirLightColor = Shader.PropertyToID("_WeatherMakerDirLightColor");
        public static readonly int _WeatherMakerDirLightCount = Shader.PropertyToID("_WeatherMakerDirLightCount");
        public static readonly int _WeatherMakerDirLightDirection = Shader.PropertyToID("_WeatherMakerDirLightDirection");
        public static readonly int _WeatherMakerDirLightPosition = Shader.PropertyToID("_WeatherMakerDirLightPosition");
        public static readonly int _WeatherMakerDirLightPower = Shader.PropertyToID("_WeatherMakerDirLightPower");
        public static readonly int _WeatherMakerDirLightQuaternion = Shader.PropertyToID("_WeatherMakerDirLightQuaternion");
        public static readonly int _WeatherMakerDirLightVar1 = Shader.PropertyToID("_WeatherMakerDirLightVar1");
        public static readonly int _WeatherMakerDirLightViewportPosition = Shader.PropertyToID("_WeatherMakerDirLightViewportPosition");
        public static readonly int _WeatherMakerDownsampleScale = Shader.PropertyToID("_WeatherMakerDownsampleScale");
        public static readonly int _WeatherMakerEnableToneMapping = Shader.PropertyToID("_WeatherMakerEnableToneMapping");
        public static readonly int _WeatherMakerFogBoxCenter = Shader.PropertyToID("_WeatherMakerFogBoxCenter");
        public static readonly int _WeatherMakerFogBoxMax = Shader.PropertyToID("_WeatherMakerFogBoxMax");
        public static readonly int _WeatherMakerFogBoxMaxDir = Shader.PropertyToID("_WeatherMakerFogBoxMaxDir");
        public static readonly int _WeatherMakerFogBoxMin = Shader.PropertyToID("_WeatherMakerFogBoxMin");
        public static readonly int _WeatherMakerFogBoxMinDir = Shader.PropertyToID("_WeatherMakerFogBoxMinDir");
        public static readonly int _WeatherMakerFogCloudShadowStrength = Shader.PropertyToID("_WeatherMakerFogCloudShadowStrength");
        public static readonly int _WeatherMakerFogColor = Shader.PropertyToID("_WeatherMakerFogColor");
        public static readonly int _WeatherMakerFogDensity = Shader.PropertyToID("_WeatherMakerFogDensity");
        public static readonly int _WeatherMakerFogDensityScatter = Shader.PropertyToID("_WeatherMakerFogDensityScatter");
        public static readonly int _WeatherMakerFogDirectionalLightScatterIntensity = Shader.PropertyToID("_WeatherMakerFogDirectionalLightScatterIntensity");
        public static readonly int _WeatherMakerFogDitherLevel = Shader.PropertyToID("_WeatherMakerFogDitherLevel");
        public static readonly int _WeatherMakerFogEmissionColor = Shader.PropertyToID("_WeatherMakerFogEmissionColor");
        public static readonly int _WeatherMakerFogFactorMultiplier = Shader.PropertyToID("_WeatherMakerFogFactorMultiplier");
        public static readonly int _WeatherMakerFogGlobalShadow = Shader.PropertyToID("_WeatherMakerFogGlobalShadow");
        public static readonly int _WeatherMakerFogHeight = Shader.PropertyToID("_WeatherMakerFogHeight");
        public static readonly int _WeatherMakerFogHeightFalloffPower = Shader.PropertyToID("_WeatherMakerFogHeightFalloffPower");
        public static readonly int _WeatherMakerFogLightAbsorption = Shader.PropertyToID("_WeatherMakerFogLightAbsorption");
        public static readonly int _WeatherMakerFogLightFalloff = Shader.PropertyToID("_WeatherMakerFogLightFalloff");
        public static readonly int _WeatherMakerFogLightShadowBrightness = Shader.PropertyToID("_WeatherMakerFogLightShadowBrightness");
        public static readonly int _WeatherMakerFogLightShadowDecay = Shader.PropertyToID("_WeatherMakerFogLightShadowDecay");
        public static readonly int _WeatherMakerFogLightShadowDither = Shader.PropertyToID("_WeatherMakerFogLightShadowDither");
        public static readonly int _WeatherMakerFogLightShadowDitherMagic = Shader.PropertyToID("_WeatherMakerFogLightShadowDitherMagic");
        public static readonly int _WeatherMakerFogLightShadowInvSampleCount = Shader.PropertyToID("_WeatherMakerFogLightShadowInvSampleCount");
        public static readonly int _WeatherMakerFogLightShadowMaxRayLength = Shader.PropertyToID("_WeatherMakerFogLightShadowMaxRayLength");
        public static readonly int _WeatherMakerFogLightShadowMultiplier = Shader.PropertyToID("_WeatherMakerFogLightShadowMultiplier");
        public static readonly int _WeatherMakerFogLightShadowPower = Shader.PropertyToID("_WeatherMakerFogLightShadowPower");
        public static readonly int _WeatherMakerFogLightShadowSampleCount = Shader.PropertyToID("_WeatherMakerFogLightShadowSampleCount");
        public static readonly int _WeatherMakerFogLightSunIntensityReducer = Shader.PropertyToID("_WeatherMakerFogLightSunIntensityReducer");
        public static readonly int _WeatherMakerFogLinearFogFactor = Shader.PropertyToID("_WeatherMakerFogLinearFogFactor");
        public static readonly int _WeatherMakerFogMaxFogFactor = Shader.PropertyToID("_WeatherMakerFogMaxFogFactor");
        public static readonly int _WeatherMakerFogMode = Shader.PropertyToID("_WeatherMakerFogMode");
        public static readonly int _WeatherMakerFogNoiseAdder = Shader.PropertyToID("_WeatherMakerFogNoiseAdder");
        public static readonly int _WeatherMakerFogNoiseEnabled = Shader.PropertyToID("_WeatherMakerFogNoiseEnabled");
        public static readonly int _WeatherMakerFogNoiseMultiplier = Shader.PropertyToID("_WeatherMakerFogNoiseMultiplier");
        public static readonly int _WeatherMakerFogNoisePercent = Shader.PropertyToID("_WeatherMakerFogNoisePercent");
        public static readonly int _WeatherMakerFogNoisePositionOffset = Shader.PropertyToID("_WeatherMakerFogNoisePositionOffset");
        public static readonly int _WeatherMakerFogNoiseSampleCount = Shader.PropertyToID("_WeatherMakerFogNoiseSampleCount");
        public static readonly int _WeatherMakerFogNoiseSampleCountInverse = Shader.PropertyToID("_WeatherMakerFogNoiseSampleCountInverse");
        public static readonly int _WeatherMakerFogNoiseScale = Shader.PropertyToID("_WeatherMakerFogNoiseScale");
        public static readonly int _WeatherMakerFogNoiseVelocity = Shader.PropertyToID("_WeatherMakerFogNoiseVelocity");
        public static readonly int _WeatherMakerFogSpherePosition = Shader.PropertyToID("_WeatherMakerFogSpherePosition");
        public static readonly int _WeatherMakerFogStartDepth = Shader.PropertyToID("_WeatherMakerFogStartDepth");
        public static readonly int _WeatherMakerFogEndDepth = Shader.PropertyToID("_WeatherMakerFogEndDepth");
        public static readonly int _WeatherMakerFogSunShaftMode = Shader.PropertyToID("_WeatherMakerFogSunShaftMode");
        public static readonly int _WeatherMakerFogSunShaftsDitherMagic = Shader.PropertyToID("_WeatherMakerFogSunShaftsDitherMagic");
        public static readonly int _WeatherMakerFogSunShaftsParam1 = Shader.PropertyToID("_WeatherMakerFogSunShaftsParam1");
        public static readonly int _WeatherMakerFogSunShaftsParam2 = Shader.PropertyToID("_WeatherMakerFogSunShaftsParam2");
        public static readonly int _WeatherMakerFogSunShaftsTintColor = Shader.PropertyToID("_WeatherMakerFogSunShaftsTintColor");
        public static readonly int _WeatherMakerFogVolumePower = Shader.PropertyToID("_WeatherMakerFogVolumePower");
        public static readonly int _WeatherMakerFogVolumetricLightMode = Shader.PropertyToID("_WeatherMakerFogVolumetricLightMode");
        public static readonly int _WeatherMakerInverseProj = Shader.PropertyToID("_WeatherMakerInverseProj");
        public static readonly int _WeatherMakerInverseView = Shader.PropertyToID("_WeatherMakerInverseView");
        public static readonly int _WeatherMakerNightMultiplier = Shader.PropertyToID("_WeatherMakerNightMultiplier");
        public static readonly int _WeatherMakerNoiseTexture3D = Shader.PropertyToID("_WeatherMakerNoiseTexture3D");
        public static readonly int _WeatherMakerPointLightAtten = Shader.PropertyToID("_WeatherMakerPointLightAtten");
        public static readonly int _WeatherMakerPointLightColor = Shader.PropertyToID("_WeatherMakerPointLightColor");
        public static readonly int _WeatherMakerPointLightCount = Shader.PropertyToID("_WeatherMakerPointLightCount");
        public static readonly int _WeatherMakerPointLightDirection = Shader.PropertyToID("_WeatherMakerPointLightDirection");
        public static readonly int _WeatherMakerPointLightPosition = Shader.PropertyToID("_WeatherMakerPointLightPosition");
        public static readonly int _WeatherMakerPointLightVar1 = Shader.PropertyToID("_WeatherMakerPointLightVar1");
        public static readonly int _WeatherMakerPointLightViewportPosition = Shader.PropertyToID("_WeatherMakerPointLightViewportPosition");
        public static readonly int _WeatherMakerSkyAtmosphereParams = Shader.PropertyToID("_WeatherMakerSkyAtmosphereParams");
        public static readonly int _WeatherMakerSkyDitherLevel = Shader.PropertyToID("_WeatherMakerSkyDitherLevel");
        public static readonly int _WeatherMakerSkyEnableNightTwinkle = Shader.PropertyToID("_WeatherMakerSkyEnableNightTwinkle");
        public static readonly int _WeatherMakerSkyEnableSunEclipse = Shader.PropertyToID("_WeatherMakerSkyEnableSunEclipse");
        public static readonly int _WeatherMakerSkyLightPIScattering = Shader.PropertyToID("_WeatherMakerSkyLightPIScattering");
        public static readonly int _WeatherMakerSkyLightScattering = Shader.PropertyToID("_WeatherMakerSkyLightScattering");
        public static readonly int _WeatherMakerSkyMie = Shader.PropertyToID("_WeatherMakerSkyMie");
        public static readonly int _WeatherMakerSkyMieG = Shader.PropertyToID("_WeatherMakerSkyMieG");
        public static readonly int _WeatherMakerSkyRadius = Shader.PropertyToID("_WeatherMakerSkyRadius");
        public static readonly int _WeatherMakerSkyRenderType = Shader.PropertyToID("_WeatherMakerSkyRenderType");
        public static readonly int _WeatherMakerSkyScale = Shader.PropertyToID("_WeatherMakerSkyScale");
        public static readonly int _WeatherMakerSkySphereRadius = Shader.PropertyToID("_WeatherMakerSkySphereRadius");
        public static readonly int _WeatherMakerSkySphereRadiusSquared = Shader.PropertyToID("_WeatherMakerSkySphereRadiusSquared");
        public static readonly int _WeatherMakerSkyTintColor = Shader.PropertyToID("_WeatherMakerSkyTintColor");
        public static readonly int _WeatherMakerSkyTotalMie = Shader.PropertyToID("_WeatherMakerSkyTotalMie");
        public static readonly int _WeatherMakerSkyTotalRayleigh = Shader.PropertyToID("_WeatherMakerSkyTotalRayleigh");
        public static readonly int _WeatherMakerSkyYOffset2D = Shader.PropertyToID("_WeatherMakerSkyYOffset2D");
        public static readonly int _WeatherMakerSpotLightAtten = Shader.PropertyToID("_WeatherMakerSpotLightAtten");
        public static readonly int _WeatherMakerSpotLightColor = Shader.PropertyToID("_WeatherMakerSpotLightColor");
        public static readonly int _WeatherMakerSpotLightCount = Shader.PropertyToID("_WeatherMakerSpotLightCount");
        public static readonly int _WeatherMakerSpotLightDirection = Shader.PropertyToID("_WeatherMakerSpotLightDirection");
        public static readonly int _WeatherMakerSpotLightPosition = Shader.PropertyToID("_WeatherMakerSpotLightPosition");
        public static readonly int _WeatherMakerSpotLightSpotEnd = Shader.PropertyToID("_WeatherMakerSpotLightSpotEnd");
        public static readonly int _WeatherMakerSpotLightVar1 = Shader.PropertyToID("_WeatherMakerSpotLightVar1");
        public static readonly int _WeatherMakerSpotLightViewportPosition = Shader.PropertyToID("_WeatherMakerSpotLightViewportPosition");
        public static readonly int _WeatherMakerSunColor = Shader.PropertyToID("_WeatherMakerSunColor");
        public static readonly int _WeatherMakerSunDirectionDown = Shader.PropertyToID("_WeatherMakerSunDirectionDown");
        public static readonly int _WeatherMakerSunDirectionDown2D = Shader.PropertyToID("_WeatherMakerSunDirectionDown2D");
        public static readonly int _WeatherMakerSunDirectionUp = Shader.PropertyToID("_WeatherMakerSunDirectionUp");
        public static readonly int _WeatherMakerSunDirectionUp2D = Shader.PropertyToID("_WeatherMakerSunDirectionUp2D");
        public static readonly int _WeatherMakerSunLightPower = Shader.PropertyToID("_WeatherMakerSunLightPower");
        public static readonly int _WeatherMakerSunPositionNormalized = Shader.PropertyToID("_WeatherMakerSunPositionNormalized");
        public static readonly int _WeatherMakerSunPositionWorldSpace = Shader.PropertyToID("_WeatherMakerSunPositionWorldSpace");
        public static readonly int _WeatherMakerSunTintColor = Shader.PropertyToID("_WeatherMakerSunTintColor");
        public static readonly int _WeatherMakerSunVar1 = Shader.PropertyToID("_WeatherMakerSunVar1");
        public static readonly int _WeatherMakerTemporaryDepthTexture = Shader.PropertyToID("_WeatherMakerTemporaryDepthTexture");
        public static readonly int _WeatherMakerTemporalReprojectionEnabled = Shader.PropertyToID("_WeatherMakerTemporalReprojectionEnabled");
        public static readonly int _WeatherMakerTemporalUV = Shader.PropertyToID("_WeatherMakerTemporalUV");
        public static readonly int _WeatherMakerTime = Shader.PropertyToID("_WeatherMakerTime");
        public static readonly int _WeatherMakerTimeSin = Shader.PropertyToID("_WeatherMakerTimeSin");
        public static readonly int _WeatherMakerTimeAngle = Shader.PropertyToID("_WeatherMakerTimeAngle");
        public static readonly int _WeatherMakerVolumetricPointSpotMultiplier = Shader.PropertyToID("_WeatherMakerVolumetricPointSpotMultiplier");
        public static readonly int _WeatherMakerWeatherMapScale = Shader.PropertyToID("_WeatherMakerWeatherMapScale");
        public static readonly int _WeatherMakerWeatherMapSeed = Shader.PropertyToID("_WeatherMakerWeatherMapSeed");
        public static readonly int _WeatherMakerWeatherMapTexture = Shader.PropertyToID("_WeatherMakerWeatherMapTexture");
        public static readonly int _WeatherMakerYDepthParams = Shader.PropertyToID("_WeatherMakerYDepthParams");
        public static readonly int _WeatherMakerYDepthTexture = Shader.PropertyToID("_WeatherMakerYDepthTexture");
        public static readonly int _ZTest = Shader.PropertyToID("_ZTest");
        public static readonly int _Zwrite = Shader.PropertyToID("_Zwrite");
        public static readonly int rtp_snow_strength = Shader.PropertyToID("rtp_snow_strength");
        public static readonly int TERRAIN_GlobalWetness = Shader.PropertyToID("TERRAIN_GlobalWetness");
        public static readonly int TERRAIN_RainIntensity = Shader.PropertyToID("TERRAIN_RainIntensity");

        #endregion Ids

        static WMS()
        {
        }

        public static void SetColorArray(Material m, int prop, Color c1, Color c2, Color c3, Color c4)
        {
            tmpColorArray[0] = c1;
            tmpColorArray[1] = c2;
            tmpColorArray[2] = c3;
            tmpColorArray[3] = c4;
            m.SetColorArray(prop, tmpColorArray);
        }

        public static void SetFloatArray(Material m, int prop, float f1, float f2, float f3, float f4)
        {
            tmpFloatArray[0] = f1;
            tmpFloatArray[1] = f2;
            tmpFloatArray[2] = f3;
            tmpFloatArray[3] = f4;
            m.SetFloatArray(prop, tmpFloatArray);
        }

        public static void SetFloatArrayRotation(Material m, int prop, float f1, float f2, float f3, float f4)
        {
            tmpFloatArray2[0] = Mathf.Cos(f1 * Mathf.Deg2Rad);
            tmpFloatArray2[1] = Mathf.Cos(f2 * Mathf.Deg2Rad);
            tmpFloatArray2[2] = Mathf.Cos(f3 * Mathf.Deg2Rad);
            tmpFloatArray2[3] = Mathf.Cos(f4 * Mathf.Deg2Rad);
            tmpFloatArray2[4] = Mathf.Sin(f1 * Mathf.Deg2Rad);
            tmpFloatArray2[5] = Mathf.Sin(f2 * Mathf.Deg2Rad);
            tmpFloatArray2[6] = Mathf.Sin(f3 * Mathf.Deg2Rad);
            tmpFloatArray2[7] = Mathf.Sin(f4 * Mathf.Deg2Rad);
            m.SetFloatArray(prop, tmpFloatArray2);
        }

        public static void SetVectorArray(Material m, int prop, Vector4 v1, Vector4 v2, Vector4 v3, Vector4 v4)
        {
            tmpVectorArray[0] = v1;
            tmpVectorArray[1] = v2;
            tmpVectorArray[2] = v3;
            tmpVectorArray[3] = v4;
            m.SetVectorArray(prop, tmpVectorArray);
        }

        public static void DisableKeywords(this Material m, params string[] keywords)
        {
            foreach (string keyword in keywords)
            {
                m.DisableKeyword(keyword);
            }
        }

        public static void Initialize()
        {
        }
    }
}
