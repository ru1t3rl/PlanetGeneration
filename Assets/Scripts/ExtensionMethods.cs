using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ru1t3rl;

namespace Ru1t3rl
{
    public static class ExtensionMethods
    {
        public static Texture2D ToTexture2D(this Gradient g, float stepCount, Vector2Int size)
        {
            Texture2D texture = new Texture2D(size.x, size.y);
            Color value;
            for (int i = 0; i < stepCount; i++)
            {
                value = g.Evaluate(1f / stepCount * i);
                texture.SetPixels(Mathf.FloorToInt(i * (size.x / stepCount)), 0, Mathf.FloorToInt(size.x / stepCount), size.y, new Color[size.x * size.y].Select(x => value).ToArray());
            }
            texture.Apply();
            return texture;
        }
    }
}