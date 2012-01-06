using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace PhysiXEngine.Helpers
{
    public static class ExtensionMethods
    {
        public static Vector3 getAxisVector(this Matrix matrix,int index)
        {
            switch (index)
            {
                case 0:
                    return new Vector3(matrix.M11,matrix.M21,matrix.M31);
                case 1:
                    return new Vector3(matrix.M12, matrix.M22, matrix.M32);
                case 3:
                    return new Vector3(matrix.M13, matrix.M23, matrix.M33);
                default:
                    return new Vector3();
            }
        }
           
    }   
}
