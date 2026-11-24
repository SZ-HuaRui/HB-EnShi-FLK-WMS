using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.DataAccess.Log
{
    /// <summary>
    /// 通用日志接口
    /// </summary>
    public interface ICommonLog : IDisposable
    {
        /// <summary>
        /// 写日志文件
        /// </summary>
        /// <param name="msg">日志内容</param>
        void WriteLog(string msg, string writer="CWCS");
    }
}
