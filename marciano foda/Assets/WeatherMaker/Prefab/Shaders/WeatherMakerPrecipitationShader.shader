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

Shader "WeatherMaker/WeatherMakerPrecipitationShader"
{
    Properties
	{
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "gray" {}
		_TintColor ("Tint Color (RGB)", Color) = (1, 1, 1, 1)
		_PointSpotLightMultiplier ("Point/Spot Light Multiplier", Range (0, 10)) = 1
		_DirectionalLightMultiplier ("Directional Light Multiplier", Range (0, 10)) = 1
		_ShadowStrength("Shadow strength, 0 for no dir light shadows", Range(0.0, 1.0)) = 0.0
		_InvFade ("Soft Particles Factor", Range(0.001, 100.0)) = 1.0
		_AmbientLightMultiplier ("Ambient light multiplier", Range(0, 4)) = 1
		_Intensity ("Increase the alpha value by this multiplier", Range(0, 10)) = 1
		_SrcBlendMode ("SrcBlendMode (Source Blend Mode)", Int) = 5 // SrcAlpha
		_DstBlendMode ("DstBlendMode (Destination Blend Mode)", Int) = 10 // OneMinusSrcAlpha
		_ParticleDitherLevel("Dither Level", Range(0, 1)) = 0.002
		_ParticleZClip("Particle Z Distance Clip", Range(0, 10)) = 2
		_ShadowStrength("Shadow strength", Range(0.0, 1.0)) = 1.0
    }

    SubShader
	{
        Tags { "Queue" = "Transparent-2" }
		LOD 100

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9
		

		ENDCG

        Pass
		{
			ZWrite Off
			Cull Back
			Blend [_SrcBlendMode] [_DstBlendMode]
 
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
			#pragma multi_compile_particles
			#pragma multi_compile_instancing
			#pragma multi_compile __ ORTHOGRAPHIC_MODE
			#pragma multi_compile __ WEATHER_MAKER_PER_PIXEL_LIGHTING

			#define WEATHER_MAKER_FILTER_LIGHT_ORTHOGRAPHIC
			#define WEATHER_MAKER_LIGHT_NO_NORMALS
			#define WEATHER_MAKER_LIGHT_NO_SPECULAR
			#define NULL_ZONE_RENDER_MASK 1 // precipitation is 1

			#define WEATHER_MAKER_SHADOWS_DEPTH_EXTERNAL_FUNC ComputeCloudShadowStrength
			#include "WeatherMakerCloudVolumetricShaderInclude.cginc"
			#include "WeatherMakerFogShaderInclude.cginc"

			uniform fixed _SpecularPower;
			uniform fixed _ParticleDitherLevel;
			uniform fixed _ParticleZClip;
			uniform fixed _ShadowStrength;

			static const float ambientColor = min(_WeatherMakerAmbientLightColorGround + _WeatherMakerAmbientLightColorSky, pow(_WeatherMakerAmbientLightColorGround + _WeatherMakerAmbientLightColorSky, 2.0));

			struct appdata_t
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				WM_BASE_VERTEX_INPUT
			};

		    struct v2f
            {
				fixed4 color : COLOR0;
                float4 pos : SV_POSITION;
				half2 uv_MainTex : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float4 projPos : TEXCOORD2;

#if defined(SOFTPARTICLES_ON)

				float3 rayDir : NORMAL;

#endif

				WM_BASE_VERTEX_TO_FRAG
            };
 
            v2f vert(appdata_t v)
            {
				WM_INSTANCE_VERT(v, v2f, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldPos = WorldSpaceVertexPos(v.vertex);

#if defined(WEATHER_MAKER_PER_PIXEL_LIGHTING)

				o.color.rgb = v.color.rgb * _TintColor.rgb;

#else

				fixed3 diffuseColor = v.color.rgb * _TintColor.rgb;
				wm_world_space_light_params p;
				p.worldPos = o.worldPos;
				p.diffuseColor = diffuseColor;
				p.ambientColor = ambientColor;
				p.shadowStrength = _ShadowStrength;
				o.color.rgb = CalculateLightColorWorldSpace(p);

#endif

				o.color.a = min(1.0, v.color.a * _TintColor.a * _Intensity);

#if defined(SOFTPARTICLES_ON)

				o.rayDir = (o.worldPos - _WorldSpaceCameraPos);

#endif

                o.projPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);

                return o; 
            }
			
            fixed4 frag (v2f v) : COLOR
			{       
				WM_INSTANCE_FRAG(v);

#if defined(SOFTPARTICLES_ON) && !defined(ORTHOGRAPHIC_MODE)

				fixed nearFade = v.projPos.z - _ParticleZClip;
				clip(nearFade);
				nearFade = min(1.0, nearFade * 0.5);

				float sceneZ = LinearEyeDepth(WM_SAMPLE_DEPTH_PROJ(v.projPos));
				float partZ = v.projPos.z;
				float diff = (sceneZ - partZ);
				v.color.a *= saturate(_InvFade * diff);
				v.color.a *= nearFade * ClipWorldPosNullZonesAlpha(v.worldPos);

#endif // defined(SOFTPARTICLES_ON)

#if defined(WEATHER_MAKER_PER_PIXEL_LIGHTING)

				fixed4 color = tex2D(_MainTex, v.uv_MainTex) * v.color;
				wm_world_space_light_params p;
				p.worldPos = v.worldPos;
				p.diffuseColor = color.rgb;
				p.ambientColor = ambientColor;
				p.shadowStrength = _ShadowStrength;
				color.rgb = CalculateLightColorWorldSpace(p);

#else

				fixed4 color = tex2D(_MainTex, v.uv_MainTex) * v.color;

#endif

#if UNITY_VERSION >= 201820

				color.rgb *= 3.0;

#endif

				// dither
				ApplyDither(color.rgb, v.projPos.xy, _ParticleDitherLevel);

				return color;
            }

            ENDCG
        }
    }
 
    Fallback Off
}