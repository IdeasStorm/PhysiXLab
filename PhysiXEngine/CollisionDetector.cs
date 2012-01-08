using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    class CollisionDetector
    {
        public BoundingBox World { private set; get; }
        public LinkedList<Collidable> Shapes { private set; get; }
        public List<ContactData> Detections { private set; get; }

        public CollisionDetector(BoundingBox World, LinkedList<Collidable> Shapes)
        {
            this.World = World;
            this.Shapes = Shapes;
            Detections = new List<ContactData>(Shapes.Count);   // Shapes.Count is sth arbitrary.
        }

        public CollisionDetector(LinkedList<Collidable> Shapes)
        {
            this.Shapes = Shapes;
        }



    }
}
