using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    class Sphere : Body, Collidable
    {
        public double radius { public get; protected set; }

        public ContactData generateContacts(Collidable other) {
            ContactData contactData = new ContactData(this, other);
            if (other as Sphere != null) {
                contactData.SphereAndSphere();
            }
            return contactData;
        }
    }
}
