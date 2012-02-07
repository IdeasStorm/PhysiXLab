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
using PhysiXEngine.Helpers;

namespace PhysicsLab
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Lab : Microsoft.Xna.Framework.Game
    {
        #region "Main Component"
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        #endregion

        #region "XML Recorder"
        XMLRecorder Recorder;
        #endregion

        #region "Boolean Field"
        bool spaceClicked = false;
        bool bClicked = false;
        bool cClicked = false;
        bool pClicked = false;
        #endregion

        #region "Previous State"
        MouseState oldMouseState = new MouseState();
        Vector3 previousCameraPosition = Vector3.Zero;
        private Body prevBody = null;
        #endregion

        #region "Game Component"
        public Camera camera { get; protected set; }
        public BasicLab basicLab { get; set; }
        private PanelObject panel { get; set; }
        #endregion

        #region "Selected Body"
        public Body currentBody = null;
        private bool bodySel = false;
        /// <summary>
        /// indicates weather the bodu=y is being moved by mouse or not
        /// </summary>
        bool bodyMoving = false;
        /// <summary>
        /// the current body under cursor (is moving by mouse)
        /// </summary>
        Body bdy = null;
        #endregion

        #region "Helpful"
        Ray ray;
        #endregion

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
            this.IsMouseVisible = true;

            basicLab = new BasicLab(this);
            Components.Add(basicLab);
            panel = new PanelObject(this);
            Components.Add(panel);
            camera = new Camera(this, new Vector3(0, 0, 20f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            Recorder = new XMLRecorder(basicLab.bodys,@"D:\Lab.xml");

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
            /*
            Ball ball = new Ball(0.5f);
            ball.Position = new Vector3(2f, 0f, 0f);
            ball.model = basicLab.BallModel;
            ball.Texture = basicLab.BallTexture;
            ball.SelectedTexture = basicLab.SelectedBallTexture;
            ball.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            ball.Mass = 5f;
            //ball.InverseMass = 0;
            basicLab.AddBall(ball);

            Ball ball1 = new Ball(0.5f);
            ball1.Position = new Vector3(0.1f, -2f, 0f);
            ball1.model = basicLab.BallModel;
            ball1.Texture = basicLab.BallTexture;
            ball1.SelectedTexture = basicLab.SelectedBallTexture;
            ball1.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            ball1.InverseMass = 0;
            basicLab.AddBall(ball1);
            
            Ball ball2 = new Ball(0.5f);
            ball2.model = basicLab.BallModel;
            ball2.Texture = basicLab.BallTexture;
            ball2.Mass = 10f;
            ball2.Position = new Vector3(1f, 1f, 2f);
            ball2.SelectedTexture = basicLab.SelectedBallTexture;
            ball2.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            basicLab.AddBall(ball2);

            Ball ball3 = new Ball(0.2f);
            ball3.model = basicLab.BallModel;
            ball3.Texture = basicLab.BallTexture;
            ball3.Mass = 2f;
            ball3.Position = new Vector3(2f, 1f, 2f);
            ball3.SelectedTexture = basicLab.SelectedBallTexture;
            ball3.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            basicLab.AddBall(ball3);
            
            Crate crate = new Crate(new Vector3(0.5f, 0.2f, 0.3f));
            crate.Mass = 1f;
            crate.model = basicLab.CrateModel;
            crate.Texture = basicLab.CrateTexture;
            crate.SelectedTexture = basicLab.SelectedCrateTexture;
            crate.SelectedTexture_Panel = basicLab.SelectedCrateTexture2;
            crate.Position = new Vector3(1f, -2f, 1f);
            basicLab.AddCrate(crate);

            basicLab.CreateRoom(10f, 10f, 10f);

            basicLab.AddEffect(new Spring(ball, ball1, new Vector3(1, 1, 1), 10f, 2f, 0.995f));
            
             */
            basicLab.CreateRoom(10f, 10f, 10f);

            Ball ball1 = new Ball(0.5f);
            ball1.InverseInertiaTensor = new Matrix();
            ball1.Mass = 1f;
            ball1.Position = new Vector3(0f, 8.5f, -5.5f);
            ball1.model = basicLab.BallModel;
            ball1.Texture = basicLab.BallTexture;
            ball1.SelectedTexture = basicLab.SelectedBallTexture;
            ball1.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            basicLab.AddBall(ball1);

            Ball ball2 = new Ball(0.5f);
            ball2.Mass = 1f;
            ball2.InverseInertiaTensor = new Matrix();
            ball2.Position = new Vector3(-9f, 8.5f, -6f);
            ball2.model = basicLab.BallModel;
            ball2.Texture = basicLab.BallTexture;
            ball2.SelectedTexture = basicLab.SelectedBallTexture;
            ball2.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            basicLab.AddBall(ball2);

            Ball ball3 = new Ball(0.6f);
            ball3.Mass = 1f;
            ball3.InverseInertiaTensor = new Matrix();
            ball3.Position = new Vector3(6f, 8.5f, -9f);
            ball3.model = basicLab.BallModel;
            ball3.Texture = basicLab.BallTexture;
            ball3.SelectedTexture = basicLab.SelectedBallTexture;
            ball3.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            basicLab.AddBall(ball3);


            Ball ball4 = new Ball(0.5f);
            ball4.Mass = 5f;
            ball4.InverseInertiaTensor = new Matrix();
            ball4.Position = new Vector3(7f, 8.5f, -5f);
            ball4.model = basicLab.BallModel;
            ball4.Texture = basicLab.BallTexture;
            ball4.SelectedTexture = basicLab.SelectedBallTexture;
            ball4.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            basicLab.AddBall(ball4);

            Ball FixedBall = new Ball(0.5f);
            FixedBall.InverseMass = 0;
            FixedBall.InverseInertiaTensor = new Matrix();
            FixedBall.Position = new Vector3(1.5f, 8f, -2f);
            FixedBall.model = basicLab.BallModel;
            FixedBall.Texture = basicLab.BallTexture;
            FixedBall.SelectedTexture = basicLab.SelectedBallTexture;
            FixedBall.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            basicLab.AddBall(FixedBall);

            Ball SBall = new Ball(0.5f);
            SBall.Mass = 2f;
            SBall.InverseInertiaTensor = new Matrix();
            SBall.Position = new Vector3(1.5f, 0f, -2f);
            SBall.model = basicLab.BallModel;
            SBall.Texture = basicLab.BallTexture;
            SBall.SelectedTexture = basicLab.SelectedBallTexture;
            SBall.SelectedTexture_Panel = basicLab.SelectedBallTexture2;
            basicLab.AddBall(SBall);

            //basicLab.AddEffect(new Spring(SBall, FixedBall, new Vector3(1, 1, 1), 5f, 6f, 0.9f));
            basicLab.AddEffect(new Spring(SBall, FixedBall, new Vector3(1, 1, 1), 5f, 1f, 0.9f));
            basicLab.AddEffect(new Spring(SBall, ball1, new Vector3(1, 1, 1), 5f, 1f, 0.9f));
            basicLab.AddEffect(new Spring(ball1, ball2, new Vector3(1, 1, 1), 5f, 1f, 0.9f));
            basicLab.AddEffect(new Gravity(new Vector3(0, -10f, 0)));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        { }

        public void SelectedAndMoving(MouseState mouse, KeyboardState keyboard,
            Vector2 cursorPosition,Vector2 previousCursorPosition, float totalSecond)
        {
            var cursorDelta = (cursorPosition - previousCursorPosition) * totalSecond;
            var cameraDelta = (camera.cameraPosition - previousCameraPosition) * totalSecond;

            if (!bodySel)
            {
                ray = camera.GetMouseRay(cursorPosition, GraphicsDevice.Viewport);
                if (!bodyMoving) bdy = basicLab.CheckIntersect(ray);
                if (bdy != null)
                {
                    currentBody = bdy;
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
                            ((IMoveable)bdy).Translate(-camera.cameraDirection, cameraDelta.Y + cursorDelta.Y + cameraDelta.X + cursorDelta.X);
                        }
                        else
                        {
                            Vector3 moveUnit = Vector3.Cross(camera.cameraUp, camera.cameraDirection);
                            moveUnit.Normalize();
                            ((IMoveable)bdy).Translate(-moveUnit, cameraDelta.X + cursorDelta.X);
                            ((IMoveable)bdy).Translate(-camera.cameraUp, cameraDelta.Y + cursorDelta.Y);
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

                ((IMoveable)prevBody).Translate(-camera.cameraDirection, cameraDelta.Y + cursorDelta.Y);
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

        void Crate(Vector2 cursorPosition, KeyboardState keyboard, float time)
        {
            if (keyboard.IsKeyDown(Keys.B))
                bClicked = true;
            if (keyboard.IsKeyUp(Keys.B) && bClicked)
            {
                Vector3 point = new Vector3(cursorPosition, 0.9f);
                point = GraphicsDevice.Viewport.Unproject(point, camera.projection, camera.view, Matrix.Identity);
                Body body = basicLab.CreateBall(point);
                panel.CreateDialog(body);
                currentBody = body;
                bClicked = false;
            }
            if (keyboard.IsKeyDown(Keys.C))
                cClicked = true;
            if (keyboard.IsKeyUp(Keys.C) && cClicked)
            {
                Vector3 point = new Vector3(cursorPosition, 0.9f);
                point = GraphicsDevice.Viewport.Unproject(point, camera.projection, camera.view, Matrix.Identity);
                Crate crate = basicLab.CreateCrate(point);
                panel.CreateDialog(crate);
                currentBody = crate;
                cClicked = false;
            }
            if (keyboard.IsKeyDown(Keys.P))
                pClicked = true;
            if (keyboard.IsKeyUp(Keys.P) && pClicked)
            {
                Body body = basicLab.CreateBall(camera.cameraPosition);
                panel.CreateDialog(body);
                currentBody = body;
                body.AddVelocity(ray.Direction * 25);
                pClicked = false;
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

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
            Recorder.Update();
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Recorder.Stop();
            base.OnExiting(sender, args);
        }
    }
}
