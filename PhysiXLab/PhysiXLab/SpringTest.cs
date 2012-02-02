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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpringTest : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        Ball fixedBall;
        Ball ball;
        Crate crate;
        LinkedList<Ball> balls = new LinkedList<Ball>();
        ContactGenerator cg;
        Camera camera;
        Gravity g;
        Spring sp;
        #endregion

        public SpringTest()
        {
            // TODO: Construct any child components here
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        protected override void Initialize()
        {
            fixedBall = new Ball(1f);
            fixedBall.Texture = Content.Load<Texture2D>(@"basic_material");
            fixedBall.Position = new Vector3(0f, 0, 0f);

            ball = new Ball(0.3f);
            ball.Texture = Content.Load<Texture2D>(@"basic_material");
            ball.Position = new Vector3(0f, -2f, 0f);

            crate = new Crate(new Vector3(0.07f, 0.05f, 0.05f));
            crate.Position = new Vector3(0.3f, 5f, 0.20f);
            crate.Mass = 10;

            cg = new ContactGenerator();
            cg.AddBody(fixedBall);
            cg.AddBody(ball);
            cg.AddBody(crate);

            fixedBall.model = Content.Load<Model>(@"ball");
            ball.model = fixedBall.model;
            camera = new Camera(this, new Vector3(0, 0, 10f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            g = new Gravity(new Vector3(0f, -10f, 0f));

            fixedBall.InverseMass = 0;
            fixedBall.InverseInertiaTensor = new Matrix();
            ball.Mass = 1f;
            sp = new Spring(fixedBall, ball, ball.Position, 3f, 2f, 0.995f);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            crate.LoadContent(Content);
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
                //g.AddBody(crate);
                
            }
            g.AddBody(ball);
            sp.Update(duration);
            g.Update(duration);
            fixedBall.Update(duration);
            ball.Update(duration);
            crate.Update(duration);
            cg.Update(duration);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            fixedBall.Draw(camera);
            ball.Draw(camera);
            crate.Draw(camera);
            base.Draw(gameTime);
        }
    }
}