using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Test
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RayIndicator : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public RayIndicator(Game game, Vector3 position, Vector3 Value)
            : base(game)
        {
            // TODO: Construct any child components here
            this.position = position;
            this.value = value;
            direction = Vector3.Normalize(value);
        }

        //public Matrix world { protected set; get; }
        public Vector3 position { protected set; get; }
        public Vector3 value { protected set; get; }
        private Model model { protected set; get; }
        private Vector3 direction;
        private Matrix scale;
        private Matrix rotation;
        private Matrix[] baseMatrix;

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>(@"models\Pointer");
            baseMatrix = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(baseMatrix);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            scale = Matrix.CreateScale(value.X, value.Y, value.Z);
            rotation = Matrix.CreateFromYawPitchRoll(direction.X, direction.Y, 
                direction.Z);
            base.Update(gameTime);
        }

        public void Draw(Camera camera, Model baseModel)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    if (mesh.Name == "Box01")
                    {
                        be.World = baseMatrix[mesh.ParentBone.Index] *
                            (Matrix.CreateTranslation(position) * scale) * rotation;
                        //world = world * scale;
                        //world = Matrix.Transform(world,scale);
                        //world = Vector3.Transform(new Vector3(2f, 0f, 0f), world);
                    }
                    else
                        //TODO set solution here
                        be.World = baseMatrix[mesh.ParentBone.Index] * Matrix.CreateTranslation(position) * 
                            scale * rotation;
                    be.View = camera.view;
                    be.Projection = camera.projection;
                }
                mesh.Draw();
            }
        }
    }
}
