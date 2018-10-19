using System;

namespace RayTracingDotNet
{
    public class Sphere : IHitable
    {
        public Sphere() : this(new Vec3(), 0.0f, null) {}

        public Sphere(Vec3 center, float radius, IMaterial material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public Vec3 Center { get; private set; }
        public float Radius { get; private set; }
        public IMaterial Material { get; private set; }

        public HitRecord Hit(Ray r, float tMin, float tMax)
        {
            var oc = r.Origin - Center;
            var a = r.Direction.Dot(r.Direction);
            var b = 2.0f * oc.Dot(r.Direction);
            var c = oc.Dot(oc) - Radius*Radius;
            var discriminant = b*b - 4*a*c;

            var hitRecord = new HitRecord();
            if(discriminant > 0)
            {
                var temp = (-b - (float)Math.Sqrt(discriminant)) / (2.0f*a);
                if(temp < tMax && temp > tMin)
                    return GetHitRecord(r, temp);
                temp = (-b + (float)Math.Sqrt(discriminant)) / (2.0f*a);
                if(temp < tMax && temp > tMin)
                    return GetHitRecord(r, temp);
            }
            return hitRecord;
        }

        private HitRecord GetHitRecord(Ray r, float t)
        {
            var p = r.PointAtParameter(t);
            return new HitRecord()
            {
                IsHit = true,
                T = t,
                P = p,
                Normal = (p - Center) / Radius,
                Material = Material,
            };
        }
    }
}