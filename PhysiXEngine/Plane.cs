using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class Plane : Collidable
    {
        public Vector3 direction { get; protected set; }
        public float offset { get; protected set; }
        public BoundingBox plane { get; protected set; }

        public Plane(Vector3 direction, float offset)
        {
            plane = new BoundingBox(Position, Vector3.One);
            this.direction = direction;
            this.offset = offset;
        }

        public override ContactData generateContacts(Collidable other)
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
                contactData.BoxAndHalfSpace();
            }
            return contactData;
        }
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
    }
}
