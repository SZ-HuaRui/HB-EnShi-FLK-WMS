using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class VirtualKeyboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            PostVal.Text += "1";
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            PostVal.Text += "2";
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            PostVal.Text += "3";
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            PostVal.Text += "4";
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            PostVal.Text += "5";
        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            PostVal.Text += "6";
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            PostVal.Text += "7";
        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            PostVal.Text += "8";
        }

        protected void Button9_Click(object sender, EventArgs e)
        {
            PostVal.Text += "9";
        }

        protected void Button0_Click(object sender, EventArgs e)
        {
            PostVal.Text += "0";
        }

        protected void Button10_Click(object sender, EventArgs e)
        {
            PostVal.Text = "";
        }

        protected void ButtonPost_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "", "<script type='text/javascript'>window.returnValue="+PostVal.Text.ToString()+";window.close();</script>");
        }
    }
}