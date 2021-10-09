using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Ru1t3rl.Noises;
namespace Ru1t3rl.Planets
{
    public class PlanetFace
    {
        int resolution;
        Vectori localUp;
        Vectori axisA, axisB;

        ShapeSettings shapeSettings;
        NoiseLayer[] noiseLayers;

        public float min { get; private set; }
        public float max { get; private set; }

        public Mesh Generate(int resolution, Vectori localUp, NoiseLayer noiseLayer, ShapeSettings shapeSettings, bool spherified = true, int iFace = 0)
        {
            this.localUp = localUp;
            this.resolution = resolution;
            this.noiseLayers = new NoiseLayer[] { noiseLayer };
            this.shapeSettings = shapeSettings;

            axisA = new Vectori(localUp.y, localUp.z, localUp.x);
            axisB = Vectori.Cross(localUp, axisA);

            min = float.MaxValue;
            max = float.MinValue;

            Mesh mesh = new Mesh();
            if (spherified)
                ConstructSpherifiedMesh(ref mesh, iFace);
            else
                ConstructNormalizedMesh(ref mesh, iFace);
            return mesh;
        }

        public Mesh Generate(int resolution, Vectori localUp, NoiseLayer[] noiseLayers, ShapeSettings shapeSettings, bool spherified = true, int iFace = 0)
        {
            this.localUp = localUp;
            this.resolution = resolution;
            this.noiseLayers = noiseLayers;
            this.shapeSettings = shapeSettings;

            axisA = new Vectori(localUp.y, localUp.z, localUp.x);
            axisB = Vectori.Cross(localUp, axisA);

            min = float.MaxValue;
            max = float.MinValue;

            Mesh mesh = new Mesh();
            if (spherified)
                ConstructSpherifiedMesh(ref mesh, iFace);
            else
                ConstructNormalizedMesh(ref mesh, iFace);
            return mesh;
        }

        public async Task<Mesh[]> Generate(int resolution, Vectori localUp, NoiseLayer[] noiseLayers, ShapeSettings shapeSettings, int nChunks, bool spherified = true, int iFace = 0)
        {
            this.localUp = localUp;
            this.resolution = resolution;
            this.noiseLayers = noiseLayers;
            this.shapeSettings = shapeSettings;

            Mesh[] mesh = new Mesh[nChunks * nChunks];

            return mesh;
        }

        public void Generate(ref Mesh mesh, int resolution, Vectori localUp, NoiseLayer noiseLayer, ShapeSettings shapeSettings, bool spherified = true, int iFace = 0)
        {
            this.resolution = resolution;
            this.localUp = localUp;
            this.noiseLayers = new NoiseLayer[] { noiseLayer };
            this.shapeSettings = shapeSettings;

            axisA = new Vectori(localUp.y, localUp.z, localUp.x);
            axisB = Vectori.Cross(localUp, axisA);

            min = float.MaxValue;
            max = float.MinValue;

            if (spherified)
                ConstructSpherifiedMesh(ref mesh, iFace);
            else
                ConstructNormalizedMesh(ref mesh, iFace);
        }

        /// <summary>
        /// Generate a chucnk of the face
        /// </summary>
        /// <param name="iFace">The current face</param>
        /// <param name="cChunk">The current chunk</param>
        /// <param name="nChunks"> The number of chunks</param>
        async Task<Mesh> GenerateMeshNormalizedAsync(int iFace, int cChunck, int nChunks)
        {
            Vectori[] vertices = new Vectori[resolution * resolution];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
            int triIndex = 0;

            for (int y = 0, i = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++, i++)
                {
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vectori pointOnUnitCube = (localUp + axisA * (percent.x - .5f) * 2 + axisB * (percent.y - .5f) * 2) / nChunks * cH;
                    Vectori pointOnUnitSphere = pointOnUnitCube.normalized;
                    vertices[i] = CalculatePointOnSphere(pointOnUnitSphere);

                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;
                        triIndex += 6;
                    }

                    float offsetX = ((localUp.x + localUp.z) + 1) * (1 / 3);
                    float offsetY = localUp.y / 2;

                    uvs[i] = new Vector2(
                        (x / resolution) / 3f + offsetX,
                        (y / resolution) / 2f + offsetY
                    );
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.Select(x => x.ToVector3()).ToArray();
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            return mesh;
        }

        void ConstructNormalizedMesh(ref Mesh mesh, int iFace)
        {
            Vectori[] vertices = new Vectori[resolution * resolution];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
            int triIndex = 0;

            for (int y = 0, i = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++, i++)
                {
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vectori pointOnUnitCube = localUp + axisA * (percent.x - .5f) * 2 + axisB * (percent.y - .5f) * 2;
                    Vectori pointOnUnitSphere = pointOnUnitCube.normalized;
                    vertices[i] = CalculatePointOnSphere(pointOnUnitSphere);

                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;
                        triIndex += 6;
                    }

                    float offsetX = ((localUp.x + localUp.z) + 1) * (1 / 3);
                    float offsetY = localUp.y / 2;

                    uvs[i] = new Vector2(
                        (x / resolution) / 3f + offsetX,
                        (y / resolution) / 2f + offsetY
                    );
                }
            }

            mesh.Clear();

            mesh.vertices = vertices.Select(x => x.ToVector3()).ToArray();
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
        }

        Vectori CalculatePointOnSphere(Vectori basePointOnSphere)
        {
            NoiseFilter filter;
            float elevation = 0;
            int iStart = 0;

            do
            {
                if (iStart < noiseLayers.Length - 1 && noiseLayers[iStart].enabled)
                {
                    filter = new NoiseFilter(noiseLayers[iStart].settings);
                    elevation = filter.Evaluate(basePointOnSphere.ToVector3());
                    elevation = Mathf.Max(0, elevation - noiseLayers[iStart].settings.minValue);
                }
                else
                    iStart++;
            } while (iStart < noiseLayers.Length - 1 && !noiseLayers[iStart].enabled);

            for (int iLayer = iStart; iLayer < noiseLayers.Length; iLayer++)
            {
                if (noiseLayers[iLayer].enabled)
                {
                    filter = new NoiseFilter(noiseLayers[iLayer].settings);

                    switch (noiseLayers[iLayer].blendMode)
                    {
                        case BlendMode.Multiply:
                            elevation *= filter.Evaluate(basePointOnSphere.ToVector3());
                            break;
                        case BlendMode.Additive:
                            elevation += filter.Evaluate(basePointOnSphere.ToVector3());
                            break;
                        case BlendMode.Substract:
                            elevation -= filter.Evaluate(basePointOnSphere.ToVector3());
                            break;
                    }

                    elevation = Mathf.Max(0, elevation - noiseLayers[iLayer].settings.minValue);
                }
            }

            min = elevation < min ? elevation : min;
            max = elevation > max ? elevation : max;

            return basePointOnSphere * shapeSettings.radius * (elevation + 1);
        }

        void ConstructSpherifiedMesh(ref Mesh mesh, int iFace)
        {
            Vectori[] vertices = new Vectori[resolution * resolution];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
            int triIndex = 0;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int i = x + y * resolution;
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vectori p = localUp + axisA * (percent.x - .5f) * 2 + axisB * (percent.y - .5f) * 2;
                    Vectori p2 = p * p;
                    Vectori pointOnUnitSphere = Vectori.zero;
                    pointOnUnitSphere.x = p.x * Mathf.Sqrt(1f - p2.y * .5f - p2.z * .5f + p2.y * p2.z / 3f);
                    pointOnUnitSphere.y = p.y * Mathf.Sqrt(1f - p2.x * .5f - p2.z * .5f + p2.z * p2.x / 3f);
                    pointOnUnitSphere.z = p.z * Mathf.Sqrt(1f - p2.x * .5f - p2.y * .5f + p2.x * p2.y / 3f);

                    vertices[i] = CalculatePointOnSphere(pointOnUnitSphere);

                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;
                        triIndex += 6;
                    }
                }
            }

            mesh.Clear();

            mesh.vertices = vertices.Select(x => x.ToVector3()).ToArray();
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
    }
}