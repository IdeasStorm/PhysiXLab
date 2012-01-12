using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public abstract class Collidable : Body
    {
        /// <summary>
        /// generates contact information for this collidable body with another one 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract Contact generateContacts(Collidable other);

        /// <summary>
        ///  generates contact information for this collidable body with another one 
        ///  and fill this info into an existing contact
        /// </summary>
        /// <param name="other"></param>
        /// <param name="contact">a contact to fill</param>
        /// <returns></returns>
        public abstract void generateContacts(Collidable other,Contact contact);

        /// <summary>
        /// Checks whether the body collides with another collidable or not
        /// </summary>
        /// <param name="other">the other collidable body to collides with </param>
        /// <returns></returns>
        public abstract Boolean CollidesWith(Collidable other);

        /// <summary>
        /// gets the bounding sphere arround the collidable body 
        /// </summary>
        /// <returns>the sphere arround the body's bounding volume</returns>
        public abstract BoundingSphere GetBoundingSphere();

        /// <summary>
        /// updates the bounding Volume of this Collidable Object
        /// </summary>
        protected abstract void updateBounding();

        public override void Update(float duration)
        {
            base.Update(duration);
            updateBounding();
            //TODO after or before update ?
        }
    }
}
