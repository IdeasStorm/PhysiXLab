using System;

namespace Test
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Collisions game = new Collisions())
            {
                //game.Run();
            }
            //FrictionWithCrate fr = new FrictionWithCrate();
            //fr.Run();
            FrictionTest fr = new FrictionTest();
            SpringTest sp = new SpringTest();
            //fr.Run();
            //sp.Run();

            PointToPointTest ptp = new PointToPointTest();
            ptp.Run();

            BoxAndBoxTest bab = new BoxAndBoxTest();
            //bab.Run();

            RestingTest rt = new RestingTest();
            //rt.Run();

            RodAndCableTest rodcable = new RodAndCableTest();
            //rodcable.Run();

            Bridge bridge = new Bridge();
            //bridge.Run();

            RopeTest rope = new RopeTest();
            //rope.Run();

            EnergyConservation b= new EnergyConservation();
            //b.Run();

        }
    }
#endif
}

