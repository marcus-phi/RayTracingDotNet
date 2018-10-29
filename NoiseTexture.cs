using System;

namespace RayTracingDotNet
{
    public class NoiseTexture : ITexture
    {
        public NoiseTexture(float scale)
        {
            Scale = scale;
        }

        public float Scale { get; private set; }

        public Vec3 Value(float u, float v, Vec3 p)
        {
            return new Vec3(1.0f, 1.0f, 1.0f) * 0.5f * (float)(1 + Math.Sin(Scale* p.Z + 10 * Perlin.Turb(p)));
        }
    }
}