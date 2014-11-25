using Pkg_Checker.Implementations;
using Pkg_Checker.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pkg_Checker.Helpers
{
    public class ConsoleWorker
    {
        public int StartChecking(String pdfPath, String resultPath, out int totalFileCount)
        {
            int checkedFileCount = 0;
            StreamWriter SW = null;
            bool errorOccurred = false;
            List<String> fileNames;
            totalFileCount = 0;

            fileNames = FSWalker.Walk(pdfPath, @"*.PDF", false, ref errorOccurred);
            if (errorOccurred)
                return 0;

            totalFileCount = fileNames.Count;
            try { SW = new StreamWriter(resultPath, false, Encoding.Default); }
            catch { Console.WriteLine(@"Cannot create result file for writting."); return 0; }

            foreach (var file in fileNames)
            {
                // IPdfReader reader = new iTextPdfReader();
                // reader.Init(file);
                IPdfReader reader = null;
                try { reader = new iTextPdfReader(file); }
                catch (Exception ex)
                {
                    Console.WriteLine("Fatal error: " + ex.Message);                    
                    break;
                }

                if (reader.IsValidReviewPackage())
                {
                    reader.ReadWholeFile();
                    reader.CheckCommonFields();
                    reader.CheckReviewStatus();
                    reader.CheckWorkProducts();
                    reader.CheckCheckList();
                    ++checkedFileCount;
                    foreach (var line in reader.GetDefects())
                        SW.WriteLine(CommonResource.DefectPrefix + line);
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
