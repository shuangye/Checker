using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Entities
{
    /// <summary>
    /// Describe the affected element in SCR report
    /// </summary>
    public class AffectedElement
    {
        public float SCR { get; set; }  // under which the file is checked in
        public String FileName { get; set; }  // full file name
        public int CheckedInVer { get; set; }
        // public int ApprovedVer { get; set; }
    }

    /// <summary>
    /// Describe the checked in files filled in review package coversheet.
    /// AffectedElement does not have a property called ApprovedVer, but CheckedInElement does.
    /// </summary>
    public class CheckedInElement : AffectedElement
    {
        public int ApprovedVer { get; set; }
    }
}
