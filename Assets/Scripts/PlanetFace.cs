using UnityEngine;
using System.Linq;

namespace Ru1t3rl.Planets
{
    public class PlanetFace
    {
        int resolution;
        Vectori localUp;
        Vectori axisA, axisB;

        public Mesh Generate(int resolution, Vectori localUp, bool spherified = true)
        {
            this.localUp = localUp;
            this.resolution = resolution;

            axisA = new Vectori(localUp.y, localUp.z, localUp.x);
            axisB = Vectori.Cross(localUp, axisA);

            Mesh mesh = new Mesh();
            if (spherified)
                ConstructSpherifiedMesh(ref mesh);
            else
                ConstructNormalizedMesh(ref mesh);
            return mesh;
        }

        public void Generate(ref Mesh mesh, int resolution, Vectori localUp, bool spherified = true)
        {
            this.resolution = resolution;
            this.localUp = localUp;

            axisA = new Vectori(localUp.y, localUp.z, localUp.x);
            axisB = Vectori.Cross(localUp, axisA);

            if (spherified)
                ConstructSpherifiedMesh(ref mesh);
            else
                ConstructNormalizedMesh(ref mesh);
        }

        void ConstructNormalizedMesh(ref Mesh mesh)
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
                    Vectori pointOnUnitCube = localUp + axisA * (percent.x - .5f) * 2 + axisB * (percent.y - .5f) * 2;
                    Vectori pointOnUnitSphere = pointOnUnitCube.normalized;
                    vertices[i] = pointOnUnitSphere;

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

        void ConstructSpherifiedMesh(ref Mesh mesh)
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

                    vertices[i] = pointOnUnitSphere;

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