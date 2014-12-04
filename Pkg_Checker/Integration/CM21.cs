using Pkg_Checker.Entities;
using System;
using System.Collections.Generic;
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
        public String CM21ExePath { get; set; }
        public Process CM21Process { get; set; }

        private const int maxWaitingTime = 3 * 60 * 1000;  // 3 min

        public CM21(String exePath, String EID, String PWD, String proj, String subProj, String cwd)
        {
            if (String.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
            {
                String defaultPath = @"C:\Program Files\honeywell_eng\cm21_v2_2b2\bin\cm21.exe";
                if (File.Exists(defaultPath))
                    CM21ExePath = defaultPath;
                else
                    throw new FileNotFoundException("CM21 executable is not found.");
            }
            else
                CM21ExePath = exePath;

            CM21Process = new Process();
            CM21Process.StartInfo.FileName = CM21ExePath;
            CM21Process.StartInfo.Arguments = String.Format(@"-user={0} -pass={1} -proj={2} -sub={3} -dir={4}", EID, PWD, proj, subProj, cwd);
            CM21Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            CM21Process.Start();
        }

         ~CM21()
        {
            CM21Process.Close();
        }

        // report scr/p1=SCR_NUMBER
        public void FetchSCRReport(List<SCRReport> reports)
        {
            foreach (SCRReport report in reports)
            {
                CM21Process.StartInfo.Arguments = String.Format("REPORT SCR/P1=\"{0}\"/OUTPUT={1}", report.SCRNumber, report.SCRNumber);
                CM21Process.Start();
                if (!CM21Process.WaitForExit(maxWaitingTime))
                    CM21Process.Kill();
            }
        }


        // wait for process to exit: (Process.Exit event)
        // Process.Exit += () => {}
        public void Exit()
        {
            CM21Process.StartInfo.Arguments = "EXIT";
            CM21Process.Start();
            if (!CM21Process.WaitForExit(maxWaitingTime))
                CM21Process.Kill();  // kill the hang process
        }
    }
}
