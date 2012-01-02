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
        public float invertMass { get; set; }
        public float mass 
        {
            get { return 1.0f / invertMass; }
            set { invertMass = 1.0f / invertMass; }
        }
        Vector3 position;
        Vector3 velocity;
        Vector3 acceleration;
        Vector3 forceAccumulator;

        public void AddForce(Vector3 force)
        {
            forceAccumulator += force;
        }

        public void AddVelocity(Vector3 velocity)
        {
            velocity += velocity;
        }

    }
}
