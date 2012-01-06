using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PhysiXEngine.Helpers;

namespace PhysiXEngine
{
    public class Body
    {
        public bool HasFiniteMass { get; private set; }
        private float _inverseMass;
        public float InverseMass
        {
            get
            {
                return _inverseMass;
            }
            set
            {
                HasFiniteMass = (value != 0.0f);
                _inverseMass = value;
            }
        }
        public float Mass 
        {
            get { return 1.0f / _inverseMass; }
            set 
            {
                HasFiniteMass = !float.IsInfinity(value);
                _inverseMass = 1.0f / value;
            }
        }



        public Vector3 Position {  get; set; }
        public Vector3 Velocity {  get; protected set; }
        public Vector3 Acceleration {  get; protected set; }
        public Vector3 LastFrameAcceleration {  get; protected set; }
        public Vector3 AngularAcceleration {  get; protected set; }

        private Vector3 forceAccumulator;
        private Vector3 torqueAccumulator;
        
        /// <summary>
        /// holds the inertia (independent of the axis)
        /// warning : this is in the body space
        /// </summary>
        public Matrix3 InverseInertiaTensor { get; protected set; }

        /// <summary>
        /// holds the inertia (independent of the axis)
        /// in the world space
        /// </summary>
        public Matrix3 InverseInertiaTensorWorld { get; protected set; }

        //TODO above matrices should be 3x3

        /// <summary>
        /// Angular orientation in world space
        /// </summary>
        Quaternion orientation;
        /// <summary>
        /// the angular velocity
        /// </summary>
        public Vector3 Rotation { get; protected set; }

        /// <summary>
        /// the matrix that converts a vector from the body space to the world space
        /// </summary>
        public Matrix TransformMatrix { get; protected set; }

        //TODO add angular/linear damping if needed
        //TODO add sleep support


        public Body()
        {
            Mass = 1;
            calculateDerivedData();
        }

        /// <summary>
        /// updates the body to the next state
        /// </summary>
        /// <param name="duration">the time elapsed from the past frame</param>
        public void Update(float duration)
        {
            LastFrameAcceleration = Acceleration;
            LastFrameAcceleration += forceAccumulator * _inverseMass;
            //AngularAcceleration = Vector3.Transform(AngularAcceleration,InverseInertiaTensorWorld);

            Velocity += LastFrameAcceleration * duration;
            Rotation += AngularAcceleration;

            Position += Velocity;
            orientation += Quaternion.CreateFromYawPitchRoll(Rotation.Y,Rotation.X,Rotation.Z);
            calculateDerivedData();
            clearAccumulators();
            //TODO add sleep capablilty
        }

        private void calculateDerivedData()
        {
            orientation.Normalize();

            // Calculate the transform matrix for the body.
            TransformMatrix = /*Matrix.CreateFromQuaternion(orientation) */ Matrix.CreateTranslation(Position);
            
            // Calculate the inertiaTensor in world space.
            InverseInertiaTensorWorld = InverseInertiaTensor * TransformMatrix;

            //TODO rem 3x3 4x4 problems
        }

        private void clearAccumulators()
        {
            forceAccumulator = Vector3.Zero;
            torqueAccumulator = Vector3.Zero;
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


        public Vector3 GetPointInWorldSpace(Vector3 point)
        {
            return Vector3.Transform(point, TransformMatrix);            
        }

        public void AddForceAtPoint(Vector3 force, Vector3 point)
        {
            // Convert to coordinates relative to center of mass.
            Vector3 pt = point;
            pt -= Position;
            //TODO Add Force force at Point point
        }
    }
}
