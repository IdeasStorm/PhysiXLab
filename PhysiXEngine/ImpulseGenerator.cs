using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    class ImpulseGenerator : Effect
    {
        protected Dictionary<Body, Body> ContactsList;

        public ImpulseGenerator()
        {
            ContactsList = new Dictionary<Body, Body>();
        }

        /// <summary>
        /// add new pair of Body will colliding
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        public void addCotactPair(Body body1, Body body2)
        {
            ContactsList.Add(body1, body2);
        }

        /// <summary>
        /// delete all information in the contacts list
        /// </summary>
        public void clearContactList()
        {
            ContactsList.Clear();
        }

        /// <summary>
        /// passing all contacts list calling affect function
        /// to affect each body by the another
        /// </summary>
        /// <param name="time"></param>
        public override void update(float time)
        {
            foreach (KeyValuePair<Body, Body> contactsPair in ContactsList)
            {
                Affect(contactsPair.Key, contactsPair.Value, time);
            }
        }

        public override void Affect(Body body1, Body body2, float duration)
        {
            
        }

        private void calculateDesiredDeltaVelocity(float duration)
        { 

        }

    }
}
