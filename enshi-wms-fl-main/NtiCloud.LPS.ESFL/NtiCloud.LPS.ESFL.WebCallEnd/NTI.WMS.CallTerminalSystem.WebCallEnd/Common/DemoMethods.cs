using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class DemoMethods
    {
        [Ajax.AjaxMethod]
        public string GetCustomerMac(string clientIP) //para IP is the client's IP 
        {
            string mac = "";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "nbtstat";
            process.StartInfo.Arguments = "-a " + clientIP;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            int length = output.IndexOf("MAC Address = ");
            if (length > 0)
            {
                mac = output.Substring(length + 14, 17);
            }

            process.WaitForExit();

            return mac.Replace("-", "").Trim();
        }
    }
}