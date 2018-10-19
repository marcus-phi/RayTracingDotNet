using System;

namespace RayTracingDotNet
{
    public class Camera
    {
        public Camera()
        {
            LowerLeftCorner = new Vec3(-2.0f, -1.0f, -1.0f);
            Horizontal = new Vec3(4.0f, 0.0f, 0.0f);
            Vertical = new Vec3(0.0f, 2.0f, 0.0f);
            Origin = new Vec3(0.0f, 0.0f, 0.0f);
        }

        public Vec3 Origin { get; set; }
        public Vec3 LowerLeftCorner { get; set; }
        public Vec3 Horizontal { get; set; }
        public Vec3 Vertical { get; set; }

        public Ray GetRay(float u, float v)
        {
           return new Ray(Origin, LowerLeftCorner + u*Horizontal + v*Vertical - Origin);
        }
    }
}