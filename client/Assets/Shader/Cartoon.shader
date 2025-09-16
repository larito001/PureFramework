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

        _OutlineWidth ("Outline Width", Range(0.01, 1)) = 0.02
        
        _OutLineColor ("OutLine Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
              Name "BASE"
              Tags{ "LightMode" = "SRPDefaultUnlit"}
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
                half f =  1.0 - saturate(dot(viewDir, worldNormal));
                half rim = smoothstep(_RimMin, _RimMax, f);
                rim = smoothstep(0, _RimSmooth, rim);
                half3 rimColor = rim * _RimColor.rgb *  _RimColor.a;

                col.rgb = light.color *( diffuse+rimColor);
                       return col;
            }
            ENDHLSL
        }
         Pass
        {
             Name"OUTLINE"
            Tags{ "LightMode" = "UniversalForward" }
             Cull Front 
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
            };

            Varyings vert(Attributes v)
            {
                Varyings o;

                // 1. 将顶点转换到裁剪空间
                float4 clipPos = TransformObjectToHClip(v.vertex);

                // 2. 将法线转换到观察空间
                float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                
                // 3. 将观察空间的法线转换到裁剪空间
                float3 clipNormal = TransformWViewToHClip(viewNormal);

                // 4. 归一化法线，并考虑透视除法
                float2 offset = normalize(clipNormal.xy);
                
                // 5. 矫正屏幕比例
                float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));
                float aspect = abs(nearUpperRight.y / nearUpperRight.x);
                offset.x *= aspect;

                // 6. 应用描边宽度（这里使用clipPos.w来确保远近一致）
                o.vertex.xy = clipPos.xy + offset * _OutlineWidth * clipPos.w * 0.2;
                o.vertex.z = clipPos.z;
                o.vertex.w = clipPos.w;

                return o;
            }

            half4 frag() : SV_Target
            {
                return _OutLineColor;
            }
            ENDHLSL
        }
    }

}