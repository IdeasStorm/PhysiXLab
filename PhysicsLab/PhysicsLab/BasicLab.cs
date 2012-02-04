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
        private List<Body> bodys = new List<Body>();
        private List<Body> room = new List<Body>();
        private List<PhysiXEngine.Effect> effects = new List<PhysiXEngine.Effect>();
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
        public Model CrateModel;
        public float speed = 1f;
        public bool pause = true;
        #endregion

        public BasicLab(Game game)
            : base(game)
        {
            // TODO: Construct any child components here

        }

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
            effects.Add(effect);
            if (effect as Gravity != null)
            {
                foreach (Body bdy in bodys)
                    ((Gravity)effect).AddBody(bdy);
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
                    if (((Box)bdy).box.Intersects(ray) != null)
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
            AddBall(ball);
            PhysiXEngine.Effect e = effects.Last<PhysiXEngine.Effect>();
            if (e as Gravity != null)
                ((Gravity)e).AddBody(ball);
            else
                throw new Exception();
            return ball;
        }

        public void CreateCrate(Vector3 position)
        {
            Crate crate = new Crate(new Vector3(0.5f, 0.5f, 0.5f));
            crate.Position = position;
            crate.Mass = 1f;
            crate.model = CrateModel;
            crate.Texture = CrateTexture;
            crate.SelectedTexture = SelectedCrateTexture;
            AddCrate(crate);
            PhysiXEngine.Effect e = effects.Last<PhysiXEngine.Effect>();
            if (e as Gravity != null)
                ((Gravity)e).AddBody(crate);
            else
                throw new Exception();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

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
            BallModel = Game.Content.Load<Model>(@"Models\ball");
            BallTexture = Game.Content.Load<Texture2D>(@"Textures\texBall");
            SelectedBallTexture = Game.Content.Load<Texture2D>(@"Textures\SelectedtexBall");
            SelectedBallTexture2 = Game.Content.Load<Texture2D>(@"Textures\SelectedtexBall2");


            CrateModel = Game.Content.Load<Model>(@"Models\box");
            CrateTexture = Game.Content.Load<Texture2D>(@"Textures\texBox");
            SelectedCrateTexture = Game.Content.Load<Texture2D>(@"Textures\SelectedtexBox");
            SelectedCrateTexture2 = Game.Content.Load<Texture2D>(@"Textures\SelectedtexBox2");
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
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
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
