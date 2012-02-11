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
    class EnergyConservation : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        Ball[] balls;
        Ball[] fixedBalls;
        ContactGenerator cg;
        Camera camera;
        Gravity g;
        uint numberOfballs = 4;
        #endregion

        public EnergyConservation()
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
            fixedBalls = new Ball[numberOfballs];
            Model mod = Content.Load<Model>(@"ball");
            Texture2D tex = Content.Load<Texture2D>(@"basic_material");

            g = new Gravity(Vector3.Down * 10);

            for (int i = 0; i < numberOfballs; i++)
            {
                balls[i] = new Ball(0.249f);
                fixedBalls[i] = new Ball(0.249f);

                balls[i].model = mod;
                fixedBalls[i].model = mod;

                balls[i].Mass = 1;
                fixedBalls[i].InverseMass = 0;
                fixedBalls[i].InverseInertiaTensor = new Matrix();
                balls[i].InverseInertiaTensor = new Matrix();


                balls[i].Texture = tex;
                fixedBalls[i].Texture = tex;
            }

            balls[0].Position = new Vector3(-0.25f, 0, 0);
            balls[1].Position = new Vector3(0.50f, 0, 0);
            balls[2].Position = new Vector3(1.00f, 0, 0);
            balls[3].Position = new Vector3(1.50f, 0, 0);


            fixedBalls[0].Position = new Vector3(0, 1f, 0);
            fixedBalls[1].Position = new Vector3(0.50f, 1f, 0);
            fixedBalls[2].Position = new Vector3(1.00f, 1f, 0);
            fixedBalls[3].Position = new Vector3(1.50f, 1f, 0);


            cg = new ContactGenerator();

            for (int i = 0; i < numberOfballs; i++)
            {
                cg.AddBody(balls[i]);
                cg.AddBody(fixedBalls[i]);
                cg.AddConductor(new Rod(fixedBalls[i], balls[i], 3));

            }

            //cg.velocityIterations = numberOfballs + 2;
            //cg.friction = 0.0001f;
            //cg.restitution = 1;

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
                choice = 1;
            if (keyState.IsKeyDown(Keys.D2))
                choice = 2;
            if (keyState.IsKeyDown(Keys.D3))
                choice = 3;
            if (keyState.IsKeyDown(Keys.D4))
                choice = 4;
            if (keyState.IsKeyDown(Keys.D5))
                choice = 5;
            if (keyState.IsKeyDown(Keys.D6))
                choice = 6;
            if (keyState.IsKeyDown(Keys.D7))
                choice = 7;
            if (keyState.IsKeyDown(Keys.D8))
                choice = 8;
            if (keyState.IsKeyDown(Keys.D9))
                choice = 9;
            if (keyState.IsKeyDown(Keys.D0))
                choice = 0;
            if (keyState.IsKeyDown(Keys.Space))
                grav = true;
            #endregion


            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                balls[choice].AddForce(Vector3.Left * 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                balls[choice].AddForce(Vector3.Right * 10);
            }
            if (grav)
                for (int i = 0; i < numberOfballs; i++)
                    balls[i].AddForce(Vector3.Down * 10);

            g.Update(duration);
            for (int i = 0; i < numberOfballs; i++)
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
            for (int i = 0; i < numberOfballs; i++)
            {
                balls[i].Draw(camera);
                fixedBalls[i].Draw(camera);
            }
            base.Draw(gameTime);
        }
    }
}
