using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Entities
{
    /// <summary>
    /// This class represents one SCR report in the review package
    /// </summary>
    public class SCRReport
    {
        public float SCRNumber { get; set; }
        public String Status { get; set; }  // REV, SEC, VER, etc.
        public String AffectedArea { get; set; }
        public String TargetConfig { get; set; }
        public String ClosedConfig { get; set; }
        public List<CheckedInFile> AffectedElements { get; set; }
    }
}
