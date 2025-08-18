Shader "AnimationGpuInstancing/Standard"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        _AnimTex("Animation Texture", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        [HideInInspector] [PerRendererData] _StartFrame("", Int) = 0
        [HideInInspector] [PerRendererData] _EndFrame("", Int) = 0
        [HideInInspector] [PerRendererData] _FrameCount("", Int) = 1
        [HideInInspector] [PerRendererData] _OffsetSeconds("", Float) = 0
        [HideInInspector] _PixelCountPerFrame("", Int) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_AnimTex);
            SAMPLER(sampler_AnimTex);

            float4 _MainTex_ST;
            float4 _AnimTex_TexelSize;

            half _Glossiness;
            half _Metallic;
            int _PixelCountPerFrame;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(int, _StartFrame)
                UNITY_DEFINE_INSTANCED_PROP(int, _EndFrame)
                UNITY_DEFINE_INSTANCED_PROP(int, _FrameCount)
                UNITY_DEFINE_INSTANCED_PROP(float, _OffsetSeconds)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 boneIndex : TEXCOORD2;
                float4 boneWeight : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 color : COLOR0;
            };

            float4 GetUV(int index)
            {
                int row = index / (int)_AnimTex_TexelSize.z;
                int col = index % (int)_AnimTex_TexelSize.z;
                return float4(col / _AnimTex_TexelSize.z, row / _AnimTex_TexelSize.w, 0, 0);
            }

            float4x4 GetMatrix(int startIndex, float boneIndex)
            {
                int matrixIndex = startIndex + boneIndex * 3;

                float4 row0 = SAMPLE_TEXTURE2D_LOD(_AnimTex, sampler_AnimTex, GetUV(matrixIndex).xy, 0);
                float4 row1 = SAMPLE_TEXTURE2D_LOD(_AnimTex, sampler_AnimTex, GetUV(matrixIndex + 1).xy, 0);
                float4 row2 = SAMPLE_TEXTURE2D_LOD(_AnimTex, sampler_AnimTex, GetUV(matrixIndex + 2).xy, 0);
                return float4x4(row0, row1, row2, float4(0, 0, 0, 1));
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);

                int startFrame = UNITY_ACCESS_INSTANCED_PROP(Props, _StartFrame);
                int frameCount = UNITY_ACCESS_INSTANCED_PROP(Props, _FrameCount);
                float offsetSeconds = UNITY_ACCESS_INSTANCED_PROP(Props, _OffsetSeconds);

                int offsetFrame = (int)((_Time.y + offsetSeconds) * 30);
                int currentFrame = startFrame + offsetFrame % frameCount;
                int clampedIndex = currentFrame * _PixelCountPerFrame;

                float4x4 bone1Matrix = GetMatrix(clampedIndex, v.boneIndex.x);
                float4x4 bone2Matrix = GetMatrix(clampedIndex, v.boneIndex.y);
                float4x4 bone3Matrix = GetMatrix(clampedIndex, v.boneIndex.z);
                float4x4 bone4Matrix = GetMatrix(clampedIndex, v.boneIndex.w);

                float4 localPos =
                    mul(bone1Matrix, v.positionOS) * v.boneWeight.x +
                    mul(bone2Matrix, v.positionOS) * v.boneWeight.y +
                    mul(bone3Matrix, v.positionOS) * v.boneWeight.z +
                    mul(bone4Matrix, v.positionOS) * v.boneWeight.w;

                o.positionHCS = TransformObjectToHClip(localPos.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 color = texColor * i.color;
                return float4(color.rgb, 1.0); // 强制 alpha 为 1
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
