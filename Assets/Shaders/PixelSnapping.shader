Shader "Custom/PixelSnapSprite"
{
    Properties
    {
        [MainColor] [HDR] _Color("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float2 uv : TEXCOORD0;
                float2 coord : TEXCOORD1;
                float4 positionHCS : SV_POSITION;
            };

            float4 _Color;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 objectOriginWS = unity_ObjectToWorld._m03_m13_m23;
                float4 objectOriginHClip = TransformWorldToHClip(objectOriginWS);
                float2 objectOriginNDC = objectOriginHClip.xy * 0.5f + 0.5f;

                float2 snappedObjectOriginNDC = floor(objectOriginNDC * _ScreenParams.xy) / _ScreenParams.xy;
                float2 snappingDelta = objectOriginNDC - snappedObjectOriginNDC;
                snappingDelta = snappingDelta * 2.0f;
                OUT.coord = snappingDelta;
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS) - float4(snappingDelta.x,snappingDelta.y,0,0);

                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}