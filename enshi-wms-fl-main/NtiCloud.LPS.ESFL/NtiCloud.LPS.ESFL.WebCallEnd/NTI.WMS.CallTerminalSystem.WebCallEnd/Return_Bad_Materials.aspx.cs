using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using System.Collections;
using WMSInterface;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Return_Bad_Materials : System.Web.UI.Page
    {
        private static string target_Area_Des;
        private static string targe_Area_Number;
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private static DataSet containerDetail;
        private static DataSet target_Dock;
        string UserName, UserPassword, WareHouseID;
        string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, RunsTypeCode, ItemName, DockNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSAOUR");
            UserName = Session["UserName"].ToString();
            //UserPassword = Session["PassWord"].ToString();
            WareHouseID = Session["WarehouseID"].ToString();
            Equipment = Session["Equipment"].ToString();
            WorkShop = Session["WorkShop"].ToString(); //车间 
            Run = Session["Runs"].ToString(); //班组 
            MachineName = Session["MachineName"].ToString();
            RunsGroup = Session["RunsGroup"].ToString();
            DockNumber = Session["DockNumber"].ToString();
            Response.Expires = -1;

            if (Session["MachineNumber"] == null)
            {
                string Url = "Control_Panel.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无机台信息，请先到机台设置中设置机台信息!');window.location='" + Url + "';</script>");
                return;
            }
            else
            {
                MachineNumber = Session["MachineNumber"].ToString();
            }
            if (Session["productitemname"] != null)
            {
                LoginUser.ItemName = Session["productitemname"].ToString();
                ItemName = Session["productitemname"].ToString();
            }
            if (Session["MachineName"] != null) MachineName = Session["MachineName"].ToString();
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
                    break;
                case "4":
                    RunsGroup = "丁班";
                    break;
            }
            LoginUser.RunsCode = Run;
            RunsTypeCode = RunsGroup + "-" + Run;
            LoginUser.RunsTypeCode = RunsGroup + "-" + Run;
            if (!Page.IsPostBack)
            {
                InitDataSet();
                BindingListview();
                InitTextBlock();
                BindingCombobox();
            }
        }

        private void InitDataSet()
        {
            containerDetail = new DataSet();
            containerDetail.Clear();
            containerDetail = null; //rpb.GetContainerDetailByDockNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, DockNumber);
            if (containerDetail != null)
            {
                containerDetail.Tables[0].Columns.Add("ENABLEDODD");

                foreach (DataRow row in containerDetail.Tables[0].Rows)
                {
                    row["FIELD4"] = row["QTY"];
                    row["ENABLEDODD"] = Properties.Settings.Default.EnabledODD;
                    row["QTY"] = 0;
                }
            }
            target_Dock = new DataSet();
            target_Dock.Clear();
            target_Dock = rpb.GetReturnPalletDestination();
        }

        private void BindingListview()
        {
            if (containerDetail == null || containerDetail.Tables.Count == 0 || containerDetail.Tables[0].Rows.Count == 0)
            {
                lv_Return_Bad.DataSource = null;
                lv_Return_Bad.DataBind();
                //ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('没有退坏料的物料!');window.location='Call_Terminal.aspx';</script>");
                return;
            }
            lv_Return_Bad.DataSource = containerDetail.Tables[0].DefaultView;
            lv_Return_Bad.DataBind();
        }

        public void InitTextBlock()
        {
            bool flag = false;
            if (containerDetail != null && containerDetail.Tables[0].Rows.Count > 0)
            {
                flag = true;
            }
            txtblk_Sock_Code.Text = flag ? containerDetail.Tables[0].Rows[0]["CURPALLETNUMBER"].ToString() : "未设置";
            txtblk_Platform_Num.Text = DockNumber;
            txtblk_Brand.Text = ItemName;
            txtblk_Section.Text = flag ? containerDetail.Tables[0].Rows[0]["MODELCODE"].ToString() : "未设置";
        }

        private void BindingCombobox()
        {
            ListItem listItem = new ListItem("平衡高架库", Properties.Settings.Default.ReturnPallet_TargetAddress);
            target_Area.Items.Add(listItem);
            //try
            //{
            //    target_Area.DataSource = target_Dock.Tables[0].DefaultView;
            //    target_Area.DataTextField = "Description";
            //    target_Area.DataValueField = "EquipmentNumber";
            //    target_Area.DataBind();
            //}
            //catch
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没能获取到目的区域!');</script>");
            //}
        }

        /// <summary>
        /// 增加退坏料原因
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btn_Add_Click(object sender, EventArgs e)
        //{
        //    //Return_Materials_Result rmr = new Return_Materials_Result();
        //    //rmr.Owner = this;
        //    //rmr.ShowDialog();
        //    //Button button = sender as Button;
        //    string s = button.Tag.ToString();
        //    if (containerDetail != null)
        //    {
        //        foreach (DataRow row in containerDetail.Tables[0].Rows)
        //        {
        //            if (row["CONTAINERDETAILID"].ToString() == s)
        //            {
        //                row["FIELD3"] = Return_Materials_Result.reason;
        //            }
        //        }
        //    }
        //}

        protected void target_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            target_Area_Des = target_Area.SelectedItem.Text.ToString();
            targe_Area_Number = target_Area.SelectedItem.Value.ToString();

            if (containerDetail == null || containerDetail.Tables[0].Rows.Count < 1)
            {
                return;
            }

            foreach (DataRow row in containerDetail.Tables[0].Rows)
            {
                row["FIELD2"] = target_Area_Des;
            }
        }

        protected void lv_Return_Bad_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Return_Bad.PageIndex = e.NewPageIndex;
            foreach (GridViewRow row in lv_Return_Bad.Rows)
            {
                TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
                string ContainerDetailID = txtContainerDetailID.Text.Trim();
                TextBox textbox = (TextBox)row.FindControl("QTY");
                if (!string.IsNullOrEmpty(textbox.Text))
                {
                    if (CommonFuctions.GetNum(textbox.Text.Trim()))
                    {
                        CompareItemQtyToBom(textbox, containerDetail.Tables[0], ContainerDetailID);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确!');</script>");
                    }
                }
                else
                {
                    textbox.Text = "0";
                }
            }
            BindingListview();
        }

        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            if (lv_Return_Bad.Rows != null)
            {
                foreach (GridViewRow row in lv_Return_Bad.Rows)
                {
                    TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
                    string ContainerDetailID = txtContainerDetailID.Text.Trim();
                    TextBox textbox = (TextBox)row.FindControl("QTY");
                    TextBox tbodd1 = (TextBox)row.FindControl("ODD1");
                    TextBox tbodd2 = (TextBox)row.FindControl("ODD2");
                    TextBox tbodd3 = (TextBox)row.FindControl("ODD3");
                    HashSet<TextBox> tbodds = new HashSet<TextBox>();
                    tbodds.Add(tbodd1);
                    tbodds.Add(tbodd2);
                    tbodds.Add(tbodd3);
                    if (!string.IsNullOrEmpty(textbox.Text))
                    {
                        //if (CommonFuctions.CheckQty(textbox.Text.Trim()))//(CommonFuctions.GetDecemal(textbox.Text))
                        //{
                        CompareItemQtyToBom(textbox, containerDetail.Tables[0], ContainerDetailID);
                        //}
                        //else
                        //{
                        //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确!');</script>");
                        //}
                    }
                    else
                    {
                        textbox.Text = "0";
                    }
                    foreach (TextBox tb in tbodds)
                    {
                        if (!string.IsNullOrEmpty(tb.Text))
                        {
                            if (CommonFuctions.CheckQty(tb.Text.Trim()))//(CommonFuctions.GetDecemal(tb.Text))
                            {
                                CompareItemQtyToBom(tb, containerDetail.Tables[0], ContainerDetailID);
                            }
                            else
                            {
                                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确!');</script>");
                            }
                        }
                        else
                        {
                            tb.Text = "0";
                        }
                    }
                }
            }
            targe_Area_Number = target_Area.SelectedItem.Value.ToString();
            DataSet change_Dataset = new DataSet();
            ContainerEntity ce = new ContainerEntity();
            bool evaluate = false;
            change_Dataset = (DataSet)containerDetail;
            bool havingResult = false;
            decimal returnQty = 0;

            //判断是否已经输入退坏料原因
            foreach (DataRow row in change_Dataset.Tables[0].Rows)
            {
                if (!string.IsNullOrEmpty(row["FIELD3"].ToString()))
                {
                    havingResult = true;
                    break;
                }
            }
            //if (!havingResult)
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('对不起,您至少选择一项退坏料原因!');</script>");
            //    return;
            //}

            foreach (DataRow row in change_Dataset.Tables[0].Rows)
            {
                decimal qty = decimal.Parse(row["QTY"].ToString()) * decimal.Parse(row["RATE"].ToString());
                decimal odd1 = decimal.Parse(row["ODD1"].ToString());
                decimal odd2 = decimal.Parse(row["ODD2"].ToString());
                decimal odd3 = decimal.Parse(row["ODD3"].ToString());



                if (!rpb.CompareItemQtyToBom(row["ITEMID"].ToString(), qty, decimal.Parse(row["ODD1"].ToString()), decimal.Parse(row["ODD2"].ToString()), decimal.Parse(row["ODD3"].ToString()), row["BOMID"].ToString()))
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量不能超出BOM的数量,请重新输入!');</script>");
                    ce.ItemList.Clear();
                    return;
                }

                returnQty = returnQty + qty;
                if (evaluate == false)
                {
                    ce.Containerbarcode = row["CONTAINERBARCODE1"].ToString();
                    ce.WAREHOUSEID = row["WAREHOUSEID"].ToString();
                    ce.CONTAINERID = row["CONTAINERID"].ToString();
                    ce.Bomid = row["BOMID"].ToString();
                    ce.Modelcode = row["MODELCODE"].ToString();
                }
                ce.AddBadContainerDetail_ItemData(row["ITEMID"].ToString(), row["ITEMNAME"].ToString(),
                    qty, null, row["LOTNUMBER"].ToString(), odd1, odd2, odd3, true, row["FIELD3"].ToString(), row["ItemSupplier"].ToString());
            }
            if (returnQty <= 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料量不能为0,请重新输入!');</script>");
                ce.ItemList.Clear();
                return;
            }

            if (rpb.ClientReturnBadPallet(MachineNumber, DockNumber, ce.Bomid, ce,
                OperType.CLIENT_CALLTERMINAL_RETURNBADPALLET,
                LoginUser.HasLGV.ToString(), targe_Area_Number, RunsTypeCode) > 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退坏料，操作执行成功!');window.location='Call_Terminal.aspx';</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退坏料，操作执行失败!');</script>");
                ce.ItemList.Clear();
            }
        }

        public void TextBox_Preview(TextBox textbox, DataTable dt, string ContainerDetailID)
        {
            decimal text_initCount;
            text_initCount = decimal.Parse(textbox.Text);

            if (textbox.Text == "")
            {
                textbox.Text = text_initCount.ToString();
            }
            //Check_input(textbox, text_initCount);
            //取消退余料数量的限制
            CompareItemQtyToBom(textbox, dt, ContainerDetailID);
        }

        private void CompareItemQtyToBom(TextBox tb, DataTable dt, string ContainerDetailID)
        {
            if (dt != null)
            {
                //string s = tb.Tag.ToString();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["CONTAINERDETAILID"].ToString() == ContainerDetailID)
                    {
                        string value = row[tb.ID].ToString();

                        if (tb.Text.Length > 10)
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('您输入的数值位数太长,请重新输入!');</script>");
                            tb.Text = value;
                            return;
                        }

                        row[tb.ID] = tb.Text;     //写入修改数据到DataSet

                        if (row["MODELCODE"].ToString().Equals(Properties.Settings.Default.ContainerTypeCode_Handle))
                        {
                            return;
                        }
                        decimal qty = decimal.Parse(row["QTY"].ToString()) * decimal.Parse(row["RATE"].ToString());
                        if (!rpb.CompareItemQtyToBom(row["ITEMID"].ToString(), qty, decimal.Parse(row["ODD1"].ToString()), decimal.Parse(row["ODD2"].ToString()), decimal.Parse(row["ODD3"].ToString()), row["BOMID"].ToString()))
                        {
                            //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT, NTIMessage.RETURN_COUNT_OVER_ERROR, Hint.ERROR);
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量不能超出BOM的数量,请重新输入!');</script>");
                        }
                    }
                }
            }
        }

        protected void lv_Return_Bad_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "reason")
            {
                string s = e.CommandArgument.ToString();
                if (containerDetail != null)
                {
                    foreach (DataRow row in containerDetail.Tables[0].Rows)
                    {
                        if (row["CONTAINERDETAILID"].ToString() == s)
                        {
                            //string ss = this.@return.Value.ToString();
                            if (Session["returnValue"] != null)
                            {
                                row["FIELD3"] = Session["returnValue"].ToString();
                                Session["returnValue"] = null;
                            }
                            //else
                            //{

                            //}
                        }
                    }
                }
            }
            BindingListview();
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Call_Terminal.aspx");
        }
    }
}