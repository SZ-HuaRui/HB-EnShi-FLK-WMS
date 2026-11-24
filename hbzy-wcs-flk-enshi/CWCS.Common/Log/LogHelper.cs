using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace CWCS.Common.Log
{
    public class LogHelper
    {
        #region Function
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
                System.Diagnostics.Debug.WriteLine("Couldn't determine user account status: " + ex.Message);
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
        #endregion
        
        NLog.Logger _logger;

        private LogHelper(NLog.Logger logger)
        {
            _logger = logger;
        }

        public LogHelper(string name)
            : this(NLog.LogManager.GetLogger(name))
        {

        }

        public static LogHelper Default { get; private set; }
        static LogHelper()
        {
            Default = new LogHelper(NLog.LogManager.GetCurrentClassLogger());
        }

        #region Debug
        public void Debug(string msg, params object[] args)
        {
            _logger.Debug(msg, args);
        }        

        public void Debug(string msg, Exception err)
        {
            _logger.Debug(err, msg);
        }        
        #endregion

        #region Info
        public void Info(string msg, params object[] args)
        {
            _logger.Info(msg, args);
        }

        public void Info(string msg, Exception err)
        {
            _logger.Info(err, msg);
        }
        #endregion

        #region Warn
        public void Warn(string msg, params object[] args)
        {
            _logger.Warn(msg, args);
        }

        public void Warn(string msg, Exception err)
        {
            _logger.Warn(err, msg);
        }
        #endregion

        #region Trace
        public void Trace(string msg, params object[] args)
        {
            _logger.Trace(msg, args);
        }

        public void Trace(string msg, Exception err)
        {
            _logger.Trace(err, msg);
        }
        #endregion

        #region Error
        public void Error(string msg, params object[] args)
        {
            _logger.Error(msg, args);
        }

        public void Error(string msg, Exception err)
        {
            _logger.Error(err, msg);
        }
        #endregion

        #region Fatal
        public void Fatal(string msg, params object[] args)
        {
            _logger.Fatal(msg, args);
        }

        public void Fatal(string msg, Exception err)
        {
            _logger.Fatal(err, msg);
        }
        #endregion

        #region Custom

        public void Process(LogMsgInfo msg)
        {
            var ei = new LogEventInfo(msg.Level, _logger.Name,msg.Message);
            ei.TimeStamp = msg.TimeStamp;
            ei.Properties["Action"] = msg.Action;
            ei.Properties["Amount"] = msg.Amount;
            _logger.Log(ei);
        }

        #endregion

        /// <summary>
        /// Flush any pending log messages (in case of asynchronous targets).
        /// </summary>
        /// <param name="timeoutMilliseconds">Maximum time to allow for the flush. Any messages after that time will be discarded.</param>
        public void Flush(int? timeoutMilliseconds = null)
        {
            if (timeoutMilliseconds != null)
                NLog.LogManager.Flush(timeoutMilliseconds.Value);

            NLog.LogManager.Flush();
        }

    }
}
