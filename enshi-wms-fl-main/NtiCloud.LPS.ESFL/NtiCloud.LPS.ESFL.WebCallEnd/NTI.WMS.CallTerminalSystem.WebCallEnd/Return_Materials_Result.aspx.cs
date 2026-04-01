using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Return_Materials_Result : System.Web.UI.Page
    {
        public static string reason = string.Empty;
        private static int true_Count = 0;
        bool[] return_Bool = new bool[5] { false, false, false, false, false };
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            if (chb_Colour.Checked == true)
            {
                return_Bool[0] = true;
                true_Count++;
            }
            if (chb_Package.Checked == true)
            {
                return_Bool[1] = true;
                true_Count++;
            }
            if (chb_Quality.Checked == true)
            {
                return_Bool[2] = true;
                true_Count++;
            }
            if (chb_Erroe_Materials.Checked == true)
            {
                return_Bool[3] = true;
                true_Count++;
            }
            if (chb_Other.Checked == true)
            {
                return_Bool[4] = true;
                true_Count++;
            }
            Show(return_Bool);
            Response.Write("<script language=javascript>window.returnValue='" + reason.ToString() + "';window.close();</script>");
            Session["returnValue"] = reason.ToString();
        }
        public void Show(bool[] args)
        {
            int count = 1;
            int temp_true = 0;
            StringBuilder Show_String = new StringBuilder("");
            foreach (bool item in args)
            {
                if (item)
                {
                    temp_true++;
                    switch (count)
                    {
                        case 1: Show_String.Append("颜色错误");
                            break;
                        case 2: Show_String.Append("包装不对");
                            break;
                        case 3: Show_String.Append("质量问题");
                            break;
                        case 4: Show_String.Append("错料");
                            break;
                        case 5: Show_String.Append("其它");
                            break;
                        default: break;
                    }
                    if (temp_true == true_Count)
                    {
                        Show_String.Append(".");
                    }
                    else
                    {
                        Show_String.Append(";");
                    }
                }

                count++;
            }
            true_Count = 0;
            reason = Show_String.ToString();
        }
    }
}