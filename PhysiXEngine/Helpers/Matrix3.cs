using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysiXEngine.Helpers
{
    ///<summary>
    ///Holds an inertia tensor, consisting of a 3x3 row-major matrix.
    ///This matrix is not padding to produce an aligned structure, since
    ///it is most commonly used with a mass (single float) and two 
    ///damping coefficients to make the 12-element characteristics array
    ///of a rigid body.
    ///</summary>
    public class Matrix3
    {
        ///<summary>
        ///Holds the tensor matrix data in array form.
        ///</summary>
        public float[] data = new float[9];

        ///<summary>
        ///Creates a new matrix.
        ///</summary>
        public Matrix3()
        {
            data[0] = data[1] = data[2] = data[3] = data[4] = data[5] =
                data[6] = data[7] = data[8] = 0;
        }

        /// <summary>
        /// Constructs a Matrix3x3 from a Matrix4x4
        /// </summary>
        /// <param name="m">Matrix to copy from</param>
        public Matrix3(Matrix m)
        {
            this.data[0] = m.M11;
            this.data[1] = m.M12;
            this.data[2] = m.M13;

            this.data[3] = m.M21;
            this.data[4] = m.M22;
            this.data[5] = m.M23;

            this.data[6] = m.M31;
            this.data[7] = m.M32;
            this.data[8] = m.M33;
        }

        ///<summary>
        ///Creates a new matrix with the given three vectors making
        ///up its columns.
        ///</summary>
        public Matrix3(Vector3 compOne, Vector3 compTwo,
            Vector3 compThree)
        {
            setComponents(compOne, compTwo, compThree);
        }

        ///<summary>
        ///Creates a new matrix with explicit coefficients.
        ///</summary>
        public Matrix3(float c0, float c1, float c2, float c3, float c4, float c5,
            float c6, float c7, float c8)
        {
            data[0] = c0; data[1] = c1; data[2] = c2;
            data[3] = c3; data[4] = c4; data[5] = c5;
            data[6] = c6; data[7] = c7; data[8] = c8;
        }

        ///<summary>
        ///Sets the matrix to be a diagonal matrix with the given
        ///values along the leading diagonal.
        ///</summary>
        public void setDiagonal(float a, float b, float c)
        {
            setInertiaTensorCoeffs(a, b, c);
        }

        ///<summary> 
        ///Sets the value of the matrix from inertia tensor values.
        ///</summary>
        public void setInertiaTensorCoeffs(float ix, float iy, float iz,
            float ixy = 0, float ixz = 0, float iyz = 0)
        {
            data[0] = ix;
            data[1] = data[3] = -ixy;
            data[2] = data[6] = -ixz;
            data[4] = iy;
            data[5] = data[7] = -iyz;
            data[8] = iz;
        }

        ///<summary>
        ///Sets the value of the matrix as an inertia tensor of
        ///a rectangular block aligned with the body's coordinate 
        ///system with the given axis half-sizes and mass.
        ///</summary>
        public void setBlockInertiaTensor(Vector3 halfSizes, float mass)
        {
            Vector3 squares = new Vector3(halfSizes.X * halfSizes.X, halfSizes.Y * halfSizes.Y, halfSizes.Z * halfSizes.Z);
            setInertiaTensorCoeffs(0.3f * mass * (squares.Y + squares.Z),
                0.3f * mass * (squares.X + squares.Z),
                0.3f * mass * (squares.X + squares.Y));
        }

        ///<summary>
        ///Sets the matrix to be a skew symmetric matrix based on
        ///the given vector. The skew symmetric matrix is the equivalent
        ///of the vector product. So if a,b are vectors. a x b = A_s b
        ///where A_s is the skew symmetric form of a.
        ///</summary>
        public void setSkewSymmetric(Vector3 vector)
        {
            data[0] = data[4] = data[8] = 0;
            data[1] = -vector.Z;
            data[2] = vector.Y;
            data[3] = vector.Z;
            data[5] = -vector.X;
            data[6] = -vector.Y;
            data[7] = vector.X;
        }

        ///<summary>
        ///Sets the matrix values from the given three vector components.
        ///These are arranged as the three columns of the vector.
        ///</summary>
        public void setComponents(Vector3 compOne, Vector3 compTwo,
             Vector3 compThree)
        {
            data[0] = compOne.X;
            data[1] = compTwo.X;
            data[2] = compThree.X;
            data[3] = compOne.Y;
            data[4] = compTwo.Y;
            data[5] = compThree.Y;
            data[6] = compOne.Z;
            data[7] = compTwo.Z;
            data[8] = compThree.Z;

        }

        ///<summary>
        ///Transform the given vector by this matrix.
        ///
        ///@param vector The vector to transform.
        ///</summary>
        public static Vector3 operator *(Matrix3 m, Vector3 vector)
        {
            return new Vector3(
                vector.X * m.data[0] + vector.Y * m.data[1] + vector.Z * m.data[2],
                vector.X * m.data[3] + vector.Y * m.data[4] + vector.Z * m.data[5],
                vector.X * m.data[6] + vector.Y * m.data[7] + vector.Z * m.data[8]
            );
        }

        ///<summary>
        ///Transform the given vector by this matrix.
        ///
        ///@param vector The vector to transform.
        ///</summary>
        public Vector3 transform(Vector3 vector)
        {
            return (this) * vector;
        }

        ///<summary>
        ///Transform the given vector by the transpose of this matrix.
        ///
        ///@param vector The vector to transform.
        ///</summary>
        public Vector3 transformTranspose(Vector3 vector)
        {
            return new Vector3(
                vector.X * data[0] + vector.Y * data[3] + vector.Z * data[6],
                vector.X * data[1] + vector.Y * data[4] + vector.Z * data[7],
                vector.X * data[2] + vector.Y * data[5] + vector.Z * data[8]
            );
        }

        ///<summary>
        ///Gets a vector representing one row in the matrix.
        ///
        ///@param i The row to return.
        ///</summary>
        public Vector3 getRowVector(int i)
        {
            return new Vector3(data[i * 3], data[i * 3 + 1], data[i * 3 + 2]);
        }

        ///<summary>
        ///Gets a vector representing one axis (i.e. one column) in the matrix.
        ///
        ///@param i The row to return.
        ///
        ///@return The vector.
        ///</summary>
        public Vector3 getAxisVector(int i)
        {
            return new Vector3(data[i], data[i + 3], data[i + 6]);
        }

        ///<summary>
        ///Sets the matrix to be the inverse of the given matrix.
        ///
        ///@param m The matrix to invert and use to set this.
        ///</summary>
        public void setInverse(Matrix3 m)
        {
            float t4 = m.data[0] * m.data[4];
            float t6 = m.data[0] * m.data[5];
            float t8 = m.data[1] * m.data[3];
            float t10 = m.data[2] * m.data[3];
            float t12 = m.data[1] * m.data[6];
            float t14 = m.data[2] * m.data[6];

            /// Calculate the determinant
            float t16 = (t4 * m.data[8] - t6 * m.data[7] - t8 * m.data[8] +
                        t10 * m.data[7] + t12 * m.data[5] - t14 * m.data[4]);

            /// Make sure the determinant is non-zero.
            if (t16 == (float)0.0f) return;
            float t17 = 1 / t16;

            data[0] = (m.data[4] * m.data[8] - m.data[5] * m.data[7]) * t17;
            data[1] = -(m.data[1] * m.data[8] - m.data[2] * m.data[7]) * t17;
            data[2] = (m.data[1] * m.data[5] - m.data[2] * m.data[4]) * t17;
            data[3] = -(m.data[3] * m.data[8] - m.data[5] * m.data[6]) * t17;
            data[4] = (m.data[0] * m.data[8] - t14) * t17;
            data[5] = -(t6 - t10) * t17;
            data[6] = (m.data[3] * m.data[7] - m.data[4] * m.data[6]) * t17;
            data[7] = -(m.data[0] * m.data[7] - t12) * t17;
            data[8] = (t4 - t8) * t17;
        }

        ///<summary> Returns a new matrix containing the inverse of this matrix. ///</summary>
        public Matrix3 inverse()
        {
            Matrix3 result = new Matrix3();
            result.setInverse(this);
            return result;
        }

        ///<summary>
        ///Inverts the matrix.
        ///</summary>
        public void invert()
        {
            setInverse(this);
        }

        ///<summary>
        ///Sets the matrix to be the transpose of the given matrix.
        ///
        ///@param m The matrix to transpose and use to set this.
        ///</summary>
        public void setTranspose(Matrix3 m)
        {
            data[0] = m.data[0];
            data[1] = m.data[3];
            data[2] = m.data[6];
            data[3] = m.data[1];
            data[4] = m.data[4];
            data[5] = m.data[7];
            data[6] = m.data[2];
            data[7] = m.data[5];
            data[8] = m.data[8];
        }

        ///<summary> Returns a new matrix containing the transpose of this matrix. ///</summary>
        public Matrix3 transpose()
        {
            Matrix3 result = new Matrix3();
            result.setTranspose(this);
            return result;
        }

        ///<summary> 
        ///Returns a matrix which is this matrix multiplied by the given 
        ///other matrix. 
        ///</summary>
        public static Matrix3 operator *(Matrix3 m, Matrix3 o)
        {
            return new Matrix3(
                m.data[0] * o.data[0] + m.data[1] * o.data[3] + m.data[2] * o.data[6],
                m.data[0] * o.data[1] + m.data[1] * o.data[4] + m.data[2] * o.data[7],
                m.data[0] * o.data[2] + m.data[1] * o.data[5] + m.data[2] * o.data[8],

                m.data[3] * o.data[0] + m.data[4] * o.data[3] + m.data[5] * o.data[6],
                m.data[3] * o.data[1] + m.data[4] * o.data[4] + m.data[5] * o.data[7],
                m.data[3] * o.data[2] + m.data[4] * o.data[5] + m.data[5] * o.data[8],

                m.data[6] * o.data[0] + m.data[7] * o.data[3] + m.data[8] * o.data[6],
                m.data[6] * o.data[1] + m.data[7] * o.data[4] + m.data[8] * o.data[7],
                m.data[6] * o.data[2] + m.data[7] * o.data[5] + m.data[8] * o.data[8]
                );
        }

        ///<summary>
        ///Sets this matrix to be the rotation matrix corresponding to 
        ///the given quaternion.
        ///</summary>
        void setOrientation(Quaternion q)
        {
            data[0] = 1 - (2 * q.Y * q.Y + 2 * q.Z * q.Z);
            data[1] = 2 * q.X * q.Y + 2 * q.Z * q.W;
            data[2] = 2 * q.X * q.Z - 2 * q.Y * q.W;
            data[3] = 2 * q.X * q.Y - 2 * q.Z * q.W;
            data[4] = 1 - (2 * q.X * q.X + 2 * q.Z * q.Z);
            data[5] = 2 * q.Y * q.Z + 2 * q.X * q.W;
            data[6] = 2 * q.X * q.Z + 2 * q.Y * q.W;
            data[7] = 2 * q.Y * q.Z - 2 * q.X * q.W;
            data[8] = 1 - (2 * q.X * q.X + 2 * q.Y * q.Y);

        }


        ///<summary> 
        ///Multiplies this matrix in place by the given scalar.
        ///</summary>
        public static Matrix3 operator *(Matrix3 m, float scalar)
        {
            Matrix3 result = new Matrix3(
                m.data[0] * scalar, m.data[1] * scalar, m.data[2] * scalar,
                m.data[3] * scalar, m.data[4] * scalar, m.data[5] * scalar,
                m.data[6] * scalar, m.data[7] * scalar, m.data[8] * scalar
            );
            return result;
        }

        ///<summary>
        ///Does a component-wise addition of this matrix and the given
        ///matrix.
        ///</summary>
        public static Matrix3 operator +(Matrix3 m, Matrix3 o)
        {
            Matrix3 result = new Matrix3(
                m.data[0] + o.data[0],
                m.data[1] + o.data[1],
                m.data[2] + o.data[2],
                m.data[3] + o.data[3],
                m.data[4] + o.data[4],
                m.data[5] + o.data[5],
                m.data[6] + o.data[6],
                m.data[7] + o.data[7],
                m.data[8] + o.data[8]
            );
            return result;
        }

        ///<summary>
        ///Interpolates a couple of matrices.
        ///</summary>
        static Matrix3 linearInterpolate(Matrix3 a, Matrix3 b, float prop)
        {
            Matrix3 result = new Matrix3();
            for (uint i = 0; i < 9; i++)
            {
                result.data[i] = a.data[i] * (1 - prop) + b.data[i] * prop;
            }
            return result;
        }
    };

}

