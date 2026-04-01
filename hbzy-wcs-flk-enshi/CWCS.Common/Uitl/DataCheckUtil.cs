using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CWCS.Common.Uitl
{
    public class DataCheckUtil
    {
        public static bool IsNumber(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        
        public static bool IsInteger(string value)
        {
            return Regex.IsMatch(value, @"^\d*$");
        }
    }
}
