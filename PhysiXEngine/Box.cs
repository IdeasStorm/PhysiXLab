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
        public Matrix transform{ get; private set; }

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
                contactData.BoxAndSphere();
            }
            if (other as Plane != null)
            {
                contactData = new ContactData(other, this);
                contactData.BoxHalfSpace();
            }
            return contactData;
        }

        public override Boolean CollidesWith(Collidable other)
        {
            return false;
        }

        /// <summary>
        /// gets a vector of an axis by index (from 0 to 2)      
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetAxis(int index)
        {
            return TransformMatrix.GetAxisVector(index);
        }
    }
}
