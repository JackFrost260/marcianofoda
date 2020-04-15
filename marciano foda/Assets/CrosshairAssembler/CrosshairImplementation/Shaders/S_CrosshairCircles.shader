
Shader "Custom/CrosshairCircles"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		inColor ("inColor", Color) = (1,1,1,1)

		nElementsFloat("nElementsFloat", float) = 0
		stepAngle("stepAngle", float) = 90
		arcAngle("arcAngle", float) = 360
		elementRotation("elementRotation", float) = 0
		layerRotation("layerRotation", float) = 0
		radius("radius", float) = 40
		gap("gap", float) = 100
		thickness("thickness", float) = 4
		AAFilterSize("AAFilterSize", float) = 1
		fill("fill", float) = 0
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

            fixed4 inColor; // low precision type is usually enough for colors
            float nElementsFloat;
            float stepAngle;
			float arcAngle;
			float elementRotation;
			float layerRotation;
			float radius;
			float gap;
			float thickness;
			float AAFilterSize;
			float fill;
			float2 imageSize;
			float2 translation;
             
            struct fragmentInput {
                float4 pos : SV_POSITION;
                float2 uv : TEXTCOORD0;
            };
  
            fragmentInput vert (appdata_base v)
            {
                fragmentInput o;
  
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord.xy;
  
                return o;
            }

			float Circular(fragmentInput i)
            {
            	// param
				float2 uv;
				uv = i.uv;

				#define Y_FLIP 1

				// code circles
				#define PI 3.1415926535897932384626433832795
				#define GET_CROSS_PRODUCT(p0, p1, p2) ((normalize((p1)-(p0)).x * ((p2)-(p0)).y) - (normalize((p1)-(p0)).y * ((p2)-(p0)).x))
				#define POLAR_TO_CARTESIAN_D(theta, r) float2(cos(((theta) * (PI))/180.0f) * r, sin(((theta) * (PI))/180.0f) * r)

				float layer = 0;
				uint nElements = nElementsFloat;

				for (uint index = 0; index < nElements; ++index)
				{
					float2 o, p, q;
					float angle = (stepAngle * index) + layerRotation + (90 * Y_FLIP);
					o = POLAR_TO_CARTESIAN_D(angle, gap) + imageSize.xy / 2.0f + translation;
					float angleP = angle - arcAngle * 0.5 + elementRotation;
					p = o + POLAR_TO_CARTESIAN_D(angleP, radius);
					float angleQ = angle + arcAngle * 0.5 + elementRotation;
					q = o + POLAR_TO_CARTESIAN_D(angleQ, radius);

					float2 pixelPos = uv * imageSize.xy - o;

					// make circle
					float dist = sqrt(pow(pixelPos.x, 2) + pow(pixelPos.y, 2));
					float t = max(0, thickness*0.5 - AAFilterSize * 0.5);
					float innerMax = radius - t;
					float innerMin = innerMax - (AAFilterSize);
					float outerMin = radius + t;
					float outerMax = outerMin + (AAFilterSize);

					float innerCircle = fill > 0.5 ? 1 : smoothstep(innerMin, innerMax, dist);
					float outterCircle = smoothstep(outerMin, outerMax, dist);
					float circle = innerCircle - outterCircle;

					// make sector
					float sectorP = GET_CROSS_PRODUCT(float2(0, 0), p - o, pixelPos);
					float sectorQ = GET_CROSS_PRODUCT(q - o, float2(0, 0), pixelPos);
					float aafPositive = AAFilterSize * 0.5;
					float aafNegative = AAFilterSize * -0.5;

					float sectorPSmooth = smoothstep(aafNegative, aafPositive, sectorP);
					float sectorQSmooth = smoothstep(aafNegative, aafPositive, sectorQ);

					float sectorPFinal = sectorP > aafNegative ? sectorPSmooth : 0;
					float sectorQFinal = sectorQ > aafNegative ? sectorQSmooth : 0;

					// compute circle and arc
					float _sector = arcAngle > 180 ? min(1, sectorPFinal + sectorQFinal) : sectorPFinal * sectorQFinal;
					_sector = arcAngle >= 360 ? 1 : _sector;

					layer += circle * _sector;
				}

				return layer;
				// code end
            }    
              
			fixed4 frag(fragmentInput i) : SV_Target {

				return fixed4(inColor.r, inColor.g, inColor.b, inColor.a*Circular(i));
			}          
              
			ENDCG
		}
	}
 }