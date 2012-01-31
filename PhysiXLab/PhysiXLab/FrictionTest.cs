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
    public class FrictionTest : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        Ball fixedBall;
        Ball ball1;
        //        Crate crate;
        //        Crate crate2;
        Ball ball2;
        Ball ball3;
        LinkedList<Ball> balls = new LinkedList<Ball>();
        ContactGenerator cg;
        Camera camera;
        Gravity g;
        #endregion

        public FrictionTest()
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
            fixedBall = new Ball(100f);
            fixedBall.Texture = Content.Load<Texture2D>(@"basic_material");
            fixedBall.Position = new Vector3(100, 0, 20);

            ball1 = new Ball(10f);
            ball1.Texture = Content.Load<Texture2D>(@"basic_material");
            ball1.Position = new Vector3(100, 150, 0);

            ball2 = new Ball(11f);
            ball2.Texture = Content.Load<Texture2D>(@"basic_material");
            ball2.Position = new Vector3(130, 180, 0);

            ball3 = new Ball(12f);
            ball3.Texture = Content.Load<Texture2D>(@"basic_material");
            ball3.Position = new Vector3(130, 220, 0);

            //crate = new Crate(new Vector3(2,2,2));
            //crate.Position = new Vector3(50,150,20);

            //crate2 = new Crate(new Vector3(2, 2, 2));
            //crate2.Position = new Vector3(100, 200, 0);


            cg = new ContactGenerator();
            cg.AddBody(fixedBall);
            cg.AddBody(ball1);
            cg.AddBody(ball2);
            cg.AddBody(ball3);

            //cg.AddBody(crate);
            //cg.AddBody(crate2);

            fixedBall.model = Content.Load<Model>(@"ball");
            ball1.model = fixedBall.model;
            ball2.model = fixedBall.model;
            ball3.model = fixedBall.model;
            camera = new Camera(this, new Vector3(0, 0, 100),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            g = new Gravity(new Vector3(0f, -1f, 0f));

            fixedBall.InverseMass = 0;
            fixedBall.InverseInertiaTensor = new Matrix();
            ball1.Mass = 10;
            ball2.Mass = 10;
            ball3.Mass = 10;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //crate.LoadContent(Content);
            //crate2.LoadContent(Content);
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
                //g.AddBody(ball1);
                g.AddBody(ball1);
                //                g.AddBody(ball2);
                g.AddBody(ball3);
                //g.AddBody(crate);
                //g.AddBody(crate2);
            }

            g.Update(duration);
            fixedBall.Update(duration);
            ball1.Update(duration);
            ball2.Update(duration);
            ball3.Update(duration);
            //crate.Update(duration);
            //crate2.Update(duration);
            cg.Update(duration);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            fixedBall.Draw(camera);
            ball1.Draw(camera);
            ball2.Draw(camera);
            ball3.Draw(camera);
            //crate.Draw(camera);
            //crate2.Draw(camera);
            base.Draw(gameTime);
        }
    }
}


/* OLD CODE:

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
    public class FrictionTest : Microsoft.Xna.Framework.Game
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
        #endregion

        public FrictionTest()
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
            fixedBall = new Ball(1f);
            fixedBall.Texture = Content.Load<Texture2D>(@"basic_material");          
            fixedBall.Position = new Vector3(1, 0, 0.2f);
            
            ball = new Ball(0.3f);
            ball.Texture = Content.Load<Texture2D>(@"basic_material");
            ball.Position = new Vector3(0.5f,2f, 0);

            crate = new Crate(new Vector3(0.07f,0.05f,0.05f));
            crate.Position = new Vector3(0.3f,5f,0.20f);
            crate.Mass = 10;

            cg = new ContactGenerator();
            cg.AddBody(fixedBall);
            //cg.AddBody(ball);
            cg.AddBody(crate);

            fixedBall.model = Content.Load<Model>(@"ball");
            ball.model = fixedBall.model;
            camera = new Camera(this, new Vector3(0, 0, 0.1f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            g =new Gravity( new Vector3(0f, -10f, 0f));

            fixedBall.InverseMass = 0;
            fixedBall.InverseInertiaTensor = new Matrix();
            ball.Mass = 50;

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
                //g.AddBody(ball);
                g.AddBody(crate);
            }

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
*/