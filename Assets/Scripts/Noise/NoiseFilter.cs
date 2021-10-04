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

            noiseTexture = new Texture2D(size.x, size.y);

            SetPixels(noiseLayers[0].noiseTexture, noiseLayers[0].blendMode);
            noiseTexture.Apply();

            for (int iLayer = 1; iLayer < noiseLayers.Length; iLayer++)
            {
                if (noiseLayers[iLayer].noiseTexture != null)
                {
                    SetPixels(noiseLayers[iLayer].noiseTexture, noiseLayers[iLayer].blendMode);
                }
            }

            ReMap(0, 1);
        }

        void ReMap(int min, int max)
        {
            float color;
            for (int y = 0, x; y < size.y; y++)
            {
                for (x = 0; x < size.x; x++)
                {
                    color = Mathf.InverseLerp(min, max, noiseTexture.GetPixel(y, x).r);
                    noiseTexture.SetPixel(y, x, new Color(color, color, color));
                }
            }
            noiseTexture.Apply();
        }

        void SetPixels(Texture2D inputTexture, BlendMode blendMode)
        {
            for (int y = 0, x; y < size.y; y++)
            {
                for (x = 0; x < size.x; x++)
                {
                    try
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
                    catch (System.NullReferenceException)
                    {
                        try
                        {
                            noiseTexture.SetPixel(x, y, inputTexture.GetPixel(x, y));
                        }
                        catch (System.NullReferenceException)
                        {
                            noiseTexture.SetPixel(x, y, Color.black);
                        }
                    }
                }
            }

            noiseTexture.Apply();
        }
    }
}