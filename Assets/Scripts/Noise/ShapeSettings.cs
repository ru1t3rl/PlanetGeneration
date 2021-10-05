using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Ru1t3rl.Noises;

namespace Ru1t3rl
{
    [CreateAssetMenu(fileName = "Shape_Settings", menuName = "Noise/Shape Settings")]
    public class ShapeSettings : ScriptableObject
    {
        public float radius = 1;
        public NoiseLayer[] noiseLayers;
    }

    [System.Serializable]
    public class NoiseLayer
    {
        public string name;
        public bool enabled = true;
        public BlendMode blendMode = BlendMode.Multiply;
        public NoiseSettings settings;
    }

    public enum BlendMode
    {
        Multiply,
        Additive,
        Substract
    }
}