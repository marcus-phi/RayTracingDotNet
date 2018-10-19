using System;

namespace RayTracingDotNet
{
    public interface IMaterial
    {
        ScatterResult Scatter(Ray ray, HitRecord hitRecord);
    }

    public class ScatterResult
    {
        public bool IsScattered { get; set; }
        public Vec3 Attenuation { get; set; }
        public Ray ScatteredRay { get; set; }
    }
}