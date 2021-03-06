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
        // static members
        public static float friction = 0.1f;
        public static float restitution = 0.7f;

        protected LinkedList<Collidable> bodies;
        protected LinkedList<HalfSpace> planes;
        protected List<Contact> contactDataList;
        protected List<Link> conductorList;
        protected List<Joint> jointList;

        public ContactGenerator()
        {
            contactDataList = new List<Contact>();
            bodies = new LinkedList<Collidable>();
            planes = new LinkedList<HalfSpace>();
            conductorList = new List<Link>();
            jointList = new List<Joint>();
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

        public void AddConductor(Link conductor)
        {
            this.conductorList.Add(conductor);
        }

        public void AddJoint(Joint joint)
        {
            this.jointList.Add(joint);
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
        /// Adds a Plane to the Planes Set to check for associated collisions
        /// </summary>
        /// <param name="plane"></param>
        public void AddPlane(HalfSpace plane)
        {
            this.planes.AddLast(plane);
        }

        /// <summary>
        /// Removes a Plane from the Planes Set.
        /// warning : this method is expensive O(n) , try to not use this in update
        /// </summary>
        /// <param name="plane"></param>
        public void RemovePlane(HalfSpace plane)
        {
            this.planes.Remove(plane);
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
            foreach (Contact contact in contactDataList)
            {
                contact.InitializeAtMoment(duration);
            }
            resolveCollisonVelocity(duration);
        }

        private Vector3 CalculateFrictionlessImpulse(Contact contactData, Matrix3[] inverseInertiaTensor)
        {
            Body one = contactData.body[0];
            Body two = contactData.body[1];
            Vector3 impulseContact;

            // Build a vector that shows the change in velocity in
            // world space for a unit impulse in the direction of the contact
            // normal.
            Vector3 torquePerUnitImpulse1 = Vector3.Cross(contactData.relativeContactPosition[0], contactData.ContactNormal);
            Vector3 rotationPerUnitImpluse1 = inverseInertiaTensor[0].transform(torquePerUnitImpulse1);
            Vector3 VelocityPerUnitImpulse1 = Vector3.Cross(rotationPerUnitImpluse1, contactData.relativeContactPosition[0]);

            // Work out the change in velocity in contact coordiantes.
            float deltaVelocity = Vector3.Dot(VelocityPerUnitImpulse1, contactData.ContactNormal);

            // Add the linear component of velocity change
            deltaVelocity += one.InverseMass;

            // Check if we need to the second body's data
            if (two != null)
            {
                // Go through the same transformation sequence again
                Vector3 torquePerUnitImpulse2 = Vector3.Cross(contactData.relativeContactPosition[1], contactData.ContactNormal);
                Vector3 rotationPerUnitImpluse2 = inverseInertiaTensor[1].transform(torquePerUnitImpulse2);
                Vector3 VelocityPerUnitImpulse2 = Vector3.Cross(rotationPerUnitImpluse2, contactData.relativeContactPosition[1]);

                // Add the change in velocity due to rotation
                deltaVelocity += Vector3.Dot(VelocityPerUnitImpulse2, contactData.ContactNormal);

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

            if ((planarImpulse > impulseContact.X * contactData.friction) && (planarImpulse != 0))
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
            Vector3 impulsiveTorqueOne = Vector3.Cross(contactData.relativeContactPosition[0],impulse);
            rotationChange[0] = inverseInertiaTensor[0].transform(impulsiveTorqueOne);
            
            velocityChange[0] = impulse * one.InverseMass;

            // Apply the changes
            one.AddVelocity(velocityChange[0]);
            one.Rotation += rotationChange[0];

            if (two != null)
            {
                // Work out body one's linear and angular changes
                Vector3 impulsiveTorqueTwo = Vector3.Cross(impulse,contactData.relativeContactPosition[1]);
                rotationChange[1] = inverseInertiaTensor[1].transform(impulsiveTorqueTwo);
                velocityChange[1] = -impulse * two.InverseMass;

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
            CollisionDetector collisionGenerator = new CollisionDetector(bodies,planes);
            this.contactDataList= collisionGenerator.ReDetect();
            //if (contactDataList.Count == 0) return;
            this.contactDataList.RemoveAll((Contact contact) => { 
                return !contact.Check(); 
            });

            foreach (Link con in conductorList)
            {
                Contact temp = new Contact(con.body[0], con.body[1]);
                if (con.Check(temp))
                    contactDataList.Add(temp);
            }

            foreach (Joint joint in jointList)
            {
                Contact contact=new Contact(null,null);
                if (joint.addContact(contact))
                    contactDataList.Add(contact);                
            }
        }

        // LEVEL 2 - Resolving Penetration

        /// <summary>
        /// Holds the number of iterations to perform when resolving
        /// velocity. 
        /// </summary>
        public int velocityIterations = 4;

        /// <summary>
        /// Holds the number of iterations to perform when resolving
        /// position. 
        /// </summary>
        public int positionIterations = 4;

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
        public float velocityEpsilon = 0.01f;

        /// <summary>
        /// To avoid instability penetrations 
        /// smaller than this value are considered to be not interpenetrating.
        /// Too small and the simulation may be unstable, too large and the
        /// bodies may interpenetrate visually. A good starting point is 
        /// the default of0.01.
        /// </summary>
        public float positionEpsilon = 0.01f;


        void resolvePenetration(float duration)
        {
            int i = 0, index;
            Vector3[] linearChange = new Vector3[2]
                    , angularChange = new Vector3[2];
            // the needed ammount to ressolve penetration
            float max;
            Vector3 deltaPosition;
            // iteratively resolve interpenetration in order of severity.
            positionIterationsUsed = 0;
            while (positionIterationsUsed < positionIterations)
            {
                i=0;
                // Find biggest penetration
                max = positionEpsilon;
                index = contactDataList.Count;
                //for(i=0;i<contactDataList.Count;i++) {
                foreach (Contact c in contactDataList)
                {
                    if (c.Penetration > max)
                    {
                        max = c.Penetration;
                        index = i;
                    }
                    i++;
                }
                if (index == contactDataList.Count) break;
                //debugging code
                //if (Keyboard.GetState().IsKeyDown(Keys.R))
                //{
                //    int q = 0;
                //}
                // wake up the slept body of the pair
                contactDataList[index].WakeUpPair();
                //contactDataList[index].FixPenetration(duration);
                try
                {
                    contactDataList[index].applyPositionChange(linearChange, angularChange, max);
                }
                catch (Exception)
                {

                    contactDataList[index].revertState();
                    contactDataList.RemoveAt(index);
                    positionIterations++;
                    continue;
                }
                foreach (Contact contact in contactDataList)
                {
                    for (int b = 0; b < 2; b++)
                    {
                        if (contact.body[b] == null) continue;
                        for (int d = 0; d < 2; d++)
                        {
                            if (contact.body[b] == contactDataList[index].body[d])
                            {
                                deltaPosition = linearChange[d]
                                    + angularChange[d] + Vector3.Cross(angularChange[d], contact.relativeContactPosition[b]);

                                contact.Penetration += Vector3.Dot(deltaPosition, contact.ContactNormal) * ((b != 0) ? 1 : -1);
                            }
                        }
                    }
                }
                positionIterationsUsed++;
            }
        }

        //Level2 - resolving collison
        void resolveCollisonVelocity(float duration)
        { 
            Vector3[] velocityChange, rotationChange;
            Vector3 cp;

            int realVelocityIterations = Math.Min(velocityIterations,contactDataList.Count);
            // iteratively handle impacts in order of severity.
            velocityIterationsUsed = 0;
            while(velocityIterationsUsed < realVelocityIterations) 
            {
                // Find contact with maximum magnitude of probable velocity change.
                float max = velocityEpsilon;
                int index = contactDataList.Count;
                for(int i = 0; i < contactDataList.Count; i++)
                {
                    if (Math.Abs(contactDataList[i].desiredDeltaVelocity) > max)
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

                foreach (Contact c in contactDataList)
                {
                    for (int b = 0; b < 2; b++)
                    {
                        for (int d = 0; d < 2; d++)
                        {
                            if (c.body[b] == contactDataList[index].body[d])
                            {
                                Vector3 deletaVelocity = velocityChange[d]
                                    + Vector3.Cross(rotationChange[d], c.relativeContactPosition[b]);
                                // second or first ?
                                c.contactVelocity += c.ContactToWorld.transformTranspose(deletaVelocity) * ((b != 0) ? -1 : 1);
                                c.CalculateDeltaVelocity(duration);
                            }
                        }
                    }
                }

                velocityIterationsUsed++;
            }
        }
        #endregion

    }
}