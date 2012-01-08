using System;

namespace global
{
    class ContactData
    {
	    enum CollisionType
	    {
            None = 0x0,
            Box = 0x100,
            Sphere = 0x010,
            Plane = 0x001,
            Boxes = Box * 2,
            Spheres = Sphere * 2,
            BoxAndSphere = Box + Sphere,
            BoxAndPlane = Box + Plane
	    }
	
	    public void someMethod()
	    {
		
	    }
    }

    class Sphere : Collidable
    {
        private CollisionType t;
        List<Collidable> bodies;
        public Sphere()
        {
            t = bodies[1].getCollidableType() + this.getCollidableType();
        }
        
    }
}