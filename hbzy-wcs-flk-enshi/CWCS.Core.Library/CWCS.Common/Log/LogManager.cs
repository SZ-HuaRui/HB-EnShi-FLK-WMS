using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace CWCS.Common.Log
{
   public class LogHelperManager
    {
        /// <summary>
        /// 将时间日志记录到系统日志中
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        private static void WriteLogEntry(string source, EventLogEntryType type, string message, params object[] args)
        {
            try
            {
                EventLog.WriteEntry(source, string.Format(message, args), type);
            }
            catch { }
        }
        
        /// <summary>
        /// 判断当前操作系统是否是管理员
        /// </summary>
        /// <returns></returns>
        public static bool IsMachineAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't determine user account status: " + ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检查操作系统的当前版本号
        /// </summary>
        /// <returns></returns>
        public static bool IsValidOS()
        {
            int major = Environment.OSVersion.Version.Major;
            return (major >= 6);
        }


    }
}
