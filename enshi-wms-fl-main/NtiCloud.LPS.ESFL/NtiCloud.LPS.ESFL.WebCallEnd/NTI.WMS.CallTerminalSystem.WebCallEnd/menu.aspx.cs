using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using NTI.WMS.CallTerminalSystem.WebCallEnd.Common;
using System.Text;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd.html
{
    public partial class menu : System.Web.UI.Page
    {
        ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        CommonBLL commonBll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private CommonBLL cb = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private DataTable dockNumberDt;
        private DataSet change_Class_Task;
        private bool windowsClosed = false;
        private bool isLogOff = false;
        string UserName;
        string UserPassword;
        string WareHouseID;
        string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, hasLGV;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Change_Class"] == "" || Session["Change_Class"] == null)
            {
                GetData.GetMachineProductPlan(cb, MachineNumber);
                //GetChangeClassTask();
            }
        }
        //呼叫终端
        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Call_Terminal.aspx");
        }
        //换牌退料
        protected void Button2_Click(object sender, EventArgs e)
        {
            Response.Redirect("Change_Card.aspx");
        }
        //交接班
        protected void Button3_Click(object sender, EventArgs e)
        {
            Response.Redirect("Change_Class.aspx");
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('此功能完善中!');</script>");
        }
        /// <summary>
        /// 人工退料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button5_Click(object sender, EventArgs e)
        {
            string dockNumber = GetDockNumber(Properties.Settings.Default.ContainerTypeCode_Handle);
            if (!isGetDockNumber(Properties.Settings.Default.ContainerTypeCode_Handle))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('本机台没有设置人工站台!');</script>");
                return;
            }
            if (cb.CheckDockState(LoginUser.MachineNumber, dockNumber))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该站台没有物料,不允许进行退料操作!');</script>");
                return;
            }
            else if (!cb.CheckDockCurTaskByDockNumber_Return(dockNumber) || !cb.CheckDockReturnBill_Exists(dockNumber, LoginUser.MachineNumber))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该站台存在未完成任务或未执行的退料单,不允许进行退料操作!');</script>");
                return;
            }
            Response.Redirect("ManPower_Return_Materials.aspx");
        }
        /// <summary>
        /// 获取model区段的站台
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetDockNumber(string model)
        {
            GetDockNumbers();
            StringBuilder dockNumber = new StringBuilder();
            if (dockNumberDt != null && dockNumberDt.Rows.Count != 0)
            {
                foreach (DataRow row in dockNumberDt.Rows)
                {
                    if (model.Equals(row["MODELCODE"].ToString()))
                    {
                        dockNumber = dockNumber.Append(row["DOCKNUMBER"].ToString() + ",");
                    }
                }
                if (dockNumber.Length > 0)
                {
                    dockNumber = dockNumber.Remove(dockNumber.Length - 1, 1);
                }
            }

            return dockNumber.ToString();
        }
        /// <summary>
        /// 判断是否存在model区段的站台
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool isGetDockNumber(string model)
        {
            GetDockNumbers();
            if (dockNumberDt != null)
            {
                foreach (DataRow row in dockNumberDt.Rows)
                {
                    if (model.Equals(row["MODELCODE"].ToString()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void GetDockNumbers()
        {
            dockNumberDt = commonBll.GetMachineDockInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, MachineNumber);
        }
    }
}