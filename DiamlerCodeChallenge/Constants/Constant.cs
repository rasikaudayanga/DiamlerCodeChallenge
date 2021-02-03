using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DiamlerCodeChallenge.Constants
{
    public static class Constant
    {
        public static string DATAREPEAT = "data-repeat-";
        public static string DATACOND = "data-cond";      
    }

    public static class RegexConstant
    {
        public static Regex DOLLARREG = new Regex(@"\${[A-Z/a-z/./\s/0-9]+}");
        public static Regex PATTERNREG = new Regex(@"[A-Z/a-z/./\s/0-9]+");
    }
}