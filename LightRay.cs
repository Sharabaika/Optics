using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Optics
{
    public class Ray
    {
        public Ray(Vector2 origin, Vector2 direction)
        {
            Origin = origin;
            Direction =Vector2.Normalize(direction);            
        }

        public Vector2 Origin { get; private set; }
        public Vector2 Direction { get; private set; }

        public Ray ParalellTransfer(Vector2 v)
        {
            return new Ray(Origin + v, Direction);
        }

    }

    public class LightRay : Ray
    {
        public LightRay(Vector2 origin, Vector2 direction, float intensity) : base(origin, direction)
        {
            Intensity = intensity;
        }

        public float Intensity { get; private set; }

    }

    public class RayHit<Tray> where Tray : Ray
    {
        public Tray ray;

        private Vector2 hitPoint;
        public Vector2 Point
        {
            get
            {
                if (isHit) return hitPoint;
                throw new NoHitException();
            }
            private set
            {
                hitPoint = value;
            }
        }

        public float X
        {
            get
            {
                return Point.X;
            }
        }
        public float Y
        {
            get
            {
                return Point.Y;
            }
        }

        private bool isHit;

        public RayHit(Tray ray)
        {
            this.ray = ray;
            this.isHit = false;
        }
        public RayHit(Tray ray, Vector2 hit)
        {
            this.ray = ray;
            this.Point = hit;
            this.isHit = true;
        }
        public RayHit(Tray ray, Vector2 hit, bool ishit)
        {
            this.ray = ray;
            this.Point = hit;
            this.isHit = ishit;
        }
        public static bool operator true(RayHit<Tray> hit)
        {
            return hit.isHit;
        }
        public static bool operator false(RayHit<Tray> hit)
        {
            return !hit.isHit;
        }

        public class NoHitException : System.Exception
        {

        }
    }
}