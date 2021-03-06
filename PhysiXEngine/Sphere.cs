using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class Sphere : Collidable
    {
        public float radius { get; protected set; }
        private BoundingSphere _sphere;
        public BoundingSphere sphere { get { return _sphere; } protected set { _sphere = value; } }

        public override string ToString()
        {
            return "Sphere|" + "R=" + radius.ToString() + '|' + base.ToString();
        }

        public Sphere(string S)
            : base(S)
        {
            string[] Temp = S.Split('|');
            this.radius = float.Parse(Temp[Temp.Length - 3].Substring(2));

            sphere = new BoundingSphere(Position, radius);
            float coeff = 0.4f * Mass * radius * radius;
            setInertiaTensorCoeffs(coeff, coeff, coeff);
            UpdateMatices();
        }

        /// <summary>
        /// Creates a sphere
        /// </summary>
        /// <param name="radius">the radius of a sphere</param>
        /// <param name="mass">the mass of sphere</param>
        public Sphere(float radius, float mass = 1)
        {
            this.radius = radius;
            sphere = new BoundingSphere(Position, radius);
            Mass = mass;            
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
            onSituationChanged();
        }

        protected override void updateBounding()
        {
            _sphere.Center = this.Position;
            _sphere.Radius = radius;
        }

        public override ContactData generateContacts(Collidable other) 
        {
            Contact contact = new Contact(this, other);
            if (other as Sphere != null)
            {
                contact.SphereAndSphere();
            }
            else if (other as Box != null)
            {
                contact.SphereAndBox();
            }
            return contact.GetContactData();
        }

        public override bool generateContacts(Collidable other, Contact contact)
        {
            if (other as Sphere != null)
            {
                return contact.SphereAndSphere();
            }
            else if (other as Box != null)
            {
                return contact.SphereAndBox();
            }
            return false;
        }

        public override Boolean CollidesWith(Collidable other)
        {
            if (other as Sphere != null)
            {
                return true;
            }
            else if (other as Box != null)
            {
                return true;
            }
            return false;
        }

        public override Boolean CollidesWith(HalfSpace plane)
        {
            //TOOD add CollidesWith() code here
            PlaneIntersectionType p = sphere.Intersects(plane.plane);
            if (p == PlaneIntersectionType.Intersecting)
            {
                return true;
            }
            return false;
        }

        public override BoundingSphere GetBoundingSphere()
        {
            return sphere;
        }

        public override Vector3 getHalfSize()
        {
            return new Vector3(radius, radius, radius);
        }

        public override void onMassChanged()
        {
            base.onMassChanged();
            if (InverseMass == 0)
            {
                InverseInertiaTensor = new Matrix();
                return;
            }
            float coeff = 0.4f * Mass * radius * radius;
            setInertiaTensorCoeffs(coeff, coeff, coeff);
        }
    }
}