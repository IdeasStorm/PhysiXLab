using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace PhysiXEngine
{
    class ContactData
    {
        /**
         * Holds the bodies that are involved in the contact. The
         * second of these can be NULL, for contacts with the scenery.
         */
        private Collidable[] body = new Collidable[2];

        /**
         * Holds the position of the contact in world coordinates.
         */
        private Vector3 contactPoint;

        /**
         * Holds the direction of the contact in world coordinates.
         */
        private Vector3 contactNormal;

        /**
         * Holds the depth of penetration at the contact point. If both
         * bodies are specified then the contact point should be midway
         * between the inter-penetrating points.
         */
        private double penetration;

        public ContactData(Collidable firstBody, Collidable secondBody)
        {
            this.body[0] = firstBody;
            this.body[1] = secondBody;
        }

        public void SphereAndSphere()
        {
            //Cache the sphere positions
            Vector3 positionOne = ((Sphere)body[0]).Position;
            Vector3 positionTwo = ((Sphere)body[1]).Position;

            // Find the vector between the objects
            Vector3 midline = positionOne - positionTwo;
            double size = midline.Length();

            // We manually create the normal, because we have the
            // size to hand.
            contactNormal = Vector3.Multiply(midline,(float)(1.0f/size));
            contactPoint = positionOne + Vector3.Multiply(midline,0.5f);
            penetration = ((Sphere)body[0]).radius + ((Sphere)body[1]).radius - size;
        }

        public void SphereAndPlane()
        {
            Sphere sphere = (Sphere)body[0];
            Plane plane = (Plane)body[1];

            // Cache the sphere position
            Vector3 position = sphere.Position;

            // Find the distance from the plane
            double centreDistance = Vector3.Dot(plane.direction, position) - plane.offset;

            // Check which side of the plane we're on
            contactNormal = plane.direction;
            penetration = -centreDistance;
            if (centreDistance < 0)
            {
                contactNormal *= -1;
                penetration = -penetration;
            }
            penetration += sphere.radius;

            contactPoint = position - Vector3.Multiply(plane.direction, (float)centreDistance);
        }
    }
}
