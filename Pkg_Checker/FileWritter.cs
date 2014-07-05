using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pkg_Checker
{
    static class FileWritter
    {
        static public String OutputFilePath { get; set; }
        static public StreamWriter SW { get; set; }

        static void Initizlize(String outputFilePath = @".\Check_Result.txt")
        {
            OutputFilePath = outputFilePath;
            try
            {
                SW = new StreamWriter(OutputFilePath, true, Encoding.Default);
            }
            catch
            {
                SW = null;
            }
        }
    }
}
