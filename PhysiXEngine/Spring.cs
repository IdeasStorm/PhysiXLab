using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class Spring : Constraint
    {
        /**
         * The point of connection of the spring to the other object,
         * in that object's local coordinates.
         */
        public Vector3 otherConnectionPoint { get; private set; }

        /**
         * The point of connection of the spring to the object,
         * in that object's local coordinates.
         */
        public Vector3 ConnectionPoint { get; private set; }

        /** Holds the sprint constant. */
        public float springConstant { get; private set; }

        /** Holds the rest length of the spring. */
        public float restLength { get; private set; }

        public float C { get; private set; }

        /** Creates a new spring with the given parameters. */
        public Spring(Body first, Body other, float springConstant, float restLength, float C, 
            Vector3 ConnectionPoint = new Vector3(), Vector3 otherConnectionPoint = new Vector3()) : 
            base(
            /** The particle at the first end of the spring. */
            first,
            /** The particle at the other end of the spring. */
            other)
        {
            this.otherConnectionPoint = otherConnectionPoint;
            this.ConnectionPoint = ConnectionPoint;
            this.springConstant = springConstant;
            this.restLength = restLength;
            this.C = C;
        }

        /** Applies the spring force to the given rigid body. */
        protected override void Affect()
        {
            //TODO Add GetPointInWorldSpace for box
            // Calculate the two ends in world space
            //Vector3 lws = bodys[0].Position;
            Vector3 lws = bodys[0].GetPointInWorldSpace(ConnectionPoint);
            //Vector3 ows = bodys[1].Position;
            Vector3 ows = bodys[1].GetPointInWorldSpace(otherConnectionPoint);

            // Calculate the vector of the spring
            Vector3 force = lws - ows;

            // Calculate the magnitude of the force
            float magnitude = force.Length();
            magnitude = Math.Abs(magnitude - restLength);
            magnitude *= springConstant;

            // Calculate the final force and apply it
            force.Normalize();
            Vector3 secondforce = force * -magnitude;
            bodys[0].AddForce(secondforce, lws);
            force *= magnitude;
            bodys[1].AddForce(force, ows);
            if (bodys[0].InverseMass != 0)
                bodys[0].AddForce(Vector3.Multiply(bodys[0].Velocity, (C - 1f) * bodys[0].Mass));
            if (bodys[1].InverseMass != 0)
                bodys[1].AddForce(Vector3.Multiply(bodys[1].Velocity, (C - 1f) * bodys[0].Mass));
        }
    }
}
