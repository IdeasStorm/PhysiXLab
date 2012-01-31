using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    //public class Cable : Conductor
    //{
        
    //    ///<summry>
    //    ///Holds the maximum length of the cable.
    //    ///</summry>
    //    float maxLength;

    //    ///<summry>
    //    ///Holds the restitution (bounciness) of the cable.
    //    ///</summry>
    //    float restitution;

    //    public Cable(Collidable one, Collidable two, float max, float restitution)
    //    {
    //        body[0] = one;
    //        body[1] = two;
    //        maxLength = max;
    //        this.restitution = restitution;
    //    }

    //    ///<summry>
    //    /// Fills the given contact structure with the contact needed
    //    /// to keep the cable from over-extending.
    //    ///</summry>
    //    public uint addContact(Contact contact)
    //    {
    //        // Find the length of the cable
    //        float length = currentLength();

    //        // Check if we're over-extended
    //        if (length < maxLength)
    //        {
    //            return 0;
    //        }

    //        // Otherwise return the contact
    //        contact.body[0] = body[0];
    //        contact.body[1] = body[1];


    //        // Calculate the normal
    //        Vector3 normal = body[1].Position - body[0].Position;
    //        normal.Normalize();
    //        contact.ContactNormal = normal;

    //        contact.Penetration = length - maxLength;
    //        contact.restitution = restitution;

    //        return 1;
    //    }

    //}
    //public class Rod : Conductor
    //{
   
    //    /**
    //     * Holds the length of the rod.
    //     */
    //    float length;

    //    public Rod(Collidable one, Collidable two, float len, float restitution)
    //    {
    //        body[0] = one;
    //        body[1] = two;
    //        length = len;
    //    }

    //    /**
    //     * Fills the given contact structure with the contact needed
    //     * to keep the rod from extending or compressing.
    //     */
    //    public uint addContact(Contact contact)
    //    {
    //        // Find the length of the rod
    //        float currentLen = currentLength();

    //        // Check if we're over-extended
    //        if (currentLen == length)
    //        {
    //            return 0;
    //        }

    //        // Otherwise return the contact
    //        contact.body[0] = body[0];
    //        contact.body[1] = body[1];

    //        // Calculate the normal
    //        Vector3 normal = body[1].Position - body[0].Position;
    //        normal.Normalize();

    //        // The contact normal depends on whether we're extending or compressing
    //        if (currentLen > length)
    //        {
    //            contact.ContactNormal = normal;
    //            contact.Penetration = currentLen - length;
    //        }
    //        else
    //        {
    //            contact.ContactNormal = normal * -1;
    //            contact.Penetration = length - currentLen;
    //        }

    //        // Always use zero restitution (no bounciness)
    //        contact.restitution = 0;

    //        return 1;
    //    }
    //}
}
