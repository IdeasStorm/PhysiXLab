﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PhysiXEngine.Helpers;

namespace PhysiXEngine
{
    public class ContactGenerator : Effect
    {
        protected LinkedList<Collidable> bodies;
        protected List<ContactData> contactDataList;
        static float velocityLimit = 0.25f;

        public ContactGenerator()
        {
            contactDataList = new List<ContactData>();
        }

        public void AddContactData(ContactData contactdata)
        {
            contactDataList.Add(contactdata);
        }

        public void ClearContactData()
        {
            contactDataList.Clear();
        }

        /// <summary>
        /// passing all contacts list calling affect function
        /// to affect each body by the another
        /// </summary>
        /// <param name="time"></param>
        public override void Update(float time)
        {
            frameDuration = time;
            foreach (ContactData contactData in contactDataList)
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
            //contactData.desiredDeltaVelocity = deltaVelocity;
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
        #region "Levels of resolving contacts"
        //LEVEL 1

        void calculateContactInformations(float duration)
        {
            BoundingBox world = new BoundingBox();
            //TODO !! make real world 
            CollisionDetector collisionGenerator = new CollisionDetector(world,bodies);
            this.contactDataList= collisionGenerator.Detect();
            // initializing contacts
            foreach (ContactData contactData in contactDataList)
            {
                contactData.InitializeAtMoment(duration);
            }
        }

        // LEVEL 2 - Resolving Penetration

        /// <summary>
        /// Holds the number of iterations to perform when resolving
        /// velocity. 
        /// </summary>
        uint velocityIterations;

        /// <summary>
        /// Holds the number of iterations to perform when resolving
        /// position. 
        /// </summary>
        uint positionIterations;

        /// <summary>
        /// Stores the number of velocity iterations used in the 
        /// last call to resolve contacts.
        /// </summary>
        uint velocityIterationsUsed;

        /// <summary>
        /// Stores the number of position iterations used in the 
        ///last call to resolve contacts.
        /// </summary>
        uint positionIterationsUsed;

        /// <summary>
        /// To avoid instability velocities smaller
        /// than this value are considered to be zero. Too small and the
        /// simulation may be unstable, too large and the bodies may
        /// interpenetrate visually. A good starting point is the default 
        /// of 0.01.
        /// </summary>
        float velocityEpsilon = 0.01f;

        /// <summary>
        /// To avoid instability penetrations 
        /// smaller than this value are considered to be not interpenetrating.
        /// Too small and the simulation may be unstable, too large and the
        /// bodies may interpenetrate visually. A good starting point is 
        /// the default of0.01.
        /// </summary>
        float positionEpsilon = 0.01f;


        void resolvePenetration(float duration)
        {
            int i=0,index;
            Vector3[] velocityChange = new Vector3[2]
                    , rotationChange = new Vector3[2];
            // the needed ammount to ressolve penetration
            float[] rotationAmount = new float[2];
            float max;
            Vector3 cp;

            // iteratively resolve interpenetration in order of severity.
            positionIterationsUsed = 0;
            while(positionIterationsUsed < positionIterations)
            {
                // Find biggest penetration
                max = positionEpsilon;
                index = contactDataList.Count;
                //for(i=0;i<contactDataList.Count;i++) {
                foreach (ContactData c in contactDataList) {
                    if(c.Penetration > max)
                    {
                        max = (float)c.Penetration;
                        //TODO change Penetration to flaot from origin
                        index=i;
                    }
                    i++;
                }
                if (index == contactDataList.Count) break;

                // wake up the slept body of the pair
                contactDataList[index].WakeUpPair();

                // Resolve the penetration.
                contactDataList[index].applyPositionChange(velocityChange,
                    rotationChange,
                    rotationAmount,
                    max);//-positionEpsilon);

                // Again this action may have changed the penetration of other 
                // bodies, so we update contacts.
                for(i = 0; i < contactDataList.Count; i++)
                {
                    if(contactDataList[i].body[0] == contactDataList[index].body[0])
                    {
                        cp = Vector3.Cross(rotationChange[0], contactDataList[i].relativeContactPosition[0]);

                        cp += velocityChange[0];

                        contactDataList[i].Penetration -=
                            rotationAmount[0] * Vector3.Dot(cp,contactDataList[i].ContactNormal);
                    }
                    else if(contactDataList[i].body[0]==contactDataList[index].body[1])
                    {
                        cp = Vector3.Cross(rotationChange[1], contactDataList[i].relativeContactPosition[0]);

                        cp += velocityChange[1];

                        contactDataList[i].Penetration -= rotationAmount[1] *
                            Vector3.Dot(cp,contactDataList[i].ContactNormal);
                    }

                    if(contactDataList[i].body[1] != null)
                    {
                        if(contactDataList[i].body[1]==contactDataList[index].body[0])
                        {
                            cp = Vector3.Cross(rotationChange[0],contactDataList[i].relativeContactPosition[1]);

                            cp += velocityChange[0];

                            contactDataList[i].Penetration += rotationAmount[0] *
                                Vector3.Dot(cp,contactDataList[i].ContactNormal);
                        }
                        else if(contactDataList[i].body[1]==contactDataList[index].body[1])
                        {
                            cp = Vector3.Cross(rotationChange[i], contactDataList[i].relativeContactPosition[1]);

                            cp += velocityChange[1];

                            contactDataList[i].Penetration += rotationAmount[1] *
                                Vector3.Dot(cp, contactDataList[i].ContactNormal);
                        }
                    }
                }
                positionIterationsUsed++;
    }
}
        #endregion

    }

    ///
    /// just a stub , remove this plz
    class CollisionDetector
    {
        public CollisionDetector(BoundingBox World, LinkedList<Collidable> Shapes)
        {
            //TODO link the real class & delete this
        }


        public List<ContactData> ContactsList { get; set; }

        internal List<ContactData> Detect()
        {
            throw new NotImplementedException();
        }
    }
}