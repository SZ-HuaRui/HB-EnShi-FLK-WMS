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
    public partial class Dock_Materials : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionKey"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindGridView();
            }
        }

        private void BindGridView()
        {
            CommonBLL cb = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

            string dockcode = Request.QueryString["dockcode"].ToString();

            DataTable dtBomInfo = cb.GetBomByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineNumber, dockcode, LoginUser.SchemeId, LoginUser.MachineId, LoginUser.Token);

            if (dtBomInfo == null || dtBomInfo.Rows.Count <= 0)
            {
                lv_Dock_Materials.DataSource = null;
                lv_Dock_Materials.DataBind();
                return;
            }

            string bomId = string.Empty;

            for (int i = 0; i < dtBomInfo.Rows.Count; i++)
            {
                int cnt = Convert.ToInt32(dtBomInfo.Rows[i]["cnt"].ToString());
                string _bomId = dtBomInfo.Rows[i]["t_id"].ToString();
                if (cnt > 0)
                {
                    bomId = _bomId;
                    break;
                }
            }
            if (string.IsNullOrEmpty(bomId))
            {
                bomId = dtBomInfo.Rows[dtBomInfo.Rows.Count - 1]["t_id"].ToString();
            }

            //DataTable dt = cb.GetBomDetailByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineNumber, dockcode, LoginUser.SchemeId, LoginUser.MachineId, LoginUser.Token);

            DataTable dt = cb.GetBomDetailByMachineNumber1(LoginUser.Url, LoginUser.SessionKey, bomId, LoginUser.WarehouseID, LoginUser.MachineNumber, dockcode, LoginUser.SchemeId, LoginUser.MachineId, LoginUser.Token);

            if (dt != null && dt.Rows.Count > 0)
            {
                LblBomCode.Text = dt.Rows[0]["bom_code"].ToString();
                dt.Columns.Remove("bom_code");

                //dt.DefaultView.RowFilter = "qty > 0";
                lv_Dock_Materials.DataSource = dt.DefaultView;
                lv_Dock_Materials.DataBind();
            }
            else
            {
                lv_Dock_Materials.DataSource = null;
                lv_Dock_Materials.DataBind();
            }
        }

        protected void GridViewReturnBill_ToNext_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Dock_Materials.PageIndex = e.NewPageIndex;
            BindGridView();
        }

        //protected void Button1_Click(object sender, EventArgs e)
        //{
        //    Page.RegisterStartupScript("close ", " <script> window.opener=null;window.top.close() </script>");
        //}
    }
}