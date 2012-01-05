using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public abstract class Effect
    {
        protected float duration;

        public abstract void Update(float time);

    }
}
