using System;

namespace RayTracingDotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nx = 720;
            var ny = 480;
            var ns = 128;
            Console.WriteLine(string.Format("P3\n{0} {1}\n255", nx, ny));

            /*var world = new HitableList()
            {
                new Sphere(new Vec3(0.0f, 0.0f, -1.0f), 0.5f, new Lambertian(new Vec3(0.8f, 0.3f, 0.3f))),
                new Sphere(new Vec3(0.0f, -100.5f, -1.0f), 100.0f, new Lambertian(new Vec3(0.8f, 0.8f, 0.0f))),
                new Sphere(new Vec3(1.0f, 0.0f, -1.0f), 0.5f, new Metal(new Vec3(0.8f, 0.6f, 0.2f), 1.0f)),

                // make bubble by having a smaller sphere with negative radius
                new Sphere(new Vec3(-1.0f, 0.0f, -1.0f), 0.5f, new Dielectric(1.5f)),
                new Sphere(new Vec3(-1.0f, 0.0f, -1.0f), -0.45f, new Dielectric(1.5f)),
            };*/

            var world = new BvhNode(RandomScene());

            var lookFrom = new Vec3(13.0f, 2.0f, 3.0f);
            var lookAt = new Vec3(0.0f, 0.0f, 0.0f);
            var focusDistance = 10.0f;
            var aperture = 0.05f;
            var cam = new Camera(lookFrom, lookAt, new Vec3(0.0f, 1.0f, 0.0f), 20.0f, ((float)nx) / ((float)ny), aperture, focusDistance);

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

        private static HitableList RandomScene()
        {
            var scene = new HitableList()
            {
                new Sphere(new Vec3(0.0f, -1000.0f, 0.0f), 1000.0f, new Lambertian(new Vec3(0.5f, 0.5f, 0.5f))),
                new Sphere(new Vec3(0.0f, 1.0f, 0.0f), 1.0f, new Dielectric(1.5f)),
                new Sphere(new Vec3(-4.0f, 1.0f, 0.0f), 1.0f, new Lambertian(new Vec3(0.4f, 0.2f, 0.1f))),
                new Sphere(new Vec3(4.0f, 1.0f, 0.0f), 1.0f, new Metal(new Vec3(0.7f, 0.6f, 0.5f), 0.0f)),
            };

            for (var a = -11; a < 11; a++)
            {
                for (var b = -11; b < 11; b++)
                {
                    var chooseMat = Utils.NextFloat();
                    var center = new Vec3(a + 0.9f * Utils.NextFloat(), 0.2f, b + 0.9f * Utils.NextFloat());
                    if ((center - new Vec3(4.0f, 0.2f, 0.0f)).Length > 0.9f)
                    {
                        IMaterial material = null;
                        if (chooseMat < 0.8) //diffuse
                            material = new Lambertian(new Vec3(Utils.NextFloat() * Utils.NextFloat(), Utils.NextFloat() * Utils.NextFloat(), Utils.NextFloat() * Utils.NextFloat()));
                        else if (chooseMat < 0.95f) //metal
                            material = new Metal(new Vec3(0.5f * (1 + Utils.NextFloat()), 0.5f * (1 + Utils.NextFloat()), 0.5f * (1 + Utils.NextFloat())), 0.5f * Utils.NextFloat());
                        else
                            material = new Dielectric(1.5f);
                        scene.Add(new Sphere(center, 0.2f, material));
                    }
                }
            }

            return scene;
        }

        private static Vec3 Color(Ray r, IHitable world, int depth)
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