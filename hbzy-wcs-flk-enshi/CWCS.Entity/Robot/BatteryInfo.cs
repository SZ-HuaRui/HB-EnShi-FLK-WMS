using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity.Robot
{
    public class BatteryInfo
    {
        //电芯条码
        public string BatteryCode;

        //电池位置
        public int Position;

        //电芯结果（0: OK 其他NG）
        public string BatteryResult;

    }
}
