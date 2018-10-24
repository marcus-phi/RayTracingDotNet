using System;
using System.Collections.Generic;
using System.Linq;

namespace RayTracingDotNet
{
    public class BvhNode : IHitable
    {
        public BvhNode(IEnumerable<IHitable> hitables)
        {
            var axis = (int)(3 * Utils.NextFloat());
            var sorted = hitables.OrderBy(h =>
            {
                var box = h.BoundingBox();
                return box.IsOK ? box.Content.Min[axis] : float.NegativeInfinity;
            }).ToList();

            var count = sorted.Count;
            if (count > 0)
            {
                if (count == 1)
                    Left = Right = sorted[0];
                else
                {
                    Left = new BvhNode(sorted.Take(count / 2));
                    Right = new BvhNode(sorted.Skip(count / 2));
                }
            }

            var leftBox = Left != null ? Left.BoundingBox() : new Result<AABB>();
            var rightBox = Right != null ? Right.BoundingBox() : new Result<AABB>();
            if (leftBox.IsOK && rightBox.IsOK)
                Box = leftBox.Content + rightBox.Content;
        }

        public IHitable Left { get; private set; }
        public IHitable Right { get; private set; }
        private AABB Box { get; set; }

        public Result<HitRecord> Hit(Ray r, float tMin, float tMax)
        {
            if (Box == null || !Box.Hit(r, tMin, tMax))
                return new Result<HitRecord>();

            var hitLeft = Left != null ? Left.Hit(r, tMin, tMax) : new Result<HitRecord>();
            var hitRight = Right != null ? Right.Hit(r, tMin, tMax) : new Result<HitRecord>();
            if (hitLeft.IsOK && hitRight.IsOK)
                return hitLeft.Content.T < hitRight.Content.T ? hitLeft : hitRight;
            else if (hitLeft.IsOK)
                return hitLeft;
            else if (hitRight.IsOK)
                return hitRight;
            else
                return new Result<HitRecord>();
        }

        public Result<AABB> BoundingBox()
        {
            return new Result<AABB>(Box);
        }
    }
}