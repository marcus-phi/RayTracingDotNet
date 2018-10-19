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

        public static Vec3 RandomInUnitDisk()
        {
            Vec3 p;
            do
            {
                p = 2.0f * new Vec3(NextFloat(), NextFloat(), 0.0f) - new Vec3(1.0f, 1.0f, 0.0f);
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

        public static Result<Vec3> Refract(Vec3 v, Vec3 n, float niOverNt)
        {
            var uv = v.UnitVector();
            var dt = uv.Dot(n);
            var discriminant = 1.0 - niOverNt * niOverNt * (1 - dt * dt);
            return discriminant > 0 ? new Result<Vec3>(niOverNt * (uv - n * dt) - n * (float)Math.Sqrt(discriminant)) : new Result<Vec3>();
        }
    }

    public class Result<T>
        where T : class
    {
        public Result() : this(null) { }

        public Result(T content)
        {
            Content = content;
        }

        public bool IsOK { get { return Content != null; } }
        public T Content { get; private set; }
    }
}