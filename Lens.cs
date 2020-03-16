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

        //public static (Tray reflection, Tray refraction, bool isFullRefraction) ReflectAndRefract<Tray>(Tray ray, Vector2 normal, float n1, float n2);
        public abstract void HandleRay(LightRay ray,List<RayHit<LightRay>> hits,List<LightRay> secondaryRays);
    }
}