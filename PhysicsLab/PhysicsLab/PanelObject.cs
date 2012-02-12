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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PanelObject : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region "Component"
        Panel panel;
        Panel spPanel;
        Panel springPanel;
        Body currentBody = null;
        #endregion

        #region "Boolean Field"
        public bool BodyAdded { get; private set; }
        #endregion

        #region "Old State"
        Vector3 oldPos = Vector3.Zero;
        Vector3 oldVel = Vector3.Zero;
        Vector3 oldAcc = Vector3.Zero;
        Vector3 oldHlf = Vector3.Zero;
        float oldC = 0f;
        float oldrestLength = 0f;
        float oldConstant = 0f;
        float oldRad = 0f;
        Body previousBody = null;
        Spring currentSpring = null;
        #endregion

        enum Type
        {
            Body, Spring
        };
        Type type = Type.Body;

        public PanelObject(Game game)
            : base(game)
        { }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            panel = new Panel(this.Game, new
                Vector2(Game.Window.ClientBounds.Width - 255,
                Game.Window.ClientBounds.Height - 285),
                250, 280);
            Game.Components.Add(panel);
            spPanel = new Panel(this.Game, new Vector2(10, 10), 140, 40);
            Game.Components.Add(spPanel);
            springPanel = new Panel(this.Game, 
                new Vector2(10, 
                    Game.Window.ClientBounds.Height - (((Lab)Game).basicLab.springs.Count * 20 + 35)),
                    100, ((Lab)Game).basicLab.springs.Count * 20 + 20);
            springPanel.Height += 15;
            Game.Components.Add(springPanel);
            base.Initialize();
        }

        public void Reset()
        {
            Initialize();
            if (previousBody != null)
                ((Drawable)previousBody).ShowPanel = false;
        }

        public void CreateDialog(Body body)
        {
            Reset();
            if (body as Ball != null)
                panel.AddField("Radius", ((Ball)body).radius);
            panel.AddField("Mass", body.Mass);
            if (body as Box != null)
            {
                panel.AddLabel("HalfSize", "HalfSize");
                panel.AddXYZ(((Box)body).HalfSize, "hlf");
            }
            panel.AddLabel("Position", "Position");
            panel.AddXYZ(body.Position, "pos");
            panel.AddLabel("Velocity", "Velocity");
            panel.AddXYZ(body.Velocity, "vel");
            panel.AddLabel("Acceleration", "Acceleration");
            panel.AddXYZ(body.LastFrameAcceleration, "acc");
            panel.AddOkButton();
            panel.AddCancelButton();
            panel.AddApplyButton();
            panel.Show = true;
            currentBody = body;
            previousBody = currentBody;
            BodyAdded = true;
            ((Drawable)body).ShowPanel = true;
            type = Type.Body;
        }

        public void CreateSpringPanel()
        {
            Reset();
            foreach (String str in ((Lab)Game).basicLab.springs.Keys)
                springPanel.AddButton(str, str);//, "MainPanel", springPanel.ButtonPosition, 70, 20);
            springPanel.Show = true;
            type = Type.Spring;
        }

        private void UpdateFeildPanel()
        {
            if (panel.Show)
            {
                if (type == Type.Body)
                {
                    if (oldPos != previousBody.Position)
                    {
                        panel.SetVlaue("posX", previousBody.Position.X);
                        panel.SetVlaue("posY", previousBody.Position.Y);
                        panel.SetVlaue("posZ", previousBody.Position.Z);
                        oldPos = previousBody.Position;
                    }
                    if (oldVel != previousBody.Velocity)
                    {
                        panel.SetVlaue("velX", previousBody.Velocity.X);
                        panel.SetVlaue("velY", previousBody.Velocity.Y);
                        panel.SetVlaue("velZ", previousBody.Velocity.Z);
                        oldVel = previousBody.Velocity;
                    }
                    if (oldAcc != previousBody.LastFrameAcceleration)
                    {
                        panel.SetVlaue("accX", previousBody.LastFrameAcceleration.X);
                        panel.SetVlaue("accY", previousBody.LastFrameAcceleration.Y);
                        panel.SetVlaue("accZ", previousBody.LastFrameAcceleration.Z);
                        oldAcc = previousBody.LastFrameAcceleration;
                    }
                    if (previousBody as Ball != null)
                    {
                        if (oldRad != ((Ball)previousBody).radius)
                        {
                            panel.SetVlaue("Radius", ((Ball)previousBody).radius);
                            oldRad = ((Ball)previousBody).radius;
                        }
                    }
                    else if (previousBody as Box != null)
                    {
                        if (oldHlf != ((Box)previousBody).HalfSize)
                        {
                            panel.SetVlaue("hlfX", ((Box)previousBody).HalfSize.X);
                            panel.SetVlaue("hlfY", ((Box)previousBody).HalfSize.Y);
                            panel.SetVlaue("hlfZ", ((Box)previousBody).HalfSize.Z);
                            oldHlf = ((Box)previousBody).HalfSize;
                        }
                    }
                }
                else if (type == Type.Spring)
                {
                    if (oldC != currentSpring.C)
                    {
                        panel.SetVlaue("C", currentSpring.C);
                        oldC = currentSpring.C;
                    }
                    if (oldrestLength != currentSpring.restLength)
                    {
                        panel.SetVlaue("RestLength", currentSpring.restLength);
                        oldrestLength = currentSpring.restLength;
                    }
                    if (oldConstant != currentSpring.springConstant)
                    {
                        panel.SetVlaue("Constant", currentSpring.springConstant);
                        oldConstant = currentSpring.springConstant;
                    }
                }
            }
        }

        public void SpringActivated()
        {
            spPanel.AddLabel("adsp", "Add Spring Activated");
            spPanel.Show = true;
        }


        private void PanelShow(MouseState mouse)
        {
            if (mouse.RightButton == ButtonState.Pressed)
            {
                if (currentBody != null)
                {
                    if (currentBody.InverseMass != 0)
                    {
                        if (((Lab)Game).WaitForOther)
                        {
                            ((Lab)Game).basicLab.CrateSpring(previousBody, currentBody);
                            ((Lab)Game).WaitForOther = false;
                            spPanel.Close();
                        }
                        else
                        {
                            if (previousBody != null)
                                ((Drawable)previousBody).ShowPanel = false;
                            Reset();
                            CreateDialog(currentBody);
                            ((Drawable)currentBody).ShowPanel = true;
                            previousBody = currentBody;
                        }
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            

        }

        private void ApplyChanges()
        {
            if (panel.Clicked)
            {
                if (type == Type.Body)
                {
                    previousBody.Mass = panel.GetVlaue("Mass");
                    previousBody.Position = new Vector3(panel.GetVlaue("posX"),
                        panel.GetVlaue("posY"), panel.GetVlaue("posZ"));
                    previousBody.SetVelocity(new Vector3(panel.GetVlaue("velX"),
                        panel.GetVlaue("velY"), panel.GetVlaue("velZ")));
                    previousBody.SetAcceleration(new Vector3(panel.GetVlaue("accX"),
                        panel.GetVlaue("accY"), panel.GetVlaue("accZ")));
                    if (previousBody as Ball != null)
                        ((Ball)previousBody).SetRadius(panel.GetVlaue("Radius"));
                    if (previousBody as Box != null)
                    {
                        ((Box)previousBody).SetHalfSize(
                            new Vector3(panel.GetVlaue("hlfX"), panel.GetVlaue("hlfY"), panel.GetVlaue("hlfZ")));
                    }
                }
                else if (type == Type.Spring)
                {
                    currentSpring.C = panel.GetVlaue("C");
                    currentSpring.restLength = panel.GetVlaue("RestLength");
                    currentSpring.springConstant = panel.GetVlaue("Constant");
                }
                panel.Applied = true;
            }
        }

        private void CheckPanelClosed()
        {
            if (previousBody != null && !panel.Show && BodyAdded)
            {
                ApplyChanges();
                ((Drawable)previousBody).ShowPanel = false;
                BodyAdded = false;
                previousBody = null;
            }
            if (type == Type.Spring)
                ApplyChanges();
        }

        void CrateSpringPanel()
        {
            if (springPanel.springClicked != "")
            {
                currentSpring = ((Lab)Game).basicLab.springs[springPanel.springClicked];
                Reset();
                panel.AddField("Constant", currentSpring.springConstant);
                panel.AddField("RestLength", currentSpring.restLength);
                panel.AddField("C", currentSpring.C);
                panel.AddOkButton();
                panel.AddCancelButton();
                panel.AddApplyButton();
                panel.Show = true;
            }
        }

        void SpringPanelPanel()
        {
            if (springPanel.springClicked != null)
            {
                CrateSpringPanel();
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            currentBody = ((Lab)Game).currentBody;
            PanelShow(mouse);
            if (panel.Show && !panel.Applied)
                ApplyChanges();
            UpdateFeildPanel();
            CheckPanelClosed();
            SpringPanelPanel();

            base.Update(gameTime);
        }
    }
}
