﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicsLab
{
    public interface Drawable
    {
        void Draw(Camera camera);

        bool Selected { get; set; }

        bool ShowPanel { get; set; }
    }
}
