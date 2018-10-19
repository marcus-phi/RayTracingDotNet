using System;

namespace RayTracingDotNet
{
    public interface IMaterial
    {
        Result<Scatter> Scatter(Ray ray, HitRecord hitRecord);
    }

    public class Scatter
    {
        public Vec3 Attenuation { get; set; }
        public Ray ScatteredRay { get; set; }
    }
}