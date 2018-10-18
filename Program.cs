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
            for(var j = ny-1; j >= 0; j--) {
                for(var i = 0; i < nx; i++) {
                    var col = new Vec3((float)i/(float)nx, (float)j/(float)ny, 0.2f);
                    var ir = (int)(255.99*col[0]);
                    var ig = (int)(255.99*col[1]);
                    var ib = (int)(255.99*col[2]);
                    Console.WriteLine(string.Format("{0} {1} {2}", ir, ig, ib));
                }
            }
        }
    }
}