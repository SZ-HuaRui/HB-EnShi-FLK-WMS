using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Return_Remnant_Manage : System.Web.UI.Page
    {
        string UserName, UserPassword, StartTime, EndTime;
        string WareHouseID, ItemName, ItemCode;
        string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, hasLGV, EquipmentNumber, RunsTypeCode;
        private System.Data.DataSet machineNumberDS;
        public DataSet ReturnTaskDS;
        private DataTable UsedDataTable
        {
            set { ViewState["UsedDataTable"] = value; }
            get { return ViewState["UsedDataTable"] as DataTable; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSAOUR");
            EquipmentNumber = Session["Equipment"].ToString();
            UserName = Session["UserName"].ToString();
            UserPassword = Session["PassWord"].ToString();
            WareHouseID = Session["WarehouseID"].ToString();
            WorkShop = Session["WorkShop"].ToString(); //车间 
            Run = Session["Runs"].ToString(); //班组 

            if (Session["MachineNumber"] == null)
            {
                Session["IsToCall"] = "Return_Remnant_Manage.aspx";
                string Url = "Set_Number.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无机台信息，请先到机台设置中设置机台信息!');window.location='" + Url + "';</script>");
                return;
            }
            else
            {
                MachineNumber = Session["MachineNumber"].ToString();
                LoginUser.MachineNumber = MachineNumber;
            }
            RunsGroup = Session["RunsGroup"].ToString();

            Response.Expires = -1;
            if (Session["MachineName"] != null)
            {
                MachineName = Session["MachineName"].ToString();
            }
            if (Session["productitemname"] != null)
            {
                LoginUser.ItemName = Session["productitemname"].ToString();
                ItemName = Session["productitemname"].ToString();
            }
            if (!Page.IsPostBack)
            {
                SetMachineNumber(WorkShop);
                if (machineNumberDS == null || machineNumberDS.Tables.Count <= 0 ||
                machineNumberDS.Tables[0].Rows.Count <= 0 ||
                (!machineNumberDS.Tables[0].Columns.Contains("EQUIPMENTNUMBER")) ||
                (!machineNumberDS.Tables[0].Columns.Contains("DESCRIPTION")))
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没有获取到机台数据!');</script>");
                    return;
                }
                else
                {
                    //根据选择的车间类型去过滤   pengenqi 2012/7/11
                    DataView dv = machineNumberDS.Tables[0].DefaultView;
                    machineNumberDS.Tables[0].DefaultView.Sort = "EQUIPMENTNUMBER asc";
                    dv.RowFilter = "code='" + WorkShop + "'";
                    DataTable dtByWorkShop = dv.ToTable();
                    cb_MachineNumber.DataSource = dtByWorkShop;//machineNumberDS;
                    cb_MachineNumber.DataValueField = "EQUIPMENTNUMBER";
                    cb_MachineNumber.DataTextField = "DESCRIPTION";
                    cb_MachineNumber.DataBind();
                    cb_MachineNumber.Items.Insert(0, "-请选择机台编号-");
                    if (Request.Cookies["MachineDockSetting"] != null)
                    {
                        //MachineNumber = Convert.ToString(Request.Cookies["MachineDockSetting"].Values["MachineNumber"]);
                        if (cb_MachineNumber.Items.FindByText(MachineNumber) != null)
                        {
                            cb_MachineNumber.Items.FindByText(MachineNumber).Selected = true;
                            cb_MachineNumber.SelectedItem.Text = MachineNumber;
                        }
                    }
                    //txtBlock_MachineName.Text =cb_MachineNumber.SelectedItem.Text;
                }
                StartTime = DateTime.Now.ToString("yyyy-MM-dd ") + "00:00:00";
                EndTime = DateTime.Now.ToString("yyyy-MM-dd ") + "23:59:59";
                InitData(MachineNumber, WareHouseID, StartTime, EndTime);
                LoadRunsType();
            }
        }

        public void SetMachineNumber(string workShop)
        {
            CommonBLL cb = new CommonBLL(LoginUser.EquipmentNumber, UserName, UserPassword, WareHouseID);
            if (!string.IsNullOrEmpty(workShop))
            {
                //machineNumberDS = cb.GetMachineDataByWorkShopType(workShop);
            }
        }

        public void InitData(string MachineNumber, string WareHouseID, string StartTime,string EndTime)
        {
            ReturnPalletBLL rb = new ReturnPalletBLL(EquipmentNumber, UserName, UserPassword, WareHouseID);
            ReturnTaskDS = new DataSet();
            ReturnTaskDS = rb.CheckReturnRemnantTask(MachineNumber, WareHouseID, StartTime, EndTime);
            if (ReturnTaskDS == null || ReturnTaskDS.Tables[0].Rows == null || ReturnTaskDS.Tables[0].Rows.Count == 0)
            {
                UsedDataTable = null;
                lv_ReturnMaterial.DataSource = null;
                lv_ReturnMaterial.DataBind();
            }
            else
            {
                UsedDataTable = ReturnTaskDS.Tables[0];
                lv_ReturnMaterial.DataSource = ReturnTaskDS.Tables[0];
                lv_ReturnMaterial.DataBind();
            }
        }

        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            try
            {
                if (MakeReturnQtyChange())
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('修改退料数量成功!');</script>");
                    UsedDataTable.Clear();
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('修改退料数量失败!');</script>");
                    //UsedDataTable.Clear();
                    return;
                }
            }
            catch { }
        }

        private bool MakeReturnQtyChange()
        {
            ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, UserName, LoginUser.Password, LoginUser.WarehouseID);
            bool flag = true;
            string dockNumber_Succeed = string.Empty;
            string dockNumber_Error = string.Empty;
            int totalint = 0;
            bool bFind = false;
            try
            {
                #region
                if (lv_ReturnMaterial.Rows.Count != 0)
                {
                    totalint = lv_ReturnMaterial.Rows.Count;
                    foreach (GridViewRow row in lv_ReturnMaterial.Rows)
                    {
                        if (bFind)
                            break;
                        if (flag)
                        {
                            TextBox TxtContainerdetailid = (TextBox)row.FindControl("Containerdetailid");
                            string Containerdetailid = TxtContainerdetailid.Text.ToString();
                            TextBox txtQty = (TextBox)row.FindControl("txtQTY");
                            TextBox txtField1=(TextBox)row.FindControl("Field1");

                            if (txtQty.Text == "")
                            {
                                txtQty.Text = "0";
                            }
                            Decimal returnqty = 0;
                            if (CommonFuctions.CheckDecemal(txtQty.Text.Trim()))
                            {
                                returnqty = Convert.ToDecimal(txtQty.Text.Trim());

                                for (int i = 0; i < UsedDataTable.Rows.Count; i++)
                                {
                                    if (UsedDataTable.Rows[i]["Containerdetailid"].Equals(Containerdetailid))
                                    {
                                        UsedDataTable.Rows[i]["qty"] = returnqty;
                                        UsedDataTable.Rows[i]["field1"] = txtField1.Text.Trim().ToString();
                                    }
                                }
                            }
                            else
                            {
                                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确!');</script>");
                                bFind = true;
                                break;
                            }
                        }
                    }
                }
                if (bFind)
                {
                    return false;
                }
                #endregion

                DataTable change_DataTable = new DataTable();
                change_DataTable = UsedDataTable;

                if (rpb.ChangeReturnQty(WareHouseID,change_DataTable))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }

                return flag;
            }
            catch
            {
                return false;
            }
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (cb_MachineNumber.SelectedIndex != 0)
            {
                MachineNumber = cb_MachineNumber.SelectedValue.ToString();
            }
            if (EntTime.Text != "" || EntTime.Text != null)
            {
                EndTime=StartTime = EntTime.Text.Trim();
                if (cbx_Run.SelectedValue == "Runs_2")
                {
                    StartTime = StartTime + " " + Properties.Settings.Default.zao.Trim().ToString();
                    EndTime = EndTime + " " + Properties.Settings.Default.zhong.Trim().ToString();
                }
                else if (cbx_Run.SelectedValue == "Runs_3")
                {
                    StartTime = StartTime + " " + Properties.Settings.Default.zhong.Trim().ToString();
                    EndTime = EndTime + " " + Properties.Settings.Default.wan.Trim().ToString();
                }
                else if (cbx_Run.SelectedValue == "Runs_1")
                {
                    StartTime=Convert.ToDateTime(StartTime).AddDays(-1).ToString("yyyy-MM-dd ") + Properties.Settings.Default.wan.Trim().ToString();
                    EndTime = EndTime + " " + Properties.Settings.Default.zao.Trim().ToString();
                }
                else if (cbx_Run.SelectedIndex == 0)
                {
                    StartTime = StartTime + " 00:00:00";
                    EndTime = EndTime + " 23:59:59";
                }
            }

            InitData(MachineNumber, WareHouseID, StartTime, EndTime);
        }

        private void LoadRunsType()
        {
            cbx_Run.Items.Clear();
            CommonBLL bll = new CommonBLL(LoginUser.EquipmentNumber, UserName, LoginUser.Password, LoginUser.WarehouseID);

            DataSet ds_RunsType = null;// bll.GetRunsType(Properties.Settings.Default.TypeCode_RunsFlag);
            if (ds_RunsType == null || ds_RunsType.Tables.Count == 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作执行失败，无法访问服务器，或服务器未能返回有效信息！');</script>");
                return;
            }
            cbx_Run.DataSource = ds_RunsType.Tables[0].DefaultView;
            cbx_Run.DataValueField = "code";
            cbx_Run.DataTextField = "description";
            cbx_Run.DataBind();
            cbx_Run.Items.Insert(0, "-请选择班次-");
        }

        protected void lv_ReturnMaterial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_ReturnMaterial.PageIndex = e.NewPageIndex;
            InitData(MachineNumber, WareHouseID, StartTime, EndTime);
        }
    }
}