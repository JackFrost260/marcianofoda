Shader "WeatherMaker/WeatherMakerSkyPlaneShader"
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
		Tags { "Queue" = "Geometry" }
		Cull Off ZWrite Off ZTest LEqual Fog { Mode Off } Blend Off

		CGINCLUDE

		#pragma target 3.5
		#pragma exclude_renderers gles
		#pragma exclude_renderers d3d9
		

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			// note sky plane is always procedural Unity style
			#include "WeatherMakerSkyShaderInclude.cginc"

			fixed _WeatherMakerSkyYOffset2D;

			v2fSky vert (appdata_base v)
			{
				WM_INSTANCE_VERT(v, v2fSky, o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy; // TRANSFORM_TEX not supported
				o.ray = lerp(float3(0.0, 0.0, 1.0), float3(0.0, 1.0, 0.0), max(0.0, o.uv.y - _WeatherMakerSkyYOffset2D));
				procedural_sky_info i = CalculateScatteringCoefficients(_WeatherMakerSunDirectionDown2D, _WeatherMakerSunColor.rgb * pow(_WeatherMakerSunColor.a, 0.5), 1.0, normalize(o.ray));
				o.inScatter = i.inScatter;
				o.outScatter = i.outScatter;
				o.normal = float3(0.0, 0.0, 0.0);
                return o;
			}
			
			fixed4 frag (v2fSky i) : SV_Target
			{
				WM_INSTANCE_FRAG(i);
				i.ray = normalize(i.ray);
				fixed3 sunColor = _WeatherMakerSunColor.rgb * _WeatherMakerSunColor.a;
				procedural_sky_info p = CalculateScatteringColor(_WeatherMakerSunDirectionDown2D, sunColor, 0.0, i.ray, i.inScatter, i.outScatter);
				fixed3 nightColor = GetNightColor(i.ray, i.uv, p.skyColor.a);
				fixed3 result = (((p.inScatter + p.outScatter) * _WeatherMakerSkyTintColor)) + nightColor;
				ApplyDither(result.rgb, i.uv, _WeatherMakerSkyDitherLevel);
				return float4(result.rgb, 1.0);
			}

			ENDCG
		}
	}
}
