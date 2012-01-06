using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysiXEngine
{
    public class Sphere : Collidable
    {
        public float radius { get; protected set; }

        /// <summary>
        /// Creates a sphere
        /// </summary>
        /// <param name="radius">the radius of a sphere</param>
        /// <param name="mass">the mass of sphere</param>
        public Sphere(float radius, float mass=1)
        {
            this.radius = radius;
            float coeff = 0.4f * Mass * radius * radius;
            setInertiaTensorCoeffs(coeff, coeff, coeff);
            calculateDerivedData();
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