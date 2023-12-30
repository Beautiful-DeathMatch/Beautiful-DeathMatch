Shader "Custom/Outline"
{
    Properties
    {
        [MainTexture] _BaseMap ("Base Map", 2D) = "white" {}
        [MainColor] _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineDistance("Outline Distance", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "universalPipeline" }
        LOD 100

        Pass
        {
            Name "Main"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags {"LightMode" = "Outline"} // ForwardRenderer 애셋의 렌더 피쳐에 Render Objects를 추가한 뒤 Filters > LightMode Tags에 Outline 을 추가하면 추가 패스로 그려짐

            Cull Front

            HLSLPROGRAM
            #pragma vertex vert // 버텍스 쉐이더 진입 함수 이름 설정
            #pragma fragment frag // 픽셀 쉐이더 진입 함수 이름 설정
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;    
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                half _OutlineDistance;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                //OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                // 월드 노말 방식, 카메라 거리에 따른 원근 교정 X
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = mul(UNITY_MATRIX_M, IN.normalOS.xyz);
                positionWS += normalWS * _OutlineDistance;
                OUT.positionHCS = TransformWorldToHClip(positionWS);

                /*
                // 월드 노멀 방식, 카메라 거리에 따른 원근 교정 O
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = mul(UNITY_MATRIX_M, IN.normalOS.xyz);
                float distToCam = length(_WorldSpaceCameraPos - positionWS);
                positionWS += normalWS * _OutlineDistance * distToCam;
                OUT.positionHCS = TransformWorldToHClip(positionWS);

                // 스크린 노멀 방식, 카메라 거리에 따른 원근 교정 O
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                float3 clipNormal = TransformObjectToHClip(IN.normalOS * 100); // 100을 곱하는 이유 : 1 이하로 작은 값인 노말 방향은 클립스페이스의 퍼스펙티브가 적용되면서 화면 바깥쪽에서는 방향이 뒤집히는 왜곡이 발생하므로 클립 변환 전에 방향이 뒤집히지 않을 정도로 충분히 큰 벡터로 가공
                clipNormal = normalize(float3(clipNormal.xy, 0)); // 매우 큰 방향값을 정규화
                OUT.positionHCS.xyz += normalize(clipNormal) * _OutlineDistance * OUT.positionHCS.w; // 클립공간의 w 값은 카메라 공간의 z값과 같다. 즉, 카메라로부터 버택스까지의 거리
                */

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
           
           ENDHLSL
        }
    }
}
