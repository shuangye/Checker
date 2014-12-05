using Pkg_Checker.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Integration
{
    /// <summary>
    /// C:\Program Files\honeywell_eng\cm21_v2_2b2\bin\cm21.exe
    /// 1. Login: cm21.exe -user=%EID% -pass=%Password% -proj=B7E7 -sub=B7E7FMS -dir=C:\
    /// 2. Fetch: cm21.exe fetch XXX.TDF
    /// 3. Exit:  cm21.exe exit
    /// </summary>
    public class CM21
    {
        [DefaultValue(@"C:\Program Files\honeywell_eng\cm21_v2_2b2\bin\cm21.exe")]
        public String CM21ExePath { get; set; }
        public Process CM21Process { get; set; }
        public int ExitCode { get; set; }

        private const int maxWaitingTime = 3 * 60 * 1000;  // 3 min

#warning Search registry for installed apps.
        public CM21(String EID, String PWD, String proj, String subProj, String outputPath)
        {
            CM21ExePath = @"C:\Program Files\honeywell_eng\cm21_v2_2b2\bin\cm21.exe";
            if (!File.Exists(CM21ExePath))               
               // query registry
               throw new FileNotFoundException("CM21 executable is not found.");           

            CM21Process = new Process();
            CM21Process.StartInfo.FileName = CM21ExePath;
            CM21Process.StartInfo.Arguments = String.Format(@"-user={0} -pass={1} -proj={2} -sub={3} -def={4}",
                EID, PWD, proj, subProj, outputPath);
            CM21Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            CM21Process.Start();
            if (CM21Process.WaitForExit(1))
            {
                // ExitCode == 0 means logged in successfully
                if (CM21Process.HasExited && 0 != CM21Process.ExitCode)
                    throw new ApplicationException(String.Format("CM21 login failed with exit code {0}.", CM21Process.ExitCode));
            }
            else
            {
                if (!CM21Process.HasExited)
                    CM21Process.Kill();  // HasExited property will become true after calling Kill()
                throw new ApplicationException("CM21 process is stuck or exited unexpectly.");
            }
        }

         ~CM21()
        {
            CM21Process.Close();
        }

        // report scr/p1=SCR_NUMBER
        public void FetchSCRReport(IEnumerable<SCRReport> reports)
        {            
           foreach (SCRReport report in reports)
           {
               // CM21Process.HasExited: is previous command has finished?
               if (null != CM21Process && CM21Process.HasExited)
               {
                   String scrNumber = report.SCRNumber.ToString("0.00");
                   CM21Process.StartInfo.Arguments = String.Format("REPORT SCR/P1=\"{0}\"/OUTPUT={1}", scrNumber, scrNumber);
                   CM21Process.Start();
                   if (!CM21Process.WaitForExit(maxWaitingTime))
                       CM21Process.Kill();
               }
           }            
        }

        // wait for process to exit: (Process.Exit event)
        // Process.Exit += () => {}
        public void Exit()
        {
            if (null != CM21Process)
            {
                CM21Process.StartInfo.Arguments = "EXIT";
                CM21Process.Start();
                if (!CM21Process.WaitForExit(maxWaitingTime))
                    CM21Process.Kill();
                else
                    CM21Process.Close();
            }
        }
    }
}
