using CWCS.DataAccess.CommUtil;
using CWCS.DataAccess.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CWCS.DataAccess.DataProvider
{
    /// <summary>
    /// 通用执行日志
    /// </summary>
    public class CommandLog
    {
        private const int BufferCount = 10;
        private static CommandLog _Instance;
        private static object lockObj = new object();
        private System.Diagnostics.Stopwatch watch = null;

        private ICommonLog _logWriter;
        /// <summary>
        /// 获取或者设置日志写入对象
        /// </summary>
        public ICommonLog LogWriter
        {
            get
            {
                if (_logWriter == null)
                {
                    InnerLogWriter logger = new InnerLogWriter();
                    logger.DataLogFile = DataLogFile;
                    logger.SaveCommandLog = SaveCommandLog;
                    logger.LogBufferCount = LogBufferCount;

                    _logWriter = logger;
                }

                return _logWriter;
            }
            set
            {
                _logWriter = value;
            }
        }

        #region 日志配置相关属性设置
        //日志相关
        private static string _dataLogFile;
        /// <summary>
        /// 获取或者设置日志文件的路径
        /// </summary>
        public static string DataLogFile
        {
            get
            {
                if (string.IsNullOrEmpty(_dataLogFile))
                {
                    _dataLogFile = System.Configuration.ConfigurationManager.AppSettings["DataLogFile"];
                    setDataLogFile();
                }
                return _dataLogFile;
            }
            set
            {
                _dataLogFile = value;
                setDataLogFile();
            }
        }

        private static void setDataLogFile()
        {
            if (!string.IsNullOrEmpty(_dataLogFile))
            {
                CommonUtil.ReplaceWebRootPath(ref _dataLogFile);
                string temp = System.Configuration.ConfigurationManager.AppSettings["SaveCommandLog"];
                if (temp != null)
                    _saveCommandLog = temp.ToUpper() == "TRUE";
            }
        }

        private static bool _saveCommandLog;
        /// <summary>
        /// 是否记录日志文件
        /// </summary>
        public static bool SaveCommandLog
        {
            get
            {
                string temp = DataLogFile;//必须先调用下，以计算_saveCommandLog
                return _saveCommandLog;
            }
            set { _saveCommandLog = value; }
        }

        private static long _logExecutedTime = -1;
        /// <summary>
        /// 需要记录的时间，只有该值等于0会记录所有查询，否则只记录大于该时间的查询。单位毫秒。
        /// </summary>
        public static long LogExecutedTime
        {
            get
            {
                if (_logExecutedTime == -1)
                {
                    string temp = System.Configuration.ConfigurationManager.AppSettings["LogExecutedTime"];
                    if (string.IsNullOrEmpty(temp))
                        _logExecutedTime = 0;
                    else
                        long.TryParse(temp, out _logExecutedTime);
                }
                return _logExecutedTime;
            }
            set { _logExecutedTime = value; }
        }

        private static int _logBufferCount = -1;
        /// <summary>
        /// 日志信息的缓存数量，默认是20条
        /// </summary>
        public static int LogBufferCount
        {
            get
            {
                if (_logBufferCount == -1)
                {
                    string temp = System.Configuration.ConfigurationManager.AppSettings["LogBufferCount"];
                    if (string.IsNullOrEmpty(temp))
                    {
                        _logBufferCount = BufferCount;
                    }
                    else
                    {
                        if (!int.TryParse(temp, out _logBufferCount))
                            _logBufferCount = BufferCount;
                    }
                }
                return _logBufferCount;
            }
            set
            {
                _logBufferCount = value;
            }
        }

        #endregion

        /// <summary>
        /// 获取单例对象(不开启执行时间记录)
        /// </summary>
        public static CommandLog Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (lockObj)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new CommandLog();
                        }
                    }
                }
                return _Instance;
            }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CommandLog()
        {

        }

        /// <summary>
        /// 是否开启执行时间记录
        /// </summary>
        /// <param name="startStopwatch"></param>
        public CommandLog(bool startStopwatch)
        {
            if (startStopwatch)
            {
                watch = new System.Diagnostics.Stopwatch();
                watch.Start();
            }
        }

        /// <summary>
        /// 重新开始记录执行时间
        /// </summary>
        public void ReSet()
        {
            if (watch != null)
            {
                watch.Reset();
                watch.Start();
            }
        }

        /// <summary>
        /// 获取当前执行的实际SQL语句
        /// </summary>
        public string CommandText
        {
            private set;
            get;
        }

        /// <summary>
        /// 写命令日志和执行时间（如果开启的话）
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <param name="userid">调用命令的源名称</param>
        /// <param name="elapsedMilliseconds">执行时间</param>
        public void WriteLog(IDbCommand command, string userid, out long elapsedMilliseconds)
        {
            CommandText = command.CommandText;
            elapsedMilliseconds = 0;
            if (watch != null)
            {
                elapsedMilliseconds = watch.ElapsedMilliseconds;
                if (SaveCommandLog)
                {
                    if ((LogExecutedTime > 0 && elapsedMilliseconds > LogExecutedTime) || LogExecutedTime == 0)
                    {
                        RecordCommandLog(command, userid);
                        this.LogWriter.WriteLog("Execueted Time(ms):" + elapsedMilliseconds + "\r\n", userid);
                    }
                }
                watch.Stop();
            }
            else
            {
                if (SaveCommandLog)
                    RecordCommandLog(command, userid);
            }
        }

        public void WriteLog(string msg, string userid)
        {
            this.LogWriter.WriteLog(msg, userid);
        }

        /// <summary>
        /// 写错误日志，将使用 DataLogFile 配置键的文件名写文件
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <param name="errmsg">调用命令的源名称</param>
        public void WriteErrLog(IDbCommand command, string errmsg)
        {
            if (!string.IsNullOrEmpty(DataLogFile))
            {
                errmsg += ":Error";
                RecordCommandLog(command, errmsg);
            }
        }

        /// <summary>
        /// 获取日志文本
        /// </summary>
        /// <returns>日志文本</returns>
        public string GetLog()
        {
            StreamReader sr = File.OpenText(DataLogFile);
            string text = sr.ReadToEnd();
            sr.Close();
            return text;
        }

        /// <summary>
        /// 记录命令信息
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <param name="userid">执行人</param>
        private void RecordCommandLog(IDbCommand command, string writer)
        {
            string temp = "//" + DateTime.Now.ToString() + " @" + writer + " 执行命令：\r\nSQL=\"" + command.CommandText + "\"\r\n//命令类型：" + command.CommandType.ToString();
            if (command.Transaction != null)
                temp = temp.Replace("执行命令", "执行事务");
            this.LogWriter.WriteLog(temp);
            if (command.Parameters.Count > 0)
            {
                this.LogWriter.WriteLog("//" + command.Parameters.Count + "个命令参数：");
                for (int i = 0; i < command.Parameters.Count; i++)
                {
                    IDataParameter p = (IDataParameter)command.Parameters[i];
                    this.LogWriter.WriteLog("Parameter[\"" + p.ParameterName + "\"]\t=\t\"" + Convert.ToString(p.Value) + "\"  \t\t\t//DbType=" + p.DbType.ToString());
                }
            }
        }
        /// <summary>
        /// 将日志全部写入
        /// </summary>
        public void Dispose()
        {
            this.LogWriter.Dispose();
        }

    }

    class InnerLogWriter : ICommonLog
    {
        public string DataLogFile { get; set; }
        public bool SaveCommandLog { get; set; }
        public int LogBufferCount { get; set; }

        private List<string> _logBuffer = new List<string>();
        private DateTime _lastWrite = DateTime.Now;
        private const int WriteTime = 10;//10秒写入一次

        /// <summary>
        ///写入日志消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="userid">发送者</param>
        public void WriteLog(string msg, string writer)
        {
            if (SaveCommandLog)
                WriteLog("//" + DateTime.Now.ToString() + " @" + writer + " ：" + msg + "\r\n");
        }

        /// <summary>
        /// 批量写入日志
        /// </summary>
        /// <param name="log"></param>
        public void WriteLog(string log)
        {
            if (!string.IsNullOrEmpty(log))
                _logBuffer.Add(log);

            if (_logBuffer.Count > 0 && (DateTime.Now.Subtract(_lastWrite).TotalSeconds > WriteTime || _logBuffer.Count > LogBufferCount))
            {
                List<string> tempList = _logBuffer;
                _logBuffer = new List<string>();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (string item in tempList)
                {
                    sb.Append(item);
                    sb.Append("\r\n");
                }
                string writeText = sb.ToString();

                //无锁异步写
                using (FileStream fs = new FileStream(DataLogFile, FileMode.Append, FileAccess.Write, FileShare.Write, 2048, FileOptions.Asynchronous))
                {
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(writeText);
                    IAsyncResult writeResult = fs.BeginWrite(buffer, 0, buffer.Length,
                        (asyncResult) =>
                        {
                            FileStream fStream = (FileStream)asyncResult.AsyncState;
                            fStream.EndWrite(asyncResult);
                        },
                        fs);
                    fs.Flush();
                }
                _lastWrite = DateTime.Now;
            }
        }

        public void Dispose()
        {
            _lastWrite = DateTime.Now.AddMinutes(-10);
        }
    }
}
