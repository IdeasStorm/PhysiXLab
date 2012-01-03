using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    class Box : Collidable
    {
        public ContactData generateContacts(Collidable other)
        {
            ContactData contactData = null;
            if (other as Box != null)
            {
                contactData = new ContactData(other, this);
                contactData.BoxAndBox();
            }
            if (other as Sphere != null)
            {
                contactData = new ContactData(other, this);
                contactData.BoxAndSphere();
            }
            if (other as Plane != null)
            {
                contactData = new ContactData(other, this);
                contactData.BoxHalfSpace();
            }
            return contactData;
        }
    }
}
