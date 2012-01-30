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
            Vector3 squaredSize = halfSize * halfSize;
            this.setInertiaTensorCoeffs(
                0.3f*Mass*(squaredSize.Y+squaredSize.Z),
                0.3f*Mass*(squaredSize.X + squaredSize.Z),
                0.3f*Mass*(squaredSize.X + squaredSize.Y));
        }

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(@"Box");
            Vector3 diag1 = (model.Bones["diag01"].Parent.Transform * model.Bones["diag01"].Transform).Translation;
            Vector3 diag2 = (model.Bones["diag02"].Parent.Transform * model.Bones["diag02"].Transform).Translation;
            HalfSize = (diag2 - diag1) / 2f;
            HalfSize = new Vector3(Math.Abs(HalfSize.X),Math.Abs(HalfSize.Y),Math.Abs(HalfSize.Z));
            //HalfSize = new Vector3(2,2,2);
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
                    be.World = mesh.ParentBone.Transform * TransformMatrix;
                    be.View = camera.view;
                    be.Projection = camera.projection;
                }
                mesh.Draw();
            }
        }
    }
}
