using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    abstract class Effect
    {
        protected HashSet<Body> bodies;

        public void addBody(Body body)
        {
            bodies.Add(body);
        }

        public void removeBody(Body body)
        {
            bodies.Remove(body);
        }

        public void update(float time)
        {
            foreach (Body body in bodies)
            {
                affect(body);
            }
        }

        public void affect(Body body);


    }
}
