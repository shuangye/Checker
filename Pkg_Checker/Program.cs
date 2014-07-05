using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Pkg_Checker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // read configuration at session start to avoid reading configuration mutiple times
            // consider the Singleton design pattern
            // Pkg_Checker.Configuration.ReadCfg();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWin());            
        }
    }
}
