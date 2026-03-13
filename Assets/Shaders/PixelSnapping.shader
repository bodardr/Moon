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
            #include "PixelUtility.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            float4 _Color;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.uv = IN.uv;
                OUT.positionHCS = SnapToPixelCoords(IN.positionOS, unity_ObjectToWorld);

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