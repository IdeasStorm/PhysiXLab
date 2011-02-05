using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysiXEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace PhysicsLab
{
    public class Crate : Box, Drawable, IMoveable
    {
        public Model model { set; get; }
        public Texture2D Texture { get; set; }
        public Texture2D SelectedTexture { get; set; }
        public Texture2D SelectedTexture_Panel { get; set; }
        public bool Selected { get; set; }
        public bool ShowPanel { get; set; }

        private Vector3 modelSize;
        public Crate(Vector3 halfSize)
            : base(halfSize)
        {
            // TODO Add Mass Change Code
        }

        public Crate(Model model, Texture2D Texture, string S)
            : base(S)
        {
            this.Texture = Texture;
            this.model = model;
            Vector3 squaredSize = HalfSize * HalfSize;
            this.setInertiaTensorCoeffs(
                0.3f * Mass * (squaredSize.Y + squaredSize.Z),
                0.3f * Mass * (squaredSize.X + squaredSize.Z),
                0.3f * Mass * (squaredSize.X + squaredSize.Y));
        }

        public override string ToString()
        {
            return "Crate|" + base.ToString();
        }

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(@"Box");
            Vector3 modelHalfSize = (model.Bones["halfsize"].Parent.Transform * model.Bones["halfsize"].Transform).Translation;
            updateBounding();
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
                    be.World = mesh.ParentBone.Transform *  Matrix.CreateScale(HalfSize) * TransformMatrix;
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
