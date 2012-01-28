using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    using PhysiXEngine.Helpers;
    public class Box : Collidable
    {
        public Vector3 HalfSize { get; protected set; }
        private BoundingBox _box = new BoundingBox();
        public BoundingBox box { get { return _box; } private set { _box = value; } }

        public Box(Vector3 halfSize)
        {
            HalfSize = halfSize;
            updateBounding();
        }

        public override ContactData generateContacts(Collidable other)
        {
            Contact contact = null;
            contact = new Contact(this, other);
            if (other as Box != null)
            {
                contact.BoxAndBox();
            }
            if (other as Sphere != null)
            {
                contact.SphereAndBox();
            }
            return contact.GetContactData();
        }

        protected override void updateBounding()
        {
            _box.Min = Vector3.Transform(-HalfSize, TransformMatrix);
            _box.Max = Vector3.Transform(HalfSize, TransformMatrix);
        }

        public override Boolean CollidesWith(Collidable other)
        {
            //TOOD add CollidesWith() code here
            if (other as Box != null)
            {
                //box.Intersects(((Sphere)other).GetBoundingSphere);
                return box.Intersects(((Box)other).box);
            }
            else if (other as Sphere != null)
            {
                return box.Intersects(((Sphere)other).GetBoundingSphere());
            }
            return false;
        }

        public override Boolean CollidesWith(HalfSpace plane)
        {
            //TOOD add CollidesWith() code here
            PlaneIntersectionType p = box.Intersects(plane.plane);
            if (p == PlaneIntersectionType.Intersecting)
            {
                return true;
            }
            return false;
        }

        public override BoundingSphere GetBoundingSphere()
        {
            return BoundingSphere.CreateFromBoundingBox(box);
        }

        public override void generateContacts(Collidable other, Contact contact)
        {
            if (other as Box != null)
            {
                contact.BoxAndBox();
            }
            if (other as Sphere != null)
            {
                contact.SphereAndBox();
            }
        }
    }
}