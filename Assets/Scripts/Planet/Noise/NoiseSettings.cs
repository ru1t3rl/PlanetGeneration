using UnityEngine;

namespace Ru1t3rl.Noises
{
    [System.Serializable]
    public class NoiseSettings
    {
        [Header("Noise")]
        public NoiseType type = NoiseType.SimpleX;
        public string seed = string.Empty;

        [Header("General")]
        public float strength = 1;
        [Range(1, 8)]
        public int numLayers = 1;
        public float baseRoughness = 1;
        public float roughness = 2;
        public float persistence = .5f;
        public Vector3 centre;
        public float minValue = .1f;

        [Header("Ridgid Noise")]
        public float weightMultiplier = 1;
    }

    public enum NoiseType
    {
        SimpleX,
        Ridgid
    }
}