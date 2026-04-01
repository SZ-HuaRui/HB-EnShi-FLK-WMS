using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    class Command_ReturnBadMaterials : Command
    {
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        public Command_ReturnBadMaterials()
        {
            TaskTypeCode = TaskType.TASK_CALLTERMINAL_RETURNBADPALLET;
        }

        public override string Execute()
        {
            //rpb.GetContainerDetailByDockNumber(LoginUser.MachineNumber, Info.DockNumber);

            //Return_Bad_Materials rbm = new Return_Bad_Materials();
            //Nullable<bool> result = rbm.ShowDialog();
            //if (result == true)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return null;
        }
        public override bool Check()
        {
            return rpb.CheckAllowed_ReturnPallet(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.DockNumber, LoginUser.Token);
            //if (!rpb.CheckAllowed_ReturnPallet(Info.MachineNumber, Info.DockNumber))
            //{
            //    //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT, NTIMessage.NOOPERATORMES + "“退坏料”操作.", Hint.ERROR);
            //    return false;
            //}
            //return true;
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