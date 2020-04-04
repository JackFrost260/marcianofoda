﻿// Weather Maker for Unity
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

#ifndef __WEATHER_MAKER_CLOUD_SHADER__
#define __WEATHER_MAKER_CLOUD_SHADER__

#include "WeatherMakerSkyShaderInclude.cginc"

#define blendClouds(cloudColor, finalColor) \
	finalColor.rgb = (cloudColor.rgb * cloudColor.a) + (finalColor.rgb * (1.0 - cloudColor.a)); \
	finalColor.a = max(finalColor.a, cloudColor.a);

// flat
UNITY_DECLARE_TEX2D(_CloudNoise1);
UNITY_DECLARE_TEX2D_NOSAMPLER(_CloudNoise2);
UNITY_DECLARE_TEX2D_NOSAMPLER(_CloudNoise3);
UNITY_DECLARE_TEX2D_NOSAMPLER(_CloudNoise4);
//UNITY_DECLARE_TEX2D_NOSAMPLER(_CloudNoiseMask1);
//UNITY_DECLARE_TEX2D_NOSAMPLER(_CloudNoiseMask2);
//UNITY_DECLARE_TEX2D_NOSAMPLER(_CloudNoiseMask3);
//UNITY_DECLARE_TEX2D_NOSAMPLER(_CloudNoiseMask4);
uniform float4 _CloudNoiseScale[4];
uniform float4 _CloudNoiseMultiplier[4];
uniform float _CloudNoiseRotation[8]; // first 4 cos, second 4 sin
uniform float3 _CloudNoiseVelocity[4];
//uniform float _CloudNoiseMaskScale[4];
//uniform float2 _CloudNoiseMaskOffset[4];
//uniform float3 _CloudNoiseMaskVelocity[4];
//uniform float _CloudNoiseMaskRotation[8]; // first 4 cos, second 4 sin
uniform fixed4 _CloudColor[4];
uniform fixed4 _CloudEmissionColor[4];
uniform fixed _CloudAmbientMultiplier[4];
uniform fixed _CloudScatterMultiplier[4];
uniform float _CloudCover[4];
uniform float _CloudDensity[4];
uniform float _CloudHeight[4];
uniform float _CloudLightAbsorption[4];
uniform float _CloudHorizonFadeMultiplier[4];
uniform float _CloudSharpness[4];
uniform float _CloudShadowThreshold[4];
uniform float _CloudShadowPower[4];
uniform float4 _CloudHenyeyGreensteinPhase[4];
uniform float _CloudShadowMultiplier;
uniform float _CloudRayOffset[4]; // brings clouds down at the horizon at the cost of stretching them over the top
uniform float _WeatherMakerCloudDitherLevel;
uniform float3 _WeatherMakerCloudCameraPosition;

static const float3 weatherMakerCloudCameraPosition = _WeatherMakerCloudCameraPosition;

// current layer
int _CloudIndex = 0;

static const fixed3 ambientFlatCloudColor = (_WeatherMakerAmbientLightColorGround.rgb + _WeatherMakerAmbientLightColorSky.rgb);

inline float CloudVolumetricHenyeyGreenstein(float lightDotEye, float lightIntensity, float4 phase)
{
	// f(x) = (1 - g)^2 / (4PI * (1 + g^2 - 2g*cos(x))^[3/2])
	// _CloudHenyeyGreensteinPhase.x = forward, _CloudHenyeyGreensteinPhase.y = back
	float g = phase.x;
	float gSquared = g * g;
	float oneMinusGSquared = (1.0 - gSquared);
	float onePlusGSquared = 1.0 + gSquared;
	float twoGSquared = 2.0 * g;
	float falloff = onePlusGSquared - (twoGSquared * lightDotEye);
	float forward = (oneMinusGSquared / (pow(falloff, 1.5)));

	float g2 = phase.y;
	float gSquared2 = g2 * g2;
	float oneMinusGSquared2 = (1.0 - gSquared2);
	float onePlusGSquared2 = 1.0 + gSquared2;
	float twoGSquared2 = 2.0 * g2;
	float falloff2 = onePlusGSquared2 - (twoGSquared2 * lightDotEye);
	float back = (oneMinusGSquared2 / (pow(falloff2, 1.5)));

	return ((forward * phase.z) + (back * phase.w));

	/*
	float g = _CloudHenyeyGreensteinPhase.x;
	float g2 = g * g;
	float h = _CloudHenyeyGreensteinPhase.z * ((1.0f - g2) / pow((1.0f + g2 - 2.0f * g * lightDotEye), 1.5f));
	g = _CloudHenyeyGreensteinPhase.y;
	g2 = g * g;
	h += (_CloudHenyeyGreensteinPhase.w * ((1.0f - g2) / pow((1.0f + g2 - 2.0f * g * lightDotEye), 1.5f)));
	return h;
	*/
}

// returns world pos of cloud plane intersect
float3 CloudRaycastWorldPosPlane(float3 ray, float depth)
{
	float3 planePos = float3(0, _CloudHeight[_CloudIndex], 0);
	float distanceToPlane;
	float planeMultiplier = RayPlaneIntersect(weatherMakerCloudCameraPosition, ray, float3(0.0, 1.0, 0.0), planePos, distanceToPlane);
	planeMultiplier *= (depth > distanceToPlane);
	float3 intersectPos = weatherMakerCloudCameraPosition + (ray * distanceToPlane);
	return planeMultiplier * intersectPos;
}

inline float CalculateNoiseXZ(texture2D<float4> noiseTex, float3 worldPos, float scale, float2 offset, float2 velocity, float multiplier, float adder)
{
	float2 noiseUV = float2(worldPos.x * scale, worldPos.z * scale);
	noiseUV += offset + velocity;
	return (UNITY_SAMPLE_TEX2D_SAMPLER(noiseTex, _CloudNoise1, noiseUV).a + adder) * multiplier;
}

inline float ComputeCloudFBMInner(float3 rayDir, float3 worldPos, Texture2D<float4> noiseTex)
{
	//float3 noisePos = (worldPos * _CloudNoiseScale[_CloudIndex].x) + _CloudNoiseVelocity[_CloudIndex];
	//return ((tex2Dlod(noiseTex, float4(noisePos.xz, 0.0, 0.0)).a));

	float fbm = 0.0;
	float hasY = (float)(_CloudNoiseScale[_CloudIndex].y > 0.0 && _CloudNoiseMultiplier[_CloudIndex].y > 0.0);
	float hasZ = (float)(_CloudNoiseScale[_CloudIndex].z > 0.0 && _CloudNoiseMultiplier[_CloudIndex].z > 0.0);
	float hasW = (float)(_CloudNoiseScale[_CloudIndex].w > 0.0 && _CloudNoiseMultiplier[_CloudIndex].w > 0.0);
	float3 noisePos;

	//float sampleCount = _CloudSampleCount[_CloudIndex];
	//float3 step = rayDir * _CloudSampleStepMultiplier[_CloudIndex].x;
	//float3 step2 = rayDir * _CloudSampleStepMultiplier[_CloudIndex].y;
	//float3 step3 = rayDir * _CloudSampleStepMultiplier[_CloudIndex].z;
	//float3 step4 = rayDir * _CloudSampleStepMultiplier[_CloudIndex].w;
	//float i = 0.0;
	//float maxFbm = sampleCount * 0.2;

	//// UNITY_LOOP
	//for (; i < sampleCount && fbm < maxFbm; i++)
	//{
		noisePos = ((worldPos/* + (step * i)*/) * _CloudNoiseScale[_CloudIndex].x) + _CloudNoiseVelocity[_CloudIndex];
		fbm += ((UNITY_SAMPLE_TEX2D_SAMPLER(noiseTex, _CloudNoise1, float2(RotateUV(noisePos.xz, _CloudNoiseRotation[_CloudIndex + 4], _CloudNoiseRotation[_CloudIndex]))).a) * _CloudNoiseMultiplier[_CloudIndex].x);
		UNITY_BRANCH
		if (hasY)
		{
			noisePos = ((worldPos/* + (step2 * i)*/) * _CloudNoiseScale[_CloudIndex].y) + _CloudNoiseVelocity[_CloudIndex];
			fbm += ((UNITY_SAMPLE_TEX2D_SAMPLER(noiseTex, _CloudNoise1, float2(RotateUV(noisePos.xz, _CloudNoiseRotation[_CloudIndex + 4], _CloudNoiseRotation[_CloudIndex]))).a) * _CloudNoiseMultiplier[_CloudIndex].y);
		}
		UNITY_BRANCH
		if (hasZ)
		{
			noisePos = ((worldPos/* + (step3 * i)*/) * _CloudNoiseScale[_CloudIndex].z) + _CloudNoiseVelocity[_CloudIndex];
			fbm += ((UNITY_SAMPLE_TEX2D_SAMPLER(noiseTex, _CloudNoise1, float2(RotateUV(noisePos.xz, _CloudNoiseRotation[_CloudIndex + 4], _CloudNoiseRotation[_CloudIndex]))).a) * _CloudNoiseMultiplier[_CloudIndex].z);
		}
		UNITY_BRANCH
		if (hasW)
		{
			noisePos = ((worldPos/* + (step4 * i)*/) * _CloudNoiseScale[_CloudIndex].w) + _CloudNoiseVelocity[_CloudIndex];
			fbm += ((UNITY_SAMPLE_TEX2D_SAMPLER(noiseTex, _CloudNoise1, float2(RotateUV(noisePos.xz, _CloudNoiseRotation[_CloudIndex + 4], _CloudNoiseRotation[_CloudIndex]))).a) * _CloudNoiseMultiplier[_CloudIndex].w);
		}
	//}

	return fbm;

}

float ComputeCloudFBMOutter(float3 rayDir, float3 worldPos, Texture2D<float4> noiseTex/*, Texture2D<float4> maskTex*/)
{
	// calculate cloud values
	float sharpness = _CloudSharpness[_CloudIndex];
	float cover = _CloudCover[_CloudIndex];
	float fbm = ComputeCloudFBMInner(rayDir, worldPos, noiseTex);
	//fbm = saturate(sharpness > 0.0 ? (1.0 - (pow(_CloudSharpness[_CloudIndex], fbm - (1.0 - pow(cover, 0.5))))) : (fbm * cover));
	if (sharpness > 0.0)
	{
		fbm = min(1.0, (1.0 - (pow(sharpness, (1.5 * fbm) - (1.0 - cover)))));
	}
	else
	{
		fbm = fbm * cover;
	}

	/*
	UNITY_BRANCH
	if (_CloudNoiseMaskScale[_CloudIndex] > 0.0)
	{
		float2 maskRotated = RotateUV(worldPos.xz, _CloudNoiseMaskRotation[_CloudIndex + 4], _CloudNoiseMaskRotation[_CloudIndex]);
		float maskNoise = CalculateNoiseXZ(maskTex, float3(maskRotated.x, 0.0, maskRotated.y), _CloudNoiseMaskScale[_CloudIndex], _CloudNoiseMaskOffset[_CloudIndex], _CloudNoiseMaskVelocity[_CloudIndex], 1.0, 0.0);
		fbm *= maskNoise;
	}
	*/

	return fbm;
}

fixed3 ComputeDirectionalLightCloud(float3 rayDir, float fbm, float scatterMultiplier)
{
	fixed3 finalColor = fixed3Zero;

	// take advantage of the fact that dir lights are sorted by perspective/ortho and then by intensity
	UNITY_LOOP
	for (uint i = 0; i < uint(_WeatherMakerDirLightCount) && _WeatherMakerDirLightVar1[i].y == 0.0 && _WeatherMakerDirLightColor[i].a > 0.001; i++)
	{
		// direct light
		float powerX = _WeatherMakerDirLightPower[i].x;
		float powerY = _WeatherMakerDirLightPower[i].y;
		float3 lightDir = _WeatherMakerDirLightPosition[i].xyz;
		float lightDot = max(0.0, dot(lightDir, rayDir));
		float4 lightColor = _WeatherMakerDirLightColor[i];
		lightDot = pow(lightDot, powerX);

		// ensure a min amount of scatter
		float lightMultiplier = (lightDot * scatterMultiplier);

		// indirect light
		float indirectLight = (lightColor.a * lightColor.a);
		indirectLight /= pow(fbm, 0.5);

		finalColor += ((lightColor.rgb * indirectLight) + (lightColor.rgb * lightMultiplier * lightMultiplier * powerY));
	}

	return finalColor;
}

fixed3 ComputeCloudLighting(fixed3 cloudColor, float fbm, float3 rayDir, float3 worldPos, fixed alphaAccum)
{
	float cloudDensity = min(1.0, _CloudDensity[_CloudIndex] * 6.0);
	float invCloudDensity = min(10.0, 1.0 / max(0.0001, cloudDensity));
	float invFbm = 1.0 - fbm;
	float dirLightReducer = max(0.0, 1.0 - alphaAccum);
	float scatterMultiplier = invCloudDensity * (1.0 - alphaAccum) * _CloudScatterMultiplier[_CloudIndex];
	fixed3 cloudDirLight = ComputeDirectionalLightCloud(rayDir, fbm, scatterMultiplier);

	// reduce directional lights by previous density (higher layers)
	cloudDirLight *= dirLightReducer;

	// reduce directional light by cloud light absorption factor
	float dirLightDensityFactor = min(1.0, invFbm * _CloudLightAbsorption[_CloudIndex] * 10.0);

	// additional lights, probably under or inside the clouds, so reduce as the particle density decreases by fbm multiply
	wm_world_space_light_params p;
	p.worldPos = worldPos;
	p.diffuseColor = cloudColor;
	p.ambientColor = ambientFlatCloudColor * _CloudAmbientMultiplier[_CloudIndex];
	p.shadowStrength = -1.0;
	fixed3 litColor = (fbm * CalculateLightColorWorldSpace(p));
	litColor += (cloudDirLight * dirLightDensityFactor);
	return cloudColor * litColor;
}

fixed4 ComputeCloudColor(float3 rayDir, float depth, Texture2D<float4> noiseTex,/* Texture2D<float4> maskTex,*/ float4 screenUV, inout fixed alphaAccum)
{
	float3 worldPos = CloudRaycastWorldPosPlane(rayDir, depth);

	// miss, exit out
	UNITY_BRANCH
	if (worldPos.y < 1.0)
	{
		return fixed4Zero;
	}
	else
	{
		float fbm = ComputeCloudFBMOutter(rayDir, worldPos, noiseTex/*, maskTex*/);

		// fast out for transparent area, avoid lots of unnecessary calculations
		UNITY_BRANCH
		if (fbm < 0.005)
		{
			return fixed4(0.0, 0.0, 0.0, 0.004);
		}
		else
		{
			// compute lighting
			fixed3 cloudColor = _CloudColor[_CloudIndex];
			cloudColor = ComputeCloudLighting(cloudColor, fbm, rayDir, worldPos, alphaAccum);

			// calculate alpha for the particle
			fixed alpha = min(1.0, (fbm * 3.0));
			alpha = pow(alpha, 1.0 - alpha);
			alpha *= lerp(1.0, clamp(rayDir.y * 4.0, 0.5, 1.0), _CloudHorizonFadeMultiplier[_CloudIndex]);
			alpha *= alpha;
			alpha = max(alpha, 0.004);

			//alpha = min(1.0, alpha * fbm * fbm * 3.5);

			// calculate directional light reduction for future layers
			alphaAccum = min(1.0, alphaAccum + (alpha * _CloudDensity[_CloudIndex] * _CloudDensity[_CloudIndex]));

			return fixed4(cloudColor + (_CloudEmissionColor[_CloudIndex].rgb * _CloudEmissionColor[_CloudIndex].a), alpha);
		}
	}
}

#endif // __WEATHER_MAKER_CLOUD_SHADER__
