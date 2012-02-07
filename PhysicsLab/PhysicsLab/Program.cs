using System;

namespace PhysicsLab
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //using (Lab game = new Lab())
            //{
            //    game.Run();
            //}
            BCXMLPlayerTest other = new BCXMLPlayerTest();
            //other.Run();
            Lab O = new Lab();
            O.Run();
        }
    }
#endif
}

