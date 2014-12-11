#undef _CLI
using Pkg_Checker.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Pkg_Checker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static String AppName { get { return "Review Package Checker"; } }

#if _CLI
        [DllImport("kernel32.dll")] 
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")] 
        static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
#endif
        
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWin(args));

#if _CLI
            if (null != args)
            {
                int checkedFileCount = 0;
                int totalFileCount = 0;
                ConsoleWorker worker = new ConsoleWorker();

                // redirect console output to parent process;
                // must be before any calls to Console.WriteLine()
                AttachConsole(-1);

                switch(args.Length)
                {                       
                    case 3:
                        checkedFileCount = worker.CheckPackage(args[1], args[2], out totalFileCount, null, null, null);
                        break;
                    case 6:
                        checkedFileCount = worker.CheckPackage(args[1], args[2], out totalFileCount, args[3], args[4], args[5]);
                        break;
                    case 7:
                        Match match = Regex.Match(args[6], @"\d+");
                        if (match.Success)
                            checkedFileCount = worker.CheckPackage(args[1], args[2], out totalFileCount, args[3], args[4], args[5], int.Parse(match.Value));
                        else
                            checkedFileCount = worker.CheckPackage(args[1], args[2], out totalFileCount, args[3], args[4], args[5]);
                        break;
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                    default:
                        PrintUsage(args);
                        break;
                }
                
                if (totalFileCount <= 0)
                    Console.WriteLine(@"No .PDF files found under the specified path.");
                else
                    Console.WriteLine(@"Checked {0} of {1} file(s).", checkedFileCount, totalFileCount);
                                
                FreeConsole();                
            }
                           
            else  // started with GUI
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWin());
            } 
#endif
        }

        static void PrintUsage(String[] args)
        {
            Console.WriteLine(@"Usage (without CM21): {0} path/to/pdf path/to/check/result", args[0]);
            Console.WriteLine(@"Usage (with CM21): {0} path/to/pdf path/to/check/result EID CM21PWD SCR/Report/Download/Path [timeout in seconds]",
                args[0]);
        }
    }
}
