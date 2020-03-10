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

        public float focalLength { get; private set; }
        public float width { get; private set; }

        public float LeftPoint (float y) => leftSurface.Point(y).X;
        public float RightPoint(float y) => rightSurface.Point(y).X;

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
            Height= height;
        }

        private (RayHit<Ray> hit, Ray refraction, Ray Reflection,bool fullReflection) HandleSide(Ray ray, ILine side, bool isInner)
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

                var reflection = new Ray(hit.Point, Vector2.Reflect(ray.Direction, normal));

                bool isFullReflection = false;
                if (float.IsNaN(refractionAngle))
                {
                    isFullReflection = true;
                }

                var refraction = new Ray(hit.Point, Vector2.Transform(-normal, Matrix3x2.CreateRotation(-refractionAngle)));
                return (hit, refraction, reflection, isFullReflection);
            }

            return (hit, ray, ray,false);
        }

        // Dont use
        public override Ray HandleRay(Ray ray,List<RayHit<Ray>> hits, List<Ray> secondaryRays)
        {
            ILine side, otherSide;
            bool isInner = (ray.Origin.Y * ray.Origin.Y <= 2 * focalLength * (ray.Origin.X + width)) &&
                (ray.Origin.Y * ray.Origin.Y <= -2 * focalLength * (ray.Origin.X - width));

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
            var (hit, innerRefraction, mainReflection,fullRef) = HandleSide(ray, side, isInner);
            hits.Add(hit);
            secondaryRays.Add(mainReflection);
            if (hit)
            {
                var (hit2, mainRefraction, innerReflection,fullRef1) = HandleSide(innerRefraction, otherSide,true);                
                hits.Add(hit2);

                // Processing inner reflections
                void Swap<T>(ref T lhs, ref T rhs)
                {
                    T temp;
                    temp = lhs;
                    lhs = rhs;
                    rhs = temp;
                }
                Ray newRay = innerReflection;
                for (int i = 0; i < 3; i++)
                {
                    var (newHit, secondaryRefraction, secondaryReflection,fullRef2) = HandleSide(newRay, side, true);
                    if (newHit)
                    {
                        hits.Add(newHit);
                        secondaryRays.Add(secondaryRefraction);
                    }
                    else
                    {
                        Swap(ref side, ref otherSide);
                        (newHit, secondaryRefraction, secondaryReflection,fullRef2) = HandleSide(newRay, side, true);
                        if (newHit)
                        {
                            hits.Add(newHit);
                            secondaryRays.Add(secondaryRefraction);
                        }
                    }
                    newRay = secondaryReflection;
                    Swap(ref side, ref otherSide);
                }

                return mainRefraction;
            }
            return innerRefraction;
        }

        public void HandleRayV2(Ray rayToHandle, List<RayHit<Ray>> hits, List<Ray> secondaryRays)
        {


            Stack<Ray> raysToHandle = new Stack<Ray>();
            ILine side = toLeft(rayToHandle.Origin) ? leftSurface : rightSurface;
            ILine otherSide = toRight(rayToHandle.Origin) ? leftSurface : rightSurface;

            var (hit, innerRefraction, mainReflection, fullRef) = HandleSide(rayToHandle, side, true);
            if (hit)
            {
                if (fullRef)
                {
                    hits.Add(hit);
                    secondaryRays.Add(mainReflection);
                    return;
                }
                raysToHandle.Push(innerRefraction);

                // all are inside
                for (int i = 0; i < 3; i++)
                {                      
                    var ray = raysToHandle.Pop();
                    var (newHit, newRefraction, newReflection, fullref) = HandleSide(ray, otherSide,true);
                    var (newHit2, newRefraction2, newReflection2, fullref2) = HandleSide(ray, otherSide, true);

                    if(newHit && newHit2)
                    {
                        // TODO
                    }

                    if (newHit)
                    {
                        raysToHandle.Push(newReflection);
                        hits.Add(hit);

                        if (fullref)
                        {
                        }
                        else
                        {
                            secondaryRays.Add(newRefraction);
                        }
                    }
                    else
                    {
                        flipSides();
                        raysToHandle.Push(ray);
                    }
                }


            }

            void flipSides()
            {
                var tmp = side;
                side = otherSide;
                otherSide = tmp;
            }
        }


        public bool isOnLeftBorder(Vector2 p)
        {
            return (p.Y * p.Y == 2 * focalLength * (p.X + width));
        }
        public bool isOnRightBorder(Vector2 p)
        {
            return (p.Y * p.Y == -2 * focalLength * (p.X - width));
        }
        public bool toLeft(Vector2 p)
        {
            return (p.X < Center.X);
        }
        public bool toRight(Vector2 p)
        {
            return p.X > Center.X;
        }

    }
}
