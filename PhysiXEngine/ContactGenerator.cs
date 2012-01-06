using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
            Vector3 deltaVelocity = calculateDeltaVelocity(contactData);


        }

        /// <summary>
        /// Calculates and sets the internal value for the delta velocity.
        /// </summary>
        /// <param name="contactData">the contact Data that contains the bodies and contct informations</param>
        public Vector3 calculateDeltaVelocity(ContactData contactData)
        {
            Body body1 = contactData.body[0];
            Body body2 = contactData.body[1];

            const float velocityLimit = (float)0.25f;

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
            Vector3 deltaVelocity = new Vector3(0);
            deltaVelocity.X = -contactData.contactVelocity.X - thisRestitution * ((contactData.contactVelocity.X - velocityFromAcc));
            return deltaVelocity;
        }


    }
}