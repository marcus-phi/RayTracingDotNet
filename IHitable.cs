using System;
using System.Collections.Generic;

namespace RayTracingDotNet
{
    public interface IHitable
    {
        Result<HitRecord> Hit(Ray r, float tMin, float tMax);
    }

    public class HitRecord
    {
        public float T { get; set; }
        public Vec3 P { get; set; }
        public Vec3 Normal { get; set; }
        public IMaterial Material { get; set; }
    }

    public class HitableList : List<IHitable>, IHitable
    {
        public Result<HitRecord> Hit(Ray r, float tMin, float tMax)
        {
            var hitResult = new Result<HitRecord>();
            var closestSoFar = tMax;
            foreach (var hitable in this)
            {
                var temp = hitable.Hit(r, tMin, closestSoFar);
                if (temp.IsOK)
                {
                    closestSoFar = temp.Content.T;
                    hitResult = new Result<HitRecord>(temp.Content);
                }
            }
            return hitResult;
        }
    }
}