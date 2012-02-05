using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysiXEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsLab
{
    public class Ball : Sphere, Drawable, IMoveable
    {
        public Model model { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D SelectedTexture { get; set; }
        public Texture2D SelectedTexture_Panel { get; set; }
        public bool Selected { get; set; }
        public bool ShowPanel { get; set; }

        public Ball(float radius) 
            : base(radius)
        {}

        public Ball(Model model, Texture2D Texture, string S)
            : base(S)
        {
            this.Texture = Texture;
            this.model = model;
        }

        public override string ToString()
        {
            return "Ball|" + base.ToString();
        }

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
                        }
                        be.Alpha = 3;
                        Selected = false;
                    }
                    if (ShowPanel)
                    {
                        if (SelectedTexture_Panel != null)
                        {
                            be.Texture = this.SelectedTexture_Panel;
                            be.TextureEnabled = true;
                        }
                        be.Alpha = 3;
                    }
                }
                mesh.Draw();
            }
        }

        public void Translate(Vector3 axis, float distance)
        {
            Position += axis * distance;
        }
    }
}
