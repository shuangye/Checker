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

        /// <summary>
        /// 很宽容地取子字符串：若开始标志未找到，则子串从头开始；若结束标志未找到，则子串持续到结尾。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="beginMark"></param>
        /// <param name="endMark"></param>
        /// <param name="beginMarkIncluded">is begin mark included?</param>
        /// <param name="endMarkIncluded">is end mark included?</param>
        /// <returns></returns>
        public static String SubString(this String str, String beginMark, String endMark, bool beginMarkIncluded, bool endMarkIncluded)
        {
            if (String.IsNullOrEmpty(beginMark) || String.IsNullOrEmpty(endMark))
                return null;

            int subStringStartLocation, subStringLength;
            int beginMarkLocation = str.IndexOf(beginMark);
            int endMarkLocation = str.IndexOf(endMark);
            if (beginMarkLocation < 0 && endMarkLocation < 0)
                return null;  // both marks not found

            if (beginMarkLocation < 0)
                subStringStartLocation = 0;  // zero based
            else            
                subStringStartLocation = beginMarkIncluded ? beginMarkLocation : beginMarkLocation + beginMark.Length;

            if (endMarkLocation < 0)
                subStringLength = str.Length - subStringStartLocation;
            else
                subStringLength = endMarkLocation + (endMarkIncluded ? endMark.Length : 0) - subStringStartLocation;

            return str.Substring(subStringStartLocation, subStringLength);            
        }
    }
}
