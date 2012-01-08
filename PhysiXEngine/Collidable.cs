using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public abstract class Collidable : Body
    {
        public abstract ContactData generateContacts(Collidable other);
        public abstract Boolean CollidesWith(Collidable other);
        public abstract BoundingSphere GetBoundingSphere();
    }
}
