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
using PhysiXEngine;

namespace Test
{
    class Bridge : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        Ball [] balls;
        Ball[] fixedBalls;
        ContactGenerator cg;
        Camera camera;
        Gravity g;
        #endregion

        public Bridge()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        protected override void Initialize()
        {
            balls = new Ball[10];
            fixedBalls = new Ball[10];
            Model mod = Content.Load<Model>(@"ball");
            Texture2D tex = Content.Load<Texture2D>(@"basic_material");

            g = new Gravity(Vector3.Down * 10);
            
            for (int i = 0; i < 10; i++)
            {
                balls[i] = new Ball(0.5f);
                fixedBalls[i] = new Ball(0.1f);
                balls[i].model = mod;
               
                balls[i].Mass = 10;
                fixedBalls[i].InverseMass = 0;
                fixedBalls[i].InverseInertiaTensor = new Matrix();
                g.AddBody(balls[i]);

                balls[i].Texture = tex;
                fixedBalls[i].Texture = tex;
            }

            balls[0].Position = new Vector3(-5, 0, 0);
            balls[1].Position = new Vector3(-3, 0, 0);
            balls[2].Position = new Vector3(0, 0, 0);
            balls[3].Position = new Vector3(3, 0, 0);
            balls[4].Position = new Vector3(5, 0, 0);
            balls[5].Position = new Vector3(-5, 0, 3);
            balls[6].Position = new Vector3(-3, 0, 3);
            balls[7].Position = new Vector3(0, 0, 3);
            balls[8].Position = new Vector3(3, 0, 3);
            balls[9].Position = new Vector3(5, 0, 3);

            fixedBalls[0].Position = new Vector3(-5, 0, 0);
            fixedBalls[1].Position = new Vector3(-3, 0, 0);
            fixedBalls[2].Position = new Vector3(0, 0, 0);
            fixedBalls[3].Position = new Vector3(3, 0, 0);
            fixedBalls[4].Position = new Vector3(5, 0, 0);
            fixedBalls[5].Position = new Vector3(-5, 0, 3);
            fixedBalls[6].Position = new Vector3(-3, 0, 3);
            fixedBalls[7].Position = new Vector3(0, 0, 3);
            fixedBalls[8].Position = new Vector3(3, 0, 3);
            fixedBalls[9].Position = new Vector3(5, 0, 3);

            cg = new ContactGenerator();
            
            for (int i = 0; i < 10; i++)
            {
                cg.AddBody(balls[i]);
                cg.AddBody(fixedBalls[i]);
                cg.AddConductor(new Cable(fixedBalls[i], balls[i], 2, 0.7f));
            }
          
            camera = new Camera(this, new Vector3(0, 0, 0.1f),Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float duration = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            
            g.Update(duration);
            for (int i = 0; i < 10; i++)
            {
                fixedBalls[i].Update(duration);
                balls[i].Update(duration);
            }
            cg.Update(duration);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            for (int i = 0; i < 10; i++)
            {
                balls[i].Draw(camera);
            }
            base.Draw(gameTime);
        }
    }
}
