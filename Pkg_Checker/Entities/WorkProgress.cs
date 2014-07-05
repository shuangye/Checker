using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Entities
{
    public enum WorkType
    {
        Start = 0,
        End = 1,
        ErrorOccurred = 2
    }

    public class WorkProgress
    {
        /// <summary>
        /// Progress type. 1: start ; 2: end
        /// </summary>
        public WorkType Type { get; set; }
        public String WorkName { get; set; }
        public List<String> WorkResult { get; set; }
    }
}
