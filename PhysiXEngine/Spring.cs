using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class Spring : ForceGenerator
    {
        /**
         * The point of connection of the spring to the other object,
         * in that object's local coordinates.
         */
        public Vector3 otherConnectionPoint { get; private set; }

        /** The particle at the other end of the spring. */
        public Body other { get; private set; }

        /** Holds the sprint constant. */
        public float springConstant { get; private set; }

        /** Holds the rest length of the spring. */
        public float restLength { get; private set; }

        /** Creates a new spring with the given parameters. */
        public Spring(Body other, Vector3 otherConnectionPoint,
            float springConstant, float restLength)
        {
            this.other = other;
            this.otherConnectionPoint = otherConnectionPoint;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        /** Applies the spring force to the given rigid body. */
        protected override void Affect(Body body)
        {
            // Calculate the two ends in world space
            Vector3 lws = body.GetPointInWorldSpace(body.Position);
            Vector3 ows = other.GetPointInWorldSpace(other.Position);

            // Calculate the vector of the spring
            Vector3 force = lws - ows;

            // Calculate the magnitude of the force
            float magnitude = force.Length();
            magnitude = Math.Abs(magnitude - restLength);
            magnitude *= springConstant;

            // Calculate the final force and apply it
            force.Normalize();
            Vector3 secondforce = force * -magnitude;
            body.AddForce(secondforce, lws);
            force *= magnitude;
            other.AddForce(force, ows);
        }
    }
}
