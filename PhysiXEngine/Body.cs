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

        /// <summary>
        /// the Mass of the body in Kg
        /// </summary>
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
        public Vector3 AngularAcceleration {  get; set; }

        private Vector3 forceAccumulator;
        private Vector3 torqueAccumulator;

        private Matrix3 _inverseInertiaTensor = new Matrix3();


        public bool IsAsleep { 
        set
        {
            IsAwake = !value;
        } 
        get
        {
            return !IsAwake;
        }
        }
        public bool IsAwake { set; get; }
        public void Awake()
        {
            IsAwake = true;
        }

        /// <summary>
        /// holds the inertia (independent of the axis)
        /// warning : this is in the body space
        /// </summary>
        public Matrix3 InverseInertiaTensor {
            get{
                return _inverseInertiaTensor;
            }
            set{
                _inverseInertiaTensor = value;
            }
        }

        public Matrix3 InertiaTensor 
        {
            get
            {
                return _inverseInertiaTensor.inverse();
            }
            set
            {
                _inverseInertiaTensor.setInverse(value);
            }
        }

        /// <summary>
        /// holds the inertia (independent of the axis)
        /// in the world space
        /// </summary>
        public Matrix3 InverseInertiaTensorWorld { get; protected set; }

        //TODO above matrices should be 3x3

        /// <summary>
        /// Angular orientation in world space
        /// </summary>
        public Quaternion Orientation;

        /// <summary>
        /// stores the old position for this object providing rollingback capability
        /// </summary>
        private Vector3 _oldPosition;

        /// <summary>
        /// the angular velocity
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// the matrix that converts a vector from the body space to the world space
        /// </summary>
        public Matrix TransformMatrix { get; protected set; }

        //TODO add angular/linear damping if needed


        public Body()
        {
            Mass = 1;
            InertiaTensor = Matrix.Identity;
            Orientation = Quaternion.CreateFromYawPitchRoll(0, 0, 0);
            Awake();
            calculateDerivedData(); //TODO this must not be here!!
        }

        /// <summary>
        /// updates the body to the next state
        /// </summary>
        /// <param name="duration">the time elapsed from the past frame</param>
        public virtual void Update(float duration)
        {
            if (IsAsleep) return;
            LastFrameAcceleration = Acceleration;
            LastFrameAcceleration += forceAccumulator * _inverseMass;
            AngularAcceleration = InverseInertiaTensorWorld.transform(AngularAcceleration);
            //AngularAcceleration = Vector3.Transform(AngularAcceleration,InverseInertiaTensorWorld);

            Velocity += LastFrameAcceleration * duration;
            Rotation += AngularAcceleration;

            Position += Velocity;
            Orientation += Quaternion.CreateFromAxisAngle(Rotation, MathHelper.Pi) * (duration/2f) * Orientation;
            calculateDerivedData();
            clearAccumulators();
            // add damping 
        }

        protected void calculateDerivedData()
        {
            Orientation.Normalize();

            // Calculate the transform matrix for the body.
            TransformMatrix = Matrix.CreateFromQuaternion(Orientation) *  Matrix.CreateTranslation(Position);
            
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
            this.Velocity += velocity;
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

            forceAccumulator += force;
            torqueAccumulator += Vector3.Cross(pt , force);
        }

        /// <summary>
        /// an alias of AddForceAtPoint 
        /// </summary>
        /// <see cref="AddForceAtPoint"/>
        /// <param name="force"></param>
        /// <param name="point"></param>
        public void AddForce(Vector3 force, Vector3 point)
        {
            AddForceAtPoint(force, point);
        }

        /// <summary>
        /// Sets the value of the matrix from inertia tensor values.
        /// </summary>
        protected void setInertiaTensorCoeffs(float ix, float iy, float iz,
            float ixy = 0, float ixz = 0, float iyz = 0)
        {
            InertiaTensor.setInertiaTensorCoeffs(ix, iy, iz, ixy, ixz, iyz);
        }

        /// <summary>
        /// gets a vector of an axis by index (from 0 to 2)      
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetAxis(int index)
        {
            return TransformMatrix.GetAxisVector(index);
        }

        /// <summary>
        /// revert Position changes in the last frame
        /// </summary>
        public void RevertChanges()
        {
            Position = this._oldPosition;
            // TODO save this position somewhere
        }

        /// <summary>
        /// indicates the state of this body (moving or not)
        /// </summary>
        public bool IsMoving {
            get { 
                return (Velocity != Vector3.Zero);
            }
        }
    }
}
