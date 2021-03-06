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
    public class BasicLab : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region "Main Components"
        private SpriteBatch spriteBatch;
        #endregion

        #region "Physics Components"
        public List<Body> bodys = new List<Body>();
        private List<Body> room = new List<Body>();
        private List<PhysiXEngine.Effect> effects = new List<PhysiXEngine.Effect>();
        public Dictionary<String, Spring> springs = new Dictionary<String, Spring>();
        private ContactGenerator cg = new ContactGenerator();
        #endregion

        #region "Graphics Components"
        public Texture2D BallTexture;
        public Texture2D SelectedBallTexture;
        public Texture2D SelectedBallTexture2;
        public Model BallModel;
        public Texture2D CrateTexture;
        public Texture2D SelectedCrateTexture;
        public Texture2D SelectedCrateTexture2;
        public Texture2D Ground;
        public Texture2D Wall;
        public Model CrateModel;
        public float speed = 1f;
        public bool pause = true;
        private bool tClicked = false;
        private bool Changed = false;
        private Vector3 oldG = Vector3.Zero;
        private int numOfSpring = 0;
        #endregion

        #region "Game Component"
        Panel panel = null;
        #endregion

        public BasicLab(Game game)
            : base(game)
        { }

        public void AddBall(Ball ball) 
        {
            bodys.Add(ball);
            cg.AddBody(ball);
        }

        public void AddBall(Vector3 position = new Vector3(), 
            float radius = 0.5f, float mass = 5f)
        {
            Ball ball = new Ball(radius);
            ball.Mass = mass;
            ball.Position = position;
            ball.model = BallModel;
            ball.Texture = BallTexture;
            ball.SelectedTexture = SelectedBallTexture;
            bodys.Add(ball);
            cg.AddBody(ball);
        }

        public void AddBall(Model model, Texture2D texture, Texture2D selectedTexture, 
            Vector3 position = new Vector3(), float radius = 0.5f, float mass = 5f)
        {
            Ball ball = new Ball(radius);
            ball.Mass = mass;
            ball.Position = position;
            ball.model = model;
            ball.Texture = texture;
            ball.SelectedTexture = selectedTexture;
            bodys.Add(ball);
            cg.AddBody(ball);
        }


        public void AddCrate(Crate crate)
        {
            bodys.Add(crate);
            cg.AddBody(crate);
        }

        public void AddCrate(Vector3 halfSize, 
            Vector3 position = new Vector3(), float mass = 5f)
        {
            Crate crate = new Crate(halfSize);
            crate.Mass = mass;
            crate.Position = position;
            crate.model = CrateModel;
            crate.Texture = CrateTexture;
            bodys.Add(crate);
            cg.AddBody(crate);
        }

        public void AddCrate(Model model, Texture2D texture, Vector3 halfSize, 
            Vector3 position = new Vector3(), float mass = 5f)
        {
            Crate crate = new Crate(halfSize);
            crate.Mass = mass;
            crate.Position = position;
            crate.model = model;
            crate.Texture = texture;
            bodys.Add(crate);
            cg.AddBody(crate);
        }

        public void AddToRoom(Crate crate)
        {
            room.Add(crate);
            cg.AddBody(crate);
        }

        public void AddToRoom(Ball ball)
        {
            room.Add(ball);
            cg.AddBody(ball);
        }

        public void AddEffect(PhysiXEngine.Effect effect)
        {

            if (effect as Gravity != null)
            {
                foreach (Body bdy in bodys)
                    ((Gravity)effect).AddBody(bdy);
                effects.Add(effect);
            }
            else if (effects.Count > 0)
            {
                PhysiXEngine.Effect ef = effects.Last<PhysiXEngine.Effect>();
                effects.Remove(ef);
                effects.Add(effect);
                effects.Add(ef);
            }
            else
            {
                effects.Add(effect);
            }
            if (effect as Spring != null)
            {
                numOfSpring++;
                springs.Add("Spring" + numOfSpring, (Spring)effect);
            }
        }

        public Body CheckIntersect(Ray ray)
        {
            foreach (Body bdy in bodys)
            {
                if (bdy as Sphere != null)
                {
                    if (((Sphere)bdy).sphere.Intersects(ray) != null)
                    {
                        ((Ball)bdy).Selected = true;
                        return bdy;
                    }
                }
                else if (bdy as Box != null)
                {
                    //TODO make a better algo for intersecting
                    if (((Box)bdy).GetBoundingSphere().Intersects(ray) != null)
                    {
                        ((Crate)bdy).Selected = true;
                        return bdy;
                    }
                }
            }
            return null;
        }

        public Ball CreateBall(Vector3 position)
        {
            Ball ball = new Ball(0.5f);
            ball.Position = position;
            ball.Mass = 1f;
            ball.model = BallModel;
            ball.Texture = BallTexture;
            ball.SelectedTexture = SelectedBallTexture;
            ball.SelectedTexture_Panel = SelectedBallTexture2;
            AddBall(ball);
            PhysiXEngine.Effect e = effects.Last<PhysiXEngine.Effect>();
            if (e as Gravity != null)
                ((Gravity)e).AddBody(ball);
            else
                throw new Exception();
            return ball;
        }

        public Crate CreateCrate(Vector3 position)
        {
            Crate crate = new Crate(new Vector3(0.5f, 0.5f, 0.5f));
            crate.Position = position;
            crate.Mass = 1f;
            crate.model = CrateModel;
            crate.Texture = CrateTexture;
            crate.SelectedTexture = SelectedCrateTexture;
            crate.SelectedTexture_Panel = SelectedCrateTexture2;
            AddCrate(crate);
            PhysiXEngine.Effect e = effects.Last<PhysiXEngine.Effect>();
            if (e as Gravity != null)
                ((Gravity)e).AddBody(crate);
            else
                throw new Exception();
            return crate;
        }

        public void CrateSpring(Body one, Body two)
        {
            AddEffect(new Spring(one, two, 5f, 6f, 0.9f));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            panel = new Panel(this.Game, new Vector2(Game.Window.ClientBounds.Width / 2 - 200 / 2,
                Game.Window.ClientBounds.Height / 2 - 150 / 2), 220, 150);

            Game.Components.Add(panel);

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

            BallModel = Game.Content.Load<Model>(@"Models\ball");
            BallTexture = Game.Content.Load<Texture2D>(@"Textures\texBall");
            SelectedBallTexture = Game.Content.Load<Texture2D>(@"Textures\SelectedtexBall");
            SelectedBallTexture2 = Game.Content.Load<Texture2D>(@"Textures\SelectedtexBall2");


            CrateModel = Game.Content.Load<Model>(@"Models\box");
            CrateTexture = Game.Content.Load<Texture2D>(@"Textures\texBox");
            SelectedCrateTexture = Game.Content.Load<Texture2D>(@"Textures\SelectedtexBox");
            SelectedCrateTexture2 = Game.Content.Load<Texture2D>(@"Textures\SelectedtexBox2");


            Ground = Game.Content.Load<Texture2D>(@"Textures\Ground");
            Wall = Game.Content.Load<Texture2D>(@"Textures\Wall");
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        public void CreateRoom(float width, float length, float height)
        {
            Crate ground = new Crate(new Vector3(width, 4f, height));
            ground.Position = new Vector3(0f, -9f, 0f);
            ground.model = CrateModel;
            ground.Texture = Ground;
            ground.SelectedTexture = null;
            ground.InverseMass = 0;
            ground.Lock();
            AddToRoom(ground);

            Crate RightWall = new Crate(new Vector3(0.01f, length, height));
            RightWall.Position = new Vector3(width+0.01f, 5.01f, 0f);
            RightWall.model = CrateModel;
            RightWall.Texture = Wall;
            RightWall.SelectedTexture = null;
            RightWall.InverseMass = 0;
            RightWall.Lock();
            AddToRoom(RightWall);

            Crate FrontWall = new Crate(new Vector3(width, length, 0.01f));
            FrontWall.Position = new Vector3(0f, 5.01f, -(height + 0.01f));
            FrontWall.model = CrateModel;
            FrontWall.Texture = Game.Content.Load<Texture2D>(@"Textures\Wall");
            FrontWall.SelectedTexture = null;
            FrontWall.InverseMass = 0;
            FrontWall.Lock();
            AddToRoom(FrontWall);

            Crate LeftWall = new Crate(new Vector3(0.01f, length, height));
            LeftWall.Position = new Vector3(-(width+0.01f), 5.01f, 0f);
            LeftWall.model = CrateModel;
            LeftWall.Texture = Wall;
            LeftWall.SelectedTexture = null;
            LeftWall.InverseMass = 0;
            LeftWall.Lock();
            AddToRoom(LeftWall);

            Crate Slope = new Crate(new Vector3(width / 4, length - length / 4, 0.01f));
            Slope.Position = new Vector3(width - width / 4, length/4,  0f);
            Slope.AddScaledOrientation(new Vector3(-0.9f,0,0));
            Slope.model = CrateModel;
            Slope.Texture = Ground;
            Slope.SelectedTexture = null;
            Slope.InverseMass = 0;
            Slope.Lock();
            AddToRoom(Slope);

            Crate Roof = new Crate(new Vector3(width, 0.4f, height/4));
            Roof.Position = new Vector3(0, length / 2+length/4, -(height - height/4));
            Roof.model = CrateModel;
            Roof.Texture = Ground;
            Roof.SelectedTexture = null;
            Roof.InverseMass = 0;
            Roof.Lock();
            AddToRoom(Roof);

            Crate Stop = new Crate(new Vector3(width/4, 1f, 0.1f));
            Stop.Position = new Vector3(width - width / 4, -height/2 + 1, length);
            Stop.model = CrateModel;
            Stop.Texture = Ground;
            Stop.SelectedTexture = null;
            Stop.InverseMass = 0;
            Stop.Lock();
            AddToRoom(Stop);
        }

        void CreatePanel()
        {
            Initialize();
            panel.AddField("Friction", ContactGenerator.friction);
            panel.AddField("Restitution", ContactGenerator.restitution);
            panel.AddLabel("Gravity", "Gravity");
            panel.AddXYZ(((Gravity)effects.Last<PhysiXEngine.Effect>()).gravity, "gev");
            panel.AddOkButton();
            panel.AddCancelButton();
            pause = true;
            panel.Show = true;
        }


        private void GetValue()
        {
            if (!panel.Show && Changed)
            {
                ContactGenerator.friction = panel.GetVlaue("Friction");
                ContactGenerator.restitution = panel.GetVlaue("Restitution");
                ((Gravity)effects.Last<PhysiXEngine.Effect>()).gravity = new Vector3(
                    panel.GetVlaue("gevX"), panel.GetVlaue("gevY"), panel.GetVlaue("gevZ")
                    );
                Changed = false;
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float duration = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            duration *= speed;
            if (!pause)
            {
                foreach (PhysiXEngine.Effect ef in effects)
                {
                    ef.Update(duration);
                }
                foreach (Body bdy in bodys)
                {
                    bdy.Update(duration);
                }
                cg.Update(duration);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.T))
                tClicked = true;
            if (Keyboard.GetState().IsKeyUp(Keys.T) && tClicked)
            {
                CreatePanel();
                Changed = true;
                tClicked = false;
            }
            GetValue();
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (Body bdy in room)
            {
                ((Drawable)bdy).Draw(((Lab)Game).camera);
            }
            foreach (Body bdy in bodys)
            {
                ((Drawable)bdy).Draw(((Lab)Game).camera);
            }
            

            base.Draw(gameTime);
        }
    }
}
