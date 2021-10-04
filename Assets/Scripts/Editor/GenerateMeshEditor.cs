using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ru1t3rl
{
    [CustomEditor(typeof(GenerateMesh))]
    public class GenerateMeshEditor : Editor
    {
        GenerateMesh gm;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (gm == null)
                gm = (GenerateMesh)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Combine Noise"))
            {
                gm.CombineNoise();
            }
            if (GUILayout.Button("Apply Noise"))
            {
                gm.ApplyNoise();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Generate Mesh"))
            {
                gm.GenerateMeshes();
            }
        }
    }
}