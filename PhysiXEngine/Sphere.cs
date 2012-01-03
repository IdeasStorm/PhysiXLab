using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    class Sphere : Collidable
    {
        public double radius { public get; protected set; }

        public ContactData generateContacts(Collidable other) 
        {
            ContactData contactData = null;
            if (other as Sphere != null)
            {
                contactData = new ContactData(this, other);
                contactData.SphereAndSphere();
            }
            if (other as Plane != null)
            {
                contactData = new ContactData(this, other);
                contactData.SphereAndPlane();
            }
            if (other as Box != null)
            {
                contactData = new ContactData(this, other);
                contactData.BoxAndSphere();
            }
            return contactData;
        }
    }
}
