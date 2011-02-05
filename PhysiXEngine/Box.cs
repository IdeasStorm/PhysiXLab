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

        public override string ToString()
        {
            return "Box|"
                 + "HS=" + HalfSize.X + '!' + HalfSize.Y + '!' + HalfSize.Z + '|'
                 + base.ToString();
        }

        public Box(string S)
            : base(S)
        {
            string[] Temp = S.Split('|');
            string[] HS = Temp[Temp.Length - 3].Substring(3).Split('!');
            this.HalfSize = new Vector3(float.Parse(HS[0]), float.Parse(HS[1]), float.Parse(HS[2]));
            updateBounding();
        }


        public Box(Vector3 halfSize,float mass = 1)
        {
            HalfSize = halfSize;
            updateBounding();
            Mass = mass;
        }

        public void SetHalfSize(Vector3 halfSize)
        {
            this.HalfSize = halfSize;
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

        public override void onMassChanged()
        {
            base.onMassChanged();
            if (InverseMass == 0)
            {
                InverseInertiaTensor = new Matrix();
                return;
            }
            Vector3 squaredSize = HalfSize * HalfSize;
            this.setInertiaTensorCoeffs(
                0.3f * Mass * (squaredSize.Y + squaredSize.Z),
                0.3f * Mass * (squaredSize.X + squaredSize.Z),
                0.3f * Mass * (squaredSize.X + squaredSize.Y));
        }
    }
}