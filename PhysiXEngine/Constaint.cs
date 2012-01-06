using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public abstract class Constaint : Effect
    {
        Body[] bodys = new Body[2];

        public Constaint(Body firstBody, Body SecondBody)
        {
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
