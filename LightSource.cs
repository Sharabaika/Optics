using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Optics
{
    public class LightSource
    {
        private Vector2 position;
        public Vector2 Position { get => position; private set => position = value; }

        public int NumberOfBeams = 10;

        public List<Vector2> beamsDirections;

        public float RayRange = 100f;


        public LightSource(Vector2 position, List<Vector2> beamsDirections, float rayRange)
        {
            Position = position;
            this.beamsDirections = beamsDirections;
            RayRange = rayRange;
        }
    }
}