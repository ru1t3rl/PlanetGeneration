using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using System.Text.RegularExpressions;

namespace Ru1t3rl.Noises
{
    public class NoiseFilter
    {
        NoiseSettings settings;
        Noise noise;

        const string ALPHANUMERIC = "123abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVW";

        public NoiseFilter(NoiseSettings settings)
        {
            this.settings = settings;
            noise = new Noise(System.String.IsNullOrEmpty(settings.seed) ? GenerateRandomSeed() : settings.seed.GetHashCode());
        }

        public int GenerateRandomSeed(int length = 12)
        {
            Random rnd;

            string seed = string.Empty;
            for (int i = 0; i < length; i++)
            {
                rnd = new Random(seed.GetHashCode());
                seed += ALPHANUMERIC[rnd.Next(0, ALPHANUMERIC.Length)];
            }

            return seed.GetHashCode();
        }

        public float Evaluate(Vector3 point)
        {
            float noiseValue = 0;
            float frequency = settings.baseRoughness;
            float amplitude = 1;

            for (int i = 0; i < settings.numLayers; i++)
            {
                float v = noise.Evaluate(point * frequency + settings.centre);
                noiseValue += (v + 1) * .5f * amplitude;
                frequency *= settings.roughness;
                amplitude *= settings.persistence;
            }

            //noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
            return noiseValue * settings.strength;
        }
    }
}