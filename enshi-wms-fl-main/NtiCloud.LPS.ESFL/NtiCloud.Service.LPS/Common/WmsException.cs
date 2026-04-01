using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public class WmsException : Exception
    {
        public int ErrorCode { get; } = -1;
        public string ErrorMsg { get; } = "发生错误";
        public WmsException(int code, string msg) : base(msg)
        {
            this.ErrorCode = code;
            this.ErrorMsg = msg;
        }
        public WmsException(int code, string msg, Exception ex) : base(msg, ex)
        {
            this.ErrorCode = code;
            this.ErrorMsg = msg;
        }
    }
}
