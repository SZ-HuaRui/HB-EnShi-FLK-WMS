using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class Class_TimeToString
    {
        public static string TimeToString(long DateCode)
        {
            DateTime time = (new DateTime(2006, 12, 1)).AddDays(DateCode);
            return time.ToString("yyyy年M月d日");
        }

        public static string SecondToString(int Second)
        {
            DateTime x = new DateTime();
            x = x.AddSeconds(Second);
            return x.Hour.ToString("D2") + ":" + x.Minute.ToString("D2") + ":" + x.Second.ToString("D2");
        }
    }
}