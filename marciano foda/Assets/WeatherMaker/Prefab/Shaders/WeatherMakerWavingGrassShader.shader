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

// *** NOTE ***: uncomment the next line and uncomment the line after to enable this grass shader
Shader "WeatherMaker/Grass-WavingDoublePass" {
//Shader "Hidden/TerrainEngine/Details/WavingDoublePass" {
Properties {
	_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
}

SubShader {
	Tags {
		"Queue" = "Transparent-200"
		"IgnoreProjector"="True"
		"RenderType"="Grass"
	}
	Cull Off // Grass is two-sided, so don't cull either side
	LOD 200
	ColorMask RGB
		
CGPROGRAM
#pragma surface surf Lambert vertex:WavingGrassVert alpha
#pragma exclude_renderers flash
#include "TerrainEngine.cginc"

sampler2D _MainTex;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
	o.Albedo = c.rgb;
	o.Alpha = c.a * IN.color.a;
}
ENDCG
}
	
	SubShader {
		Tags {
			"Queue" = "Transparent+200"
			"IgnoreProjector"="True"
			"RenderType"="Grass"
		}
		Cull Off // Grass is two-sided, so don't cull either side
		LOD 200
		ColorMask RGB
		
		Pass {
			Tags { "LightMode" = "Vertex" }
			Material {
				Diffuse (1,1,1,1)
				Ambient (1,1,1,1)
			}
			Lighting On
			ColorMaterial AmbientAndDiffuse
			SetTexture [_MainTex] { combine texture * primary DOUBLE, texture }
		}
		Pass {
			Tags { "LightMode" = "VertexLMRGBM" }
			BindChannels {
				Bind "Vertex", vertex
				Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
				Bind "texcoord", texcoord1 // main uses 1st uv
			}
			SetTexture [unity_Lightmap] {
				matrix [unity_LightmapMatrix]
				combine texture * texture alpha DOUBLE
			}
			SetTexture [_MainTex] { combine texture * previous QUAD, texture }
		}
	}
	
	Fallback Off
}