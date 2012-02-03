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

        bool spaceClicked = false;
        MouseState oldMouseState = new MouseState();
        SpriteFont Font;

        public Camera camera { get; protected set; }
        Vector3 previousCameraPosition = Vector3.Zero;
        public BasicLab basicLab { get; set; }
        private Body prevBody = null;
        private bool bodySel = false;

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

            this.IsMouseVisible = true;

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
            Font = Content.Load<SpriteFont>(@"GUI\Arial");

            Ball ball = new Ball(0.5f);
            ball.Position = new Vector3(2f, 0f, 0f);
            ball.model = basicLab.BallModel;
            ball.Texture = basicLab.BallTexture;
            ball.SelectedTexture = basicLab.SelectedBallTexture;
            ball.Mass = 5f;
            //ball.InverseMass = 0;
            basicLab.AddBall(ball);

            Ball ball1 = new Ball(0.5f);
            ball1.model = basicLab.BallModel;
            ball1.Texture = basicLab.BallTexture;
            ball1.Mass = 1f;
            ball1.Position = new Vector3(0.1f, -2f, 0f);
            ball1.SelectedTexture = basicLab.SelectedBallTexture;
            ball1.InverseMass = 0;
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
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            Vector2 cursorPosition = new Vector2(mouse.X, mouse.Y);
            Vector2 previousCursorPosition = new Vector2(oldMouseState.X, oldMouseState.Y);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                spaceClicked = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && spaceClicked)
            {
                spaceClicked = false;
                basicLab.pause = !basicLab.pause;
            }

            Body bdy = null;
            var cursorDelta = (cursorPosition - previousCursorPosition) *  (float)gameTime.ElapsedGameTime.TotalSeconds;
            var cameraDelta = (camera.cameraPosition - previousCameraPosition) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!bodySel)
            {
                Ray ray = camera.GetMouseRay(cursorPosition, GraphicsDevice.Viewport);
                bdy = basicLab.CheckIntersect(ray);
                if (bdy != null)
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        Vector3 tocentre = camera.cameraPosition - bdy.Position;
                        float dest = tocentre.Length();
                        cursorDelta *= dest / 10f;
                        cameraDelta *= dest / 10f;

                        if (keyboard.IsKeyDown(Keys.LeftControl))
                        {
                            bodySel = true;
                            prevBody = bdy;
                            ((Ball)bdy).Translate(Vector3.Backward, cameraDelta.Y + cursorDelta.Y);
                        }
                        else
                        {
                            ((Ball)bdy).Translate(Vector3.Right, cameraDelta.X + cursorDelta.X);
                            ((Ball)bdy).Translate(Vector3.Down, cameraDelta.Y + cursorDelta.Y);
                        }
                    }
                }
            }
            else
            {
                Vector3 tocentre = camera.cameraPosition - prevBody.Position;
                float dest = tocentre.Length();
                cursorDelta *= dest / 10f;
                cameraDelta *= dest / 10f;

                ((Ball)prevBody).Translate(Vector3.Backward, cameraDelta.Y + cursorDelta.Y);
                ((Ball)prevBody).Selected = true;
            }
            if (keyboard.IsKeyUp(Keys.LeftControl)) bodySel = false;

            oldMouseState = mouse;
            previousCameraPosition = camera.cameraPosition;

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
