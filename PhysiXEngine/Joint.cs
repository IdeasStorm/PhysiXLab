using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    class Joint
    {
        /// <summary>
        /// Holds the two body that connecting with each other
        /// </summary>
        Collidable[] body = new Collidable[2];

        public float constant{get;set;}

        public Joint(Collidable one, Collidable two, float constant)
        {
            body[0] = one;
            body[1] = two;
            this.constant = constant;
        }

        public bool addContact(ref Contact contact, int limit)
        {
            // Calculate the position of each connection point in world coordinates
            Vector3 onePosWorld = body[0].GetPointInWorldSpace(body[0].Position);
            Vector3 twoPosWorld = body[1].GetPointInWorldSpace(body[1].Position);
            
            // Calculate the length of the joint
            Vector3 oneToTwo = twoPosWorld - onePosWorld;
            Vector3 normal = oneToTwo;
            normal=Vector3.Normalize(normal);
            
            float length = oneToTwo.Length();

            // Check if it is violated
            if (length > constant)
            {
                contact.body[0] = body[0];
                
                contact.body[1] = body[1];
                contact._ContactNormal = normal;
                contact._ContactPoint = (onePosWorld + twoPosWorld) * 0.5f;
                contact._Penetration = length - constant;
                //TODO add friction and restitution to contactdata 
                contact.friction = 1.0f;
                contact.restitution = 0;
                return true;
            }

            return false;
        }

    }
}
