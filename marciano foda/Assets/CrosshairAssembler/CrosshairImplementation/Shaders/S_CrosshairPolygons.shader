
Shader "Custom/CrosshairPolygons"
{
    Properties {
    	_MainTex ("Base (RGB)", 2D) = "white" {}
		inColor ("inColor", Color) = (1,1,1,1)

		nGonFloat("nGonFloat", float) = 0
		elementRotation("elementRotation", float) = 0
		rotateToLaidPolygon("rotateToLaidPolygon", float) = 0
		radius("radius", float) = 40
		thickness("thickness", float) = 4
		AAFilterSize("AAFilterSize", float) = 1
		fillInside("fillInside", float) = 0
		nElementsFloat("nElementsFloat", float) = 4
		layerRotation("layerRotation", float) = 0
		gap("gap", float) = 100
		imageSize("imageSize", Vector) = (400, 300, 0, 0)
		stepAngle("stepAngle", float) = 90
		translation("translation", Vector) = (0, 0, 0, 0)
		scale("scale", Vector) = (1, 1, 0, 0)

    }
    SubShader {
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
            CGPROGRAM
           
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
           
            fixed4 inColor; // low precision type is usually enough for colors
            float nGonFloat;
			float elementRotation;
			float rotateToLaidPolygon;
			float radius;
			float thickness;
			float AAFilterSize;
			float fillInside;
            float nElementsFloat;
			float layerRotation;
			float gap;
			float2 imageSize;
            float stepAngle;
            float2 translation;
            float2 scale;
           
            struct fragmentInput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fragmentInput vert (appdata_base v)
            {
                fragmentInput o;

                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord.xy;

                return o;
            }

            float RegularPolygon(fragmentInput i)
            {
            	// param
				float2 uv;
				uv = i.uv;

				#define Y_FLIP 1

				// code polygons
				#define PI 3.1415926535897932384626433832795

				float2 o = translation;
				float2 pixelPos = (uv - float2(0.5, 0.5)) * imageSize.xy;

				uint nNum = nElementsFloat;
				float alpha = 0;

				for (uint elementIndex = 0; elementIndex < nNum; ++elementIndex)
				{
					float polygonAngle = stepAngle * elementIndex + (90 * Y_FLIP);
					float spreadAngle = polygonAngle + layerRotation;
					float spreadAngleRad = radians(spreadAngle);
					float2 elementOrigin = float2(cos(spreadAngleRad), sin(spreadAngleRad)) * gap;

					uint nGon = nGonFloat;

					// Draw side of polygon
					float2 pPrev, pCurr, qCurr, rCurr;
					float polygonOut = 1;
					float polygonIn = 1;
					for (uint vertexIndex = 0; vertexIndex <= nGon; ++vertexIndex)
					{
						pPrev = pCurr;

						float centralAngle = 2 * PI / nGon * (vertexIndex % nGon) + radians(rotateToLaidPolygon);

						//local position
						pCurr = float2(cos(centralAngle), sin(centralAngle)) * radius;

						//scale
						pCurr *= scale.yx;

						//rotation
						float effectiveElementAngleRad = radians(elementRotation + spreadAngle);
						float cosElementAngle = cos(effectiveElementAngleRad);
						float sinElementAngle = sin(effectiveElementAngleRad);
						pCurr = float2(cosElementAngle * pCurr.x - sinElementAngle * pCurr.y, sinElementAngle * pCurr.x + cosElementAngle * pCurr.y);

						//translation
						pCurr += o + elementOrigin;

						if (vertexIndex == 0)
							continue;

						//Compute inside/outside vertices
						float2 dirSide = normalize(pCurr - pPrev);
						float2 normalSide = float2(dirSide.y, -dirSide.x);
						float2 dirCurr = normalize(pCurr - (o + elementOrigin));

						float halfAASize = AAFilterSize * 0.5;

						qCurr = pCurr + dirCurr * (thickness * 0.5f + halfAASize) / abs(dot(dirCurr, normalSide));
						rCurr = pCurr - dirCurr * (thickness * 0.5f - halfAASize) / abs(dot(dirCurr, normalSide));

						//compute outside and inside triangle alphas
						float crossProductOut = cross(float3(dirSide, 0), float3(pixelPos - qCurr, 0)).z;
						//polygonOut *= sign(clamp((crossProductOut),0,1));
						polygonOut *= smoothstep(0, AAFilterSize, crossProductOut);
						float crossProductIn = cross(float3(dirSide, 0), float3(pixelPos - rCurr, 0)).z;
						//polygonIn *=sign(clamp((crossProductIn),0,1));
						polygonIn *= smoothstep(0, AAFilterSize, crossProductIn);
					}

					alpha += fillInside ? polygonOut : polygonOut * (1 - polygonIn);
				}

				return alpha;
				// code end
            }

			fixed4 frag(fragmentInput i) : SV_Target {				

				return fixed4(inColor.r, inColor.g, inColor.b, inColor.a*RegularPolygon(i));
			}              
            
            ENDCG
        }
    }
}