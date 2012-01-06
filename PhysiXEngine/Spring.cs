using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class Spring : Constaint
    {
        /**
         * The point of connection of the spring to the other object,
         * in that object's local coordinates.
         */
        public Vector3 otherConnectionPoint { get; private set; }

        /** The particle at the first end of the spring. */
        public Body first { get; private set; }

        /** The particle at the other end of the spring. */
        public Body other { get; private set; }

        /** Holds the sprint constant. */
        public float springConstant { get; private set; }

        /** Holds the rest length of the spring. */
        public float restLength { get; private set; }

        /** Creates a new spring with the given parameters. */
        public Spring(Body first, Body other, Vector3 otherConnectionPoint,
            float springConstant, float restLength) : base(first, other)
        {
            this.first = first;
            this.other = other;
            this.otherConnectionPoint = otherConnectionPoint;
            this.springConstant = springConstant;
            this.restLength = restLength;
        }

        /** Applies the spring force to the given rigid body. */
        protected override void Affect()
        {
            // Calculate the two ends in world space
            Vector3 lws = first.GetPointInWorldSpace(first.Position);
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
            first.AddForce(secondforce, lws);
            force *= magnitude;
            other.AddForce(force, ows);
        }
    }
}
