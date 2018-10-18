using System;

namespace RayTracingDotNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nx = 200;
            var ny = 100;
            Console.WriteLine(string.Format("P3\n{0} {1}\n255", nx, ny));

            var lowerLeftCorner = new Vec3(-2.0f, -1.0f, -1.0f);
            var horizontal = new Vec3(4.0f, 0.0f, 0.0f);
            var vertical = new Vec3(0.0f, 2.0f, 0.0f);
            var origin = new Vec3(0.0f, 0.0f, 0.0f);

            var world = new HitableList()
            {
                new Sphere(new Vec3(0.0f, 0.0f, -1.0f), 0.5f),
                new Sphere(new Vec3(0.0f, -100.5f, -1.0f), 100.0f),
            };

            for(var j = ny-1; j >= 0; j--) {
                for(var i = 0; i < nx; i++) {
                    var u = (float)i/(float)nx;
                    var v = (float)j/(float)ny;
                    var ray = new Ray(origin, lowerLeftCorner + u*horizontal + v*vertical);
                    
                    var col = Color(ray, world);
                    var ir = (int)(255.99*col[0]);
                    var ig = (int)(255.99*col[1]);
                    var ib = (int)(255.99*col[2]);
                    Console.WriteLine(string.Format("{0} {1} {2}", ir, ig, ib));
                }
            }
        }

        private static Vec3 Color(Ray r, HitableList world)
        {
            var hitRecord = world.Hit(r, 0.0f, float.MaxValue);
            if(hitRecord.IsHit)
                return 0.5f * new Vec3(hitRecord.Normal.X + 1, hitRecord.Normal.Y + 1, hitRecord.Normal.Z + 1);
            else
            {
                var unitDirection = r.Direction.UnitVector();
                var t = 0.5f * unitDirection.Y + 1.0f;
                return (1.0f - t) * new Vec3(1.0f, 1.0f, 1.0f) + t * new Vec3(0.5f, 0.7f, 1.0f);
            }
        }
    }
}