using System;

namespace RayTracingDotNet
{
    public class Lambertian : IMaterial
    {
        public Lambertian(Vec3 albedo)
        {
            Albedo = albedo;
        }

        public Vec3 Albedo { get; set; }

        public Result<Scatter> Scatter(Ray ray, HitRecord hitRecord)
        {
            var target = hitRecord.P + hitRecord.Normal + Utils.RandomInUnitSphere();
            return new Result<Scatter>(new Scatter()
            {
                ScatteredRay = new Ray(hitRecord.P, target - hitRecord.P),
                Attenuation = Albedo,
            });
        }
    }
}