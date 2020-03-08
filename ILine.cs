using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Optics
{
    public interface ILine
    {
        Vector2 Point(float t);


        /// <summary>
        /// Returns inteersection as RayHit obj
        /// </summary>
        /// <typeparam name="Tray">Tray must be derive from Ray class</typeparam>
        /// <param name="ray"></param>
        RayHit<Tray> Intersection<Tray>(Tray ray) where Tray: Ray;

        Vector2 Normal(float t);
    }
}