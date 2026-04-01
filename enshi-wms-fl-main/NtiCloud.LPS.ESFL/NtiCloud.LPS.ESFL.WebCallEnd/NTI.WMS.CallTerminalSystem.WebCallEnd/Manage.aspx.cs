using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Manage : System.Web.UI.Page
    {
        public static string user;
        protected void Page_Load(object sender, EventArgs e)
        {
            user = Session["UserName"].ToString();
            if (!IsPostBack)
            {
                txtblock_UserName.Text = user;
            }
        }

        protected void btn_Getting_Click(object sender, EventArgs e)
        {
            //Response.Redirect("Handle_Daterials_Confirm.aspx");
        }

        protected void btn_Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        protected void btn_Return_Bill_Click(object sender, EventArgs e)
        {
            Response.Redirect("Return_Materials_Confirm.aspx");
        }
    }
}