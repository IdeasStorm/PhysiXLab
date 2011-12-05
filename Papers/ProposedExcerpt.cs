// Proposed Excerpts of Our Physics Engine
//File: World.cs
class World {
	CollisionGenerator CG;
	init() {
		CG = new CollisionGenerator(this);
		Body b1 = new Body(this);
		b1.addEffect(CG);
	}
	update() {
		CG.init(); // calculating Collisions Tree and to make Collision List
		foreach(this.components as body){
			body.update();
		}
		
	}
	
}

//File: Body.cs

class Body{
	update(){
		foreach (this.Effects as effect) {
			effect.affect(this); // this will apply the impulses according to contact information calculated in CollisionGenerator.init();
		}
	}
}
