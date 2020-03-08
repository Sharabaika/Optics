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

        protected Vector2 Center;

        /// <summary>
        /// Height above main axis
        /// </summary>
        protected float height = 5f;

        protected Lens(Func<float, float> refractiveIndex, Vector2 center, float height)
        {
            this.refractiveIndex = (x) => 1.33f;//refractiveIndex;
            Center = center;
            this.height = height;
        }

        public abstract Ray HandleRay(Ray ray);
    }
}