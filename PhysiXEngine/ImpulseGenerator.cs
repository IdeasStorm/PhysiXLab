using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class ImpulseGenerator : Effect
    {
        protected LinkedList<ContactData> contactDataLinkedList;
        protected Vector3 deltaVelocity;

        public ImpulseGenerator()
        {
            contactDataLinkedList = new LinkedList<ContactData>();
        }

        public void AddContactData(ContactData contactdata)
        {
            contactDataLinkedList.AddLast(contactdata);
            deltaVelocity = new Vector3();
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

        }

        /// <summary>
        /// Calculates and sets the internal value for the delta velocity.
        /// </summary>
        /// <param name="duration"></param>
        public void calculateDeltaVelocity(ContactData contactData)
        {
            //Temp ***************
            Body body1=new Body();
            Body body2=new Body();

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
            deltaVelocity.X = -contactData.contactVelocity.X - thisRestitution * ((contactData.contactVelocity.X - velocityFromAcc));
            ///<NewVelocityCalculation            
        }
    }
}