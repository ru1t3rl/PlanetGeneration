using UnityEngine;

namespace Ru1t3rl
{

    // Wrote my own Vector struct to add some extra options for formula's like vector * vector
    public struct Vectori
    {
        public float x, y, z;

        public Vectori(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        #region operators
        public static Vectori operator +(Vectori a, Vectori b) => new Vectori(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vectori operator -(Vectori a, Vectori b) => new Vectori(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vectori operator -(Vectori a)
        {
            a.x *= -1f;
            a.y *= -1f;
            a.z *= -1;
            return a;
        }

        public static Vectori operator *(Vectori a, Vectori b) => new Vectori(a.x * b.x, a.y * b.y, a.z * b.z);
        public static Vectori operator *(Vectori a, float b)
        {
            a.x *= b;
            a.y *= b;
            a.z *= b;
            return a;
        }
        public static Vectori operator /(Vectori a, float b)
        {
            a.x /= b;
            a.y /= b;
            a.z /= b;
            return a;
        }
        #endregion

        public float Magnitude() => Mathf.Sqrt(x * x + y * y + z * z);

        public void Normalize() { this /= Magnitude(); }
        public Vectori normalized => this / Magnitude();

        public static float Dot(Vectori a, Vectori b) => a.x * b.x + a.y * b.y + a.z * b.z;

        public static Vectori Cross(Vectori a, Vectori b) => new Vectori(a.y * b.z - b.y * a.z, -(a.x * b.z - b.x * a.z), a.x * b.y - b.x * a.y);

        public static float Angle(Vectori a, Vectori b) => Mathf.Acos(Dot(a, b) / (a.Magnitude() * b.Magnitude()));

        public static Vectori zero => new Vectori(0, 0, 0);

        public Vector3 ToVector3() => new Vector3(x, y, z);

        public override string ToString() => $"({x}, {y}, {z})";
    }
}