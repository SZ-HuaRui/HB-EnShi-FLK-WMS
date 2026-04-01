using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    class Command_CallMaterials : Command
    {
        //CallPalletBLL cpb = new CallPalletBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        //CommonBLL commBll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        MessageBox message = new MessageBox();
        public Command_CallMaterials()
        {
            TaskTypeCode = TaskType.TASK_CALLTERMINAL_CALL;
        }

        public override string Execute()
        {
            CommonBLL commBll = new CommonBLL();
            CallPalletBLL cpb = new CallPalletBLL();

            return cpb.ClientCallPallet(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, Info.DockNumber,
                LoginUser.HasLGV.ToString(), LoginUser.RunsTypeCode);

        }

        public override bool Check()
        {
            CallPalletBLL cpb = new CallPalletBLL(Info.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            try
            {
                if (!cpb.CheckAllowed_CallPallet(Info.MachineNumber, Info.DockNumber))
                {
                    //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT, NTIMessage.DOCKTASKERROR, Hint.ERROR);
                    return false;
                }
                //解决空指针异常的问题  PEQ 20100117
                if (Info.ItemSource != null && Info.ItemSource.Equals("1"))
                {
                    if (!cpb.CheckContainerInventory(Info.MachineNumber, Info.DockNumber))
                    {
                        //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT, NTIMessage.DismantleProductionScheme_ShortInventory, Hint.ERROR);
                        return false;
                    }
                }
                return true;
            }
            catch { return false; }
        }

        //退空托盘+要料提示信息  add by pengenqi 2012/8/1
        public override int ExcuteCallAndReturn()
        {
            return 0;
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 要料
        /// </summary>
        /// <param name="url">微服务url</param>
        /// <param name="machineId">机台id</param>
        /// <param name="hasLGV">是否需要LGV</param>
        /// <param name="dockId">机台id</param>
        /// <param name="productPlanOutId">生产工单id</param>
        /// <param name="shiftId">班次id</param>
        /// <param name="teamId">班组id</param>
        /// <param name="token">token</param>
        /// <returns>返回提示信息（为空字符串则表示要料成功）</returns>
        public override string ExecuteCallMaterials(string url, string machineId, string hasLGV, string dockId, string productPlanOutId, string shiftId, string teamId, string token)
        {
            CallPalletBLL cpb = new CallPalletBLL();

            return cpb.ClientCallPallet(url, machineId, hasLGV, dockId, productPlanOutId, shiftId, teamId, token);
        }
    }
}