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
    public class BasicLab : Microsoft.Xna.Framework.Game
    {
        #region "Main Components"
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        #endregion

        #region "Physics Components"
        List<Body> bodys = new List<Body>();
        List<PhysiXEngine.Effect> effects = new List<PhysiXEngine.Effect>();
        ContactGenerator cg;
        #endregion

        #region "Graphics Components"
        public Camera camera { protected set; get; }
        Texture2D BallTexture;
        Model BallModel;
        Texture2D CrateTexture;
        Model CrateModel;
        public float speed = 1f;
        #endregion

        public BasicLab()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // TODO: Construct any child components here

        }

        public void AddBall(Ball ball) 
        {
            bodys.Add(ball);
            cg.AddBody(ball);
        }

        public void AddBall(float radius = 0.5f, float mass = 5f
            , Vector3 position = new Vector3())
        {
            Ball ball = new Ball(radius);
            ball.Mass = mass;
            ball.Position = position;
            ball.model = BallModel;
            ball.Texture = BallTexture;
            bodys.Add(ball);
            cg.AddBody(ball);
        }

        public void AddBall(Model model, Texture2D texture, 
            float radius = 0.5f, float mass = 5f, Vector3 position = new Vector3())
        {
            Ball ball = new Ball(radius);
            ball.Mass = mass;
            ball.Position = position;
            ball.model = model;
            ball.Texture = texture;
            bodys.Add(ball);
            cg.AddBody(ball);
        }


        public void AddCrate(Crate crate)
        {
            bodys.Add(crate);
            cg.AddBody(crate);
        }

        public void AddCrate(Vector3 halfSize, float mass = 5f, 
            Vector3 position = new Vector3())
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
            float mass = 5f, Vector3 position = new Vector3())
        {
            Crate crate = new Crate(halfSize);
            crate.Mass = mass;
            crate.Position = position;
            crate.model = model;
            crate.Texture = texture;
            bodys.Add(crate);
            cg.AddBody(crate);
        }


        public void AddEffect(PhysiXEngine.Effect effect)
        {
            effects.Add(effect);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization code here
            camera = new Camera(this, new Vector3(0, 0, 10f),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

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
            BallTexture = Content.Load<Texture2D>("texBall");
            BallModel = Content.Load<Model>("ball");
            CrateTexture = Content.Load<Texture2D>("texBox");
            CrateModel = Content.Load<Model>("box");
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
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            float duration = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            duration *= speed;
            foreach (Body bdy in bodys)
            {
                bdy.Update(duration);
            }
            foreach (PhysiXEngine.Effect ef in effects)
            {
                ef.Update(duration);
            }
            cg.Update(duration);

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
            foreach (Body bdy in bodys)
            {
                ((Drawable)bdy).Draw(camera);
            }

            base.Draw(gameTime);
        }
    }
}
