using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    class Sphere : Body, Collidable
    {
        public ContactData generateContacts(Collidable other) {
            ContactData contactData = new ContactData(this, (Body)other);
            //TODO Add inormation of this Contacts
            return contactData;
        }
    }
}
