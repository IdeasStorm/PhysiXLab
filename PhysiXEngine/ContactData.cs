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
        private Body[] body = new Body[2];

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

        public ContactData(Body firstBody, Body secondBody) {
            this.body[0] = firstBody;
            this.body[1] = secondBody;
        }
    }
}
