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
        private BoundingBox _box;
        public BoundingBox box { get { return _box; } private set { _box = value; } }

        public Box(Vector3 halfSize)
        {
            HalfSize = halfSize;
            updateBounding();
        }

        public override Contact generateContacts(Collidable other)
        {
            Contact contactData = null;
            if (other as Box != null)
            {
                contactData = new Contact(other, this);
                contactData.BoxAndBox();
            }
            if (other as Sphere != null)
            {
                contactData = new Contact(other, this);
                contactData.SphereAndBox();
            }
            if (other as Plane != null)
            {
                contactData = new Contact(other, this);
                contactData.BoxAndHalfSpace();
            }
            return contactData;
        }

        protected override void updateBounding()
        {
            _box.Min = Vector3.Transform(-HalfSize, TransformMatrix);
            _box.Max = Vector3.Transform(HalfSize, TransformMatrix);
            //TODO be sure of meaning of halfSize @JOBORY
        }

        public override Boolean CollidesWith(Collidable other)
        {
            //TOOD add CollidesWith() code here
            return false;
        }

        public override BoundingSphere GetBoundingSphere()
        {
            return BoundingSphere.CreateFromBoundingBox(box);
        }
    }
}