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
        protected float duration;

        public void AddBody(Body body)
        {
            bodies.Add(body);
        }

        public void RemoveBody(Body body)
        {
            bodies.Remove(body);
        }

        public void Update(float time)
        {
            duration = time;
            foreach (Body body in bodies)
            {
                Affect(body);
            }
        }

        public void Affect(Body body);
        public void Affect(ContactData contacData);

    }
}
