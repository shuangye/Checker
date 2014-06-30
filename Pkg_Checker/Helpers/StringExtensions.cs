using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove the first of occurrence of toStrip from str
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String Strip(this String str, String toStrip)
        {
            int location;

            if (String.IsNullOrWhiteSpace(str) || String.IsNullOrWhiteSpace(toStrip))
                return str;

            location = str.IndexOf(toStrip);
            if (location >= 0)
                return str.Remove(location, toStrip.Length);

            return str;
        }
    }
}
