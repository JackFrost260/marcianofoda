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

Shader "WeatherMaker/WeatherMakerFullScreenFogShader"
{
	Properties
	{
		[KeywordEnum(None, Constant, Linear, Exponential, ExponetialSquared)] _WeatherMakerFogMode("Fog Mode", Int) = 2
		_WeatherMakerFogColor("Fog Color", Color) = (0,1,1,1)
		_WeatherMakerFogNoiseScale("Fog Noise Scale", Range(0.0, 1.0)) = 0.0005
		_WeatherMakerFogNoiseMultiplier("Fog Noise Multiplier", Range(0.01, 1.0)) = 0.15
		_WeatherMakerFogNoiseVelocity("Fog Noise Velocity", Vector) = (0.01, 0.01, 0, 0)
		_WeatherMakerFogDensity("Fog Density", Range(0.0, 1.0)) = 0.05
		_WeatherMakerFogHeight("Fog Height", Float) = 0
		_WeatherMakerFogFactorMax("Maximum Fog Facto", Range(0.01, 1)) = 1
		_WeatherMakerFogDitherLevel("Fog Dither Level", Range(0, 1)) = 0.005
		_PointSpotLightMultiplier("Point/Spot Light Multiplier", Range(0, 10)) = 1
		_DirectionalLightMultiplier("Directional Light Multiplier", Range(0, 10)) = 1
		_AmbientLightMultiplier("Ambient Light Multiplier", Range(0, 10)) = 2

		[HideInInspector]
		_MainTex("Texture", 2D) = "red" {}
	}
	Category
	{
		Cull Off Lighting Off ZWrite Off ZTest Always Fog { Mode Off }

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9
		

		ENDCG

		SubShader
		{
			Pass
			{
				Blend [_SrcBlendMode][_DstBlendMode]

				CGPROGRAM

				#pragma vertex full_screen_vertex_shader
				#pragma fragment temporal_reprojection_fragment_custom
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma glsl_no_auto_normalization
				#pragma multi_compile_instancing
				#pragma multi_compile WEATHER_MAKER_SHADOWS_ONE_CASCADE WEATHER_MAKER_SHADOWS_SPLIT_SPHERES

				#define WEATHER_MAKER_IS_FULL_SCREEN_EFFECT
				#define NULL_ZONE_RENDER_MASK 2 // fog is 2

				#include "WeatherMakerFogShaderInclude.cginc"

				#define WEATHER_MAKER_TEMPORAL_REPROJECTION_FRAGMENT_TYPE full_screen_fragment
				#define WEATHER_MAKER_TEMPORAL_REPROJECTION_FRAGMENT_FUNC fog_box_full_screen_fragment_shader
				#define WEATHER_MAKER_TEMPORAL_REPROJECTION_BLEND_FUNC blendFogTemporal
				#define WEATHER_MAKER_TEMPORAL_REPROJECTION_OFF_SCREEN_FUNC offScreenFogTemporal
				#define WEATHER_MAKER_TEMPORAL_REPROJECTION_SAMPLE_PREV(uv, samp) WM_SAMPLE_FULL_SCREEN_TEXTURE(samp, uv.xy)

				// comment out to disable neighborhood clamping, generally leaving this on is much better than off
				#define WEATHER_MAKER_TEMPORAL_REPROJECTION_NEIGHBORHOOD_CLAMPING

				// leave commented out unless testing performance, red areas are full shader runs, try to minimize these
				// #define WEATHER_MAKER_TEMPORAL_REPROJECTION_SHOW_OVERDRAW fixed4(1,0,0,1)

				inline fixed4 blendFogTemporal(fixed4 prev, fixed4 cur, fixed4 diff, float4 uv, full_screen_fragment i);
				inline fixed4 offScreenFogTemporal(fixed4 prev, fixed4 cur, float4 uv, full_screen_fragment i);

				#include "WeatherMakerTemporalReprojectionShaderInclude.cginc"

				inline fixed4 blendFogTemporal(fixed4 prev, fixed4 cur, fixed4 diff, float4 uv, full_screen_fragment i)
				{

#if defined(WEATHER_MAKER_TEMPORAL_REPROJECTION_NEIGHBORHOOD_CLAMPING)

					// sample 8 of the nearby temporal pixels with the latest correct results and clamp the pixel color
					float2 uv1 = float2(i.uv.x + temporalReprojectionSubFrameBlurOffsets.x, i.uv.y - temporalReprojectionSubFrameBlurOffsets.w);
					float2 uv2 = float2(i.uv.x - temporalReprojectionSubFrameBlurOffsets.y, i.uv.y - temporalReprojectionSubFrameBlurOffsets.z);
					float2 uv3 = float2(i.uv.x + temporalReprojectionSubFrameBlurOffsets.y, i.uv.y + temporalReprojectionSubFrameBlurOffsets.z);
					float2 uv4 = float2(i.uv.x - temporalReprojectionSubFrameBlurOffsets.x, i.uv.y + temporalReprojectionSubFrameBlurOffsets.w);
					float2 uv5 = float2(i.uv.x + temporalReprojectionSubFrameBlurOffsets2.x, i.uv.y - temporalReprojectionSubFrameBlurOffsets2.w);
					float2 uv6 = float2(i.uv.x - temporalReprojectionSubFrameBlurOffsets2.y, i.uv.y - temporalReprojectionSubFrameBlurOffsets2.z);
					float2 uv7 = float2(i.uv.x + temporalReprojectionSubFrameBlurOffsets2.y, i.uv.y + temporalReprojectionSubFrameBlurOffsets2.z);
					float2 uv8 = float2(i.uv.x - temporalReprojectionSubFrameBlurOffsets2.x, i.uv.y + temporalReprojectionSubFrameBlurOffsets2.w);
					fixed4 col2 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv1);
					fixed4 col3 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv2);
					fixed4 col4 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv3);
					fixed4 col5 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv4);
					fixed4 col6 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv5);
					fixed4 col7 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv6);
					fixed4 col8 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv7);
					fixed4 col9 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv8);

					// we want to dither the neighboorhood clamping - for darker pixels we reduce this effect as it introduces artifacts
					// for brighter pixels, having a wider clamping range reduces flicker
					// varying the clamping range each frame also reduces flicker and introduces some variance which helps smooth things out
					// the parameters chosen here were mostly trial and error to reduce banding and jagged lines
					//fixed minA = min(cur.a, min(col2.a, min(col3.a, min(col4.a, min(col5.a, min(col6.a, min(col7.a, min(col8.a, col9.a))))))));
					fixed maxA = max(cur.a, max(col2.a, max(col3.a, max(col4.a, max(col5.a, max(col6.a, max(col7.a, max(col8.a, col9.a))))))));

					if (diff.w < 0.01)
					{
						// clamp the rgb to a smaller range of pixels to reduce bleeding pixels
						fixed3 minRgb = min(cur.rgb, min(col2.rgb, min(col3.rgb, min(col4.rgb, col5.rgb))));
						fixed3 maxRgb = max(cur.rgb, max(col2.rgb, max(col3.rgb, max(col4.rgb, col5.rgb))));
						prev.rgb = clamp(prev.rgb, minRgb, maxRgb);
					}

					// if alpha is changing enough, recompute full alpha (expensive)
					UNITY_BRANCH
					if (abs(prev.a - maxA) > 0.1)
					{

#if defined(WEATHER_MAKER_TEMPORAL_REPROJECTION_SHOW_OVERDRAW)

						return WEATHER_MAKER_TEMPORAL_REPROJECTION_SHOW_OVERDRAW;

#endif

						prev.a = ComputeFullScreenFogAlphaTemporalReprojection(i);
					}

#endif

					return prev;
				}

				inline fixed4 offScreenFogTemporal(fixed4 prev, fixed4 cur, float4 uv, full_screen_fragment i)
				{

#if defined(WEATHER_MAKER_TEMPORAL_REPROJECTION_SHOW_OVERDRAW)

					return WEATHER_MAKER_TEMPORAL_REPROJECTION_SHOW_OVERDRAW;

#else

					cur.a = ComputeFullScreenFogAlphaTemporalReprojection(i);
					return cur;

#endif

				}

				ENDCG
			}

			// depth write pass (linear 0 - 1)
			Pass
			{
				CGPROGRAM

				#pragma vertex full_screen_vertex_shader
				#pragma fragment frag
				#pragma multi_compile_instancing

				#include "WeatherMakerCoreShaderInclude.cginc"

				float4 frag(full_screen_fragment i) : SV_Target
				{ 
					WM_INSTANCE_FRAG(i);

					return WM_SAMPLE_DEPTH_DOWNSAMPLED_01(i.uv.xy);
				}

				ENDCG
			}

			/*
			// special effect fog pass, implement later
			Pass
			{
				Blend One Zero
				ZWrite Off
				ColorMask 0
			}
			// special effect blit fog pass, implement later
			Pass
			{
				Blend One One
				ZWrite Off
				ColorMask 0
			}
			*/
		}
	}
	Fallback "VertexLit"
}
