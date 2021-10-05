using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

namespace Ru1t3rl.Noises
{
    public class PerlinNoise
    {
        /*
        public static float[,,] GenerateNoiseMap3D(Vector3Int mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[,,] noiseMap = new float[mapSize.x, mapSize.y, mapSize.z];
            Random r = new Random(seed);

            #region Octaves Setup
            if (octaves < 1)
                octaves = 1;

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                octaveOffsets[i] = new Vector2(
                    r.Next(-100000, 100000) + offset.x,
                    r.Next(-100000, 100000) + offset.y
                );
            }
            #endregion

            #region Scale Setup
            if (scale <= 0f)
                scale = 0.0001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // When changing noise scale, it zooms from top-right corner
            // We will use this to make it zoom from the center instead
            float halfWidth = mapSize.x / 2f;
            float halfHeight = mapSize.y / 2f;
            float halfDepth = mapSize.z / 2f;
            #endregion

            
        }
        */

        public static float[,] GenerateNoiseMap2D(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[,] noiseMap = new float[mapHeight, mapWidth];
            Random r = new Random(seed);

            #region Octaves Setup
            if (octaves < 1)
                octaves = 1;

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                octaveOffsets[i] = new Vector2(
                    r.Next(-100000, 100000) + offset.x,
                    r.Next(-100000, 100000) + offset.y
                );
            }
            #endregion

            #region Scale Setup
            if (scale <= 0f)
                scale = 0.0001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // When changing noise scale, it zooms from top-right corner
            // We will use this to make it zoom from the center instead
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;
            #endregion

            for (int y = 0, x; y < mapHeight; y++)
            {
                for (x = 0; x < mapWidth; x++)
                {
                    // Define base values
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    // Calculate noise for each octave
                    for (int i = 0; i < octaveOffsets.Length; i++)
                    {
                        // Sample point (x, y)
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfWidth) / scale * frequency + octaveOffsets[i].y;

                        // Use unity's mathf perlin noise
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        // noiseHeight is our final noise
                        noiseHeight += perlinValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    // Find min and max noise height
                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[y, x] = noiseHeight;
                }
            }


            for (int y = 0, x; y < mapHeight; y++)
            {
                for (x = 0; x < mapWidth; x++)
                {
                    noiseMap[y, x] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[y, x]);
                }
            }

            return noiseMap;
        }

        public static float[] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[] noiseMap = new float[mapWidth * mapHeight];
            Random r = new Random(seed);

            #region Octaves Setup
            if (octaves < 1)
                octaves = 1;

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                octaveOffsets[i] = new Vector2(
                    r.Next(-100000, 100000) + offset.x,
                    r.Next(-100000, 100000) + offset.y
                );
            }
            #endregion

            #region Scale Setup
            if (scale <= 0f)
                scale = 0.0001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // When changing noise scale, it zooms from top-right corner
            // We will use this to make it zoom from the center instead
            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;
            #endregion

            for (int x = 0, y; x < mapWidth; x++)
            {
                for (y = 0; y < mapHeight; y++)
                {
                    // Define base values
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    // Calculate noise for each octave
                    for (int i = 0; i < octaveOffsets.Length; i++)
                    {
                        // Sample point (x, y)
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfWidth) / scale * frequency + octaveOffsets[i].y;

                        // Use unity's mathf perlin noise
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        // noiseHeight is our final noise
                        noiseHeight += perlinValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    // Find min and max noise height
                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[y * mapWidth + x] = noiseHeight;
                }
            }

            // Remap noise to be a value between 0f and 1f and return the map
            return noiseMap.Select(x => Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, x)).ToArray();
        }

        public static Texture2D GenerateNoiseTexture(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, int stitchWidth)
        {
            float[] noiseMap = GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset);
            MakeSeamlessHorizontally(noiseMap, mapWidth, mapHeight, stitchWidth);
            MakeSeamlessVertically(noiseMap, mapWidth, mapHeight, stitchWidth);


            Texture2D tex = new Texture2D(mapWidth, mapHeight);
            for (int i = 0; i < noiseMap.Length; i++)
            {
                tex.SetPixel(
                    i % mapWidth,
                    (int)(i / mapWidth),
                    new Color(noiseMap[i], noiseMap[i], noiseMap[i])
                );
            }

            return tex;
        }

        public static void MakeSeamlessHorizontally(float[] noiseMap, int mapWidth, int mapHeight, int stitchWidth)
        {
            int width = mapWidth;
            int height = mapHeight;

            // iterate on the stitch band (on the left
            // of the noise)
            for (int x = 0; x < stitchWidth; x++)
            {
                // get the transparency value from
                // a linear gradient
                float v = x / (float)stitchWidth;
                for (int y = 0; y < height; y++)
                {
                    // compute the "mirrored x position":
                    // the far left is copied on the right
                    // and the far right on the left
                    int o = width - stitchWidth + x;
                    // copy the value on the right of the noise
                    noiseMap[x * stitchWidth + y] = Mathf.Lerp(noiseMap[x * stitchWidth + y], noiseMap[(stitchWidth - x) * stitchWidth + y], v);
                }
            }
        }

        public static void MakeSeamlessVertically(float[] noiseMap, int mapWidth, int mapHeight, int stitchWidth)
        {
            int width = mapWidth;
            int height = mapHeight;

            // iterate through the stitch band (both
            // top and bottom sides are treated
            // simultaneously because its mirrored)
            for (int y = 0; y < stitchWidth; y++)
            {
                // number of neighbour pixels to
                // consider for the average (= kernel size)
                int k = stitchWidth - y;
                // go through the entire row
                for (int x = 0; x < width; x++)
                {
                    // compute the sum of pixel values
                    // in the top and the bottom bands
                    float s1 = 0.0f, s2 = 0.0f;
                    int c = 0;
                    for (int o = x - k; o < x + k; o++)
                    {
                        if (o < 0 || o >= width)
                            continue;
                        s1 += noiseMap[o + stitchWidth * y];
                        s2 += noiseMap[o + (height - (y * stitchWidth) - 1)];
                        c++;
                    }
                    // compute the means and assign them to
                    // the pixels in the top and the bottom
                    // rows
                    noiseMap[y * stitchWidth + x] = s1 / (float)c;
                    noiseMap[x + (height - (y * stitchWidth) - 1)] = s2 / (float)c;
                }
            }
        }
    }
}
