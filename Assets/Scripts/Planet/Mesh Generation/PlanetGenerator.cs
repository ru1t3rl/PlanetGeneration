using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ru1t3rl.Planets;
using System.Threading.Tasks;
using System.Linq;

namespace Ru1t3rl
{
    public class PlanetGenerator : MonoBehaviour
    {
        const int FACE_COUNT = 6;

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

        void OnValidate()
        {
            ApplyGradientTexture();
        }

        void Awake()
        {
            ApplyGradientTexture();
        }

        public void GenerateMesh()
        {
            Vectori test = new Vectori(10, 20, 1);
            Vectori test2 = new Vectori(5, 10, 60);

            if (meshFilters == null || meshFilters.Length == 0)
            {
                meshFilters = new MeshFilter[FACE_COUNT];
                meshRenderers = new MeshRenderer[FACE_COUNT];

                for (int i = transform.childCount; i-- > 0;)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }

            planetFaces = new PlanetFace[meshFilters.Length];

            for (int i = 0; i < FACE_COUNT; i++)
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
            // Destroy Possible previous chunks
            for (int i = transform.childCount; i-- > 0;)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            meshFilters = new MeshFilter[chunkSize.x * chunkSize.y * FACE_COUNT];
            meshRenderers = new MeshRenderer[meshFilters.Length];
            planetFaces = new PlanetFace[FACE_COUNT];

            // Generate Faces
            for (int iFace = 0, i = 0; iFace < FACE_COUNT; iFace++)
            {
                GameObject face = new GameObject($"Face_{iFace + 1}");
                face.transform.parent = transform;

                // Generate Chunks
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

                // Instantiate chunks using the generated meshes
                for (int iChunk = 0; iChunk < chunks.Length; iChunk++, i++)
                {
                    GameObject chunk = new GameObject($"{chunks[iChunk].name}");
                    chunk.transform.SetParent(face.transform);

                    meshFilters[i] = chunk.AddComponent<MeshFilter>();
                    meshRenderers[i] = chunk.AddComponent<MeshRenderer>();

                    MeshCollider collider = chunk.AddComponent<MeshCollider>();
                    collider.sharedMesh = chunks[iChunk];
                    collider.convex = true;

                    meshFilters[i].mesh = chunks[iChunk];
                    meshRenderers[i].sharedMaterial = material;

                    chunk.transform.localPosition = Vector3.zero;
                }

                face.transform.localPosition = Vector3.zero;

                min = planetFaces[iFace].min < min ? planetFaces[iFace].min : min;
                max = planetFaces[iFace].max > max ? planetFaces[iFace].max : max;
            }

            material.SetFloat("_Min", 0);
            material.SetFloat("_Max", 1);

            ApplyGradientTexture();
        }

        #region Shader Setup
        public async void ApplyGradientTexture()
        {
            try
            {
                if (meshRenderers == null)
                    meshRenderers = await GatherMeshRenderers(this.transform);

                meshRenderers.Select(x => x.sharedMaterial = material);

                material.SetFloat("_Min", 0);
                material.SetFloat("_Max", 1);
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
            catch (System.ArgumentNullException ex)
            {
                Debug.LogError($"<b>[Planet Generator - {gameObject.name}]</b> {ex.ParamName} was null\nStack Trace:{ex.StackTrace}");
            }
        }

        public void SaveGradientTexture()
        {
            albedoTexture = albedoGradient.ToTexture2D(stepCount, gradientMapSize);
            smoothnessTexture = smoothnessGradient.ToTexture2D(stepCount, gradientMapSize);
            metallicTexture = metallicGradient.ToTexture2D(stepCount, gradientMapSize);
        }

        /// <summary>
        ///  Get the mesh renderers of all children recursive async
        /// </summary>
        /// <param name="transform">The transform to search it's children</param>
        /// <return>The mesh renderers present in all the childeren</return>
        async Task<MeshRenderer[]> GatherMeshRenderers(Transform transform)
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            Task<MeshRenderer[]>[] childTasks = new Task<MeshRenderer[]>[transform.childCount];

            for (int iTask = 0; iTask < childTasks.Length; iTask++)
            {
                childTasks[iTask] = GatherMeshRenderers(transform.GetChild(iTask));
            }

            for (int iChild = 0; iChild < transform.childCount; iChild++)
            {
                renderers.Add(transform.GetChild(iChild).GetComponent<MeshRenderer>());
            }

            await Task.WhenAll(childTasks);

            for (int iTask = 0; iTask < childTasks.Length; iTask++)
            {
                renderers.AddRange(childTasks[iTask].Result);
            }

            return renderers.ToArray();
        }

        #endregion
    }
}