using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physix
{
    /**
    * A contact represents two bodies in contact. Resolving a
    * contact removes their interpenetration, and applies sufficient
    * impulse to keep them apart. Colliding bodies may also rebound.
    * Contacts can be used to represent positional joints, by making
    * the contact constraint keep the bodies in their correct
    * orientation.
    */
    class Contact
    {
        /**
         * Holds the bodies that are involved in the contact. The
         * second of these can be NULL, for contacts with the scenery.
         */
        public RigidBody[] body = new RigidBody[2];

        /**
         * Holds the lateral friction coefficient at the contact.
         */
        public Double friction;

        /**
         * Holds the normal restitution coefficient at the contact.
         */
        public Double restitution;

        /**
         * Holds the position of the contact in world coordinates.
         */
        public Vector3 contactPoint;

        /**
         * Holds the direction of the contact in world coordinates.
         */
        public Vector3 contactNormal;

        /**
         * Holds the depth of penetration at the contact point. If both
         * bodies are specified then the contact point should be midway
         * between the inter-penetrating points.
         */
        public Double penetration;

        /**
         * A transform matrix that converts co-ordinates in the contact's
         * frame of reference to world co-ordinates. The columns of this
         * matrix form an orthonormal set of vectors.
         */
        protected Matrix contactToWorld;

        /**
         * Holds the closing velocity at the point of contact. This is set
         * when the calculateInternals function is run.
         */
        protected Vector3 contactVelocity;

        /**
         * Holds the required change in velocity for this contact to be
         * resolved.
         */
        protected Double desiredDeltaVelocity;

        /**
         * Holds the world space position of the contact point relative to
         * centre of each body. This is set when the calculateInternals
         * function is run.
         */
        protected Vector3[] relativeContactPosition = new Vector3[2];


        /**
         * Sets the data that doesn't normally depend on the position
         * of the contact (i.e. the bodies, and their material properties).
         */
        public void setBodyData(RigidBody one, RigidBody two, Double friction, Double restitution)
        {
            this.body[0] = one;
            this.body[1] = two;
            this.friction = friction;
            this.restitution = restitution;
        }

        /**
         * Calculates internal data from state data. This is called before
         * the resolution algorithm tries to do any resolution. It should
         * never need to be called manually.
         */
        protected void calculateInternals(Double duration)
        {
            // Check if the first object is NULL, and swap if it is.
            if (!body[0])
            {
                swapBodies();
            }
            //assert(body[0]);

            // Calculate an set of axis at the contact point.
            calculateContactBasis();

            // Store the relative position of the contact relative to each body
            relativeContactPosition[0] = contactPoint - body[0].getPosition();
            if (body[1])
            {
                relativeContactPosition[1] = contactPoint - body[1].getPosition();
            }

            // Find the relative velocity of the bodies at the contact point.
            contactVelocity = calculateLocalVelocity(0, duration);
            if (body[1])
            {
                contactVelocity -= calculateLocalVelocity(1, duration);
            }

            // Calculate the desired change in velocity for resolution
            calculateDesiredDeltaVelocity(duration);
        }

        /**
         * Reverses the contact. This involves swapping the two rigid bodies
         * and reversing the contact normal. The internal values should then
         * be recalculated using calculateInternals (this is not done
         * automatically).
         */
        protected void swapBodies()
        {
            contactNormal *= -1;

            RigidBody temp = body[0];
            body[0] = body[1];
            body[1] = temp;
        }

        /**
         * Updates the awake state of rigid bodies that are taking
         * place in the given contact. A body will be made awake if it
         * is in contact with a body that is awake.
         */
        protected void matchAwakeState()
        {
            // Collisions with the world never cause a body to wake up.
            if (!body[1])
                return;

            bool body0awake = body[0].getAwake();
            bool body1awake = body[1].getAwake();

            // Wake up only the sleeping one
            if (body0awake ^ body1awake)
            {
                if (body0awake)
                {
                    body[1].setAwake();
                }
                else
                {
                    body[0].setAwake();
                }
            }
        }

    }
}