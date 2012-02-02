using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace PhysiXEngine.Helpers
{
    public static class ExtensionMethods
    {
        public static Vector3 GetAxisVector(this Matrix matrix,int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector3(matrix.M11,matrix.M21,matrix.M31);
                case 1:
                    return new Vector3(matrix.M12, matrix.M22, matrix.M32);
                case 2:
                    return new Vector3(matrix.M13, matrix.M23, matrix.M33);
                case 3:
                    return new Vector3(matrix.M14, matrix.M24, matrix.M34);
                default:
                    return new Vector3();
            }
        }

        public static void normalized(ref Quaternion q)
        {
            float d = q.W * q.W + q.X * q.X + q.Y * q.Y + q.Z * q.Z;

            // Check for zero length quaternion, and use the no-rotation
            // quaternion in that case.
            if (d == 0)
            {
                q.W = 1;
                return;
            }

            d = (1.0f) /((float) Math.Sqrt(d));
            q.W *= d;
            q.X *= d;
            q.Y *= d;
            q.Z *= d;
        }

        public static Quaternion AddScaledVector(this Quaternion quat, Vector3 vector, float scale)
        {
            Quaternion q = new Quaternion(vector.X * scale, vector.Y * scale, vector.Z * scale, 0);
            q = q * quat;
            quat.W += q.W * 0.5f;
            quat.X += q.X * 0.5f;
            quat.Y += q.Y * 0.5f;
            quat.Z += q.Z * 0.5f;
            return quat;
        }

        enum CollisionType
        {
            None = 0x0,
            Box = 0x100,
            Sphere = 0x010,
            Plane = 0x001,
            Boxes = Box * 2,
            Spheres = Sphere *2,
            BoxAndSphere = Box + Sphere,
            BoxAndPlane = Box + Plane
        }
           
    }   
}
