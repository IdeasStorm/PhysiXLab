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
            calculateDerivedData();
        }

        protected override void updateBounding()
        {
            _sphere.Center = Vector3.Transform(Vector3.Zero, TransformMatrix);
        }

        public override Contact generateContacts(Collidable other) 
        {
            Contact contactData = null;
            if (other as Sphere != null)
            {
                contactData = new Contact(this, other);
                contactData.SphereAndSphere();
            }
            if (other as Plane != null)
            {
                contactData = new Contact(this, other);
                contactData.SphereAndPlane();
            }
            if (other as Box != null)
            {
                contactData = new Contact(this, other);
                contactData.SphereAndBox();
            }
            return contactData;
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