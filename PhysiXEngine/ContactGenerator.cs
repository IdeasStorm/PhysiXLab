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
        protected LinkedList<Collidable> bodies;
        protected List<Contact> contactDataList;

        public ContactGenerator()
        {
            contactDataList = new List<Contact>();
            bodies = new LinkedList<Collidable>();
        }

        public void AddContactData(Contact contactdata)
        {
            contactDataList.Add(contactdata);
        }

        public void ClearContactData()
        {
            contactDataList.Clear();
        }

        /// <summary>
        /// Adds a body to the collison List to check for it
        /// </summary>
        /// <param name="body"></param>
        public void AddBody(Collidable body)
        {
            this.bodies.AddLast(body);
        }

        /// <summary>
        /// Removes a body from collison List 
        /// warning : this method is expensive O(n) , try to not use this in update
        /// </summary>
        /// <param name="body"></param>
        public void RemoveBody(Collidable body)
        {
            this.bodies.Remove(body);
        }

        /// <summary>
        /// passing all contacts list calling affect function
        /// to affect each body by the another
        /// </summary>
        /// <param name="time"></param>
        public override void Update(float duration)
        {            
            frameDuration = duration;
            calculateContactInformations(duration);
            resolvePenetration(duration);
            resolveCollisonVelocity(duration);
        }

        public void Affect(Contact contactData)
        {
            // Calculating Desired Delta Velocity
            contactData.InitializeAtMoment(frameDuration);
            //float deltaVelocity = CalculateDeltaVelocity(contactData);


        }

        private Vector3 CalculateFrictionlessImpulse(Contact contactData, Matrix3[] inverseInertiaTensor)
        {
            Body one = contactData.body[0];
            Body two = contactData.body[1];
            Vector3 impulseContact;

            // Build a vector that shows the change in velocity in
            // world space for a unit impulse in the direction of the contact
            // normal.
            Vector3 deltaVelWorldOne = Vector3.Cross(contactData.relativeContactPosition[0], contactData.ContactNormal);
            deltaVelWorldOne = inverseInertiaTensor[0].transform(deltaVelWorldOne);
            deltaVelWorldOne = Vector3.Cross(deltaVelWorldOne, contactData.relativeContactPosition[0]);

            // Work out the change in velocity in contact coordiantes.
            float deltaVelocity = Vector3.Dot(deltaVelWorldOne, contactData.ContactNormal);

            // Add the linear component of velocity change
            deltaVelocity += one.InverseMass;

            // Check if we need to the second body's data
            if (two != null)
            {
                // Go through the same transformation sequence again
                Vector3 deltaVelWorldTwo = Vector3.Cross(contactData.relativeContactPosition[1], contactData.ContactNormal);
                deltaVelWorldTwo = inverseInertiaTensor[1].transform(deltaVelWorldTwo);
                deltaVelWorldTwo = Vector3.Cross(deltaVelWorldTwo, contactData.relativeContactPosition[1]);

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

        private Vector3 CalculateFrictionImpulse(Contact contactData, Matrix3[] inverseInertiaTensor)
        {
            Body one = contactData.body[0];
            Body two = contactData.body[1];
            Vector3 impulseContact;
            float inverseMass = one.InverseMass;

            // The equivalent of a cross product in matrices is multiplication
            // by a skew symmetric matrix - we build the matrix for converting
            // between linear and angular quantities.
            Matrix3 impulseToTorque = new Matrix3();
            impulseToTorque.setSkewSymmetric(contactData.relativeContactPosition[0]);

            // Build the matrix to convert contact impulse to change in velocity
            // in world coordinates.
            Matrix3 deltaVelWorld = impulseToTorque;
            deltaVelWorld *= inverseInertiaTensor[0];
            deltaVelWorld *= impulseToTorque;
            deltaVelWorld *= -1;

            // Check if we need to add body two's data
            if (two != null)
            {
                // Set the cross product matrix
                impulseToTorque.setSkewSymmetric(contactData.relativeContactPosition[1]);

                // Calculate the velocity change matrix
                Matrix3 deltaVelWorldTwo = impulseToTorque;
                deltaVelWorldTwo *= inverseInertiaTensor[1];
                deltaVelWorldTwo *= impulseToTorque;
                deltaVelWorldTwo *= -1;

                // Add to the total delta velocity.
                deltaVelWorld += deltaVelWorldTwo;

                // Add to the inverse mass
                inverseMass += two.InverseMass;
            }

            // Do a change of basis to convert into contact coordinates.
            Matrix3 deltaVelocity = contactData.ContactToWorld.transpose();
            deltaVelocity *= deltaVelWorld;
            deltaVelocity *= contactData.ContactToWorld;

            // Add in the linear velocity change
            deltaVelocity.data[0] += inverseMass;
            deltaVelocity.data[4] += inverseMass;
            deltaVelocity.data[8] += inverseMass;

            // Invert to get the impulse needed per unit velocity
            Matrix3 impulseMatrix = deltaVelocity.inverse();

            // Find the velocities that will be removed
            Vector3 velKill = new Vector3(contactData.desiredDeltaVelocity,
                -contactData.contactVelocity.Y,
                -contactData.contactVelocity.Z);

            // Find the impulse to kill target velocities
            impulseContact = impulseMatrix.transform(velKill);

            // Check for exceeding friction
            float planarImpulse = (float)Math.Sqrt(Convert.ToDouble(impulseContact.Y * impulseContact.Y + impulseContact.Z * impulseContact.Z));
            
            if (planarImpulse > impulseContact.X * contactData.friction)
            {
                // We need to use dynamic friction
                impulseContact.Y /= planarImpulse;
                impulseContact.Z /= planarImpulse;

                impulseContact.X = deltaVelocity.data[0] +
                    deltaVelocity.data[1] * contactData.friction * impulseContact.Y +
                    deltaVelocity.data[2] * contactData.friction * impulseContact.Z;
                impulseContact.X = contactData.desiredDeltaVelocity / impulseContact.X;
                impulseContact.Y *= contactData.friction * impulseContact.X;
                impulseContact.Z *= contactData.friction * impulseContact.X;
            }
            return impulseContact;
        }

        public void ApplyVelocityChange(Contact contactData,out Vector3[] velocityChange,out Vector3[] rotationChange)
        {
            Body one = contactData.body[0];
            Body two = contactData.body[1];
            velocityChange = new Vector3[2];
            rotationChange = new Vector3[2];
            // Get hold of the inverse mass and inverse inertia tensor, both in
            // world coordinates.
            Matrix3[] inverseInertiaTensor = new Matrix3[2];
            inverseInertiaTensor[0] = one.InverseInertiaTensorWorld;
            if (two != null)
                inverseInertiaTensor[1] = two.InverseInertiaTensorWorld;

            // We will calculate the impulse for each contact axis
            Vector3 impulseContact;

            if (contactData.friction == 0.0f)
            {
                //ther is no friction
                impulseContact = CalculateFrictionlessImpulse(contactData, inverseInertiaTensor);
            }
            else
            {
                // Otherwise we may have impulses that aren't in the direction of the
                // contact, so we need the more complex version.
                impulseContact = CalculateFrictionImpulse(contactData, inverseInertiaTensor);
            }

            // Convert impulse to world coordinates
            Vector3 impulse = contactData.ContactToWorld.transform(impulseContact);

            // Split in the impulse into linear and rotational components
            Vector3 impulsiveTorqueOne = Vector3.Cross(contactData.relativeContactPosition[0], impulse);
            rotationChange[0] = inverseInertiaTensor[0].transform(impulsiveTorqueOne);

            velocityChange[0] = Vector3.Zero;
            velocityChange[0] += impulse * one.InverseMass;

            // Apply the changes
            one.AddVelocity(velocityChange[0]);
            one.Rotation += rotationChange[0];

            if (two != null)
            {
                // Work out body one's linear and angular changes
                Vector3 impulsiveTorqueTwo = Vector3.Cross(contactData.relativeContactPosition[1], impulse);
                rotationChange[1] = inverseInertiaTensor[1].transform(impulsiveTorqueTwo);
                velocityChange[1] = - impulse * two.InverseMass;

                // And apply them.
                two.AddVelocity(velocityChange[1]);
                two.Rotation += rotationChange[1];
            }
        }

        #region "Levels of resolving contacts"
        //LEVEL 1

        void calculateContactInformations(float duration)
        {
            // BoundingBox world = new BoundingBox();
            // TODO !! make real world
            // CollisionDetector collisionGenerator = new CollisionDetector(world, bodies);
            CollisionDetector collisionGenerator = new CollisionDetector(bodies);
            this.contactDataList= collisionGenerator.ReDetect();
            // initializing contacts
            foreach (Contact contactData in contactDataList)
            {
                contactData.body[0].generateContacts(contactData.body[1], contactData);
                contactData.InitializeAtMoment(duration);
            }
        }

        // LEVEL 2 - Resolving Penetration

        /// <summary>
        /// Holds the number of iterations to perform when resolving
        /// velocity. 
        /// </summary>
        uint velocityIterations = 4;

        /// <summary>
        /// Holds the number of iterations to perform when resolving
        /// position. 
        /// </summary>
        uint positionIterations = 4;

        //TODO modify above values

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
                foreach (Contact c in contactDataList) {
                    if(c.Penetration > max)
                    {
                        max = c.Penetration;                        
                        index=i;
                    }
                    i++;
                }
                if (index == contactDataList.Count) break;
                // an item was chosen 
                Collidable BodyOne = contactDataList[index].body[0];
                Collidable BodyTwo = contactDataList[index].body[1];
                if (BodyOne.IsMoving) BodyOne.RevertChanges();
                else if (BodyTwo.IsMoving) BodyTwo.RevertChanges();
                positionIterationsUsed++;
    }
}

        //Level2 - resolving collison
        void resolveCollisonVelocity(float duration)
        { 
            Vector3[] velocityChange, rotationChange;
            Vector3 cp;

            // iteratively handle impacts in order of severity.
            velocityIterationsUsed = 0;
            while(velocityIterationsUsed < velocityIterations) 
            {
                // Find contact with maximum magnitude of probable velocity change.
                float max = velocityEpsilon;
                int index = contactDataList.Count;
                for(int i = 0; i < contactDataList.Count; i++)
                {
                    if (contactDataList[i].desiredDeltaVelocity > max)
                    {
                        max = contactDataList[i].desiredDeltaVelocity;
                        index = i;
                    }
                }
                if (index == contactDataList.Count) break;

                // Match the awake state at the contact
                contactDataList[index].WakeUpPair();

                // Do the resolution on the contact that came out top.
                ApplyVelocityChange(contactDataList[index],out velocityChange,out rotationChange);

                // With the change in velocity of the two bodies, the update of 
                // contact velocities means that some of the relative closing 
                // velocities need recomputing.
                for (int i = 0; i < contactDataList.Count; i++)
                {
                    if (contactDataList[i].body[0] != null)
                    {
                        if (contactDataList[i].body[0] == contactDataList[index].body[0])
                        {
                            cp = Vector3.Cross(rotationChange[0],contactDataList[i].relativeContactPosition[0]);

                            cp += velocityChange[0];

                            contactDataList[i].contactVelocity += 
                                contactDataList[i].ContactToWorld.transformTranspose(cp);
                            contactDataList[i].CalculateDeltaVelocity(duration);
                            //TODO move this to contact data & check params e.g duration
                        }
                        else if (contactDataList[i].body[0]==contactDataList[index].body[1])
                        {
                            cp = Vector3.Cross(rotationChange[1], contactDataList[i].relativeContactPosition[0]);

                            cp += velocityChange[1];

                            contactDataList[i].contactVelocity += 
                                contactDataList[i].ContactToWorld.transformTranspose(cp);
                            contactDataList[i].CalculateDeltaVelocity(duration);
                            //TODO move this to contact data & check params e.g duration
                        }
                    }

                    if (contactDataList[i].body[1] != null )
                    {
                        if (contactDataList[i].body[1]==contactDataList[index].body[0])
                        {
                            cp = Vector3.Cross(rotationChange[0], contactDataList[i].relativeContactPosition[1]);

                            cp += velocityChange[0];

                            contactDataList[i].contactVelocity -= 
                                contactDataList[i].ContactToWorld.transformTranspose(cp);
                            contactDataList[i].CalculateDeltaVelocity(duration);
                            //TODO move this to contact data & check params e.g duration
                        }
                        else if (contactDataList[i].body[1]==contactDataList[index].body[1])
                        {
                            cp = Vector3.Cross(rotationChange[1], contactDataList[i].relativeContactPosition[1]);

                            cp += velocityChange[1];

                            contactDataList[i].contactVelocity -= 
                                contactDataList[i].ContactToWorld.transformTranspose(cp);
                            contactDataList[i].CalculateDeltaVelocity(duration);
                            //TODO move this to contact data & check params e.g duration
                        }
                    }
                }
                velocityIterationsUsed++;
    }
        }
        #endregion

    }
}