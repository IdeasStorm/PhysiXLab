using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    abstract class Collidable : Body
    {
        public ContactData generateContacts(Collidable other);
        public Boolean CollidesWith(Collidable other);
    }
}
