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

namespace PhysicsLab
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Lab : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera camera { get; protected set; }
        public BasicLab basicLab { get; set; }

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
            camera = new Camera(this, new Vector3(0, 0, 10f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            basicLab = new BasicLab(this);
            Components.Add(basicLab);

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
            Ball ball = new Ball(0.5f);
            ball.model = basicLab.BallModel;
            ball.Texture = basicLab.BallTexture;
            //ball.Mass = 5f;
            ball.InverseMass = 0;
            basicLab.AddBall(ball);

            Ball ball1 = new Ball(0.5f);
            ball1.model = basicLab.BallModel;
            ball1.Texture = basicLab.BallTexture;
            ball1.Mass = 1f;
            ball1.Position = new Vector3(1f, -2f, 0f);
            //ball1.InverseMass = 0;
            basicLab.AddBall(ball1);
            /*
            Crate crate = new Crate(new Vector3(0.5f, 0.2f, 0.3f));
            crate.Mass = 1f;
            crate.model = basicLab.CrateModel;
            crate.Position = new Vector3(1f, -2f, 0f);
            basicLab.AddCrate(crate);*/
            basicLab.AddEffect(new Spring(ball, ball1, ball1.Position, 1f, 0.2f, 0.995f));
            basicLab.AddEffect(new Gravity(new Vector3(0, -10f, 0)));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
