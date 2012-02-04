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
    class Rope
    {
        #region "testing components"
        List<Ball>Balls;
        //Collidable upObject;
        //Collidable downObject;
        ContactGenerator cg;
        //Camera camera;
        Gravity g;
        Game game;
        #endregion
        
        public Rope(float length,ContactGenerator cg,Game game,Gravity gravity)
        {
            Balls = new List<Ball>();
            int n = (int)length * 6;

            this.g = gravity;
            this.cg=cg;
            this.game = game;

            Model model=game.Content.Load<Model>(@"ball");
            Texture2D texture=game.Content.Load<Texture2D>(@"basic_material");

            for (int i = 0; i < n; i++)
            {
                Balls.Add(new Ball(0.02f));
                
                Balls[i].model = model;
                Balls[i].Texture = texture;
                Balls[i].Position = new Vector3(0,-i*0.1f,0.0f);
                Balls[i].InverseMass = 1f;
                //g.AddBody(Balls[i]);
                Balls[i].InverseInertiaTensor = new Matrix();
                //cg.AddBody(Balls[i]);
            }

            //Balls[Balls.Count - 1].Position = new Vector3(2, 0, 3);
            //Balls[(Balls.Count - 1) / 2].Position = new Vector3(1, 0, 3);
            //Balls[0].Position = new Vector3(0, 0, 3);
            Ball b = new Ball(0.02f);
            b.Position = Balls[(Balls.Count - 1) / 2].Position+new Vector3(0,-0.1f,3);
            b.Mass = 1;
            //g.AddBody(b);
            b.InverseInertiaTensor = new Matrix();
            //cg.AddConductor(new Rod(Balls[(Balls.Count - 1)/2], b, 0.1f));
            //cg.AddConductor(new Rod(b,Balls[(Balls.Count - 1) / 2], 0.1f));

            for (int i = 0; i < n-1; i++)
            {
                //if (i == (Balls.Count) / 2)//|| i == (Balls.Count - 1) / 2)
                //{
                //    cg.AddConductor(new Rod(Balls[i], b, 0.1f));//,0.6f));
                //    cg.AddConductor(new Rod(b, Balls[i+1], 0.1f));//,0.6f));
                //    continue; 
                //}
                //if (i == ((Balls.Count - 1) / 2)+1)
                //{
                //    cg.AddConductor(new Rod(b, Balls[i], 0.1f));//,0.6f));
                //    continue;
                //}

                //if (i % 2 == 0)
                  //  cg.AddConductor(new Cable(Balls[i], Balls[i + 1], 0.1f, 0.3f));//,0.6f));
                //else
                    cg.AddConductor(new Rod(Balls[i], Balls[i + 1], 0.12f));//,0.6f));
            }
        }

        /// <summary>
        /// constractor of Rope with upper body upper and bottom body bottom
        /// </summary>
        /// <param name="length"></param>
        /// <param name="cg"></param>
        /// <param name="upper"></param>
        /// <param name="bottom"></param>
        public Rope(float length, ContactGenerator cg, Game game, Gravity gravity, Collidable upper, Collidable bottom)
            : this(length, cg,game,gravity)
        {
            AddUpperBody(upper);
            AddBottomBody(bottom);
        }


        public void AddUpperBody(Collidable body)
        {
            cg.AddConductor(new Rod(body, Balls[0], 0.5f));//,0.6f));
        }

        public void AddBottomBody(Collidable body)
        {
            cg.AddConductor(new Rod(Balls[Balls.Count - 1], body, 0.5f));//,0.6f));
        }

        public void update(float duration)
        {
            foreach (Ball b in Balls)
            {
                //g.Update(duration);
                b.Update(duration);
                
            }
 
        }

        public void Draw(Camera camera)
        {
            foreach (Ball b in Balls)
            {
                b.Draw(camera);
            }
        }

    }
}
