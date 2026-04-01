using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class HavingPermissions
    {
        public static bool EstimatePermissions(string[] permissions, string permissionString)
        {
            int value;
            for (int i = 0; i < permissions.Length; i++)
            {
                value = permissions[i].CompareTo(permissionString);
                if (value == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}