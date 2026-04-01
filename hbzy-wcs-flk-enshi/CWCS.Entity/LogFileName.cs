using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Entity
{
    public static class LogFileName
    {
        public static string ConveyorLog => "输送机日志";
        public static string AgvLog => "AGV日志";
        public static string ScLog => "堆垛机日志";
        public static string ConveyorExceptionLog => "输送机异常日志";
        public static string AgvExceptionLog => "AGV异常日志";
        public static string ScExceptionLog => "堆垛机异常日志";

    }
}
