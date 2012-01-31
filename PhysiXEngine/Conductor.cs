using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    /// <summary>
    /// This is the basic polymorphic interface for contact generators
    /// applying to particles
    /// </summary>
    public class Conductor
    {
        public Collidable[] body = new Collidable[2];


        ///<summary>
        /// Fills the given contact structure with the generated
        /// contact. The contact pointer should point to the first
        /// available contact in a contact array, where limit is the
        /// maximum number of contacts in the array that can be written
        /// to. The method returns the number of contacts that have
        /// been written.
        ///</summary>
        //public uint addContact(Contact contact);
        //public abstract uint addContact();
        //abstract public uint addContact(Contact contact);

        /**
        * Returns the current length of the link.
        */
        public float currentLength()
        {
            Vector3 length = body[0].Position - body[1].Position;
            return length.Length();
        }
    }
}
