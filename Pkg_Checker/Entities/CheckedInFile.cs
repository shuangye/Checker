using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Entities
{
    public class CheckedInFile
    {
        public float SCR { get; set; }  // under which the file is checked in
        public String FileName { get; set; }  // full file name
        public int CheckedInVer { get; set; }
        public int ApprovedVer { get; set; }
    }
}
