Shader "Custom/AtmospherePostFX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        planetCentre ("Planet Centre", Vector) = (0, 0, 0, 0)
        planetRadius ("Planet Radius", float) = 1
        atmosphereRadius ("Atmosphere Radius", float) = 1

        numInScatteringPoints ("Scattering Points", float) = 2
        numOpticalDepthPoints ("Optical Depth Points", float) = 2
        densityFalloff ("Density Fall off", float) = 1
        exposure ("Exposure", float) = 1
        
        scatteringCoefficients ("Scatter Coefficients", Vector) = (700, 530, 440, 0)

        dirToSun("Dir to sun", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Math.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewVector : TEXCOORD1;
            };

            sampler2D _MainTex, _CameraDepthTexture;
            float4 _MainTex_ST;

            float3 dirToSun;

            float3 planetCentre;
            float planetRadius;
            float atmosphereRadius;

            // Paramaters
            int numInScatteringPoints;
            int numOpticalDepthPoints;
            float4 scatteringCoefficients;
            float densityFalloff;
            float exposure;
            int useExposure;

            float densityAtPoint(float3 densitySamplePoint) {
                float heightAboveSurface = length(densitySamplePoint - planetCentre) - planetRadius;
                float height01 = heightAboveSurface / (atmosphereRadius - planetRadius);
                float localDensity = exp(-height01 * densityFalloff) * (1 - height01);
                return localDensity;
            }
            
            float opticalDepth(float3 rayOrigin, float3 rayDir, float rayLength) {
                float3 densitySamplePoint = rayOrigin;
                float stepSize = rayLength / (numOpticalDepthPoints - 1);
                float opticalDepth = 0;

                for (int i = 0; i < numOpticalDepthPoints; i ++) {
                    float localDensity = densityAtPoint(densitySamplePoint);
                    opticalDepth += localDensity * stepSize;
                    densitySamplePoint += rayDir * stepSize;
                }
                return opticalDepth;
            }

            float3 calculateLight(float3 rayOrigin, float3 rayDir, float rayLength, float3 originalCol) {				
                float3 inScatterPoint = rayOrigin;
                float stepSize = rayLength / (numInScatteringPoints - 1);
                float3 inScatteredLight = 0;
                float viewRayOpticalDepth = 0;

                for (int i = 0; i < numInScatteringPoints; i ++) {
                    float sunRayLength = raySphere(planetCentre, atmosphereRadius, inScatterPoint, dirToSun).y;
                    float sunRayOpticalDepth = opticalDepth(inScatterPoint, dirToSun, sunRayLength);
                    viewRayOpticalDepth = opticalDepth(inScatterPoint, -rayDir, stepSize * i);
                    float3 transmittance = exp(-(sunRayOpticalDepth + viewRayOpticalDepth) * scatteringCoefficients);
                    float localDensity = densityAtPoint(inScatterPoint);
                    
                    inScatteredLight += localDensity * transmittance * scatteringCoefficients * stepSize;;
                    inScatterPoint += rayDir * stepSize;
                }
                float originalColTrans = exp(-viewRayOpticalDepth);                
                return originalCol * originalColTrans + inScatteredLight;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
                // (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv.xy * 2 - 1, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float sceneDephtNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float sceneDepth = LinearEyeDepth(sceneDephtNonLinear) * length(i.viewVector);
                
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float2 hitInfo = raySphere(planetCentre, atmosphereRadius, rayOrigin, rayDir);
                float dstToAtmosphere = hitInfo.x;
                float dstThroughAtmosphere = min(hitInfo.y, sceneDepth - dstToAtmosphere);

                if(dstThroughAtmosphere > 0) {
                    const float epsilon = 0.0001;
                    float3 pointInAtmosphere = rayOrigin + rayDir * (dstToAtmosphere + epsilon);
                    float3 light = calculateLight(pointInAtmosphere, rayDir, dstThroughAtmosphere - epsilon * 2, col);
                    
                    if(useExposure == 1) {
                        return 1 - exp(-exposure * float4(light, exposure));
                        } else if(useExposure == 0) {
                        return float4(light, 0);
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }

    Fallback "Standard/Diffuse"
}
