using System;
using System.Collections.Generic;
using System.Text;

namespace WMSInterface
{
    [Serializable]
    public class WorkFlowResult
    {
        private int _result;  //运行结果
        private string  _runInfo;   //运行信息

        public int Result
        {
            get { return this._result; }
            set { this._result = value; }
        }

        public string Info
        {
            get { return this._runInfo; }
            set { this._runInfo = value; }
        }

    }
}
