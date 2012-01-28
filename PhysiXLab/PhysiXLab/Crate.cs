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
namespace Test
{
    public class Crate : Box
    {
        public Model model {private set; get; }

        public Crate(Vector3 halfSize)
            : base(halfSize)
        {
            this.Mass = 10f;
        }

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(@"Box");
            Vector3 diag1 = model.Bones["diag01"].Transform.Translation;
            Vector3 diag2 = model.Bones["diag02"].Transform.Translation;
            HalfSize = (diag2 - diag1) / 2f;
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
                    be.EnableDefaultLighting();
                    be.World = mesh.ParentBone.Transform /* Matrix.CreateScale(HalfSize.Length()) */ * TransformMatrix;
                    be.View = camera.view;
                    be.Projection = camera.projection;
                }
                mesh.Draw();
            }
        }
    }
}
