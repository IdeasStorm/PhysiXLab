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
