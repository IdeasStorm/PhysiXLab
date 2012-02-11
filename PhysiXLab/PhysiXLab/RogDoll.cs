//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using PhysiXEngine;

//namespace Test
//{
//    class RogDoll : Microsoft.Xna.Framework.Game
//    {
//        GraphicsDeviceManager graphics;

//        #region "testing components"
//        int NUM_CRATES=12;
//        int NUM_JOINTS=11;

//        Crate[] crates;
//        Joint[] joints;

//        ContactGenerator cg;
//        Camera camera;
//        Gravity g;
//        #endregion

//        public RogDoll()
//        {
//            graphics = new GraphicsDeviceManager(this);
//            Content.RootDirectory = "Content";
//        }

//        /// <summary>
//        /// Allows the game component to perform any initialization it needs to before starting
//        /// to run.  This is where it can query for any required services and load content.
//        /// </summary>
//        protected override void Initialize()
//        {
//            crates = new Crate[NUM_CRATES];

//            crates[0]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[1]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[2]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[3]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[4]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[5]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[6]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[7]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[8]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[9]  = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[10] = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crates[11] = new Crate(new Vector3(0.1f, 0.1f, 0.1f));


//            for (int i = 0; i < 12; i++)
//            {
//                crates = new Crate();
//            }

//            fixedCrate = new Crate();
//            fixedCrate.Position = new Vector3(0f, 0f, 0f);
//            fixedCrate.Mass = 1;

//            crate = new Crate(new Vector3(0.1f, 0.1f, 0.1f));
//            crate.Position = new Vector3(-0f, -1f, -0f);
//            crate.Mass = 1;

//            cg = new ContactGenerator();
//            //cg.AddBody(fixedCrate);
//            //cg.AddBody(crate);

//            cg.AddJoint(new PhysiXEngine.Joint(fixedCrate, crate, new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f), 1f));

//            camera = new Camera(this, new Vector3(0, 0, 0.1f),
//                Vector3.Zero, Vector3.Up);
//            Components.Add(camera);
//            g = new Gravity(new Vector3(0f, -10f, 0f));

//            fixedCrate.InverseMass = 0;
//            fixedCrate.InverseInertiaTensor = new Matrix();
//            //crate.InverseInertiaTensor = new Matrix();
//            base.Initialize();
//        }

//        protected override void LoadContent()
//        {
//            crate.LoadContent(Content);
//            fixedCrate.LoadContent(Content);
//            base.LoadContent();
//        }

//        bool spaceClicked;
//        Vector3 temp = Vector3.Zero;
//        /// <summary>
//        /// Allows the game component to update itself.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        protected override void Update(GameTime gameTime)
//        {
//            float duration = gameTime.ElapsedGameTime.Milliseconds / 1000f;
//            if (Keyboard.GetState().IsKeyDown(Keys.Space))
//                spaceClicked = true;
//            if (Keyboard.GetState().IsKeyUp(Keys.Space) && spaceClicked)
//            {
//                crate.AddForce(Vector3.Down * 10);
//            }
//            if (Keyboard.GetState().IsKeyDown(Keys.O))
//            {
//                crate.AddForce(Vector3.Up * 100);
//            }

//            g.Update(duration);
//            fixedCrate.Update(duration);
//            crate.Update(duration);
//            cg.Update(duration);

//            base.Update(gameTime);
//        }

//        protected override void Draw(GameTime gameTime)
//        {
//            GraphicsDevice.Clear(Color.CornflowerBlue);
//            fixedCrate.Draw(camera);
//            crate.Draw(camera);

//            base.Draw(gameTime);
//        }
//    }
//}
