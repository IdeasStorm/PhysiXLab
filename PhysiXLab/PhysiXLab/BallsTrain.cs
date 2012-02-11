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
    class BallsTrain : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        Ball fixedBall;
        Ball[] balls;
        ContactGenerator cg;
        Camera camera;
        Gravity g;
        uint numberOfballs = 4;
        #endregion

        public BallsTrain()
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
            balls = new Ball[numberOfballs];
            fixedBall = new Ball(0.5f);
            Model mod = Content.Load<Model>(@"ball");
            Texture2D tex = Content.Load<Texture2D>(@"basic_material");

            g = new Gravity(Vector3.Down * 1);
            fixedBall.InverseMass = 0;
            fixedBall.InverseInertiaTensor = new Matrix();

            fixedBall.model = mod;
           

            for (int i = 0; i < numberOfballs; i++)
            {
                balls[i] = new Ball(0.1f);
                

                balls[i].model = mod;
                

                balls[i].Mass = 10;
                
                balls[i].InverseInertiaTensor = new Matrix();


                balls[i].Texture = tex;
                fixedBall.Texture = tex;
            }

            balls[0].Position = new Vector3(-0.25f, -1, 0);
            balls[1].Position = new Vector3(0.50f, -1, 0);
            balls[2].Position = new Vector3(1.00f, -1, 0);
            balls[3].Position = new Vector3(1.50f, -1, 0);


            fixedBall.Position = new Vector3(0, 0f, 0);
            


            cg = new ContactGenerator();

            for (int i = 0; i < numberOfballs; i++)
            {
                cg.AddBody(balls[i]);
                //cg.AddBody(fixedBall);
                cg.AddConductor(new Rod(fixedBall, balls[i], 2));

            }

            //cg.velocityIterations = numberOfballs ;
            //cg.positionIterations = numberOfballs*10 ;
            //cg.friction = .1f;
            //cg.restitution = 0.7f;

            camera = new Camera(this, new Vector3(0, 0, 0.1f), Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        int choice = 0;
        bool grav = false;
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float duration = gameTime.ElapsedGameTime.Milliseconds / 1000f;

            #region enter valu of chice
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.D1))
                choice = 0;
            if (keyState.IsKeyDown(Keys.D2))
                choice = 1;
            if (keyState.IsKeyDown(Keys.D3))
                choice = 2;
            if (keyState.IsKeyDown(Keys.D4))
                choice = 3;
            if (keyState.IsKeyDown(Keys.Space))
                grav = true;
            #endregion

            if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
                balls[choice].AddForce(Vector3.UnitZ * -10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                balls[choice].AddForce(Vector3.UnitZ * 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                balls[choice].AddForce(Vector3.Left * 100);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                balls[choice].AddForce(Vector3.Right * 100);
            }
            if (grav)
                for (int i = 0; i < numberOfballs; i++)
                    balls[i].AddForce(Vector3.Down * 10);

            g.Update(duration);
            for (int i = 0; i < numberOfballs; i++)
            {
                fixedBall.Update(duration);
                balls[i].Update(duration);
            }
            cg.Update(duration);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            for (int i = 0; i < numberOfballs; i++)
            {
                balls[i].Draw(camera);
                fixedBall.Draw(camera);
            }
            base.Draw(gameTime);
        }
    }
}
