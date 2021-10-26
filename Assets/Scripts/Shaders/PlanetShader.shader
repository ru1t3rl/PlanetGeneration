// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PlanetShader"
{
    Properties
    {
        _Tess ("Tessellation", Range(1, 32)) = 4
        _Phong ("Phong Strengh", Range(0,1)) = 0.5
        _MinDist("Min distance", Range(0, 10)) = 1
        _MaxDist("Max distance", Range(0, 30)) = 20

        [Toggle] _UseEmission("Use Emission", float) = 0
        _MainTex("Cube Map", Cube) = "black" {}
        _CubeMapStrength("Cube Map Strength", Range(0, 1)) = .5
        _GradientAlbedoStrength("Gradient Strength", Range(0, 1)) = .5
        
        _Min ("Min", Float) = 0
        _Max ("Max", Float) = 1
        
        _GradientAlbedo("Gradient Albedo", 2D) = "white" {}
        

        _GradientSmoothness("Smoothness Gradient", 2D) = "grey" {}         
        _Glossiness ("Smoothness Intensity", Range(0,1)) = 0.5   

        _GradientMetallic("Gradient Metallic", 2D) = "grey" {}
        _Metallic ("Metallic Intensity", Range(0,1)) = 1


        _BaseHeight("Base Height", Float) = 0
        _HeightOffset ("Height Offset", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM        
        #pragma surface surf Standard fullforwardshadows tessellate:tessDistance tessphong:_Phong
        #pragma target 5.0
        #include "Tessellation.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 localPos;
        };

        float _UseEmission;
        sampler2D _GradientAlbedo, _GradientSmoothness, _GradientMetallic;
        samplerCUBE _MainTex;
        uniform float _Min = 0, _Max = 0;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _HeightOffset, _BaseHeight, _CubeMapStrength, _GradientAlbedoStrength;

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

        float _Phong, _Tess, _MinDist, _MaxDist;
        float4 tessDistance (appdata_full v0, appdata_full v1, appdata_full v2) {
            float minDist = _MinDist;
            float maxDist = _MaxDist;
            return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 localPos = IN.worldPos -  mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

            _Max += _BaseHeight;

            float height = length(localPos) / (_Max - _Min) + _HeightOffset;  
            float4 color = tex2D(_GradientAlbedo, float2(height, 0));
            float smoothness = tex2D(_GradientSmoothness, float2(height, 0)) * _Glossiness;
            float metallic = tex2D(_GradientMetallic, float2(height, 0)).r * _Metallic;

            float4 color2 =texCUBE(_MainTex, float3(localPos.x, localPos.y, localPos.z)) * _CubeMapStrength + color * _GradientAlbedoStrength;
            o.Albedo = color2;// * height;
            o.Alpha = 1 - height;
            
            if(_UseEmission)
            o.Emission = color2;

            // Metallic and smoothness come from slider variables
            o.Metallic = metallic;
            o.Smoothness = smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
