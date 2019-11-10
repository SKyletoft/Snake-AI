using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewSnake_GUI {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main () {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1();
            Application.Run(form);
            form.workerThread1.Abort();
            form.workerThread2.Abort();
            form.workerThread3.Abort();
        }
    }
}
