using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_AI {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main () {
            Console.WriteLine("init'd");

            var AINetwork = new AI.Evolution();
            AINetwork.init();
            var frameBuffer = new System.Drawing.Bitmap(20, 20);
            do {
                var start = DateTime.Now;
                for (var i = 0; i < 1000; i++) {
                    AINetwork.cycle(ref frameBuffer, true);
                    Console.WriteLine("Ended generation");
                }
                Console.WriteLine("Ended 1000 gens; {0}", (DateTime.Now - start));
            } while (Console.ReadKey(true).Key == ConsoleKey.Enter);

            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            */
            Console.Write("Ended\nPress any key to quit...");
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }
}
