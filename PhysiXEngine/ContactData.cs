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
            Vector3 positionOne = ((Body)body[0]).position;
            Vector3 positionTwo = ((Body)body[1]).position;

            // Find the vector between the objects
            Vector3 midline = positionOne - positionTwo;
            double size = midline.Length();

            // We manually create the normal, because we have the
            // size to hand.
            contactNormal = Vector3.Multiply(midline,(float)(1.0f/size));
            contactPoint = positionOne + Vector3.Multiply(midline,0.5f);
            penetration = ((Sphere)body[0]).radius + ((Sphere)body[1]).radius - size;
        }
    }
}
