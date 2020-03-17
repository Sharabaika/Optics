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

        public float LeftPoint(float y) => leftSurface.Point(y).X;
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
            Height = height;
        }

        public override bool IsIntersects<Tray>(Tray ray)
        {
            var p1 = leftSurface.Intersection(ray, false);
            var p2 = rightSurface.Intersection(ray, false);
            return (p1 || p2) ? true : false;
            //return p1 || p2; //??
        }

        private (RayHit<LightRay> hit, LightRay refraction, LightRay Reflection, bool fullReflection)
            HandleSide(LightRay ray, ILine side, bool isInner, bool ignoreFirst= false)
        {
            var hit = side.Intersection(ray,ignoreFirst);
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

                var (reflection, refraction, isFullReflection) = ReflectAndRefract(ray, normal, hit.Point, 1, n);

                return (hit, refraction, reflection, isFullReflection);
            }

            return (hit, ray, ray, false);
        }

        public override void HandleRay(LightRay rayToHandle, List<RayHit<LightRay>> hits, List<LightRay> secondaryRays)
        {
            Stack<LightRay> raysToHandle = new Stack<LightRay>();
            ILine side = toLeft(rayToHandle.Origin) ? leftSurface : rightSurface;
            ILine otherSide = toRight(rayToHandle.Origin) ? leftSurface : rightSurface;

            var (hit, innerRefraction, mainReflection, fullRef) = HandleSide(rayToHandle, side, false);
            if (hit)
            {
                if(!float.IsNaN(mainReflection.Intensity) && mainReflection.Intensity>0f)
                    secondaryRays.Add(mainReflection);
                hits.Add(hit);

                if (fullRef)
                {
                    return;
                }
                raysToHandle.Push(innerRefraction);

                int reflectionDepth = 0;
                bool reflectsOnSameSide = false;
                bool previousHit = false;
                // all are inside
                while(raysToHandle.Count>0 && reflectionDepth<128)
                {

                    var ray = raysToHandle.Pop();
                    var (newHit, newRefraction, newReflection, fullref) = HandleSide(ray, otherSide, true, reflectsOnSameSide);

                    reflectsOnSameSide = false;

                    if (newHit)
                    {
                        previousHit = true;
                        reflectionDepth++;

                        if (newReflection.Intensity >= 0.001f)
                            raysToHandle.Push(newReflection);
                        hits.Add(newHit);

                        if (fullref)
                        {
                        }
                        else
                        {
                            if(newRefraction.Intensity>= 0.001f)
                                secondaryRays.Add(newRefraction);
                        }
                    }
                    else
                    {
                        if (previousHit == false)
                            throw new Exception();
                        raysToHandle.Push(ray);
                        reflectsOnSameSide = true;
                        previousHit = false;
                    }
                    flipSides();
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
