using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RayTracingDotNet
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var inFile = args.Length > 0 ? args[0] : "./scene.json";
            var sceneDef = JsonConvert.DeserializeObject<SceneDefinition>(File.ReadAllText(inFile));
            ShowSceneDef(sceneDef);

            var outFile = args.Length > 1 ? args[1] : "./output.ppm";
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine("=====================================\n");
            var sb = new StringBuilder();
            Render(sceneDef, sb);
            using (var file = new StreamWriter(outFile, false, Encoding.Default, 65536))
                file.Write(sb);
            stopWatch.Stop();
            Console.WriteLine("=====================================");
            Console.WriteLine($"Complete: {stopWatch.Elapsed:g}");
        }

        private static void ShowSceneDef(SceneDefinition sceneDef)
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("ImageWidth: " + sceneDef.ImageWidth);
            Console.WriteLine("ImageHeight: " + sceneDef.ImageHeight);
            Console.WriteLine("NumSamples: " + sceneDef.NumSamples);
            Console.WriteLine("ChunkSize: " + sceneDef.ChunkSize);
            Console.WriteLine("CameraLookFrom: " + string.Join(", ", sceneDef.CameraLookFrom));
            Console.WriteLine("CameraLookAt: " + string.Join(", ", sceneDef.CameraLookAt));
            Console.WriteLine("CameraFocusDistance: " + sceneDef.CameraFocusDistance);
            Console.WriteLine("CameraAperture: " + sceneDef.CameraAperture);
        }

        private static void Render(SceneDefinition sceneDef, StringBuilder output)
        {
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

            // var world = TwoPerlinSpheres();

            var lookFrom = new Vec3(sceneDef.CameraLookFrom[0], sceneDef.CameraLookFrom[1], sceneDef.CameraLookFrom[2]);
            var lookAt = new Vec3(sceneDef.CameraLookAt[0], sceneDef.CameraLookAt[1], sceneDef.CameraLookAt[2]);
            var focusDistance = sceneDef.CameraFocusDistance;
            var aperture = sceneDef.CameraAperture;
            var cam = new Camera(lookFrom, lookAt, new Vec3(0.0f, 1.0f, 0.0f), 20.0f, sceneDef.ImageWidth / (float)sceneDef.ImageHeight, aperture, focusDistance);

            var totalCount = sceneDef.ImageWidth * sceneDef.ImageHeight;
            var currentCount = 0;

            var sw = new Stopwatch();
            sw.Start();
            
            output.AppendLine($"P3\n{sceneDef.ImageWidth} {sceneDef.ImageHeight}\n255");

            if (sceneDef.ChunkSize > 0)
            {
                var chunks = Enumerable.Range(0, totalCount).Chunk(sceneDef.ChunkSize * sceneDef.ChunkSize).ToArray();
                var bag = new ConcurrentBag<Tuple<int, StringBuilder>>();
                var progress = new Progress<int>(cnt => ReportProgress(cnt, totalCount, sw.Elapsed)) as IProgress<int>;
                Parallel.ForEach(chunks, chunk =>
                {
                    var chunkOutput = new StringBuilder();
                    foreach (var idx in chunk)
                    {
                        var i = idx % sceneDef.ImageWidth;
                        var j = sceneDef.ImageHeight - 1 - idx / sceneDef.ImageWidth;
                        
                        RenderPart(i, j, sceneDef, cam, world, chunkOutput);

                        Interlocked.Increment(ref currentCount);
                        progress.Report(currentCount);
                    }
                    bag.Add(new Tuple<int, StringBuilder>(chunk.First(), chunkOutput));
                });

                foreach (var tuple in bag.OrderBy(x => x.Item1))
                    output.Append(tuple.Item2);
            }
            else
            {
                for (var j = sceneDef.ImageHeight - 1; j >= 0; j--)
                {
                    for (var i = 0; i < sceneDef.ImageWidth; i++)
                    {
                        RenderPart(i, j, sceneDef, cam, world, output);
                        ReportProgress(++currentCount, totalCount, sw.Elapsed);
                    }
                }
            }
            
            sw.Stop();
            Console.WriteLine($"\nElapsed: {sw.Elapsed.Hours:00}:{sw.Elapsed.Minutes:00}:{sw.Elapsed.Seconds:00}");
        }

        private static HitableList RandomScene()
        {
            var checker = new CheckerTexture(
                new ConstantTexture(new Vec3(0.2f, 0.3f, 0.1f)),
                new ConstantTexture(new Vec3(0.9f, 0.9f, 0.9f))
            );
            var scene = new HitableList()
            {
                new Sphere(new Vec3(0.0f, -1000.0f, 0.0f), 1000.0f, new Lambertian(checker)),
                new Sphere(new Vec3(0.0f, 1.0f, 0.0f), 1.0f, new Dielectric(1.5f)),
                new Sphere(new Vec3(-4.0f, 1.0f, 0.0f), 1.0f, new Lambertian(new ConstantTexture(new Vec3(0.4f, 0.2f, 0.1f)))),
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
                        IMaterial material;
                        if (chooseMat < 0.8) //diffuse
                            material = new Lambertian(new ConstantTexture(new Vec3(Utils.NextFloat() * Utils.NextFloat(), Utils.NextFloat() * Utils.NextFloat(), Utils.NextFloat() * Utils.NextFloat())));
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

        private static IHitable TwoPerlinSpheres()
        {
            var pertext = new NoiseTexture(4.0f);
            return new HitableList()
            {
                new Sphere(new Vec3(0.0f, -1000.0f, 0.0f), 1000.0f, new Lambertian(pertext)),
                new Sphere(new Vec3(0.0f, 2.0f, 0.0f), 2.0f, new Lambertian(pertext))
            };
        }

        private static void RenderPart(int i, int j, SceneDefinition sceneDef, Camera cam, IHitable world, StringBuilder output)
        {
            var col = new Vec3();
            for (var s = 0; s < sceneDef.NumSamples; s++)
            {
                var u = (i + Utils.NextFloat()) / sceneDef.ImageWidth;
                var v = (j + Utils.NextFloat()) / sceneDef.ImageHeight;
                var r = cam.GetRay(u, v);
                col += Color(r, world, 0);
            }
            col /= sceneDef.NumSamples;
            // gamma correction
            col = new Vec3((float)Math.Sqrt(col.R), (float)Math.Sqrt(col.G), (float)Math.Sqrt(col.B));
            var ir = (int)(255.99 * col[0]);
            var ig = (int)(255.99 * col[1]);
            var ib = (int)(255.99 * col[2]);
            output.AppendLine($"{ir} {ig} {ib}");
        }

        private static void ReportProgress(int currentCount, int totalCount, TimeSpan elapsed)
        {
            var percent = currentCount * 100.0f / totalCount;
            var eta = percent > 0 ? elapsed / percent * 100 : TimeSpan.Zero;

            Console.CursorLeft = 0;
            Console.Write($"Progress: {percent:F2}%, elapsed: {elapsed:g}, eta: {eta:g}");
        }
    }
}