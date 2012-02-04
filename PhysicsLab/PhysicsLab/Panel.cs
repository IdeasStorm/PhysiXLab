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


namespace PhysicsLab
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Panel : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Panel(Game game, Vector2 position, int width, int height)
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
            _ButtonPosition = new Vector2(15, 15);
            _LabelPosition = new Vector2(15,  15);
            _TexBoxPosition = new Vector2(95, 15);
        }

        protected Vector2 Position;
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Form Frm;

        private Texture2D PanelTexture;
        private Texture2D ButtonTexture;
        private Texture2D TextBoxTexture;
        private SpriteFont Font;

        private List<Button> Buttons;
        private List<TextBox> TextBoxes;
        private List<Label> Lables;

        private Vector2 _ButtonPosition;
        public Vector2 ButtonPosition { get { return _ButtonPosition; } set { _ButtonPosition = value; } }
        private Vector2 _LabelPosition;
        public Vector2 LabelPosition { get { return _LabelPosition; } set { _LabelPosition = value; } }
        private Vector2 _TexBoxPosition;
        public Vector2 TexBoxPosition { get { return _TexBoxPosition; } set { _TexBoxPosition = value; } }

        public bool Show = false;
        public bool Applied = false;

        private Dictionary<String, float> Fields;
        private Dictionary<String, Form> Panels;

        SpriteBatch spriteBatch;
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }

        void ApplyButtonClicked(Control sender)
        {
            foreach (TextBox tb in TextBoxes)
            {
                float val;
                float.TryParse(tb.Text, out val);
                Fields[tb.Name] = val;
            }
            Applied = true;
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

        void Changed(Control sender)
        {
            Applied = false;
        }

        public void NewForm()
        {
            Frm = new Form("MyForm", "",
                new Rectangle((int)Position.X, (int)Position.Y, Width, Height),
                PanelTexture, Font, Color.Black);
        }

        public void AddButton(String buttonName, String buttonText, String panelName,
            Vector2 position, int width, int height)
        {

            Button myButton = new Button(buttonName, buttonText,
                new Rectangle((int)position.X, (int)position.Y, width, height), ButtonTexture, Font, Color.Black);
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
        }

        public void AddButton(String buttonName = "Btn", String buttonText = "Button",
            String PanelName = "MainPanel", int width = 70, int height = 20)
        {
            if (PanelName == "MainPanel")
            {
                Button myButton = new Button(buttonName, buttonText,
                new Rectangle((int)_ButtonPosition.X, (int)_ButtonPosition.Y, width, height),
                ButtonTexture, Font, Color.Black);
                Buttons.Add(myButton);
                Frm.AddControl(myButton);
            }
            else
            {
                AddButton(buttonName, buttonText, PanelName, _ButtonPosition, width, height);
            }
            _ButtonPosition.Y += 25;
        }

        public void AddTextBox(String textBoxName, String value, String PanelName,
            int maxLength, Vector2 position, int width, int height)
        {
            TextBox myTextBox = new TextBox(textBoxName, value, maxLength,
                new Rectangle((int)position.X, (int)position.Y, width, height),
                TextBoxTexture, Font, Color.Black);
            TextBoxes.Add(myTextBox);
            if (PanelName == "MainPanel")
                Frm.AddControl(myTextBox);
            {
                Form myFrm = null;
                Panels.TryGetValue(PanelName, out myFrm);
                if (myFrm != null)
                {
                    myFrm.AddControl(myTextBox);
                }
            }
            TextBoxes.Last<TextBox>().onChange += new EHandler(Changed);
        }

        public void AddTextBox(String textBoxName = "txt", String value = "", String PanelName = "MainPanel",
            int maxLength = 5, int width = 70, int height = 20)
        {
            if (PanelName == "MainPanel")
            {
                TextBox myText = new TextBox(textBoxName, value, maxLength,
                new Rectangle((int)_TexBoxPosition.X, (int)_TexBoxPosition.Y, width, height),
                TextBoxTexture, Font, Color.Black);
                TextBoxes.Add(myText);
                Frm.AddControl(myText);
                TextBoxes.Last<TextBox>().onChange += new EHandler(Changed);
            }
            else
            {
                AddTextBox(textBoxName, value, PanelName, maxLength, _TexBoxPosition, width, height);
            }
            
            _TexBoxPosition.Y += 25;
        }

        public void AddLabel(String labelName, String labelText, Vector2 position, String PanelName)
        {
            Label myLabel = new Label(labelName, labelName, new Vector2(position.X, position.Y),
                Font, Color.Black, 15, 2);
            Lables.Add(myLabel);
            if (PanelName == "MainPanel")
                Frm.AddControl(myLabel);
            else
            {
                Form myFrm = null;
                Panels.TryGetValue(PanelName, out myFrm);
                if (myFrm != null)
                {
                    myFrm.AddControl(myLabel);
                }
            }
        }

        public void AddLabel(String labelName = "lb", String labelText = "Label", String PanelName = "MainPanel")
        {
            if (PanelName == "MainPanel")
            {
                Label myLabel = new Label(labelName, labelText, new Vector2(_LabelPosition.X, _LabelPosition.Y),
                Font, Color.Black, 15, 2);
                Lables.Add(myLabel);
                Frm.AddControl(myLabel);
            }
            else
            {
                AddLabel(labelName, labelText, _LabelPosition, PanelName);
            }
            _LabelPosition.Y += 25;
        }

        public void AddField(String name, float value, String panelName = "MainPanel")
        {
            Fields.Add(name, value);
            AddLabel(name, name, panelName);
            AddTextBox(name, value.ToString(), panelName);
        }

        public void AddPanel(String panelName, Vector2 position, int width, int height)
        {
            Panels.Add(panelName, new Form(panelName, "",
                new Rectangle((int)position.X, (int)position.Y, width, height),
                PanelTexture, Font, Color.White));
        }

        public void AddXYZ(Vector3 vec, String type)
        {
            _LabelPosition.X += 15f;
            AddLabel("X: ", type + "X", new Vector2(LabelPosition.X, LabelPosition.Y), "MainPanel");
            AddTextBox(type + "X", String.Format("{0:0.0}", vec.X), "MainPanel", 5, 
                new Vector2(LabelPosition.X + 15, LabelPosition.Y - 0.5f), 30, 20);
            Fields.Add(type + "X", vec.X);
            ////////////////////////////////////////////////////////////////////////////////////////////
            _LabelPosition.X += 60f;
            AddLabel("Y: ", type + "Y", new Vector2(LabelPosition.X, LabelPosition.Y), "MainPanel");
            AddTextBox(type + "Y", String.Format("{0:0.0}", vec.Y), "MainPanel", 5,
                new Vector2(LabelPosition.X + 15, LabelPosition.Y - 0.5f), 30, 20);
            Fields.Add(type + "Y", vec.Y);
            ////////////////////////////////////////////////////////////////////////////////////////////
            _LabelPosition.X += 60f;
            AddLabel("Z: ", type + "Z", new Vector2(LabelPosition.X, LabelPosition.Y), "MainPanel");
            AddTextBox(type + "Z", String.Format("{0:0.0}", vec.Z), "MainPanel", 5,
                new Vector2(LabelPosition.X + 15, LabelPosition.Y - 0.5f), 30, 20);
            Fields.Add(type + "Z", vec.Z);
            ////////////////////////////////////////////////////////////////////////////////////////////
            _LabelPosition.X = 15f;
            _LabelPosition.Y += 25f;
        }

        public void ClearAll()
        {
            TextBoxes.Clear();
            Buttons.Clear();
            Lables.Clear();
            Fields.Clear();
            Panels.Clear();
            NewForm();
        }

        public void ResetAll()
        {
            _ButtonPosition = new Vector2(15, 15);
            _LabelPosition = new Vector2(15, 15);
            _TexBoxPosition = new Vector2(95, 15);
        }

        public void AddApplyButton(String panelName = "MainPanel")
        {
            AddButton("Apply", "Apply", "MainPanel", new Vector2(Width - 80 * 3, Height - 30), 70, 20);
            Buttons.Last<Button>().onClick += new EHandler(ApplyButtonClicked);
        }

        public void AddOkButton(String panelName = "MainPanel")
        {
            AddButton("OK", "OK", panelName, new Vector2(Width - 80 * 2, Height - 30), 70, 20);
            Buttons.Last<Button>().onClick += new EHandler(OkButtonClicked);
        }

        public void AddCancelButton(String panelName = "MainPanel")
        {
            AddButton("Cancel", "Cancel", panelName, new Vector2(Width - 80, Height - 30), 70, 20);
            Buttons.Last<Button>().onClick += new EHandler(CancelButtonClicked);
        }

        public float GetVlaue(String fieldName)
        {
            float value = 0f;
            Fields.TryGetValue(fieldName, out value);
            return value;
        }

        public void SetVlaue(String fieldName, float value)
        {
            foreach (TextBox tb in TextBoxes)
            {
                if (tb.Name == fieldName)
                    tb.Text = String.Format("{0:0.0}", value);
            }
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
            ButtonTexture = Game.Content.Load<Texture2D>(@"GUI\texButton");
            TextBoxTexture = Game.Content.Load<Texture2D>(@"GUI\texTextBox");
            PanelTexture = Game.Content.Load<Texture2D>(@"GUI\texForm");
            Font = Game.Content.Load<SpriteFont>(@"GUI\Arial");
            NewForm();
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
