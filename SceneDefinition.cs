using System;

namespace RayTracingDotNet
{
    public class SceneDefinition
    {
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int NumSamples { get; set; }

        public float[] CameraLookFrom { get; set; }
        public float[] CameraLookAt { get; set; }
        public float CameraFocusDistance { get; set; }
        public float CameraAperture { get; set; }
    }
}