using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    class ImpulseGenerator : Effect
    {
        protected LinkedList<ContactData> contactDataLinkedList;

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
        public override void update(float time)
        {
            duration = time;
            foreach (ContactData contactData in contactDataLinkedList)
            {
                Affect(contactData);
            }
        }

        public override void Affect(ContactData contactData)
        {

        }

        /// <summary>
        /// Calculates and sets the internal value for the delta velocity.
        /// </summary>
        /// <param name="duration"></param>
        public void calculateDeltaVelocity(ContactData contactData)
        {
            Body body1;
            Body body2;

            const float velocityLimit = (float)0.25f;

            ///>NewVelocityCalculation
            // Calculate the acceleration induced velocity accumulated this frame
            float velocityFromAcc = Vector3.Dot(body1.LastFrameAcceleration,contactData.contactNormal) * duration ;

            if (body2 != null)
            {
                velocityFromAcc -= Vector3.Dot(body2.LastFrameAcceleration, contactData.contactNormal) * duration;
            }

            // If the velocity is very slow, limit the restitution
            float thisRestitution = contactData.restitution;
            if (Math.Sqrt(contactData.contactVelocity.X) < velocityLimit)
            {
                thisRestitution = (float)0.0f;
            }

            // Combine the bounce velocity with the removed
            // acceleration velocity.
            contactData.deltaVelocity.X = -contactData.contactVelocity.X - thisRestitution * ((contactData.contactVelocity.X - velocityFromAcc));
            ///<NewVelocityCalculation            
        }
    }
}