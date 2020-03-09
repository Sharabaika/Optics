using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Optics
{
    public class ParabolicLens : Lens
    {
        private ILine leftSurface;
        private ILine rightSurface;

        private float focalLength;
        private float width;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="refractiveIndex"> Refractive index as function of Y, -height<y<height </param>
        /// <param name="center">Center of lens</param>
        /// <param name="height">Distance from center to the top. Lens may be cut in order to fit in height</param>
        /// <param name="p">Focal length of parabola</param>
        /// <param name="d">Width in center</param>
        public ParabolicLens(Func<float, float> refractiveIndex, Vector2 center, float height, float p, float d) : base(refractiveIndex, center, height)
        {
            leftSurface = new HorizontalParabola(center - Vector2.UnitX * d, p, -height, height) as ILine;
            rightSurface = new HorizontalParabola(center + Vector2.UnitX * d, -p, -height, height) as ILine;
            focalLength = p;
            width = d;
        }

        private (RayHit<Ray> hit, Ray refraction, Ray Reflection) HandleSide(Ray ray, ILine side, bool isInner)
        {
            var hit = side.Intersection(ray);
            if (hit)
            {
                Vector2 normal;
                var n = refractiveIndex(hit.Y);

                if (isInner)
                {
                    normal = -side.Normal(hit.Y);
                    n = 1f / n;
                }
                else
                {
                    normal = side.Normal(hit.Y);
                }

                var incidenceAngle = (float)(Math.Atan2(-normal.Y,-normal.X) - Math.Atan2(ray.Direction.Y, ray.Direction.X));
                var refractionAngle = (float)Math.Asin(Math.Sin(incidenceAngle) / n);

                var refraction = new Ray(hit.Point, Vector2.Transform(-normal, Matrix3x2.CreateRotation(refractionAngle)));
                var reflection = new Ray(hit.Point, Vector2.Reflect(ray.Direction, normal));
                return (hit, refraction, reflection);
            }

            return (hit, ray, ray);
        }

        public override Ray HandleRay(Ray ray)
        {
            ILine side, otherSide;
            bool isInner = (ray.Origin.Y * ray.Origin.Y <= 2 * focalLength * (ray.Origin.X + width)) &&
                (ray.Origin.Y * ray.Origin.Y <= 2 * focalLength * (ray.Origin.X - width));

            if ((ray.Direction.X > 0f && !isInner) || (ray.Direction.X < 0 && isInner))
            {
                side = leftSurface;
                otherSide = rightSurface;
            }
            else
            {
                side = rightSurface;
                otherSide = leftSurface;
            }
            var (hit, innerRefraction, mainReflection) = HandleSide(ray, side, isInner);

            if (hit)
            {
                var (hit2, mainRefraction, innerReflection) = HandleSide(innerRefraction, otherSide,true);
                return mainRefraction;
            }
            return innerRefraction;
        }
    }
}
