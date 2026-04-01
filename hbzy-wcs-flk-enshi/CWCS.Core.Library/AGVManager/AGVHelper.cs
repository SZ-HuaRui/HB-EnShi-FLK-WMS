using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CWCS.Entity;

namespace CWCS.Core.Library.AGVManager
{
  public class AGVHelper
    {
      public enum ModuleName { NT8000, LED, Conveyer, RF, CMServer };
        public class GP
        {
            public const int MaxCmdCount = 5;					//发给LGV的最大命令数
            public const int IsBusyCmdBufferCount = 2;			//让AGV忙命令缓冲数

            public const int LGV_Init = 1;
            public const int LGV_BeginFetch = 2;
            public const int LGV_Loaded = 3;
            public const int LGV_BeginUnLoad = 4;
            public const int LGV_UnLoaded = 5;
            public const int LGV_CarWashLoaded = 6;
            public const int CancelBeforeFetch = 7;
            public const int EmptyFetch = 8;
            public const int DestOccupy = 9;

            public const int UnKnowedPallet = -100;	   //LGV上出现的未知托盘。

            public const int NormalMaxCmdCount = 3;	//正常状况下最大命令数
            public const int BusyMaxCmdCount = 5;	//忙状况下最大命令数
        }

        //定义程序处理的代理
        public delegate void TransmitNotifyHandler(ModuleName mName, NotifyCommand cmd, object state);
        public TimerGetTask tTask = new TimerGetTask();
        public TimerChangePriority tChange = new TimerChangePriority();
        public struct LGVOrderPara //此结构定义了生成LGV指令所需的参数
        {
            public Int32 fromStation;
            public Int32 toStation;
            public Int32 ikey;
        }


      public  class TimerGetTask
        {
            public int counter = 0;
            public Timer tmr;
        }

      public class TimerChangePriority
      {
          public int counter = 0;
          public Timer tmr;
      }


    }
}
