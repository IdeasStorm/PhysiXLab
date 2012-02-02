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
        public abstract ContactData generateContacts(Collidable other);

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
        public abstract Boolean CollidesWith(HalfSpace plane);
        /// <summary>
        /// gets the bounding sphere arround the collidable body 
        /// </summary>
        /// <returns>the sphere arround the body's bounding volume</returns>
        public abstract BoundingSphere GetBoundingSphere();

        /// <summary>
        /// updates the bounding Volume of this Collidable Object
        /// </summary>
        protected abstract void updateBounding();

        protected override void onSituationChanged()
        {
            base.onSituationChanged();
            updateBounding();
        }

        public abstract Vector3 getHalfSize();
    }
}
