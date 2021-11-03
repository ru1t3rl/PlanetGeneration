using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ru1t3rl.Planets.Atmos
{
    [CreateAssetMenu(menuName = "Planets/Atmosphere", fileName = "Atmosphere_Settings")]
    public class AtmosphereSettings : ScriptableObject
    {
        public int inScatterPoints = 10;
        public int opticalDepthPoints = 10;
        public float densityFallOff = 4;
        public float atmosphereScale = 1;
        public Vector3 waveLengths = new Vector3(700, 530, 440);
        public float scatteringStrength = 1;
        public bool useExposure = true;
        public float exposure = 1f;


        public void SetProperties(ref Material material, float bodyRadius)
        {
            float scatterR = Mathf.Pow(400 / waveLengths.x, 4);
            float scatterG = Mathf.Pow(400 / waveLengths.y, 4);
            float scatterB = Mathf.Pow(400 / waveLengths.z, 4);
            Vector3 scatteringCoef = new Vector3(scatterR, scatterB, scatterG) * scatteringStrength;

            material.SetVector("scatteringCoefficients", scatteringCoef);
            material.SetInt("numInScatteringPoints", inScatterPoints);
            material.SetInt("numOpticalDepthPoints", opticalDepthPoints);
            material.SetFloat("atmosphereRadius", (1 + atmosphereScale) * bodyRadius);
            material.SetFloat("planetRadius", bodyRadius);
            material.SetFloat("densityFalloff", densityFallOff);
            material.SetFloat("exposure", exposure);
            material.SetInt("useExposure", useExposure ? 1 : 0);
        }
    }
}