Shader "Custom/PlanetShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Min ("Min", Float) = 0
        _Max ("Max", Float) = 1
        _GradientTex("Gradient", 2D) = "white" {}
        _HeightOffset ("Height Offset", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;
        };

        
        sampler2D _MainTex, _GradientTex;
        uniform float _Min = 0, _Max = 0;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _HeightOffset;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        
        float invLerp(float from, float to, float value){
            return (value - from) / (to - from);
        }

        float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
            float rel = invLerp(origFrom, origTo, value);
            return lerp(targetFrom, targetTo, rel);
        }

        float4 Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax)
        {
            return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localPos = v.vertex.xyz;
        }        

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float height = length(IN.localPos) / (_Max - _Min) + _HeightOffset;  
            float4 color = tex2D(_GradientTex, float2(height, 0));
            o.Albedo = color;// * height;
            o.Alpha = 1 - height;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
