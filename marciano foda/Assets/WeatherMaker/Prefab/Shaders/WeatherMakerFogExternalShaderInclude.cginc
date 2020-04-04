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

// note: define NULL_ZONE_RENDER_MASK for the type of mask your shader is rendering (see NullZoneScript.cs)

#ifndef WEATHER_MAKER_FOG_EXTERNAL_SHADER_INCLUDED
#define WEATHER_MAKER_FOG_EXTERNAL_SHADER_INCLUDED

#define FOG_LIGHT_POINT_SAMPLE_COUNT 5.0
#define FOG_LIGHT_POINT_SAMPLE_COUNT_INVERSE (1.0 / FOG_LIGHT_POINT_SAMPLE_COUNT)
#define FOG_LIGHT_SPOT_SAMPLE_COUNT 16.0
#define FOG_LIGHT_SPOT_SAMPLE_COUNT_INVERSE (1.0 / FOG_LIGHT_SPOT_SAMPLE_COUNT)
#define FOG_LIGHT_AREA_SAMPLE_COUNT 10.0
#define FOG_LIGHT_AREA_SAMPLE_COUNT_INVERSE (1.0 / FOG_LIGHT_AREA_SAMPLE_COUNT)
#define FOG_SHADOW_BASE_LIGHT_INTENSITY 0.5

uniform sampler3D _WeatherMakerNoiseTexture3D;

uniform float _WeatherMakerVolumetricPointSpotMultiplier = 1.0;

uniform uint _WeatherMakerFogMode;
uniform float _WeatherMakerFogStartDepth;
uniform float _WeatherMakerFogEndDepth;
uniform fixed _WeatherMakerFogLinearFogFactor;
uniform fixed4 _WeatherMakerFogColor;
uniform fixed4 _WeatherMakerFogEmissionColor;
uniform fixed _WeatherMakerFogHeightFalloffPower;
uniform fixed _WeatherMakerFogLightAbsorption;
uniform fixed _WeatherMakerFogDitherLevel;
uniform float _WeatherMakerFogNoisePercent; // percent of noise to use. 0 percent would be a noise value of 1, 1 would be the full noise value.
uniform float _WeatherMakerFogNoiseScale;
uniform float _WeatherMakerFogNoiseAdder;
uniform float _WeatherMakerFogNoiseMultiplier;
uniform float _WeatherMakerFogNoiseSampleCount;
uniform float _WeatherMakerFogNoiseSampleCountInverse;
uniform float3 _WeatherMakerFogNoiseVelocity;
uniform float3 _WeatherMakerFogNoisePositionOffset;
uniform float _WeatherMakerFogHeight;
uniform float4 _WeatherMakerFogBoxCenter;
uniform float3 _WeatherMakerFogBoxMin;
uniform float3 _WeatherMakerFogBoxMax;
uniform float3 _WeatherMakerFogBoxMinDir;
uniform float3 _WeatherMakerFogBoxMaxDir;
uniform float4 _WeatherMakerFogSpherePosition;
uniform float4 _WeatherMakerFogVolumePower;
uniform float _WeatherMakerFogFactorMax;
uniform float _WeatherMakerFogCloudShadowStrength;
uniform fixed _WeatherMakerFogDensity;
uniform fixed _WeatherMakerFogFactorMultiplier;
uniform fixed _WeatherMakerFogDensityScatter;

uniform float _WeatherMakerFogLightShadowSampleCount;
uniform float _WeatherMakerFogLightShadowInvSampleCount;
uniform float _WeatherMakerFogLightShadowMaxRayLength;
uniform float _WeatherMakerFogLightShadowMultiplier;
uniform float _WeatherMakerFogLightShadowBrightness;
uniform float _WeatherMakerFogLightShadowPower;
uniform float _WeatherMakerFogLightShadowDecay;
uniform float _WeatherMakerFogLightShadowDither;
uniform float4 _WeatherMakerFogLightShadowDitherMagic;

uniform fixed4 _WeatherMakerFogLightFalloff = fixed4(1.2, 0.0, 0.0, 0.0); // spot light radius light falloff, 0, 0, 0
uniform fixed _WeatherMakerFogLightSunIntensityReducer = 0.8;
uniform fixed _WeatherMakerFogDirectionalLightScatterIntensity = 5.0;

uniform int _WeatherMakerFogNoiseEnabled;
#define WM_FOG_NOISE_ENABLED (_WeatherMakerFogNoiseEnabled)
#define WM_FOG_HEIGHT_ENABLED (_WeatherMakerFogHeight > 0.0)

uniform int _WeatherMakerFogVolumetricLightMode;
#define WM_FOG_VOLUMETRIC_LIGHT_MODE_NONE (_WeatherMakerFogVolumetricLightMode == 0)
#define WM_FOG_VOLUMETRIC_LIGHT_MODE_NOT_NONE (_WeatherMakerFogVolumetricLightMode)
#define WM_FOG_VOLUMETRIC_LIGHT_MODE_ENABLE (_WeatherMakerFogVolumetricLightMode == 1)
#define WM_FOG_VOLUMETRIC_LIGHT_MODE_ENABLE_WITH_SHADOWS (_WeatherMakerFogVolumetricLightMode == 2)

static const float fogLightDitherLevel = _WeatherMakerFogDitherLevel * 64.0;
static const fixed3 fogColorWithIntensity = (_WeatherMakerFogColor.rgb * _WeatherMakerFogColor.a);

inline float CalculateFogFactor(float depth)
{
	float fogFactor;
	switch (_WeatherMakerFogMode)
	{
	case 1:
		// constant
		fogFactor = _WeatherMakerFogDensity * ceil(saturate(depth));
		break;

	case 2:
		// linear
		fogFactor = min(1.0, depth * _WeatherMakerFogLinearFogFactor);
		break;

	case 3:
		// exponential
		// simple height formula
		// const float extinction = 0.01;
		// float fogFactor = saturate((_WeatherMakerFogDensity * exp(-(_WorldSpaceCameraPos.y - _WeatherMakerFogHeight) * extinction) * (1.0 - exp(-depth * rayDir.y * extinction))) / rayDir.y);
		//fogFactor = 1.0 - saturate(1.0 / (exp(depth * _WeatherMakerFogDensity)));
		fogFactor = 1.0 - saturate(exp2(depth * -_WeatherMakerFogDensity));
		break;

	case 4:
		// exponetial squared
		//float expFog = exp(depth * _WeatherMakerFogDensity);
		//fogFactor = 1.0 - saturate(1.0 / (expFog * expFog));
		float expFog = exp2(depth * -_WeatherMakerFogDensity);
		fogFactor = 1.0 - saturate((expFog * expFog));
		break;

	default:
		fogFactor = 0.0;
		break;
	}
	return min(_WeatherMakerFogFactorMax, fogFactor * _WeatherMakerFogFactorMultiplier);
}

inline fixed GetMieScattering(float cosAngle)
{
	const float MIEGV_COEFF = 0.1;
	const float4 MIEGV = float4(1.0 - (MIEGV_COEFF * MIEGV_COEFF), 1.0 + (MIEGV_COEFF * MIEGV_COEFF), 2.0 * MIEGV_COEFF, 1.0f / (4.0f * 3.14159265358979323846));
	return MIEGV.w * (MIEGV.x / (pow(MIEGV.y - (MIEGV.z * cosAngle), 1.5)));
}

// compute fog using optimized external fog function, no shadows, no noise and no volumetric light
// color is the current pixel color
// rayDir should be normalized, ray direction of the current pixel in world space pointing away from camera
// worldPos the world space position of the pixel
// dirLightColor is the primary dir light color, alpha is intensity, can use float4 _WeatherMakerDirLightColor[0]
// dirLightDir is negative of direction of primary dir light, can use float4 _WeatherMakerDirLightPosition[0].xyz
// ambientColor ambient light color, can use fixed3 _WeatherMakerAmbientLightColor, _WeatherMakerAmbientLightColorSky, _WeatherMakerAmbientLightColorGround or _WeatherMakerAmbientLightColorEquator.
// dirLightPower is how much the dir light should scatter away from the source, can use fixed4 _WeatherMakerDirLightPower[0].x
inline fixed4 ComputeFogLightingExternal
(
	fixed4 color,
	float3 rayDir,
	float3 worldPos,
	fixed4 dirLightColor,
	float3 dirLightDir,
	fixed3 ambientColor,
	fixed dirLightPower
)
{
	// for water or other complex material, depth is not always where the fog is, so let the caller pass the correct world pos and get depth from that
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	depth = max(0.0, depth - _WeatherMakerFogStartDepth - max(0.0, (depth - _WeatherMakerFogEndDepth)));
	float fogFactor = CalculateFogFactor(depth);
	
	// keep fog factor 0 - 1 minus a tiny bit
	fogFactor = saturate(fogFactor) * 0.985;

#if !defined(UNITY_PASS_FORWARDADD)

	float cosAngle = max(0.0, dot(dirLightDir, rayDir));
	float scatterIntensity = pow(cosAngle, dirLightPower) * // spread
		GetMieScattering(cosAngle) * // mie factor
		_WeatherMakerFogLightAbsorption * // absorption
		_WeatherMakerFogDirectionalLightScatterIntensity * // intensity
		(1.0 - fogFactor) * // forward scattering factor
		_WeatherMakerFogDensityScatter * // forward scattering intensity
		max(0.0, 1.0 - (_WeatherMakerFogDensity * 1.2)); // density factor
	fixed3 dirLightColorWithIntensity = dirLightColor.rgb * dirLightColor.a;
	fixed3 fogLightColorWithIntensity = (fogFactor * (dirLightColorWithIntensity + (dirLightColorWithIntensity * scatterIntensity)));
	fixed3 ambientColorWithIntensity = min(1.0, fogFactor * ambientColor * max(0.0, 1.0 - dirLightColor.a)); // reduce ambient with small fog factor and full dir light intensity
	fixed3 fogColor = (fogColorWithIntensity * (fogLightColorWithIntensity + ambientColorWithIntensity));

	return lerp(color, fixed4(fogColor, 1.0), fogFactor);

#else

	return lerp(color, fixed4(0.0, 0.0, 0.0, 0.0), fogFactor);

#endif

}

// calls ComputeFogLightingExternal and passes the weather maker fog parameters and light settings
#if defined(ENABLE_EXTERNAL_FOG_FUNCTION_WITH_WEATHER_MAKER_FOG_PARAMS)

uniform fixed4 _WeatherMakerDirLightColor[16];
uniform float4 _WeatherMakerDirLightPosition[16];
uniform float4 _WeatherMakerDirLightPower[16];
uniform fixed3 _WeatherMakerAmbientLightColor;
uniform fixed3 _WeatherMakerAmbientLightColorSky;
uniform fixed3 _WeatherMakerAmbientLightColorGround;
static const fixed3 weatherMakerAmbientLightColorCombined = (_WeatherMakerAmbientLightColor + _WeatherMakerAmbientLightColorSky + _WeatherMakerAmbientLightColorGround);

fixed4 ComputeFogLightingExternalWithWeatherMakerFogParams(fixed4 color, float3 worldPos)
{
	float3 rayDir = normalize(worldPos - _WorldSpaceCameraPos);
	fixed4 dirLightColor = _WeatherMakerDirLightColor[0];
	float3 dirLightDir = _WeatherMakerDirLightPosition[0].xyz;
	fixed3 ambientColor = weatherMakerAmbientLightColorCombined;
	fixed dirLightPower = _WeatherMakerDirLightPower[0].x;

	return ComputeFogLightingExternal(color, rayDir, worldPos, dirLightColor, dirLightDir, ambientColor, dirLightPower);
}

#endif

#endif
