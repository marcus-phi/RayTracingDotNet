using System;

namespace RayTracingDotNet
{
    public static class Perlin
    {
        static Perlin()
        {
            RanVec = PerlinGenerate();
            PermX = PerlinGeneratePerm();
            PermY = PerlinGeneratePerm();
            PermZ = PerlinGeneratePerm();
        }

        public static float Noise(Vec3 p)
        {
            var u = p.X - Math.Floor(p.X);
            var v = p.Y - Math.Floor(p.Y);
            var w = p.Z - Math.Floor(p.Z);
            var i = (int)Math.Floor(p.X);
            var j = (int)Math.Floor(p.Y);
            var k = (int)Math.Floor(p.Z);
            var c = new Vec3[2,2,2];
            for (var di = 0; di < 2; di++)
                for (var dj = 0; dj < 2; dj++)
                    for (var dk = 0; dk < 2; dk++)
                        c[di,dj,dk] = RanVec[PermX[(i+di)&255] ^ PermY[(j+dj)&255] ^ PermZ[(k+dk)&255]];
            return PerlinInterp(c, (float)u, (float)v, (float)w);
        }

        public static float Turb(Vec3 p, int depth=7)
        {
            var accum = 0.0;
            var temp = new Vec3(p.X, p.Y, p.Z);
            var weight = 1.0;
            for(var i = 0; i < depth; i++)
            {
                accum += weight * Noise(temp);
                weight *= 0.5;
                temp *= 2;
            }
            return (float)Math.Abs(accum);
        }

        private static Vec3[] RanVec;
        private static int[] PermX;
        private static int[] PermY;
        private static int[] PermZ;

        private static Vec3[] PerlinGenerate()
        {
            var p = new Vec3[256];
            for(var i = 0; i < p.Length; i++)
                p[i] = new Vec3(-1 + 2*Utils.NextFloat(), -1 + 2*Utils.NextFloat(), -1 + 2*Utils.NextFloat()).UnitVector();
            return p;
        }

        private static void Permute(int[] p)
        {
            for(var i = p.Length - 1; i > 0; i--)
            {
                var target = (int)(Utils.NextFloat() * (i+1));
                var tmp = p[i];
                p[i] = p[target];
                p[target] = tmp;
            }
        }

        private static int[] PerlinGeneratePerm()
        {
            var p = new int[256];
            for(var i = 0; i < p.Length; i++)
                p[i] = i;
            Permute(p);
            return p;
        }

        private static float PerlinInterp(Vec3[,,] c, float u, float v, float w)
        {
            var uu = u*u*(3-2*u);
            var vv = v*v*(3-2*v);
            var ww = w*w*(3-2*w);
            var accum = 0.0;
            for (var i = 0; i < 2; i++)
                for (var j = 0; j < 2; j++)
                    for (var k = 0; k < 2; k++)
                    {
                        var weight = new Vec3(u-i, v-j, w-k);
                        accum += (i*uu + (1-i)*(1-uu)) * (j*vv + (1-j)*(1-vv)) * (k*ww + (1-k)*(1-ww)) * c[i,j,k].Dot(weight);
                    }
            return (float)accum;
        }
    }
}