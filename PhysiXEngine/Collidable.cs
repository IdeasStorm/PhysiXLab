using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    abstract class Collidable : Body
    {
        public abstract ContactData generateContacts(Collidable other);
        public abstract Boolean CollidesWith(Collidable other);
    }
}
