using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ru1t3rl.Noises
{
    [CreateAssetMenu(fileName = "Noise Layer", menuName = "Noise/Noise Layer")]
    public class NoiseLayer : ScriptableObject
    {
        public NoiseSettings noiseSettings;
        public BlendMode blendMode;
        public Texture2D noiseTexture;

        public void GenerateNoise()
        {
            noiseTexture = PerlinNoise.GenerateNoiseTexture(
                noiseSettings.Size.x, noiseSettings.Size.y,
                noiseSettings.Seed.GetHashCode(),
                noiseSettings.NoiseScale,
                noiseSettings.Octaves,
                noiseSettings.Persistance,
                noiseSettings.Lacunarity,
                noiseSettings.Offset,
                noiseSettings.StitchWidth
            );
        }
    }

    [CustomEditor(typeof(NoiseLayer))]
    public class NoiseLayerEditor : Editor
    {
        NoiseLayer layer;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (layer == null)
            {
                layer = (NoiseLayer)target;
            }

            if (GUILayout.Button("Generate Noise"))
            {
                if (layer != null)
                    layer.GenerateNoise();
            }
        }
    }

    public enum BlendMode
    {
        Multiply,
        Additive,
        Substract
    }
}
