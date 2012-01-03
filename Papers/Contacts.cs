/**
 * Constructs an arbitrary orthonormal basis for the contact.
 * This is stored as a 3x3 matrix, where each vector is a column
 * (in other words the matrix transforms contact space into world
 * space). The x direction is generated from the contact normal,
 * and the y and z directionss are set so they are at right angles to
 * it.
 */
public void calculateContactBasis()
{
    Vector3[] contactTangent = new Vector3[2];

    // Check whether the Z-axis is nearer to the X or Y axis
    if(Math.Abs(contactNormal.X) > Math.Abs(contactNormal.Y))
    { 
	// Scaling factor to ensure the results are normalised
        double s = 1.0 / Math.Sqrt(contactNormal.Z * contactNormal.Z + contactNormal.X * contactNormal.X);

        // The new X-axis is at right angles to the world Y-axis
        contactTangent[0].X = contactNormal.Z * (float)s;
        contactTangent[0].Y = 0.0f;
        contactTangent[0].Z = -contactNormal.X * (float)s;

        // The new Y-axis is at right angles to the new X- and Z- axes
        contactTangent[1].X = contactNormal.Y * contactTangent[0].X;
        contactTangent[1].Y = contactNormal.Z * contactTangent[0].X - contactNormal.X * contactTangent[0].Z;
        contactTangent[1].Z = -contactNormal.Y*contactTangent[0].X;
    }
    else
    {
        // Scaling factor to ensure the results are normalised
        double s = 1.0 / Math.Sqrt(contactNormal.Z * contactNormal.Z + 
            contactNormal.Y*contactNormal.Y);

        // The new X-axis is at right angles to the world X-axis
        contactTangent[0].X = 0;
        contactTangent[0].Y = -contactNormal.Z * (float)s;
        contactTangent[0].Z = contactNormal.Y * (float)s;

        // The new Y-axis is at right angles to the new X- and Z- axes
        contactTangent[1].X = contactNormal.Y*contactTangent[0].Z - contactNormal.Z*contactTangent[0].Y;
        contactTangent[1].Y = -contactNormal.X*contactTangent[0].Z;
        contactTangent[1].Z = contactNormal.X*contactTangent[0].Y;
    }

    //return new axis
}