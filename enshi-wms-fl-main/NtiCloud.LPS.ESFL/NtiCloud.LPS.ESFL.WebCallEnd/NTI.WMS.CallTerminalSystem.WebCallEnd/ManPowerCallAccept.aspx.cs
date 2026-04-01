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
    public partial class ManPowerCallAccept : System.Web.UI.Page
    {
        string UserName;
        string UserPassword;
        string WareHouseID;
        string ItemID, ItemCode, ItemName, StateCode;
        string runsGroup;
        string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, hasLGV, RunsTypeCode;
        DataSet bill_Hand;
        GridViewRow drv;
        ArrayList list = new ArrayList();
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSAOUR");
            if (Session["productitemname"] == null || Session["WorkShop"] == null || Session["Equipment"] == null || Session["UserName"] == null || Session["WarehouseID"] == null || Session["RunsGroup"] == null || Session["MachineNumber"] == null)
            {
                Response.Redirect("pass.aspx");
            }
            else
            {
                UserName = Session["UserName"].ToString();
                LoginUser.UserName = Session["UserName"].ToString();
                //UserPassword = Session["PassWord"].ToString();
                WareHouseID = Session["WarehouseID"].ToString();
                LoginUser.WarehouseID = WareHouseID;
                Equipment = Session["Equipment"].ToString();
                LoginUser.EquipmentNumber = Equipment;
                WorkShop = Session["WorkShop"].ToString(); //车间 
                Run = Session["Runs"].ToString(); //班次 
                RunsGroup = Session["RunsGroup"].ToString();//班组
                Response.Expires = -1;
                ItemName = Session["productitemname"].ToString();
                // }
                //modified by pengenqi 2012/5/24
                string Url = "Control_Panel.aspx";
                if (Session["MachineNumber"] == null)
                {
                }
                else
                {
                    MachineNumber = Session["MachineNumber"].ToString();
                    LoginUser.MachineNumber = MachineNumber;
                }

                if (Session["MachineName"] != null)
                {
                    MachineName = Session["MachineName"].ToString();
                    LoginUser.MachineName = MachineName;
                }
                if (!IsPostBack)
                {
                    switch (Run)
                    {
                        case "1":
                            Run = "后夜班";
                            break;
                        case "2":
                            Run = "上午班";
                            break;
                        case "3":
                            Run = "下午班";
                            break;
                        case "4":
                            Run = "前夜班";
                            break;
                    }
                    LoginUser.RunsCode = Run;
                    switch (RunsGroup)
                    {
                        case "1":
                            RunsGroup = "甲班";
                            break;
                        case "2":
                            RunsGroup = "乙班";
                            break;
                        case "3":
                            RunsGroup = "丙班";
                            break; ;
                        case "4":
                            RunsGroup = "丁班";
                            break;
                    }
                    GetMaterialsBill_Hand();
                    BindingListView();
                    Label1.Text = MachineNumber + "机台 人工领料";
                    Label2.Text = (MachineNumber == null) ? "未设置" : MachineNumber;//机台编号
                    labdocknumber.Text = (MachineName == null) ? "未设置" : MachineName;//机台名称

                    Label3.Text = (ItemName == null) ? "未设置" : ItemName;//生产牌号
                    labbc.Text = RunsGroup + "-" + Run;
                }
            }
        }

        public void GetMaterialsBill_Hand()
        {
            MaterialsBill_HandWork_BLL mhb = new MaterialsBill_HandWork_BLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            bill_Hand = mhb.GetMaterialsBill_HandWork();
            if (bill_Hand == null || bill_Hand.Tables[0].Rows.Count <= 0)
            {
                btn_All_Ensure.Enabled = false;
                //btn_Ensure.Enabled = false;
                return;
            }
            bill_Hand.Tables[0].DefaultView.RowFilter = "MACHINENUMBER='"+MachineNumber+"'";
            bill_Hand.Tables[0].DefaultView.RowFilter = "Statecode='Task_Sent'";
            //foreach (DataRow row in bill_Hand.Tables[0].Rows)
            //{
            //    if ((row["STATECODE"].ToString()).Equals(HandStates.HandStates_New))
            //    {
            //        row["ODD1"] = "0";
            //    }
            //}
            //btn_Ensure.Enabled = true;
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

        protected void chkSelect_select(object sender, EventArgs e)
        {
            drv = (GridViewRow)((CheckBox)sender).Parent.Parent;
            string docknumber = ((Label)drv.FindControl("Machinedock")).Text.ToString();
            foreach (GridViewRow grv in lv_Materials_Hand.Rows)
            {
                string othercode = ((Label)grv.FindControl("Machinedock")).Text.ToString();
                if (docknumber != othercode)
                {
                    ((CheckBox)grv.FindControl("chkSelect")).Checked = false;
                }
                else
                {
                    ((CheckBox)grv.FindControl("chkSelect")).Checked = true;
                }
            }
        }

        protected void chkSelectAll_select(object sender, EventArgs e)
        {
            drv = (GridViewRow)((CheckBox)sender).Parent.Parent;
            foreach (GridViewRow grv in lv_Materials_Hand.Rows)
            {

                ((CheckBox)grv.FindControl("chkSelect")).Checked = true;
            }
        }

        protected void btn_All_Ensure_Click(object sender, EventArgs e)
        {
            MaterialsBill_HandWork_BLL mhb = new MaterialsBill_HandWork_BLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            string tasknumber;
            GetMaterialsBill_Hand();
            if (bill_Hand == null || bill_Hand.Tables[0] == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台没有人工领料单!');</script>");
                return;
            }
            for (int i = 0; i < bill_Hand.Tables[0].Rows.Count; i++)
            {
                tasknumber = bill_Hand.Tables[0].Rows[i]["Tasknumber"].ToString();
                if (mhb.FinishManPowerCallTask(tasknumber))
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作成功!');</script>");
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作失败,请重试!');</script>");
                }
                //for (int j = i + 1; j < bill_Hand.Tables[0].Rows.Count; j++)
                //{
                //    if (bill_Hand.Tables[0].Rows[j]["tasknumber"].ToString().Equals(tasknumber))
                //    {
                //        j++;
                //    }
                //    else
                //    {
                //        i = j;
                //        break;
                //    }
                //}
                //list.Add(tasknumber);
            }
            GetMaterialsBill_Hand();
            BindingListView();
        }

        protected void btn_Ensure_Click(object sender, EventArgs e)
        {
            MaterialsBill_HandWork_BLL mhb = new MaterialsBill_HandWork_BLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            foreach (GridViewRow grv in lv_Materials_Hand.Rows)
            {
                CheckBox cb = (CheckBox)grv.FindControl("chkSelect");
                if (cb.Checked == true)
                {
                    string tasknumber = ((TextBox)grv.FindControl("txtTaskNumber")).Text.Trim();
                    if (mhb.FinishManPowerCallTask(tasknumber))
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作成功!');</script>");
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('接收物料操作失败,请重试!');</script>");
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请选择需要确认的人工领料单!');</script>");
                }
            }
            GetMaterialsBill_Hand();
            BindingListView();
        }

        protected void lv_Materials_Hand_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Materials_Hand.PageIndex = e.NewPageIndex;
            GetMaterialsBill_Hand();
        }

        protected void btn_Fresh_Click(object sender, EventArgs e)
        {
            GetMaterialsBill_Hand();
            BindingListView();
        }

        protected void btn_Cancle_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }
    }
}