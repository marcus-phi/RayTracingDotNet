using System;

namespace RayTracingDotNet
{
    public interface ITexture
    {
        Vec3 Value(float u, float v, Vec3 p);
    }

    public class ConstantTexture : ITexture
    {
        public ConstantTexture() : this(new Vec3()) {}

        public ConstantTexture(Vec3 color)
        {
            Color = color;
        }

        public Vec3 Color { get; private set; }

        public Vec3 Value(float u, float v, Vec3 p)
        {
            return Color;
        }
    }
}