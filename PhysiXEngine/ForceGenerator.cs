using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    abstract class ForceGenerator : Effect
    {
        protected HashSet<Body> bodies;

        public ForceGenerator() 
        {
            bodies = new HashSet<Body>();
        }

        public void AddBody(Body body)
        {
            bodies.Add(body);
        }

        public void RemoveBody(Body body)
        {
            bodies.Remove(body);
        }

        public override void Update(float duration)
        {
            foreach (Body body in bodies)
            {
                Affect(body);
            }
        }

        public abstract void Affect(Body other);
        
    }
}
