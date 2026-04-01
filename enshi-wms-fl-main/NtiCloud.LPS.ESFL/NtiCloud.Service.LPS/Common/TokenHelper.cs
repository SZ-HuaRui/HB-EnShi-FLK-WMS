using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public class TokenHelper
    {
        public static bool IsNullOrEmpty(Dictionary<string,string> token)
        {
            if (!token.ContainsKey("warehouseId") ||
                !token.ContainsKey("appCode"))
            {
                return true;
            }
            return string.IsNullOrEmpty(token["warehouseId"]) ||
                   string.IsNullOrEmpty(token["appCode"]);
        }
    }
}
