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

Shader "WeatherMaker/WeatherMakerFullScreenWetnessOverlayShader"
{
	Properties
	{
		_OverlayTexture("Texture", 2D) = "white" {}
		_OverlayIntensity("Overlay Intensity", Float) = 0.0
		_OverlayReflectionIntensity("Overlay Reflection Intensity", Float) = 0.0
		_OverlayNormalReducer("Overlay Normal Power", Float) = 0.5
		_OverlayScale("Overlay Scale", Float) = 0.005
		_OverlayOffset("Overlay Offset", Vector) = (0.0, 0.0, 0.0, 0.0)
		_OverlayVelocity("Overlay Velocity", Vector) = (0.0, 0.0, 0.0, 0.0)
		_OverlayColor("Overlay Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_OverlaySpecularColor("Overlay Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_OverlaySpecularIntensity("Overlay Specular Intensity", Float) = 4.0
		_OverlaySpecularPower("Overlay Specular Power", Float) = 4.0
		_OverlayNoiseTexture("Noise Texture", 2D) = "white" {}
		_OverlayNoiseMultiplier("Noise Multiplier", Float) = 1.0
		_OverlayNoisePower("Noise Power", Float) = 1.0
		_OverlayNoiseAdder("Noise Adder", Float) = 0.0
		_OverlayNoiseScale("Noise Scale", Float) = 0.005
		_OverlayNoiseOffset("Noise Offset", Vector) = (0.0, 0.0, 0.0, 0.0)
		_OverlayNoiseVelocity("Noise Velocity", Vector) = (0.0, 0.0, 0.0, 0.0)
		_OverlayMinHeight("Min height", Float) = 0.0
		_OverlayMinHeigtNoiseMultiplier("Min height noise multiplier", Float) = 1.0
		_OverlayMinHeightNoiseScale("Min height noise scale", Float) = 0.04
		_OverlayMinHeightNoiseOffset("Min height noise offset", Vector) = (0.0, 0.0, 0.0, 0.0)
		_OverlayMinHeightNoiseVelocity("Min height noise velocity", Vector) = (0.0, 0.0, 0.0, 0.0)
		_DirectionalLightMultiplier("Directional light multiplier", Float) = 1.0
		_PointSpotLightMultiplier("Point/spot light multiplier", Float) = 1.0
		_AmbientLightMultiplier("Ambient light multiplier", Float) = 0.15
	}
	SubShader
	{
		Cull Off Lighting Off ZWrite Off ZTest [_ZTest] Fog{ Mode Off }
		Blend [_SrcBlendMode][_DstBlendMode]

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
			#pragma vertex full_screen_vertex_shader_refl
			#pragma fragment full_screen_overlay_fragment_shader
			#pragma multi_compile_instancing
			#pragma multi_compile __ WEATHER_MAKER_DEFERRED_SHADING

			#define WEATHER_MAKER_IS_FULL_SCREEN_EFFECT
			#define WEATHER_MAKER_DISABLE_AREA_LIGHTS 1
			#if !defined(UNITY_NO_SCREENSPACE_SHADOWS)
			#define WEATHER_MAKER_SHADOWS_SCREEN
			#endif
			#define NULL_ZONE_RENDER_MASK 4 // overlay is 4

			inline fixed4 wetnessOverlayComputeReflection(fixed4 reflectionColor, fixed4 overlayColor)
			{
				overlayColor.rgb *= (reflectionColor.rgb * reflectionColor.a);
				return overlayColor;
			}

			#define WEATHER_MAKER_OVERLAY_ENABLE_REFLECTION wetnessOverlayComputeReflection


			#include "WeatherMakerWaterShaderInclude.cginc"
			#include "WeatherMakerOverlayShaderInclude.cginc"

			fixed4 full_screen_overlay_fragment_shader(full_screen_fragment_reflection i) : SV_Target
			{
				WM_INSTANCE_FRAG(i);
				float3 rayDir = i.forwardLine;
				float3 worldPos, normal;
				float depth;
				
				UNITY_BRANCH
				if (GetOverlayInputs(i.uv.xy, rayDir, rayDir, worldPos, normal, depth))
				{
					return ComputeOverlayColor(worldPos, rayDir, normal, depth, i.uv.xy);
				}
				else
				{
					return fixed4Zero;
				}
			}

			ENDCG
		}
	}
	Fallback Off
}
