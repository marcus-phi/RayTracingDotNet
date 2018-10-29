using System;

namespace RayTracingDotNet
{
    public class Lambertian : IMaterial
    {
        public Lambertian(ITexture albedo)
        {
            Albedo = albedo;
        }

        public ITexture Albedo { get; private set; }

        public Result<Scatter> Scatter(Ray ray, HitRecord hitRecord)
        {
            var target = hitRecord.P + hitRecord.Normal + Utils.RandomInUnitSphere();
            return new Result<Scatter>(new Scatter()
            {
                ScatteredRay = new Ray(hitRecord.P, target - hitRecord.P),
                Attenuation = Albedo.Value(0, 0, hitRecord.P),
            });
        }
    }
}