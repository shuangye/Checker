using Pkg_Checker.Implementations;
using Pkg_Checker.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pkg_Checker.Helpers
{
    [Obsolete]
    public class ConsoleWorker
    {

        public int CheckPackage(String pdfPath, String resultPath, out int totalFileCount,
            String EID, String CM21Password, String SCRReportDownloadPath, int timeout = 60)
        {
            bool checkWithCM21 = !String.IsNullOrWhiteSpace(EID);
            int checkedFileCount = 0;
            StreamWriter SW = null;
            bool errorOccurred = false;            
            totalFileCount = 0;

            if (checkWithCM21)
            {
                if (String.IsNullOrWhiteSpace(CM21Password))
                {
                    Console.WriteLine("Invalid CM21 credential.");
                    return checkedFileCount;
                }

                try
                {
                    if (!Directory.Exists(SCRReportDownloadPath))
                        Console.WriteLine(String.Format("Specified directory {0} is not found.", SCRReportDownloadPath));
                    Directory.GetAccessControl(SCRReportDownloadPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot write to the SCR report download path: " + ex.Message);
                    return checkedFileCount;
                }

                EID = EID.Trim().ToUpper();                
            }

            List<String> fileNames = FSWalker.Walk(pdfPath, @"*.PDF", false, ref errorOccurred);
            if (errorOccurred)
                return 0;

            totalFileCount = fileNames.Count;
            try { SW = new StreamWriter(resultPath, false, Encoding.Default); }
            catch { Console.WriteLine(@"Cannot create result file for writting."); return checkedFileCount;  }

            foreach (var file in fileNames)
            {
                IPdfReader reader = null;
                try { reader = new iTextPdfReader(file); }
                catch (Exception ex)
                {
                    Console.WriteLine("Fatal error: " + ex.Message);                    
                    continue;
                }

                if (reader.IsValidReviewPackage())
                {
                    reader.ReadWholeFile();
                    reader.CheckCommonFields();
                    reader.CheckReviewStatus();
                    reader.CheckWorkProducts();
                    reader.CheckCheckList();
                    if (checkWithCM21)
                        reader.CheckWithCM21(EID, CM21Password, SCRReportDownloadPath, timeout);

                    ++checkedFileCount;
                    foreach (var line in reader.GetDefects())
                        SW.WriteLine(CommonResource.DefectPrefix + line);
                    foreach (var line in reader.GetWarnings())
                        SW.WriteLine(CommonResource.WarningPrefix + line);
                    SW.WriteLine(@"Checked " + file);
                    SW.WriteLine(CommonResource.LineSeperator);
                }
            }

            if (null != SW)
                SW.Close();
            return checkedFileCount;
        }
    }
}
