using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace AnarchyEngine.Core.Helpers
{
    public struct Matrix3
    {
        public Matrix3(float f)
        {
            this.Elements = new float[3, 3];
        }

        public Matrix3(Matrix m)
        {
            this.Elements = new float[3, 3];

            this.Elements[0, 0] = m.M11;
            this.Elements[0, 1] = m.M12;
            this.Elements[0, 2] = m.M13;

            this.Elements[1, 0] = m.M21;
            this.Elements[1, 1] = m.M22;
            this.Elements[1, 2] = m.M23;

            this.Elements[2, 0] = m.M31;
            this.Elements[2, 1] = m.M32;
            this.Elements[2, 2] = m.M33;
        }

        public Matrix3(Matrix3 m)
        {
            this.Elements = new float[3, 3];

            this.Elements[0, 0] = m[0, 0];
            this.Elements[0, 1] = m[0, 1];
            this.Elements[0, 2] = m[0, 2];

            this.Elements[1, 0] = m[1, 0];
            this.Elements[1, 1] = m[1, 1];
            this.Elements[1, 2] = m[1, 2];

            this.Elements[2, 0] = m[2, 0];
            this.Elements[2, 1] = m[2, 1];
            this.Elements[2, 2] = m[2, 2];
        }

        public float this[int row, int column]
        {
            get
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                return this.Elements[row, column];
            }
            set
                {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                this.Elements[row, column] = value;
            }
        }

        public Vector3 Row(int row)
        {
            return new Vector3(this.Elements[row, 0],
                               this.Elements[row, 1],
                               this.Elements[row, 2]);
        }

        public Vector3 Column(int column)
        {
            return new Vector3(this.Elements[0, column],
                               this.Elements[1, column],
                               this.Elements[2, column]);
        }

        float[,] Elements;
    }

    public static class Matrices
    {
        public static Vector3 Multiply(Vector3 v, Matrix m)
        {
            return
                new Vector3(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + m.M41,
                            v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + m.M42,
                            v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + m.M43);
        }

        public static Matrix3 Abs(Matrix3 m3)
        {
            Matrix3 absMatrix = new Matrix3(0);

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    absMatrix[r, c] = Math.Abs(m3[r, c]);
                }
            }

            return absMatrix;
        }

    }
}