using System;

namespace RayTracingDotNet
{
    public class Metal : IMaterial
    {
        public Metal(Vec3 albedo, float fuzz)
        {
            Albedo = albedo;
            Fuzz = fuzz < 1 ? fuzz : 1.0f;
        }

        public Vec3 Albedo { get; set; }
        public float Fuzz { get; set; }

        public Result<Scatter> Scatter(Ray ray, HitRecord hitRecord)
        {
            var reflected = Utils.Reflect(ray.Direction.UnitVector(), hitRecord.Normal);
            var scattered = new Ray(hitRecord.P, reflected + Fuzz * Utils.RandomInUnitSphere());

            return scattered.Direction.Dot(hitRecord.Normal) > 0 ?
                    new Result<Scatter>(new Scatter
                    {
                        ScatteredRay = scattered,
                        Attenuation = Albedo,
                    }) :
                    new Result<Scatter>();
        }
    }
}