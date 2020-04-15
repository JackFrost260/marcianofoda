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

Shader "WeatherMaker/WeatherMakerCloudShadowShader"
{
	Properties
	{
		[Header(Shadow)]
		_CloudShadowMapAdder("Adder", Range(-1.0, 1.0)) = -0.4
		_CloudShadowMapMultiplier("Multiplier", Range(0.01, 10.0)) = 4.0
		_CloudShadowMapPower("Power", Range(0.0, 256.0)) = 1.0
		_CloudShadowDetails("Shadow Details", Int) = 1
		_WeatherMakerCloudVolumetricShadowDither("Dither", Range(0.0, 1.0)) = 0.05
		[NoScaleOffset] _WeatherMakerCloudShadowDetailTexture("Detail Texture", 2D) = "white" {}
		_WeatherMakerCloudShadowDetailScale("Detail Scale", Range(0.0, 1.0)) = 0.0001
		_WeatherMakerCloudShadowDetailIntensity("Detail Intensity", Range(0.0, 10.0)) = 1.0
		_WeatherMakerCloudShadowDetailFalloff("Detail Falloff", Range(0.0, 1.0)) = 0.0
		_WeatherMakerCloudShadowDistanceFade("Distance Fade", Range(0.0, 4.0)) = 1.75
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always BlendOp [_BlendOp]

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl_no_auto_normalization
		#pragma multi_compile_instancing
		#pragma vertex full_screen_vertex_shader

		#define WEATHER_MAKER_ENABLE_TEXTURE_DEFINES
		#define WEATHER_MAKER_IS_FULL_SCREEN_EFFECT

		#include "WeatherMakerCloudVolumetricShaderInclude.cginc"

		struct v2fCloudShadow
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 rayDir : TEXCOORD1;
		};

		uniform int _CloudShadowDetails;

		ENDCG
		
		Pass
		{
			CGPROGRAM

			#pragma fragment shadowFrag

			float4 shadowFrag(wm_full_screen_fragment i) : SV_Target
			{
				// screen shadows
				WM_INSTANCE_FRAG(i);

				float depth = GetDepth01(i.uv);
				UNITY_BRANCH
				if (depth < 1.0)
				{
					float3 worldPos = weatherMakerCloudCameraPosition + (depth * i.forwardLine);
					float existingShadow = WM_SAMPLE_FULL_SCREEN_TEXTURE(_MainTex5, i.uv.xy).r;
					return ComputeCloudShadowStrength(worldPos, 0, existingShadow, _CloudShadowDetails);
				}
				else
				{
					return 1.0;
				}
			}

			ENDCG
		}

		/*
		Pass
		{
			CGPROGRAM

			#pragma fragment shadowFrag

			float shadowFrag(wm_full_screen_fragment i) : SV_Depth
			{
				// TODO: Does not work at all
				WM_INSTANCE_FRAG(i);
				float3 worldPos = wm_getWorldPosFromShadowUV(i.uv.xy);
				float shadowStrength = ComputeCloudShadowStrength(worldPos, 0, _CloudShadowDetails);
				return ceil(max(0.0, shadowStrength - 0.2));
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma fragment shadowFrag

			float4 shadowFrag(wm_full_screen_fragment i) : SV_Target
			{
				// clone shadow texture
				WM_INSTANCE_FRAG(i);
				return min(weatherMakerGlobalShadow, WM_SAMPLE_FULL_SCREEN_TEXTURE(_MainTex, i.uv.xy).r);
			}

			ENDCG
		}
		*/
	}

	Fallback Off
}