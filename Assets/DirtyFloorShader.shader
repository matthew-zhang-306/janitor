Shader "DirtyFloorShader"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _SubTex("SubMain?", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _PerlinFreq("Frequency of Perlin", float) = 1
        _PerlinMax("Max alter from Perlin", float) = 0.4
        _PerlinMin("Min alter from Perlin", float) = 0

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    ENDHLSL

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float4  color       : COLOR;
                float2	uv          : TEXCOORD0;
                float2	lightingUV  : TEXCOORD1;
                float2  worldPos    : TEXCOORD2;
                float3x3  tileuv      : TEXCOORD3;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_SubTex);
            SAMPLER(sampler_SubTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);



            half4 _MainTex_ST;
            half4 _NormalMap_ST;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            uniform float _PerlinFreq;
            uniform float _PerlinMax;
            uniform float _PerlinMin;

            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289 + _PerlinFreq;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p)
            {
                float2 ip = floor(p * _PerlinFreq);
                float2 fp = frac(p * _PerlinFreq);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 clipVertex = o.positionCS / o.positionCS.w;
                o.lightingUV = ComputeScreenPos(clipVertex).xy;
                o.worldPos = float2 (mul (unity_ObjectToWorld, v.positionOS).xy);
                
                o.color = v.color;  
                
                uint4 convert = floor(256 * v.color);
                
                o.tileuv[0][1] = float ((convert.x) & 3);
                
                //Cardinals
                o.tileuv[1][0] = float((convert.y) & 3);
                o.tileuv[1][1] = 0;//(convert.y >> 4) & 3;
                o.tileuv[1][2] = float((convert.z >> 4) & 3);
                o.tileuv[2][1] = float((convert.w >> 4) & 3);

                //Diagonals
                //Note diagonals have slight effects from cardinals to prevent sticking out
                o.tileuv[0][2] = float ((convert.y >> 4) & 3);
                o.tileuv[2][2] = float ((convert.w) & 3);
                o.tileuv[0][0] = float ((convert.x >> 4) & 3);
                o.tileuv[2][0] = float ((convert.z) & 3);
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                // return i.color;
                static const float ratio = ((1.0 / 3) * 0.65);
                half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv); // + float4(0, clamp (, 0, 1) , 0,0);
                i.tileuv[1][1] = ceil (main.w / ratio);
                float2 suv = (i.uv * 6 - floor (i.uv * 6));

                // if ((suv.x - 0.5) * (suv.x - 0.5) + (suv.y - 0.5) * (suv.y - 0.5) <= 0.05 ) {
                //     return float4(1,0,0,1);
                // }

                int cx = suv.x > 0.5;
                int cy = suv.y > 0.5;

                float bl = (i.tileuv[cx][cy] + i.tileuv[1][1]) / 2.0;
                float tl = (i.tileuv[cx][cy+1] + i.tileuv[1][1]) / 2.0;
                float br = (i.tileuv[cx+1][cy] + i.tileuv[1][1]) / 2.0;
                float tr = (i.tileuv[cx+1][cy+1] + i.tileuv[1][1]) /2.0;

                // if (i.tileuv[1][1] == 51) {
                //     return float4 (0,1,0,1);    
                // }
                
                suv = suv * 2 - floor (suv * 2);
                float value = lerp (lerp (bl, br, suv.x), lerp (tl, tr, suv.x), suv.y);
                // return float4 (value * ratio, 0, 0, 1);
                //float4 mul = float4 (1,1,1,value);
                main.w = value * ratio;
                // main.w = main.w > value * ratio  ? lerp (main.w, value * ratio, 0) : value * ratio;
                // main.y = clamp (main.y + clamp (unity_gradientNoise(i.worldPos), _PerlinMin , 1) * _PerlinMax, 0, 1);
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                return CombinedShapeLightShared(main, mask, i.lightingUV);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
                float4 tangent      : TANGENT;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
                float3  normalWS		: TEXCOORD1;
                float3  tangentWS		: TEXCOORD2;
                float3  bitangentWS		: TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalMap_ST;  // Is this the right way to do this?

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
                o.uv = attributes.uv;
                o.color = attributes.color;
                o.normalWS = TransformObjectToWorldDir(float3(0, 0, -1));
                o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            float4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));
                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
            }
            ENDHLSL
        }
        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color		: COLOR;
                float2 uv			: TEXCOORD0;
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float4  color			: COLOR;
                float2	uv				: TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;

                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.uv = attributes.uv;
                o.color = attributes.color;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return mainTex;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
