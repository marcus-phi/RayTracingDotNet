using System;

namespace RayTracingDotNet
{
    public class Camera
    {
        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 up, float verticalFov, float aspect, float aperture, float focusDistance)
        {
            var theta = (float)(verticalFov * Math.PI / 180.0f);
            var halfHeight = (float)Math.Tan(theta / 2);
            var halfWidth = aspect * halfHeight;
            Origin = lookFrom;
            W = (lookFrom - lookAt).UnitVector();
            U = up.Cross(W).UnitVector();
            V = W.Cross(U);
            LowerLeftCorner = Origin - halfWidth * focusDistance * U - halfHeight * focusDistance * V - focusDistance * W;
            Horizontal = 2 * halfWidth * focusDistance * U;
            Vertical = 2 * halfHeight * focusDistance * V;
            LensRadius = aperture / 2.0f;
        }

        public Vec3 Origin { get; set; }
        public Vec3 LowerLeftCorner { get; set; }
        public Vec3 Horizontal { get; set; }
        public Vec3 Vertical { get; set; }
        private Vec3 U { get; set; }
        private Vec3 W { get; set; }
        private Vec3 V { get; set; }
        public float LensRadius { get; set; }

        public Ray GetRay(float s, float t)
        {
            var rd = LensRadius * Utils.RandomInUnitDisk();
            var offset = U * rd.X + V * rd.Y;
            return new Ray(Origin + offset, LowerLeftCorner + s * Horizontal + t * Vertical - Origin - offset);
        }
    }
}