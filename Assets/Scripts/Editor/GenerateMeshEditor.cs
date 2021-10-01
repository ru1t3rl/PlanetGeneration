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

            if (GUILayout.Button("Generate"))
            {
                gm.Generate();
            }
        }
    }
}