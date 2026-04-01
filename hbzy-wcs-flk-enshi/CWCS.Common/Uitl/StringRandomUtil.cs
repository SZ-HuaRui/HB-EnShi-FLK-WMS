using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Common.Uitl
{
    public class StringRandomUtil
    {
        public static string Guid32Bit()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
