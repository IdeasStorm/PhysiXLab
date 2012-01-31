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
    class RodAndCableTest : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        Ball fixedBall;
        Ball ball;
        ContactGenerator cg;
        Camera camera;
        Gravity g;
        #endregion

        public RodAndCableTest()
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
            fixedBall = new Ball(1);
            fixedBall.model = Content.Load<Model>(@"ball");
            
            fixedBall.Texture = Content.Load<Texture2D>(@"basic_material");
            fixedBall.Position = new Vector3(0, 4, 0);
            
            ball = new Ball(1);
            ball.Texture = Content.Load<Texture2D>(@"basic_material");
            ball.Position = new Vector3(0, 0, 0);
            ball.model = fixedBall.model;

            ball.Mass = 10;

            cg = new ContactGenerator();
            cg.AddBody(fixedBall);
            cg.AddBody(ball);
            cg.AddConductor(new Cable(fixedBall, ball, 10, 0.5f));
            camera = new Camera(this, new Vector3(0, 0, 0.1f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            g = new Gravity(new Vector3(0f, -10f, 0f));

            fixedBall.InverseMass = 0;
            fixedBall.InverseInertiaTensor = new Matrix();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        bool spaceClicked;

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float duration = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                spaceClicked = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && spaceClicked)
            {
                spaceClicked = false;
                g.AddBody(ball);
            }

            g.Update(duration);
            fixedBall.Update(duration);
            ball.Update(duration);
            cg.Update(duration);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            fixedBall.Draw(camera);
            ball.Draw(camera);
            base.Draw(gameTime);
        }
    }
}
