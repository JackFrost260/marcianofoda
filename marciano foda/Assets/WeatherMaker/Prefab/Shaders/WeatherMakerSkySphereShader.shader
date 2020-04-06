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

// Resources:
// http://library.nd.edu/documents/arch-lib/Unity/SB/Assets/SampleScenes/Shaders/Skybox-Procedural.shader
//

// TODO: Better sky: https://github.com/ngokevin/kframe/blob/master/components/sun-sky/shaders/fragment.glsl
// TODO: Better sky: https://threejs.org/examples/js/objects/Sky.js

Shader "WeatherMaker/WeatherMakerSkySphereShader"
{
	Properties
	{
		[NoScaleOffset] _MainTex("Day Texture", 2D) = "blue" {}
		[NoScaleOffset] _DawnDuskTex("Dawn/Dusk Texture", 2D) = "orange" {}
		[NoScaleOffset] _NightTex("Night Texture", 2D) = "black" {}
		_NightSkyMultiplier("Night Sky Multiplier", Range(0, 1)) = 0
		_NightVisibilityThreshold("Night Visibility Threshold", Range(0, 1)) = 0
		_NightIntensity("Night Intensity", Range(0, 32)) = 2
		_NightPower("Night Power", Range(0.01, 4.0)) = 1.0
		_NightDuskReducer("Night Dusk Reducer", Range(0.0, 256.0)) = 128.0
		_NightTwinkleSpeed("Night Twinkle Speed", Range(0, 100)) = 16
		_NightTwinkleVariance("Night Twinkle Variance", Range(0, 10)) = 1
		_NightTwinkleMinimum("Night Twinkle Minimum Color", Range(0, 1)) = 0.02
		_NightTwinkleRandomness("Night Twinkle Randomness", Range(0, 5)) = 0.15
	}
	SubShader
	{
		Tags { "Queue" = "Geometry+1" }

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9

		#include "WeatherMakerSkyShaderInclude.cginc"

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl_no_auto_normalization
		#pragma multi_compile_instancing

		fixed _WeatherMakerSkyYOffset2D;

		inline fixed4 SkyTexturedColor(fixed4 skyColor, fixed3 nightColor, fixed2 uv)
		{
			fixed4 dayColor = tex2D(_MainTex, uv) * _WeatherMakerDayMultiplier;
			fixed4 dawnDuskColor = tex2D(_DawnDuskTex, uv);
			fixed4 dawnDuskColor2 = dawnDuskColor * _WeatherMakerDawnDuskMultiplier;
			dayColor += dawnDuskColor2;

			// hide night texture wherever dawn/dusk is opaque, reduce if clouds
			nightColor *= (1.0 - dawnDuskColor.a);

			// blend texture on top of sky
			fixed4 result = ((dayColor * dayColor.a) + (skyColor * (1.0 - dayColor.a)));

			// blend previous result on top of night
			return ((result * result.a) + (fixed4(nightColor, 1.0) * (1.0 - result.a)));
		}

		inline fixed4 SkyNonTexturedColor(fixed4 skyColor, fixed3 nightColor)
		{
			return skyColor + fixed4(nightColor, 0.0);
		}

		v2fSky vert(appdata_base v)
		{
			WM_INSTANCE_VERT(v, v2fSky, o);
			o.vertex = UnityObjectToClipPosFarPlane(v.vertex);
			o.uv = v.texcoord.xy; // TRANSFORM_TEX not supported
			o.ray = -WorldSpaceViewDir(v.vertex);
			float3 normRay = normalize(o.ray);
			normRay.y += _WeatherMakerSkyYOffset2D;
			procedural_sky_info i = CalculateScatteringCoefficients(_WeatherMakerSunDirectionUp, _WeatherMakerSunColor.rgb, 1.0, normRay);
			o.inScatter = i.inScatter;
			o.outScatter = i.outScatter;
			o.normal = float3(0.0, 0.0, 0.0);
			return o;
		}

		fixed4 fragBase(v2fSky i)
		{
			WM_INSTANCE_FRAG(i);

			//return tex2D(_NightTex, i.uv);

			fixed4 result;
			i.ray = normalize(i.ray);
			i.ray.y += _WeatherMakerSkyYOffset2D;
			fixed4 skyColor;
			fixed3 nightColor;
			fixed sunMoon;

			UNITY_BRANCH
			if (WM_ENABLE_PROCEDURAL_TEXTURED_SKY || WM_ENABLE_PROCEDURAL_SKY)
			{
				procedural_sky_info p = CalculateScatteringColor(_WeatherMakerSunDirectionUp, _WeatherMakerSunColor.rgb, _WeatherMakerSunVar1.x, i.ray, i.inScatter, i.outScatter);
				skyColor = p.skyColor;
				skyColor.rgb *= _WeatherMakerSkyTintColor;
				nightColor = GetNightColor(i.ray, i.uv, skyColor.a);
				UNITY_BRANCH
				if (WM_ENABLE_PROCEDURAL_TEXTURED_SKY)
				{
					result = SkyTexturedColor(skyColor, nightColor, i.uv);
				}
				else
				{
					result = SkyNonTexturedColor(skyColor, nightColor);
				}
			}
			else if (WM_ENABLE_PROCEDURAL_TEXTURED_SKY_PREETHAM || WM_ENABLE_PROCEDURAL_SKY_PREETHAM)
			{
				skyColor = CalculateSkyColorPreetham(i.ray, _WeatherMakerSunDirectionUp, true);
				skyColor.rgb *= _WeatherMakerSkyTintColor;
				nightColor = GetNightColor(i.ray, i.uv, skyColor.a);
				if (WM_ENABLE_PROCEDURAL_TEXTURED_SKY_PREETHAM)
				{
					result = SkyTexturedColor(skyColor, nightColor, i.uv);
				}
				else
				{
					result = SkyNonTexturedColor(skyColor, nightColor);
				}
			}
			else // WM_ENABLE_TEXTURED_SKY
			{
				nightColor = GetNightColor(i.ray, i.uv, 0.0);
				fixed4 dayColor = tex2D(_MainTex, i.uv) * _WeatherMakerDayMultiplier;
				fixed4 dawnDuskColor = (tex2D(_DawnDuskTex, i.uv) * _WeatherMakerDawnDuskMultiplier);
				result = (dayColor + dawnDuskColor + fixed4(nightColor, 0.0));
			}

			ApplyDither(result.rgb, i.uv, _WeatherMakerSkyDitherLevel);
			return result;
		}

		fixed4 frag(v2fSky i) : SV_Target
		{
			return fragBase(i);
		}

		ENDCG

		Pass
		{
			Tags { }
			Cull Front Lighting Off ZWrite Off ZTest LEqual
			Blend One Zero

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}
	}

	FallBack Off
}
