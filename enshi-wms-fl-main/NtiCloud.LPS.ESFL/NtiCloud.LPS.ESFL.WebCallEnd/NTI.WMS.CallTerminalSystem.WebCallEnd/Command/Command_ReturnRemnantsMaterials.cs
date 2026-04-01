using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    class Command_ReturnRemnantsMaterials : Command
    {
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        public Command_ReturnRemnantsMaterials()
        {
            TaskTypeCode = TaskType.TASK_CALLTERMINAL_RETURNPALLET;
        }

        public override string Execute()
        {
            //Return_Remnant_Material rm = new Return_Remnant_Material();
            //Nullable<bool> result = rm.ShowDialog();
            //if (result == true)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            //return false;
            return null;
        }
        /// <summary>
        /// 退余料前检查站台当前是否存在未执行结束的任务
        /// </summary>
        /// <returns>true：存在；false：不存在</returns>
        public override bool Check()
        {
            return rpb.CheckDockCurTask(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.DockNumber, LoginUser.Token);
        }

        //退空托盘+要料提示信息  add by pengenqi 2012/8/1
        public override int ExcuteCallAndReturn()
        {
            return 0;
            //throw new NotImplementedException();
        }

        public override string ExecuteCallMaterials(string url, string machineId, string hasLGV, string dockId, string productPlanOutId, string shiftId, string teamId, string token)
        {
            throw new NotImplementedException();
        }
    }
}