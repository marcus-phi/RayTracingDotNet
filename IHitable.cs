using System;
using System.Collections.Generic;

namespace RayTracingDotNet
{
    public interface IHitable
    {
        HitRecord Hit(Ray r, float tMin, float tMax);
    }

    public class HitRecord
    {
        public bool IsHit { get; set; }
        public float T { get; set; }
        public Vec3 P { get; set; }
        public Vec3 Normal { get; set; }
    }

    public class HitableList : List<IHitable>, IHitable
    {
        public HitRecord Hit(Ray r, float tMin, float tMax)
        {
            var hitRecord = new HitRecord();
            var closestSoFar = tMax;
            foreach(var hitable in this)
            {
                var rec = hitable.Hit(r, tMin, closestSoFar);
                if(rec.IsHit)
                {
                    closestSoFar = hitRecord.T;
                    hitRecord = rec;
                }
            }
            return hitRecord;
        }
    }
}