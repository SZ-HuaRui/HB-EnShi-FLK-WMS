using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using System.Text;
using NTI.WMS.CallTerminalSystem.WebCallEnd.Common;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Control_Panel : System.Web.UI.Page
    {
        private DataTable dockNumberDt;
        string UserName;
        string WareHouseID;
        string ItemID, ItemCode, ItemName, StateCode;
        string Equipment, Run, MachineNumber, RunsGroup, RunsGroupId, MachineName, hasLGV;
        ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        CommonBLL commonBll = new CommonBLL();

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSAOUR");

            if (Session["SessionKey"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            UserName = LoginUser.UserName;
            WareHouseID = LoginUser.WarehouseID;
            Equipment = LoginUser.EquipmentNumber;
            Run = LoginUser.RunsCode; //班次 
            RunsGroup = LoginUser.RunsGroup;//班组 
            RunsGroupId = LoginUser.RunsGroupId;//班组id
            hasLGV = Session["HasLGV"].ToString();
            LoginUser.HasLGV = Convert.ToInt32(hasLGV.ToString());
            Response.Expires = -1;
            MachineNumber = LoginUser.MachineNumber;
            MachineName = LoginUser.MachineName;
            if (!Page.IsPostBack)
            {
                //机台生产牌号
                bool getPlanIsSucc = GetMachineCurPlanByMachineNumber();

                if (getPlanIsSucc == false)
                {
                    return;
                }

                if (string.IsNullOrEmpty(MachineNumber))
                    MachineNumber = "未设置";
                if (string.IsNullOrEmpty(MachineName))
                    MachineName = "未设置";

                Label1.Text = "车间:" + Session["WorkShopName"] + " 机台:" + MachineName + " 班次:" + Run + " 班组:"
                    + RunsGroup + " 生产日期:" + System.DateTime.Now.ToShortDateString();

                //LoginUser.LoadUserPermissions(UserName);  //获取用户权限

                //是否需要接班
                GetChangeClassTask();
            }
        }
        #endregion

        #region 机台设置
        /// <summary>
        /// 机台设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Setting_Click(object sender, EventArgs e)
        {
            Response.Redirect("Set_Number.aspx");

            //if (LoginUser.IsAdmin || HavingPermissions.EstimatePermissions(LoginUser.tempermission, PermissionString.WorkShop_CurMachineNumberSetting))
            //{
            //    Session["IsToCall"] = "";
            //    Response.Redirect("Set_Number.aspx");
            //}
            //else
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('您没有设置机台编号的权限，请在WMS中设置权限!');</script>");
            //}
        }

        #endregion

        #region 批量退余料
        /// <summary>
        /// 批量退余料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Change_Card_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoginUser.SchemeId))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台没有生产工单，请先创建生产工单！');</script>");
                return;
            }
            //DataTable dtTasks = commonBll.GetMachinePersonTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.SchemeId, LoginUser.WarehouseID, LoginUser.MachineId, Properties.Settings.Default.TaskTypes, LoginUser.Token);
            //if (dtTasks == null || dtTasks.Rows.Count > 0)
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台有未完成的人工领料和退料任务，请等待所有任务全部完成之后，再进行此操作!');</script>");
            //    return;
            //}
            //任务不包含退废料
            DataTable dtTasks = commonBll.GetMachineTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.SchemeId, LoginUser.WarehouseID, LoginUser.MachineId, Properties.Settings.Default.TaskTypes, LoginUser.Token,"0");
            if (dtTasks != null && dtTasks.Rows.Count <= 0)
            {
                Response.Redirect("Change_Card.aspx");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台有未完成的任务，请等待所有任务全部完成之后，再进行此操作!');</script>");
                return;
            }
        }
        #endregion

        #region 退出
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_LogOff_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Call_Terminal_Log.aspx");
        }
        #endregion

        #region 呼叫终端
        /// <summary>
        /// 呼叫终端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Call_Timinal_Click(object sender, EventArgs e)
        {
            ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(MachineNumber, UserName, LoginUser.Password, LoginUser.WarehouseID);
            if (string.IsNullOrEmpty(LoginUser.SchemeId))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台没有生产工单，请先创建生产工单！');</script>");
                return;
            }
            if (string.IsNullOrEmpty(LoginUser.MachineNumber))
            {
                Response.Redirect("Set_Number.aspx");
                return;
            }
            DataTable change_Class_Task = rbob.GetNoFinishMachineReturnMaterialBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.Token);

            string url = "Call_Terminal.aspx";
            if ("EquipmentUsage_Hand".Equals(LoginUser.WorkShop))
            {
                url = "Call_Terminal_Hand.aspx";  //手包区
            }
            else if ("EquipmentUsage_Prouduct".Equals(LoginUser.WorkShop))
            {
                url = "Call_Terminal_Product.aspx";  //20号机组对应的用于成品转移的机台
            }
            else if ("EquipmentUsage_SuppleMaterial".Equals(LoginUser.WorkShop))
            {
                url = "Call_Terminal_SemiFinishedProduct.aspx";  //辅料暂存区对应的机台
            }

            if (change_Class_Task == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无法访问服务器,请重新登录！');window.location='" + url + "';</script>");
                return;
            }
            if (change_Class_Task.Rows.Count > 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('已经进行了交班，无法进行呼叫终端操作！');</script>");
                return;
            }

            Response.Redirect(url);
        }
        #endregion

        #region 交班
        /// <summary>
        /// 交班
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Change_Class_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoginUser.SchemeId))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台没有生产工单，请先创建生产工单！');</script>");
                return;
            }

            DataTable dtTasks = commonBll.GetMachinePersonTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.SchemeId, LoginUser.WarehouseID, LoginUser.MachineId, Properties.Settings.Default.TaskTypes, LoginUser.Token);
            if (dtTasks == null || dtTasks.Rows.Count > 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台有未完成的人工领料和退料任务，请等待所有任务全部完成之后，再进行此操作!');</script>");
                return;
            }
            //任务不包含退废料
            dtTasks = commonBll.GetMachineTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.SchemeId, LoginUser.WarehouseID, LoginUser.MachineId, Properties.Settings.Default.TaskTypes, LoginUser.Token,"0"); //commonBll.GetMachineTaskByMachineNumber(LoginUser.EquipmentNumber, UserName, LoginUser.Password, MachineNumber, Properties.Settings.Default.TaskTypes);
            if (dtTasks != null && dtTasks.Rows.Count <= 0)
            {
                Response.Redirect("Change_Class.aspx");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台有未完成的任务，请等待所有任务全部完成之后，再进行此操作!');</script>");
                return;
            }
        }
        #endregion

        #region 接班
        /// <summary>
        /// 接班
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_ClassDetails_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LoginUser.SchemeId))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台没有生产工单，请先创建生产工单！');</script>");
                return;
            }
            ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(MachineNumber, UserName, LoginUser.Password, LoginUser.WarehouseID);
            DataTable initData = rbob.GetNoFinishMachineReturnMaterialBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.Token);
            //获取不到交班托盘信息，则无交班信息
            if ((initData == null) || (initData.Rows.Count == 0))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('未查询到交接班信息！');</script>");
                return;
            }
            Response.Redirect("Change_Class_Accept.aspx");
        }
        #endregion

        #region 获取机台生产牌号
        /// <summary>
        /// 获取机台生产牌号
        /// </summary>
        public bool GetMachineCurPlanByMachineNumber()
        {
            CommonBLL bll = new CommonBLL();
            DataTable dt = bll.GetMachineCurPlanByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.MachineId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, LoginUser.Token);
            if (dt == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('获取生产牌号失败！');</script>");
                return false;
            }
            if (dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台未设定生产牌号！');</script>");
                return false;
            }
            if (dt.Rows.Count != 1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台设定了多个生产工单！');</script>");
                return false;
            }

            //StateCode = dt.Rows[0]["STATE_CODE"].ToString();
            LoginUser.ItemID = dt.Rows[0]["item_id"].ToString();
            LoginUser.ItemName = dt.Rows[0]["item_name"].ToString();
            LoginUser.MachineId = dt.Rows[0]["equipment_id"].ToString();
            LoginUser.MachineNumber = dt.Rows[0]["equipment_code"].ToString();
            LoginUser.MachineName = dt.Rows[0]["equipment_name"].ToString();
            LoginUser.RunsCode = dt.Rows[0]["shift_name"].ToString();//shift_name
            LoginUser.RunsCodeId = dt.Rows[0]["shift_id"].ToString();//shift_id
            LoginUser.RunsGroup = dt.Rows[0]["team_name"].ToString();//team_name
            LoginUser.RunsGroupId = dt.Rows[0]["team_id"].ToString();//team_id
            LoginUser.SchemeId = dt.Rows[0]["t_id"].ToString();
            LoginUser.WorkShop = dt.Rows[0]["equipment_usage"].ToString();
            return true;
        }
        #endregion

        #region 查询接班记录
        /// <summary>
        /// 查询接班记录
        /// </summary>
        private void GetChangeClassTask()
        {
            ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(MachineNumber, UserName, LoginUser.Password, LoginUser.WarehouseID);
            DataTable change_Class_Task = rbob.GetNoFinishMachineReturnMaterialBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.Token);

            if (change_Class_Task == null)
            {
                string url = "Call_Terminal_Log.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无法访问服务器,请重新登录！');window.location='" + url + "';</script>");
                return;
            }
            if (change_Class_Task.Rows.Count == 0)
            {
                return;
            }
            //if (change_Class_Task.Rows.Count != 1)
            //{
            //    string url = "Call_Terminal_Log.aspx";
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台上的待接班记录不唯一,请重新登录！');window.location='" + url + "';</script>");
            //    return;
            //}

            string createdate = DateTime.Parse(change_Class_Task.Rows[0]["modify_date"].ToString()).ToString("yyyy-MM-dd");
            string runsgroups = change_Class_Task.Rows[0]["team_id"].ToString();
            if (!RunsGroupId.Equals(runsgroups) || !DateTime.Now.ToString("yyyy-MM-dd").Equals(createdate))
            {
                Response.Redirect("Change_Class_Hint.aspx");
            }
        }
        #endregion

        #region 注释的代码
        ///// <summary>
        ///// 获取model区段的站台
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //private string GetDockNumber(string model)
        //{
        //    GetDockNumbers();
        //    StringBuilder dockNumber = new StringBuilder();
        //    if (dockNumberDt != null && dockNumberDt.Rows.Count != 0)
        //    {
        //        foreach (DataRow row in dockNumberDt.Rows)
        //        {
        //            if (model.Equals(row["MODELCODE"].ToString()))
        //            {
        //                dockNumber = dockNumber.Append(row["DOCKNUMBER"].ToString() + ",");
        //            }
        //        }
        //        if (dockNumber.Length > 0)
        //        {
        //            dockNumber = dockNumber.Remove(dockNumber.Length - 1, 1);
        //        }
        //    }

        //    return dockNumber.ToString();
        //}

        ///// <summary>
        ///// 获取本机台站台
        ///// </summary>
        //private void GetDockNumbers()
        //{
        //    dockNumberDt = commonBll.GetMachineDockInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, MachineNumber);
        //}

        ///// <summary>
        ///// 判断是否存在model区段的站台
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //private bool isGetDockNumber(string model)
        //{
        //    GetDockNumbers();
        //    if (dockNumberDt != null)
        //    {
        //        foreach (DataRow row in dockNumberDt.Rows)
        //        {
        //            if (model.Equals(row["MODELCODE"].ToString()))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}



        ////设置换牌
        //protected void btn_SetProduct_Click(object sender, EventArgs e)
        //{
        //    if (LoginUser.MachineNumber == "")
        //    {
        //        Session["IsToCall"] = "MachineProduct.aspx";
        //        string Url = "Set_Number.aspx";
        //        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请先设置机台！');window.location='" + Url + "';</script>");
        //        return;
        //    }

        //    Response.Redirect("MachineProduct.aspx");
        //} 
        #endregion
    }
}
