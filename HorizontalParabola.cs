using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;


namespace Optics
{
    public class HorizontalParabola : ILine
    {
        private Vector2 origin;
        private float p = 2f;
        private float yFrom = -5, yTo = 5;

        /// <summary>
        /// y**2=2px parabola with local (x0,y0) in origin V2
        /// </summary>
        /// <param name="origin">origin of local coords</param>
        /// <param name="p">facal length</param>
        /// <param name="yFrom">yFrom<y<yTo</param>
        /// <param name="yTo">yFrom<y<yTo</param>
        public HorizontalParabola(Vector2 origin, float p, float yFrom, float yTo)
        {
            this.origin = origin;
            this.p = p;
            this.yFrom = yFrom;
            this.yTo = yTo;
        }


        /// <summary>
        /// Returns inteersection as RayHit obj
        /// </summary>
        /// <typeparam name="Tray">Tray must be derive from Ray class</typeparam>
        /// <param name="ray"></param>
        public RayHit<Tray> Intersection<Tray>(Tray ray) where Tray: Ray
        {
            var localRay = ray.ParalellTransfer(-origin);
            var _a = 2 * p;

            if (localRay.Direction.Y == 0)                
                return new RayHit<Tray>(ray,Point(localRay.Origin.Y),
                    yFrom<=localRay.Origin.Y && localRay.Origin.Y<=yTo &&
                    localRay.Origin.X * localRay.Direction.X<0
                    );

            if (localRay.Direction.X == 0)
            {
                var Y1 = Math.Sqrt(2 * p * localRay.Origin.X);
                var Y2 = -Y1;
                var y =(float)(localRay.Direction.Y > 0 ? Y1 : Y2);
                return new RayHit<Tray>(ray,Point(y),
                    yFrom<=y && y<=yTo &&
                    localRay.Origin.X * p > 0
                    );
            }


            var _k = localRay.Direction.Y / localRay.Direction.X;
            var _b = -localRay.Direction.Y / localRay.Direction.X * localRay.Origin.X + localRay.Origin.Y;

            var a = 1f / _a;
            var b = -1f / _k;
            var c = _b / _k;
            var d = b * b - 4f * a * c;
            if (d < 0)
                return new RayHit<Tray>(ray);
            if (d == 0)
            {
                var y = -b / 2f / a;
                return new RayHit<Tray>(ray, Point(y),
                    yFrom <= y && y <= yTo
                    );
            }

            float y1 = (-b + (float)Math.Sqrt(d)) / 2f / a;
            float y2 = (-b - (float)Math.Sqrt(d)) / 2f / a;

            var p1 = Point(y1);
            var p2 = Point(y2);

            var v1 = p1 - localRay.Origin;
            var v2 = p2 - localRay.Origin;

            float d1 = Vector2.Dot(v1, localRay.Direction);
            float d2 = Vector2.Dot(v2, localRay.Direction);


            if (d1 > 0 && d2>0)
            {
                if (Math.Abs(d1) < Math.Abs(d2))
                {
                    return new RayHit<Tray>(ray, p1);
                }
                else
                {
                    return new RayHit<Tray>(ray, p2);
                }
            }
            else
            {
                if (d1>0)
                {
                    return new RayHit<Tray>(ray, p1);
                }
                else if(d2>0)
                {
                    return new RayHit<Tray>(ray, p2);
                }
            }

            //if (Math.Abs(d1) < Math.Abs(d2) && d1 > 0)
            //{
            //    if (yFrom <= y1 && y1 <= yTo)
            //        return new RayHit<Tray>(ray, p1);
            //}
            //else if (yFrom <= y2 && y2 <= yTo)
            //{
            //    return new RayHit<Tray>(ray,p2);
            //}
            return new RayHit<Tray>(ray);
        }

        /// <summary>
        /// Returns outer normal vector
        /// </summary>
        public Vector2 Normal(float y)
        {
            if (y == 0)
                return new Vector2(-p, 0);
            var yPrime = p / y;
            Vector2 tangent =Vector2.Normalize(new Vector2(1, yPrime));
            Vector2 normal = Vector2.Transform(tangent, Matrix3x2.CreateRotation((float)Math.PI/2f*Math.Sign(y)));
            return normal;            
        }

        /// <summary>
        /// Point on line 
        /// </summary>
        public Vector2 Point(float y)
        {
            return new Vector2(y * y / (2f * p), y) + origin;
        }
    }

}