using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Change_Class_Hint : System.Web.UI.Page
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
                tb_User.Text = LoginUser.UserName;
                LblMachine.Text = LoginUser.MachineNumber;
            }
        }

        protected void tb_oper_Click(object sender, EventArgs e)
        {
            Response.Redirect("Change_Class_Accept.aspx");
        }

        protected void tb_Close_Click(object sender, EventArgs e)
        {
            Session["Change_Class"] = "haved";
            Response.Redirect("Control_Panel.aspx");

        }

        protected void btn_Setting_Click(object sender, EventArgs e)
        {
            Response.Redirect("Set_Number_new.aspx");
        }
    }
}