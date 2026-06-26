using UnityEngine;

namespace yourvrexperience.Utils
{
    public static class AxisConverter
    {
        public static Vector3 ConvertVector(Vector3 v)
        {
            return new Vector3(v.x, v.z, v.y);
        }

        public static Quaternion ConvertRotation(Quaternion q)
        {
            return new Quaternion(-q.x, -q.z, -q.y, q.w);
        }

        public static Vector3 ConvertScale(Vector3 s)
        {
            return new Vector3(s.x, s.z, s.y);
        }

        public static void ConvertPose(ref Vector3 position, ref Quaternion rotation)
        {
            position = ConvertVector(position);
            rotation = ConvertRotation(rotation);
        }

        public static Matrix4x4 ConvertMatrix(Matrix4x4 m)
        {
            return Swap * m * Swap;
        }

        public static readonly Matrix4x4 Swap = BuildSwap();

        private static Matrix4x4 BuildSwap()
        {
            Matrix4x4 c = Matrix4x4.zero;
            c.m00 = 1f; // X -> X
            c.m12 = 1f; // input Z -> output Y
            c.m21 = 1f; // input Y -> output Z
            c.m33 = 1f;
            return c;
        }

        public static Vector3 ConvertVectorIfNeeded(Vector3 v, bool convert)
        {
            return convert ? ConvertVector(v) : v;
        }

        public static Quaternion ConvertRotationIfNeeded(Quaternion q, bool convert)
        {
            return convert ? ConvertRotation(q) : q;
        }

        public static Vector3 ConvertScaleIfNeeded(Vector3 s, bool convert)
        {
            return convert ? ConvertScale(s) : s;
        }
    }
}