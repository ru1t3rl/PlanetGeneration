Shader "Custom/Atmosphere"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _SphereRadius("Sphere Radius", Float) = 1
        _SpherePosition("Sphere Position", vector) = (0, 0, 0, 0)
        _Thickness("Thinkess", float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        float _SphereRadius, _Thickness;
        float3 _SpherePosition;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        bool raySphereIntersectBool(float3 ro, float3 rd, float3 so, float sr) {
            float t = dot(so-ro, rd);
            float3 P = ro + rd*t;
            float y = length(so-P);

            return y <= sr;
        }

        float2 raySphereIntersectBasic(float3 ro, float3 rd, float3 so, float sr) {
            float t = dot(so-ro, rd);
            float3 P = ro + rd*t;
            float y = length(so-P);

            if(y > sr){
                return float2(-1, -1);
            }

            float x = sqrt(sr * sr - y * y);
            float t1 = max(t - x, 0);
            float t2 = t + x;

            return float2(t1, t2);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 so = -(float4(_SpherePosition, 1) * _WorldSpaceCameraPos);
            float2 rsi = raySphereIntersectBasic(_WorldSpaceCameraPos, normalize(IN.viewDir), so, _SphereRadius);

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            c = (1, 1, 1, 1);          
            
            o.Albedo = c.rgb;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            o.Alpha = clamp((rsi.y - rsi.x) * _Thickness, 0, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
