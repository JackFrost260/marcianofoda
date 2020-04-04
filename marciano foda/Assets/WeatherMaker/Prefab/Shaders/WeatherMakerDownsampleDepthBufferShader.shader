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

Shader "WeatherMaker/WeatherMakerDownsampleDepthBufferShader"
{
	Properties
	{
	}
	Subshader
	{
		Cull Off Lighting Off ZWrite Off ZTest Always Fog { Mode Off } Blend One Zero

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9

		#define WEATHER_MAKER_IS_FULL_SCREEN_EFFECT
		#define DEPTH_SAMPLE_MODE 0 // 0 = max, 1 = min, 2 = alternate max/min

		#include "WeatherMakerCoreShaderInclude.cginc"

		uniform float _DownsampleDepthScale;

#define TEX_OFF 0.5

		static const float2 texelSizeDepth = _CameraDepthTexture_TexelSize.xy;
		static const float2 tapsDepth[4] =
		{
			//(-texelSizeDepth),
			//(-texelSizeDepth) + float2(texelSizeDepth.x * 2.0, 0.0),
			//(-texelSizeDepth) + float2(0.0, texelSizeDepth.y * 2.0),
			//(-texelSizeDepth) + (texelSizeDepth.x * 2.0)
			float2(-TEX_OFF, -TEX_OFF) * texelSizeDepth,
			float2(-TEX_OFF, TEX_OFF) * texelSizeDepth,
			float2(TEX_OFF, -TEX_OFF) * texelSizeDepth,
			float2(TEX_OFF, TEX_OFF) * texelSizeDepth
		};

		static const float2 texelSizeMain = 0.5 * _CameraDepthTexture_TexelSize.xy * _DownsampleDepthScale;
		static const float2 tapsMain[4] =
		{
			//(-texelSizeMain),
			//(-texelSizeMain) + float2(texelSizeMain.x * 2.0, 0.0),
			//(-texelSizeMain) + float2(0.0, texelSizeMain.y * 2.0),
			//(-texelSizeMain) + (texelSizeMain.x * 2.0)
			float2(-TEX_OFF, -TEX_OFF) * texelSizeMain,
			float2(-TEX_OFF, TEX_OFF) * texelSizeMain,
			float2(TEX_OFF, -TEX_OFF) * texelSizeMain,
			float2(TEX_OFF, TEX_OFF) * texelSizeMain
		};

#define SAMPLE_DEPTH_4(source, i, taps) \
		float depth1 = UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(source, i.uv.xy + taps[0])); \
		float depth2 = UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(source, i.uv.xy + taps[1])); \
		float depth3 = UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(source, i.uv.xy + taps[2])); \
		float depth4 = UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(source, i.uv.xy + taps[3]));

#define SAMPLE_DEPTH_4_01(source, i, taps) \
		float depth1 = WM_LINEAR_DEPTH_01(UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(source, i.uv.xy + taps[0]))); \
		float depth2 = WM_LINEAR_DEPTH_01(UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(source, i.uv.xy + taps[1]))); \
		float depth3 = WM_LINEAR_DEPTH_01(UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(source, i.uv.xy + taps[2]))); \
		float depth4 = WM_LINEAR_DEPTH_01(UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(source, i.uv.xy + taps[3])));

#if DEPTH_SAMPLE_MODE == 0 // max

#define DownsampleDepth(source, i, taps, texelSize) \
		SAMPLE_DEPTH_4(source, i, taps); \
		return (max(depth1, max(depth2, max(depth3, depth4))));

#define DownsampleDepth01(source, i, taps, texelSize) \
		SAMPLE_DEPTH_4_01(source, i, taps); \
		return (max(depth1, max(depth2, max(depth3, depth4))));

#elif DEPTH_SAMPLE_MODE == 1 // min

#define DownsampleDepth(source, i, taps, texelSize) \
		SAMPLE_DEPTH_4(source, i, taps); \
		return (min(depth1, min(depth2, min(depth3, depth4))));

#define DownsampleDepth01(source, i, taps, texelSize) \
		SAMPLE_DEPTH_4_01(source, i, taps); \
		return (min(depth1, min(depth2, min(depth3, depth4))));

#elif DEPTH_SAMPLE_MODE == 2 // checkerboard

#define DownsampleDepth(source, i, taps, texelSize) \
		SAMPLE_DEPTH_4(source, i, taps); \
		float minDepth = min(min(depth1, depth2), min(depth3, depth4)); \
		float maxDepth = max(max(depth1, depth2), max(depth3, depth4)); \
		int2 position = floor(i.uv.xy * texelSize.zw); \
		int index = fmod(position.x + position.y, 2.0); \
		return lerp(maxDepth, minDepth, index);

#define DownsampleDepth01(source, i, taps, texelSize) \
		SAMPLE_DEPTH_4_01(source, i, taps); \
		float minDepth = min(min(depth1, depth2), min(depth3, depth4)); \
		float maxDepth = max(max(depth1, depth2), max(depth3, depth4)); \
		int2 position = floor(i.uv.xy * texelSize.zw); \
		int index = fmod(position.x + position.y, 2.0); \
		return lerp(maxDepth, minDepth, index);

#else

#error Invalid downsample mode

#endif

		float4 frag1(full_screen_fragment_vertex_uv i) : SV_Target
		{
			WM_INSTANCE_FRAG(i);
			return WM_LINEAR_DEPTH_01(UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)));
		}

		float4 frag2(full_screen_fragment_vertex_uv i) : SV_Target
		{
			WM_INSTANCE_FRAG(i);
			DownsampleDepth01(_CameraDepthTexture, i, tapsDepth, _CameraDepthTextureHalf_TexelSize);
		}

		float4 frag3(full_screen_fragment_vertex_uv i) : SV_Target
		{
			WM_INSTANCE_FRAG(i);
			DownsampleDepth(_CameraDepthTextureHalf, i, tapsMain, _CameraDepthTextureQuarter_TexelSize);
		}

		float4 frag4(full_screen_fragment_vertex_uv i) : SV_Target
		{
			WM_INSTANCE_FRAG(i);
			DownsampleDepth(_CameraDepthTextureQuarter, i, tapsMain, _CameraDepthTextureEighth_TexelSize);
		}

		float4 frag5(full_screen_fragment_vertex_uv i) : SV_Target
		{
			WM_INSTANCE_FRAG(i);
			DownsampleDepth(_CameraDepthTextureEighth, i, tapsMain, _CameraDepthTextureSixteenth_TexelSize);
		}

		ENDCG

		Pass // straight up copy
		{
			CGPROGRAM

			#pragma vertex full_screen_vertex_shader_vertex_uv
			#pragma fragment frag1
			#pragma multi_compile_instancing

			ENDCG
		}

		Pass // copy full depth texture to half
		{
			CGPROGRAM

			#pragma vertex full_screen_vertex_shader_vertex_uv
			#pragma fragment frag2
			#pragma multi_compile_instancing

			ENDCG
		}

		Pass // copy half depth texture to quarter
		{
			CGPROGRAM

			#pragma vertex full_screen_vertex_shader_vertex_uv
			#pragma fragment frag3
			#pragma multi_compile_instancing

			ENDCG
		}

		Pass // copy quarter depth texture to eighth
		{
			CGPROGRAM

			#pragma vertex full_screen_vertex_shader_vertex_uv
			#pragma fragment frag4
			#pragma multi_compile_instancing

			ENDCG
		}

		Pass // copy eighth depth texture to sixteenth
		{
			CGPROGRAM

			#pragma vertex full_screen_vertex_shader_vertex_uv
			#pragma fragment frag5
			#pragma multi_compile_instancing

			ENDCG
		}
	}

	Fallback Off
}