using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    class Command_ReturnNullPallet : Command
    {
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

        public Command_ReturnNullPallet()
        {
            TaskTypeCode = TaskType.TASK_CALLTERMINAL_RETURNNULLPALLET;
        }


        public override string Execute()
        {
            return rpb.ClientReturnNullPallet(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, Info.MachineDockID, LoginUser.HasLGV.ToString(), LoginUser.RunsCodeId,LoginUser.RunsGroupId,LoginUser.SchemeId,LoginUser.MachineId,LoginUser.Token);
        }


        public override bool Check()
        {
            if (!rpb.CheckAllowed_ReturnPallet(Info.MachineNumber, Info.DockNumber))
            {
                //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT, NTIMessage.NOOPERATORMES + "“退空托盘” 操作.", Hint.ERROR);
                return false;
            }
            return true;
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