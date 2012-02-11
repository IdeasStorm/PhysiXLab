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

            Ball b = new Ball(2);
            b.Position = Vector3.Up * 1000;

            cg = new ContactGenerator();
            cg.AddBody(b);

            joints = new Joint[NUM_JOINTS];

            #region oldcode
            /*
            //Arms
            crates[0] = new Crate(new Vector3(0.3f, 1f, 0.2f));
            crates[1] = new Crate(new Vector3(0.3f, 1f, 0.2f));
            crates[2] = new Crate(new Vector3(0.3f, 1f, 0.2f));
            crates[3] = new Crate(new Vector3(0.3f, 1f, 0.2f));

            
            crates[4] = new Crate(new Vector3(0.4f, 0.3f, 0.6f));
            
            
            
            crates[5] = new Crate(new Vector3(0.3f, 0.3f, 0.6f));
            crates[6] = new Crate(new Vector3(0.4f, 0.3f, 0.7f));
            crates[7] = new Crate(new Vector3(0.4f, 0.5f, 0.4f));
            
            //Legs
            crates[8] = new Crate(new Vector3(0.2f, 0.8f, 0.2f));
            crates[9] = new Crate(new Vector3(0.2f, 0.8f, 0.2f));
            crates[10] = new Crate(new Vector3(0.2f, 0.8f, 0.2f));
            crates[11] = new Crate(new Vector3(0.2f, 0.8f, 0.2f));


            crates[0].Position=new Vector3(0,0.9f,-0.5f);
            crates[1].Position = new Vector3(0, 3.159f, -0.56f);
            crates[2].Position = new Vector3(0, 0.993f, 0.5f);
            crates[3].Position = new Vector3(0, 3.15f, 0.56f);
            crates[4].Position = new Vector3(-0.054f, 4.683f, 0.013f);
            crates[5].Position = new Vector3(0.043f, 5.603f, 0.013f);
            crates[6].Position = new Vector3(0, 6.485f, 0.013f);
            crates[7].Position = new Vector3(0, 7.759f, 0.013f);
            crates[8].Position = new Vector3(0, 5.946f, -1.066f);
            crates[9].Position = new Vector3(0, 4.024f, -1.066f);
            crates[10].Position = new Vector3(0, 5.946f, 1.066f);
            crates[11].Position = new Vector3(0, 4.024f, 1.066f);
           
            for (int i = 0; i < NUM_CRATES; i++)
            {
                crates[i].Mass = 1;
                crates[i].Position = Vector3.Zero;
            }

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
            */
            #endregion

            //Head
            crates[0] = new Crate(new Vector3(0.1f / 2f, 0.1f / 2f, 0.1f / 2f));

            //Arms
            crates[1] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            crates[2] = new Crate(new Vector3(0.1f / 2f, 0.3f / 2f, 0.1f / 2f));
            crates[3] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            crates[4] = new Crate(new Vector3(0.1f / 2f, 0.3f / 2f, 0.1f / 2f));

            //Shoulder
            crates[5] = new Crate(new Vector3(0.5f / 2f, 0.1f / 2f, 0.1f / 2f));

            //chest
            crates[6] = new Crate(new Vector3(0.4f / 2f, 0.1f / 2f, 0.1f / 2f));

            //Stomach
            crates[7] = new Crate(new Vector3(0.4f / 2f, 0.1f / 2f, 0.1f / 2f));

            //Legs
            crates[8] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            crates[9] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            crates[10] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            crates[11] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));


            //Head 
            crates[0].Position = new Vector3(0, 0.45f, 0);

            //Arms
            crates[1].Position = new Vector3(0.35f, 0.25f, 0);
            crates[2].Position = new Vector3(0.35f, -.05f, 0);
            crates[3].Position = new Vector3(-0.35f, .25f, 0);
            crates[4].Position = new Vector3(-0.35f, -0.05f, 0);

            //Shoulder
            crates[5].Position = new Vector3(0, 0.3f, -.05f);

            //chest
            crates[6].Position = new Vector3(0, 0.15f, 0);

            //Center Stomach
            crates[7].Position = new Vector3(0, 0f, 0.05f);

            //legs
            crates[8].Position = new Vector3(0.15f, -0.2f, 0);
            crates[9].Position = new Vector3(0.15f, -0.45f, 0);
            crates[10].Position = new Vector3(-0.15f, -0.2f, 0);
            crates[11].Position = new Vector3(-0.15f, -0.45f, 0);



            //Head and shoulders :)
            joints[0] = new Joint(crates[0], crates[5], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);

            //right arm
            joints[1] = new Joint(crates[1], crates[2], new Vector3(0, -0.1f, 0.05f), new Vector3(0, 0.15f, 0), 0.05f);

            //left arm
            joints[2] = new Joint(crates[3], crates[4], new Vector3(0, -0.1f, 0.05f), new Vector3(0, 0.15f, 0), 0.05f);

            //right leg
            joints[3] = new Joint(crates[8], crates[9], new Vector3(0, -0.1f, 0.05f), new Vector3(0, 0.1f, 0), 0.05f);

            //left leg
            joints[4] = new Joint(crates[10], crates[11], new Vector3(0, -0.1f, 0.05f), new Vector3(0, 0.1f, 0), 0.05f);

            //right arm with shoulders
            joints[5] = new Joint(crates[5], crates[1], new Vector3(.25f, 0.05f, 0), new Vector3(-0.05f, 0.1f, 0), 0.05f);

            //left arm with shoulders
            joints[6] = new Joint(crates[5], crates[3], new Vector3(-.25f, 0.05f, 0), new Vector3(0.05f, 0.1f, 0), 0.05f);


            joints[7] = new Joint(crates[5], crates[6], new Vector3(0, -0.05f, 0.05f), new Vector3(0, 0.05f, 0), 0.05f);

            joints[8] = new Joint(crates[6], crates[7], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, -.05f), 0.05f);

            joints[9] = new Joint(crates[7], crates[8], new Vector3(.15f, -0.05f, 0), new Vector3(0, 0.1f, 0), 0.05f);

            joints[10] = new Joint(crates[7], crates[10], new Vector3(-.15f, -0.05f, 0), new Vector3(0, 0.1f, 0), 0.05f);


            for (int i = 0; i < NUM_CRATES; i++)
            {
                crates[i].InverseMass = 1;
                //cg.AddBody(crates[i]);
                //crates[i].InverseInertiaTensor = new Matrix();
            }


            for (int i = 0; i < NUM_JOINTS; i++)
                cg.AddJoint(joints[i]);

            camera = new Camera(this, new Vector3(0, 0, 0.1f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            g = new Gravity(new Vector3(0f, -1f, 0f));

            crates[0].InverseMass = 0;
            //crates[7].InverseMass = 0;
            //crates[6].InverseInertiaTensor = new Matrix();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            for (int i = 0; i < NUM_CRATES; i++)
                crates[i].LoadContent(Content);

            base.LoadContent();
        }

        //bool spaceClicked=false;
        //int counter = 0;
        Vector3 temp = Vector3.Zero;
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float duration = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
            //{
            //    for (int i = 0; i < NUM_CRATES; i++)
            //        g.AddBody(crates[i]);
            //}

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
               // for (int i = 0; i < NUM_CRATES; i++)
                 //   crates[i].AddForce(Vector3.Down / 10);
                crates[0].InverseMass = 1;
                //crates[6].AddForce(Vector3.Down);
                crates[0].AddForce(Vector3.Down);
                crates[1].AddForce(Vector3.Down);
                crates[3].AddForce(Vector3.Down);
                crates[8].AddForce(Vector3.Down);
                crates[10].AddForce(Vector3.Down);
            }

            
            //if (Keyboard.GetState().IsKeyDown(Keys.B))
            //{
            //    spaceClicked = true;
            //}


            //if (spaceClicked&&counter<NUM_JOINTS)
            //{
            //    cg.DeleteJoint(joints[counter]);
            //    counter++;
            //}

            g.Update(duration);
            for (int i = 0; i < NUM_CRATES; i++)
                crates[i].Update(duration);
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
