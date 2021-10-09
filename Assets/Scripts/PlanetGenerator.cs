using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ru1t3rl.Planets;
using Ru1t3rl.Noises;
using System.Linq;

namespace Ru1t3rl
{
    public class PlanetGenerator : MonoBehaviour
    {
        [Range(2, 256)]
        public int resolution = 10;
        [SerializeField] bool spherified = true;
        [SerializeField] Material material;

        MeshFilter[] meshFilters;
        MeshRenderer[] meshRenderers;
        PlanetFace[] planetFaces;

        Vector3[] directions = { Vector3.up, Vector3.down,
                                 Vector3.left, Vector3.right,
                                 Vector3.forward, Vector3.back};

        [SerializeField] ShapeSettings shapeSettings;
        [SerializeField] Gradient albedoGradient, smoothnessGradient, metallicGradient;
        Texture2D albedoTexture, smoothnessTexture, metallicTexture;
        [SerializeField] int stepCount;
        [SerializeField] Vector2Int gradientMapSize;

        ShapeSettings prevShapeSettings;

        float min, max;

        public void GenerateMesh()
        {

            Vectori test = new Vectori(10, 20, 1);
            Vectori test2 = new Vectori(5, 10, 60);

            if (meshFilters == null || meshFilters.Length == 0)
            {
                meshFilters = new MeshFilter[6];
                meshRenderers = new MeshRenderer[6];

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
                    meshRenderers[i] = face.AddComponent<MeshRenderer>();
                    meshRenderers[i].sharedMaterial = material;
                    meshFilters[i] = face.AddComponent<MeshFilter>();
                }

                planetFaces[i] = new PlanetFace();
                meshFilters[i].sharedMesh = planetFaces[i].Generate(
                    resolution,
                    new Vectori(directions[i].x, directions[i].y, directions[i].z),
                    shapeSettings.noiseLayers,
                    shapeSettings,
                    spherified,
                    i
                );

                meshFilters[i].transform.localPosition = Vector3.zero;

                min = planetFaces[i].min < min ? planetFaces[i].min : min;
                max = planetFaces[i].max > max ? planetFaces[i].max : max;
            }

            material.SetFloat("_Min", min);
            material.SetFloat("_Max", max);

            ApplyGradientTexture();
        }

        public void ApplyGradientTexture()
        {
            meshRenderers.Select(x => x.sharedMaterial = material);
            material.SetFloat("_Min", min);
            material.SetFloat("_Max", max);
            material.SetFloat("_BaseHeight", shapeSettings.radius);

            material.SetTexture("_GradientAlbedo", albedoTexture != null
                ? albedoTexture
                : albedoGradient.ToTexture2D(stepCount, gradientMapSize));

            material.SetTexture("_GradientSmoothness", smoothnessTexture != null
                ? smoothnessTexture
                : smoothnessGradient.ToTexture2D(stepCount, gradientMapSize));

            material.SetTexture("_GradientMetallic", metallicTexture != null
                ? metallicTexture
                : metallicGradient.ToTexture2D(stepCount, gradientMapSize));
        }

        public void SaveGradientTexture()
        {
            albedoTexture = albedoGradient.ToTexture2D(stepCount, gradientMapSize);
            smoothnessTexture = smoothnessGradient.ToTexture2D(stepCount, gradientMapSize);
            metallicTexture = metallicGradient.ToTexture2D(stepCount, gradientMapSize);
        }
    }
}