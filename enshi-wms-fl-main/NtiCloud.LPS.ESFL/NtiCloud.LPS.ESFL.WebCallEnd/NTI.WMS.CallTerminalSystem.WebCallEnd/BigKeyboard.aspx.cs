using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class BigKeyboard : System.Web.UI.Page
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

        protected void ButtonA_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonA.Text;
        }

        protected void ButtonBack_Click(object sender, EventArgs e)
        {
            string str = PostVal.Text.ToString();
            str = str.Remove(str.Length - 1);
            PostVal.Text = str;
        }

        protected void ButtonQ_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonQ.Text;
        }

        protected void ButtonW_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonW.Text;
        }

        protected void ButtonE_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonW.Text;
        }

        protected void ButtonR_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonR.Text;
        }

        protected void ButtonT_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonT.Text;
        }

        protected void ButtonY_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonY.Text;
        }

        protected void ButtonU_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonU.Text;
        }

        protected void ButtonI_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonI.Text;
        }

        protected void ButtonO_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonO.Text;
        }

        protected void ButtonP_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonP.Text;
        }

        protected void ButtonXing_Click(object sender, EventArgs e)
        {
            PostVal.Text += "*";
        }

        protected void ButtonS_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonS.Text;
        }

        protected void ButtonD_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonD.Text;
        }

        protected void ButtonF_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonF.Text;
        }

        protected void ButtonG_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonG.Text;
        }

        protected void ButtonH_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonH.Text;
        }

        protected void ButtonJ_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonJ.Text;
        }

        protected void ButtonK_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonK.Text;
        }

        protected void ButtonL_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonL.Text;
        }

        protected void ButtonEnter_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "", "<script type='text/javascript'>window.returnValue=" + PostVal.Text.ToString() + ";window.close();</script>");
        }

        protected void ButtonTab_Click(object sender, EventArgs e)
        {

        }

        protected void kbtn___Click(object sender, EventArgs e)
        {
            PostVal.Text += "`";
        }

        protected void ButtonZ_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonZ.Text;
        }

        protected void ButtonX_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonX.Text;
        }

        protected void ButtonC_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonC.Text;
        }

        protected void ButtonV_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonV.Text;
        }

        protected void ButtonB_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonB.Text;
        }

        protected void ButtonN_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonN.Text;
        }

        protected void ButtonM_Click(object sender, EventArgs e)
        {
            PostVal.Text += ButtonM.Text;
        }

        protected void Buttondian_Click(object sender, EventArgs e)
        {
            PostVal.Text += ".";
        }

        protected void Button11_Click(object sender, EventArgs e)
        {
            PostVal.Text += "/";
        }

        protected void ButtonPost_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "", "<script type='text/javascript'>window.returnValue=" + PostVal.Text.ToString() + ";window.close();</script>");
        }

        protected void ButtoncapsLock_Click(object sender, EventArgs e)
        {
            ButtonA.Text = "A";
            ButtonB.Text = "B";
            ButtonC.Text = "C";
            ButtonD.Text = "D";
            ButtonE.Text = "E";
            ButtonF.Text = "F";
            ButtonG.Text = "G";
            ButtonH.Text = "H";
            ButtonI.Text = "I";
            ButtonJ.Text = "J";
            ButtonK.Text = "K";
            ButtonL.Text = "L";
            ButtonM.Text = "M";
            ButtonN.Text = "N";
            ButtonO.Text = "O";
            ButtonP.Text = "P";
            ButtonQ.Text = "Q";
            ButtonR.Text = "R";
            ButtonS.Text = "S";
            ButtonT.Text = "T";
            ButtonU.Text = "U";
            ButtonV.Text = "V";
            ButtonW.Text = "W";
            ButtonX.Text = "X";
            ButtonY.Text = "Y";
            ButtonZ.Text = "Z";
        }

        protected void ButtonShift_Click1(object sender, EventArgs e)
        {
            ButtonA.Text = "a";
            ButtonB.Text = "b";
            ButtonC.Text = "c";
            ButtonD.Text = "d";
            ButtonE.Text = "e";
            ButtonF.Text = "f";
            ButtonG.Text = "g";
            ButtonH.Text = "h";
            ButtonI.Text = "i";
            ButtonJ.Text = "j";
            ButtonK.Text = "k";
            ButtonL.Text = "l";
            ButtonM.Text = "m";
            ButtonN.Text = "n";
            ButtonO.Text = "o";
            ButtonP.Text = "p";
            ButtonQ.Text = "q";
            ButtonR.Text = "r";
            ButtonS.Text = "s";
            ButtonT.Text = "t";
            ButtonU.Text = "u";
            ButtonV.Text = "v";
            ButtonW.Text = "w";
            ButtonX.Text = "x";
            ButtonY.Text = "y";
            ButtonZ.Text = "z";
        }
    }
}