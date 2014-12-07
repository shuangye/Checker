using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Entities
{
    public enum WorkType
    {
        Start = 1,
        End = 2,
        ErrorOccurred = 3,
        FatalError = 4
    }

    public class WorkProgress
    {
        /// <summary>
        /// Progress type. 1: start ; 2: end
        /// </summary>
        public WorkType Type { get; set; }
        public String WorkName { get; set; }
        public List<String> Defects { get; set; }
        public List<String> Warnings { get; set; }
    }
}
