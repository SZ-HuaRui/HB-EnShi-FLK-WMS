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
    public partial class Materials_Hand : System.Web.UI.Page
    {
        private static string dockNumber = string.Empty;
        private static string dockName = string.Empty;
        private static DataTable docks;
        private static DataSet bill_Hand;
        private static GridViewRow drv;
        private CommonBLL commonBll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private MaterialsBill_HandWork_BLL mhb = new MaterialsBill_HandWork_BLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        static string UserName;
        static string UserPassword;
        static string WareHouseID;
        static string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, hasLGV;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["Equipment"] == null || Session["UserName"] == null ||Session["WarehouseID"] == null || Session["RunsGroup"] == null)
            //{
            //    Response.Redirect("pass.aspx");
            //}
            //else
            //{
            UserName = Session["UserName"].ToString();
            //  UserPassword = Session["PassWord"].ToString();
            WareHouseID = Session["WarehouseID"].ToString();
            Equipment = Session["Equipment"].ToString();
            WorkShop = Session["WorkShop"].ToString(); //车间 
            Run = Session["Runs"].ToString(); //班组 
            //MachineNumber = Session["MachineNumber"].ToString();
            RunsGroup = Session["RunsGroup"].ToString();
            hasLGV = Session["HasLGV"].ToString();
            // }
            if (Session["MachineNumber"] == null)
            {
                string Url = "Control_Panel.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无机台信息，请先到机台设置中设置机台信息!');window.location='" + Url + "';</script>");
                return;
            }
            else
            {
                MachineNumber = Session["MachineNumber"].ToString();
            }
            if (Session["MachineName"] != null) MachineName = Session["MachineName"].ToString();
            if (!IsPostBack)
            {
                GetDockDataSet();
                GetMaterialsBill_Hand();
                BindingListView();
                Label1.Text = MachineNumber + "机台 人工领料";
                Label2.Text = (MachineNumber == null) ? "未设置" : MachineNumber;//机台编号
                labdocknumber.Text = (MachineName == null) ? "未设置" : MachineName;//机台名称

                Label3.Text = (LoginUser.ItemName == null) ? "未设置" : LoginUser.ItemName;//生产牌号
                string ClassGroup = XmlReader.GetClassGroup(); //班次
                labbc.Text = RunsGroup;
            }
        }
        #region 数据及其绑定
        public void GetDockDataSet()
        {
            docks = commonBll.GetMachineDockInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, MachineNumber);
            foreach (DataRow row in docks.Rows)
            {
                if (Properties.Settings.Default.ContainerTypeCode_Handle.Equals(row["MODELCODE"].ToString()))
                {
                    dockName = row["DOCKNAME"].ToString();
                    dockNumber = row["DOCKNUMBER"].ToString();
                }
            }
        }
        public void GetMaterialsBill_Hand()
        {
            bill_Hand = mhb.GetMaterialsBill_HandWork_Warehouse(MachineNumber);
            if (bill_Hand == null || bill_Hand.Tables[0].Rows.Count <= 0)
            {
                btn_All_Ensure.Enabled = false;
                btn_Ensure.Enabled = false;
                return;
            }
            foreach (DataRow row in bill_Hand.Tables[0].Rows)
            {
                if ((row["STATECODE"].ToString()).Equals(HandStates.HandStates_New))
                {
                    row["ODD1"] = "0";
                }
            }
            btn_Ensure.Enabled = true;
            btn_All_Ensure.Enabled = true;
        }
        public void BindingListView()
        {
            if (bill_Hand == null || bill_Hand.Tables[0].Rows.Count == 0 || bill_Hand.Tables.Count <= 0)
            {
                lv_Materials_Hand.DataSource = null;
                lv_Materials_Hand.DataBind();
                return;
            }
            else
            {
                lv_Materials_Hand.DataSource = bill_Hand.Tables[0].DefaultView;
                lv_Materials_Hand.DataBind();
            }
        }
        #endregion

        protected void btn_Create_Click(object sender, EventArgs e)
        {
            if (commonBll.CheckDockState(MachineNumber, dockNumber) == false)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该站台存在使用中,不允许进行人工要料操作!');</script>");
                return;
            }
            Response.Redirect("Materials_Select.aspx?DockNumber=" + dockNumber + "&DockName=" + dockName);
            //Materials_Select ms = new Materials_Select();
            //this.Hide();
            //ms.ShowDialog();
            //btn_Fresh_Click(null, null);
            //this.ShowDialog();
        }

        protected void btn_Fresh_Click(object sender, EventArgs e)
        {
            GetMaterialsBill_Hand();
            BindingListView();
        }

        protected void chkSelect_select(object sender, EventArgs e)
        {
            drv = (GridViewRow)((CheckBox)sender).Parent.Parent;
            string itemcode = ((Label)drv.FindControl("CItemCode")).Text.ToString();
            foreach (GridViewRow grv in lv_Materials_Hand.Rows)
            {
                string othercode = ((Label)grv.FindControl("CItemCode")).Text.ToString();
                if (itemcode != othercode)
                {
                    ((CheckBox)grv.FindControl("chkSelect")).Checked = false;
                }
                else
                {
                    ((CheckBox)grv.FindControl("chkSelect")).Checked = true;
                }
            }
        }

        protected void btn_Ensure_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow grv in lv_Materials_Hand.Rows)
            {
                CheckBox cb = (CheckBox)grv.FindControl("chkSelect");
                if (cb.Checked == true)
                {
                    string itemname = ((TextBox)grv.FindControl("CItemName")).Text.Trim();
                    string s_qty = ((TextBox)grv.FindControl("CFactCount")).Text.Trim();
                    string odd1 = ((TextBox)grv.FindControl("CODD1")).Text.Trim();
                    string odd2 = ((TextBox)grv.FindControl("CODD2")).Text.Trim();
                    string odd3 = ((TextBox)grv.FindControl("CODD3")).Text.Trim();

                    if (Ensure_Accept(grv) > 0)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作成功!');</script>");
                        //btn_Fresh_Click(null, null);
                        GetMaterialsBill_Hand();
                        BindingListView();
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作失败,请重试!');</script>");
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请选择人工领料的物料单!');</script>");
                }
            }
        }
        private int Ensure_Accept(GridViewRow drv)
        {
            string billID = ((TextBox)drv.FindControl("BILLID")).Text.ToString();//Row["BILLID"].ToString();
            decimal count = decimal.Parse(((TextBox)drv.FindControl("CFactCount")).Text.ToString());
            decimal odd1 = decimal.Parse(((TextBox)drv.FindControl("CODD1")).Text.ToString());
            decimal odd2 = decimal.Parse(((TextBox)drv.FindControl("CODD2")).Text.ToString());
            decimal odd3 = decimal.Parse(((TextBox)drv.FindControl("CODD3")).Text.ToString());
            //decimal odd1 = decimal.Parse(drv.Row["ODD1"].ToString());
            //decimal odd2 = decimal.Parse(drv.Row["ODD2"].ToString());
            //decimal odd3 = decimal.Parse(drv.Row["ODD3"].ToString());
            decimal qty = count + odd1 + odd2 + odd3;
            string states = ((TextBox)drv.FindControl("STATECODE")).Text.ToString();

            if (!billID.Equals(string.Empty) && qty >= 0 && states.Equals(HandStates.HandStates_Send))
            {
                return mhb.Client_AcceptMaterials_HandWork(MachineNumber, dockNumber, billID, LoginUser.RunsTypeCode);
            }
            else
            {
                return -1;
            }
        }

        protected void btn_All_Ensure_Click(object sender, EventArgs e)
        {
            string succ_Accept = string.Empty;
            string fail_Accept = string.Empty;

            //for (int i = 0; i < lv_Materials_Hand.Rows.Count; i++)
            //{
            foreach (GridViewRow drv in lv_Materials_Hand.Rows)
            {
                string itemName = ((TextBox)drv.FindControl("CItemName")).Text.ToString();
                if (Ensure_Accept(drv) > 0)
                {
                    succ_Accept += itemName;
                    succ_Accept += "、";
                }
                else
                {
                    fail_Accept += itemName;
                    fail_Accept += "、";
                }
            }
            if (succ_Accept != string.Empty)
            {
                succ_Accept = succ_Accept.Substring(0, (succ_Accept.Length - 1));
            }
            if (fail_Accept != string.Empty)
            {
                fail_Accept = fail_Accept.Substring(0, fail_Accept.Length - 1);
            }
            if (fail_Accept != string.Empty && succ_Accept != string.Empty)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作成功," + fail_Accept + ":接收物料操作失败,请重试!');</script>");
                //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT, "“" + succ_Accept + "”" + NTIMessage.ACCEPT_SUCC + "," + "“" + fail_Accept + "”" + NTIMessage.ACCEPT_ERROR, Hint.WARNING);
            }
            else if (succ_Accept != string.Empty)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作成功!');</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作失败,请重试!');</script>");
            }
            btn_Fresh_Click(null, null);
        }

        protected void btn_Cancle_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        protected void lv_Materials_Hand_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Materials_Hand.PageIndex = e.NewPageIndex;
            GetMaterialsBill_Hand();
        }
    }
}