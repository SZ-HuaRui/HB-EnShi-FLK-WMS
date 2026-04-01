using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class Command_ReturnNullPalletAndCallMaterials : Command
    {
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private CallPalletBLL cpb = new CallPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName,
            LoginUser.Password, LoginUser.WarehouseID);

        public override string Execute()
        {
            switch (cpb.ClientReturnAndCallPallet(Info.MachineNumber, Info.DockNumber, Info.ModelCode,
                LoginUser.HasLGV.ToString(), LoginUser.RunsTypeCode))
            {
                case 2: 
                    //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT,
                    //"“" + RadioButtonOperator.ReturnNullPalletAndCallPallet + "”" + NTIMessage.OPERATORSUCCEED, 30000);
                    return null;
                case 1: 
                    //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT,
                    //"“" + RadioButtonOperator.ReturnNullPallet + "”" + NTIMessage.OPERATORSUCCEED + ", “要料”"
                    //+ NTIMessage.OPERATORFAIL, Hint.ERROR);
                    return null;
                default:
                    //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT,
                    // "“" + RadioButtonOperator.ReturnNullPalletAndCallPallet + "”" + NTIMessage.OPERATORFAIL,
                    //Hint.ERROR);
                    return null;
            }
            //return false;
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
            try
            {
                return cpb.ClientReturnAndCallPallet(Info.MachineNumber, Info.DockNumber, Info.ModelCode,
                    LoginUser.HasLGV.ToString(), LoginUser.RunsTypeCode);
                //throw new NotImplementedException();
            }
            catch { return 0; }
        }

        public override string ExecuteCallMaterials(string url, string machineId, string hasLGV, string dockId, string productPlanOutId, string shiftId, string teamId, string token)
        {
            throw new NotImplementedException();
        }
    }
}