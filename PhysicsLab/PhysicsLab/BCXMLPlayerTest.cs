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
using PhysicsLab;
using PhysiXEngine;
using PhysiXEngine.Helpers;

namespace PhysicsLab
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BCXMLPlayerTest : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Body> Bodies = new List<Body>(10);
        XMLRecorder recorder;
        Model BallModel;
        Model CrateModel;
        Camera camera;
        Texture2D BallTexture;
        Texture2D CrateTexture;

        public BCXMLPlayerTest()
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
            for (int i = 0; i < 10; i++)
            {
                Ball b = new Ball((float)(1 + (i * 0.1)));
                b.Mass = 10 * i;
                b.Position = new Vector3(50, 40f + 5 * i, 50f);
                Bodies.Add(b);
            }

            camera = new Camera(this, new Vector3(0, 0, 0.1f), Vector3.Zero, Vector3.Up);
            Components.Add(camera);

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

            BallModel = this.Content.Load<Model>(@"Models\ball");
            BallTexture = this.Content.Load<Texture2D>(@"Textures\texBall");

            CrateModel = this.Content.Load<Model>(@"Models\box");
            CrateTexture = this.Content.Load<Texture2D>(@"Textures\texBox");
            recorder = new XMLRecorder(Bodies);
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
