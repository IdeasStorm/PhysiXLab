using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    public class Sphere : Collidable
    {
        public float radius { get; protected set; }

        public Sphere(float radius)
        {
            this.radius = radius;
        }

        public override ContactData generateContacts(Collidable other) 
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

        public override Boolean CollidesWith(Collidable other)
        {
            //TODO add Detection logic
            return false;
        }
    }
}