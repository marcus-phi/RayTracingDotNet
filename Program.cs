using System;

namespace RayTracingDotNet
{
    public class Program
    {
        private static Random rand = new Random();

        public static void Main(string[] args)
        {
            var nx = 200;
            var ny = 100;
            var ns = 100;
            Console.WriteLine(string.Format("P3\n{0} {1}\n255", nx, ny));

            var world = new HitableList()
            {
                new Sphere(new Vec3(0.0f, 0.0f, -1.0f), 0.5f),
                new Sphere(new Vec3(0.0f, -100.5f, -1.0f), 100.0f),
            };

            var cam = new Camera();

            for(var j = ny-1; j >= 0; j--) {
                for(var i = 0; i < nx; i++) {
                    var col = new Vec3();
                    for(var s = 0; s < ns; s++)
                    {
                        var u = (float)(i + rand.NextDouble())/(float)nx;
                        var v = (float)(j + rand.NextDouble())/(float)ny;
                        var r = cam.GetRay(u, v);
                        col += Color(r, world);
                    }
                    col /= (float)ns;
                    // gamma correction
                    col = new Vec3((float)Math.Sqrt(col.R), (float)Math.Sqrt(col.G), (float)Math.Sqrt(col.B));
                    var ir = (int)(255.99*col[0]);
                    var ig = (int)(255.99*col[1]);
                    var ib = (int)(255.99*col[2]);
                    Console.WriteLine(string.Format("{0} {1} {2}", ir, ig, ib));
                }
            }
        }

        private static Vec3 Color(Ray r, HitableList world)
        {
            var hitRecord = world.Hit(r, 0.001f, float.MaxValue);
            if(hitRecord.IsHit)
            {
                var target = hitRecord.P + hitRecord.Normal + RandomInUnitSphere();
                return 0.5f * Color(new Ray(hitRecord.P, target - hitRecord.P), world);
            }
            else
            {
                var unitDirection = r.Direction.UnitVector();
                var t = 0.5f * unitDirection.Y + 1.0f;
                return (1.0f - t) * new Vec3(1.0f, 1.0f, 1.0f) + t * new Vec3(0.5f, 0.7f, 1.0f);
            }
        }

        private static Vec3 RandomInUnitSphere()
        {
            Vec3 p;
            do
            {
                p = 2.0f * new Vec3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()) - new Vec3(1.0f, 1.0f, 1.0f);
            } while (p.SqLength >= 1.0f);
            return p;
        }
    }
}