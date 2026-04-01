using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using System.Collections;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Change_Class_Accept : System.Web.UI.Page
    {
        private DataTable initData1
        {
            set { ViewState["initData1"] = value; }
            get { return ViewState["initData1"] as DataTable; }
        }

        private string BillId
        {
            get { return ViewState["BILLID"].ToString(); }
            set { ViewState["BILLID"] = value; }
        }

        /// <summary>
        /// 交接班站台id
        /// </summary>
        private string ChangeClassDock
        {
            set { ViewState["ChangeClassDock"] = value; }
            get
            {
                if (ViewState["ChangeClassDock"] == null)
                    return null;
                return ViewState["ChangeClassDock"].ToString();
            }
        }

        /// <summary>
        /// 是否交接人工领料数据（1:交；0:不交）
        /// </summary>
        private string IsChangePersonReceive
        {
            set { ViewState["IsChangePersonReceive"] = value; }
            get
            {
                if (ViewState["IsChangePersonReceive"] == null)
                    return null;
                return ViewState["IsChangePersonReceive"].ToString();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionKey"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            Response.Expires = -1;

            if (Session["MachineNumber"] == null)
            {
                string Url = "Set_Number.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请先设置机台信息!');window.location='" + Url + "';</script>");
                return;
            }

            if (!Page.IsPostBack)
            {
                label1.Text = (LoginUser.MachineNumber == "") ? "未设置" : LoginUser.MachineNumber;//MachineNumber;
                labdocknumber.Text = (LoginUser.MachineName == "") ? "未设置" : LoginUser.MachineName;
                labbc.Text = LoginUser.RunsTypeCode;
                label2.Text = (LoginUser.ItemName == "") ? "未设置" : LoginUser.ItemName;

                GetAndBindData();
            }
        }

        private void GetAndBindData()
        {
            ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

            DataTable dt = rpb.GetClassDockNumberByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.Token);
            if (dt == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无法访问服务器！');location.href='Control_Panel.aspx';</script>");
                return;
            }
            if (dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台未初始化交接班站台！');location.href='Control_Panel.aspx';</script>");
                return;
            }
            if (dt.Rows.Count != 1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台对应的交接班站台有多个，一个机台只能有一个交接班站台！');location.href='Control_Panel.aspx';</script>");
                return;
            }
            ChangeClassDock = dt.Rows[0]["dock_id"].ToString();
            IsChangePersonReceive = dt.Rows[0]["pallet_code"].ToString();//是否交接人工领料数据（1:交；0:不交）
            string docID = dt.Rows[0]["doc_id"].ToString();//交接班站台中的当前单据id存放的是交班的工单id

            DataTable initData = rpb.GetMachineCurItemsInfoWhenChangeClass(LoginUser.Url, IsChangePersonReceive, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.Token);
            //获取不到交班托盘信息，则无交班信息
            if ((initData == null) || (initData.Rows.Count == 0))
            {
                lv_Change_Class.DataSource = null;
                lv_Change_Class.DataBind();
                btn_Ensure_Next.Visible = false;
                btn_Modify.Visible = false;
                return;
            }
            //将initData放入viewstate中
            DataTable dtTaskItemInfo = rpb.GetTheNewItemNumByPlanOutId(LoginUser.Url, IsChangePersonReceive, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, docID, LoginUser.Token);
            //根据生产工单获取不到最新的机台要料/接班任务信息时，表明无接班信息
            if ((dtTaskItemInfo == null) || (dtTaskItemInfo.Rows.Count == 0))
            {
                lv_Change_Class.DataSource = null;
                lv_Change_Class.DataBind();
                btn_Ensure_Next.Visible = false;
                btn_Modify.Visible = false;
                return;
            }
            for (int i = 0; i < dtTaskItemInfo.Rows.Count; i++)
            {
                for (int k = 0; k < initData.Rows.Count; k++)
                {
                    if (dtTaskItemInfo.Rows[i]["container_d_id"].ToString() == initData.Rows[k]["containerDetailId"].ToString() && dtTaskItemInfo.Rows[i]["container_id"].ToString() == initData.Rows[k]["container_id"].ToString())
                    {
                        initData.Rows[k]["planQty"] = dtTaskItemInfo.Rows[i]["plan_qty"].ToString();
                    }
                }
            }            
            initData1 = initData;

            lv_Change_Class.DataSource = initData;
            lv_Change_Class.DataBind();

            //查询是否已经做交班
            ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            DataTable dtChangeClass = rbob.GetChangeClassAcceptInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, docID, ChangeClassDock, LoginUser.Token);

            if (dtChangeClass == null || dtChangeClass.Rows.Count == 0)
            {
                return;
            }
            if (dtChangeClass.Rows.Count != 1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台存在站台设关联了两个盘型，请进行修改！');location.href='Control_Panel.aspx';</script>");
                return;
            }

            string modifyDate = DateTime.Parse(dtChangeClass.Rows[0]["modify_date"].ToString()).ToString("yyyy-MM-dd");//交班时间
            string shiftId = dtChangeClass.Rows[0]["shift_id"].ToString().Trim();//交班班次id
            string teamId = dtChangeClass.Rows[0]["team_id"].ToString().Trim();//交班班组id
            string shiftName = dtChangeClass.Rows[0]["shiftName"].ToString().Trim();//交班班次名称
            string teamName = dtChangeClass.Rows[0]["teamName"].ToString().Trim();//交班班组名称
            label3.Text = DateTime.Parse(dtChangeClass.Rows[0]["modify_date"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
            lblRuns.Text = "(" + teamName + "-" + shiftName + ")";
            if (LoginUser.RunsCodeId == shiftId && LoginUser.RunsGroupId == teamId && DateTime.Now.ToString("yyyy-MM-dd").Equals(modifyDate))
            {
                btn_Ensure_Next.Visible = false;
                btn_Modify.Visible = false;
            }

            //BillId = initData.Rows[0]["BILLID"].ToString().Trim();
            //LoginUser.DockNumber = initData.Rows[0]["DOCK_NUMBER"].ToString().Trim();
           
        }

        /// <summary>
        /// 确认接收，即接班
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Ensure_Next_Click(object sender, EventArgs e)
        {
            ReturnPalletBLL rb = new ReturnPalletBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

            //string msg = rb.ClientConfirmReturnBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineNumber,
            //    LoginUser.DockNumber, LoginUser.SchemeId, BillId, LoginUser.RunsTypeCode);
            JsonObject jo = new JsonObject();
            JsonArray details = new JsonArray();
            foreach (DataRow row in initData1.Rows)
            {
                JsonObject obj = new JsonObject();
                obj.Add("containerId", row["container_id"].ToString());//容器明细id
                obj.Add("containerDetailId", row["containerDetailId"].ToString());//容器明细id
                obj.Add("itemId", row["itemId"].ToString());//物料id
                obj.Add("itemName", row["itemName"].ToString());//物料名称
                obj.Add("planQty", row["planQty"].ToString());//要料时物料的数量
                obj.Add("qty", row["qty"].ToString());//物料数量
                obj.Add("lotNo", row["lotNo"].ToString());//批次
                obj.Add("ownerId", row["ownerId"].ToString());//供应商
                details.Add(obj);
            }
            jo.Add("ItemList", details);

            string msg = string.Empty;
            try
            {
                msg = rb.ClientModifyReturnBill(LoginUser.Url, IsChangePersonReceive, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, jo.ToString(), LoginUser.HasLGV.ToString(), LoginUser.Token);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            if (string.IsNullOrEmpty(msg))
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('接班成功。');location.href='Control_Panel.aspx';</script>");
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>alert('接班失败！" + msg + "');</script>");
            }
        }

        //插入当班接班信息 PEQ 20110118
        public bool InsertClassInfo()
        {
            bool operate = false;

            //自动生成班次
            DateTime dtNow = System.DateTime.Now;
            string autoRunId = string.Format("{0:yyyyMMddHHmmss}", dtNow);
            autoRunId = autoRunId + "-" + LoginUser.MachineNumber;

            //生产日期
            DateTime dateNow = new DateTime();
            dateNow = System.DateTime.Now.AddHours(-7);
            string strDateNow = string.Format("{0:u}", dateNow);
            int dateIndex = strDateNow.ToString().IndexOf(" ");
            string productNow = strDateNow.Substring(0, dateIndex).Trim();

            //BLL层
            CommonBLL commonBll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

            string runId = autoRunId;
            //string teamDate = LoginUser.RunsTypeCode;//班组
            string classState = "未交班";//状态
            string remark = "用户名";
            string autorunId = "";
            string productdate = productNow;

            //获取上一班已经交班的班次
            DataTable dsPreClass = commonBll.GetPreClassInfo(LoginUser.WarehouseID, LoginUser.MachineNumber, LoginUser.WorkShop, "已交班").Tables[0];
            string preClass = string.Empty;
            if (dsPreClass != null && dsPreClass.Rows.Count > 0)
                preClass = dsPreClass.Rows[0]["FIELD2"].ToString();

            try
            {
                //插入当前班的信息
                operate = commonBll.InsertClassInfo(LoginUser.WarehouseID, runId, LoginUser.WorkShop, LoginUser.RunsCode, classState,
                    LoginUser.MachineNumber, remark, autorunId, productdate, preClass);
                //修改上一班的状态
                operate = this.UpdatePreClassStatus(LoginUser.WarehouseID, LoginUser.WorkShop, LoginUser.MachineNumber, preClass, "已完成", autorunId);

                return operate;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 修改上一班次的状态
        /// </summary>
        public bool UpdatePreClassStatus(string WarehouseID, string WorkShop, string MachineNumber, string RunID, string ClassState, string nextClass)
        {
            CommonBLL commonBll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

            int updateClassStatus = commonBll.UpdateStatus(WarehouseID, WorkShop, MachineNumber, RunID, ClassState, nextClass);
            if (updateClassStatus > 0) return true;
            else
                return false;
        }

        private void UpdateContainerDetailsQty(TextBox tb, string containerDetailId)
        {
            //ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            //if (string.IsNullOrEmpty(containerDetailId)) return;
            ////否则直接修改containerdetail的数据
            ////获取textbox中的数值
            //string count = tb.Text;
            ////注销修改containerdetail的数据  peq 20110426
            //rbob.UpdateReturnItemQty(containerDetailId, decimal.Parse(count), 0, 0, 0);
            ////GetAndBindData();


            //if (string.IsNullOrEmpty(containerDetailId)) return;
            ////否则直接修改containerdetail的数据
            ////获取textbox中的数值
            //string count = tb.Text;
            //if (lv_Change_Class.Rows == null) return;
            //lv_Change_Class = this.GetReturnItem(decimal.Parse(count), initData1, containerDetailId);//containerDetail
            //this.lv_Change_Class.DataSource = lv_Change_Class;//containerDetail
            //this.lv_Change_Class.DataBind();
        }

        protected void btn_Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        protected void GridViewReturnBill_ToNext_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Change_Class.PageIndex = e.NewPageIndex;

            if ((initData1 == null) || (initData1.Rows.Count == 0))
            {
                lv_Change_Class.DataSource = null;
                lv_Change_Class.DataBind();
                btn_Ensure_Next.Visible = false;
                btn_Modify.Visible = false;
                return;
            }
            lv_Change_Class.DataSource = initData1;
            lv_Change_Class.DataBind();
            //CheckQty();
        }

        #region 检查余料数量是否符合格式要求
        /// <summary>
        /// 检查退余料数量是否符合格式要求
        /// </summary>
        /// <returns>false：不符合；true：符合</returns>
        private bool CheckQty()
        {
            bool bFind = false;
            foreach (GridViewRow row in lv_Change_Class.Rows)
            {
                if (bFind)
                {
                    break;
                }
                TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
                string ContainerDetailID = txtContainerDetailID.Text.Trim();
                TextBox textbox = (TextBox)row.FindControl("txtQTY");
                if (!string.IsNullOrEmpty(textbox.Text))
                {
                    if (CommonFuctions.CheckDecemal(textbox.Text.Trim()))
                    {
                        this.UpdateContainerDetailsQty(textbox, ContainerDetailID);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确,请重新输入!');</script>");
                        bFind = true;
                        break;
                    }
                }
            }
            if (bFind)
            {
                return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 修改接班数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Modify_Click(object sender, EventArgs e)
        {
            Response.Redirect("Change_Class_View.aspx");
        }
    }
}