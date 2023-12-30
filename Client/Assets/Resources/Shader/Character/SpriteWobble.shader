Shader "Universal Render Pipeline/FX/Sprite Wobble"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Intensity("Intensity", Range(0.0, 360.0)) = 20.0
        _Speed("Speed", Range(0, 3)) = 0.0
        // [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        // [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        // [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        LOD 100
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment WSpriteFrag
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Intensity;
                int _Speed;
            CBUFFER_END

            // 텍스처 변수 선언
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct appdata
            {
            	float4 vertex : POSITION;
            	float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformWorldToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 WSpriteFrag(v2f IN) : SV_Target
            {
                int speed = round(_Speed);
                float sine = sin((IN.uv.y + abs(_Time[speed]))* _Intensity);
                IN.uv.x += sine * 0.01;
                
                float4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                // c.rgb *= c.a;
                return c;
            }
            ENDHLSL
        }
    }
}