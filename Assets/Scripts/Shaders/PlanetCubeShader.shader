// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PlanetCubeShader"
{
    Properties 
    {
        _Tess ("Tessellation", Range(1, 32)) = 4
        _MinDist("Min distance", Range(0, 10)) = 1
        _MaxDist("Max distance", Range(0, 30)) = 20
        _CubeMap ("Cube Map", Cube) = "white" {}
    }
    SubShader 
    {
        Pass 
        {
            Tags { "DisableBatching"="True" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma tessellate tessDistance
            #include "UnityCG.cginc"
            #include "Tessellation.cginc"
            
            samplerCUBE _CubeMap;
            
            struct v2f {
                float4 pos : SV_Position;
                half3 uv : TEXCOORD0;
            };
            
            float _Tess, _MinDist, _MaxDist;
            float4 tessDistance (appdata_full v0, appdata_full v1, appdata_full v2) {
                float minDist = _MinDist;
                float maxDist = _MaxDist;
                return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
            }

            v2f vert (appdata_img v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                o.uv = v.vertex.xyz * half3(-1,1,1); // mirror so cubemap projects as expected
                v.vertex += length(texCUBE(_CubeMap, o.uv));
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                return texCUBE(_CubeMap, i.uv);
            }
            ENDCG
        }
    }
}
