using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WMSInterface;

namespace NTI.WMS.CallTerminalSystem.DAL
{
    public class WriteLog
    {
        //System.IO.Directory.GetCurrentDirectory();

        //日志文件写入目录
        private static string filePath = System.AppDomain.CurrentDomain.BaseDirectory;//@"C:\Logs\WebCallEnd";
        private static LogWriter.LogWriter write = null;

        private WriteLog()
        { }

        private static LogWriter.LogWriter GetWrite()
        {
            if (write == null)
            {
                write = new LogWriter.LogWriter(filePath, "CallTerminal", "yyyy-MM-dd", ".log");
            }
            return write;
        }


        public static void Write(WorkFlowResult rs, string source)
        {
            try
            {
                string infoStr = rs == null ? "没有操作权限" : rs.Info;
                if (string.IsNullOrEmpty(infoStr))
                {
                    infoStr = "SYSTEM.ERROR";
                }
                GetWrite().WriteLine(source, infoStr);
                //GetWrite().Flush();
            }
            catch
            { }
            finally { }
        }

        public static void Write(string text, string source)
        {
            try
            {
                GetWrite().WriteLine(source, text);
            }
            catch
            { }
            finally { }
        }

        //写系统运行日志
        public static void WriterSystemLogs(string content, bool isWriteLineFirst = false)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                string logPath = filePath + DateTime.Now.ToString("yyyy-MM-dd");
                CheckFilePath(logPath);
                string fileName = logPath + "Log_" + DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                fs = new FileStream(fileName, FileMode.Append);
                sw = new StreamWriter(fs);
                if (isWriteLineFirst) sw.WriteLine();
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        //写系统错误日志
        public static void WriterErrorLogs(string content)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                string logPath = filePath + DateTime.Now.ToString("yyyy-MM-dd") + "\\ErrorLogs";
                CheckFilePath(logPath);
                string fileName = logPath + "ErrLog_" + DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                fs = new FileStream(fileName, FileMode.Append);
                sw = new StreamWriter(fs);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n" + content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        //检查目录是否存在
        private static void CheckFilePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
