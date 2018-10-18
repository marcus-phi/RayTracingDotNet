using System;

namespace RayTracingDotNet
{
    public class Vec3
    {
        private readonly float[] e = new float[3];

        public Vec3() : this(0, 0 ,0) {}

        public Vec3(float e0, float e1, float e2)
        {
            e[0] = e0; e[1] = e1; e[2] = e2;
        }

        public float X { get { return e[0]; } }
        public float Y { get { return e[1]; } }
        public float Z { get { return e[2]; } }
        public float R { get { return e[0]; } }
        public float G { get { return e[1]; } }
        public float B { get { return e[2]; } }

        public static Vec3 operator +(Vec3 v) { return v; }
        public static Vec3 operator -(Vec3 v) { return new Vec3(-v.X, -v.Y, -v.Z); } 
        public float this[int idx] { get { return e[idx]; } set { e[idx] = value; }}

        public static Vec3 operator +(Vec3 a, Vec3 b) { return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }
        public static Vec3 operator -(Vec3 a, Vec3 b) { return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
        public static Vec3 operator *(Vec3 a, Vec3 b) { return new Vec3(a.X * b.X, a.Y * b.Y, a.Z * b.Z); }
        public static Vec3 operator /(Vec3 a, Vec3 b) { return new Vec3(a.X / b.X, a.Y / b.Y, a.Z / b.Z); }
        public static Vec3 operator *(Vec3 v, float a) { return new Vec3(v.X * a, v.Y * a, v.Z * a); }
        public static Vec3 operator *(float a, Vec3 v) { return new Vec3(v.X * a, v.Y * a, v.Z * a); }
        public static Vec3 operator /(Vec3 v, float a) { return new Vec3(v.X / a, v.Y / a, v.Z / a); }

        public float Length { get { return (float)Math.Sqrt(SqLength); } }
        public float SqLength { get { return X*X + Y*Y + Z*Z; } }

        public void MakeUnitVector()
        {
            var k = 1 / Length;
            e[0] *= k;
            e[1] *= k;
            e[2] *= k;
        }

        public Vec3 UnitVector()
        {
            return this / Length;
        }

        public float Dot(Vec3 v)
        {
            return X*v.X + Y*v.Y + Z*v.Z;
        }

        public Vec3 Cross(Vec3 v)
        {
            return new Vec3(
                Y*v.Z - Z*v.Y,
                Z*v.X - X*v.Z,
                X*v.Y - Y*v.X
            );
        }
    }
}