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


namespace Test
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {

        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public bool locked { get; set; }
        private Vector3 target;
        public Vector3 Target
        {
            set { target = value; locked = true; }
            get { return target; }
        }

        /// <summary>
        /// frees the target so the camera depends on direction
        /// </summary>
        public void freeTarget() {
            locked = false;
        }

        // Camera vectors to rotate and Move Camera
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        //speed of camera
        float speed = 0.1f;

        //to rotate camera
        MouseState prevMouseState;


        public Camera(Game game)
            : base(game)
        {            
        }

        //define new view matrix
        private void CreateLookAt()
        {
            if (!locked)
                target = cameraPosition + cameraDirection;
            view = Matrix.CreateLookAt(cameraPosition, target, cameraUp);
        }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up,bool locked =false)
            : base(game)
        {
            //view = Matrix.CreateLookAt(pos, target, up);
            // Build camera view matrix
            cameraPosition = pos;
            this.target = target;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            this.locked = locked;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height,
                1, 3000);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2
                , Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cameraPosition += cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPosition -= cameraDirection * speed;
            // Move side to side
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;

            // Yaw rotation
            cameraDirection = Vector3.Transform(cameraDirection
                , Matrix.CreateFromAxisAngle(cameraUp, (-MathHelper.PiOver4 / 200) *
                (Mouse.GetState().X - prevMouseState.X)));
            //cameraPosition += new Vector3(1,0,0) * -(Mouse.GetState().X - prevMouseState.X);

            // Roll rotation
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                cameraUp = Vector3.Transform(cameraUp
                    , Matrix.CreateFromAxisAngle(cameraDirection
                    , MathHelper.PiOver4 / 45));
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                cameraUp = Vector3.Transform(cameraUp
                    , Matrix.CreateFromAxisAngle(cameraDirection
                    , -MathHelper.PiOver4 / 45));
            }

            // Pitch rotation
            cameraDirection = Vector3.Transform(cameraDirection
                , Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection)
                , (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - prevMouseState.Y)));


            cameraDirection = Vector3.Transform(cameraDirection
                , Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection)
                , (MathHelper.PiOver4 / 180) * (Mouse.GetState().Y - prevMouseState.Y)));



            // Reset prevMouseState
            prevMouseState = Mouse.GetState();


            // Recreate the camera view matrix
            CreateLookAt();
            base.Update(gameTime);
        }
    }
}
