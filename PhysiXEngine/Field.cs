using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public abstract class Field : Effect// : ForceGenerator
    {
        protected HashSet<Body> bodies;

        public Field() 
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
            frameDuration = duration;
            foreach (Body body in bodies)
            {
                Affect(body);
            }
        }

        protected abstract void Affect(Body other);
    }


}
