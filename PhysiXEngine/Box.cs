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
        public Vector3 HalfSize { get; private set; }
        public BoundingBox box { get; private set; }        

        public override ContactData generateContacts(Collidable other)
        {
            ContactData contactData = null;
            if (other as Box != null)
            {
                contactData = new ContactData(other, this);
                contactData.BoxAndBox();
            }
            if (other as Sphere != null)
            {
                contactData = new ContactData(other, this);
                contactData.SphereAndBox();
            }
            if (other as Plane != null)
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

        public BoundingBox GetBounding()
        {
            return box;
        }
    }
}
