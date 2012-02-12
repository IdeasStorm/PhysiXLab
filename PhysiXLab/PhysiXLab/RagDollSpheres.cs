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
    class RagDollSpheres : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        #region "testing components"
        //int NUM_CRATES = 12;
        int NUM_JOINTS = 14;
        int NUM_BALLS = 15;
        //Crate[] crates;
        Ball[] balls;
        Joint[] joints;

        ContactGenerator cg;
        Camera camera;
        Gravity g;
        #endregion

        public RagDollSpheres()
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

            //crates = new Crate[NUM_CRATES];

            Ball b = new Ball(2);
            b.Position = Vector3.Up * 1000;

            cg = new ContactGenerator();
            cg.AddBody(b);

            joints = new Joint[NUM_JOINTS];
            balls = new Ball[NUM_BALLS];
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

            #region
            ////Head
            //crates[0] = new Crate(new Vector3(0.1f / 2f, 0.1f / 2f, 0.1f / 2f));

            ////Arms
            //crates[1] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            //crates[2] = new Crate(new Vector3(0.1f / 2f, 0.3f / 2f, 0.1f / 2f));
            //crates[3] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            //crates[4] = new Crate(new Vector3(0.1f / 2f, 0.3f / 2f, 0.1f / 2f));

            ////Shoulder
            //crates[5] = new Crate(new Vector3(0.5f / 2f, 0.1f / 2f, 0.1f / 2f));

            ////chest
            //crates[6] = new Crate(new Vector3(0.4f / 2f, 0.1f / 2f, 0.1f / 2f));

            ////Stomach
            //crates[7] = new Crate(new Vector3(0.4f / 2f, 0.1f / 2f, 0.1f / 2f));

            ////Legs
            //crates[8] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            //crates[9] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            //crates[10] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));
            //crates[11] = new Crate(new Vector3(0.1f / 2f, 0.2f / 2f, 0.1f / 2f));


            ////Head 
            //crates[0].Position = new Vector3(0, 0.45f, 0);

            ////Arms
            //crates[1].Position = new Vector3(0.35f, 0.25f, 0);
            //crates[2].Position = new Vector3(0.35f, -.05f, 0);
            //crates[3].Position = new Vector3(-0.35f, .25f, 0);
            //crates[4].Position = new Vector3(-0.35f, -0.05f, 0);

            ////Shoulder
            //crates[5].Position = new Vector3(0, 0.3f, -.05f);

            ////chest
            //crates[6].Position = new Vector3(0, 0.15f, 0);

            ////Center Stomach
            //crates[7].Position = new Vector3(0, 0f, 0.05f);

            ////legs
            //crates[8].Position = new Vector3(0.15f, -0.2f, 0);
            //crates[9].Position = new Vector3(0.15f, -0.45f, 0);
            //crates[10].Position = new Vector3(-0.15f, -0.2f, 0);
            //crates[11].Position = new Vector3(-0.15f, -0.45f, 0);

            ////Head and shoulders :)
            //joints[0] = new Joint(crates[0], crates[5], new Vector3(0, -0.05f, -0.05f), new Vector3(0, 0.05f, 0), 0.05f);

            ////right arm
            //joints[1] = new Joint(crates[1], crates[2], new Vector3(0, -0.1f, 0.05f), new Vector3(0, 0.15f, 0), 0.05f);

            ////left arm
            //joints[2] = new Joint(crates[3], crates[4], new Vector3(0, -0.1f, 0.05f), new Vector3(0, 0.15f, 0), 0.05f);

            ////right leg
            //joints[3] = new Joint(crates[8], crates[9], new Vector3(0, -0.1f, 0.05f), new Vector3(0, 0.1f, 0), 0.05f);

            ////left leg
            //joints[4] = new Joint(crates[10], crates[11], new Vector3(0, -0.1f, 0.05f), new Vector3(0, 0.1f, 0), 0.05f);

            ////right arm with shoulders
            //joints[5] = new Joint(crates[5], crates[1], new Vector3(.25f, 0.05f, 0), new Vector3(-0.05f, 0.1f, 0), 0.05f);

            ////left arm with shoulders
            //joints[6] = new Joint(crates[5], crates[3], new Vector3(-.25f, 0.05f, 0), new Vector3(0.05f, 0.1f, 0), 0.05f);


            //joints[7] = new Joint(crates[5], crates[6], new Vector3(0, -0.05f, 0.05f), new Vector3(0, 0.05f, 0), 0.05f);

            //joints[8] = new Joint(crates[6], crates[7], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, -.05f), 0.05f);

            //joints[9] = new Joint(crates[7], crates[8], new Vector3(.15f, -0.05f, 0), new Vector3(0, 0.1f, 0), 0.05f);

            //joints[10] = new Joint(crates[7], crates[10], new Vector3(-.15f, -0.05f, 0), new Vector3(0, 0.1f, 0), 0.05f);
            #endregion


            #region newBall
            balls[0] = new Ball(0.1f);
            balls[1] = new Ball(0.15f);
            balls[2] = new Ball(0.15f);
            balls[3] = new Ball(0.05f);
            balls[4] = new Ball(0.05f);
            balls[5] = new Ball(0.05f);
            balls[6] = new Ball(0.05f);
            balls[7] = new Ball(0.05f);
            balls[8] = new Ball(0.05f);
            balls[9] = new Ball(0.05f);
            balls[10] = new Ball(0.05f);
            balls[11] = new Ball(0.05f);
            balls[12] = new Ball(0.05f);
            balls[13] = new Ball(0.05f);
            balls[14] = new Ball(0.05f);
            #endregion

            #region newBall
            balls[0].Position = new Vector3(0, 0.6f, 0);
            balls[1].Position = new Vector3(0, 0.3f, 0);
            balls[2].Position = new Vector3(0, -0.05f, 0);

            balls[3].Position = new Vector3(0.25f, 0.3f, 0);
            balls[4].Position = new Vector3(0.25f, 0.15f, 0);
            balls[5].Position = new Vector3(0.25f, 0, 0);
            balls[6].Position = new Vector3(-0.25f, 0.3f, 0);
            balls[7].Position = new Vector3(-0.25f, 0.15f, 0);
            balls[8].Position = new Vector3(-0.25f, 0, 0);

            balls[9].Position = new Vector3(0.1f, -.25f, 0);
            balls[10].Position = new Vector3(0.1f, -.4f, 0);
            balls[11].Position = new Vector3(0.1f, -.55f, 0);
            balls[12].Position = new Vector3(-0.1f, -.25f, 0);
            balls[13].Position = new Vector3(-0.1f, -.4f, 0);
            balls[14].Position = new Vector3(-0.1f, -.55f, 0);
            #endregion
            #region Masses
            balls[0].Mass = 5;
            balls[1].Mass = 5;
            balls[2].Mass = 5;
            balls[3].Mass = 1;
            balls[4].Mass = 1;
            balls[5].Mass = 1;
            balls[6].Mass = 1;
            balls[7].Mass = 1;
            balls[8].Mass = 1;
            balls[9].Mass = 1;
            balls[10].Mass = 1;
            balls[11].Mass = 1;
            balls[12].Mass = 1;
            balls[13].Mass = 1;
            balls[14].Mass = 1;
            #endregion

            //head and shoulder :)
            joints[0] = new Joint(balls[0], balls[1], new Vector3(0, -0.1f,0f), new Vector3(0, 0.15f, 0), 0.05f);

            
            joints[1] = new Joint(balls[1], balls[2], new Vector3(0, -0.15f, 0), new Vector3(0, 0.15f, 0), 0.05f);

            //right arm with shoulders
            joints[2] = new Joint(balls[1], balls[3], new Vector3(0.1f, 0.1f, 0), new Vector3(-0.04f, 0.03f, 0), 0.05f);

            //left arm with shoulders
            joints[3] = new Joint(balls[1], balls[6], new Vector3(-0.1f, 0.1f, 0), new Vector3(0.04f, 0.03f, 0), 0.05f);

            //right arm
            joints[4] = new Joint(balls[3], balls[4], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);
            joints[5] = new Joint(balls[4], balls[5], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);            
            
            //left arm
            joints[6] = new Joint(balls[6], balls[7], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);
            joints[7] = new Joint(balls[7], balls[8], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);

            //Stomach with right leg
            joints[8] = new Joint(balls[2], balls[9], new Vector3(0.1f, -0.15f, 0), new Vector3(0, 0.05f, 0), 0.05f);

            //Stomach with left leg
            joints[9] = new Joint(balls[2], balls[12], new Vector3(-.1f, -0.15f, 0), new Vector3(0, 0.05f, 0), 0.05f);

            //right leg
            joints[10] = new Joint(balls[9], balls[10], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);
            joints[11] = new Joint(balls[10], balls[11], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);

            joints[12] = new Joint(balls[12], balls[13], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);
            joints[13] = new Joint(balls[13], balls[14], new Vector3(0, -0.05f, 0), new Vector3(0, 0.05f, 0), 0.05f);

            Model model = this.Content.Load<Model>(@"ball");
            Texture2D texture = this.Content.Load<Texture2D>(@"basic_material");

            for (int i = 0; i < NUM_BALLS; i++)
            {
                balls[i].model = model;
                balls[i].Texture = texture;
                cg.AddBody(balls[i]);
                //balls[i].Mass = 1;
            }


            for (int i = 0; i < NUM_JOINTS; i++)
                cg.AddJoint(joints[i]);

                camera = new Camera(this, new Vector3(0, 0, 0.1f),
                    Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            g = new Gravity(new Vector3(0f, -1f, 0f));

            balls[0].InverseMass = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //for (int i = 0; i < NUM_CRATES; i++)
            //crates[i].LoadContent(Content);

            base.LoadContent();
        }

        //bool spaceClicked=false;
        //int counter = 0;
        int choice;
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

            #region enter valu of chice
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.D1))
                choice = 1;
            if (keyState.IsKeyDown(Keys.D2))
                choice = 2;
            if (keyState.IsKeyDown(Keys.D3))
                choice = 3;
            if (keyState.IsKeyDown(Keys.D4))
                choice = 4;
            if (keyState.IsKeyDown(Keys.D5))
                choice = 5;
            if (keyState.IsKeyDown(Keys.D6))
                choice = 6;
            if (keyState.IsKeyDown(Keys.D7))
                choice = 7;
            if (keyState.IsKeyDown(Keys.D8))
                choice = 8;
            if (keyState.IsKeyDown(Keys.D9))
                choice = 9;
            if (keyState.IsKeyDown(Keys.D0))
                choice = 0;
            #endregion

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                balls[choice].AddForce(Vector3.Up);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                balls[choice].AddForce(Vector3.Down);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                balls[choice].AddForce(Vector3.Left);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                balls[choice].AddForce(Vector3.Right);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                // for (int i = 0; i < NUM_CRATES; i++)
                //   crates[i].AddForce(Vector3.Down / 10);
                balls[0].Mass = 1;
                //balls[6].AddForce(Vector3.Down);
                balls[0].AddForce(Vector3.Down*2);

                balls[11].AddForce(Vector3.Down);
                balls[14].AddForce(Vector3.Down);
                balls[5].AddForce(Vector3.Right);
                balls[8].AddForce(Vector3.Left);
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
            for (int i = 0; i < NUM_BALLS; i++)
                balls[i].Update(duration);
            cg.Update(duration);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            for (int i = 0; i < NUM_BALLS; i++)
                balls[i].Draw(camera);

            base.Draw(gameTime);
        }
    }
}
