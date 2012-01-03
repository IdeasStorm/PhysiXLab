using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class Body
    {
        public float inverseMass { get; set; }
        public float mass 
        {
            get { return 1.0f / inverseMass; }
            set { inverseMass = 1.0f / inverseMass; }
        }
        Vector3 position { public get; protected set; }
        Vector3 velocity { public get; protected set; }
        Vector3 acceleration { public get; protected set; }

        Vector3 forceAccumulator;
        Vector3 torqueAccumulator;
        
        /// <summary>
        /// holds the inertia (independent of the axis)
        /// warning : this is in the body space
        /// </summary>
        Matrix inverseInertiaTensor;

        /// <summary>
        /// holds the inertia (independent of the axis)
        /// in the world space
        /// </summary>
        Matrix inverseInertiaTensorWorld;

        //TODO above matrices should be 3x3

        /// <summary>
        /// Angular orientation in world space
        /// </summary>
        Quaternion orientation;
        /// <summary>
        /// the angular velocity
        /// </summary>
        Vector3 rotation;

        /// <summary>
        /// the matrix that converts a vector from the body space to the world space
        /// </summary>
        Matrix transformMatrix;

        //TODO add angular/linear damping if needed
        //TODO add sleep support
        


        /// <summary>
        /// updates the body to the next state
        /// </summary>
        /// <param name="duration">the time elapsed from the past frame</param>
        public void update(float duration)
        {
            acceleration = forceAccumulator * inverseMass;
            velocity += acceleration;
            position += velocity;
        }

        /// <summary>
        /// Adds a force to force accumulator
        /// </summary>
        /// <param name="force">the force to be added </param>
        public void AddForce(Vector3 force)
        {
            forceAccumulator += force;
        }

        /// <summary>
        /// Adds a Velocity vector to the current velocity
        /// </summary>
        /// <param name="velocity"></param>
        public void AddVelocity(Vector3 velocity)
        {
            velocity += velocity;
        }

    }
}
