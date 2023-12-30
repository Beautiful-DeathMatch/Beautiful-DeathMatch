Shader "Custom/CartoonLambert"
{
    Properties
    {
        _BaseMap ("_BaseMap", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {        
            Name "CartoonLambert"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 normal       : TEXCOORD1; // Normal
                float3 lightDir     : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
            CBUFFER_END
            
            Varyings vert (Attributes IN)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                o.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                o.normal = TransformObjectToWorldNormal(IN.normalOS);
                o.lightDir = normalize(_MainLightPosition.xyz);
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                i.normal = normalize(i.normal);
                
                float4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                float NdotL = saturate(dot(i.normal, i.lightDir) * 0.5 + 0.5); // * 0.5 + 0.5 (Half Lambert)
                if (NdotL > 0.7)
                {
                   NdotL = 1;
                }
                else
                {
                    NdotL = 0.3;
                }

                float3 ambient = SampleSH(i.normal);
                float3 lighting = NdotL * _MainLightColor.rgb + ambient;
                
                col.rgb *= lighting;
                return col;
            }
            ENDHLSL
        }
    }
}
