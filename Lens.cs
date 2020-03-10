using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;

namespace Optics
{
    public abstract class Lens
    {
        protected Func<float, float> refractiveIndex= (x)=>1f;

        /// <summary>
        /// Left half of lens, right one is semetric about the center
        /// </summary>

        public Vector2 Center { get; protected set; }

        /// <summary>
        /// Height above main axis
        /// </summary>
        public float Height { get; protected set; }

        protected Lens(Func<float, float> refractiveIndex, Vector2 center, float height)
        {
            this.refractiveIndex = (x) => 1.33f;//refractiveIndex;
            Center = center;
            this.Height = height;
        }
        public abstract Ray HandleRay(Ray ray,List<RayHit<Ray>> hits,List<Ray> secondaryRays);
    }
}