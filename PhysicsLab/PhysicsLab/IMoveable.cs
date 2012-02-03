using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsLab
{
    interface IMoveable
    {
        /// <summary>
        /// Pans the entity along the X, Y, and Z directions.
        /// </summary>
        /// <param name="axis">The direction to pan the entity.</param>
        /// <param name="distance">The amount of X, Y, and Z distance to pan the entity. Units are assumed to be world units.</param>
        void Translate(Vector3 axis, float distance);
    }
}
