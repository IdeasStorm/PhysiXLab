using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysiXEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsLab
{
    public class Ball : Sphere, Drawable
    {
        public Model model { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D SelectedTexture { get; set; }
        public bool Selected = false;

        public Ball(float radius) 
            : base(radius)
        {}

        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    if (Texture != null)
                    {
                        be.Texture = this.Texture;
                        be.TextureEnabled = true;
                    }
                    be.EnableDefaultLighting();
                    be.World = mesh.ParentBone.Transform * Matrix.CreateScale(radius) * TransformMatrix;
                    be.View = camera.view;
                    be.Projection = camera.projection;
                    be.Alpha = 1f;
                    if (Selected)
                    {
                        if (SelectedTexture != null)
                        {
                            be.Texture = this.SelectedTexture;
                            be.TextureEnabled = true;
                            be.Alpha = 3;
                        }
                        Selected = false;
                    }
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// Pans the entity along the X, Y, and Z directions.
        /// </summary>
        /// <param name="axis">The direction to pan the entity.</param>
        /// <param name="distance">The amount of X, Y, and Z distance to pan the entity. Units are assumed to be world units.</param>
        public void Translate(Vector3 axis, float distance)
        {
            Position += axis * distance;
        }
    }
}
