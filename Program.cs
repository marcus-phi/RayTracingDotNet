using System;

namespace RayTracingDotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nx = 200;
            var ny = 100;
            var ns = 100;
            Console.WriteLine(string.Format("P3\n{0} {1}\n255", nx, ny));

            var world = new HitableList()
            {
                new Sphere(new Vec3(0.0f, 0.0f, -1.0f), 0.5f, new Lambertian(new Vec3(0.8f, 0.3f, 0.3f))),
                new Sphere(new Vec3(0.0f, -100.5f, -1.0f), 100.0f, new Lambertian(new Vec3(0.8f, 0.8f, 0.0f))),
                new Sphere(new Vec3(1.0f, 0.0f, -1.0f), 0.5f, new Metal(new Vec3(0.8f, 0.6f, 0.2f), 1.0f)),

                // make bubble by having a smaller sphere with negative radius
                new Sphere(new Vec3(-1.0f, 0.0f, -1.0f), 0.5f, new Dielectric(1.5f)),
                new Sphere(new Vec3(-1.0f, 0.0f, -1.0f), -0.45f, new Dielectric(1.5f)),
            };

            var cam = new Camera();

            for (var j = ny - 1; j >= 0; j--)
            {
                for (var i = 0; i < nx; i++)
                {
                    var col = new Vec3();
                    for (var s = 0; s < ns; s++)
                    {
                        var u = (float)(i + Utils.NextFloat()) / (float)nx;
                        var v = (float)(j + Utils.NextFloat()) / (float)ny;
                        var r = cam.GetRay(u, v);
                        col += Color(r, world, 0);
                    }
                    col /= (float)ns;
                    // gamma correction
                    col = new Vec3((float)Math.Sqrt(col.R), (float)Math.Sqrt(col.G), (float)Math.Sqrt(col.B));
                    var ir = (int)(255.99 * col[0]);
                    var ig = (int)(255.99 * col[1]);
                    var ib = (int)(255.99 * col[2]);
                    Console.WriteLine(string.Format("{0} {1} {2}", ir, ig, ib));
                }
            }
        }

        private static Vec3 Color(Ray r, HitableList world, int depth)
        {
            var hitResult = world.Hit(r, 0.001f, float.MaxValue);
            if (hitResult.IsOK)
            {
                var scatterResult = hitResult.Content.Material.Scatter(r, hitResult.Content);
                if (depth < 50 && scatterResult.IsOK)
                    return scatterResult.Content.Attenuation * Color(scatterResult.Content.ScatteredRay, world, depth + 1);
                else
                    return new Vec3();
            }
            else
            {
                var unitDirection = r.Direction.UnitVector();
                var t = 0.5f * unitDirection.Y + 1.0f;
                return (1.0f - t) * new Vec3(1.0f, 1.0f, 1.0f) + t * new Vec3(0.5f, 0.7f, 1.0f);
            }
        }
    }
}