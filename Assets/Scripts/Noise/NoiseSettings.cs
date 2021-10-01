using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

namespace Ru1t3rl.Noises
{
    [CreateAssetMenu(fileName = "Noise Generation", menuName = "Noise/Perlin")]
    public class NoiseSettings : ScriptableObject
    {
        [Header("Noise settings")]
        public string Seed;
        public Vector2Int Size;
        // The more octaves, the longer generation will take
        public int Octaves;
        [Range(0, 1)]
        public float Persistance;
        public float Lacunarity;
        public float NoiseScale;
        public Vector2 Offset;
    }
}
