using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Helpers
{
    public class Proc
    {
        public static void Kill(String procName)
        {
            if (String.IsNullOrWhiteSpace(procName))
                return;

            Process[] targetProcesses = Process.GetProcessesByName(procName);
            if (null != targetProcesses && targetProcesses.Count() > 0)
            {
                try
                {
                    foreach (var item in targetProcesses)
                        item.Kill();
                }
                catch
                {
                    // do nothing
                }
            }
        }
    }
}
