using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace PhysiXEngine
{
    class ContactData
    {
        /**
         * Holds the bodies that are involved in the contact. The
         * second of these can be NULL, for contacts with the scenery.
         */
        private Collidable[] body = new Collidable[2];

        /**
         * Holds the position of the contact in world coordinates.
         */
        public Vector3 contactPoint { public get; protected set; }

        /**
         * Holds the direction of the contact in world coordinates.
         */
        public Vector3 contactNormal { public get; protected set; }

        /**
         * Holds the depth of penetration at the contact point. If both
         * bodies are specified then the contact point should be midway
         * between the inter-penetrating points.
         */
        public double penetration { public get; protected set; }

        public Matrix ContactToWorld { public get; private set; }

        public float restitution { public get; protected set; }
        public Vector3 contactVelocity { public get; protected set; }


        public ContactData(Collidable firstBody, Collidable secondBody)
        {
            this.body[0] = firstBody;
            this.body[1] = secondBody;
        }

        public void SphereAndSphere()
        {
            //Cache the sphere positions
            Vector3 positionOne = ((Sphere)body[0]).Position;
            Vector3 positionTwo = ((Sphere)body[1]).Position;

            // Find the vector between the objects
            Vector3 midline = positionOne - positionTwo;
            double size = midline.Length();

            // We manually create the normal, because we have the
            // size to hand.
            contactNormal = Vector3.Multiply(midline,(float)(1.0f/size));
            contactPoint = positionOne + Vector3.Multiply(midline,0.5f);
            penetration = ((Sphere)body[0]).radius + ((Sphere)body[1]).radius - size;
        }

        public void SphereAndPlane()
        {
            Sphere sphere = (Sphere)body[0];
            Plane plane = (Plane)body[1];

            // Cache the sphere position
            Vector3 position = sphere.Position;

            // Find the distance from the plane
            double centreDistance = Vector3.Dot(plane.direction, position) - plane.offset;

            // Check which side of the plane we're on
            contactNormal = plane.direction;
            penetration = -centreDistance;
            if (centreDistance < 0)
            {
                contactNormal *= -1;
                penetration = -penetration;
            }
            penetration += sphere.radius;

            contactPoint = position - Vector3.Multiply(plane.direction, (float)centreDistance);
        }

        public void BoxAndSphere()
        {
            Box box = (Box)body[0];
            Sphere sphere = (Sphere)body[1];
            
        }

        public void BoxHalfSpace()
        {
            Box box = (Box)body[0];
            Plane plane = (Plane)body[1];

            // We have an intersection, so find the intersection points. We can make
            // do with only checking vertices. If the box is resting on a plane
            // or on an edge, it will be reported as four or two contact points.

            // Go through each combination of + and - for each half-size
            double[,] mults = new double[8,3] {{1,1,1},{-1,1,1},{1,-1,1},{-1,-1,1},
                                       {1,1,-1},{-1,1,-1},{1,-1,-1},{-1,-1,-1}};    
            

            //Contact* contact = data->contacts;
            //unsigned contactsUsed = 0;
            for (int i = 0; i < 8; i++) {

                // Calculate the position of each vertex
                Vector3 vertexPos = new Vector3((float)mults[i,0], (float)mults[i,1], (float)mults[i,2]);

                //vertexPos.componentProductUpdate(box.halfSize);
                //vertexPos = box.transform.transform(vertexPos);

                ///>BoxPlaneTestOne
                // Calculate the distance from the plane
                double vertexDistance = Vector3.Dot(vertexPos, plane.direction);

                // Compare this to the plane's distance
                if (vertexDistance <= plane.offset)
                {
                    // The contact point is halfway between the vertex and the
                    // plane - we multiply the direction by half the separation 
                    // distance and add the vertex location.
                    contactPoint = plane.direction;
                    contactPoint = Vector3.Multiply(contactPoint,(float)(vertexDistance - plane.offset));
                    contactPoint += vertexPos;
                    contactNormal = plane.direction;
                    penetration = plane.offset - vertexDistance;

                }
            }
        }

        public void BoxAndBox()
        {
            Box box = (Box)body[0];
            Plane sphere = (Plane)body[1];

        }

        /// <summary>
        /// Constructs an arbitrary orthonormal basis for the contact.
        ///  This is stored as a 3x3 matrix, where each vector is a column
        /// (in other words the matrix transforms contact space into world
        ///space). The x direction is generated from the contact normal,
        /// and the y and z directionss are set so they are at right angles to
        /// it.
        /// </summary>  
        public void calculateContactBasis()
        {
            Vector3[] contactTangent = new Vector3[2];

            // Check whether the Z-axis is nearer to the X or Y axis
            if (Math.Abs(contactNormal.X) > Math.Abs(contactNormal.Y))
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
                contactTangent[1].Z = -contactNormal.Y * contactTangent[0].X;
            }
            else
            {
                // Scaling factor to ensure the results are normalised
                double s = 1.0 / Math.Sqrt(contactNormal.Z * contactNormal.Z +
                    contactNormal.Y * contactNormal.Y);

                // The new X-axis is at right angles to the world X-axis
                contactTangent[0].X = 0;
                contactTangent[0].Y = -contactNormal.Z * (float)s;
                contactTangent[0].Z = contactNormal.Y * (float)s;

                // The new Y-axis is at right angles to the new X- and Z- axes
                contactTangent[1].X = contactNormal.Y * contactTangent[0].Z - contactNormal.Z * contactTangent[0].Y;
                contactTangent[1].Y = -contactNormal.X * contactTangent[0].Z;
                contactTangent[1].Z = contactNormal.X * contactTangent[0].Y;
            }

            // Make a matrix from the three vectors.
            ContactToWorld = Matrix.CreateWorld(contactNormal, contactTangent[0], contactTangent[1]);
            
            
            //return new axis
        }

    }

}
