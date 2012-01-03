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
            ContactData contactData = new ContactData(this, other);
            
            return contactData;
        }
    }
}
