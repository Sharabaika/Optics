using System;
using System.Collections.Generic;
using System.Text;

namespace Optics
{
    class OpticSystem
    {
        LightSource lightSource;
        List<Lens> lenses;
        List<Ray> rays;

        public OpticSystem(LightSource lightSource, List<Lens> lenses)
        {
            this.lightSource = lightSource;
            this.lenses = lenses;
            rays = new List<Ray>();
        }
    }
}
