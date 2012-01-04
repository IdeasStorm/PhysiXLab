using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    /// <summary>
    /// Hold a particle information Position,Mass, ...
    /// </summary>
    class Particle
    {
        /// <summary>
        /// Holds the inverse of the mass of the particle
        /// </summary>
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
        /// Holds the mass of the particle
        /// </summary>
        public float mass
        {
            get { return 1.0f / _inverseMass; }
            set
            {
                HasFiniteMass = !float.IsInfinity(value);
                _inverseMass = 1.0f / _inverseMass;
            }
        }
        /// <summary>
        /// Holds true if particle has a very beg mass
        ///       False if not
        /// </summary>
        public bool HasFiniteMass { get; private set; }

        ///<summary>
        ///Holds the amount of damping applied to linear motion
        ///</summary>
        public float damping { get; set; }

        ///<summary>
        ///Holds the linear position of the particle in world space
        ///</summary>
        public Vector3 position{ get; set; }
      
        ///<summary>
        ///Holds the linear velocity of the particle in world space.
        ///</summary>
        public Vector3 velocity{ get; set; }

        ///<summary>
        ///Holds the accumulated force to applied in the next frame
        ///</summary>
        public Vector3 forceAccum{ get; set; }

        ///<summary>
        ///Holds the acceleration of the particle
        ///</summary>
        public Vector3 acceleration{ get; set; }

        ///<summary>
        ///Integrates the particle forward in time by the given amount
        ///</summary>
        public void integrate(float duration)
        {
            // We don't integrate things with zero mass.
            if (InverseMass <= 0.0f || duration < 0)
                return;

            // Update linear position.
            Vector3.Add(position, Vector3.Multiply(velocity, duration));

            // Work out the acceleration from the force
            Vector3 resultingAcc = acceleration;

            Vector3.Add(resultingAcc, Vector3.Multiply(forceAccum, InverseMass));

            // Update linear velocity from the acceleration.   
            Vector3.Add(velocity, Vector3.Multiply(resultingAcc, duration));

            // Impose drag
            float damp = (float)Math.Pow(Convert.ToDouble(damping), Convert.ToDouble(duration));
            velocity = Vector3.Multiply(velocity, damp);

            // Clear the forces.
            clearAccumulator();
        }

        ///<summary>
        /// Clears the forces applied to the particle   
        ///</summary>
        public void clearAccumulator()
        {
            forceAccum.Equals(new Vector3(0, 0, 0));
        }

        /// <summary>
        /// Adds the given force to the particle
        /// </summary>
        /// <param name="force"></param>
        public void addForce(Vector3 force)
        {
            forceAccum = Vector3.Add(forceAccum, force);
        }
    }
}
