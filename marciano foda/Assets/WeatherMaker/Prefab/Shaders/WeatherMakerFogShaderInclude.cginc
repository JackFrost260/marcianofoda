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

// receive shadows: http://www.gamasutra.com/blogs/JoeyFladderak/20140416/215612/Let_there_be_shadow.php

#ifndef WEATHER_MAKER_FOG_SHADER_INCLUDED
#define WEATHER_MAKER_FOG_SHADER_INCLUDED

#include "WeatherMakerNullZoneShaderInclude.cginc"
#include "WeatherMakerLightShaderInclude.cginc"
#include "WeatherMakerFogExternalShaderInclude.cginc"

inline float CalculateFogFactorWithDither(float depth, float2 screenUV)
{
	// slight dither to fog factor to give it a little more randomness
	float fogFactor = CalculateFogFactor(depth);
	float dither = 0.003 * frac(_WeatherMakerTime.x + (cos(dot(screenUV, ditherMagic.xy)) * ditherMagic.z));
	return min(1.0, fogFactor + dither);
}

inline float CalculateFogDirectionalLightScatter(float3 rayDir, float3 lightDir, float fogFactor, float power, float multiplier)
{
	float cosAngle = max(0.0, dot(lightDir, rayDir));
	float scatter = pow(cosAngle, power);
	return scatter * GetMieScattering(cosAngle) * _WeatherMakerFogLightAbsorption * _WeatherMakerFogDirectionalLightScatterIntensity * (1.0 - fogFactor) * multiplier * max(0.0, 1.0 - (_WeatherMakerFogDensity * 1.2));
}

fixed3 ComputeDirectionalLightFog(float3 rayOrigin, float3 rayDir, float rayLength, float fogFactor, float2 screenUV)
{
	float fogFactorSquared = fogFactor * fogFactor;

	// add full ambient as sun intensity approaches 0
	fixed3 ambient = min(1.0, _WeatherMakerAmbientLightColorGround.rgb * _AmbientLightMultiplier * max(0.0, 1.0 - _WeatherMakerSunColor.a));
	fixed3 lightColor = ambient * fogFactor;

	// sun light + scatter
	fixed3 sunLightColor = (_WeatherMakerSunColor.rgb * _WeatherMakerSunColor.a * _DirectionalLightMultiplier);
	float scatter = CalculateFogDirectionalLightScatter(rayDir, _WeatherMakerSunDirectionUp, fogFactor, _WeatherMakerSunLightPower.x, _WeatherMakerFogDensityScatter);
	static const float cloudShadowSquared = lerp(1.0, _WeatherMakerCloudGlobalShadow2 * _WeatherMakerCloudGlobalShadow2, _WeatherMakerFogCloudShadowStrength);
	static const float cloudShadowSquared2 = cloudShadowSquared * cloudShadowSquared;
	fixed3 baseScatter = (fogFactor * ((sunLightColor * _WeatherMakerCloudGlobalShadow2) + (sunLightColor * scatter * cloudShadowSquared)));

	UNITY_BRANCH
	if (WM_FOG_VOLUMETRIC_LIGHT_MODE_ENABLE_WITH_SHADOWS && WM_CAMERA_RENDER_MODE_NORMAL && _WeatherMakerFogLightShadowBrightness > 0.0 && cloudShadowSquared2 > 0.001)
	{
		float4 wpos = float4(rayOrigin, 1.0);
		float shadowPower = 0.0;
		float4 cascadeWeights;
		float4 samplePos;
		float shadowDepth;
		float lightDot = max(_WeatherMakerFogLightShadowDecay, dot(_WeatherMakerSunDirectionUp, rayDir));
		//float dither = 1.0 + (_WeatherMakerFogLightShadowDither * (tex2Dlod(_WeatherMakerBlueNoiseTexture, float4(screenUV + (_WeatherMakerTime.y), 0.0, 0.0)) - 0.5));

		// dithering
		float dither = (1.0 + (_WeatherMakerFogLightShadowDither / max(0.4, fogFactor + (2 * lightDot)) * frac(cos(dot(screenUV * _WeatherMakerTime.x, ditherMagic.xy)) * ditherMagic.z) *
			(frac(dot(_WeatherMakerFogLightShadowDitherMagic.xy, screenUV * _WeatherMakerFogLightShadowDitherMagic.zw)) - 0.5)));

		lightDot = pow(lightDot, _WeatherMakerFogLightShadowPower);
		lightDot = 1.0 + (_WeatherMakerFogLightShadowMultiplier * lightDot);

		float sampleCount = floor(min(rayLength * 3, _WeatherMakerFogLightShadowSampleCount));
		float invSampleCount = (1.0 / (float)sampleCount);
		float stepAmount = (min(rayLength, _WeatherMakerFogLightShadowMaxRayLength) * invSampleCount);
		float3 stepDir = rayDir * stepAmount;

		stepDir *= dither;

		// for sun, ray march through the shadow map
		UNITY_LOOP
		for (int i = 0; i < int(sampleCount); i++)
		{
			wpos.xyz += stepDir;
			cascadeWeights = GET_CASCADE_WEIGHTS(wpos);
			samplePos = GET_SHADOW_COORDINATES(wpos, cascadeWeights);
			shadowPower += UNITY_SAMPLE_SHADOW(_WeatherMakerShadowMapTexture, samplePos);
		}

		float fogShadowInfluence = saturate(1.0 - (2.0 * fogFactor));
		float shadowing = FOG_SHADOW_BASE_LIGHT_INTENSITY + (fogShadowInfluence * shadowPower * invSampleCount * lightDot * _WeatherMakerFogLightShadowBrightness);
		lightColor += (baseScatter * shadowing * cloudShadowSquared2);
	}
	else
	{
		lightColor += baseScatter;
	}

	// moon light + scatter
	UNITY_LOOP
	for (uint i = 0; i < uint(_WeatherMakerDirLightCount); i++)
	{
		UNITY_BRANCH
		if (_WeatherMakerDirLightDirection[i].w == 0.0) // not a sun
		{
			fixed moonIntensity = _WeatherMakerDirLightColor[i].a * _DirectionalLightMultiplier;
			fixed3 moonLightColor = _WeatherMakerDirLightColor[i].rgb * moonIntensity;
			scatter = CalculateFogDirectionalLightScatter(rayDir, _WeatherMakerDirLightPosition[i].xyz, fogFactor, _WeatherMakerDirLightPower[i].x, _WeatherMakerFogDensityScatter);
			moonLightColor = (fogFactor * (moonLightColor + (moonLightColor * scatter)));
			lightColor += moonLightColor;
		}
	}

	// don't apply weatherMakerGlobalShadow as it includes the fog which will self shadow itself and get too dark
	lightColor *= _WeatherMakerFogDensityScatter * _WeatherMakerCloudGlobalShadow;

	return lightColor;
}

inline void ComputeLightColorForFogPointLight
(
	float3 rayOrigin,
	float3 rayDir,
	float lightAmount,
	float distanceToLight,
	float4 lightPos,
	fixed4 lightColor,
	float4 lightAtten,
	float lightMultiplier,
	float3 ditherColor,
	inout fixed3 accumLightColor)
{
	// amount to move for each sample
	float3 step;

	// fog factor reducer is the amount of fog in front of the light, used to reduce the light
	float fogFactorReducer = 1.0 - CalculateFogFactor(distanceToLight * 0.33);

	// amount of fog on light ray
	float fogFactorOnRay = CalculateFogFactor(lightAmount);

	// sample points along the ray
	float lightSample;
	float attenSample = 0.0;
	float eyeLightDot;
	float3 startPos = rayOrigin + (rayDir * distanceToLight);
	float3 currentPos;
	float3 toLight;

	step = rayDir * (lightAmount * FOG_LIGHT_POINT_SAMPLE_COUNT_INVERSE);
	currentPos = startPos - (step * 0.5);
	lightSample = 0.0;
	float dither = 1.0 + (fogLightDitherLevel * RandomFloat(rayDir + _WeatherMakerTime.x));
	step *= dither;

	UNITY_LOOP
	for (int i = 0; i < int(FOG_LIGHT_POINT_SAMPLE_COUNT); i++)
	{
		currentPos += step;
		toLight = currentPos - lightPos.xyz;
		lightSample += dot(toLight, toLight);
	}

	// average samples
	lightSample *= FOG_LIGHT_POINT_SAMPLE_COUNT_INVERSE;

	// calculate atten from distance from center
	lightSample = max(0.0, 1.0 - (lightSample * lightAtten.w));
	lightSample *= lightSample * min(1.0, fogFactorOnRay * 10.0) * lightColor.a;

	// as camera approaches light position, reduce amount of light
	// right next to the light there is less light travelling to the eye through the fog
	toLight = _WorldSpaceCameraPos - lightPos.xyz;
	float d = dot(toLight, toLight);
	lightAmount = lightSample * clamp(d * lightAtten.w, 0.2, 1.0);

	// apply color
	accumLightColor += (lightColor.rgb * lightAmount * fogFactorReducer * fogFactorOnRay * lightMultiplier * _WeatherMakerFogLightAbsorption

#if defined(UNITY_COLORSPACE_GAMMA)

		// brighten up gamma space to make it look more like linear
		* 1.8

#endif

		);
}

inline void ComputeLightColorForFogSpotLight
(
	float3 rayOrigin,
	float3 rayDir,
	float lightAmount,
	float distanceToLight,
	float4 lightPos,
	fixed4 lightColor,
	float4 lightAtten,
	float4 lightDir,
	float4 lightEnd,
	float lightMultiplier,
	float3 ditherColor,
	inout fixed3 accumLightColor)
{
	// amount to move for each sample
	float3 step;

	// fog factor reducer is the amount of fog in front of the light, used to reduce the light
	float fogFactorReducer = 1.0 - CalculateFogFactor(distanceToLight * 0.33);

	// amount of fog on light ray
	float fogFactorOnRay = CalculateFogFactor(lightAmount);

	// sample points along the ray
	float lightSample;
	float attenSample = 0.0;
	float eyeLightDot;
	float3 startPos = rayOrigin + (rayDir * distanceToLight);
	float3 currentPos;
	float3 toLight;

	float dotSample1 = 0.0;
	float dotSample2 = 0.0;
	float distanceSample = 0.0;

	lightSample = 9999999.0;
	step = rayDir * (lightAmount * FOG_LIGHT_SPOT_SAMPLE_COUNT_INVERSE);
	currentPos = startPos - (step * 0.5);
	float dither = 1.0 + (fogLightDitherLevel * RandomFloat(rayDir + _WeatherMakerTime.x));
	step *= dither;

	UNITY_LOOP
	for (int i = 0; i < int(FOG_LIGHT_SPOT_SAMPLE_COUNT); i++)
	{
		currentPos += step;
		toLight = currentPos - lightPos.xyz;
		eyeLightDot = dot(toLight, toLight);
		distanceSample += eyeLightDot;
		lightSample = min(eyeLightDot, lightSample);
		eyeLightDot = saturate(((dot(normalize(toLight), lightDir.xyz)) - lightAtten.x) * lightAtten.y);
		dotSample1 = max(eyeLightDot, dotSample1);
		dotSample2 += eyeLightDot;
	}

	// calculate dot attenuation, light at more of an angle is dimmer
	eyeLightDot = (dotSample1 * 0.75) + (dotSample2 * FOG_LIGHT_SPOT_SAMPLE_COUNT_INVERSE * 0.25);
	dotSample1 = pow(eyeLightDot, _WeatherMakerFogLightFalloff.x * lightPos.w);

	// calculate light attenuation
	lightSample = 1.0 / (1.0 + (lightSample * lightAtten.z));

	// increase as eye looks at center from forward direction
	eyeLightDot = max(0.0, dot(-rayDir, lightDir.xyz));
	dotSample1 *= (1.0 + (lightColor.a * 2.0 * pow(eyeLightDot, 3.0)));

	// reduce light right near the tip to eliminate hard edges
	lightAmount = (min(1.0, distanceSample * FOG_LIGHT_SPOT_SAMPLE_COUNT_INVERSE)) * lightSample * dotSample1 * lightColor.a;

	// apply color
	accumLightColor += (lightColor.rgb * lightAmount * fogFactorReducer * fogFactorOnRay * lightMultiplier * _WeatherMakerFogLightAbsorption

#if defined(UNITY_COLORSPACE_GAMMA)

		// brighten up gamma space to make it look more like linear
		* 1.8

#endif

		);
}

#if !defined(WEATHER_MAKER_DISABLE_AREA_LIGHTS)

inline void ComputeLightColorForFogAreaLight
(
	float3 rayOrigin,
	float3 rayDir,
	float lightAmount,
	float distanceToLight,
	fixed4 lightColor,
	float4 lightAtten,
	float3 lightPos,
	float3 lightDir,
	float3 lightBoxMin,
	float3 lightBoxMax,
	float lightMultiplier,
	float3 ditherColor,
	inout fixed3 accumLightColor)
{
	// amount to move for each sample
	float3 step;

	// fog factor reducer is the amount of fog in front of the light, used to reduce the light
	float fogFactorReducer = 1.0 - CalculateFogFactor(distanceToLight * 0.33);

	// amount of fog on light ray
	float fogFactorOnRay = CalculateFogFactor(lightAmount);

	// sample points along the ray
	float lightSample;
	float attenSample = 0.0;
	float eyeLightDot;
	float3 startPos = rayOrigin + (rayDir * distanceToLight);
	float3 currentPos;
	float3 toLight;
	float centerDistanceSquared;
	float dx;
	float dy;
	step = rayDir * (lightAmount * FOG_LIGHT_AREA_SAMPLE_COUNT_INVERSE);

	currentPos = startPos - (step * 0.5);
	lightSample = 0.0;

	float dither = 1.0 + (fogLightDitherLevel * RandomFloat(rayDir + _WeatherMakerTime.x));
	step *= dither;

	UNITY_LOOP
	for (int i = 0; i < int(FOG_LIGHT_AREA_SAMPLE_COUNT); i++)
	{
		currentPos += step;
		toLight = currentPos - lightPos.xyz;
		eyeLightDot = dot(toLight, toLight);
		attenSample = (1.0 / (1.0 + (eyeLightDot * lightAtten.z)));
		attenSample *= (1.0 - min(1.0, (eyeLightDot * lightAtten.w)));
		dx = max(lightBoxMin.x - toLight.x, toLight.x - lightBoxMax.x);
		dy = max(lightBoxMin.y - toLight.y, toLight.y - lightBoxMax.y);
		attenSample *= min(1.0, ((dx + dx) * (dy + dy)) * lightAtten.x);
		lightSample += attenSample;
	}

	// average samples
	lightSample *= FOG_LIGHT_AREA_SAMPLE_COUNT_INVERSE * CalculateFogFactor(lightAmount * 3.0) * lightColor.a; // * 3 because area lights look too dim in the fog otherwise

	// adjust intensity as eye looks at light
	eyeLightDot = max(0.0, -dot(rayDir, lightDir.xyz));
	lightSample *= (1.0 + eyeLightDot);

	// apply color
	accumLightColor += (lightColor.rgb * lightSample * fogFactorReducer * fogFactorOnRay * lightMultiplier * _WeatherMakerFogLightAbsorption

#if defined(UNITY_COLORSPACE_GAMMA)

		// brighten up gamma space to make it look more like linear
		* 1.8

#endif

		);
}

#endif

fixed3 ComputePointLightFog(float3 rayOrigin, float3 rayDir, float rayLength, float fogFactor, fixed lightMultiplier, float3 ditherColor)
{
	float lightAmount;
	float distanceToLight;
	float2 sphere;
	fixed3 lightColor = fixed3Zero;

	// point lights
	UNITY_LOOP
	for (int lightIndex = 0; lightIndex < _WeatherMakerPointLightCount; lightIndex++)
	{
		// get the length of the ray intersecting the point light sphere
		sphere = RaySphereIntersect(rayOrigin, rayDir, rayLength, _WeatherMakerPointLightPosition[lightIndex]);
		UNITY_BRANCH
		if (sphere.y > 0.0)
		{
			// compute lighting for the point light
			ComputeLightColorForFogPointLight(rayOrigin, rayDir, sphere.y, sphere.x, _WeatherMakerPointLightPosition[lightIndex],
				_WeatherMakerPointLightColor[lightIndex], _WeatherMakerPointLightAtten[lightIndex], lightMultiplier, ditherColor, lightColor);
		}
	}

	return lightColor;
}

fixed3 ComputeSpotLightFog(float3 rayOrigin, float3 rayDir, float rayLength, float fogFactor, fixed lightMultiplier, float3 ditherColor)
{
	float lightAmount;
	float distanceToLight;
	fixed3 lightColor = fixed3Zero;

	// spot lights
	UNITY_LOOP
	for (int lightIndex = 0; lightIndex < _WeatherMakerSpotLightCount; lightIndex++)
	{
		// get the length of the ray intersecting the spot light cone
		UNITY_BRANCH
		if (RayConeIntersect(rayOrigin, rayDir, rayLength, _WeatherMakerSpotLightPosition[lightIndex], _WeatherMakerSpotLightDirection[lightIndex],
			_WeatherMakerSpotLightSpotEnd[lightIndex], _WeatherMakerSpotLightAtten[lightIndex].x, lightAmount, distanceToLight))
		{
			// compute lighting for the spot light
			ComputeLightColorForFogSpotLight(rayOrigin, rayDir, lightAmount, distanceToLight, _WeatherMakerSpotLightPosition[lightIndex],
				_WeatherMakerSpotLightColor[lightIndex], _WeatherMakerSpotLightAtten[lightIndex], _WeatherMakerSpotLightDirection[lightIndex],
				_WeatherMakerSpotLightSpotEnd[lightIndex], lightMultiplier, ditherColor, lightColor);
		}
	}

	return lightColor;
}

fixed3 ComputeAreaLightFog(float3 rayOrigin, float3 rayDir, float rayLength, float fogFactor, fixed lightMultiplier, float3 ditherColor)
{
	fixed3 lightColor = fixed3Zero;

#if !defined(WEATHER_MAKER_DISABLE_AREA_LIGHTS)

	// area lights are always the origin pointing forward on z axis
	const float3 lightPos = float3Zero;
	const float3 lightDir = float3(0.0, 0.0, 1.0);

	float lightAmount;
	float distanceToLight;

	// area lights
	UNITY_LOOP
	for (int lightIndex = 0; lightIndex < _WeatherMakerAreaLightCount; lightIndex++)
	{
		// get the length of the ray intersecting the area light box
		rayOrigin = RotatePointZeroOriginQuaternion(rayOrigin - _WeatherMakerAreaLightPosition[lightIndex], _WeatherMakerAreaLightRotation[lightIndex]);
		rayDir = RotatePointZeroOriginQuaternion(rayDir, _WeatherMakerAreaLightRotation[lightIndex]);

		UNITY_BRANCH
		if (RayBoxIntersect(rayOrigin, rayDir, rayLength, _WeatherMakerAreaLightMinPosition[lightIndex].xyz,
			_WeatherMakerAreaLightMaxPosition[lightIndex].xyz, lightAmount, distanceToLight))
		{
			// compute lighting for the area light
			ComputeLightColorForFogAreaLight(rayOrigin, rayDir, lightAmount, distanceToLight, _WeatherMakerAreaLightColor[lightIndex],
				_WeatherMakerAreaLightAtten[lightIndex], lightPos, lightDir, _WeatherMakerAreaLightMinPosition[lightIndex],
				_WeatherMakerAreaLightMaxPosition[lightIndex], lightMultiplier, ditherColor, lightColor);
		}
	}

#endif

	return lightColor;
}

// f is fog factor, rayLength is distance of fog in ray, savedDepth is depth buffer
fixed4 ComputeFogLighting(float3 rayOrigin, float3 rayDir, float rayLength, float fogFactor, float2 screenUV, float noise)
{
	// TODO: Null zones do not eliminate light, they just reduce fog amount for pixel,
	// is there an optimized way to determine how much of a light volume is null zoned out?
	// skip expensive lighting where there is no fog
	UNITY_BRANCH
	if (fogFactor < 0.00001)
	{
		return fixed4Zero;
	}
	else
	{
		fixed4 lightColor = fixed4Zero;

		// directional light / ambient
		lightColor.rgb += ComputeDirectionalLightFog(rayOrigin, rayDir, rayLength, fogFactor, screenUV);

		UNITY_BRANCH
		if (WM_FOG_VOLUMETRIC_LIGHT_MODE_NOT_NONE)
		{
			float lightMultiplier = _PointSpotLightMultiplier * _WeatherMakerVolumetricPointSpotMultiplier;
			float3 ditherColor = float3One;
			lightColor.rgb += ComputePointLightFog(rayOrigin, rayDir, rayLength, fogFactor, lightMultiplier, ditherColor);
			lightColor.rgb += ComputeSpotLightFog(rayOrigin, rayDir, rayLength, fogFactor, lightMultiplier, ditherColor);
			lightColor.rgb += ComputeAreaLightFog(rayOrigin, rayDir, rayLength, fogFactor, lightMultiplier, ditherColor);
		}

		// take advantage of the fact that dir lights are sorted by perspective/ortho and then by intensity
		UNITY_LOOP
		for (uint lightIndex = 0; lightIndex < uint(_WeatherMakerDirLightCount) && _WeatherMakerDirLightVar1[lightIndex].y == 0.0 && _WeatherMakerDirLightColor[lightIndex].a > 0.001; lightIndex++)
		{
			lightColor.rgb += ComputeDirLightShaftColor(screenUV, fogFactor, _WeatherMakerDirLightViewportPosition[lightIndex], _WeatherMakerFogColor * _WeatherMakerDirLightColor[lightIndex].rgb * _WeatherMakerDirLightVar1[lightIndex].z);
		}

		lightColor.rgb += _WeatherMakerFogEmissionColor.rgb; // _WeatherMakerFogEmissionColor.a is already built into the shader property
		lightColor.rgb *= (_WeatherMakerFogColor.rgb * noise);

		lightColor.a = fogFactor;
		ApplyDither(lightColor.rgb, screenUV, _WeatherMakerFogDitherLevel);
		return lightColor;
	}
}

inline float CalculateFogNoise3D(float3 pos, float3 rayDir, float rayLength, float scale, float3 velocity)
{
	float n = 0.0;
	float n2;
	float3 step = rayDir * scale;

	//pos += RotatePoint(_WeatherMakerFogBoxCenter, float3(0, _WeatherMakerTime.x * 4, 0));// _WeatherMakerTime.y * 0.02, _WeatherMakerTime.x * 0.03));
	pos += _WeatherMakerFogNoisePositionOffset;

	pos *= scale;
	//pos += (step * int(_WeatherMakerFogNoiseSampleCount));

	UNITY_LOOP
	for (int i = 0; i < int(_WeatherMakerFogNoiseSampleCount); i++)
		//for (int i = int(_WeatherMakerFogNoiseSampleCount); i > 0; i--)
	{
		float4 uv = float4(pos + velocity, 0.0);
		n += tex3Dlod(_WeatherMakerNoiseTexture3D, uv).a;
		pos += step;

		//n2 = tex3Dlod(_WeatherMakerNoiseTexture3D, float4(pos + velocity, -999.0)).a;
		//n2 = (n2 + _WeatherMakerFogNoiseAdder) * _WeatherMakerFogNoiseMultiplier;
		//n = n2 + (n * (1.0 - n2));
		//pos -= step;
	}

	//return n;
	n = ((n * _WeatherMakerFogNoiseSampleCountInverse) + _WeatherMakerFogNoiseAdder) * _WeatherMakerFogNoiseMultiplier;
	return lerp(1.0, n, _WeatherMakerFogNoisePercent);
}

inline float CalculateFogNoise3DOne(float3 pos, float scale, float3 velocity)
{
	return tex3Dlod(_WeatherMakerNoiseTexture3D, float4((pos * scale) + velocity, -999.0)).a;
}

inline void RaycastFogBoxFullScreen(float3 rayDir, float3 forwardLine, inout float depth, out float3 startPos, out float noise)
{
	// depth is 0-1 value, which needs to be changed to world space distance
	float3 endPos = lerp(_WorldSpaceCameraPos + (depth * forwardLine), _WorldSpaceCameraPos + (rayDir * depth * _ProjectionParams.z), WM_CAMERA_RENDER_MODE_CUBEMAP);

	// calculate depth exactly in world space
	depth = distance(endPos, _WorldSpaceCameraPos);

	UNITY_BRANCH
	if (WM_FOG_HEIGHT_ENABLED)
	{
		// cast ray, get amount of intersection with the fog box
		float3 boxMin, boxMax;
		float distanceToBox;
		float intersect;
		GetFullScreenBoundingBox(_WeatherMakerFogHeight, boxMin, boxMax);
		intersect = RayBoxIntersect(_WorldSpaceCameraPos, rayDir, depth, boxMin, boxMax, depth, distanceToBox);

		// calculate start pos where fog begins
		startPos = _WorldSpaceCameraPos + (rayDir * distanceToBox);

		// remove null zones from depth calculation
		GetNullZonesDepth(startPos, rayDir, depth);

		// we want a branch here, avoid that nasty 3d texture lookup if no fog noise
		UNITY_BRANCH
		if (WM_FOG_NOISE_ENABLED)
		{
			// calculate noise
			noise = CalculateFogNoise3D(startPos, rayDir, depth, _WeatherMakerFogNoiseScale, _WeatherMakerFogNoiseVelocity);
		}
		else
		{
			noise = 1.0;
		}

		// remove noise where there is no fog
		noise *= (depth > 0.0 && _WeatherMakerFogDensity > 0.0);

		// reduce noise as fog gets near the top
		noise *= lerp(1.0, _WeatherMakerFogHeightFalloffPower, saturate(startPos.y / boxMax.y));
	}
	else
	{
		// move start pos to fog start
		startPos = _WorldSpaceCameraPos + (rayDir * _WeatherMakerFogStartDepth);

		// remove from depth where the start and end ranges denote no fog
		depth = max(0.0, depth - _WeatherMakerFogStartDepth - max(0.0, (depth - _WeatherMakerFogEndDepth)));

		// remove null zones from depth calculation
		GetNullZonesDepth(startPos, rayDir, depth);

		// we want a branch here, avoid that nasty 3d texture lookup if no fog noise
		UNITY_BRANCH
		if (WM_FOG_NOISE_ENABLED)
		{
			noise = CalculateFogNoise3D(startPos, rayDir, depth, _WeatherMakerFogNoiseScale, _WeatherMakerFogNoiseVelocity);
		}
		else
		{
			noise = 1.0;
		}
	}
}

// returns the original scene depth
inline void RaycastFogBox(float3 rayDir, float3 normal, float2 screenUV, inout float depth, out float3 startPos, out float noise)
{
	// cast ray, get amount of intersection with the fog box
	float distanceToBox;

	UNITY_BRANCH
	if (RayBoxIntersect(_WorldSpaceCameraPos, rayDir, depth, _WeatherMakerFogBoxMin, _WeatherMakerFogBoxMax, depth, distanceToBox) == 0.0)
	{
		depth = 0.0;
		startPos = float3Zero;
		noise = 0.0;
	}
	else
	{
		startPos = _WorldSpaceCameraPos + (rayDir * distanceToBox);

		UNITY_BRANCH
		if (WM_FOG_NOISE_ENABLED)
		{
			// calculate noise
			noise = CalculateFogNoise3D(startPos, rayDir, depth, _WeatherMakerFogNoiseScale, _WeatherMakerFogNoiseVelocity);
		}
		else
		{
			noise = 1.0;
		}

		UNITY_BRANCH
		if (_WeatherMakerFogVolumePower.x > 0.0)
		{
			// attempt to smooth height and edges of box fog, these are much more noticeable than sphere fog
			float3 endPos = startPos + (rayDir * depth);
			float3 pos = startPos;
			const float samples = 8;
			const float invSamples = 1.0 / 8.0;
			float smoothFactor = 0.0;
			float3 step = (endPos - startPos) * invSamples;
			float3 diam = (_WeatherMakerFogBoxMax - _WeatherMakerFogBoxMin);
			float diamYInv = 1.0 / diam.y;
			float3 halfDiamInv = 1.0 / (diam * 0.5);
			float yMax = _WeatherMakerFogBoxMax.y;
			for (int i = 0; i < samples; i++)
			{
				pos += step;
				float3 diff = min(pos - _WeatherMakerFogBoxMin, _WeatherMakerFogBoxMax - pos);
				float heightFactor = saturate(0.75 + ((yMax - pos.y) * diamYInv));
				heightFactor = pow(heightFactor, _WeatherMakerFogVolumePower.z);
				diff *= halfDiamInv;
				smoothFactor += (heightFactor * pow(max(_WeatherMakerFogVolumePower.y, min(diff.x + diff.y, min(diff.y + diff.z, diff.z + diff.x))), _WeatherMakerFogVolumePower.w));
				//factor += max(0.1, min(diff.x, max(diff.y, diff.z)));
				//factor += max(0.3, (diff.x + diff.y + diff.z) * 0.33);
				//factor += clamp(diff.x + diff.y + diff.z, 1, 2);
			}
			smoothFactor = min(1.0, smoothFactor * invSamples);

			noise *= smoothFactor;

			// reset startPos to new point
			startPos = _WorldSpaceCameraPos + (rayDir * distanceToBox);
		}
	}
}

inline void RaycastFogSphere(float3 rayDir, float3 normal, inout float depth, out float3 startPos, out float noise)
{
	float4 pos = _WeatherMakerFogSpherePosition;
	float2 sphere = RaySphereIntersect(_WorldSpaceCameraPos, rayDir, depth, pos);

	UNITY_BRANCH
	if (sphere.y <= 0.0)
	{
		depth = 0.0;
		startPos = float3Zero;
		noise = 0.0;
	}
	else
	{
		depth = sphere.y;
		startPos = _WorldSpaceCameraPos + (rayDir * sphere.x);

		// attempt to smooth height and edges of sphere fog
		float halfDepth = depth * 0.5;
		float halfDepthSquared = halfDepth * halfDepth;
		float factor = min(1.0, halfDepthSquared * _WeatherMakerFogVolumePower.x * _WeatherMakerFogVolumePower.y * 10.0);

		if (WM_FOG_NOISE_ENABLED)
		{
			// calculate noise
			noise = CalculateFogNoise3D(startPos, rayDir, depth, _WeatherMakerFogNoiseScale, _WeatherMakerFogNoiseVelocity);
		}
		else
		{
			noise = 1.0;
		}

		noise *= pow(factor, _WeatherMakerFogVolumePower.w);
	}
}

// sphere is xyz, w = radius squared, returns clarity
inline float RayMarchFogSphere(volumetric_data i, int iterations, float4 sphere, float density, float outerDensity, out float clarity, out float3 rayDir, out float3 sphereCenterViewSpace, out float maxDistance)
{
	float2 screenUV = i.projPos.xy / i.projPos.w;
	maxDistance = length(DECODE_EYEDEPTH(WM_SAMPLE_DEPTH(screenUV)) / normalize(i.viewPos).z);
	rayDir = normalize(i.viewPos.xyz);
	sphereCenterViewSpace = mul((float3x3)UNITY_MATRIX_V, (_WorldSpaceCameraPos - sphere.xyz));
	float invSphereRadiusSquared = 1.0 / sphere.w;

	// calculate sphere intersection
	float b = -dot(rayDir, sphereCenterViewSpace);
	float c = dot(sphereCenterViewSpace, sphereCenterViewSpace) - sphere.w;
	float d = sqrt((b * b) - c);
	float dist = b - d;
	float dist2 = b + d;

	/*
	float fA = dot(rayDir, rayDir);
	float fB = 2 * dot(rayDir, sphereCenterViewSpace);
	float fC = dot(sphereCenterViewSpace, sphereCenterViewSpace) - sphere.w;
	float fD = fB * fB - 4 * fA * fC;
	// if (fD <= 0.0f) { return; } // not sure if this is needed, doesn't seem to trigger very often
	float recpTwoA = 0.5 / fA;
	float DSqrt = sqrt(fD);
	// the distance to the front of sphere, or 0 if inside the sphere. This is the distance from the camera where sampling begins.
	float dist = max((-fB - DSqrt) * recpTwoA, 0);
	// total distance to the back of the sphere.
	float dist2 = max((-fB + DSqrt) * recpTwoA, 0);
	*/

	// stop at the back of the sphere or depth buffer, whichever is the smaller distance.
	float backDepth = min(maxDistance, dist2);

	// calculate initial sample distance, and the distance between samples.
	float samp = dist;
	float step_distance = (backDepth - dist) / (float)iterations;

	// how much does each step get modified? approaches 1 with distance.
	float step_contribution = (1 - 1 / pow(2, step_distance)) * density;

	// 1 means no fog, 0 means completely opaque fog
	clarity = 1;

	UNITY_LOOP
	for (int i = 0; i < iterations; i++)
	{
		float3 position = sphereCenterViewSpace + (rayDir * samp);
		float val = saturate(outerDensity * (1.0 - (dot(position, position) * invSphereRadiusSquared)));
		clarity *= (1.0 - saturate(val * step_contribution));
		samp += step_distance;
	}

	return clarity;
}

inline void PreFogFragment(volumetric_data i, out float depth, out float depth01, out float2 screenUV, out float3 rayDir)
{
	// get the depth of this pixel
	screenUV = i.projPos.xy / i.projPos.w;
	depth = WM_SAMPLE_DEPTH(screenUV);
	depth01 = WM_LINEAR_DEPTH_01(depth);
	depth = length(DECODE_EYEDEPTH(depth) / normalize(i.viewPos).z);
	rayDir = normalize(i.rayDir);
}

inline fixed4 PostFogFragment(float3 startPos, float3 rayDir, float amount, float noise, float2 screenUV, out float fogFactor)
{
	GetNullZonesDepth(startPos, rayDir, amount);
	fogFactor = saturate(CalculateFogFactor(amount) * noise);
	return ComputeFogLighting(startPos, rayDir, amount, fogFactor, screenUV, noise);
}

inline fixed ComputeFullScreenFogAlphaTemporalReprojection(full_screen_fragment i)
{
	UNITY_BRANCH
	if (_WeatherMakerFogMode == 0)
	{
		return 0.0;
	}
	else
	{
		float3 rayDir = GetFullScreenRayDir(i.rayDir);
		float depth01 = WM_SAMPLE_DEPTH_DOWNSAMPLED_TEMPORAL_REPROJECTION_01(i.uv.xy);
		float noise;
		float3 startPos;
		float depth = depth01; // gets set to the fog amount on the ray
		RaycastFogBoxFullScreen(rayDir, i.forwardLine, depth, startPos, noise);
		return saturate(CalculateFogFactorWithDither(depth, i.uv) * noise);
	}
}

// VERTEX AND FRAGMENT SHADERS ----------------------------------------------------------------------------------------------------

volumetric_data fog_volume_vertex_shader(appdata_base v)
{
	return GetVolumetricData(v);
}

fixed4 fog_box_full_screen_fragment_shader(full_screen_fragment i) : SV_Target
{
	UNITY_BRANCH
	if (_WeatherMakerFogMode == 0)
	{
		return fixed4Zero;
	}
	float3 rayDir = GetFullScreenRayDir(i.rayDir);
	float depth01 = WM_SAMPLE_DEPTH_DOWNSAMPLED_TEMPORAL_REPROJECTION_01(i.uv.xy);
	float noise;
	float3 startPos;
	float depth = depth01; // gets set to the fog amount on the ray
	RaycastFogBoxFullScreen(rayDir, i.forwardLine, depth, startPos, noise);
	float fogFactor = saturate(CalculateFogFactorWithDither(depth, i.uv) * noise);
	return ComputeFogLighting(startPos, rayDir, depth, fogFactor, i.uv, noise);
}

fixed4 fog_box_fragment(volumetric_data i, out float depth, out float fogFactor, out float3 rayDir)
{
	WM_INSTANCE_FRAG(i);
	float noise;
	float2 screenUV;
	float depth01;
	PreFogFragment(i, depth, depth01, screenUV, rayDir);
	float3 startPos;
	RaycastFogBox(rayDir, i.normal, screenUV, depth, startPos, noise);

	UNITY_BRANCH
	if (_WeatherMakerFogMode == 0)
	{
		fogFactor = 0.0;
		return fixed4Zero;
	}
	else
	{
		return PostFogFragment(startPos, rayDir, depth, noise, screenUV, fogFactor);
	}
}

fixed4 fog_box_fragment_shader(volumetric_data i) : SV_TARGET
{
	WM_INSTANCE_FRAG(i);

	UNITY_BRANCH
	if (_WeatherMakerFogMode == 0)
	{
		return fixed4Zero;
	}

	float depth, fogFactor;
	float3 rayDir;
	return fog_box_fragment(i, depth, fogFactor, rayDir);
}

fixed4 fog_sphere_fragment_shader(volumetric_data i) : SV_TARGET
{
	WM_INSTANCE_FRAG(i);

	UNITY_BRANCH
	if (_WeatherMakerFogMode == 0)
	{
		return fixed4Zero;
	}

	float noise, fogFactor;
	float2 screenUV;
	float3 startPos, rayDir;
	float depth, depth01;
	PreFogFragment(i, depth, depth01, screenUV, rayDir);
	RaycastFogSphere(rayDir, i.normal, depth, startPos, noise);
	return PostFogFragment(startPos, rayDir, depth, noise, screenUV, fogFactor);
}

#endif
