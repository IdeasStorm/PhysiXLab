using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace PhysiXEngine
{
    using PhysiXEngine.Helpers;
    public class Contact
    {
        /**
         * Holds the bodies that are involved in the contact. The
         * second of these can be NULL, for contacts with the scenery.
         */
        public Collidable[] body = new Collidable[2];

        //public ContactData contactData;

        /**
         * Holds the position of the contact in world coordinates.
         */
        private Vector3 _ContactPoint;
        public Vector3 ContactPoint { get { return _ContactPoint; } set { _ContactPoint = value; } }

        /**
         * Holds the direction of the contact in world coordinates.
         */
        private Vector3 _ContactNormal;
        public Vector3 ContactNormal { get { return _ContactNormal; } set { _ContactNormal = value; } }

        /**
         * Holds the depth of penetration at the contact point. If both
         * bodies are specified then the contact point should be midway
         * between the inter-penetrating points.
         */
        private float _Penetration;
        public float Penetration { get { return _Penetration; } set { _Penetration = value; } }

        private HalfSpace plane;

        public Matrix3 ContactToWorld { get; private set; }

        public float restitution { get; set; }
        public Vector3 contactVelocity { get; set; }

        public float desiredDeltaVelocity { get; set; }

        public  Vector3[] relativeContactPosition = new Vector3[2];

        public float friction { get; set; }

        public bool WithPlane { get; protected set; }

        public Contact(Collidable firstBody, Collidable secondBody)
        {
            this.body[0] = firstBody;
            this.body[1] = secondBody;
            ContactToWorld = new Matrix3();
            restitution = 0.7f;
            friction = 0.1f; // TODO add a dynamic mechanism

        }

        public Contact(Collidable firstBody, Plane plane)
        {
            this.body[0] = firstBody;
            this.body[1] = null;
            this.WithPlane = true;
            this.plane = new HalfSpace(plane);
            ContactToWorld = new Matrix3();
            restitution = 0.7f;
            friction = 0.3f; // TODO add a dynamic mechanism
        }

        #region Calculate internel information 

        /// <summary>
        /// Calculates and sets the internal value for the delta velocity.
        /// </summary>
        /// <param name="contactData">the contact Data that contains the bodies and contct informations</param>
        public void CalculateDeltaVelocity(float frameDuration)
        {
            const float velocityLimit = 0.25f;

            Body body1 = this.body[0];
            Body body2 = this.body[1];

            // NewVelocityCalculation
            // Calculate the acceleration induced velocity accumulated this frame
            float velocityFromAcc = Vector3.Dot(body1.LastFrameAcceleration,this._ContactNormal) * frameDuration ;

            if (body2 != null)
            {
                velocityFromAcc -= Vector3.Dot(body2.LastFrameAcceleration, this._ContactNormal) * frameDuration;
            }

            // If the velocity is very slow, limit the restitution
            float thisRestitution = this.restitution;
            if (Math.Abs(this.contactVelocity.X) < velocityLimit)
            {
                thisRestitution = 0.0f;
            }

            // Combine the bounce velocity with the removed
            // acceleration velocity.            
            this.desiredDeltaVelocity = -contactVelocity.X - thisRestitution * ((this.contactVelocity.X - velocityFromAcc));
            //this.desiredDeltaVelocity = Math.Abs(this.desiredDeltaVelocity);
            //contactData.desiredDeltaVelocity = deltaVelocity;
        }

        private void SwapBodies()
        {
            this._ContactNormal *= -1;

            Collidable temp = this.body[0];
            this.body[0] = this.body[1];
            this.body[1] = temp;
        }

        #endregion

        #region "contactData Extraction "

        bool overlapOnAxis(Box one, Box two, Vector3 axis, Vector3 toCentre)
        {
            // Project the half-size of one onto axis
            float oneProject = transformToAxis(one, axis);
            float twoProject = transformToAxis(two, axis);

            // Project this onto the axis
            float distance = Math.Abs(Vector3.Dot(toCentre, axis));

            // Check for overlap
            return (distance < oneProject + twoProject);
        }

        bool CheckBoxAndCheck(Box one, Box two)
        {
            //#define TEST_OVERLAP(axis) overlapOnAxis(one, two, (axis), toCentre)
            Vector3 toCentre = two.Position - one.Position;
            bool first = true, second = true, third = true;
            for (int i = 0; i < 3; i++)
            {
                first = first && overlapOnAxis(one, two, one.GetAxis(i), toCentre);
                second = second && overlapOnAxis(one, two, two.GetAxis(i), toCentre);
            }
            for (int i = 0,j=0, k=0; i < 9; i++, j++)
            {
                if (i % 3 == 0 && i != 0)
                {
                    j = 0;
                    k++;
                }
                third = third && overlapOnAxis(one, two, Vector3.Cross(one.GetAxis(k), two.GetAxis(j)),toCentre);
            }
            return first && second && third;
        }

        public bool SphereAndSphere()
        {            
            //Cache the sphere positions
            Sphere s1=((Sphere)body[0]);
            Sphere s2=((Sphere)body[1]);
            Vector3 positionOne = s1.Position;
            Vector3 positionTwo = s2.Position;
            if ((positionOne - positionTwo).Length() > s1.radius + s2.radius)
                return false;
            // Find the vector between the objects
            Vector3 midline = positionOne - positionTwo;
            float size = midline.Length();

            // We manually create the normal, because we have the
            // size to hand.
            _ContactNormal = Vector3.Multiply(midline, (float)(1.0f / size));
            _ContactPoint = positionOne + Vector3.Multiply(midline, 0.5f);
            _Penetration = ((Sphere)body[0]).radius + ((Sphere)body[1]).radius - size;
            return true;
        }

        public void SphereAndHalfSpace()
        {
            Sphere sphere = (Sphere)body[0];

            // Cache the sphere position
            Vector3 position = sphere.Position;

            // Find the distance from the plane
            float ballDistance = Vector3.Dot(plane.direction, position) - 
                sphere.radius - plane.offset;

            if (ballDistance >= 0) return;

            // Create the contact - it has a normal in the plane direction.
            _ContactNormal = plane.direction;
            _Penetration = -ballDistance;
            _ContactPoint = position - plane.direction * (ballDistance + sphere.radius);
        }

        public void SphereAndPlane()
        {
            Sphere sphere = (Sphere)body[0];

            // Cache the sphere position
            Vector3 position = sphere.Position;

            // Find the distance from the plane
            float centreDistance = Vector3.Dot(plane.direction, position) - plane.offset;

            // Check which side of the plane we're on
            _ContactNormal = plane.direction;
            _Penetration = -centreDistance;
            if (centreDistance < 0)
            {
                _ContactNormal *= -1;
                _Penetration = -_Penetration;
            }
            _Penetration += sphere.radius;

            _ContactPoint = position - Vector3.Multiply(plane.direction, (float)centreDistance);
        }

        public void BoxAndHalfSpace()
        {
            Box box = (Box)body[0];

            // We have an intersection, so find the intersection points. We can make
            // do with only checking vertices. If the box is resting on a plane
            // or on an edge, it will be reported as four or two contact points.
            Vector3[] cornars = new Vector3[12];//= box.box.GetCorners();
            //TODO HORRIBLE  initiate the var above
            // Go through each combination of + and - for each half-size

            for (int i = 0; i < 8; i++)
            {

                // Calculate the position of each vertex
                Vector3 vertexPos;
                vertexPos = Vector3.Transform(cornars[i], box.TransformMatrix);

                // Calculate the distance from the plane
                float vertexDistance = Vector3.Dot(vertexPos, plane.direction);

                // Compare this to the plane's distance
                if (vertexDistance <= plane.offset)
                {
                    // The contact point is halfway between the vertex and the
                    // plane - we multiply the direction by half the separation 
                    // distance and add the vertex location.
                    _ContactPoint = plane.direction;
                    _ContactPoint = Vector3.Multiply(_ContactPoint, (float)(vertexDistance - plane.offset));
                    _ContactPoint += vertexPos;
                    _ContactNormal = plane.direction;
                    _Penetration = plane.offset - vertexDistance;

                }
            }
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
            if (Math.Abs(_ContactNormal.X) > Math.Abs(_ContactNormal.Y))
            {
                // Scaling factor to ensure the results are normalised
                float s = 1.0f / (float)Math.Sqrt(_ContactNormal.Z * _ContactNormal.Z + _ContactNormal.X * _ContactNormal.X);

                // The new X-axis is at right angles to the world Y-axis
                contactTangent[0].X = _ContactNormal.Z * (float)s;
                contactTangent[0].Y = 0.0f;
                contactTangent[0].Z = -_ContactNormal.X * (float)s;

                // The new Y-axis is at right angles to the new X- and Z- axes
                contactTangent[1].X = _ContactNormal.Y * contactTangent[0].X;
                contactTangent[1].Y = _ContactNormal.Z * contactTangent[0].X - _ContactNormal.X * contactTangent[0].Z;
                contactTangent[1].Z = -_ContactNormal.Y * contactTangent[0].X;
            }
            else
            {
                // Scaling factor to ensure the results are normalised
                float s = 1.0f / (float)Math.Sqrt(_ContactNormal.Z * _ContactNormal.Z +
                    _ContactNormal.Y * _ContactNormal.Y);

                // The new X-axis is at right angles to the world X-axis
                contactTangent[0].X = 0;
                contactTangent[0].Y = -_ContactNormal.Z * (float)s;
                contactTangent[0].Z = _ContactNormal.Y * (float)s;

                // The new Y-axis is at right angles to the new X- and Z- axes
                contactTangent[1].X = _ContactNormal.Y * contactTangent[0].Z - _ContactNormal.Z * contactTangent[0].Y;
                contactTangent[1].Y = -_ContactNormal.X * contactTangent[0].Z;
                contactTangent[1].Z = _ContactNormal.X * contactTangent[0].Y;
            }

            // Make a matrix from the three vectors.
            ContactToWorld.setComponents(_ContactNormal, contactTangent[0], contactTangent[1]);

            //return new axis
        }

        /// <summary>
        /// fill contact data between box and point
        /// </summary>
        /// <param name="box"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        //public bool BoxAndPoint(Box box, Vector3 point)
        public void BoxAndPoint(Box box, Vector3 point)
        {
            // Transform the point into box coordinates.
            Vector3 pointToBoxCor = Vector3.Transform(point, Matrix.Invert(box.TransformMatrix));

            Vector3 normal;

            // Check each axis looking for the axis on which the penetration is least deep.
            #region X axis
            float minDepth = box.HalfSize.X - Math.Abs(pointToBoxCor.X);

            if (minDepth < 0)
                //return false;
                return;

            normal = box.GetAxis(0) * ((pointToBoxCor.X < 0) ? -1 : 1);
            #endregion
            #region Y axis
            float depth = box.HalfSize.Y - Math.Abs(pointToBoxCor.Y);

            if (depth < 0)
                //return false;
                return;
            else if (depth < minDepth)
            {
                minDepth = depth;
                normal = box.GetAxis(1) * ((pointToBoxCor.Y < 0) ? -1 : 1);
            }
            #endregion
            #region Z axis
            depth = box.HalfSize.Z - Math.Abs(pointToBoxCor.Z);

            if (depth < 0)
                //return false;
                return;
            else if (depth < minDepth)
            {
                minDepth = depth;
                normal = box.GetAxis(2) * ((pointToBoxCor.Z < 0) ? -1 : 1);
            }
            #endregion

            //filling data
            _ContactNormal = normal;
            _ContactPoint = point;
            _Penetration = minDepth;

            body[0] = box;

            // Note that we don’t know what rigid body the point
            // belongs to, so we just use NULL. Where this is called
            // this value can be left, or filled in.
            body[1] = null;

            //TODO
            //restitution = TODO;
            //friction    = TODO;
            //return true;
        }

        void ReOrder() {
            Collidable temp = body[0];
            body[0] = body[1];
            body[1] = temp;
        }

        //public bool BoxAndSphere()
        public bool SphereAndBox()
        {
            Sphere sphere = null;
            Box box = null;
            if (body[1] is Box)
                ReOrder();
            box = (Box)body[0];
            sphere = (Sphere)body[1];
            
            // Transform the centre of the sphere into box coordinates  
            Vector3 centre = sphere.Position;
            Vector3 spherToBoxCor = Vector3.Transform(centre, Matrix.Invert(box.TransformMatrix));
            //Vector3 spherToBoxCor = sphere.Position - box.Position;

            if (Math.Abs(spherToBoxCor.X) - sphere.radius > box.HalfSize.X ||
                Math.Abs(spherToBoxCor.Y) - sphere.radius > box.HalfSize.Y ||
                Math.Abs(spherToBoxCor.Z) - sphere.radius > box.HalfSize.Z)
            {
                return false;
            }

            //Vector3 closestPt = new Vector3();
            Vector3 closestPt = Vector3.Zero;
            float dist = 0f;

            // Find the closest point in the box to the target 
            //     point and generate the contact fromit
            // Clamp each coordinate to the box.
            dist = spherToBoxCor.X;
            if (dist > box.HalfSize.X)
                dist = box.HalfSize.X;
            if (dist < -box.HalfSize.X)
                dist = -box.HalfSize.X;
            closestPt.X = dist;

            dist = spherToBoxCor.Y;
            if (dist > box.HalfSize.Y)
                dist = box.HalfSize.Y;
            if (dist < -box.HalfSize.Y)
                dist = -box.HalfSize.Y;
            closestPt.Y = dist;

            dist = spherToBoxCor.Z;
            if (dist > box.HalfSize.Z)
                dist = box.HalfSize.Z;
            if (dist < -box.HalfSize.Z)
                dist = -box.HalfSize.Z;
            closestPt.Z = dist;

            // Check we're in contact
            dist = (closestPt - spherToBoxCor).LengthSquared();

            if (dist > sphere.radius * sphere.radius)
            {
                return false;
            }

            Vector3 closestPtWorld = Vector3.Transform(closestPt, box.TransformMatrix);
            
            _ContactNormal = closestPtWorld - centre;
            _ContactNormal.Normalize();
            _ContactPoint = closestPtWorld;
            _Penetration = sphere.radius - (float)Math.Sqrt(dist);
            return true;
        }

        public static float penetrationOnAxis(Box one, Box two, Vector3 axis, Vector3 toCentre)
        {
            // Project the half-size of one onto axis
            float oneProject = transformToAxis(one, axis);
            float twoProject = transformToAxis(two, axis);

            // Project this onto the axis
            float distance = Math.Abs(Vector3.Dot(toCentre, axis));

            // Return the overlap (i.e. positive indicates
            // overlap, negative indicates separation).
            return oneProject + twoProject - distance;
        }

        private static float transformToAxis(Box box, Vector3 axis)
        {
            return
                box.HalfSize.X * Math.Abs(Vector3.Dot(axis, box.GetAxis(0))) +
                box.HalfSize.Y * Math.Abs(Vector3.Dot(axis, box.GetAxis(1))) +
                box.HalfSize.Z * Math.Abs(Vector3.Dot(axis, box.GetAxis(2)));
        }

        private bool tryAxis(Box one, Box two, Vector3 axis, Vector3 toCentre, int index,
            // These values may be updated
            ref float smallestPenetration, ref int smallestCase)
        {
            if (axis.LengthSquared() < 0.0001) return true;
            axis.Normalize();
            float penetration = penetrationOnAxis(one, two, axis, toCentre);
            if (penetration < 0) // there is no Contact
                return false;
            if (penetration < smallestPenetration) // Set the Smallest
            {
                smallestPenetration = penetration;
                smallestCase = index;
            }
            return true;
        }

        void fillPointFaceBoxBox(Box one, Box two, Vector3 toCentre,
            int best, float pen )
        {
            // This method is called when we know that a vertex from
            // box two is in contact with box one.

            // We know which axis the collision is on (i.e. best), 
            // but we need to work out which of the two faces on 
            // this axis.
            Vector3 normal = one.GetAxis(best);
            if (Vector3.Dot(one.GetAxis(best), toCentre) > 0)
            {
                normal *= -1f;
            }

            // Work out which vertex of box two we're colliding with.
            // Using toCentre doesn't work!
            Vector3 vertex = two.HalfSize;
            if (Vector3.Dot(two.GetAxis(0), normal) < 0) 
                vertex.X = -vertex.X;
            if (Vector3.Dot(two.GetAxis(1), normal) < 0) 
                vertex.Y = -vertex.Y;
            if (Vector3.Dot(two.GetAxis(2), normal) < 0) 
                vertex.Z = -vertex.Z;
    
            // Create the contact data
            _ContactNormal = normal;
            _Penetration = pen;
            _ContactPoint = Vector3.Transform(vertex, two.TransformMatrix);
        }

        private static Vector3 contactPoint(Vector3 pOne, Vector3 dOne,
            float oneSize, Vector3 pTwo, Vector3 dTwo, float twoSize,

            // If this is true, and the contact point is outside
            // the edge (in the case of an edge-face contact) then
            // we use one's midpoint, otherwise we use two's.
            bool useOne)
        {
            Vector3 toSt, cOne, cTwo;
            float dpStaOne, dpStaTwo, dpOneTwo, smOne, smTwo;
            float denom, mua, mub;


            smOne = dOne.LengthSquared();
            smTwo = dTwo.LengthSquared();
            dpOneTwo = Vector3.Dot(dTwo, dOne);

            toSt = pOne - pTwo;
            dpStaOne = Vector3.Dot(dOne, toSt);
            dpStaTwo = Vector3.Dot(dTwo, toSt);

            denom = smOne * smTwo - dpOneTwo * dpOneTwo;

            // Zero denominator indicates parrallel lines
            if (Math.Abs(denom) < 0.0001f) {
                return useOne ? pOne : pTwo;
            }

            mua = (dpOneTwo * dpStaTwo - smTwo * dpStaOne) / denom;
            mub = (smOne * dpStaTwo - dpOneTwo * dpStaOne) / denom;

            // If either of the edges has the nearest point out
            // of bounds, then the edges aren't crossed, we have
            // an edge-face contact. Our point is on the edge, which
            // we know from the useOne parameter.
            if (mua > oneSize || mua < -oneSize ||
                mub > twoSize || mub < -twoSize) 
            {
                return useOne ? pOne : pTwo;
            }
            else
            {
                cOne = pOne + dOne * mua;
                cTwo = pTwo + dTwo * mub;

                return Vector3.Multiply(cOne, 0.5f) + Vector3.Multiply(cTwo, 0.5f);
            }
        }

        bool CheckBoxBoxQuick(Box one, Box two)
        {
            // TODO fix this Code |
            //                    v
            List<Vector3> intersectedCorners1 = new List<Vector3>();
            intersectedCorners1.RemoveAll((corner) => (false /* add prediction code */));
            if (intersectedCorners1.Count > 2)
            {
                Vector3 a = intersectedCorners1[0] - intersectedCorners1[1];
                Vector3 b = intersectedCorners1[2] - intersectedCorners1[0];
                Vector3 axis = Vector3.Cross(a,b);
                axis.Normalize();
                Vector3 cp = Vector3.Lerp(intersectedCorners1[1],intersectedCorners1[2],0.5f);                
                ContactPoint = cp;
                ContactNormal = axis;
                return true;
            }
            return false;
        }

        public bool BoxAndBox()
        {
            Box one = (Box)body[0];
            Box two = (Box)body[1];
           
            //Vector3 toCentre = two.GetAxis(3) - one.GetAxis(3);
            Vector3 toCentre = two.Position - one.Position;
            // We start assuming there is no contact
            float pen = float.MaxValue;
            int best = int.MaxValue;

            // Now we check each axes, returning if it gives us
            // a separating axis, and keeping track of the axis with
            // the smallest penetration otherwise.
            for (int i = 0; i < 3; i++)
            {
                if (!tryAxis(one, two, one.GetAxis(i), toCentre, i, ref pen, ref best))
                    return false;
            }

            for (int i = 0; i < 3; i++)
            {
                if (!tryAxis(one, two, two.GetAxis(i), toCentre, i+3, ref pen, ref best))
                    return false;
            }

            // Store the best axis-major, in case we run into almost
            // parallel edge collisions later
            int bestSingleAxis = best;

            for (int i = 0, j = 0, k = 0, w = 6; w < 15; i++, k++, w++)
            {
                if (i % 3 == 0 && i != 0)
                {
                    j++;
                    k = 0;
                }
                if (!tryAxis(one, two, Vector3.Cross(one.GetAxis(j), two.GetAxis(k)),
                    toCentre, w, ref pen, ref best))
                    return false;
            }

            // We now know there's a collision, and we know which
            // of the axes gave the smallest penetration. We now
            // can deal with it in different ways depending on
            // the case.
            if (best < 3)
            {
                // We've got a vertex of box two on a face of box one.
                fillPointFaceBoxBox(one, two, toCentre, best, pen);
                return true;
            }
            else if (best < 6)
            {
                // We've got a vertex of box one on a face of box two.
                // We use the same algorithm as above, but swap around
                // one and two (and therefore also the vector between their 
                // centres).
                fillPointFaceBoxBox(two, one, toCentre * -1.0f, best - 3, pen);
                return true;
            }
            else
            {
                // We've got an edge-edge contact. Find out which axes
                best -= 6;
                int oneAxisIndex = best / 3;
                int twoAxisIndex = best % 3;
                Vector3 oneAxis = one.GetAxis((int)oneAxisIndex);
                Vector3 twoAxis = two.GetAxis((int)twoAxisIndex);
                Vector3 axis = Vector3.Cross(oneAxis, twoAxis);
                if (axis.Length() != 0 ) axis.Normalize();

                // The axis should point from box one to box two.
                if (Vector3.Dot(axis, toCentre) > 0) 
                    axis = axis * -1.0f;

                // We have the axes, but not the edges: each axis has 4 edges parallel 
                // to it, we need to find which of the 4 for each object. We do 
                // that by finding the point in the centre of the edge. We know 
                // its component in the direction of the box's collision axis is zero 
                // (its a mid-point) and we determine which of the extremes in each 
                // of the other axes is closest.
                Vector3 ptOnOneEdge = one.HalfSize;
                Vector3 ptOnTwoEdge = two.HalfSize;

                float oneVal = 0f, twoVal = 0f;
                for (int i = 0; i < 3; i++)
                {
                    // This doesn't work!
                    if (i == oneAxisIndex)
                    {
                        if (i == 0) ptOnOneEdge.X = 0;
                        if (i == 1) ptOnOneEdge.Y = 0;
                        if (i == 2) ptOnOneEdge.Z = 0;
                    }
                    else if (Vector3.Dot(one.GetAxis(i), axis) > 0)
                    {
                        if (i == 0) ptOnOneEdge.X *= -1;
                        if (i == 1) ptOnOneEdge.Y *= -1;
                        if (i == 2) ptOnOneEdge.Z *= -1;
                    }

                    if (i == twoAxisIndex) 
                    {
                        if (i == 0) ptOnTwoEdge.X = 0;
                        if (i == 1) ptOnTwoEdge.Y = 0;
                        if (i == 2) ptOnTwoEdge.Z = 0;
                    }
                    else if (Vector3.Dot(two.GetAxis(i), axis) < 0)
                    {
                        if (i == 0) ptOnTwoEdge.X *= -1;
                        if (i == 1) ptOnTwoEdge.Y *= -1;
                        if (i == 2) ptOnTwoEdge.Z *= -1;
                    }

                }

                // Move them into world coordinates (they are already oriented
                // correctly, since they have been derived from the axes).
                ptOnOneEdge = Vector3.Transform(ptOnOneEdge, one.TransformMatrix);
                ptOnTwoEdge = Vector3.Transform(ptOnTwoEdge, two.TransformMatrix);

                // So we have a point and a direction for the colliding edges.
                // We need to find out point of closes approach of the two 
                // line-segments.

                if (oneAxisIndex == 0) oneVal = one.HalfSize.X;
                if (oneAxisIndex == 1) oneVal = one.HalfSize.Y;
                if (oneAxisIndex >= 2) oneVal = one.HalfSize.Z;

                if (twoAxisIndex == 0) twoVal = two.HalfSize.X;
                if (twoAxisIndex == 1) twoVal = two.HalfSize.Y;
                if (twoAxisIndex >= 2) twoVal = two.HalfSize.Z;

                Vector3 vertex = contactPoint(ptOnOneEdge, oneAxis, oneVal,
                        ptOnTwoEdge, twoAxis, twoVal, bestSingleAxis > 2);

                // We can fill the contact.
                axis.Normalize();
                _Penetration = pen;
                _ContactNormal = axis;
                _ContactPoint = vertex;
                return true;
            }
            
        }
        #endregion

        #region "Collision/Penetration Resulution"
        public void InitializeAtMoment(float duration)
        {
            // Check if the first object is NULL, and swap if it is.
            if (body[0] == null)
            {
                body[0] = body[1];
                body[1] = null;
            }

            // Calculate an set of axis at the contact point.
            calculateContactBasis();

            // Store the relative position of the contact relative to each body
            relativeContactPosition[0] = _ContactPoint - body[0].Position;
            if (body[1] != null) {
                relativeContactPosition[1] = _ContactPoint - body[1].Position;
            }

            // Find the relative velocity of the bodies at the contact point.
            contactVelocity = calculateLocalVelocity(0, duration);
            if (body[1] != null) {
                contactVelocity -= calculateLocalVelocity(1, duration);
            }
            //TODO performance issue 2 calls !!
            // Calculate the desired change in velocity for resolution
            CalculateDeltaVelocity(duration);
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

        public void WakeUpPair()
        {
            // don't wake up if collided with the world
            if (body[1] == null) return;
            // Wake up only the sleeping one
            if (body[0].IsAwake ^ body[1].IsAwake)
            {
                if (body[0].IsAwake) body[1].Awake();
                else body[0].Awake();
            }
        }

        internal void applyPositionChange(Vector3[] linearChange, Vector3[] angularChange,float Penetration)
            //unused penetration
        {
            float angularLimit = 0.1f;//0.1f;
            float[] angularMove = new float[2],
                    linearMove = new float[2];

            float totalInertia = 0;
            float[] linearInertia = new float[2];
            float[] angularInertia = new float[2];

            
            // We need to work out the inertia of each object in the direction
            // of the contact normal, due to angular inertia only. 
            for (int i = 0; i < 2; i++) {
                if (body[i] != null)
                {
                    Matrix3 inverseInertiaTensor = body[i].InverseInertiaTensorWorld;

                    // Use the same procedure as for calculating frictionless
                    // velocity change to work out the angular inertia.
                    Vector3 angularInertiaWorld = Vector3.Cross(relativeContactPosition[i] , _ContactNormal);
                    angularInertiaWorld = inverseInertiaTensor.transform(angularInertiaWorld);
                    angularInertiaWorld = Vector3.Cross(angularInertiaWorld , relativeContactPosition[i]);
                    angularInertia[i] = Vector3.Dot(angularInertiaWorld , _ContactNormal);

                    // The linear component is simply the inverse mass
                    linearInertia[i] = body[i].InverseMass;
                    // Keep track of the total inertia from all components
                    totalInertia += linearInertia[i] + angularInertia[i];
                }
            }

            

            for (int i = 0; i < 2; i++)
            {
                if (body[i] != null)
                {
                    float sign = (i == 0) ? 1 : -1;
                    angularMove[i] = sign * Penetration * (angularInertia[i] / totalInertia);
                    linearMove[i] = sign * Penetration * (linearInertia[i] / totalInertia);

                    // To avoid angular projections that are too great (when mass is large
                    // but inertia tensor is small) limit the angular move.
                    Vector3 projection = relativeContactPosition[i] 
                        + ContactNormal * Vector3.Dot(-relativeContactPosition[i],ContactNormal);
                    
                    // Use the small angle approximation for the sine of the angle (i.e.
                    // the magnitude would be sine(angularLimit) * projection.magnitude
                    // but we approximate sine(angularLimit) to angularLimit).
                    float maxMagnitude = angularLimit * projection.Length();

                    if (angularMove[i] < -maxMagnitude)
                    {
                        float totalMove = angularMove[i] + linearMove[i];
                        angularMove[i] = -maxMagnitude;
                        linearMove[i] = totalMove - angularMove[i];
                    }
                    else if (angularMove[i] > maxMagnitude)
                    {
                        float totalMove = angularMove[i] + linearMove[i];
                        angularMove[i] = maxMagnitude;
                        linearMove[i] = totalMove - angularMove[i];
                    }

                    // We have the linear amount of movement required by turning
                    // the rigid body (in angularMove[i]). We now need to
                    // calculate the desired rotation to achieve that.
                    if (angularMove[i] == 0)
                    {
                        // Easy case - no angular movement means no rotation.
                        angularChange[i] = Vector3.Zero;
                    }
                    else
                    {
                        // Work out the direction we'd like to rotate in.
                        Vector3 targetAngularDirection = Vector3.Cross(relativeContactPosition[i], ContactNormal);

                        Matrix3 inverseInertiaTensor = body[i].InverseInertiaTensorWorld;

                        // Work out the direction we'd need to rotate to achieve that
                        angularChange[i] =
                            inverseInertiaTensor.transform(targetAngularDirection) *
                            (angularMove[i] / angularInertia[i]);
                    }

                    //calculate linear change
                    linearChange[i] = ContactNormal * linearMove[i];

                    // applying changes
                    body[i].Position += linearChange[i];
                    body[i].AddScaledOrientation(angularChange[i],1f);

                }
            }
            
        }
        #endregion

        /// <summary>
        /// Fixes Penetration Problem using "Imponderable Penetration Resolving Algorithm"
        /// it's a kind of binarysearch between collision moment and the moment before it
        /// based on penetration sign (+/-)
        /// <author>MhdSyrwan</author>
        /// </summary>
        public void FixPenetration(float duration)
        {
            // select the faster body
            //ReOrder();
            Collidable chosen;
            if (body[0] == null) 
                chosen = body[1];
            else
                chosen = (this.body[0].Velocity.Length() > this.body[1].Velocity.Length()) ? body[0] : body[1];
            this.revertState();
            this.Check();
            return;
            // revert to the moment before collision moment
            // starting binary search loop
            do
            {
                //if ((PositionA - PositionB).Length() <= 0.00001) return;
                chosen.Position += chosen.Velocity * duration * 0.1f;
                chosen.Orientation = chosen.Orientation.AddScaledVector(chosen.Rotation, duration * 0.1f);
            } while (!this.Check());
            //this.InitializeAtMoment(duration);
            
        }

        public ContactData GetContactData()
        {
            ContactData contactData = new ContactData();
            contactData.ContactNormal = _ContactNormal;
            contactData.ContactPoint = _ContactPoint;
            contactData.Penetration = _Penetration;
            return contactData;
        }

        public void FillFromContactData(ContactData contactData)
        {
            _ContactNormal = contactData.ContactNormal;
            _ContactPoint = contactData.ContactPoint;
            _Penetration = contactData.Penetration;
        }

        /// <summary>
        /// Refills contact data
        /// </summary>
        public bool Check()
        {
            Penetration = 0;
            if (WithPlane)
                return plane.generateContacts(body[0], this);
            else
            {
                try // try if the first know any thing about the other
                {
                    return body[0].generateContacts(body[1], this);
                }
                catch (Exception)
                {
                    return body[1].generateContacts(body[0], this);
                }
            }
        }

        /// <summary>
        /// detects whether contact has a collision actually or not
        /// </summary>
        /// <returns></returns>
        public bool IsColliding()
        {
            if (this.WithPlane)
                return body[0].CollidesWith(plane);
            else
                return body[0].CollidesWith(body[1]);
            //TODO add try catch IDontKnowException
        }

        public bool BothFixed()
        {
            return (body[0].InverseMass + body[1].InverseMass == 0);
        }

        public bool equals(object obj)
        {
            Contact other = obj as Contact;
            if (other != null)
            {
                if ( ((other.body[0] == body[0]) && (other.body[1] == body[1]))
                    || ((other.body[0] == body[1] ) && (other.body[1]==body[0]) ) )
                    return true;
            }
            return false;
        }

        public void revertState()
        {
            Collidable chosen;
            if (body[0] == null)
                chosen = body[1];
            else
                chosen = (this.body[0].Velocity.Length() > this.body[1].Velocity.Length()) ? body[0] : body[1];
            chosen.RevertChanges();
        }
    }
}
