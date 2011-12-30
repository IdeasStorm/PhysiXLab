using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Physix
{
    /**
    * A helper class that contains information for the detector to use
    * in building its contact data.
    */
    class CollisionData
    {
        /** Holds the contact array to write into. */
        Contact[] contacts;

        /** Holds the maximum number of contacts the array can take. */
        ulong contactsLeft;
    }
}
