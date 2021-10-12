using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ru1t3rl
{
    [CustomEditor(typeof(PlanetGenerator))]
    public class GeneratePlanetEditor : Editor
    {
        PlanetGenerator gm;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (gm == null)
                gm = (PlanetGenerator)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Gradient"))
            {
                gm.SaveGradientTexture();
            }
            if (GUILayout.Button("Apply Gradient"))
            {
                gm.ApplyGradientTexture();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Generate Mesh"))
            {
                gm.GenerateMesh();
            }

            if (GUILayout.Button("Generate Chunks"))
            {
                gm.GenerateChunks();
            }
        }
    }
}