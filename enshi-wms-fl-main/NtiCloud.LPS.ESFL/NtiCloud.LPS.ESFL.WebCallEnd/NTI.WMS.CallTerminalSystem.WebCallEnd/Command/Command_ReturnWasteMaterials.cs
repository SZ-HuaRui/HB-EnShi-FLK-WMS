using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    class Command_ReturnWasteMaterials : Command
    {
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

        public Command_ReturnWasteMaterials()
        {
            TaskTypeCode = TaskType.TASK_CALLTERMINAL_RETURNWASTEPALLET;
        }

        public override bool Check()
        {
            throw new NotImplementedException();
        }

        public override int ExcuteCallAndReturn()
        {
            throw new NotImplementedException();
        }

        public override string Execute()
        {
            return rpb.ReturnWasteMaterials(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, Info.MachineDockID, LoginUser.HasLGV.ToString(), LoginUser.RunsCodeId, LoginUser.RunsGroupId, LoginUser.SchemeId, LoginUser.MachineId, LoginUser.Token);
        }

        public override string ExecuteCallMaterials(string url, string machineId, string hasLGV, string dockId, string productPlanOutId, string shiftId, string teamId, string token)
        {
            throw new NotImplementedException();
        }
    }
}