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
        //List<Panel> panels = new List<Panel>();
        //Dictionary<Panel, Body> panels = new Dictionary<Panel, Body>();
        Panel panel;

        Body currentBody = null;
        Body previousBody = null;

        bool BodyAdded = false;

        Vector3 oldPos = Vector3.Zero;
        Vector3 oldVel = Vector3.Zero;
        Vector3 oldAcc = Vector3.Zero;
        Vector3 oldHlf = Vector3.Zero;
        float oldRad = 0f;

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
            base.Initialize();
        }

        public void CreateDialog(Body body)
        {
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
            panel.AddXYZ(body.Acceleration, "acc");
            panel.AddOkButton();
            panel.AddCancelButton();
            panel.AddApplyButton();
            panel.Show = true;
            currentBody = body;
            previousBody = currentBody;
            BodyAdded = true;
        }

        private void UpdateFeildPanel()
        {
            if (panel.Show)
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
                if (oldAcc != previousBody.Acceleration)
                {
                    panel.SetVlaue("accX", previousBody.Acceleration.X);
                    panel.SetVlaue("accY", previousBody.Acceleration.Y);
                    panel.SetVlaue("accZ", previousBody.Acceleration.Z);
                    oldAcc = previousBody.Acceleration;
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
        }


        private void PanelShow(MouseState mouse)
        {
            if (mouse.RightButton == ButtonState.Pressed)
            {
                if (previousBody != null)
                    ((Drawable)previousBody).ShowPanel = false;
                if (currentBody.InverseMass != 0)
                {
                    //RightMouseClicked = true;
                    Initialize();
                    CreateDialog(currentBody);
                    ((Drawable)currentBody).ShowPanel = true;
                    previousBody = currentBody;
                }
                
            }
        }

        private void ApplyChanges()
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
            panel.Applied = true;
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

            base.Update(gameTime);
        }
    }
}
