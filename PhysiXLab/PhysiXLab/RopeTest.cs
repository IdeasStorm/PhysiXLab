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
    class RopeTest : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        Ball fixedBall;
        Ball ball;
        ContactGenerator cg;
        Camera camera;
        Gravity g;
        Rope rope;
        #endregion

        public RopeTest()
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
            fixedBall = new Ball(0.1f);
            fixedBall.model = Content.Load<Model>(@"ball");
            fixedBall.Texture = Content.Load<Texture2D>(@"basic_material");
            fixedBall.Position = new Vector3(0, 4, 0);

            fixedBall.InverseMass = 0;
            fixedBall.InverseInertiaTensor = new Matrix();

            ball = new Ball(0.1f);
            ball.model = Content.Load<Model>(@"ball");

            ball.Texture = Content.Load<Texture2D>(@"basic_material");
            ball.Position = new Vector3(0, -4, 0);

            ball.Mass = 1;
            ball.InverseInertiaTensor = new Matrix();

            cg = new ContactGenerator();
            
            
            g = new Gravity(new Vector3(0f, -5f, 0f));

            rope = new Rope(4, cg, this, g);
            rope.AddUpperBody(fixedBall);
            rope.AddBottomBody(ball);

            cg.AddBody(fixedBall);
            cg.AddBody(ball);

            camera = new Camera(this, new Vector3(0, 0, 0.1f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        bool spaceClicked;
        Vector3 temp = Vector3.Zero;
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

            if(Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                ball.AddForce(Vector3.Up * -10);
            }

            if(Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                ball.AddForce(Vector3.Up * 10);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
               ball.AddForce(Vector3.UnitZ * -10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
               ball.AddForce(Vector3.UnitZ * 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
               ball.AddForce(Vector3.Left * 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
               ball.AddForce(Vector3.Right * 10);
            }
                        
            g.Update(duration);

            rope.update(duration);
            fixedBall.Update(duration);
            ball.Update(duration);
            
            cg.Update(duration);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            rope.Draw(camera);
            ball.Draw(camera);
            fixedBall.Draw(camera);
            base.Draw(gameTime);
        }
    }
}
