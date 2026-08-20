// This shader fills the mesh shape with a color predefined in the code.
Shader "Blit/CameraPixelBlit"
{
    Properties {}

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            ZWrite Off ZTest Always Blend Off Cull Off

            Name "PixelUpscale"

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment FragNearest
            
            uniform float2 _Offset;

            float4 frag(Varyings input) : SV_Target
            {
                #if defined(USE_TEXTURE2D_X_AS_ARRAY) && defined(BLIT_SINGLE_SLICE)
                return SAMPLE_TEXTURE2D_ARRAY_LOD(_BlitTexture, sampler_PointClamp, input.texcoord.xy, _BlitTexArraySlice, _BlitMipLevel);
                #endif

                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                return SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_PointClamp, input.texcoord.xy, _BlitMipLevel);
            }
            ENDHLSL
        }
    }
}