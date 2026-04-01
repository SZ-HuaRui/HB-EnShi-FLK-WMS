using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using WMSInterface.Entity50;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class PalletType : System.Web.UI.Page
    {
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        string Equipment, UserName, Password, WareHouseID, Run, WorkShop, MachineName;
        //机台编号
        private string MachineNumber;
        string RunsGroup = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["Equipment"] == null || Session["WorkShop"] == null || Session["UserName"] == null || Session["WarehouseID"] == null || Session["Runs"] == null)
            //{
            //    Response.Redirect("pass.aspx");
            //}
            //else
            //{
                Equipment = Session["Equipment"].ToString();
                UserName = Session["UserName"].ToString();
                //Password = Session["PassWord"].ToString();
                WareHouseID = Session["WarehouseID"].ToString();
                Run = Session["Runs"].ToString();             //班组
                WorkShop = Session["WorkShop"].ToString();
                RunsGroup = Session["RunsGroup"].ToString(); //班时
                //MachineName = LoginUser.MachineName;

                if (Session["MachineName"] != null)
                    MachineName = Session["MachineName"].ToString();

                if (Session["MachineNumber"] != null)
                {
                    MachineNumber = Session["MachineNumber"].ToString();
                }
           // }

            if (!IsPostBack)
            {
                this.LoadMachinePalletType();

                Label1.Text = (MachineNumber == null) ? "未设置" : MachineNumber;//机台编号
                labdocknumber.Text = (MachineName == null) ? "未设置" : MachineName;//机台名称

                Label2.Text = (LoginUser.ItemName == null) ? "未设置" : LoginUser.ItemName;//生产牌号
                string ClassGroup = XmlReader.GetClassGroup(); //班次

                if (Run.Equals("1"))
                {
                    Run = "早班";
                }
                if (Run.Equals("2"))
                {
                    Run = "中班";
                }
                if (Run.Equals("3"))
                {
                    Run = "夜班";
                }
                labbc.Text = Run;

                this.LoadPalletType();
            }

            this.SetTowProductName();
        }

        //加载机台对应的托盘类型
        private void LoadMachinePalletType()
        {
            try
            {
                Dictionary<string, string> Param = new Dictionary<string, string>();

                LoginInfo logininfo = new LoginInfo();
                logininfo.WarehouseID = WareHouseID;
                logininfo.UserID = UserName;

                DataTable dtMachinePalletType = rpb.LoadMachinePalletType(logininfo, Param);

                DataView dv = dtMachinePalletType.DefaultView;
                dv.RowFilter = "machinenumber='" + MachineNumber + "'";
                gvChangeItem.DataSource = dv;
                gvChangeItem.DataBind();

            }
            catch { }
        }

        //根据机台类型加载托盘类型
        private void LoadPalletType()
        {
            try
            {
                Dictionary<string, string> Param = new Dictionary<string, string>();

                LoginInfo logininfo = new LoginInfo();
                logininfo.WarehouseID = WareHouseID;
                logininfo.UserID = UserName;

                DataTable dtProduct = rpb.GetModelN(logininfo,Param);

                dtProduct.DefaultView.Sort = "code desc";
                DropDownList1.DataValueField = "modelid";
                DropDownList1.DataTextField = "description";
                DropDownList1.DataSource = dtProduct;
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, "-请选择-");
            }
            catch { }
        }

        protected void btn_Ensure_Click(object sender, EventArgs e)
        {
            try
            {
                int haveContainerBarcode = 0;
                string haveBarDock = string.Empty;
                int checkedCount = 0;

                if (gvChangeItem.Rows != null)
                {
                    foreach (GridViewRow row in gvChangeItem.Rows)
                    {
                        if (((CheckBox)row.FindControl("ckbSelect")).Checked == true)
                        {
                            checkedCount++;

                            TextBox txtDockStatus = (TextBox)row.FindControl("txtCURCONTAINERID");
                            string bindContainerID = txtDockStatus.Text.Trim();

                            Label txtDock = (Label)row.FindControl("lblCurPalletNumber1");
                            string docknumber = txtDock.Text.Trim();

                            if (!string.IsNullOrEmpty(bindContainerID))
                            {
                                haveContainerBarcode++;
                                haveBarDock +="、"+ docknumber;
                            }
                        }
                    }
                }
                if (checkedCount == 0)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请选中要设置托盘类型的站台');</script>");
                    return;
                }

                if (haveContainerBarcode > 0)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('您选中的站台"+haveBarDock+"存在托盘，请先退托盘再设置托盘类型!');</script>");
                    return;
                }

                int selectCount = 0;
                if (gvChangeItem.Rows != null)
                {
                    foreach (GridViewRow row in gvChangeItem.Rows)
                    {
                        if (((CheckBox)row.FindControl("ckbSelect")).Checked == true)
                        {
                            WMSInterface.Entity50.MachineDockN machinedock = new MachineDockN();
                            //lblCurPalletNumber1
                            Label txtDock = (Label)row.FindControl("lblCurPalletNumber1");
                            machinedock.DockName = txtDock.Text.Trim();
                            Label txtDockNumber = (Label)row.FindControl("lblCurPalletNumber");
                            machinedock.DockNumber = txtDockNumber.Text.Trim();
                            TextBox txtDockStatus = (TextBox)row.FindControl("dockstatus");
                            machinedock.DockStatus = txtDockStatus.Text.Trim();
                            TextBox txtItemSource = (TextBox)row.FindControl("itemsource");
                            machinedock.ItemSource = Convert.ToInt32(txtItemSource.Text.Trim());
                            TextBox txtMachineDockId = (TextBox)row.FindControl("txtContainerDetailID");
                            machinedock.MachineDockID = txtMachineDockId.Text.Trim();
                            TextBox txtMachineNumber = (TextBox)row.FindControl("machinenumber");
                            machinedock.MachineNumber = txtMachineNumber.Text.Trim();
                            TextBox txtMachineType = (TextBox)row.FindControl("machinetype");
                            machinedock.MachineType = txtMachineType.Text.Trim();
                            Label txtPalletType = (Label)row.FindControl("TxtItemName");
                            machinedock.ModelCode = this.DropDownList1.SelectedItem.Text.Trim();//txtPalletType.Text.Trim();
                            TextBox txtPalletId = (TextBox)row.FindControl("modelid");
                            machinedock.ModelID = this.DropDownList1.SelectedValue.Trim();//txtPalletId.Text.Trim();
                            TextBox txtPreDockNumber = (TextBox)row.FindControl("predocknumber");
                            machinedock.PerDockNumber = txtPreDockNumber.Text.Trim();

                            machinedock.WarehouseID = WareHouseID;
                            machinedock.UserID = UserName;
                            machinedock.ZoneID = null;

                            LoginInfo logininfo = new LoginInfo();
                            logininfo.WarehouseID = WareHouseID;
                            logininfo.UserID = UserName;

                            WMSInterface.Entity50.Result result = rpb.UpdateMachineDockN(logininfo,machinedock);

                            if (result.Res > 0)
                            {
                                selectCount++;
                            }

                        }
                    }
                }
                if (selectCount > 0)
                {
                    this.LoadMachinePalletType();
                    // 
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('设置托盘类型成功!');</script>");
                    //Response.Redirect("Call_Terminal.aspx");
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('设置托盘类型失败!');</script>");
                }
            }
            catch { }
        }

        protected void btn_Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Call_Terminal.aspx");
        }

        protected void ckbSelect_CheckedChanged(object sender, EventArgs e)
        {
        }

        protected void gvChangeItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SetTowProductName()
        {
            int twoProducts = 0;
            string ddProductName = string.Empty;
            string onlyOne = string.Empty;

            DataSet twoProductNames = rpb.GetTwoProductName(WareHouseID, MachineNumber);
            if (twoProductNames != null && twoProductNames.Tables.Count > 0 && twoProductNames.Tables[0].Rows.Count > 1)
            {
                for (int z = 0; z < twoProductNames.Tables[0].Rows.Count; z++)
                {
                    if (twoProductNames.Tables[0].Rows[z]["name"] != null && twoProductNames.Tables[0].Rows[z]["name"].ToString().Equals(ddProductName))
                    {
                        continue;
                    }
                    else
                    {
                        ddProductName = twoProductNames.Tables[0].Rows[z]["name"].ToString();
                        twoProducts++;
                    }
                }

                if (twoProducts > 1)
                {
                    this.Label4.Visible = this.Label5.Visible = true;

                    string beforeProductName = string.Empty;
                    for (int x = 0; x < twoProductNames.Tables[0].Rows.Count; x++)
                    {
                        beforeProductName = twoProductNames.Tables[0].Rows[x]["name"].ToString();
                        if (!string.IsNullOrEmpty(beforeProductName))
                        {
                            if (x == 0)
                            {
                                this.Label2.Text = beforeProductName;
                            }
                            else
                            {
                                this.Label5.Text = beforeProductName;
                            }
                        }
                    }
                }
                else
                {
                    if (twoProductNames != null && twoProductNames.Tables.Count > 0 && twoProductNames.Tables[0].Rows.Count > 0)
                    {
                        onlyOne = twoProductNames.Tables[0].Rows[0]["name"].ToString();
                        if (!string.IsNullOrEmpty(onlyOne))
                        {
                            if (!onlyOne.Equals(LoginUser.ItemName))
                            {
                                this.Label4.Visible = this.Label5.Visible = true;
                                this.Label5.Text = onlyOne;
                            }
                        }
                    }
                }
            }
            else
            {
                if (twoProductNames != null && twoProductNames.Tables.Count > 0 && twoProductNames.Tables[0].Rows.Count > 0)
                {
                    onlyOne = twoProductNames.Tables[0].Rows[0]["name"].ToString();
                    if (!string.IsNullOrEmpty(onlyOne))
                    {
                        if (!onlyOne.Equals(LoginUser.ItemName))
                        {
                            this.Label4.Visible = this.Label5.Visible = true;
                            this.Label5.Text = onlyOne;
                        }
                    }
                }
            }
        }
    }
}