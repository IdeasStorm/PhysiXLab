using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    interface Collidable
    {
        public ContactData generateContacts(Collidable other);
    }
}
