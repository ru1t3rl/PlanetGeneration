using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = System.Random;

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
                System.String.IsNullOrEmpty(noiseSettings.Seed) ? RandomSeed().GetHashCode() : noiseSettings.Seed.GetHashCode(),
                noiseSettings.NoiseScale,
                noiseSettings.Octaves,
                noiseSettings.Persistance,
                noiseSettings.Lacunarity,
                noiseSettings.Offset,
                noiseSettings.StitchWidth
            );

            noiseTexture.Apply();
        }

        string RandomSeed(int length = 12)
        {
            Random r = new Random();
            string seed = string.Empty;
            for (int i = 0; i < length; i++)
            {
                seed += System.Convert.ToChar(r.Next(33, 125));
                r = new Random(seed.GetHashCode());
            }

            Debug.Log($"<b>[Noise Layer]</b> Used random seed: {seed}");

            return seed;
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
