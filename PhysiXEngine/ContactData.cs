using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public struct ContactData
    {
        /**
         * Holds the position of the contact in world coordinates.
         */
        public Vector3 ContactPoint { get; set; }

        /**
         * Holds the direction of the contact in world coordinates.
         */
        public Vector3 ContactNormal { get; set; }

        /**
         * Holds the depth of penetration at the contact point. If both
         * bodies are specified then the contact point should be midway
         * between the inter-penetrating points.
         */
        public float Penetration { get; set; }
    }
}
