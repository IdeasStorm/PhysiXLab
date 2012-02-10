using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class Joint
    {
        /// <summary>
        /// Holds the two body that connecting with each other
        /// </summary>
        Collidable[] body = new Collidable[2];

        /// <summary>
        /// Holds the relative location of the connection for each body, given in local coordinates
        /// </summary>
        Vector3[] position = new Vector3[2];

        public float constant { get; set; }

        //float limit;

        public Joint(Collidable one, Collidable two, Vector3 pos1, Vector3 pos2, float constant)//,float limit)
        {
            body[0] = one;
            body[1] = two;

            position[0] = pos1;
            position[1] = pos2;

            this.constant = constant;

            //this.limit = limit;
        }

        public bool addContact(Contact contact)
        {
            // Calculate the position of each connection point in world coordinates
            Vector3 onePosWorld = body[0].GetPointInWorldSpace(position[0]);
            Vector3 twoPosWorld = body[1].GetPointInWorldSpace(position[1]);

            // Calculate the length of the joint
            Vector3 oneToTwo = twoPosWorld - onePosWorld;
            Vector3 normal = oneToTwo;
            normal = Vector3.Normalize(normal);

            float length = oneToTwo.Length();

            // Check if it is violated
            if (length > constant)
            {
                contact.body[0] = body[0];

                contact.body[1] = body[1];
                contact.ContactNormal = normal;
                contact.ContactPoint = (onePosWorld + twoPosWorld) * 0.5f;
                contact.Penetration = length - constant;
                //TODO add friction and restitution to contactdata 
                contact.friction = 1.0f;
                contact.restitution = 0;
                return true;
            }

            return false;
        }

    }
}
