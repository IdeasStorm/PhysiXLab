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
            // todo : no code should be here
        }

        public override Boolean CollidesWith(Collidable other)
        {
            //TOOD add CollidesWith() code here
            if (other as Box != null)
            {
                //box.Intersects(((Sphere)other).GetBoundingSphere);
                return true;
            }
            else if (other as Sphere != null)
            {
                return true;
            }
            return false;
        }

        public override Boolean CollidesWith(HalfSpace plane)
        {
            return true;
        }

        public override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Position,HalfSize.Length());
        }

        public override bool generateContacts(Collidable other, Contact contact)
        {
            if (other as Box != null)
            {
                return contact.BoxAndBox();
            }
            else if (other as Sphere != null)
            {
                return contact.SphereAndBox();
            }
            return false;
        }

        public override Vector3 getHalfSize()
        {
            return HalfSize;
        }
    }
}