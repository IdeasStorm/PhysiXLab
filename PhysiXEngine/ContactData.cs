using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace PhysiXEngine
{
    using PhysiXEngine.Helpers;
    public class ContactData
    {
        /**
         * Holds the bodies that are involved in the contact. The
         * second of these can be NULL, for contacts with the scenery.
         */
        public Collidable[] body = new Collidable[2];

        /**
         * Holds the position of the contact in world coordinates.
         */
        public Vector3 ContactPoint { get; protected set; }

        /**
         * Holds the direction of the contact in world coordinates.
         */
        public Vector3 ContactNormal { get; protected set; }

        /**
         * Holds the depth of penetration at the contact point. If both
         * bodies are specified then the contact point should be midway
         * between the inter-penetrating points.
         */
        public double Penetration { get; protected set; }

        public Matrix3 ContactToWorld { get; private set; }

        public float restitution { get; protected set; }
        public Vector3 contactVelocity { get; protected set; }
        //TODO remoce the var above me

        private Vector3[] relativeContactPosition = new Vector3[2];

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
            ContactNormal = Vector3.Multiply(midline, (float)(1.0f / size));
            ContactPoint = positionOne + Vector3.Multiply(midline, 0.5f);
            Penetration = ((Sphere)body[0]).radius + ((Sphere)body[1]).radius - size;
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
            ContactNormal = plane.direction;
            Penetration = -centreDistance;
            if (centreDistance < 0)
            {
                ContactNormal *= -1;
                Penetration = -Penetration;
            }
            Penetration += sphere.radius;

            ContactPoint = position - Vector3.Multiply(plane.direction, (float)centreDistance);
        }

        public void BoxHalfSpace()
        {
            Box box = (Box)body[0];
            Plane plane = (Plane)body[1];

            // We have an intersection, so find the intersection points. We can make
            // do with only checking vertices. If the box is resting on a plane
            // or on an edge, it will be reported as four or two contact points.
            Vector3[] cornars = box.box.GetCorners();
            // Go through each combination of + and - for each half-size
            /*double[,] mults = new double[8,3] {{1,1,1},{-1,1,1},{1,-1,1},{-1,-1,1},
                                       {1,1,-1},{-1,1,-1},{1,-1,-1},{-1,-1,-1}};*/

            for (int i = 0; i < 8; i++)
            {

                // Calculate the position of each vertex
                //Vector3 vertexPos = new Vector3((float)mults[i,0], (float)mults[i,1], (float)mults[i,2]);
                Vector3 vertexPos;
                vertexPos = Vector3.Cross(cornars[i], box.HalfSize);

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
                    ContactPoint = plane.direction;
                    ContactPoint = Vector3.Multiply(ContactPoint, (float)(vertexDistance - plane.offset));
                    ContactPoint += vertexPos;
                    ContactNormal = plane.direction;
                    Penetration = plane.offset - vertexDistance;

                }
            }
        }

        public void BoxAndBox()
        {
            Box box1 = (Box)body[0];
            Box box2 = (Box)body[1];

            Vector3 midLine = box2.Position - box1.Position;

        }

        /// <summary>
        /// Todo write a summry abou this method :)
        /// </summary>
        /// <param name="box"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float TransformToAxis(Box box, Vector3 axis)
        {
            return
            box.HalfSize.X * Math.Abs(Vector3.Dot(axis, box.GetAxis(0))) +
            box.HalfSize.Y * Math.Abs(Vector3.Dot(axis, box.GetAxis(1))) +
            box.HalfSize.Z * Math.Abs(Vector3.Dot(axis, box.GetAxis(2)));
        }

        /// <summary>
        ///  return true if the two boxes are overlaping in each other
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        bool OverlapOnAxis(Box one, Box two, Vector3 axis)
        {
            // Project the half-size of one onto axis.
            float oneProject = TransformToAxis(one, axis);
            float twoProject = TransformToAxis(two, axis);
            // Find the vector between the two centers.
            Vector3 toCenter = two.GetAxis(3) - one.GetAxis(3);
            // Project this onto the axis.
            float distance = Vector3.Dot(toCenter, axis);
            // Check for overlap.
            return (distance < oneProject + twoProject);
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
            if (Math.Abs(ContactNormal.X) > Math.Abs(ContactNormal.Y))
            {
                // Scaling factor to ensure the results are normalised
                double s = 1.0 / Math.Sqrt(ContactNormal.Z * ContactNormal.Z + ContactNormal.X * ContactNormal.X);

                // The new X-axis is at right angles to the world Y-axis
                contactTangent[0].X = ContactNormal.Z * (float)s;
                contactTangent[0].Y = 0.0f;
                contactTangent[0].Z = -ContactNormal.X * (float)s;

                // The new Y-axis is at right angles to the new X- and Z- axes
                contactTangent[1].X = ContactNormal.Y * contactTangent[0].X;
                contactTangent[1].Y = ContactNormal.Z * contactTangent[0].X - ContactNormal.X * contactTangent[0].Z;
                contactTangent[1].Z = -ContactNormal.Y * contactTangent[0].X;
            }
            else
            {
                // Scaling factor to ensure the results are normalised
                double s = 1.0 / Math.Sqrt(ContactNormal.Z * ContactNormal.Z +
                    ContactNormal.Y * ContactNormal.Y);

                // The new X-axis is at right angles to the world X-axis
                contactTangent[0].X = 0;
                contactTangent[0].Y = -ContactNormal.Z * (float)s;
                contactTangent[0].Z = ContactNormal.Y * (float)s;

                // The new Y-axis is at right angles to the new X- and Z- axes
                contactTangent[1].X = ContactNormal.Y * contactTangent[0].Z - ContactNormal.Z * contactTangent[0].Y;
                contactTangent[1].Y = -ContactNormal.X * contactTangent[0].Z;
                contactTangent[1].Z = ContactNormal.X * contactTangent[0].Y;
            }

            // Make a matrix from the three vectors.
            ContactToWorld.setComponents(ContactNormal, contactTangent[0], contactTangent[1]);

            //return new axis
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool BoxAndPoint(Box box, Vector3 point)
        {
            // Transform the point into box coordinates.

            Vector3 pointToBoxCor = Vector3.Transform(point, Matrix.Invert(box.TransformMatrix));

            Vector3 normal;

            // Check each axis looking for the axis on which the penetration is least deep.

            #region X axis
            float minDepth = box.HalfSize.X - Math.Abs(pointToBoxCor.X);

            if (minDepth < 0)
                return false;

            normal = box.GetAxis(0) * ((pointToBoxCor.X < 0) ? -1 : 1);
            #endregion
            #region Y axis
            float depth = box.HalfSize.Y - Math.Abs(pointToBoxCor.Y);

            if (depth < 0)
                return false;
            else
                if (depth < minDepth)
                {
                    minDepth = depth;
                    normal = box.GetAxis(1) * ((pointToBoxCor.Y < 0) ? -1 : 1);
                }
            #endregion
            #region Z axis
            depth = box.HalfSize.Z - Math.Abs(pointToBoxCor.Z);

            if (depth < 0)
                return false;
            else if (depth < minDepth)
            {
                minDepth = depth;
                normal = box.GetAxis(2) * ((pointToBoxCor.Z < 0) ? -1 : 1);
            }
            #endregion

            //filling data
            ContactNormal = normal;
            ContactPoint = point;
            Penetration = minDepth;

            body[0] = box;

            body[1] = null;

            //TOdo
            //restitution = TODO;
            //friction    = TODO;
            return true;
        }

        public int BoxAndSphere()
        {
            Box box;
            Sphere sphere;
            if (body[0] is Box)
            {
                box = (Box)body[0];
                sphere = (Sphere)body[1];
            }
            else
            {
                box = (Box)body[1];
                sphere = (Sphere)body[0];
            }
            
            // Transform the centre of the sphere into box coordinates
  
            Vector3 spherToBoxCor =Vector3.Transform(sphere.GetAxis(3),Matrix.Invert(box.TransformMatrix));

            // Early out check to see if we can exclude the contact
            if (Math.Abs(spherToBoxCor.X) - sphere.radius > box.HalfSize.X ||Math.Abs(spherToBoxCor.Y) - sphere.radius > box.HalfSize.Y ||Math.Abs(spherToBoxCor.Z) - sphere.radius > box.HalfSize.Z)
                return 0;

            Vector3 closestPt=new Vector3();
            float dist;

            // Clamp each coordinate to the box.
            dist = spherToBoxCor.X;
            if (dist > box.HalfSize.X) dist = box.HalfSize.X;
            if (dist < -box.HalfSize.X) dist = -box.HalfSize.X;
            closestPt.X = dist;

            dist = spherToBoxCor.Y;
            if (dist > box.HalfSize.Y) dist = box.HalfSize.Y;
            if (dist < -box.HalfSize.Y) dist = -box.HalfSize.Y;
            closestPt.Y = dist;

            dist = spherToBoxCor.Z;
            if (dist > box.HalfSize.Z) dist = box.HalfSize.Z;
            if (dist < -box.HalfSize.Z) dist = -box.HalfSize.Z;
            closestPt.Z = dist;

            //// Check we're in contact
            //dist = (closestPt - spherToBoxCor).squareMagnitude();
            //if (dist > sphere.radius * sphere.radius) return 0;

            //// Compile the contact
            //Vector3 closestPtWorld = box.transform.transform(closestPt);

            //Contact* contact = data->contacts;
            //contact->contactNormal = (closestPtWorld - centre);
            //contact->contactNormal.normalise();
            //contact->contactPoint = closestPtWorld;
            //contact->penetration = sphere.radius - real_sqrt(dist);
            //contact->setBodyData(box.body, sphere.body,
            //    data->friction, data->restitution);

            //data->addContacts(1);
            return 1;
        }

        public void InitializeAtMoment(float duration)
        {
            // Check if the first object is NULL, and swap if it is.
            if (body[0] == null)
            {
                body[0] = body[1];
                body[1] = null;
            }
            //assert(body[0]);
            //TODO check what is assert really doing

            // Calculate an set of axis at the contact point.
            calculateContactBasis();

            // Store the relative position of the contact relative to each body
            relativeContactPosition[0] = ContactPoint - body[0].Position;
            if (body[1] != null) {
                relativeContactPosition[1] = ContactPoint - body[1].Position;
            }

            // Find the relative velocity of the bodies at the contact point.
            contactVelocity = calculateLocalVelocity(0, duration);
            if (body[1] != null) {
                contactVelocity -= calculateLocalVelocity(1, duration);
            }

            // will Calculate the desired change in velocity for resolution after this method in ContactGenerator Class
        }

        Vector3 calculateLocalVelocity(int bodyIndex,float duration)
        {
            Body thisBody= body[bodyIndex];

            // Work out the velocity of the contact point.
            Vector3 velocity = Vector3.Cross(thisBody.Rotation , relativeContactPosition[bodyIndex]);
            velocity += thisBody.Velocity;

            // Turn the velocity into contact-coordinates.
            Vector3 contactVelocity = ContactToWorld.transformTranspose(velocity);

            // Calculate the ammount of velocity that is due to forces without
            // reactions.
            Vector3 accVelocity = thisBody.LastFrameAcceleration * duration;

            // Calculate the velocity in contact-coordinates.
            accVelocity = ContactToWorld.transformTranspose(accVelocity);

            // We ignore any component of acceleration in the contact normal 
            // direction, we are only interested in planar acceleration
            accVelocity.X = 0;

            // Add the planar velocities - if there's enough friction they will 
            // be removed during velocity resolution
            contactVelocity += accVelocity;

            // And return it
            return contactVelocity;
        }
    }
}
