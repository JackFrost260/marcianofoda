﻿//
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

#include "WeatherMakerCloudVolumetricShaderInclude.cginc"
#include "WeatherMakerFogShaderInclude.cginc"

#define EXTERNAL_FOG_PASS_FUNC ComputeWeatherMakerFog
#define EXTERNAL_SHADOW_PASS_FUNC ComputeWeatherMakerShadowsFade

// --------------------------------------------------------------------------------
// External shader integration functions
// --------------------------------------------------------------------------------

// compute fog light using built in weather maker lighting
// color is the current pixel color
// worldPos the world space position of the pixel
// volumetric can be true for volumetric point,spot,area light or false for just dir light
fixed4 ComputeWeatherMakerFog
(
	fixed4 color,
	float3 worldPos,
	bool volumetric
);

// compute weather maker shadow term (returns 0 to 1, 0 is full shadow, 1 is no shadow)
// worldPos the world space position of the pixel
// existing shadow is simply the shadow multiplier (0 to 1) if it is known for the pixel, pass 1.0 if it is unknown
// sampleDetails can be true for extra shadow details or false for better performance
float ComputeWeatherMakerShadows
(
	float3 worldPos,
	float existingShadow,
	bool sampleDetails
);

// --------------------------------------------------------------------------------
inline fixed4 ComputeWeatherMakerFog(fixed4 color, float3 worldPos, bool volumetric)
{
	float3 rayDir = normalize(worldPos - _WorldSpaceCameraPos);

	UNITY_BRANCH
	if (_WeatherMakerFogMode == 0 || _WeatherMakerFogDensity == 0)
	{
		return color;
	}
	else
	{
		float depth = distance(_WorldSpaceCameraPos, worldPos);
		float3 startPos;
		float noise;
		float2 uv = 0.001 * (worldPos.xz + worldPos.y);
		RaycastFogBoxFullScreen(rayDir, depth, worldPos, startPos, noise);
		float fogFactor = saturate(CalculateFogFactorWithDither(depth, uv) * noise);

#if !defined(UNITY_PASS_FORWARDADD)

		float4 fogColor = ComputeFogLighting(startPos, rayDir, depth, fogFactor, uv, noise, true);
		return lerp(color, fogColor, fogFactor);

#else

		return lerp(color, fixed4(0.0, 0.0, 0.0, 0.0), fogFactor);

#endif

	}
}

inline float ComputeWeatherMakerShadows(float3 worldPos, float existingShadow, bool sampleDetails)
{
	float shadow = ComputeCloudShadowStrength(worldPos, 0, existingShadow, sampleDetails);
	return shadow * shadow * shadow;
}

inline float ComputeWeatherMakerShadowsFade(float3 worldPos, float existingShadow, bool sampleDetails, inout half shadowFade)
{
	shadowFade = 0.0;
	float shadow = ComputeCloudShadowStrength(worldPos, 0, existingShadow, sampleDetails);
	return shadow * shadow * shadow;
}

#endif
