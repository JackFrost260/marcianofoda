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
		_CloudShadowMapMinimum("Minimum", Range(0.0, 1.0)) = 0.0
		_CloudShadowMapMaximum("Maximum", Range(0.0, 1.0)) = 0.6
		_CloudShadowMapPower("Power", Range(0.0, 16.0)) = 1.0
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always BlendOp Min

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl_no_auto_normalization
		#pragma multi_compile_instancing
		#pragma vertex full_screen_vertex_shader

		#define WEATHER_MAKER_IS_FULL_SCREEN_EFFECT

		#include "WeatherMakerCloudVolumetricShaderInclude.cginc"

		struct v2fCloudShadow
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 rayDir : TEXCOORD1;
		};

		ENDCG
		
		Pass
		{
			CGPROGRAM

			#pragma fragment shadowFrag

			float4 shadowFrag(full_screen_fragment i) : SV_Target
			{
				// screen shadows
				WM_INSTANCE_FRAG(i);

				float depth = GetDepth01(i.uv);
				UNITY_BRANCH
				if (depth < 1.0)
				{
					float3 worldPos = weatherMakerCloudCameraPosition + (depth * i.forwardLine);
					return ComputeCloudShadowStrength(worldPos, 0);
				}
				else
				{
					return 1.0;
				}
			}

			ENDCG
		}

		Pass
		{
			Cull Off ZWrite On ZTest Less

			CGPROGRAM

			#pragma fragment shadowFrag

			float shadowFrag(full_screen_fragment i) : SV_DEPTH
			{
				// TODO: depth shadows, not working, needs more research
				// convert uv to world space
				WM_INSTANCE_FRAG(i);
				float3 worldPos = weatherMakerCloudCameraPosition + (GetDepth01(i.uv) * i.forwardLine);
				float shadowStrength = 1.0 - ComputeCloudShadowStrength(worldPos, 0);
				return ceil(max(0.0, shadowStrength - 0.2));
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma fragment shadowFrag

			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex3);

			float4 shadowFrag(full_screen_fragment i) : SV_Target
			{
				// clone shadow texture
				WM_INSTANCE_FRAG(i);
				return min(weatherMakerGlobalShadow, WM_SAMPLE_FULL_SCREEN_TEXTURE(_MainTex3, i.uv.xy).r);
			}

			ENDCG
		}
	}

	Fallback Off
}