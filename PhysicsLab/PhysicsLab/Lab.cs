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
        private Panel bodyPanel = null;

        float timeOfGame = 0;
        float timeToCreate = 100f;

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
            basicLab = new BasicLab(this);
            Components.Add(basicLab);
            bodyPanel = new Panel(this, new 
                Vector2(Window.ClientBounds.Width - 255, 
                Window.ClientBounds.Height - 255), 
                250, 250);
            Components.Add(bodyPanel);
            camera = new Camera(this, new Vector3(0, 0, 20f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            this.IsMouseVisible = true;

            base.Initialize();
        }

        void CreateRoom()
        {
            Crate ground = new Crate(new Vector3(10f, 4f, 10f));
            ground.Position = new Vector3(0f, -9f, 0f);
            ground.model = basicLab.CrateModel;
            ground.Texture = Content.Load<Texture2D>(@"Textures\Ground");
            ground.SelectedTexture = null;
            ground.InverseMass = 0;
            ground.Lock();
            basicLab.AddToRoom(ground);

            Crate RightWall = new Crate(new Vector3(0.01f, 10f, 10f));
            RightWall.Position = new Vector3(10.01f, 5.01f, 0f);
            RightWall.model = basicLab.CrateModel;
            RightWall.Texture = Content.Load<Texture2D>(@"Textures\Wall");
            RightWall.SelectedTexture = null;
            RightWall.InverseMass = 0;
            RightWall.Lock();
            basicLab.AddToRoom(RightWall);

            Crate FrontWall = new Crate(new Vector3(10f, 10f, 0.01f));
            FrontWall.Position = new Vector3(0f, 5.01f, -10.01f);
            FrontWall.model = basicLab.CrateModel;
            FrontWall.Texture = Content.Load<Texture2D>(@"Textures\Wall");
            FrontWall.SelectedTexture = null;
            FrontWall.InverseMass = 0;
            FrontWall.Lock();
            basicLab.AddToRoom(FrontWall);

            Crate LeftWall = new Crate(new Vector3(0.01f, 10f, 10f));
            LeftWall.Position = new Vector3(-10.01f, 5.01f, 0f);
            LeftWall.model = basicLab.CrateModel;
            LeftWall.Texture = Content.Load<Texture2D>(@"Textures\Wall");
            LeftWall.SelectedTexture = null;
            LeftWall.InverseMass = 0;
            LeftWall.Lock();
            basicLab.AddToRoom(LeftWall);
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
            
            Ball ball2 = new Ball(0.5f);
            ball2.model = basicLab.BallModel;
            ball2.Texture = basicLab.BallTexture;
            ball2.Mass = 10f;
            ball2.Position = new Vector3(1f, 1f, 2f);
            ball2.SelectedTexture = basicLab.SelectedBallTexture;
            basicLab.AddBall(ball2);

            Ball ball3 = new Ball(0.2f);
            ball3.model = basicLab.BallModel;
            ball3.Texture = basicLab.BallTexture;
            ball3.Mass = 2f;
            ball3.Position = new Vector3(2f, 1f, 2f);
            ball3.SelectedTexture = basicLab.SelectedBallTexture;
            basicLab.AddBall(ball3);
            
            Crate crate = new Crate(new Vector3(0.5f, 0.2f, 0.3f));
            crate.Mass = 1f;
            crate.model = basicLab.CrateModel;
            crate.Texture = basicLab.CrateTexture;
            crate.SelectedTexture = basicLab.SelectedCrateTexture;
            crate.Position = new Vector3(1f, -2f, 1f);
            basicLab.AddCrate(crate);

            CreateRoom();

            basicLab.AddEffect(new Spring(ball, ball1, new Vector3(1, 1, 1), 10f, 2f, 0.995f));
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
        /// indicates weather the bodu=y is being moved by mouse or not
        /// </summary>
        bool bodyMoving = false;
        /// <summary>
        /// the current body under cursor (is moving by mouse)
        /// </summary>
        Body bdy = null;

        public void SelectedAndMoving(MouseState mouse, KeyboardState keyboard,
            Vector2 cursorPosition,Vector2 previousCursorPosition, float totalSecond)
        {
            
            var cursorDelta = (cursorPosition - previousCursorPosition) * totalSecond;
            var cameraDelta = (camera.cameraPosition - previousCameraPosition) * totalSecond;

            if (!bodySel)
            {
                Ray ray = camera.GetMouseRay(cursorPosition, GraphicsDevice.Viewport);
                if (!bodyMoving) bdy = basicLab.CheckIntersect(ray);
                if (bdy != null)
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                        bodyMoving = true;
                    else
                    {
                        bodyMoving = false;
                        bdy = null;
                    }

                    if (bodyMoving)
                    {
                        Vector3 tocentre = camera.cameraPosition - bdy.Position;
                        float dest = tocentre.Length();
                        cursorDelta *= dest / 10f;
                        cameraDelta *= dest / 10f;
                        ((Drawable)bdy).Selected = true;

                        if (keyboard.IsKeyDown(Keys.LeftControl))
                        {
                            bodySel = true;
                            prevBody = bdy;
                            ((IMoveable)bdy).Translate(Vector3.Backward, cameraDelta.Y + cursorDelta.Y);
                        }
                        else
                        {
                            ((IMoveable)bdy).Translate(Vector3.Right, cameraDelta.X + cursorDelta.X);
                            ((IMoveable)bdy).Translate(Vector3.Down, cameraDelta.Y + cursorDelta.Y);
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

                ((IMoveable)prevBody).Translate(Vector3.Backward, cameraDelta.Y + cursorDelta.Y);
                ((Drawable)prevBody).Selected = true;
            }
            if (keyboard.IsKeyUp(Keys.LeftControl)) bodySel = false;
        }


        public void PauseAndPlay(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Space))
                spaceClicked = true;
            if (keyboard.IsKeyUp(Keys.Space) && spaceClicked)
            {
                spaceClicked = false;
                basicLab.pause = !basicLab.pause;
            }
        }

        protected void CreateDialog(Panel pnl, Body body)
        {
            if (body as Ball != null)
            {
                Ball ball= (Ball)body;
                pnl.AddField("Mass", ball.Mass);
                pnl.AddField("Radius", ball.radius);
                pnl.AddLabel("Position");
                pnl.AddPosition(ball.Position);
                pnl.AddLabel("Velocity");
                pnl.AddPosition(ball.Velocity);
                pnl.AddLabel("Acceleration");
                pnl.AddPosition(ball.Acceleration);
                pnl.AddOkButton();
                pnl.AddCancelButton();
            }
        }

        void Crate(Vector2 cursorPosition, KeyboardState keyboard, float time)
        {
            if (keyboard.IsKeyDown(Keys.B))
            {
                timeOfGame += time;
                if (timeOfGame > timeToCreate)
                {
                    timeOfGame = 0;
                    Vector3 point = new Vector3(cursorPosition, 0.9f);
                    point = GraphicsDevice.Viewport.Unproject(point, camera.projection, camera.view, Matrix.Identity);
                    CreateDialog(bodyPanel, basicLab.CreateBall(point));
                    bodyPanel.Show = true;
                }
            }
            if (keyboard.IsKeyDown(Keys.C))
            {
                timeOfGame += time;
                if (timeOfGame > timeToCreate)
                {
                    timeOfGame = 0;
                    Vector3 point = new Vector3(cursorPosition, 0.9f);
                    point = GraphicsDevice.Viewport.Unproject(point, camera.projection, camera.view, Matrix.Identity);
                    basicLab.CreateCrate(point);
                }
            }
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

            PauseAndPlay(keyboard);
            Crate(cursorPosition, keyboard, gameTime.ElapsedGameTime.Milliseconds);
            SelectedAndMoving(mouse, keyboard, cursorPosition, previousCursorPosition, 
                (float)gameTime.ElapsedGameTime.TotalSeconds);

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
