using System;

namespace RayTracingDotNet
{
    public class AABB
    {
        public AABB() : this(new Vec3(), new Vec3()) { }

        public AABB(Vec3 min, Vec3 max)
        {
            Min = min;
            Max = max;
        }

        public Vec3 Min { get; private set; }
        public Vec3 Max { get; private set; }

        public bool Hit(Ray ray, float tMin, float tMax)
        {
            for (var a = 0; a < 3; a++)
            {
                var invD = 1.0f / ray.Direction[a];
                var t0 = (Min[a] - ray.Origin[a]) * invD;
                var t1 = (Max[a] - ray.Origin[a]) * invD;
                if (invD < 0.0f)
                {
                    var tmp = t0;
                    t0 = t1;
                    t1 = tmp;
                }
                tMin = Math.Max(t0, tMin);
                tMax = Math.Min(t1, tMax);
                if (tMax <= tMin)
                    return false;
            }
            return true;
        }

        public static AABB operator +(AABB a, AABB b)
        {
            var min = new Vec3(Math.Min(a.Min.X, b.Min.X), Math.Min(a.Min.Y, b.Min.Y), Math.Min(a.Min.Z, b.Min.Z));
            var max = new Vec3(Math.Max(a.Max.X, b.Max.X), Math.Max(a.Max.Y, b.Max.Y), Math.Max(a.Max.Z, b.Max.Z));
            return new AABB(min, max);
        }
    }
}