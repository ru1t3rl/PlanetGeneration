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

        [SerializeField] ShapeSettings _shapeSettings;
        public ShapeSettings shapeSettings => _shapeSettings;
        [SerializeField] Gradient albedoGradient, smoothnessGradient, metallicGradient;
        Texture2D albedoTexture, smoothnessTexture, metallicTexture;
        [SerializeField] int stepCount;
        [SerializeField] Vector2Int gradientMapSize;


        [Header("Chunk Stuff")]
        public Vector2Int chunkSize;

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
                    _shapeSettings.noiseLayers,
                    _shapeSettings,
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

        public async void GenerateChunks()
        {
            for (int i = transform.childCount; i-- > 0;)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            meshFilters = new MeshFilter[chunkSize.x * chunkSize.y * 6];
            meshRenderers = new MeshRenderer[meshFilters.Length];
            planetFaces = new PlanetFace[6];

            for (int iFace = 0; iFace < 6; iFace++)
            {
                GameObject face = new GameObject($"Face_{iFace + 1}");
                face.transform.parent = transform;

                planetFaces[iFace] = new PlanetFace();
                Mesh[] chunks = await planetFaces[iFace].GenerateChunksAsync(
                    resolution,
                    new Vectori(directions[iFace].x, directions[iFace].y, directions[iFace].z),
                    _shapeSettings.noiseLayers,
                    _shapeSettings,
                    chunkSize,
                    spherified,
                    iFace
                );

                for (int iChunk = 0; iChunk < chunks.Length; iChunk++)
                {
                    GameObject chunk = new GameObject($"{chunks[iChunk].name}");
                    chunk.transform.SetParent(face.transform);

                    meshFilters[iFace] = chunk.AddComponent<MeshFilter>();
                    meshRenderers[iFace] = chunk.AddComponent<MeshRenderer>();

                    meshFilters[iFace].mesh = chunks[iChunk];
                    meshRenderers[iFace].sharedMaterial = material;

                    chunk.transform.localPosition = Vector3.zero;
                }

                face.transform.localPosition = Vector3.zero;

                min = planetFaces[iFace].min < min ? planetFaces[iFace].min : min;
                max = planetFaces[iFace].max > max ? planetFaces[iFace].max : max;
            }

            material.SetFloat("_Min", min);
            material.SetFloat("_Max", max);

            ApplyGradientTexture();
        }

        #region Shader Setup
        public void ApplyGradientTexture()
        {
            meshRenderers.Select(x => x.sharedMaterial = material);
            material.SetFloat("_Min", min);
            material.SetFloat("_Max", max);
            material.SetFloat("_BaseHeight", _shapeSettings.radius);

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
        #endregion
    }
}