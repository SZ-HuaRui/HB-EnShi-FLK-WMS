using CWCS.Common;
using CWCS.Entity;
using CWCS.Entity.Convery;
using CWCS.Entity.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CWCS.Core.Library.PLCManager
{
   public abstract class PLCHelper
   {
       public static Dictionary<string, OPCServer> PLCList = new Dictionary<string, OPCServer>();//PLC列表
       public TimerManager pft = new TimerManager();
       public Action<object> DoWork;
       public Action<List<ConveryEntity>> RefreshStation;
       public Action<PLCStatus> ShowPLCStatus;
       public Action<string> WriteToDowntLog;
       public PLCHelper plchelper;
       public abstract string[] ReadPLCInfo(string EquipNum,int index=0);
       protected void ShowMsg(string message)
       {
         WriteToDowntLog?.Invoke(message);
       }

   }
}
