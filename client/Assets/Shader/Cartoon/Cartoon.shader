// Upgrade NOTE: commented out 'float3 _WorldSpaceCameraPos', a built-in variable

Shader "Custom/Carton"
{

    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _MainColor("Main Color", Color) = (1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.7, 0.7, 0.8)
        _ShadowRange ("Shadow Range", Range(0, 1)) = 0.5
        _ShadowSmooth("Shadow Smooth", Range(0, 1)) = 0.2

        _RimMin ("RimMin", Range(0, 1)) = 0.5
        _RimMax ("RimMax", Range(0, 1)) = 0.5
        _RimSmooth ("RimSmooth", Range(0, 1)) = 0.5
        _RimColor("RimColor", Color) = (0.5,0.5,0.5,0.7)

        _OutlineWidth ("Outline Width", Range(0.01, 2)) = 0.02

        _OutLineColor ("OutLine Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "BASE"
            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }
            Cull Back
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half3 _MainColor;
            half3 _ShadowColor;
            half _ShadowRange;
            half _ShadowSmooth;

            half _RimMin;
            half _RimMax;
            half _RimSmooth;
            half4 _RimColor;

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };


            Varyings vert(Attributes v)
            {
                // Varyings OUT;
                // OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // return OUT;

                Varyings o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = TransformObjectToHClip(v.vertex);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 col = 1;
                Light light = GetMainLight();
                half3 cameraPos = GetCameraPositionWS();
                half4 mainTex = tex2D(_MainTex, i.uv);

                half3 viewDir = normalize(cameraPos.xyz - i.worldPos.xyz);
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(light.direction.xyz);
                half halfLambert = dot(worldNormal, worldLightDir) * 0.5 + 0.5;
                half ramp = smoothstep(0, _ShadowSmooth, halfLambert - _ShadowRange);
                half3 diffuse = lerp(_ShadowColor, _MainColor, ramp);
                // half3 diffuse = halfLambert > _ShadowRange ? _MainColor : _ShadowColor;
                diffuse *= mainTex;
                //��Ե��
                half f = 1.0 - saturate(dot(viewDir, worldNormal));
                half rim = smoothstep(_RimMin, _RimMax, f);
                rim = smoothstep(0, _RimSmooth, rim);
                half3 rimColor = rim * _RimColor.rgb * _RimColor.a;

                col.rgb = light.color * (diffuse + rimColor);
                return col;
            }
            ENDHLSL
        }
        
    Pass
        {
            Name "OUTLINE"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            Cull Front
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            half _OutlineWidth;
            half4 _OutLineColor;

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            // 基于视距的轮廓宽度调整
            float CalculateOutlineScale(float3 worldPos)
            {
                float distance = length(GetWorldSpaceViewDir(worldPos));
                return _OutlineWidth * (1.0 + distance * 0.05);
            }

            Varyings vert(Attributes v)
            {
                Varyings o;

                // 世界空间位置和法线
                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                float3 worldNormal = TransformObjectToWorldNormal(v.normal);

                // 视图方向
                o.viewDir = GetWorldSpaceViewDir(worldPos);

                // 基于视角的轮廓调整
                float outlineScale = CalculateOutlineScale(worldPos);

                // 使用法线扩展而不是视图空间扩展，获得更稳定的轮廓
                float3 outlineOffset = worldNormal * outlineScale * 0.01;

                // 应用轮廓偏移
                worldPos += outlineOffset;

                // 转换到裁剪空间
                o.vertex = TransformWorldToHClip(worldPos);
                o.worldNormal = worldNormal;
                o.screenPos = ComputeScreenPos(o.vertex);

                return o;
            }

            // 高质量边缘抗锯齿
            float CalculateEdgeAA(float2 screenUV, float depth)
            {
                // 计算屏幕空间导数
                float2 dx = ddx(screenUV);
                float2 dy = ddy(screenUV);
                float gradient = max(length(dx), length(dy));

                // 基于深度和梯度的抗锯齿
                float edgeAA = saturate(1.0 - gradient * 100.0 * _OutlineWidth);
                edgeAA = smoothstep(0.0, 0.3, edgeAA);

                return edgeAA;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // 屏幕空间坐标
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float depth = i.screenPos.z / i.screenPos.w;

                // 高质量抗锯齿
                float alpha = CalculateEdgeAA(screenUV, depth);

                // 基于视角的透明度调整（可选）
                float viewDot = dot(normalize(i.viewDir), normalize(i.worldNormal));
                float viewFactor = saturate(abs(viewDot) * 2.0);
                alpha *= viewFactor;

                // 确保最小透明度
                alpha = max(alpha, 0.3);

                return half4(_OutLineColor.rgb, _OutLineColor.a * alpha);
            }
            ENDHLSL
        }

    }

}