using System;

namespace RayTracingDotNet
{
    public static class Utils
    {
        private static Random Rand = new Random();

        public static Vec3 RandomInUnitSphere()
        {
            Vec3 p;
            do
            {
                p = 2.0f * new Vec3(NextFloat(), NextFloat(), NextFloat()) - new Vec3(1.0f, 1.0f, 1.0f);
            } while (p.SqLength >= 1.0f);
            return p;
        }
        public static float NextFloat()
        {
            return (float)Rand.NextDouble();
        }

        public static Vec3 Reflect(Vec3 v, Vec3 n)
        {
            return v - 2 * v.Dot(n) * n;
        }
    }
}