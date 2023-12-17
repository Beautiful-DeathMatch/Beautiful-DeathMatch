Shader "Custom/Tree"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" { }
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.pos = TransformObjectToHClip(input.pos.xyz);
                output.uv = input.uv;
                return output;
                
            }
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 frag(Varyings i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                
                // Check if alpha value is less than or equal to 0.1
                if (col.a <= 0.1)
                {
                    discard;
                }
               
                return col;
            }
            ENDHLSL
        }
    }
}