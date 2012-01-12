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
            using (Lab game = new Lab())
            {
                game.Run();
            }
        }
    }
#endif
}

