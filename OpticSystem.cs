using System;
using System.Numerics;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Optics
{
    public class OpticSystem
    {
        List<Lens> lenses;

        public OpticSystem(List<Lens> lenses)
        {
            this.lenses = lenses;
        }

        public void HandleRay(LightRay ray, List<RayHit<LightRay>> hits, List<LightRay> rays)
        {
            Stack<(LightRay ray, int indexOfSender)> raysToHandle = new Stack<(LightRay ray, int index)>();

            raysToHandle.Push((ray, -1));


            List<LightRay> tmpRays = new List<LightRay>();
            List<RayHit<LightRay>> tmpHits = new List<RayHit<LightRay>>();


            while (raysToHandle.Count > 0)
            {
                (LightRay r, int currentIndex) = raysToHandle.Pop();

                int[] mayIntersect = { currentIndex - 1, currentIndex + 1 };
                foreach (var i in mayIntersect)
                {
                    tmpHits.Clear();
                    tmpRays.Clear();

                    if (i >= 0 && i < lenses.Count)
                    {
                        lenses[i].HandleRay(r, tmpHits, tmpRays);
                        if (tmpHits.Count > 0)
                        {
                            foreach (var rayToHandle in tmpRays)
                            {
                                raysToHandle.Push((rayToHandle, i));
                            }
                            foreach (var hit in tmpHits)
                            {
                                hits.Add(hit);
                            }
                            break;
                        }
                        else
                        {
                            rays.Add(r);
                        }
                    }
                }

            }

        }
    }
}
