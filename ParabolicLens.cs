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
        }



        public override Ray HandleRay(Ray ray)
        {

            var hit = leftSurface.Intersection<Ray>(ray);
            if (hit)
            {
                var normal = leftSurface.Normal(hit.Point.Y);
                var reflection = new Ray(hit.Point, Vector2.Reflect(ray.Direction,normal));
                // znau chto ne tak on otrazaetsya no kakayz seichas raznitsa
                var incidenceAngle = (float)Math.Atan2(normal.X - hit.ray.Direction.X, normal.Y - hit.ray.Direction.Y)+Math.PI/2f;
                var s = Math.Sin(incidenceAngle);
                // sini = n*sinr  sinr = (sin i) / n
                var refractionAngle = (float)Math.Asin(Math.Sin(incidenceAngle) / refractiveIndex(hit.Y));

                var innerRay = new Ray(hit.Point, Vector2.Transform(-normal, Matrix3x2.CreateRotation(refractionAngle)));


                // TODO second lens


                return innerRay;
            }
            return new Ray(Vector2.Zero, Vector2.UnitX);
        }
    }
}
