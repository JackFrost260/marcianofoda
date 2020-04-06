// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:True,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:34847,y:32332,varname:node_4013,prsc:2|diff-6969-OUT,emission-3883-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32613,y:32640,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:6399,x:32505,y:32923,varname:_node_6399,prsc:2,tex:ded18945800974e4ba3df1479efd63db,ntxv:0,isnm:False|UVIN-5015-OUT,TEX-6323-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:6323,x:31965,y:33132,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ded18945800974e4ba3df1479efd63db,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4383,x:32512,y:33185,varname:_node_4383,prsc:2,tex:ded18945800974e4ba3df1479efd63db,ntxv:0,isnm:False|UVIN-1516-OUT,TEX-6323-TEX;n:type:ShaderForge.SFN_TexCoord,id:6115,x:31891,y:32907,varname:node_6115,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:5015,x:32156,y:32901,varname:node_5015,prsc:2|A-6115-UVOUT,B-3002-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3002,x:32126,y:33271,ptovrint:False,ptlb:Size_First,ptin:_Size_First,varname:_Size_First,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1516,x:32166,y:32741,varname:node_1516,prsc:2|A-6765-OUT,B-6115-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:6765,x:31910,y:32790,ptovrint:False,ptlb:Size_Second,ptin:_Size_Second,varname:_Size_Second,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Lerp,id:7109,x:32838,y:32926,varname:node_7109,prsc:2|A-4383-RGB,B-6399-RGB,T-5245-OUT;n:type:ShaderForge.SFN_Vector1,id:5245,x:32489,y:33136,varname:node_5245,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:9414,x:34169,y:32593,varname:node_9414,prsc:2|A-1304-RGB,B-7109-OUT;n:type:ShaderForge.SFN_Fresnel,id:5963,x:33321,y:32562,varname:node_5963,prsc:2|NRM-1992-OUT,EXP-7389-OUT;n:type:ShaderForge.SFN_NormalVector,id:1992,x:32858,y:32617,prsc:2,pt:False;n:type:ShaderForge.SFN_Add,id:9959,x:34411,y:32682,varname:node_9959,prsc:2|A-9414-OUT,B-3883-OUT;n:type:ShaderForge.SFN_Lerp,id:2037,x:33749,y:32389,varname:node_2037,prsc:2|A-604-OUT,B-8918-RGB,T-323-OUT;n:type:ShaderForge.SFN_Vector1,id:604,x:33155,y:32518,varname:node_604,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:7389,x:32947,y:32748,ptovrint:False,ptlb:Fresnel_Exponent,ptin:_Fresnel_Exponent,varname:_Fresnel_Exponent,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.1,cur:0.7260171,max:10;n:type:ShaderForge.SFN_Tex2d,id:1063,x:33770,y:32903,ptovrint:False,ptlb:Embers,ptin:_Embers,varname:_Embers,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:804834d5cd81bcf4896b8fed1036fde2,ntxv:0,isnm:False|UVIN-4794-UVOUT;n:type:ShaderForge.SFN_Color,id:8918,x:33200,y:32269,ptovrint:False,ptlb:Ambient,ptin:_Ambient,varname:_Ambient,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5790442,c2:0.6617647,c3:0.6412272,c4:1;n:type:ShaderForge.SFN_TexCoord,id:4794,x:33267,y:33104,varname:node_4794,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:3883,x:34159,y:32379,varname:node_3883,prsc:2|A-2037-OUT,B-6923-OUT;n:type:ShaderForge.SFN_Multiply,id:6923,x:33968,y:32784,varname:node_6923,prsc:2|A-323-OUT,B-1063-RGB,C-3834-OUT,D-7462-OUT;n:type:ShaderForge.SFN_Slider,id:3834,x:33920,y:33041,ptovrint:False,ptlb:Embers power,ptin:_Emberspower,varname:_Emberspower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:100;n:type:ShaderForge.SFN_RemapRange,id:5218,x:33434,y:32820,varname:node_5218,prsc:2,frmn:0.9,frmx:1,tomn:0,tomx:1|IN-6399-G;n:type:ShaderForge.SFN_Clamp01,id:7462,x:33669,y:32796,varname:node_7462,prsc:2|IN-5218-OUT;n:type:ShaderForge.SFN_VertexColor,id:7336,x:33949,y:32022,varname:node_7336,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6969,x:34607,y:32069,varname:node_6969,prsc:2|A-7336-RGB,B-447-OUT;n:type:ShaderForge.SFN_Clamp01,id:323,x:33669,y:32594,varname:node_323,prsc:2|IN-5963-OUT;n:type:ShaderForge.SFN_Clamp01,id:447,x:34595,y:32438,varname:node_447,prsc:2|IN-9959-OUT;proporder:1304-6323-3002-6765-7389-8918-1063-3834;pass:END;sub:END;*/

Shader "Almgp/MegaGrunt_mobile" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Size_First ("Size_First", Float ) = 1
        _Size_Second ("Size_Second", Float ) = 10
        _Fresnel_Exponent ("Fresnel_Exponent", Range(0.1, 10)) = 0.7260171
        _Ambient ("Ambient", Color) = (0.5790442,0.6617647,0.6412272,1)
        _Embers ("Embers", 2D) = "white" {}
        _Emberspower ("Embers power", Range(0, 100)) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Size_First;
            uniform float _Size_Second;
            uniform float _Fresnel_Exponent;
            uniform sampler2D _Embers; uniform float4 _Embers_ST;
            uniform float4 _Ambient;
            uniform float _Emberspower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - 0;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float2 node_1516 = (_Size_Second*i.uv0);
                float4 _node_4383 = tex2D(_MainTex,TRANSFORM_TEX(node_1516, _MainTex));
                float2 node_5015 = (i.uv0*_Size_First);
                float4 _node_6399 = tex2D(_MainTex,TRANSFORM_TEX(node_5015, _MainTex));
                float node_604 = 0.0;
                float node_5963 = pow(1.0-max(0,dot(i.normalDir, viewDirection)),_Fresnel_Exponent);
                float node_323 = saturate(node_5963);
                float4 _Embers_var = tex2D(_Embers,TRANSFORM_TEX(i.uv0, _Embers));
                float3 node_3883 = (lerp(float3(node_604,node_604,node_604),_Ambient.rgb,node_323)+(node_323*_Embers_var.rgb*_Emberspower*saturate((_node_6399.g*9.999998+-8.999998))));
                float3 diffuseColor = (i.vertexColor.rgb*saturate(((_Color.rgb*lerp(_node_4383.rgb,_node_6399.rgb,0.5))+node_3883)));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = node_3883;
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Size_First;
            uniform float _Size_Second;
            uniform float _Fresnel_Exponent;
            uniform sampler2D _Embers; uniform float4 _Embers_ST;
            uniform float4 _Ambient;
            uniform float _Emberspower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float2 node_1516 = (_Size_Second*i.uv0);
                float4 _node_4383 = tex2D(_MainTex,TRANSFORM_TEX(node_1516, _MainTex));
                float2 node_5015 = (i.uv0*_Size_First);
                float4 _node_6399 = tex2D(_MainTex,TRANSFORM_TEX(node_5015, _MainTex));
                float node_604 = 0.0;
                float node_5963 = pow(1.0-max(0,dot(i.normalDir, viewDirection)),_Fresnel_Exponent);
                float node_323 = saturate(node_5963);
                float4 _Embers_var = tex2D(_Embers,TRANSFORM_TEX(i.uv0, _Embers));
                float3 node_3883 = (lerp(float3(node_604,node_604,node_604),_Ambient.rgb,node_323)+(node_323*_Embers_var.rgb*_Emberspower*saturate((_node_6399.g*9.999998+-8.999998))));
                float3 diffuseColor = (i.vertexColor.rgb*saturate(((_Color.rgb*lerp(_node_4383.rgb,_node_6399.rgb,0.5))+node_3883)));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Size_First;
            uniform float _Size_Second;
            uniform float _Fresnel_Exponent;
            uniform sampler2D _Embers; uniform float4 _Embers_ST;
            uniform float4 _Ambient;
            uniform float _Emberspower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_604 = 0.0;
                float node_5963 = pow(1.0-max(0,dot(i.normalDir, viewDirection)),_Fresnel_Exponent);
                float node_323 = saturate(node_5963);
                float4 _Embers_var = tex2D(_Embers,TRANSFORM_TEX(i.uv0, _Embers));
                float2 node_5015 = (i.uv0*_Size_First);
                float4 _node_6399 = tex2D(_MainTex,TRANSFORM_TEX(node_5015, _MainTex));
                float3 node_3883 = (lerp(float3(node_604,node_604,node_604),_Ambient.rgb,node_323)+(node_323*_Embers_var.rgb*_Emberspower*saturate((_node_6399.g*9.999998+-8.999998))));
                o.Emission = node_3883;
                
                float2 node_1516 = (_Size_Second*i.uv0);
                float4 _node_4383 = tex2D(_MainTex,TRANSFORM_TEX(node_1516, _MainTex));
                float3 diffColor = (i.vertexColor.rgb*saturate(((_Color.rgb*lerp(_node_4383.rgb,_node_6399.rgb,0.5))+node_3883)));
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
