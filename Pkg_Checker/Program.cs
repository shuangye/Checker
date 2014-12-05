using Pkg_Checker.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pkg_Checker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static String AppName { get { return "Review Package Checker"; } }

        [DllImport("kernel32.dll")] 
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")] 
        static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();            
            if (null != args && args.Length > 1)
            {
                // redirect console output to parent process;
                // must be before any calls to Console.WriteLine()
                AttachConsole(-1);

                // args: app path/to/pdf path/to/check/result
                if (3 == args.Length)
                { 
                    int checkedFileCount = 0;
                    int totalFileCount = 0;
                    ConsoleWorker worker = new ConsoleWorker();
                    checkedFileCount = worker.StartChecking(args[1], args[2], out totalFileCount);
                    if (0 == totalFileCount)
                        Console.WriteLine(@"No .PDF files found under the specified path.");
                    else
                        Console.WriteLine(@"Checked {0} of {1} file(s).", checkedFileCount, totalFileCount);
                }
                else                
                    Console.WriteLine(@"Usage: {0} path/to/pdf path/to/check/result", args[0]);
                                
                FreeConsole();                
            }
                           
            else  // started with GUI
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWin());
            }
        }
    }
}
