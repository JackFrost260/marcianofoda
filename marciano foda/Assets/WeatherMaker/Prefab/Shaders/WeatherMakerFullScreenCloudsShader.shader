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

// http://bitsquid.blogspot.com/2016/07/volumetric-clouds.html
// https://github.com/greje656/clouds
// http://patapom.com/topics/Revision2013/Revision%202013%20-%20Real-time%20Volumetric%20Rendering%20Course%20Notes.pdf

Shader "WeatherMaker/WeatherMakerFullScreenCloudsShader"
{
	Properties
	{
		_PointSpotLightMultiplier("Point/Spot Light Multiplier", Range(0, 10)) = 1
		_DirectionalLightMultiplier("Directional Light Multiplier", Range(0, 10)) = 1
		_AmbientLightMultiplier("Ambient light multiplier", Range(0, 4)) = 1
	}
	SubShader
	{
		Cull Off Lighting Off ZWrite Off ZTest Always Fog { Mode Off }
		Blend [_SrcBlendMode][_DstBlendMode]

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9
		

		#define WEATHER_MAKER_DEPTH_SHADOWS_OFF
		#define WEATHER_MAKER_LIGHT_NO_DIR_LIGHT
		#define WEATHER_MAKER_LIGHT_NO_NORMALS
		#define WEATHER_MAKER_LIGHT_NO_SPECULAR
		#define WEATHER_MAKER_IS_FULL_SCREEN_EFFECT

		// WARNING - THIS WILL BE VERY PERFORMANCE INTENSIVE AND CAN LOCK UP THE EDITOR
		//#define VOLUMETRIC_CLOUD_REAL_TIME_NOISE

		#include "WeatherMakerCloudVolumetricShaderInclude.cginc"

		inline void GetDepthAndRay(float4 uv, inout float3 rayDir, float3 forwardLine, out float depth, out float depth01)
		{
			rayDir = GetFullScreenRayDir(rayDir);
			depth01 = WM_SAMPLE_DEPTH_DOWNSAMPLED_01(uv.xy);
			//depth01 = WM_SAMPLE_DEPTH_AREA_DOWNSAMPLED_TEMPORAL_REPROJECTION_01(uv.xy);

#if defined(WEATHER_MAKER_ENABLE_VOLUMETRIC_CLOUDS)

			if (depth01 >= 1.0)
			{
				// if depth is max value, make an "infinite" depth
				depth = lerp(_CloudPlanetRadiusVolumetric, 1000000.0, _CloudPlanetRadiusVolumetric <= 0.0);
			}
			else
			
#endif

			if (WM_CAMERA_RENDER_MODE_CUBEMAP)
			{
				depth = length(rayDir * depth01 * _ProjectionParams.z);
			}
			else
			{
				depth = length(depth01 * forwardLine);
			}
		}

		ENDCG

		// color pass
		Pass
		{
			CGPROGRAM

			#pragma vertex full_screen_vertex_shader
			#pragma fragment temporal_reprojection_fragment_custom
			#pragma multi_compile_instancing

			#define WEATHER_MAKER_TEMPORAL_REPROJECTION_FRAGMENT_TYPE full_screen_fragment
			#define WEATHER_MAKER_TEMPORAL_REPROJECTION_FRAGMENT_FUNC full_screen_clouds_frag_impl
			#define WEATHER_MAKER_TEMPORAL_REPROJECTION_BLEND_FUNC blendCloudTemporal

			//#define WEATHER_MAKER_TEMPORAL_REPROJECTION_OFF_SCREEN_FUNC offScreenCloudTemporal

			// comment out to disable neighborhood clamping, generally leaving this on is much better than off but can cause flickering pixels
			#define WEATHER_MAKER_TEMPORAL_REPROJECTION_NEIGHBORHOOD_CLAMPING

			// leave commented out unless testing performance, red areas are full shader runs, try to minimize these
			//#define WEATHER_MAKER_TEMPORAL_REPROJECTION_SHOW_OVERDRAW fixed4(1,0,0,1)

			inline fixed4 blendCloudTemporal(fixed4 prev, fixed4 cur, fixed4 diff, float4 uv, full_screen_fragment i);
			inline fixed4 offScreenCloudTemporal(fixed4 prev, fixed4 cur, float4 uv, full_screen_fragment i);
			fixed4 full_screen_clouds_frag_impl(full_screen_fragment i) : SV_Target;

			#include "WeatherMakerTemporalReprojectionShaderInclude.cginc"

			inline fixed4 blendCloudTemporal(fixed4 prev, fixed4 cur, fixed4 diff, float4 uv, full_screen_fragment i)
			{
				UNITY_BRANCH
				if (prev.a < 0.003)
				{
					prev = cur;
				}

#if defined(WEATHER_MAKER_TEMPORAL_REPROJECTION_NEIGHBORHOOD_CLAMPING)

				// if dir or point light changes, force update
				else if ((diff.w < 0.1 && diff.x > 0.5) || _WeatherMakerDirLightVar1[0].x > 0.01 || _WeatherMakerPointLightVar1[0].x > 0.01)
				{
					// sample 4 of the nearby temporal pixels with the latest correct results and clamp the pixel color
					float2 uv1 = float2(i.uv.x + temporalReprojectionSubFrameBlurOffsets.x, i.uv.y - temporalReprojectionSubFrameBlurOffsets.w);
					float2 uv2 = float2(i.uv.x - temporalReprojectionSubFrameBlurOffsets.y, i.uv.y - temporalReprojectionSubFrameBlurOffsets.z);
					float2 uv3 = float2(i.uv.x + temporalReprojectionSubFrameBlurOffsets.y, i.uv.y + temporalReprojectionSubFrameBlurOffsets.z);
					float2 uv4 = float2(i.uv.x - temporalReprojectionSubFrameBlurOffsets.x, i.uv.y + temporalReprojectionSubFrameBlurOffsets.w);
					//float2 uv5 = float2(i.uv.x + temporalReprojectionSubFrameBlurOffsets.x, i.uv.y - temporalReprojectionSubFrameBlurOffsets.w);
					//float2 uv6 = float2(i.uv.x - temporalReprojectionSubFrameBlurOffsets.y, i.uv.y - temporalReprojectionSubFrameBlurOffsets.z);
					//float2 uv7 = float2(i.uv.x + temporalReprojectionSubFrameBlurOffsets.y, i.uv.y + temporalReprojectionSubFrameBlurOffsets.z);
					//float2 uv8 = float2(i.uv.x - temporalReprojectionSubFrameBlurOffsets.x, i.uv.y + temporalReprojectionSubFrameBlurOffsets.w);
					fixed4 col2 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv1);
					fixed4 col3 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv2);
					fixed4 col4 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv3);
					fixed4 col5 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv4);
					//fixed4 col6 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv5);
					//fixed4 col7 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv6);
					//fixed4 col8 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv7);
					//fixed4 col9 = WM_SAMPLE_FULL_SCREEN_TEXTURE(_TemporalReprojection_SubFrame, uv8);
					//fixed4 minCol = min(cur, min(col2, min(col3, min(col4, min(col5, min(col6, min(col7, min(col8, col9))))))));
					//fixed4 maxCol = max(cur, max(col2, max(col3, max(col4, max(col5, max(col6, max(col7, max(col8, col9))))))));
					fixed4 minCol = min(cur, min(col2, min(col3, min(col4, col5))));
					fixed4 maxCol = max(cur, max(col2, max(col3, max(col4, col5))));
					prev = clamp(prev, minCol, maxCol);
				}

#endif

				return prev;
			}

			inline fixed4 offScreenCloudTemporal(fixed4 prev, fixed4 cur, float4 uv, full_screen_fragment i)
			{
				return cur;
			}

			fixed4 full_screen_clouds_frag_impl(full_screen_fragment i) : SV_Target
			{
				float depth, depth01;
				GetDepthAndRay(i.uv, i.rayDir, i.forwardLine, depth, depth01);
				float3 cloudRay = i.rayDir;
				float3 worldPos;
				fixed4 finalColor = fixed4Zero;
				fixed4 cloudColor;
				fixed alphaAccum = 0.0;

				// top layer
				UNITY_BRANCH
				if (_CloudCover[3] > 0.0)
				{
					_CloudIndex = 3;
					cloudColor = ComputeCloudColor(normalize(float3(cloudRay.x, cloudRay.y + _CloudRayOffset[3], cloudRay.z)), depth, _CloudNoise4, /*_CloudNoiseMask4,*/ i.uv, alphaAccum);
					blendClouds(cloudColor, finalColor);
				}

				// next layer
				UNITY_BRANCH
				if (_CloudCover[2] > 0.0)
				{
					_CloudIndex = 2;
					cloudColor = ComputeCloudColor(normalize(float3(cloudRay.x, cloudRay.y + _CloudRayOffset[2], cloudRay.z)), depth, _CloudNoise3, /*_CloudNoiseMask3,*/ i.uv, alphaAccum);
					blendClouds(cloudColor, finalColor);
				}

				// next layer
				UNITY_BRANCH
				if (_CloudCover[1] > 0.0)
				{
					_CloudIndex = 1;
					cloudColor = ComputeCloudColor(normalize(float3(cloudRay.x, cloudRay.y + _CloudRayOffset[1], cloudRay.z)), depth, _CloudNoise2, /*_CloudNoiseMask2,*/ i.uv, alphaAccum);
					blendClouds(cloudColor, finalColor);
				}

				// bottom layer
				UNITY_BRANCH
				if (_CloudCover[0] > 0.0)
				{
					_CloudIndex = 0;
					cloudColor = ComputeCloudColor(normalize(float3(cloudRay.x, cloudRay.y + _CloudRayOffset[0], cloudRay.z)), depth, _CloudNoise1, /*_CloudNoiseMask1,*/ i.uv, alphaAccum);
					blendClouds(cloudColor, finalColor);
				}

				// pre-multiply flat layer clouds
				finalColor *= finalColor.a;

#if defined(WEATHER_MAKER_ENABLE_VOLUMETRIC_CLOUDS)

				// volumetric layer
				UNITY_BRANCH
				if (_CloudCoverVolumetric > 0.0)
				{
					// volumetric cloud already correct rgb based on alpha
					cloudColor = ComputeCloudColorVolumetric(cloudRay, i.uv, depth);
					cloudColor *= cloudColor.a;

					// custom blend
					finalColor = cloudColor + (finalColor * (1.0 - cloudColor.a));
				}

				UNITY_BRANCH
				if (finalColor.a < 0.999 && weatherMakerNightMultiplierSquared > 0.001)
				{
					// blend aurora borealis, the aurora is already pre-multiplied by alpha
					fixed4 auroraColor = ComputeAurora(_WorldSpaceCameraPos, cloudRay, i.uv, depth) * weatherMakerNightMultiplierSquared;
					finalColor.rgb = (auroraColor.rgb * (1.0 - finalColor.a)) + finalColor.rgb;
					finalColor.a = max(auroraColor.a, finalColor.a);
				}

#endif

				return finalColor;
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

			float4 frag(full_screen_fragment i) : SV_Target
			{ 
				WM_INSTANCE_FRAG(i);

#if defined(WEATHER_MAKER_ENABLE_VOLUMETRIC_CLOUDS)

				UNITY_BRANCH
				if (_CloudCoverVolumetric > 0.0)
				{
					UNITY_BRANCH
					if (unity_OrthoParams.w == 0.0)
					{
						// get the 0-1 depth of the cloud layer start
						float depth, depth01;
						// don't use ray offset, we want the exact depth buffer value
						GetDepthAndRay(i.uv, i.rayDir, i.forwardLine, depth, depth01);
						float3 startPos, startPos2;
						float rayLength, rayLength2;
						float distanceToSphere, distanceToSphere2;
						uint iterations = SetupCloudRaymarch(weatherMakerCloudCameraPosition, i.rayDir, depth, depth, volumetricSphereInner, volumetricSphereOutter,
							startPos, rayLength, distanceToSphere, startPos2, rayLength2, distanceToSphere2);

						// return 1.0 / ((_ZBufferParams.x * z) + _ZBufferParams.y);
						// TODO: The left of this lerp is incorrect far above the clouds, figure out why...
						//float cloudLayerDepth01 = GetDepth01FromWorldSpaceRay(i.rayDir, distanceToSphere);
						float depthPos = length(depth01 * i.forwardLine);
						float cloudLayerDepth01 = saturate(lerp(0.0, depth01, distanceToSphere / depthPos));
						return lerp(cloudLayerDepth01, 1.0, (iterations == 0 || rayLength < 0.1));
					}
					else
					{
						// orthographic not supported
						return 1.0;
					}
				}
				else

#endif

				{
					return 1.0;
				}
			}

			ENDCG
		}
		
		// cloud ray render pass
		Pass
		{
			Blend One Zero

			CGPROGRAM

			#pragma vertex full_screen_vertex_shader
			#pragma fragment frag
			#pragma multi_compile_instancing

			fixed4 frag(full_screen_fragment i) : SV_Target
			{ 
				WM_INSTANCE_FRAG(i);

#if defined(WEATHER_MAKER_ENABLE_VOLUMETRIC_CLOUDS) && VOLUMETRIC_CLOUD_RENDER_MODE == 1
				
				fixed3 shaftColor = fixed3Zero;

				// take advantage of the fact that dir lights are sorted by perspective/ortho and then by intensity
				UNITY_LOOP
				for (uint lightIndex = 0; lightIndex < uint(_WeatherMakerDirLightCount) && _WeatherMakerDirLightVar1[lightIndex].y == 0.0 && _WeatherMakerDirLightColor[lightIndex].a > 0.001; lightIndex++)
				{
					shaftColor += ComputeDirLightShaftColor(i.uv.xy, 0.01, _WeatherMakerDirLightViewportPosition[lightIndex], _WeatherMakerDirLightColor[lightIndex].rgb * _WeatherMakerDirLightVar1[lightIndex].z);
				}
				return fixed4(shaftColor, 0.0);

#else

				return fixed4Zero;

#endif

			}

			ENDCG
		}

		// cloud ray blit pass
		Pass
		{
			Blend One One

			CGPROGRAM

			#pragma vertex full_screen_vertex_shader
			#pragma fragment frag
			#pragma multi_compile_instancing

			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex4);

			fixed4 frag(full_screen_fragment i) : SV_Target
			{ 
				WM_INSTANCE_FRAG(i);

				return WM_SAMPLE_FULL_SCREEN_TEXTURE(_MainTex4, i.uv);
			}

			ENDCG
		}
	}
}
