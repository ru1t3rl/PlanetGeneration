using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ru1t3rl.Noises
{
    [System.Serializable]
    public class NoiseFilter
    {
        public Vector2Int size;
        public NoiseLayer[] noiseLayers;
        public Texture2D noiseTexture { get; private set; }

        public void CombineLayers()
        {
            if (noiseLayers.Length <= 0)
                return;

            SetPixels(noiseLayers[0].noiseTexture, noiseLayers[0].blendMode);

            for (int iLayer = 1; iLayer < noiseLayers.Length; iLayer++)
            {
                noiseLayers[iLayer].GenerateNoise();
                if (noiseLayers[iLayer].noiseTexture != null)
                {
                    SetPixels(noiseLayers[iLayer].noiseTexture, noiseLayers[iLayer].blendMode);
                }
            }
        }

        void SetPixels(Texture2D inputTexture, BlendMode blendMode)
        {
            for (int y = 0, x; y < size.y; y++)
            {
                for (x = 0; x < size.x; x++)
                {
                    switch (blendMode)
                    {
                        case BlendMode.Multiply:
                            noiseTexture.SetPixel(x, y, noiseTexture.GetPixel(x, y) * inputTexture.GetPixel(x, y));
                            break;
                        case BlendMode.Additive:
                            noiseTexture.SetPixel(x, y, noiseTexture.GetPixel(x, y) + inputTexture.GetPixel(x, y));
                            break;
                        case BlendMode.Substract:
                            noiseTexture.SetPixel(x, y, noiseTexture.GetPixel(x, y) - inputTexture.GetPixel(x, y));
                            break;
                    }
                }
            }

            noiseTexture.Apply();
        }
    }
}