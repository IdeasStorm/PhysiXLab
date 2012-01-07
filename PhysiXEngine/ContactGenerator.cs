using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PhysiXEngine.Helpers;

namespace PhysiXEngine
{
    public class ContactGenerator : Effect
    {
        protected LinkedList<ContactData> contactDataLinkedList;
        static float velocityLimit = 0.25f;

        public ContactGenerator()
        {
            contactDataLinkedList = new LinkedList<ContactData>();
        }

        public void AddContactData(ContactData contactdata)
        {
            contactDataLinkedList.AddLast(contactdata);
        }

        public void ClearContactData()
        {
            contactDataLinkedList.Clear();
        }

        /// <summary>
        /// passing all contacts list calling affect function
        /// to affect each body by the another
        /// </summary>
        /// <param name="time"></param>
        public override void Update(float time)
        {
            frameDuration = time;
            foreach (ContactData contactData in contactDataLinkedList)
            {
                Affect(contactData);
            }
        }

        public void Affect(ContactData contactData)
        {
            // Calculating Desired Delta Velocity
            contactData.InitializeAtMoment(frameDuration);
            float deltaVelocity = calculateDeltaVelocity(contactData);


        }

        /// <summary>
        /// Calculates and sets the internal value for the delta velocity.
        /// </summary>
        /// <param name="contactData">the contact Data that contains the bodies and contct informations</param>
        public float calculateDeltaVelocity(ContactData contactData)
        {
            Body body1 = contactData.body[0];
            Body body2 = contactData.body[1];

            // NewVelocityCalculation
            // Calculate the acceleration induced velocity accumulated this frame
            float velocityFromAcc = Vector3.Dot(body1.LastFrameAcceleration,contactData.ContactNormal) * frameDuration ;

            if (body2 != null)
            {
                velocityFromAcc -= Vector3.Dot(body2.LastFrameAcceleration, contactData.ContactNormal) * frameDuration;
            }

            // If the velocity is very slow, limit the restitution
            float thisRestitution = contactData.restitution;
            if (Math.Sqrt(contactData.contactVelocity.X) < velocityLimit)
            {
                thisRestitution = (float)0.0f;
            }

            // Combine the bounce velocity with the removed
            // acceleration velocity.
            float deltaVelocity;
            deltaVelocity = -contactData.contactVelocity.X - thisRestitution * ((contactData.contactVelocity.X - velocityFromAcc));
            contactData.desiredDeltaVelocity = deltaVelocity;
            return deltaVelocity;
        }

        private Vector3 CalculateFrictionlessImpulse(ContactData contactData, Matrix3[] inverseInertiaTensor)
        {
            Body one = contactData.body[0];
            Body two = contactData.body[1];
            Vector3 impulseContact;

            // Build a vector that shows the change in velocity in
            // world space for a unit impulse in the direction of the contact
            // normal.
            Vector3 deltaVelWorldOne = Vector3.Multiply(contactData.relativeContactPosition[0], contactData.ContactNormal);
            deltaVelWorldOne = inverseInertiaTensor[0].transform(deltaVelWorldOne);
            deltaVelWorldOne = Vector3.Multiply(deltaVelWorldOne, contactData.relativeContactPosition[0]);

            // Work out the change in velocity in contact coordiantes.
            float deltaVelocity = Vector3.Dot(deltaVelWorldOne, contactData.ContactNormal);

            // Add the linear component of velocity change
            deltaVelocity += one.InverseMass;

            // Check if we need to the second body's data
            if (two != null)
            {
                // Go through the same transformation sequence again
                Vector3 deltaVelWorldTwo = Vector3.Multiply(contactData.relativeContactPosition[1], contactData.ContactNormal);
                deltaVelWorldTwo = inverseInertiaTensor[1].transform(deltaVelWorldTwo);
                deltaVelWorldTwo = Vector3.Multiply(deltaVelWorldTwo, contactData.relativeContactPosition[1]);

                // Add the change in velocity due to rotation
                deltaVelocity += Vector3.Dot(deltaVelWorldTwo, contactData.ContactNormal);

                // Add the change in velocity due to linear motion
                deltaVelocity += two.InverseMass;
            }

            // Calculate the required size of the impulse
            impulseContact.X = contactData.desiredDeltaVelocity / deltaVelocity;
            impulseContact.Y = 0;
            impulseContact.Z = 0;
            return impulseContact;
        }
    }
}