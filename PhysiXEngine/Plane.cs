using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class HalfSpace
    {
        public Vector3 direction { get; protected set; }
        public float offset { get; protected set; }
        public Plane plane { get; protected set; }

        public HalfSpace(Plane plane)
        {
            this.plane = plane;
            this.direction = direction;
            this.offset = plane.D;
        }
        /*
        protected override void updateBounding()
        {
            //TODO add update logic
        }

        public override Contact generateContacts(Collidable other)
        {
            Contact contactData = null;
            
            return contactData;
        }
        */
        public void generateContacts(Collidable other, Contact contact)
        {
            if (other as Sphere != null)
            {
                contact.SphereAndPlane();
            }
            if (other as Box != null)
            {
                contact.BoxAndHalfSpace();
            }
        }

        public ContactData generateContacts(Collidable other)
        {
            Contact contact = new Contact(other, this.plane);
            if (other as Sphere != null)
            {
                contact.SphereAndPlane();
            }
            if (other as Box != null)
            {
                contact.BoxAndHalfSpace();
            }
            return contact.GetContactData();
        }
        /*
        public override Boolean CollidesWith(Collidable other)
        {
            return false;
        }

        public BoundingBox GetBounding<T>()
        {
            return plane;
        }
        public override BoundingSphere GetBoundingSphere()
        {
            return BoundingSphere.CreateFromBoundingBox(plane);
        }
         */
    }
}
