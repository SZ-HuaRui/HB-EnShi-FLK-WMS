using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Windows.Forms;
using NTI.WMS.CallTerminalSystem.WebCallEnd.Common;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Set_Number_new : System.Web.UI.Page
    {
        CommonBLL bll = new CommonBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionKey"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            if (!Page.IsPostBack)
            {
                if (Session["HasLGV"] != null)
                {
                    this.checkBox_LGV.Checked = Session["HasLGV"].ToString() == "1";
                }
                else
                {
                    this.checkBox_LGV.Checked = false;
                }
                SetMachineNumber("");  //机台
                //BindRunsCode();//班次
                //BindRunsGroup();//班组
            }
        }


        #region 机台
        /// <summary>
        /// 获取机台
        /// </summary>
        /// <param name="workShop"></param>
        public void SetMachineNumber(string workShop)
        {
            DataTable dtMachine = bll.GetMachineDataByWorkShopType(LoginUser.Url, LoginUser.UserName, Session["SessionKey"].ToString(), Session["Token"].ToString(), LoginUser.WarehouseID, workShop);
            //ddlMachine.DataValueField = "EQUIPMENT_NUMBER";
            //ddlMachine.DataTextField = "EQUIPMENT_NAME";
            ddlMachine.DataValueField = "t_id";
            ddlMachine.DataTextField = "equipment_name";
            ddlMachine.DataSource = dtMachine;
            ddlMachine.DataBind();

            if (LoginUser.MachineId != "" && workShop == "")
            {
                ddlMachine.SelectedValue = LoginUser.MachineId;
            }
        }
        #endregion

        #region 确定
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Ensure_Click(object sender, EventArgs e)
        {
            if (ddlMachine.SelectedValue == "")
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('请选择机台！');</script>");
                return;
            }
            //if (ddlRunsCode.SelectedValue == "")
            //{
            //    ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('请选择班次！');</script>");
            //    return;
            //}
            //if (ddlRunsGroup.SelectedValue == "")
            //{
            //    ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('请选择班组！');</script>");
            //    return;
            //}

            CommonBLL bll = new CommonBLL();
            //DataTable dt = bll.GetMachineCurPlanByMachineNumber(LoginUser.Url, LoginUser.SessionKey, ddlMachine.SelectedValue, ddlRunsCode.SelectedValue, ddlRunsGroup.SelectedValue, LoginUser.Token);
            DataTable dt = bll.GetMachineCurPlanByMachineNumber(LoginUser.Url, LoginUser.SessionKey, ddlMachine.SelectedValue, "", "", LoginUser.Token);
            if (dt == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('获取生产牌号失败！');</script>");
                return;
            }
            if (dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台未设定生产牌号！');</script>");
                return;
            }
            if (dt.Rows.Count != 1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台设定了多个生产工单！');</script>");
                return;
            }

            LoginUser.MachineId = ddlMachine.SelectedValue;
            LoginUser.MachineNumber = ddlMachine.SelectedItem.Text;
            LoginUser.MachineName = ddlMachine.SelectedItem.Text;
            LoginUser.RunsCode = dt.Rows[0]["shift_name"].ToString() ;//shift_name
            LoginUser.RunsCodeId = dt.Rows[0]["shift_id"].ToString();//shift_id
            LoginUser.RunsGroup = dt.Rows[0]["team_name"].ToString();//team_name
            LoginUser.RunsGroupId = dt.Rows[0]["team_id"].ToString();//team_id
            LoginUser.RunsTypeCode = LoginUser.RunsGroup + "-" + LoginUser.RunsCode;

            #region LGV
            if (checkBox_LGV.Checked == true)
            {
                LoginUser.HasLGV = 1;
            }
            else
            {
                LoginUser.HasLGV = 0;
            }
            #endregion

            //bool isHave = isHavePlanInfo();
            //if (isHave == false)
            //{
            //    return;
            //}
            
            Response.Redirect("Control_Panel.aspx");

        }
        #endregion

        #region 返回
        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Cancle_Click(object sender, EventArgs e)
        {
            //bool isHave = isHavePlanInfo();
            //if (isHave == false)
            //{
            //    return;
            //}
            Response.Redirect("Control_Panel.aspx");
        }
        #endregion

        #region 退出
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Exit_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Call_Terminal_Log.aspx");
        }
        #endregion

        #region 废弃代码
        //#region 获取是否设置工单信息
        ///// <summary>
        ///// 获取是否设置工单信息
        ///// </summary>
        ///// <returns></returns>
        //public bool isHavePlanInfo()
        //{
        //    DataTable dt = bll.GetMachineCurPlanByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.MachineId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, LoginUser.Token);
        //    if (dt == null)
        //    {
        //        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('获取生产牌号失败！');</script>");
        //        return false;
        //    }
        //    if (dt.Rows.Count == 0)
        //    {
        //        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台未设定生产牌号！');</script>");
        //        return false;
        //    }
        //    return true;
        //}
        //#endregion

        //#region 班次
        ///// <summary>
        ///// 绑定班次
        ///// </summary>
        //private void BindRunsCode()
        //{
        //    string msg = string.Empty;
        //    DataTable dtRuns = bll.GetRunsCode(LoginUser.Url, Session["SessionKey"].ToString(), Session["Token"].ToString(), out msg);
        //    if (!string.IsNullOrEmpty(msg))
        //    {
        //        ClientScript.RegisterStartupScript(Page.GetType(), "", $"<script>alert('{msg}');</script>");
        //        return;
        //    }
        //    if (dtRuns == null)
        //    {
        //        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('连接服务器失败！');</script>");
        //        return;
        //    }
        //    if (dtRuns.Rows.Count == 0)
        //    {
        //        if (dtRuns.Columns.Count == 0)
        //        {
        //            dtRuns.Columns.Add("t_id", typeof(string));
        //            dtRuns.Columns.Add("shift_code", typeof(string));
        //            dtRuns.Columns.Add("shift_name", typeof(string));
        //        }
        //        InsertRunsCodeItem(dtRuns);
        //    }
        //    LoadRunsCodeItem(dtRuns, ddlRunsCode, "RunsCode", LoginUser.RunsCode);
        //}

        ///// <summary>
        ///// 初始化班次下拉框信息
        ///// </summary>
        ///// <param name="dtRuns"></param>
        ///// <param name="type"></param>
        //private static void InsertRunsCodeItem(DataTable dtRuns)
        //{
        //    DataRow row = dtRuns.NewRow();
        //    row["t_id"] = "";
        //    row["shift_name"] = "未初始化";
        //    row["shift_code"] = "未初始化";
        //    dtRuns.Rows.Add(row);
        //}
        ///// <summary>
        ///// 加载班次下拉框信息
        ///// </summary>
        ///// <param name="dtRuns"></param>
        ///// <param name="ddl"></param>
        ///// <param name="type"></param>
        ///// <param name="selected"></param>
        //private void LoadRunsCodeItem(DataTable dtRuns, DropDownList ddl, string type, string selected)
        //{
        //    DataView dvRunsCode = new DataView(dtRuns);
        //    //dvRunsCode.RowFilter = "TYPE_CODE = '" + type + "'";
        //    ddl.DataValueField = "t_id";
        //    ddl.DataTextField = "shift_name";
        //    ddl.DataSource = dvRunsCode;
        //    ddl.DataBind();
        //    if (ddl.Items.Count == 1 && ddl.SelectedValue == "")
        //    {
        //        ddl.Enabled = false;
        //        return;
        //    }
        //    if (selected == "") return;

        //    foreach (ListItem item in ddl.Items)
        //    {
        //        if (item.Text == selected)
        //        {
        //            item.Selected = true;
        //            break;
        //        }
        //    }
        //}
        //#endregion

        //#region 班组
        ///// <summary>
        ///// 绑定班组
        ///// </summary>
        //private void BindRunsGroup()
        //{
        //    DataTable dtRuns = bll.GetRunsGroup(LoginUser.Url, Session["SessionKey"].ToString(), Session["Token"].ToString());
        //    if (dtRuns == null)
        //    {
        //        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('连接服务器失败！');</script>");
        //        return;
        //    }
        //    if (dtRuns.Rows.Count == 0)
        //    {
        //        #region
        //        if (dtRuns.Columns.Count == 0)
        //        {
        //            dtRuns.Columns.Add("t_id", typeof(string));
        //            dtRuns.Columns.Add("team_code", typeof(string));
        //            dtRuns.Columns.Add("team_name", typeof(string));
        //        }
        //        InsertRunsGroupItem(dtRuns);
        //        #endregion
        //    }
        //    LoadRunsGroupItem(dtRuns, ddlRunsGroup, "RunsGroup", LoginUser.RunsGroup);
        //}

        ///// <summary>
        ///// 初始化班组下拉框信息
        ///// </summary>
        ///// <param name="dtRuns"></param>
        ///// <param name="type"></param>
        //private static void InsertRunsGroupItem(DataTable dtRuns)
        //{
        //    DataRow row = dtRuns.NewRow();
        //    row["t_id"] = "";
        //    row["team_name"] = "未初始化";
        //    row["team_code"] = "未初始化";
        //    dtRuns.Rows.Add(row);
        //}
        ///// <summary>
        ///// 加载班组下拉框信息
        ///// </summary>
        ///// <param name="dtRuns"></param>
        ///// <param name="ddl"></param>
        ///// <param name="type"></param>
        ///// <param name="selected"></param>
        //private void LoadRunsGroupItem(DataTable dtRuns, DropDownList ddl, string type, string selected)
        //{
        //    DataView dvRunsCode = new DataView(dtRuns);
        //    //dvRunsCode.RowFilter = "TYPE_CODE = '" + type + "'";
        //    ddl.DataValueField = "t_id";
        //    ddl.DataTextField = "team_name";
        //    ddl.DataSource = dvRunsCode;
        //    ddl.DataBind();
        //    if (ddl.Items.Count == 1 && ddl.SelectedValue == "")
        //    {
        //        ddl.Enabled = false;
        //        return;
        //    }
        //    if (selected == "") return;

        //    foreach (ListItem item in ddl.Items)
        //    {
        //        if (item.Text == selected)
        //        {
        //            item.Selected = true;
        //            break;
        //        }
        //    }
        //}
        //#endregion 
        #endregion
    }
}