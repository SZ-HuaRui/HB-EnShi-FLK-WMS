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
    public partial class MachineProduct : System.Web.UI.Page
    {
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

        //加载
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionKey"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            if (!IsPostBack)
            {
                Label1.Text = (LoginUser.MachineNumber == null) ? "未设置" : LoginUser.MachineNumber;
                labdocknumber.Text = (LoginUser.MachineName == null) ? "未设置" : LoginUser.MachineName;
                Label2.Text = (LoginUser.ItemName == null) ? "未设置" : LoginUser.ItemName;
                labbc.Text = LoginUser.RunsTypeCode;

                //this.LoadMachineProduct();

                LoadProduct();
            }
        }

        private void LoadMachineProduct()
        {
            try
            {
                DataSet dsMachineProduct = rpb.LoadMachineProduct(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

                DataView dv = dsMachineProduct.Tables[0].DefaultView;
                dv.RowFilter = "EQUIPMENTNUMBER='" + LoginUser.MachineNumber + "'";
                if (string.IsNullOrEmpty(LoginUser.MachineName))
                {
                    gvChangeItem.DataSource = null;
                }
                else
                {
                    gvChangeItem.DataSource = dv;
                }
                gvChangeItem.DataBind();

            }
            catch { }
        }

        private void LoadProduct()
        {
            CommonBLL bll = new CommonBLL();
            DataTable dtItems = bll.GetItemCP(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID);

            if (dtItems == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无法访问服务器！');</script>");
            }
            else if (dtItems.Rows.Count > 0)
            {
                foreach (DataRow row in dtItems.Rows)
                {
                    if (row["ITEM_ID"].ToString() == LoginUser.ItemID)
                    {
                        dtItems.Rows.Remove(row);
                        break;
                    }
                }
            }

            DropDownList1.DataValueField = "ITEM_ID";
            DropDownList1.DataTextField = "ITEM_NAME";
            DropDownList1.DataSource = dtItems;
            DropDownList1.DataBind();
            DropDownList1.Items.Insert(0, "-请选择-");
        }

        protected void gvChangeItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void ckbSelect_CheckedChanged(object sender, EventArgs e)
        {
        }


        //确定
        protected void btn_Ensure_Click(object sender, EventArgs e)
        {
            CommonBLL bll = new CommonBLL();

            try
            {
                string itemId = DropDownList1.SelectedValue.ToString();
                if (LoginUser.ItemID == itemId)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('选择的牌号和当前一致！');</script>");
                    return;
                }

                string msg = bll.UpdateMachineProductPlan(LoginUser.Url, LoginUser.SessionKey, LoginUser.MachineNumber, itemId);

                if (string.IsNullOrEmpty(msg))
                {
                    LoginUser.ItemID = DropDownList1.SelectedValue.ToString();
                    LoginUser.ItemName = this.Label2.Text = this.DropDownList1.SelectedItem.Text.Trim();

                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('修改机台牌号成功。');location='Control_Panel.aspx';</script>");
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('牌号修改失败！" + msg + "');</script>");
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('" + ex.Message + "');</script>");
            }

        }

        protected void btn_Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        private DataSet SetTowProductName()
        {
            try
            {
                DataSet twoProductNames = rpb.GetTwoProductName(LoginUser.WarehouseID, LoginUser.MachineNumber);
                return twoProductNames;
            }
            catch { return null; }
        }
    }
}