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
using FuchsGUI;


namespace Test
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Penal : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Penal(Game game, Point position, int width, int height)
            : base(game)
        {
            // TODO: Construct any child components here
            Buttons = new List<Button>();
            TextBoxes = new List<TextBox>();
            Lables = new List<Label>();

            this.Position = position;
            this.Width = width;
            this.Height = height;
            _ButtonPosition = new Point(position.X + 15, position.Y + 15);
            _LabelPosition = new Point(position.X + 15, position.Y + 15);
            _TexBoxPosition = new Point(position.X + 15, position.Y + 15);
        }

        private Point Position;
        private int Width;
        private int Height;

        private Form frm;

        private Texture2D PenalTexture;
        private Texture2D ButtonTexture;
        private Texture2D TextBoxTexture;
        private SpriteFont Font;

        private List<Button> Buttons;
        private List<TextBox> TextBoxes;
        private List<Label> Lables;

        private Point _ButtonPosition;
        public Point ButtonPosition { get { return _ButtonPosition; } set { _ButtonPosition = value; } }
        private Point _LabelPosition;
        public Point LabelPosition { get { return _LabelPosition; } set { _LabelPosition = value; } }
        private Point _TexBoxPosition;
        public Point TexBoxPosition { get { return _TexBoxPosition; } set { _TexBoxPosition = value; } }

        SpriteBatch spriteBatch;
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            
            base.Initialize();
        }
        /*
        void ButtonClicked(Control sender)
        {
            Lables.Last<Label>().Text = sender.Name;
        }
         */

        public void AddButton(String buttonName, Point Position, int Width, int Height)
        {
            Buttons.Add(new Button("bt" + (Buttons.Count + 1), buttonName,
                new Rectangle(50, 50, 100, 50), ButtonTexture, Font, Color.Black));
            frm.AddControl(Buttons.Last<Button>());
        }

        public void AddButton(String buttonName = "Button", int width = 70, int height = 20)
        {
            Buttons.Add(new Button("bt" + (Buttons.Count + 1), buttonName,
                new Rectangle(_ButtonPosition.X, _ButtonPosition.Y, width, height), 
                ButtonTexture, Font, Color.Black));
            frm.AddControl(Buttons.Last<Button>());
            //Buttons.Last<Button>().onClick += new EHandler(ButtonClicked);
            _ButtonPosition.Y += 25;
        }

        public void AddTextBox(String value, int maxLength, Point position, int width, int height)
        {
            TextBoxes.Add(new TextBox("txt" + (TextBoxes.Count+1), value, maxLength, 
                new Rectangle(position.X, position.Y, width, height), 
                TextBoxTexture, Font, Color.Black));
            frm.AddControl(TextBoxes.Last<TextBox>());
        }

        public void AddTextBox(String value = "", int maxLength = 5, int width = 70, int height = 20)
        {
            TextBoxes.Add(new TextBox("txt" + (TextBoxes.Count + 1), value, maxLength,
                new Rectangle(_TexBoxPosition.X, _TexBoxPosition.Y, width, height),
                TextBoxTexture, Font, Color.Black));
            frm.AddControl(TextBoxes.Last<TextBox>());
            _TexBoxPosition.Y += 25;
        }

        public void AddLabel(String labelName, Point position)
        {
            Lables.Add(new Label("lb" + (Lables.Count + 1), labelName, 
                new Vector2(position.X, position.Y), Font, Color.White, 15, 2));
            frm.AddControl(Lables.Last<Label>());
        }

        public void AddLabel(String labelName = "Label")
        {
            Lables.Add(new Label("lb" + (Lables.Count + 1), labelName,
                new Vector2(_LabelPosition.X, _LabelPosition.Y), Font, Color.White, 15, 2));
            frm.AddControl(Lables.Last<Label>());
            _LabelPosition.Y += 25;
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
            ButtonTexture = Game.Content.Load<Texture2D>("GUI\texButton");
            TextBoxTexture = Game.Content.Load<Texture2D>("GUI\texTextBox");
            PenalTexture = Game.Content.Load<Texture2D>("GUI\texForm");
            Font = Game.Content.Load<SpriteFont>("GUI\Arial");
            frm = new Form("MyForm", "", 
                new Rectangle(Position.X, Position.Y, Width, Height), 
                PenalTexture, Font, Color.Black);
            
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            frm.Update(mouseState, keyboardState);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            frm.Draw(spriteBatch);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
