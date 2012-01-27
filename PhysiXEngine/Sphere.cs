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
            
            float coeff = 0.4f * Mass * radius * radius;
            setInertiaTensorCoeffs(coeff, coeff, coeff);
            UpdateMatices();
        }

        protected override void updateBounding()
        {
            _sphere.Center = Vector3.Transform(Vector3.Zero, TransformMatrix);
        }

        public override Contact generateContacts(Collidable other) 
        {
            Contact contactData = null;
            
            return contactData;
        }

        public override void generateContacts(Collidable other, Contact contact)
        {
            if (other as Sphere != null)
            {
                contact.SphereAndSphere();
            }
            /*if (other as HalfSpace != null)
            {
                contact.SphereAndPlane();
            }*/
            if (other as Box != null)
            {
                contact.SphereAndBox();
            }
        }

        public override Boolean CollidesWith(Collidable other)
        {
            //TODO add Detection logic
            return false;
        }

        public override BoundingSphere GetBoundingSphere()
        {
            return sphere;
        }
    }
}