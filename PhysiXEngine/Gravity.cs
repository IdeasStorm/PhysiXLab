using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class Gravity:Field
    {                
        /// <summary>
        /// acceleration of the gravity
        /// </summary>
        private Vector3 gravity{get;set;}

        /// <summary>
        /// Creates the effect with the given acceleration 
        /// </summary>
        /// <param name="?"></param>
        public Gravity(Vector3 gravity)
        {
            this.gravity = gravity;
        }

        /// <summary>
        /// override affect
        /// update body position and velocity
        /// </summary>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        public override void Affect(Body body)
        {

            if (!body.HasFiniteMass)
                return;

            //Vector3 velocity=Vector3.Multiply(gravity,body.Mass);
            //velocity=Vector3.Multiply(velocity,duration);
            //body.AddVelocity(velocity);                        
            //TODO delete above lines @Adnan

            body.AddForce(Vector3.Multiply(gravity,body.Mass));
        }
    }
}
