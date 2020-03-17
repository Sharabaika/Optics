using System.Collections;
using System.Collections.Generic;
using System;
using System.Numerics;

namespace Optics
{
    public abstract class Lens
    {
        protected Func<float, float> refractiveIndex;

        public Vector2 Center { get; protected set; }

        /// <summary>
        /// Height above main axis
        /// </summary>
        public float Height { get; protected set; }

        protected Lens(Func<float, float> refractiveIndex, Vector2 center, float height)
        {
            this.refractiveIndex = refractiveIndex;
            Center = center;
            this.Height = height;
        }

        public abstract bool IsIntersects<Tray>(Tray ray) where Tray : Ray;
        public static (LightRay reflection, LightRay refraction, bool isFullRefraction) ReflectAndRefract(LightRay ray, Vector2 normal,Vector2 point, float n1, float n2)
        {
            var incidenceAngle = (float)(Math.Atan2(normal.Y, normal.X) - Math.Atan2(-ray.Direction.Y, -ray.Direction.X));
            while (Math.Abs(incidenceAngle) > Math.PI / 2f)
                incidenceAngle = incidenceAngle - 2f * (float)Math.PI * Math.Sign(incidenceAngle);
            var refractionAngle = (float)Math.Asin(Math.Sin(incidenceAngle) * n1/ n2);

            float reflCoeficent = (float)(Math.Sin(Math.Abs(incidenceAngle) - Math.Abs(refractionAngle)) /
                                  (Math.Sin(Math.Abs(incidenceAngle) + Math.Abs(refractionAngle))));

            bool isFullReflection = false;
            if (float.IsNaN(refractionAngle))
            {
                isFullReflection = true;
                reflCoeficent = 1;
            }
            if(incidenceAngle == 0f)
            {
                reflCoeficent = 0;
                isFullReflection = false;
            }

            var reflectionDirection = Vector2.Reflect(ray.Direction, normal);
            var reflection = new LightRay(point, reflectionDirection, ray.Intensity * reflCoeficent * reflCoeficent);

            var refraction = new LightRay(point, Vector2.Transform(-normal, Matrix3x2.CreateRotation(-refractionAngle)), ray.Intensity - reflection.Intensity);

            return (reflection, refraction, isFullReflection);
        }

        public abstract void HandleRay(LightRay ray,List<RayHit<LightRay>> hits,List<LightRay> secondaryRays);
    }
}