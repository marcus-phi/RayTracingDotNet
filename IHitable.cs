using System;
using System.Collections.Generic;

namespace RayTracingDotNet
{
    public interface IHitable
    {
        Result<HitRecord> Hit(Ray r, float tMin, float tMax);
        Result<AABB> BoundingBox();
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

        public Result<AABB> BoundingBox()
        {
            if (Count < 1)
                return new Result<AABB>();

            var firstBox = this[0].BoundingBox();
            if (!firstBox.IsOK)
                return new Result<AABB>();

            var box = firstBox.Content;
            for (var i = 0; i < Count; i++)
            {
                var tempBox = this[i].BoundingBox();
                if (!tempBox.IsOK)
                    return new Result<AABB>();
                box += tempBox.Content;
            }

            return new Result<AABB>(box);
        }
    }
}