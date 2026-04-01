using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd.Common
{
    public class DataChanged
    {
        public static int GetLine(DataTable dt, TextBox tb, string field, string containerDetailID)
        {
            //string containerDetailID = tb.Tag.ToString();
            int line = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (containerDetailID.Equals(row[field].ToString()))
                {
                    return line;
                }
                line++;
            }
            return 0;
        }
    }
    public class GetData
    {
        /// <summary>
        /// 获取生产牌名
        /// </summary>
        /// <param name="machineNumber"></param>
        public static void GetMachineProductPlan(CommonBLL cb, string machineNumber)
        {
            CommonBLL bll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            try
            {
                //add by hj 2012/08/28
                DataSet machineProductPlan = null; // cb.GetMachineCurPlanByMachineNumber(machineNumber);
                if (machineProductPlan == null || machineProductPlan.Tables.Count <= 0
                    || machineProductPlan.Tables[0].Rows.Count <= 0)
                {
                    //bll.TrackCallEndErrors(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.WarehouseID, 0, "GetData方法GetMachineProductPlan获取当前机台生产牌号为空", "GetData方法GetMachineProductPlan获取当前机台生产牌号为空", "GetData方法GetMachineProductPlan获取当前机台生产牌号为空");
                    LoginUser.ItemID = null;
                    LoginUser.ItemName = null;
                    return;
                }
                LoginUser.ItemID = machineProductPlan.Tables[0].Rows[0]["ITEMID"].ToString();

                LoginUser.ItemName = machineProductPlan.Tables[0].Rows[0]["NAME"].ToString();
            }
            catch(Exception exception)
            {
               // bll.TrackCallEndErrors(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.WarehouseID, 0, "GetData" + exception.Message, "GetData" + exception.Message, "GetData" + exception.Message);
            }
        }
    }
}