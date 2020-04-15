
Shader "Custom/CrosshairRectangles"
{
    Properties {
    	_MainTex ("Base (RGB)", 2D) = "white" {}
		tex ("tex", 2D) = "white" {}

        layerColor ("layerColor", Color) = (1,1,1,1)
		bgColor ("bgColor", Color) = (0,0,0,0)

        nElementsFloat("nElementsFloat", float) = 0
        stepAngle("stepAngle", float) = 90
        elementRotation("elementRotation", float) = 0
		layerRotation("layerRotation", float) = 0
		gap("gap", float) = 100
		AAFilterSize("AAFilterSize", float) = 1

		size("size", Vector) = (4, 100, 0, 0)
		sizePivot("sizePivot", Vector) = (0.5, 0.5, 0, 0)
		rotationPivot("rotationPivot", Vector) = (0.5, 0.5, 0, 0)
		imageSize("imageSize", Vector) = (400, 300, 0, 0)
		translation("translation", Vector) = (0, 0, 0, 0)

    }
    SubShader {
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D tex;
            fixed4 layerColor; // low precision type is usually enough for colors
            fixed4 bgColor;
            float nElementsFloat;
            float stepAngle;
            float elementRotation;
            float layerRotation;
            float gap;
            float AAFilterSize;
            float2 size;
            float2 sizePivot;
            float2 rotationPivot;
            float2 imageSize;
            float2 translation;
           
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

            float4 Rectangles(fragmentInput i)
            {
            	// param
				float2 uv;
				uv = i.uv;

				#define texSampler 0
				#define Texture2DSample(tex,sam,tc) tex2D (tex, tc)
				#define Y_FLIP 1

				// code rectangles
				#define PI 3.1415926535897932384626433832795
				#define GET_CROSS2(v0, v1) ((v0).x * (v1).y - (v0).y * (v1).x)
				#define POLAR_TO_CARTESIAN_D(theta) float2(cos(((theta) * (PI))/180.0f), sin(((theta) * (PI))/180.0f))

				uint nElements = nElementsFloat;

				float2 pixelPos = uv * imageSize.xy;
				float2 originPos = imageSize.xy * float2(0.5f, 0.5f) + translation;
				float4 color = float4(0, 0, 0, 0);
				float aaAlpha = 0;

				if (size.x * size.y > 0) {
					for (int index = nElements - 1; index >= 0; --index)
					{
						float layerAngle = (stepAngle * index) + layerRotation;
						float elementAngle = layerAngle + elementRotation;

						float2 scaleBaseX = POLAR_TO_CARTESIAN_D(layerAngle);
						float2 scaleBaseY = float2(-scaleBaseX.y, scaleBaseX.x);

						float2 rotationBaseX = POLAR_TO_CARTESIAN_D(elementAngle);
						float2 rotationBaseY = float2(-rotationBaseX.y, rotationBaseX.x);

						float2 elementDirFromCenter = scaleBaseY * Y_FLIP;

						//float2 gapPos = originPos + gap * scaleBaseY * Y_FLIP;
						//float2 resizedCenterPos = gapPos + (0.5 - sizePivot.x)*scaleBaseX*size.x + (0.5 - sizePivot.y)*scaleBaseY*size.y; 
						//float2 rotPivotPos = resizedCenterPos + (rotationPivot.x - 0.5)*scaleBaseX*size.x + (rotationPivot.y - 0.5)*scaleBaseY*size.y; 
						//float2 finalCenterPos = rotPivotPos + (0.5 - rotationPivot.x)*rotationBaseX*size.x + (0.5 - rotationPivot.y)*rotationBaseY*size.y; 	

						float2 finalCenterPos = originPos
							+ gap * elementDirFromCenter
							+ ((rotationPivot.x - sizePivot.x) * scaleBaseX + (0.5 - rotationPivot.x) * rotationBaseX) * size.x
							+ ((rotationPivot.y - sizePivot.y) * scaleBaseY + (0.5 - rotationPivot.y) * rotationBaseY) * size.y;

						float2 sizeHalf = size * 0.5f;

						//origin: finalCenterPos
						//basis: rotationBaseY, rotationBaseX
						float2 p = pixelPos - finalCenterPos;
						float2 d = float2(GET_CROSS2(-rotationBaseY, p), GET_CROSS2(rotationBaseX, p));
						float2 aaStart = max(0, sizeHalf - AAFilterSize * 0.5f);
						float2 aaVal = 1 - smoothstep(0, AAFilterSize, abs(d) - aaStart);
						float alphaRectangle = aaVal.x * aaVal.y;

						float2 tc = clamp(0.5 * d / sizeHalf + 0.5f, 0, 1);

						float4 texel = Texture2DSample(tex, texSampler, tc);
						float alphaTexel = texel.a * alphaRectangle;

						//under operator
						if (alphaTexel > 0) {
							float newAlpha = color.a + (1 - color.a) * alphaTexel;
							color.rgb = (color.rgb * color.a + texel.rgb * alphaTexel * (1 - color.a)) / newAlpha;
							color.a = newAlpha;
						}
						aaAlpha += alphaRectangle;
					}
				}

				if (aaAlpha < 1) {
					float newAlpha = color.a + (1 - color.a) * bgColor.a;
					color.rgb = (color.rgb * color.a + bgColor.rgb * bgColor.a * (1 - color.a)) / newAlpha;
					color.a = newAlpha;
				}

				color *= layerColor;

				return color;
				// code end
            }

			fixed4 frag(fragmentInput i) : SV_Target {				

				return Rectangles(i);
			}              
            
            ENDCG
        }
    }
}