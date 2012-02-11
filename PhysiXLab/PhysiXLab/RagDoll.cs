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
    class RagDoll : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        int NUM_CRATES = 12;
        int NUM_JOINTS = 11;

        Crate[] crates;
        Joint[] joints;

        ContactGenerator cg;
        Camera camera;
        Gravity g;
        #endregion

        public RagDoll()
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
            crates = new Crate[NUM_CRATES];

            //Arms
            crates[0] = new Crate(new Vector3(0.3f, 1f, 0.2f));
            crates[1] = new Crate(new Vector3(0.3f, 1f, 0.2f));
            crates[2] = new Crate(new Vector3(0.3f, 1f, 0.2f));
            crates[3] = new Crate(new Vector3(0.3f, 1f, 0.2f));

            //
            crates[4] = new Crate(new Vector3(0.4f, 0.3f, 0.6f));
            
            
            //
            crates[5] = new Crate(new Vector3(0.3f, 0.3f, 0.6f));
            crates[6] = new Crate(new Vector3(0.4f, 0.3f, 0.7f));
            crates[7] = new Crate(new Vector3(0.4f, 0.5f, 0.4f));
            
            //Legs
            crates[8] = new Crate(new Vector3(0.2f, 0.8f, 0.2f));
            crates[9] = new Crate(new Vector3(0.2f, 0.8f, 0.2f));
            crates[10] = new Crate(new Vector3(0.2f, 0.8f, 0.2f));
            crates[11] = new Crate(new Vector3(0.2f, 0.8f, 0.2f));



            for (int i = 0; i < NUM_CRATES; i++)
            {
                crates[i].Mass = 1;
                crates[i].Position = Vector3.Zero;
            }

            Ball b = new Ball(2);
            b.Position = Vector3.Up * 1000;

            cg = new ContactGenerator();
            cg.AddBody(b);

            joints = new Joint[NUM_JOINTS];


            //right leg
            joints[0]=new Joint(crates[0],crates[1],new Vector3(0,1,0),new Vector3(0,-1,0),0.15f);
            
            //Left leg
            joints[1]=new Joint(crates[2],crates[3],new Vector3(0,1,0),new Vector3(0,-1,0),0.15f);
            
            //right arm
            joints[2]=new Joint(crates[9],crates[8],new Vector3(0,1,0),new Vector3(0,-1,0),0.15f);
            
            //left arm
            joints[3]=new Joint(crates[8],crates[1],new Vector3(0,1,0),new Vector3(0,-1,0),0.15f);      

            //stomach to waist
            joints[4]=new Joint(crates[4],crates[5],new Vector3(0.05f,0.5f,0),new Vector3(-0.04f,-0.45f,0),0.15f);      

            //
            joints[5]=new Joint(crates[5],crates[6],new Vector3(-0.04f,0.4f,0),new Vector3(0,-4,0),0.15f);      

            joints[6]=new Joint(crates[6],crates[7],new Vector3(0,0.5f,0),new Vector3(0,-0.7f,0),0.15f);      

            joints[7]=new Joint(crates[1],crates[4],new Vector3(0,1,0),new Vector3(0,-0.45f,-.5f),0.15f);      

            joints[8]=new Joint(crates[3],crates[4],new Vector3(0,1,0),new Vector3(0,-0.45f,0.5f),0.15f);      

            joints[9]=new Joint(crates[6],crates[8],new Vector3(0,0.3f,-.8f),new Vector3(0,0.8f,.3f),0.15f);      

            joints[10]=new Joint(crates[6],crates[10],new Vector3(0,0.3f,0.8f),new Vector3(0,0.8f,-0.3f),0.15f);
            
            for (int i = 0; i < NUM_JOINTS; i++)
                cg.AddJoint(joints[i]);

            camera = new Camera(this, new Vector3(0, 0, 0.1f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            g = new Gravity(new Vector3(0f, -10f, 0f));

            crates[4].InverseMass = 0;
            crates[4].InverseInertiaTensor = new Matrix();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            for (int i = 0; i < NUM_CRATES; i++)
                crates[i].LoadContent(Content);

            base.LoadContent();
        }

        bool spaceClicked;
        Vector3 temp = Vector3.Zero;
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
                for (int i = 0; i < NUM_CRATES; i++)
                    crates[i].AddForce(Vector3.Down * 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                for (int i = 0; i < NUM_CRATES; i++)
                crates[i].AddForce(Vector3.Up * 10);
            }

            g.Update(duration);
            for (int i = 0; i < NUM_CRATES; i++)
                crates[i].Update(duration) ;
            cg.Update(duration);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            for (int i = 0; i < NUM_CRATES; i++)
                crates[i].Draw(camera);

            base.Draw(gameTime);
        }
    }
}
