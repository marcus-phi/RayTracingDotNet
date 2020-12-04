using System;

namespace RayTracingDotNet
{
    public class Dielectric : IMaterial
    {
        public Dielectric(float refIdx)
        {
            RefIdx = refIdx;
        }

        public float RefIdx { get; set; }

        public Result<Scatter> Scatter(Ray ray, HitRecord hitRecord)
        {
            var outwardNormal = new Vec3();
            var reflected = Utils.Reflect(ray.Direction, hitRecord.Normal);
            var niOverNt = 0.0f;
            var scatter = new Scatter() { Attenuation = new Vec3(1.0f, 1.0f, 1.0f) };
            var reflectProb = 0.0f;
            var cosine = 0.0f;
            if (ray.Direction.Dot(hitRecord.Normal) > 0)
            {
                outwardNormal = -hitRecord.Normal;
                niOverNt = RefIdx;
                cosine = ray.Direction.Dot(hitRecord.Normal) / ray.Direction.Length;
            }
            else
            {
                outwardNormal = hitRecord.Normal;
                niOverNt = 1.0f / RefIdx;
                cosine = -ray.Direction.Dot(hitRecord.Normal) / ray.Direction.Length;
            }

            var refractResult = Utils.Refract(ray.Direction, outwardNormal, niOverNt);
            reflectProb = refractResult.IsOK ? Schlick(cosine, RefIdx) : 1.0f;
            scatter.ScatteredRay = Utils.NextFloat() < reflectProb ? new Ray(hitRecord.P, reflected) : new Ray(hitRecord.P, refractResult.Content);

            return new Result<Scatter>(scatter);
        }

        private static float Schlick(float cosine, float refIdx)
        {
            var r0 = (1 - refIdx) / (1 + refIdx);
            r0 = r0 * r0;
            return r0 + (1 - r0) * (float)Math.Pow(1 - cosine, 5);
        }
    }
}