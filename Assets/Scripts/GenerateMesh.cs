using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ru1t3rl.Planets;
using Ru1t3rl.Noises;

namespace Ru1t3rl
{
    public class GenerateMesh : MonoBehaviour
    {
        [Range(2, 256)]
        public int resolution = 10;
        [SerializeField] bool spherified = true;
        [SerializeField] Material material;

        MeshFilter[] meshFilters;
        PlanetFace[] planetFaces;

        Vector3[] directions = { Vector3.up, Vector3.down,
                                 Vector3.left, Vector3.right,
                                 Vector3.forward, Vector3.back};

        [Header("Noise Settings")]
        [SerializeField] Texture2D noiseTexture;
        [SerializeField] NoiseFilter noiseFilter;

        public void GenerateMeshes()
        {
            Vectori test = new Vectori(10, 20, 1);
            Vectori test2 = new Vectori(5, 10, 60);

            if (meshFilters == null || meshFilters.Length == 0)
            {
                meshFilters = new MeshFilter[6];

                for (int i = transform.childCount; i-- > 0;)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }

            planetFaces = new PlanetFace[meshFilters.Length];

            for (int i = 0; i < 6; i++)
            {
                if (meshFilters[i] == null)
                {
                    GameObject face = new GameObject($"Face_{i + 1}");
                    face.transform.parent = transform;
                    face.AddComponent<MeshRenderer>().sharedMaterial = material;
                    meshFilters[i] = face.AddComponent<MeshFilter>();
                }

                planetFaces[i] = new PlanetFace();
                meshFilters[i].sharedMesh = planetFaces[i].Generate(
                    resolution,
                    new Vectori(directions[i].x, directions[i].y, directions[i].z),
                    spherified
                );
            }
        }

        public void CombineNoise()
        {
            noiseFilter.CombineLayers();
            noiseTexture = noiseFilter.noiseTexture;
        }

        public void ApplyNoise()
        {
            material.SetTexture("_MainTex", noiseTexture);

            for (int iFace = 0; iFace < 6; iFace++)
            {
                int vertices = meshFilters[0].mesh.vertices.Length * 6;
            }
        }
    }
}