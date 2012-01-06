using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    /// <summary>
    /// calculate body friction 
    /// </summary>
    public class Friction : ForceGenerator
    {
        /// <summary>
        /// Holds the velocity drag coeffificent.
        /// </summary>
        double k1;

        /// <summary>
        /// Holds the velocity squared drag coeffificent.
        /// </summary>
        double k2;

        /// <summary>
        /// Creates the generator with the given coefficients.
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        Friction(double k1, double k2)
        {
            //TODO add init code
        }

        /// <summary>
        /// Applies the drag force to the given particle.
        /// </summary>
        /// <param name="body"></param>
        protected override void Affect(Body body)
        {
            //TODO add affect Code
        }

    }
}