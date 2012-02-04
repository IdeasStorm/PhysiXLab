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

        /// <summary>
        /// Creates a sphere
        /// </summary>
        /// <param name="radius">the radius of a sphere</param>
        /// <param name="mass">the mass of sphere</param>
        public Sphere(float radius, float mass=1)
        {
            this.radius = radius;
            sphere = new BoundingSphere(Position, radius);
            Mass = mass;
            float coeff = 0.4f * Mass * radius * radius;
            setInertiaTensorCoeffs(coeff, coeff, coeff);
            UpdateMatices();
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        protected override void updateBounding()
        {
            _sphere.Center = this.Position;
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

        public override void generateContacts(Collidable other, Contact contact)
        {
            if (other as Sphere != null)
            {
                contact.SphereAndSphere();
            }
            else if (other as Box != null)
            {
                contact.SphereAndBox();
            }
        }

        public override Boolean CollidesWith(Collidable other)
        {
            if (other as Sphere != null)
            {
                return sphere.Intersects(other.GetBoundingSphere());
            }
            else if (other as Box != null)
            {
                return sphere.Intersects(((Box)other).box);
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
    }
}