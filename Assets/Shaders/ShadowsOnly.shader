Shader "Custom/ShadowsOnly"
{
    Properties
    {
        _ShadowStrength("Shadow Strength", Range(0, 1)) = 0.5
        _ShadowColor("Shadow Color", Color) = (0, 0, 0, 1)
        _ShadowThreshold("Shadow Threshold", Range(0,1)) = 0.25
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
            };

            float _ShadowStrength;
            float _ShadowThreshold;
            float4 _ShadowColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                float shadowAttenuation = MainLightRealtimeShadow(shadowCoord);

                float shadowAmount = (1.0 - shadowAttenuation) * _ShadowStrength;

                if (shadowAmount <= _ShadowThreshold)
                    discard;

                float4 col = _ShadowColor;
                col.a *= shadowAmount;
                return col;
            }
            ENDHLSL
        }
    }

    FallBack Off
}