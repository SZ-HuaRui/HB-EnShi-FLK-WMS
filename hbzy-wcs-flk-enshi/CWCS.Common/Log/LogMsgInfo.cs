using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Common.Log
{
   public class LogMsgInfo
    {
        private LogLevel _level;
        private string _Message;
        private DateTime _TimeStamp;
        private Dictionary<object, object> _Action;

       
        private Dictionary<object, object> _Amount;

        public Dictionary<object, object> Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }

        public DateTime TimeStamp
        {
            get { return _TimeStamp; }
            set { _TimeStamp = value; }
        }

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public LogLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }
        public Dictionary<object, object> Action
        {
            get { return _Action; }
            set { _Action = value; }
        }
    }
}
