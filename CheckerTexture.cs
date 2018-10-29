using System;

namespace RayTracingDotNet
{
    public class CheckerTexture : ITexture
    {
        public CheckerTexture(ITexture tex0, ITexture tex1)
        {
            Even = tex0;
            Odd = tex1;
        }

        public ITexture Even { get; private set; }
        public ITexture Odd { get; private set; }

        public Vec3 Value(float u, float v, Vec3 p)
        {
            var sines = Math.Sin(10*p.X)*Math.Sin(10*p.Y)*Math.Sin(10*p.Z);
            return sines < 0 ? Odd.Value(u, v, p) : Even.Value(u, v, p);
        }
    }
}