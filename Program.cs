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

            for(var j = ny-1; j >= 0; j--) {
                for(var i = 0; i < nx; i++) {
                    var u = (float)i/(float)nx;
                    var v = (float)j/(float)ny;
                    var ray = new Ray(origin, lowerLeftCorner + u*horizontal + v*vertical);
                    var col = Color(ray);
                    var ir = (int)(255.99*col[0]);
                    var ig = (int)(255.99*col[1]);
                    var ib = (int)(255.99*col[2]);
                    Console.WriteLine(string.Format("{0} {1} {2}", ir, ig, ib));
                }
            }
        }

        private static Vec3 Color(Ray r)
        {
            if(HitSphere(new Vec3(0.0f, 0.0f, -1.0f), 0.5f, r))
                return new Vec3(1.0f, 0.0f, 0.0f);
            var unitDirection = r.Direction.UnitVector();
            var t = 0.5f * unitDirection.Y + 1.0f;
            return (1.0f - t) * new Vec3(1.0f, 1.0f, 1.0f) + t * new Vec3(0.5f, 0.7f, 1.0f);
        }

        private static bool HitSphere(Vec3 center, float radius, Ray r)
        {
            var oc = r.Origin - center;
            var a = r.Direction.Dot(r.Direction);
            var b = 2.0f * oc.Dot(r.Direction);
            var c = oc.Dot(oc) - radius*radius;
            var discriminant = b*b - 4*a*c;
            return discriminant > 0;
        }
    }
}