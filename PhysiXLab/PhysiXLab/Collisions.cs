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
    public class Collisions : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        Ball b1;
        LinkedList<Ball> balls = new LinkedList<Ball>();
        ContactGenerator cg;
        Camera camera;
        #endregion

        public Collisions()
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
            b1 = new Ball(10f);
            b1.Texture = Content.Load<Texture2D>(@"basic_material");
            cg = new ContactGenerator();
            b1.Position = new Vector3(100,0,0);            
            cg.AddBody(b1);
            b1.model = Content.Load<Model>(@"ball");
            camera = new Camera(this, new Vector3(0, 0, 100),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            base.Initialize();
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
                Ball b = new Ball(10f);
                b.model = b1.model;
                balls.AddLast(b);
                cg.AddBody(b);
                b.AddForce(new Vector3(400, 20, 0));
            }
            cg.Update(duration);
            b1.Update(duration);
            foreach (Ball b in balls)
                b.Update(duration);
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            b1.Draw(camera);
            foreach (Ball b in balls)
                b.Draw(camera);

            base.Draw(gameTime);
        }
    }
}