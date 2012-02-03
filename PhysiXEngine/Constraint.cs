using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public abstract class Constraint : Effect
    {
        public Body[] bodys { get; protected set; }

        public Constraint(Body firstBody, Body SecondBody)
        {
            bodys = new Body[2];
            this.bodys[0] = firstBody;
            this.bodys[1] = SecondBody;
        }

        public override void Update(float duration)
        {
            frameDuration = duration;
            Affect();
        }

        protected abstract void Affect();
    }
}
