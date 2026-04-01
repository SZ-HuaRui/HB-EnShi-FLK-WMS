using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Entity
{
   public enum RobotCommand
    {
       CheckResult=1,//下发校验结果
       BindResult=2,//绑定结果
       PickCommand=3,//拆盘指令
       ReWorkResponse=4,//复测盘绑定指令
       None=0
    }
}
