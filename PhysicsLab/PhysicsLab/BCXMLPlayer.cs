using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysiXEngine;
using PhysiXEngine.Helpers;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsLab
{
    /// <summary>
    /// XML Player that takes into consideration Balls and Crates.
    /// </summary>
    public class BCXMLPlayer : XMLPlayer
    {
        public Model BallModel { set; get; }
        public Model CrateModel { set; get; }
        public Texture2D BallTexture { set; get; }
        public Texture2D CrateTexture { set; get; }
        public BCXMLPlayer(List<Body> Bodies, string FilePath)
            : base(Bodies, FilePath)
        {
        }

        public override Body NewBody(string S)
        {
            string BodyType = S.Split('|')[0];

            if (BodyType.Equals("Ball"))
                return new Ball(BallModel, BallTexture, S);
            else if (BodyType.Equals("Crate"))
                return new Crate(CrateModel, CrateTexture, S);
            else return base.NewBody(S);
        }
    }
}
