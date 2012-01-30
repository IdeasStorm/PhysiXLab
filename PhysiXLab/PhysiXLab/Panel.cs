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


namespace App
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Panel : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Panel(Game game, Point position, int width, int height)
            : base(game)
        {
            // TODO: Construct any child components here
            Buttons = new List<Button>();
            TextBoxes = new List<TextBox>();
            Lables = new List<Label>();
            Fields = new Dictionary<String, float>();
            Panels = new Dictionary<String, Form>();

            this.Position = position;
            this.Width = width;
            this.Height = height;
            _ButtonPosition = new Point(position.X + 15, position.Y + 15);
            _LabelPosition = new Point(position.X + 15, position.Y + 15);
            _TexBoxPosition = new Point(position.X + 95, position.Y + 15);
        }

        protected Point Position;
        protected int Width;
        protected int Height;

        private Form Frm;

        private Texture2D PanelTexture;
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

        public bool Show = false;

        private Dictionary<String, float> Fields;
        private Dictionary<String, Form> Panels;

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

        void OkButtonClicked(Control sender)
        {
            foreach (TextBox tb in TextBoxes)
            {
                float val;
                float.TryParse(tb.Text, out val);
                Fields[tb.Name] = val;
            }
            Show = false;
        }

        void CancelButtonClicked(Control sender)
        {
            Show = false;
        }

        public void AddButton(String buttonName, String buttonText, String panelName,
            Point position, int width, int height)
        {

            Button myButton = new Button(buttonName, buttonText,
                new Rectangle(position.X, position.Y, width, height), ButtonTexture, Font, Color.Black);
            Buttons.Add(myButton);
            if (panelName == "MainPanel")
                Frm.AddControl(myButton);
            else
            {
                Form myFrm = null;
                Panels.TryGetValue(panelName, out myFrm);
                if (myFrm != null)
                {
                    myFrm.AddControl(myButton);
                }
            }
            //Frm.AddControl(Buttons.Last<Button>());
        }

        public void AddButton(String buttonName = "Btn", String buttonText = "Button",
            String PanelName = "MainPanel", int width = 70, int height = 20)
        {
            /*
            Buttons.Add(new Button(buttonName, buttonText, 
                new Rectangle(_ButtonPosition.X, _ButtonPosition.Y, width, height), 
                ButtonTexture, Font, Color.Black));*/
            //Frm.AddControl(Buttons.Last<Button>());
            if (PanelName == "MainPanel")
            {
                Button myButton = new Button(buttonName, buttonText,
                new Rectangle(_ButtonPosition.X, _ButtonPosition.Y, width, height),
                ButtonTexture, Font, Color.Black);
                Buttons.Add(myButton);
                Frm.AddControl(myButton);
            }
            else
            {
                AddButton(buttonName, buttonText, PanelName, _ButtonPosition, width, height);
            }
            //Buttons.Last<Button>().onClick += new EHandler(ButtonClicked);
            _ButtonPosition.Y += 25;
        }

        public void AddTextBox(String textBoxName, String value, String PanelName,
            int maxLength, Point position, int width, int height)
        {
            TextBox myTextBox = new TextBox(textBoxName, value, maxLength,
                new Rectangle(position.X, position.Y, width, height),
                TextBoxTexture, Font, Color.Black);
            TextBoxes.Add(myTextBox);
            //Frm.AddControl(TextBoxes.Last<TextBox>());
            Form myFrm = null;
            Panels.TryGetValue(PanelName, out myFrm);
            if (myFrm != null)
            {
                myFrm.AddControl(myTextBox);
            }
        }

        public void AddTextBox(String textBoxName = "txt", String value = "", String PanelName = "MainPanel",
            int maxLength = 5, int width = 70, int height = 20)
        {

            //Frm.AddControl(TextBoxes.Last<TextBox>());
            if (PanelName == "MainPanel")
            {
                TextBox myText = new TextBox(textBoxName, value, maxLength,
                new Rectangle(_TexBoxPosition.X, _TexBoxPosition.Y, width, height),
                TextBoxTexture, Font, Color.Black);
                TextBoxes.Add(myText);
                Frm.AddControl(myText);
            }
            else
            {
                AddTextBox(textBoxName, value, PanelName, maxLength, _TexBoxPosition, width, height);
            }
            _TexBoxPosition.Y += 25;
        }

        public void AddLabel(String labelName, String labelText, Point position, String PanelName)
        {
            Label myLabel = new Label(labelName, labelName, new Vector2(position.X, position.Y),
                Font, Color.Black, 15, 2);
            Lables.Add(myLabel);
            //Frm.AddControl(Lables.Last<Label>());
            Form myFrm = null;
            Panels.TryGetValue(PanelName, out myFrm);
            if (myFrm != null)
            {
                myFrm.AddControl(myLabel);
            }
        }

        public void AddLabel(String labelName = "lb", String labelText = "Label", String PanelName = "MainPanel")
        {
            if (PanelName == "MainPanel")
            {
                Label myLabel = new Label(labelName, labelName, new Vector2(_LabelPosition.X, _LabelPosition.Y),
                Font, Color.Black, 15, 2);
                Lables.Add(myLabel);
                Frm.AddControl(myLabel);
            }
            else
            {
                AddLabel(labelName, labelText, _LabelPosition, PanelName);
            }
            /*
            Lables.Add(new Label(labelName, labelText,
                new Vector2(_LabelPosition.X, _LabelPosition.Y), Font, Color.White, 15, 2));
            Frm.AddControl(Lables.Last<Label>());*/
            _LabelPosition.Y += 25;
        }

        public void AddField(String name, float value, String panelName = "MainPanel")
        {
            Fields.Add(name, value);
            AddLabel(name, name, panelName);
            //_TexBoxPosition.X += 80;
            AddTextBox(name, value.ToString(), panelName);
        }

        public void AddPanel(String panelName, Point position, int width, int height)
        {
            Panels.Add(panelName, new Form(panelName, "",
                new Rectangle(position.X, position.Y, width, height),
                PanelTexture, Font, Color.White));
        }

        public void AddOkButton(String panelName = "MainPanel")
        {
            AddButton("OK", "OK", panelName, new Point(Width - 160, Height - 30), 70, 20);
            Buttons.Last<Button>().onClick += new EHandler(OkButtonClicked);
        }

        public void AddCancelButton(String panelName = "MainPanel")
        {
            AddButton("Cancel", "Cancel", panelName, new Point(Width - 80, Height - 30), 70, 20);
            Buttons.Last<Button>().onClick += new EHandler(CancelButtonClicked);
        }

        public float GetVlaue(String fieldName)
        {
            float value = 0f;
            Fields.TryGetValue(fieldName, out value);
            return value;
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
            ButtonTexture = Game.Content.Load<Texture2D>("texButton");
            TextBoxTexture = Game.Content.Load<Texture2D>("texTextBox");
            PanelTexture = Game.Content.Load<Texture2D>("texForm");
            Font = Game.Content.Load<SpriteFont>("Arial");
            Frm = new Form("MyForm", "",
                new Rectangle(Position.X, Position.Y, Width, Height),
                PanelTexture, Font, Color.Black);
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
            if (Show)
            {
                //Frm.Update(mouseState, keyboardState);
                foreach (Form frm in Panels.Values)
                {
                    //if (frm != null)
                    frm.Update(mouseState, keyboardState);
                }
                Frm.Update(mouseState, keyboardState);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {

            // TODO: Add your drawing code here
            if (Show)
            {
                spriteBatch.Begin();
                //Frm.Draw(spriteBatch);
                Frm.Draw(spriteBatch);
                foreach (Form frm in Panels.Values)
                {
                    frm.Draw(spriteBatch);
                }
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
