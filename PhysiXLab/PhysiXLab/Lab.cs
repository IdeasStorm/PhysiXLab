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

namespace PhysiXLab
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Lab : Microsoft.Xna.Framework.Game
    {
        bool paused = true;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LinkedList<Ball> balls = new LinkedList<Ball>();
        Ball ball;
        Ball dummy;
        Gravity g;
        public Camera camera { get; protected set; }

        public Lab()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(this, new Vector3(0, 0, 100),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            ball = new Ball(10f);
            dummy = new Ball(10f);
            g = new Gravity(Vector3.Down * 0.01f);
            g.AddBody(ball);
            //TODO determine which is down for the world
            //BoundingSphereRenderer.InitializeGraphics(GraphicsDevice, 10^100);
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ball.model = Content.Load<Model>(@"Ball");
            dummy.model = ball.model;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        bool spaceDown;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.P))
                paused = !paused;
            if (!paused)
            {
                                    
                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Space))
                {
                    spaceDown = true;
                }
                if (spaceDown && (Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Space))) 
                {
                    spaceDown = false;
                    Ball b = new Ball(10f);
                    b.model = ball.model;
                    g.AddBody(b);
                    balls.AddLast(b);
                    b.AddForce(new Vector3(2f, 1f, 0));
                }
                g.Update(gameTime.ElapsedGameTime.Milliseconds);

                // TODO: Add your update logic here
                ball.Update(gameTime.ElapsedGameTime.Milliseconds);
                foreach (Ball b in balls)
                    b.Update(gameTime.ElapsedGameTime.Milliseconds);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            foreach (Ball b in balls)
            {
                b.Draw(camera);
            }
            dummy.Draw(camera);
            ball.Draw(camera);
            //sphere.Draw(camera);
            //BoundingSphereRenderer.Render(new BoundingSphere(Vector3.Zero, 10f), GraphicsDevice,
            //    camera.view, camera.projection, Color.Red);
            base.Draw(gameTime);
        }
    }
}
