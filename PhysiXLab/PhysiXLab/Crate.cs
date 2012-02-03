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
        private Vector3 modelSize;
        public Crate(Vector3 halfSize)
            : base(halfSize)
        {
            Vector3 squaredSize = halfSize * halfSize;
            this.setInertiaTensorCoeffs(
                0.3f*Mass*(squaredSize.Y+squaredSize.Z),
                0.3f*Mass*(squaredSize.X + squaredSize.Z),
                0.3f*Mass*(squaredSize.X + squaredSize.Y));
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
                    be.EnableDefaultLighting();
                    be.World = mesh.ParentBone.Transform *  Matrix.CreateScale(HalfSize) * TransformMatrix;
                    be.View = camera.view;
                    be.Projection = camera.projection;
                }
                mesh.Draw();
            }
        }
    }
}
