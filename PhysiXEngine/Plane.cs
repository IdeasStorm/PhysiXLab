using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    class Plane : Collidable
    {
        public Vector3 direction { public get; protected set; }
        public double offset { public get; protected set; }

        public ContactData generateContacts(Collidable other)
        {
            ContactData contactData = null;
            if (other as Sphere != null)
            {
                contactData = new ContactData(other, this);
                contactData.SphereAndPlane();
            }
            if (other as Box != null)
            {
                contactData = new ContactData(other, this);
                contactData.BoxHalfSpace();
            }
            return contactData;
        }
    }
}
