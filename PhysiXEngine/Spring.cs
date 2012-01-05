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
         * The point of connection of the spring, in local
         * coordinates.
         */
        public Vector3 connectionPoint { get; private set; }

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
        Spring(Vector3 localConnectionPoint, Body other,
               Vector3 otherConnectionPoint, float springConstant,
               float restLength)
        {
            this.connectionPoint = localConnectionPoint;
            this.other = other;
            this.otherConnectionPoint = otherConnectionPoint;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        public override void Affect(Body other)
        {
        }

        /** Applies the spring force to the given rigid body. */
        public override void Affect(Body body, float duration)
        {
            // Calculate the two ends in world space
            Vector3 lws = body.GetPointInWorldSpace(connectionPoint);
            Vector3 ows = other.GetPointInWorldSpace(otherConnectionPoint);

            // Calculate the vector of the spring
            Vector3 force = lws - ows;

            // Calculate the magnitude of the force
            float magnitude = force.Length();
            magnitude = Math.Abs(magnitude - restLength);
            magnitude *= springConstant;

            // Calculate the final force and apply it
            force.Normalize();
            force *= -magnitude;
            body.AddForceAtPoint(force, lws);
        }
    }
}
