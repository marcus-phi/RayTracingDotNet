using System;

namespace RayTracingDotNet
{
    public class Ray
    {
        public Ray() : this(new Vec3(), new Vec3()) {}

        public Ray(Vec3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Vec3 Origin { get; private set; }
        public Vec3 Direction { get; private set; }
        public Vec3 PointAtParameter(float t) { return Origin + t * Direction; }
    }
}