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

            BoxAndBoxTest bab = new BoxAndBoxTest();
            //bab.Run();

            RodAndCableTest rodcable = new RodAndCableTest();
            rodcable.Run();
        }
    }
#endif
}

