using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysiXEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Test
{
    class Ball : Sphere
    {
        public Model model { get; set; }

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
                    be.EnableDefaultLighting();
                    be.World = mesh.ParentBone.Transform * Matrix.CreateScale(radius) * TransformMatrix;
                    be.View = camera.view;
                    be.Projection = camera.projection;
                }
                mesh.Draw();
            }
        }
    }
}
