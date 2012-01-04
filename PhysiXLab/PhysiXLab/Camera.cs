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


namespace PhysiXLab
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {

        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        // Camera vectors to rotate and Move Camera
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        //speed of camera
        float speed = 3;

        //to rotate camera
        MouseState prevMouseState;


        public Camera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        //define new view matrix
        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            //view = Matrix.CreateLookAt(pos, target, up);
            // Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
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
            // TODO: Add your initialization code here

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
            // TODO: Add your update code here
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
            cameraUp = Vector3.Transform(cameraUp
                , Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection)
                , (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - prevMouseState.Y)));


            cameraDirection = Vector3.Transform(cameraDirection
                , Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection)
                , (MathHelper.PiOver4 / 180) * (Mouse.GetState().Y - prevMouseState.Y)));
            cameraPosition += new Vector3(0, 1, 0) * (Mouse.GetState().Y - prevMouseState.Y);



            // Reset prevMouseState
            prevMouseState = Mouse.GetState();


            // Recreate the camera view matrix
            CreateLookAt();
            base.Update(gameTime);
        }
    }
}
