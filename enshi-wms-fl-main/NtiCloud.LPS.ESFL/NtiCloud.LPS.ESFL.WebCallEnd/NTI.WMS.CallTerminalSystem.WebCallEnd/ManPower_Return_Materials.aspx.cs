using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using WMSInterface;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class ManPower_Return_Materials : System.Web.UI.Page
    {
        private static string dockNumber = string.Empty;
        private static DataSet containerDetail;
        private static DataSet targetDestination;
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        static string UserName, UserPassword;
        static string WareHouseID;
        static string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, hasLGV;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["productitemname"] == null || Session["WorkShop"] == null || Session["Equipment"] == null || Session["UserName"] == null || Session["WarehouseID"] == null || Session["RunsGroup"] == null || Session["MachineNumber"] == null)
            {
                Response.Redirect("pass.aspx");
            }
            else
            {
                UserName = Session["UserName"].ToString();
                //   UserPassword = Session["PassWord"].ToString();
                WareHouseID = Session["WarehouseID"].ToString();
                Equipment = Session["Equipment"].ToString();
                WorkShop = Session["WorkShop"].ToString(); //车间 
                Run = Session["Runs"].ToString(); //班组 
                //MachineNumber = Session["MachineNumber"].ToString();
                RunsGroup = Session["RunsGroup"].ToString();
                hasLGV = Session["HasLGV"].ToString();
                //  Response.Expires = -1;
                // }
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
                if (!IsPostBack)
                {
                    GetDataSet();
                    BindingLV();
                    InitTextBlock();
                }
            }
        }
        #region 初始化站台信息
        private void InitTextBlock()
        {
            txtblk_Platform_Num.Text = dockNumber.Equals(string.Empty) ? "未设置" : dockNumber;
            txtblk_Brand.Text = (LoginUser.ItemName == null) ? "未设置" : LoginUser.ItemName;
        }
        #endregion
        #region 获取退料及目的地数据
        private void GetDataSet()
        {
            GetHandleDockNumber();
            if (!dockNumber.Equals(string.Empty))
            {
                containerDetail = null; // rpb.GetContainerDetailByDockNumber(MachineNumber, dockNumber);
                if (containerDetail == null || containerDetail.Tables[0].Rows.Count < 0)
                {
                    return;
                }

                containerDetail.Tables[0].Columns.Add("ENABLEDODD");
                containerDetail.Tables[0].Columns.Add("ExitQty");
                foreach (DataRow row in containerDetail.Tables[0].Rows)
                {
                    row["FIELD1"] = row["ODD1"];
                    row["FIELD2"] = row["ODD2"];
                    row["FIELD3"] = row["ODD3"];
                    row["FIELD4"] = row["QTY"];
                    row["ENABLEDODD"] = Properties.Settings.Default.EnabledODD;
                    row["ExitQty"] = "0";
                }
            }
            targetDestination = rpb.GetReturnPalletDestination();
        }
        #endregion
        #region 获取手工站台站台号
        private void GetHandleDockNumber()
        {
            CommonBLL cb = new CommonBLL(MachineNumber, UserName, UserPassword, WareHouseID);
            DataTable docks = cb.GetMachineDockInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, MachineNumber);
            if (docks != null)
            {
                foreach (DataRow row in docks.Rows)
                {
                    if (Properties.Settings.Default.ContainerTypeCode_Handle.Equals(row["MODELCODE"].ToString()))
                    {
                        dockNumber = row["DOCKNUMBER"].ToString();
                    }
                }
            }
        }
        #endregion
        #region 绑定数据到Listview和Combobox
        private void BindingLV()
        {
            int index = 0;
            if (containerDetail == null)
            {
                lv_Return_Materials.DataSource = null;
                lv_Return_Materials.DataBind();
                ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('没有人工退料的物料!');window.location='Control_Panel.aspx';</script>");
            }
            else
            {
                lv_Return_Materials.DataSource = containerDetail.Tables[0].DefaultView;
                lv_Return_Materials.DataBind();
            }
            if (lv_Return_Materials.Rows != null)
            {
                foreach (GridViewRow grv in lv_Return_Materials.Rows)
                {
                    TextBox txtQty = (TextBox)grv.FindControl("ExitQTY");
                    txtQty.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd1 = (TextBox)grv.FindControl("ODD1");
                    txtodd1.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd2 = (TextBox)grv.FindControl("ODD2");
                    txtodd2.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd3 = (TextBox)grv.FindControl("ODD3");
                    txtodd3.Attributes.Add("onclick", "showNumberInput(this)");
                }
            }

            if (targetDestination == null)
            {
                cb_Target.DataSource = null;
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没能获取到目的区域!');</script>");
                //MessageBoxTimeOutStatic.show(NTIMessage.SYSTEM_HINT, NTIMessage.NO_TARGET_DESTINATION, Hint.ERROR);
            }
            else
            {
                cb_Target.DataSource = targetDestination.Tables[0].DefaultView;
                cb_Target.DataValueField = "EquipmentNumber";
                cb_Target.DataTextField = "Description";
                cb_Target.DataBind();
                foreach (DataRow row in targetDestination.Tables[0].Rows)
                {
                    if ("暂存区".Equals(row["EQUIPMENTNUMBER"].ToString()))
                    {
                        cb_Target.SelectedIndex = index;
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }
        #endregion

        protected void cb_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            string target_Area_Des = cb_Target.SelectedItem.Text.ToString();
            string target_Area_Num = cb_Target.SelectedItem.Value.ToString();
            if (containerDetail == null)
            {
                return;
            }
            foreach (DataRow row in containerDetail.Tables[0].Rows)
            {
                row["FIELD41"] = target_Area_Des;
                row["FIELD31"] = target_Area_Num;
            }
            BindingLV();
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            if (lv_Return_Materials.Rows.Count <= 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('没有人工退料的物料!');window.location='Control_Panel.aspx';</script>");
            }
            if (lv_Return_Materials.Rows != null)
            {
                foreach (GridViewRow row in lv_Return_Materials.Rows)
                {
                    TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
                    string ContainerDetailID = txtContainerDetailID.Text.Trim();
                    TextBox textbox = (TextBox)row.FindControl("ExitQTY");
                    TextBox txtodd1 = (TextBox)row.FindControl("ODD1");
                    TextBox txtodd2 = (TextBox)row.FindControl("ODD2");
                    TextBox txtodd3 = (TextBox)row.FindControl("ODD3");
                    HashSet<TextBox> tbodds = new HashSet<TextBox>();
                    tbodds.Add(textbox);
                    tbodds.Add(txtodd1);
                    tbodds.Add(txtodd2);
                    tbodds.Add(txtodd3);
                    //decimal count=0;
                    foreach (TextBox tb in tbodds)
                    {
                        if (!string.IsNullOrEmpty(tb.Text))
                        {
                            string aa = tb.Text.Trim();
                            string strtempa = ".";
                            int IndexofA = aa.IndexOf(strtempa);
                            int tt = aa.Length - (IndexofA + 1);
                            if (tt > 4)
                            {
                                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('只能输入4位小数!');</script>");
                                tb.Text = "0";
                                return;
                            }
                            else
                            {
                                CompareItemQtyToBom(tb, ContainerDetailID);
                            }
                        }
                        else if (string.IsNullOrEmpty(tb.Text))
                        {
                            tb.Text = "0";
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确!');</script>");
                        }
                    }
                }
                string target_Number = string.Empty;
                ContainerEntity ce = new ContainerEntity();
                DataTable change_DT = containerDetail.Tables[0];

                bool isInit = true;
                #region MyRegion
                foreach (DataRow row in change_DT.Rows)
                {
                    string a = row["ExitQTY"].ToString();

                    decimal qty_n = decimal.Parse(row["ExitQTY"].ToString());
                    decimal qty = qty_n * decimal.Parse(row["RATE"].ToString());
                    string containerDetailID = row["CONTAINERDETAILID"].ToString();
                    string itemID = row["ITEMID"].ToString();
                    string itemName = row["ITEMNAME"].ToString();
                    string itemCode = row["CODE"].ToString();
                    string lotNumber = row["LOTNUMBER"].ToString();
                    decimal odd1 = decimal.Parse(row["ODD1"].ToString());
                    decimal odd2 = decimal.Parse(row["ODD2"].ToString());
                    decimal odd3 = decimal.Parse(row["ODD3"].ToString());

                    decimal count = decimal.Parse(row["FIELD4"].ToString()) + decimal.Parse(row["FIELD1"].ToString()) + decimal.Parse(row["FIELD2"].ToString()) + decimal.Parse(row["FIELD3"].ToString());    //人工发料单总量

                    decimal total_Count = decimal.Parse(row["ExitQTY"].ToString()) + decimal.Parse(row["ODD1"].ToString()) + decimal.Parse(row["ODD2"].ToString()) + decimal.Parse(row["ODD3"].ToString());    //人工退料单修改后的总量

                    if (total_Count > count)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量超出领料总量,请重新输入!');</script>");
                        return;
                    }

                    //if (qty_n + odd1 + odd2 + odd3 == 0)
                    //{
                    //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料量不能为0,请重新输入!');</script>");
                    //    return;
                    //}

                    if (isInit)
                    {
                        target_Number = row["FIELD3"].ToString();
                        ce.Containerbarcode = row["CONTAINERBARCODE1"].ToString();//null 8/3/17:00
                        ce.WAREHOUSEID = row["WAREHOUSEID"].ToString();
                        ce.CONTAINERID = row["CONTAINERID"].ToString();
                        ce.Bomid = row["BOMID"].ToString();//null 8/3/17:00
                        ce.Modelcode = row["MODELCODE"].ToString();
                        isInit = false;
                    }

                    ce.AddContainerDetail_ItemData(containerDetailID, itemID, itemName, qty, itemCode, lotNumber, odd1, odd2, odd3, false);
                }

                //if (rpb.ClientReturnPallet(LoginUser.MachineNumber, dockNumber, ce.Bomid, ce, OperType.CLIENT_RETURNMATERIALDETAIL, "0",
                //    target_Number, LoginUser.RunsTypeCode) > 0)
                //{
                //    ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('人工退料,操作执行成功!');</script>");
                //    GetDataSet();
                //    BindingLV();
                //}
                //else
                //{
                    ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('人工退料,操作执行失败!');</script>");
                //}
                #endregion
            }
        }
        private void CompareItemQtyToBom(TextBox tb, string ContainerDetailID)
        {
            if (containerDetail != null)
            {
                foreach (DataRow row in containerDetail.Tables[0].Rows)
                {
                    if (row["CONTAINERDETAILID"].ToString() == ContainerDetailID)
                    {
                        decimal count = decimal.Parse(row["FIELD4"].ToString()) + decimal.Parse(row["FIELD1"].ToString()) + decimal.Parse(row["FIELD2"].ToString()) + decimal.Parse(row["FIELD3"].ToString());    //人工发料单总量
                        row[tb.ID] = tb.Text;     //写入修改数据到DataSet

                        decimal qty = decimal.Parse(row["ExitQTY"].ToString()) + decimal.Parse(row["ODD1"].ToString()) + decimal.Parse(row["ODD2"].ToString()) + decimal.Parse(row["ODD3"].ToString());    //人工退料单修改后的总量

                        if (qty > count)
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量超出领料总量,请重新输入!');</script>");
                            return;
                        }
                    }
                }
            }
        }

        protected void lv_Return_Materials_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Return_Materials.PageIndex = e.NewPageIndex;
            BindingLV();
        }
    }
}