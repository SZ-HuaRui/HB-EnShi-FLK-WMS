using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public sealed class LogHelper
    {

        private static ILogger _logger = null;
        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    throw new Exception("应用环境未能正确初始化");
                }
                return _logger;
            }
        }
        public static ILogger Init(ILogger logger)
        {
            if (logger != _logger || _logger == null)
            {
                _logger = logger;
            }
            return _logger;
        }

        public static bool DebugMode { get; set; } = true;

        public static void LogDebug(string msg, params object[] args)
        {
            if (!DebugMode)
            {
                return;
            }
            if (_logger != null)
            {
                //_logger.LogDebug(msg, args);
                _logger.LogError(msg, args);
            }
            WriteLine(msg, args);
        }

        public static void LogError(string msg, params object[] args)
        {
            if (_logger != null)
            {
                _logger.LogError(msg, args);
            }
            WriteLine(msg, args);
        }

        public static void LogException(Exception ex)
        {
            Exception e = ex;
            while (e != null)
            {
                if (_logger != null)
                {
                    _logger.LogError(e.Message);
                    _logger.LogError(e.StackTrace);
                }
                WriteLine(e.Message);
                WriteLine(e.StackTrace);
                e = e.InnerException;
            }
        }

        private static void WriteLine(string msg, params object[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.WriteLine(string.Format(msg, args));
            }
        }

        public static void LogInformation(string msg, params object[] args)
        {
            if (_logger != null)
            {
                _logger.LogInformation(msg, args);
            }
            WriteLine(msg, args);
        }
    }
}
