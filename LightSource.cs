using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Optics
{
    public class LightSource
    {
        public Vector2 Position { get; private set;}

        public float RayRange { get; set; }

        public LightSource(Vector2 position, float rayRange)
        {
            Position = position;
            RayRange = rayRange;
        }

        /// <summary>
        /// Evenly spreads N of rays betwen a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        public List<Ray> CastRays(Vector2 a,Vector2 b, int N)
        {
            var rays = new List<Ray>();

            for (int i = 1; i < N+1; i++)
            {
                var v = Vector2.Lerp(a-Position, b-Position, (float)i / (N + 1f));
                rays.Add(new Ray(Position,v));
            }
            return rays;
        }
    }
}