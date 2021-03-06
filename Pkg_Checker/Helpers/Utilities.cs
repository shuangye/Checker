﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pkg_Checker.Helpers
{
    public class Utilities
    {
        const int SpecialNumber = 5;

        /// <summary>
        /// Map a program internal item number to a human-friend number
        /// </summary>
        /// <param name="number">program internal number</param>
        /// <returns>human-friend number</returns>
        public static int ProgramToHuman(int number)
        {
            if (number < SpecialNumber)
                return number;
            if (number == SpecialNumber)
                return 41;  // simulates 4.1
            else  // number > 5
                return number - 1;
        }

        /// <summary>
        /// Map a human-friend number to a program internal number
        /// </summary>
        /// <param name="number">human-friend number</param>
        /// <returns>program internal number</returns>
        public static int HumanToProgram(int number)
        {
            if (number < SpecialNumber)
                return number;
            if (number == 41)
                return SpecialNumber;
            else
                return number + 1;

        }

        public static int? RegexMatch(String input, String pattern)
        {
            if (null == input || null == pattern)
                return null;

            Match match = Regex.Match(input, pattern);
            if (match.Success)
                return int.Parse(match.Value);
            else
                return null;
        }
    }
}
