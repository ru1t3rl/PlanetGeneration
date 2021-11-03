using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ru1t3rl;

namespace Ru1t3rl
{
    public static class ExtensionMethods
    {
        /// <summary> Convert a gradient into a Texture2D </summary>
        /// <param name="stepCount">The amount of steps used to process the gradient</param>
        /// <param name="size">The size of the texture that will be returned</param>
        /// <returns>Returns the gradient as a Texture2D</returns>
        public static Texture2D ToTexture2D(this Gradient g, float stepCount, Vector2Int size)
        {
            Texture2D texture = new Texture2D(size.x, size.y);
            Color value;
            for (int i = 0; i < stepCount; i++)
            {
                value = g.Evaluate(1f / stepCount * i);
                texture.SetPixels(Mathf.FloorToInt(i * (size.x / stepCount)), 0, Mathf.FloorToInt(size.x / stepCount), size.y, new Color[Mathf.FloorToInt(size.x / stepCount) * size.y].Select(x => value).ToArray());
            }
            texture.Apply();
            return texture;
        }

        /// <summary>  Convert a gradient into a Texture2D </summary>
        /// <param name="stepCount">The amount of steps used to process the gradient</param>
        /// <param name="size">The size of the texture that will be returned</param>
        /// <param name="texture">The texture to write the gradient to</param>
        public static void ToTexture2D(this Gradient g, float stepCount, Vector2Int size, out Texture2D texture)
        {
            texture = new Texture2D(size.x, size.y);
            Color value;
            for (int i = 0; i < stepCount; i++)
            {
                value = g.Evaluate(1f / stepCount * i);
                texture.SetPixels(Mathf.FloorToInt(i * (size.x / stepCount)), 0, Mathf.FloorToInt(size.x / stepCount), size.y, new Color[Mathf.FloorToInt(size.x / stepCount) * size.y].Select(x => value).ToArray());
            }
            texture.Apply();
        }
    }
}