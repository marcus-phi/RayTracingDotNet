using System;

namespace RayTracingDotNet
{
    public static class Utils
    {
        public static Vec3 RandomInUnitSphere()
        {
            Vec3 p;
            do
            {
                p = 2.0f * new Vec3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()) - new Vec3(1.0f, 1.0f, 1.0f);
            } while (p.SqLength >= 1.0f);
            return p;
        }

        private static Random Rand = new Random();
        public static float NextFloat()
        {
            return (float)Rand.NextDouble();
        }
    }
}