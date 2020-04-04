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

#ifndef __WEATHER_MAKER_SHADER_CONSTANTS__
#define __WEATHER_MAKER_SHADER_CONSTANTS__

#pragma require 2darray

#include "UnityCG.cginc" 

#if defined(UNITY_COLORSPACE_GAMMA)
#define GAMMA 2
#define COLOR_2_GAMMA(color) color
#define COLOR_2_LINEAR(color) color * color
#define LINEAR_2_OUTPUT(color) sqrt(color)
#else
#define GAMMA 2.2
#define COLOR_2_GAMMA(color) ((unity_ColorSpaceDouble.r>2.0) ? pow(color,1.0/GAMMA) : color)
#define COLOR_2_LINEAR(color) color
#define LINEAR_2_OUTPUT(color) color
#endif

#define float4Zero float4(0.0, 0.0, 0.0, 0.0)
#define float4One float4(1.0, 1.0, 1.0, 1.0)
#define fixed4Zero fixed4(0.0, 0.0, 0.0, 0.0)
#define fixed4One fixed4(1.0, 1.0, 1.0, 1.0)
#define float3Zero float3(0.0, 0.0, 0.0)
#define float3One float3(1.0, 1.0, 1.0)
#define fixed3Zero fixed3(0.0, 0.0, 0.0)
#define fixed3One fixed3(1.0, 1.0, 1.0)
#define ditherMagic fixed4(12.9898, 78.233, 43758.5453, 241325690.2135)
#define ditherMagic2 fixed4(2.34234, 5.123124, 1024.0, 1024.0)
#define upVector float3(0.0, 1.0, 0.0)

#ifndef PI
#define PI 3.14159263
#endif

#ifndef PI2
#define PI2 (PI * 2.0)
#endif

#ifndef PI4
#define PI4 (PI * 4.0)
#endif

#define ANTI_ALIAS(v) abs(frac(v) - 0.5)

#define WM_BASE_VERTEX_INPUT UNITY_VERTEX_INPUT_INSTANCE_ID
#define WM_BASE_VERTEX_TO_FRAG UNITY_VERTEX_INPUT_INSTANCE_ID UNITY_VERTEX_OUTPUT_STEREO

#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED) || defined(STEREO_INSTANCING_ON)

#define WM_STEREO_INSTANCING_ENABLED

#endif

#define WM_INSTANCE_VERT(v, type, o) type o; UNITY_SETUP_INSTANCE_ID(v); UNITY_TRANSFER_INSTANCE_ID(v, o); UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#define WM_INSTANCE_FRAG(i) UNITY_SETUP_INSTANCE_ID(i); UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
#define WM_SAMPLE_DEPTH(uv) UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, (uv).xy))
#define WM_SAMPLE_DEPTH_PROJ(uv) UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(uv)))

#define DO_ALPHA_BLEND(fg, bg) ((fg.rgb * fg.a) + (bg.rgb * (1.0 - fg.a)))

// vertex...
struct vertex_only_input_data
{
	float4 vertex : POSITION;
	WM_BASE_VERTEX_INPUT
};

struct vertex_uv_normal
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
	WM_BASE_VERTEX_INPUT
};

struct full_screen_vertex
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	WM_BASE_VERTEX_INPUT
};

// fragment...
struct volumetric_data
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 projPos : TEXCOORD0;
	float3 rayDir : TEXCOORD1;
	float3 viewPos : TEXCOORD2;
	float3 worldPos : TEXCOORD3;
	WM_BASE_VERTEX_TO_FRAG
};

struct full_screen_fragment_vertex_uv
{
	float4 vertex : SV_POSITION;
	float2 uv : TEXCOORD0;
	WM_BASE_VERTEX_TO_FRAG
};

struct full_screen_fragment
{
	float4 vertex : SV_POSITION;
	float3 rayDir : NORMAL;
	float4 uv : TEXCOORD0; // uv, uv pixel pos
	float3 forwardLine : TEXCOORD1;
	WM_BASE_VERTEX_TO_FRAG
};

struct full_screen_fragment_reflection
{
	float4 vertex : SV_POSITION;
	float3 rayDir : NORMAL;
	float4 uv : TEXCOORD0; // uv, uv pixel pos
	float3 forwardLine : TEXCOORD1;
	float4 reflectionPos : TEXCOORD2;
	float4 screenPos : TEXCOORD3;
	WM_BASE_VERTEX_TO_FRAG
};

struct deferred_fragment
{
	float4 gBuffer0 : SV_Target0;
	float4 gBuffer1 : SV_Target1;
	float4 gBuffer2 : SV_Target2;
	float4 gBuffer3 : SV_Target3;
};

struct frag_out_with_depth
{
	fixed4 color : COLOR;
	float depth : DEPTH;
};

// https://docs.unity3d.com/Manual/SL-SamplerStates.html
// Sampler names recognized as “inline” sampler states (all case insensitive):
// “Point”, “Linear” or “Trilinear” (required) set up texture filtering mode.
// “Clamp”, “Repeat”, “Mirror” or “MirrorOnce”(required) set up texture wrap mode.
// Wrap modes can be specified per - axis(UVW), e.g.“ClampU_RepeatV”.
// “Compare”(optional) set up sampler for depth comparison; use with HLSL SamplerComparisonState type and SampleCmp / SampleCmpLevelZero functions.
SamplerState sampler_point_clamp_sampler;

uniform float4 _WeatherMakerTime;
uniform float4 _WeatherMakerTimeSin;
uniform float4 _WeatherMakerTimeAngle;
uniform float4 _CameraDepthTexture_ST;
uniform float4 _CameraDepthTexture_TexelSize;
//uniform float4 _CameraDepthTextureOne_ST;
//uniform float4 _CameraDepthTextureOne_TexelSize;
uniform float4 _CameraDepthTextureHalf_ST;
uniform float4 _CameraDepthTextureHalf_TexelSize;
uniform float4 _CameraDepthTextureQuarter_ST;
uniform float4 _CameraDepthTextureQuarter_TexelSize;
uniform float4 _CameraDepthTextureEighth_ST;
uniform float4 _CameraDepthTextureEighth_TexelSize;
uniform float4 _CameraDepthTextureSixteenth_ST;
uniform float4 _CameraDepthTextureSixteenth_TexelSize;
uniform float4 _CameraDepthNormalsTexture_ST;
uniform float4 _CameraDepthNormalsTexture_TexelSize;

//uniform sampler2D _WeatherMakerDitherTexture;
//uniform float4 _WeatherMakerDitherTexture_ST;
//uniform float4 _WeatherMakerDitherTexture_TexelSize;

// contains un-normalized direction to frustum corners for left and right eye : bottom left, top left, bottom right, top right, use unity_StereoEyeIndex * 4 to index
//uniform float3 _WeatherMakerCameraFrustumRays[8];
//uniform float3 _WeatherMakerCameraFrustumRaysTemporal[8];

// contains temporal uv coordinates
uniform float4 _WeatherMakerTemporalUV;

// inverse matrix for each eye, use unity_StereoEyeIndex to idnex
uniform float4x4 _WeatherMakerInverseView[2];
uniform float4x4 _WeatherMakerInverseProj[2];

uniform uint _WeatherMakerCameraRenderMode;
#define WM_CAMERA_RENDER_MODE_NORMAL (_WeatherMakerCameraRenderMode == 0)
#define WM_CAMERA_RENDER_MODE_NOT_NORMAL (_WeatherMakerCameraRenderMode != 0)
#define WM_CAMERA_RENDER_MODE_REFLECTION (_WeatherMakerCameraRenderMode == 1)
#define WM_CAMERA_RENDER_MODE_NOT_REFLECTION (_WeatherMakerCameraRenderMode != 1)
#define WM_CAMERA_RENDER_MODE_CUBEMAP (_WeatherMakerCameraRenderMode == 2)
#define WM_CAMERA_RENDER_MODE_NOT_CUBEMAP (_WeatherMakerCameraRenderMode != 2)

uniform float _WeatherMakerDownsampleScale;
uniform float _WeatherMakerTemporalReprojectionEnabled; // 0 = none, 1 = sub frame mode, 2 = full mode
static const uint weatherMakerTemporalReprojectionEnabled = uint(round(_WeatherMakerTemporalReprojectionEnabled));
static const uint weatherMakerTemporalReprojectionSubFrameEnabled = (weatherMakerTemporalReprojectionEnabled == 1);
static const float2 _WeatherMakerTemporalUV_VertexShaderProjection = (_WeatherMakerTemporalUV * 2.0 * weatherMakerTemporalReprojectionSubFrameEnabled);
static const float2 _WeatherMakerTemporalUV_FragmentShader = (_WeatherMakerTemporalUV * weatherMakerTemporalReprojectionSubFrameEnabled);
static const uint weatherMakerDownsampleScale = uint(round(_WeatherMakerDownsampleScale));

// 0 = blur blend, 1 = sharp blend
uniform float _TemporalReprojection_BlendMode;

static const uint weatherMakerTemporalReprojectionBlendModeBlur = (uint(round(_TemporalReprojection_BlendMode)) == 0);

#if !defined(WEATHER_MAKER_NO_DEFINE_TEXTURES)

#if defined(WEATHER_MAKER_IS_FULL_SCREEN_EFFECT) && defined(WM_STEREO_INSTANCING_ENABLED)

uniform UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
uniform UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex2);

#elif defined(WEATHER_MAKER_IS_FULL_SCREEN_EFFECT) && defined(WEATHER_MAKER_MAIN_TEX_SAMPLERS)

uniform UNITY_DECLARE_TEX2D(_MainTex);
uniform UNITY_DECLARE_TEX2D(_MainTex2);

#else

uniform sampler2D _MainTex;
uniform sampler2D _MainTex2;

#endif

UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraDepthNormalsTexture);
UNITY_DECLARE_SCREENSPACE_TEXTURE(_WeatherMakerAfterForwardOpaqueTexture);
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

#endif

//UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTextureOne);
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTextureHalf);
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTextureQuarter);
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTextureEighth);
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTextureSixteenth);

//UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraMotionVectorsTexture);
//float2 motion = WM_SAMPLE_FULL_SCREEN_TEXTURE(_CameraMotionVectorsTexture, i.uv).xy;
//float motionLength = length(motion * _CameraMotionVectorsTexture_TexelSize.zw);

uniform float4 _MainTex_ST;
uniform float4 _MainTex_TexelSize;
uniform float4 _MainTex2_ST;
uniform float4 _MainTex2_TexelSize;
uniform float4 _CameraMotionVectorsTexture_ST;
uniform float4 _CameraMotionVectorsTexture_TexelSize;
uniform sampler2D _WeatherMakerBlueNoiseTexture;

#if defined(SOFTPARTICLES_ON)

float _InvFade;

#endif

#ifndef UNITY_SAMPLE_TEX2D_SAMPLER_LOD

#define UNITY_SAMPLE_TEX2D_SAMPLER_LOD(tex, samp, uv) tex.SampleLevel(sampler##samp, uv, 0.0)

#endif

#if defined(WM_STEREO_INSTANCING_ENABLED)

#define WM_SAMPLE_FULL_SCREEN_TEXTURE_PROJ(tex, uv) UNITY_SAMPLE_TEX2DARRAY_PROJ(tex, uv)
#define WM_SAMPLE_FULL_SCREEN_TEXTURE(tex, uv) UNITY_SAMPLE_TEX2DARRAY_LOD(tex, float3((uv).xy, (float)unity_StereoEyeIndex), 0.0)
#define WM_SAMPLE_FULL_SCREEN_TEXTURE_SAMPLER(tex, samp, uv) UNITY_SAMPLE_TEX2DARRAY_SAMPLER_LOD(tex, samp, float3((uv).xy, (float)unity_StereoEyeIndex), 0.0)

#else

#define WM_SAMPLE_FULL_SCREEN_TEXTURE_PROJ(tex, uv) tex2Dproj(tex, uv)
#define WM_SAMPLE_FULL_SCREEN_TEXTURE(tex, uv) tex2Dlod(tex, float4((uv).xy, 0.0, 0.0))
#define WM_SAMPLE_FULL_SCREEN_TEXTURE_SAMPLER(tex, samp, uv) UNITY_SAMPLE_TEX2D_SAMPLER_LOD(tex, samp, uv)

#endif

#define WM_SAMPLE_TEX2D_ARRAY_STEREO(tex, uv) UNITY_SAMPLE_TEX2DARRAY_LOD(tex, float3((uv).xy, unity_StereoEyeIndex), 0.0)

#define WM_LINEAR_DEPTH_01(depth) lerp(Linear01Depth(depth), 1.0, unity_OrthoParams.w)
#define WM_SAMPLE_DEPTH_TEXTURE_01(tex, uv) WM_LINEAR_DEPTH_01(UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(tex, uv)))


float _WeatherMakerEnableToneMapping;

static const float4x4 _WeatherMakerBayerMatrix =
{
	1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
	13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
	4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
	16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
};
static const float4x4 _WeatherMakerBayerMatrixRowAccess = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };

inline float2 AdjustFullScreenUV(float2 uv)
{
	uv = UnityStereoTransformScreenSpaceTex(uv);
	//uv = UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST);

#if UNITY_UV_STARTS_AT_TOP

	if (_MainTex_TexelSize.y < 0)
	{
		uv.y = 1.0 - uv.y;
	}

#endif

	return uv;
}

inline float RandomFloat(float3 v)
{
	return (frac(frac(dot(v.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453) - 0.5) * 2.0;
	//return frac(sin(dot(v.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
}

inline float RandomFloat2D(float2 v)
{
	return (frac(frac(dot(v, float2(12.9898, 78.233))) * 43758.5453) - 0.5) * 2.0;
}

inline float RandomFloat01(float3 v)
{
	return (frac(frac(dot(v.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453));
}

// see https://bib.irb.hr/datoteka/949019.Final_0036470256_56.pdf
float Remap(float originalValue, float originalMin, float originalMax, float newMin, float newMax)
{
	return newMin + (((originalValue - originalMin) / (originalMax - originalMin)) * (newMax - newMin));
}

// Uncharted 2 tonemap from http ://filmicgames.com/archives/75
inline float3 FilmicTonemap(float3 x)
{
	static const float A = 0.15;
	static const float B = 0.50;
	static const float C = 0.10;
	static const float D = 0.20;
	static const float E = 0.02;
	static const float F = 0.30;
	return ((x * (A * x + C * B) + D * E) / (x *(A * x + B) + D * F)) - E / F;
}

inline float3 FilmicTonemapFull(float3 color, float exposure)
{
	float3 curr = FilmicTonemap(exposure * color.rgb);
	float3 whiteScale = 1.0f / FilmicTonemap(11.175);
	return curr * whiteScale;
}

inline float GetDepth01FromWorldSpaceRay(float3 rayDir, float dist)
{
	float3 localWorldPos = rayDir * dist;
	return -(mul((float3x3)UNITY_MATRIX_V, localWorldPos).z * _ProjectionParams.w);
}

inline float GetDepth01(float2 uv)
{
	return WM_LINEAR_DEPTH_01(WM_SAMPLE_DEPTH(uv));
}

inline void GetDepth01AndNormal(float2 uv, out float depth, out float3 normal)
{
	float4 samp = WM_SAMPLE_FULL_SCREEN_TEXTURE(_CameraDepthNormalsTexture, uv);
	DecodeDepthNormal(samp, depth, normal);
}

inline float WM_SAMPLE_DEPTH_DOWNSAMPLED_01(float2 uv)
{
	UNITY_BRANCH
	switch (weatherMakerDownsampleScale)
	{
	default:
		return WM_LINEAR_DEPTH_01(UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv)));

	case 2:
		return (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTextureHalf, uv)));

	case 4:
		return (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTextureQuarter, uv)));

	case 8:
		return (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTextureEighth, uv)));

	case 16:
		return (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTextureSixteenth, uv)));
	}
}

inline float WM_SAMPLE_DEPTH_DOWNSAMPLED_TEMPORAL_REPROJECTION_01(float2 uv)
{
	return WM_SAMPLE_DEPTH_DOWNSAMPLED_01(uv + _WeatherMakerTemporalUV_FragmentShader);
}

inline float WM_SAMPLE_DEPTH_AREA_DOWNSAMPLED_TEMPORAL_REPROJECTION_01(float2 uv)
{
	UNITY_BRANCH
	if (weatherMakerTemporalReprojectionSubFrameEnabled)
	{
		// for temporal reprojection we take advantage of the eighth depth texture to ensure we render clouds behind tree
		//  branches and the like, that way when the camera moves, those pixels will be available to re-use, preventing
		//  the sky and other bright pixels showing, especially at night we still get the performance benefit of large
		//  blocked areas like ground and mountains, roofs, etc.
		float2 offset = _CameraDepthTextureSixteenth_TexelSize;

#define samp _CameraDepthTextureSixteenth

		// pick max depth value out of the grid to ensure rendering happens underneath edges
		float depth = (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv)));
		depth = max(depth, (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv + offset))));
		depth = max(depth, (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv - offset))));
		depth = max(depth, (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv + float2(-offset.x, offset.y)))));
		depth = max(depth, (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv + float2(offset.x, -offset.y)))));
		depth = max(depth, (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv + float2(offset.x, 0.0)))));
		depth = max(depth, (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv - float2(offset.x, 0.0)))));
		depth = max(depth, (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv + float2(0.0, offset.y)))));
		depth = max(depth, (UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE(samp, uv - float2(0.0, offset.y)))));

#undef samp

		return depth;
	}
	else
	{
		return WM_SAMPLE_DEPTH_DOWNSAMPLED_01(uv);
	}
}

inline float2 AlignScreenUVWithDepthTexel(float2 uv)
{

#if UNITY_UV_STARTS_AT_TOP

	if (_CameraDepthTexture_TexelSize.y < 0)
	{
		uv.y = 1 - uv.y;
	}

#endif

	return (floor(uv * _CameraDepthTexture_TexelSize.zw) + 0.5) *
		abs(_CameraDepthTexture_TexelSize.xy);
}


inline fixed LerpFade(float4 lifeTime, float timeSinceLevelLoad)
{
	// the vertex will fade in, stay at full color, then fade out
	// x = creation time seconds
	// y = fade time in seconds
	// z = life time seconds

	// debug
	// return 1;

	float peakFadeIn = lifeTime.x + lifeTime.y;
	float startFadeOut = lifeTime.x + lifeTime.z - lifeTime.y;
	float endTime = lifeTime.x + lifeTime.z;
	float lerpMultiplier = saturate(ceil(timeSinceLevelLoad - peakFadeIn));
	float lerp1Scalar = saturate(((timeSinceLevelLoad - lifeTime.x + 0.000001) / max(0.000001, (peakFadeIn - lifeTime.x))));
	float lerp2Scalar = saturate(max(0, ((timeSinceLevelLoad - startFadeOut) / max(0.000001, (endTime - startFadeOut)))));
	float lerp1 = lerp1Scalar * (1.0 - lerpMultiplier);
	float lerp2 = (1.0 - lerp2Scalar) * lerpMultiplier;
	return lerp1 + lerp2;
}

inline float GetNearPlane()
{

#if defined(UNITY_REVERSED_Z)

	return 1.0 - UNITY_NEAR_CLIP_VALUE;

#else

	return UNITY_NEAR_CLIP_VALUE;

#endif

}

inline void ApplyDither(inout fixed3 rgb, float2 screenUV, fixed l)
{
	fixed3 gradient = frac(cos(dot(screenUV * _WeatherMakerTime.x, ditherMagic.xy)) * ditherMagic.z) * l;
	rgb = max(0, (rgb - gradient));
}

inline void ApplyDitherNoTime(inout fixed3 rgb, float2 screenUV, fixed l)
{
	fixed3 gradient = frac(cos(dot(screenUV, ditherMagic.xy)) * ditherMagic.z) * l;
	rgb = max(0, (rgb - gradient));
}

inline float4 UnityObjectToClipPosFarPlane(float4 vertex)
{
	float4 pos = UnityObjectToClipPos(float4(vertex.xyz, 1.0));
	if (UNITY_NEAR_CLIP_VALUE == 1.0)
	{
		// DX
		pos.z = 0.0;
	}
	else
	{
		// OpenGL
		pos.z = pos.w;
	}
	return pos;
}

inline float3 WorldSpaceVertexPosNear(float3 vertex)
{
	return mul(unity_ObjectToWorld, float4(vertex.xyz, 0.0)).xyz;
}

inline float3 WorldSpaceVertexPosFar(float3 vertex)
{
	return mul(unity_ObjectToWorld, float4(vertex.xyz, 1.0)).xyz;
}

inline float3 WorldSpaceVertexPos(float4 vertex)
{
	return mul(unity_ObjectToWorld, vertex).xyz;
}

/*
inline float3 GetFarPlaneVectorFullScreen(float2 uv, int eyeIndex)
{
	uv.x = (uv.x > 0.5);
	uv.y = (uv.y > 0.5);
	if (weatherMakerTemporalReprojectionSubFrameEnabled)
	{
		return _WeatherMakerCameraFrustumRaysTemporal[(eyeIndex * 4.0) + ((uv.x * 2.0) + uv.y)];
	}
	else
	{
		return _WeatherMakerCameraFrustumRays[(eyeIndex * 4.0) + ((uv.x * 2.0) + uv.y)];
	}
}
*/

inline float3 GetFullScreenRayDir(float3 rayDir)
{
	if (WM_CAMERA_RENDER_MODE_CUBEMAP)
	{
		// cubemaps cannot calculate the rayDir in vertex shader, each pixel needs to re-calculate the rayDir from clip pos
		// see https://github.com/chriscummings100/worldspaceposteffect/blob/master/Assets/WorldSpacePostEffect/WorldSpacePostEffect.shader

#if defined(UNITY_REVERSED_Z)

		float4 clipPos = float4(rayDir.xy, GetNearPlane(), 1.0);

#else

		// WTF OpenGL requires -1.0 w and flipping the x???
		float4 clipPos = float4(rayDir.xy, GetNearPlane(), -1.0);
		clipPos.x *= -1;

#endif

		// cannot use combined view*proj matrix with cubemap, not sure why...
		clipPos = mul(_WeatherMakerInverseProj[unity_StereoEyeIndex], clipPos);
		clipPos.xyz = clipPos.xyz / clipPos.w;
		return normalize(mul((float3x3)_WeatherMakerInverseView[unity_StereoEyeIndex], clipPos.xyz));
	}
	else
	{
		return normalize(rayDir);
	}
}

inline volumetric_data GetVolumetricData(appdata_base v)
{
	WM_INSTANCE_VERT(v, volumetric_data, o);
	o.vertex = UnityObjectToClipPosFarPlane(v.vertex);
	o.normal = UnityObjectToWorldNormal(v.normal);
	o.projPos = ComputeScreenPos(o.vertex);
	o.worldPos = WorldSpaceVertexPos(v.vertex);
	o.rayDir = -WorldSpaceViewDir(v.vertex);
	o.viewPos = UnityObjectToViewPos(v.vertex);
	return o;
}

full_screen_fragment full_screen_vertex_shader(full_screen_vertex v)
{
	WM_INSTANCE_VERT(v, full_screen_fragment, o);
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv.xy = AdjustFullScreenUV(v.uv);
	float2 cameraUV = (2.0 * v.uv.xy) - 1.0;
	if (WM_CAMERA_RENDER_MODE_CUBEMAP)
	{
		// cube map must get the ray in the fragment due to being 360 degrees view
		// temporal reprojection not supported for cubemap
		o.forwardLine = float3(cameraUV, 1.0);
	}
	else if (unity_OrthoParams.w == 0.0)
	{
		//o.forwardLine = GetFarPlaneVectorFullScreen(v.uv, unity_StereoEyeIndex);
		float4 clipPos = float4(cameraUV + _WeatherMakerTemporalUV_VertexShaderProjection, 1.0, 1.0);
		clipPos = mul(_WeatherMakerInverseProj[unity_StereoEyeIndex], clipPos);
		clipPos.xyz /= clipPos.w;
		o.forwardLine = mul((float3x3)_WeatherMakerInverseView[unity_StereoEyeIndex], clipPos.xyz);
	}
	else
	{
		float2 m = float2(_ScreenParams.x * 0.001, _ScreenParams.y * 0.001);
		o.forwardLine = float3(m.x * (o.uv.x - 0.5), m.y * (o.uv.y - 0.25), 1.0);
	}

	o.rayDir = o.forwardLine;
	o.uv.zw = o.uv.xy * _ScreenParams.xy;
	return o;
}

full_screen_fragment_reflection full_screen_vertex_shader_refl(full_screen_vertex v)
{
	WM_INSTANCE_VERT(v, full_screen_fragment_reflection, o);
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv.xy = AdjustFullScreenUV(v.uv);
	float2 cameraUV = (2.0 * v.uv.xy) - 1.0;
	if (WM_CAMERA_RENDER_MODE_CUBEMAP)
	{
		// cube map must get the ray in the fragment due to being 360 degrees view
		// temporal reprojection not supported for cubemap
		o.forwardLine = float3(cameraUV, 1.0);
	}
	else if (unity_OrthoParams.w == 0.0)
	{
		//o.forwardLine = GetFarPlaneVectorFullScreen(v.uv, unity_StereoEyeIndex);
		float4 clipPos = float4(cameraUV + _WeatherMakerTemporalUV_VertexShaderProjection, 1.0, 1.0);
		clipPos = mul(_WeatherMakerInverseProj[unity_StereoEyeIndex], clipPos);
		clipPos.xyz /= clipPos.w;
		o.forwardLine = mul((float3x3)_WeatherMakerInverseView[unity_StereoEyeIndex], clipPos.xyz);
	}
	else
	{
		float2 m = float2(_ScreenParams.x * 0.001, _ScreenParams.y * 0.001);
		o.forwardLine = float3(m.x * (o.uv.x - 0.5), m.y * (o.uv.y - 0.25), 1.0);
	}

	o.rayDir = o.forwardLine;
	o.uv.zw = o.uv.xy * _ScreenParams.xy;

	o.reflectionPos = ComputeNonStereoScreenPos(o.vertex);
	o.screenPos = ComputeScreenPos(o.vertex);

	return o;
}

full_screen_fragment_vertex_uv full_screen_vertex_shader_vertex_uv(full_screen_vertex v)
{
	WM_INSTANCE_VERT(v, full_screen_fragment_vertex_uv, o);
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = AdjustFullScreenUV(v.uv);
	return o;
}

#endif
