using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NTI.WMS.CallTerminalSystem.BLL;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Change_Class_View : System.Web.UI.Page
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

            Response.AddHeader("P3P", "CP=CAO PSAOUR");

            Response.Expires = -1;

            if (Session["MachineNumber"] == null)
            {
                Session["IsToCall"] = "Change_Class_View.aspx";
                string Url = "Set_Number.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请先设置机台!');window.location='" + Url + "';</script>");
                return;
            }


            if (!Page.IsPostBack)
            {
                label1.Text = (LoginUser.MachineNumber == "") ? "未设置" : LoginUser.MachineNumber;
                labdocknumber.Text = (LoginUser.MachineName == "") ? "未设置" : LoginUser.MachineName;
                labbc.Text = LoginUser.RunsTypeCode;
                label2.Text = LoginUser.ItemName == "" ? "未设置" : LoginUser.ItemName;

                btn_Return.Visible = true;

                GetAndBindData();

                //BindDropDownList();
                //CheckEnabledEdit();
            }
        }

        #region 加载页面数据
        /// <summary>
        /// 加载页面数据
        /// </summary>
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
            //将initData放入viewstate中
            DataTable dtTaskItemInfo = rpb.GetTheNewItemNumByPlanOutId(LoginUser.Url, IsChangePersonReceive, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, docID, LoginUser.Token);
            for (int i = 0; i < dtTaskItemInfo.Rows.Count; i++)
            {
                for (int k = 0; k < initData.Rows.Count; k++)
                {
                    if (dtTaskItemInfo.Rows[i]["container_d_id"].ToString() == initData.Rows[k]["containerDetailId"].ToString())
                    {
                        initData.Rows[k]["planQty"] = dtTaskItemInfo.Rows[i]["plan_qty"].ToString();
                    }
                }
            }
            initData1 = initData;


            if ((initData == null) || (initData.Rows.Count == 0))
            {
                lv_Change_Class.DataSource = null;
                lv_Change_Class.DataBind();
                btn_Ensure_Next.Visible = false;
                return;
            }
            lv_Change_Class.DataSource = initData;
            lv_Change_Class.DataBind();

            //查询是否已经做交班
            ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            DataTable dtChangeClass = rbob.GetMachineReturnMaterialBillWhenChangeClass(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, docID, ChangeClassDock, LoginUser.Token);

            if (dtChangeClass == null || dtChangeClass.Rows.Count == 0)
            {
                return;
            }
            if (dtChangeClass.Rows.Count != 1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台存在站台设关联了两个盘型，请进行修改！');location.href='Control_Panel.aspx';</script>");
                return;
            }

            //string modifyDate = DateTime.Parse(dtChangeClass.Rows[0]["modify_date"].ToString()).ToString("yyyy-MM-dd");//交班时间
            //string shiftId = dtChangeClass.Rows[0]["shift_id"].ToString().Trim();//交班班次id
            //string teamId = dtChangeClass.Rows[0]["team_id"].ToString().Trim();//交班班组id
            string shiftName = dtChangeClass.Rows[0]["shiftName"].ToString().Trim();//交班班次名称
            string teamName = dtChangeClass.Rows[0]["teamName"].ToString().Trim();//交班班组名称
            label3.Text = DateTime.Parse(dtChangeClass.Rows[0]["modify_date"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
            lblRuns.Text = "(" + teamName + "-" + shiftName + ")";
            //if (LoginUser.RunsCodeId == shiftId && LoginUser.RunsGroupId == teamId && DateTime.Now.ToString("yyyy-MM-dd").Equals(modifyDate))
            //{
            //    btn_Ensure_Next.Visible = false;
            //}

        } 
        #endregion

        #region 翻页
        /// <summary>
        /// 翻页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewReturnBill_ToNext_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Change_Class.PageIndex = e.NewPageIndex;
            CheckQty();
        } 
        #endregion

        #region 检查余料数量是否符合格式要求
        /// <summary>
        /// 检查余料数量是否符合格式要求
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

        #region 确定
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Ensure_Next_Click(object sender, EventArgs e)
        {
            if (TextBox2.Text.ToString().Trim() == "" || TextBox2.Text.ToString().Trim() == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请输入密码！');</script>");
                TextBox2.Visible = true;
                return;
            }
            if (!TextBox2.Text.ToString().Trim().Equals(Properties.Settings.Default.AdminPassword))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('密码错误！');</script>");
                return;
            }
            if (lv_Change_Class.Rows != null)
            {
                if (!CheckQty())
                {
                    return;
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没有物料，无法进行接班！');</script>");
                return;
            }
            ReturnPalletBLL rb = new ReturnPalletBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

            JsonObject jo = new JsonObject();
            JsonArray details = new JsonArray();
            bool isAllZero = true;
            foreach (DataRow row in initData1.Rows)
            {
                decimal qty = decimal.Parse(row["qty"].ToString());

                if (qty >= 0)
                {
                    isAllZero = false;
                    decimal planQty = Convert.ToDecimal(row["planQty"]);

                    if (qty > planQty)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('“" + row["itemName"] + "”，修改的物料数量不能超出标准数量 " + planQty + " , 请重新输入!');</script>");
                        return;
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('“" + row["itemName"] + "”，修改的物料数量不能为负数, 请重新输入!');</script>");
                    return;
                }

                JsonObject obj = new JsonObject();
                obj.Add("containerDetailId", row["containerDetailId"].ToString());//容器明细id
                obj.Add("itemId", row["itemId"].ToString());//物料id
                obj.Add("itemName", row["itemName"].ToString());//物料名称
                obj.Add("planQty", row["planQty"].ToString());//要料时物料的数量
                obj.Add("qty", qty);//修改的物料数量
                obj.Add("lotNo", row["lotNo"].ToString());//批次
                obj.Add("ownerId", row["ownerId"].ToString());//供应商
                details.Add(obj);
            }
            //if (isAllZero == true)
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('所有物料的修改后的数量不能全为0，请退空托盘！');</script>");
            //    return;
            //}

            jo.Add("ItemList", details);
            string msg = string.Empty;
            try
            {
                msg = rb.EditReturnBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, jo.ToString(), LoginUser.HasLGV.ToString(), LoginUser.Token);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            if (string.IsNullOrEmpty(msg))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('交班信息修改成功。');window.location='Change_Class_Accept.aspx';</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('修改失败！" + msg + "');</script>");
            }
        } 
        #endregion

        #region 修改接班时的物料数量
        /// <summary>
        /// 修改接班时的物料数量
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="containerDetailId"></param>
        private void UpdateContainerDetailsQty(TextBox tb, string containerDetailId)
        {
            if (string.IsNullOrEmpty(containerDetailId)) return;
            //否则直接修改containerdetail的数据
            //获取textbox中的数值
            string count = tb.Text;
            if (lv_Change_Class.Rows == null) return;
            initData1 = this.GetReturnItem(decimal.Parse(count), initData1, containerDetailId);//containerDetail
            this.lv_Change_Class.DataSource = initData1;//containerDetail
            this.lv_Change_Class.DataBind();
        } 
        #endregion

        #region 根据容器明细id，修改接班余料中物料的数量
        /// <summary>
        /// 根据容器明细id，修改接班余料中物料的数量
        /// </summary>
        /// <param name="qty">余料数量</param>
        /// <param name="dtContainerDetails">余料界面数据源</param>
        /// <param name="containerDetailId">容器明细id</param>
        /// <returns></returns>
        public DataTable GetReturnItem(decimal qty, DataTable dtContainerDetails, string containerDetailId)
        {
            DataTable dtItem = new DataTable();
            try
            {
                if (dtContainerDetails.Rows.Count <= 0) return null;
                dtItem = dtContainerDetails.Clone();
                DataRow drItem = null;

                //如果containerdetailid为空则修改所有的containerdetail的数据
                if (string.IsNullOrEmpty(containerDetailId))
                {
                    foreach (DataRow dr in dtContainerDetails.Rows)
                    {
                        dr.BeginEdit();
                        dr["QTY"] = qty;
                        dr.EndEdit();
                        drItem = dtItem.NewRow();
                        drItem = dr;
                        dtItem.Rows.Add(drItem.ItemArray);
                    }
                }
                else
                {
                    foreach (DataRow dr in dtContainerDetails.Rows)
                    {
                        if (dr["containerDetailId"] != null && dr["containerDetailId"].ToString().Equals(containerDetailId))
                        {
                            dr.BeginEdit();
                            dr["QTY"] = qty;
                            dr.EndEdit();

                        }
                        drItem = dtItem.NewRow();
                        drItem = dr;
                        dtItem.Rows.Add(drItem.ItemArray);
                    }
                }

            }
            catch { return null; }
            return dtItem;
        }
        #endregion

        #region 返回
        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Change_Class_Accept.aspx");
        } 
        #endregion

        protected void btn_Accept_Click(object sender, EventArgs e)
        {
            Response.Redirect("Change_Class_Accept.aspx");
        }

    }
}