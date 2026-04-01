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
    public partial class Return_Waste_Material : System.Web.UI.Page
    {
        string UserName, UserPassword, WareHouseID, ItemName;
        string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, hasLGV, EquipmentNumber, DockNumber;
        private string target_Area_Des;//static
        private string targe_Area_Number;//static
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private DataSet containerDetail;//static
        private DataSet return_Destination;//static
        private DataSet lastReturn;//static
        int modifyBomQty = 0;

        private string target_Area_Des1
        {
            set { ViewState["target_Area_Des1"] = value; }
            get { return ViewState["target_Area_Des1"] as string; }
        }

        private string targe_Area_Number1
        {
            set { ViewState["targe_Area_Number1"] = value; }
            get { return ViewState["targe_Area_Number1"] as string; }
        }

        private DataSet containerDetail1
        {
            set { ViewState["containerDetail1"] = value; }
            get { return ViewState["containerDetail1"] as DataSet; }
        }

        private DataSet return_Destination1
        {
            set { ViewState["return_Destination1"] = value; }
            get { return ViewState["return_Destination1"] as DataSet; }
        }

        private DataSet lastReturn1
        {
            set { ViewState["lastReturn1"] = value; }
            get { return ViewState["lastReturn1"] as DataSet; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            EquipmentNumber = LoginUser.EquipmentNumber;
            UserName = Session["UserName"].ToString();
            //  UserPassword = Session["PassWord"].ToString();
            WareHouseID = Session["WarehouseID"].ToString();
            Equipment = Session["Equipment"].ToString();
            WorkShop = Session["WorkShop"].ToString(); //车间 
            Run = Session["Runs"].ToString(); //班组 
            //MachineNumber = Session["MachineNumber"].ToString();
            RunsGroup = Session["RunsGroup"].ToString();
            hasLGV = Session["HasLGV"].ToString();
            DockNumber = Session["DockNumber"].ToString();
            ItemName = Session["productitemname"].ToString();
            Response.Expires = -1;
            MachineName = LoginUser.MachineName;
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
                InitData();
                InitTextBlock();
                BindingListview();
                BindingCombobox();

                //注销假退按钮  PEQ 20110329
                //this.rb_F_Return.Enabled = false;
                //this.rb_True_Return.Checked = true;
            }

            //重新刷新生产牌号  add by pengenqi 2012/8/6
            this.SetTowProductName();
        }

        private void InitData()
        {
            containerDetail = null; // rpb.GetContainerDetailByDockNumber(MachineNumber, DockNumber);
            if (containerDetail != null)
            {
                containerDetail.Tables[0].Columns.Add("ENABLEDODD");
                foreach (DataRow row in containerDetail.Tables[0].Rows)
                {
                    row["ENABLEDODD"] = Properties.Settings.Default.EnabledODD;
                }
            }
            //添加containerDetail中
            containerDetail1 = containerDetail;

            return_Destination = rpb.GetReturnPalletDestination();

            //return_Destination添加到viewstate中
            return_Destination1 = return_Destination;
        }

        private void InitTextBlock()
        {
            bool flag = false;
            if (containerDetail1 != null && containerDetail1.Tables[0].Rows.Count > 0)//containerDetail
            {
                flag = true;
            }
            txtblk_Sock_Code.Text = flag ? containerDetail1.Tables[0].Rows[0]["CURPALLETNUMBER"].ToString() : "未设置";//containerDetail
            txtblk_Platform_Num.Text = DockNumber;
            txtblk_Brand.Text = ItemName;
            txtblk_Section.Text = flag ? containerDetail1.Tables[0].Rows[0]["MODELCODE"].ToString() : "未设置";//containerDetail
            txtblk_Platform_Name.Text = flag ? containerDetail1.Tables[0].Rows[0]["DOCKNAME"].ToString() : "未设置";//containerDetail
        }

        private void BindingListview()
        {
            if (containerDetail1 == null || containerDetail1.Tables[0].Rows.Count == 0)//containerDetail
            {
                lv_ReturnMaterial.DataSource = null;
                lv_ReturnMaterial.DataBind();
                return;
            }
            //设置默认的退料的数量为0  
            DataSet dsContainerDetails = this.GetReturnItem(0, containerDetail1.Tables[0], "");//containerDetail

            lv_ReturnMaterial.DataSource = dsContainerDetails;
            lv_ReturnMaterial.DataBind();
        }

        public DataSet GetReturnItem(decimal qty, DataTable dtContainerDetails, string containerDetailId)
        {
            DataSet dsItem = new DataSet();
            DataTable dtItem = new DataTable();

            try
            {
                if (dtContainerDetails.Rows.Count <= 0) return null;
                dtItem = dtContainerDetails.Clone();
                DataRow drItem = null;

                //如果containerdetailid为空则修改所有的containerdetail的数据
                if (string.IsNullOrEmpty(containerDetailId))
                {
                    foreach (DataRow dr in dtContainerDetails.Rows)
                    {
                        dr.BeginEdit();
                        dr["QTY"] = qty;
                        dr.EndEdit();
                        drItem = dtItem.NewRow();
                        drItem = dr;
                        dtItem.Rows.Add(drItem.ItemArray);
                    }
                }
                else
                {
                    foreach (DataRow dr in dtContainerDetails.Rows)
                    {
                        if (dr["CONTAINERDETAILID"] != null && dr["CONTAINERDETAILID"].ToString().Equals(containerDetailId))
                        {
                            dr.BeginEdit();
                            dr["QTY"] = qty;
                            dr.EndEdit();

                        }
                        drItem = dtItem.NewRow();
                        drItem = dr;
                        dtItem.Rows.Add(drItem.ItemArray);
                    }
                }

                dsItem.Tables.Add(dtItem);
            }
            catch { return null; }
            return dsItem;
        }

        //设置退料目的地  
        private void BindingCombobox()
        {
            try
            {
                //绑定退料的目的地  所有的数据默认退往暂存区   
                //配置退料目的地  
                //ListItem myListItem1 = new ListItem("暂存区", "暂存区");
                //target_Area.Items.Add(myListItem1);
                ListItem myListItem = new ListItem("废料回收间", "1001");
                //return_Destination1.Tables[0].DefaultView.Sort = "EQUIPMENTNUMBER desc";//return_Destination
                //DataTable dtDestination = return_Destination1.Tables[0].DefaultView.ToTable();//return_Destination
                //target_Area.DataSource = return_Destination1.Tables[0].DefaultView;//return_Destination
                //target_Area.DataTextField = "description";
                //target_Area.DataValueField = "equipmentnumber";                                
                //target_Area.DataBind();
                target_Area.Items.Add(myListItem);
                target_Area.SelectedIndex = 0;

                //设置退料目的地不可选  
                if (!string.IsNullOrEmpty(Properties.Settings.Default.UnSetReturnItemPlace) && (Properties.Settings.Default.UnSetReturnItemPlace.Equals("true")))
                {
                    this.target_Area.Enabled = false;
                }
            }
            catch
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没能获取到目的区域!');</script>");
            }
        }

        protected void GridViewReturnBill_ToNext_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_ReturnMaterial.PageIndex = e.NewPageIndex;
            CheckQty();
        }

        protected void target_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            target_Area_Des = target_Area.SelectedItem.Text.ToString();
            targe_Area_Number = target_Area.SelectedValue.ToString();

            //添加target_Area_Des到viewstate中
            target_Area_Des1 = target_Area_Des;
            targe_Area_Number1 = targe_Area_Number;

            foreach (DataRow row in containerDetail1.Tables[0].Rows)//containerDetail
            {
                row["FIELD2"] = target_Area_Des;        //row[19] -> row["FIELD2"] 
            }
            foreach (GridViewRow row in lv_ReturnMaterial.Rows)
            {
                Label textbox = (Label)row.FindControl("targetAddress");
                textbox.Text = target_Area_Des;
            }
        }

        protected void RadioButton_Click(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string operName = rb.Text.ToString();
            ChangeReturn(operName);
        }

        private void ChangeReturn(string operName)
        {
            switch (operName)
            {
                case "退废料":
                    target_Area.Enabled = true;
                    foreach (DataRow row in containerDetail1.Tables[0].Rows)//containerDetail
                    {
                        row["FIELD2"] = target_Area.SelectedItem.Text.ToString();  //row[19] -> row["FIELD2"]  
                    }
                    break;
                case "假退(退下一班)":
                    target_Area.Enabled = false;
                    foreach (DataRow row in containerDetail1.Tables[0].Rows)//containerDetail
                    {
                        row["FIELD2"] = row["DOCKNUMBER"];          //row[19] -> row["FIELD2"]  
                        if (targe_Area_Number1 != row["FIELD2"].ToString())      //row[19] -> row["FIELD2"]    targe_Area_Number
                        {
                            targe_Area_Number1 = row["FIELD2"].ToString();       //row[19] -> row["FIELD2"]   targe_Area_Number
                        }
                    }
                    break;
            }
        }

        private bool CheckQty()
        {
            bool bFind = false;
            foreach (GridViewRow row in lv_ReturnMaterial.Rows)
            {
                if (bFind)
                {
                    break;
                }
                TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
                string ContainerDetailID = txtContainerDetailID.Text.Trim();
                TextBox textbox = (TextBox)row.FindControl("txtQTY");
                if (!string.IsNullOrEmpty(textbox.Text))
                {
                    if (CommonFuctions.CheckDecemal(textbox.Text.Trim()))
                    {
                        this.UpdateContainerDetailsQty(textbox, ContainerDetailID);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确,请重新输入!');</script>");
                        bFind = true;
                        break;
                    }
                }
            }
            if (bFind)
            {
                return false;
            }
            return true;
        }

        protected void btn_Finish_Click(object sender, EventArgs e)
        {
            TextBox1.Text = "ntinti56";
            if (TextBox1.Text.ToString().Trim() == "" || TextBox1.Text.ToString().Trim() == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请带班长输入退料密码!');</script>");
                TextBox1.Visible = true;
                Label1.Visible = true;
                return;
            }
            else if (TextBox1.Text.ToString().Trim().Equals(Properties.Settings.Default.AdminPassword))
            {
                if (lv_ReturnMaterial.Rows != null)
                {
                    if (!CheckQty())
                    {
                        return;
                    }
                }
                //targe_Area_Number = target_Area.SelectedItem.Value.ToString();
                DataSet change_DataSet = new DataSet();
                ContainerEntity ce = new ContainerEntity();
                change_DataSet = containerDetail1;//containerDetail
                bool evaluate = false;

                foreach (DataRow row in change_DataSet.Tables[0].Rows)
                {
                    targe_Area_Number1 = row["FIELD2"].ToString();//targe_Area_Number
                    decimal qty = decimal.Parse(row["QTY"].ToString());
                    decimal odd1 = decimal.Parse(row["ODD1"].ToString());
                    decimal odd2 = decimal.Parse(row["ODD2"].ToString());
                    decimal odd3 = decimal.Parse(row["ODD3"].ToString());

                    //取消最大配盘数量的限制  PEQ 20110412
                    if (!rpb.CompareItemQtyToBom(row["ITEMID"].ToString(), qty, odd1, odd2, odd3, row["BOMID"].ToString()))
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量不能超出BOM的数量,请重新输入!');</script>");
                        return;
                    }

                    if (evaluate == false)
                    {
                        ce.Containerbarcode = row["CONTAINERBARCODE1"].ToString();
                        ce.WAREHOUSEID = row["WAREHOUSEID"].ToString();
                        ce.CONTAINERID = row["CONTAINERID"].ToString();
                        ce.Bomid = row["BOMID"].ToString();
                        ce.Modelcode = row["MODELCODE"].ToString();
                        evaluate = true;
                    }
                    ce.AddContainerDetail_ItemData(row["ITEMID"].ToString(), row["ITEMNAME"].ToString(), qty, null, row["LOTNUMBER"].ToString(),
                        decimal.Parse(row["ODD1"].ToString()), decimal.Parse(row["ODD2"].ToString()), decimal.Parse(row["ODD3"].ToString()), false,
                        row["ITEMSUPPLIER"].ToString(), NTIVirtualMachine.Component.Util.TypeConvert.ToDecimal(row["ITEMRATE"].ToString()));
                }

                if (rb_True_Return.Checked == true)
                {
                    string test = RunsGroup + "-" + Run;
                    if (rpb.ClientReturnWaste(MachineNumber, DockNumber) > 0)//targe_Area_Number  "3041"  //this.target_Area.SelectedValue.ToString()
                    {
                        Session["rb_ReturnRemnantsMaterials"] = true;
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退 废 料，操作执行成功!');window.location='Call_Terminal.aspx';</script>");
                        TextBox1.Visible = false;
                        Label1.Visible = false;
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退 废 料，操作执行失败!');</script>");
                    }
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料密码不正确,请联系相关人员!');</script>");
            }
            //if (rb_F_Return.Checked == true)
            //{
            //    if (rpb.ClientReturnPallet_ToNext(LoginUser.MachineNumber, LoginUser.DockNumber, ce.Bomid, ce,
            //        OperType.CLIENT_RETURNMATERIAL_FALSE_THIS, targe_Area_Number,LoginUser.RunsTypeCode) > 0)
            //    {
            //        Session["rb_ReturnRemnantsMaterials"] = true;
            //        ClientScript.RegisterStartupScript(this.GetType(), "", "alert('假退，操作执行成功!');window.location='Call_Terminal.aspx'");
            //    }
            //    else
            //    {
            //        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('假退，操作执行失败!');</script>");
            //    }
            //}
        }

        private void UpdateContainerDetailsQty(TextBox tb, string containerDetailId)
        {
            if (string.IsNullOrEmpty(containerDetailId)) return;
            //否则直接修改containerdetail的数据
            //获取textbox中的数值
            string count = tb.Text;
            if (lv_ReturnMaterial.Rows == null) return;
            containerDetail1 = this.GetReturnItem(decimal.Parse(count), containerDetail1.Tables[0], containerDetailId);//containerDetail
            this.lv_ReturnMaterial.DataSource = containerDetail1.Tables[0];//containerDetail
            this.lv_ReturnMaterial.DataBind();
        }

        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            target_Area_Des = string.Empty;
            targe_Area_Number = string.Empty;
            containerDetail = null;
            return_Destination = null;
            lastReturn = null;

            Response.Redirect("Call_Terminal.aspx");
        }

        protected void lv_ReturnMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SetTowProductName()
        {
            int twoProducts = 0;
            string ddProductName = string.Empty;
            string onlyOne = string.Empty;


            DataSet twoProductNames = rpb.GetTwoProductName(WareHouseID, MachineNumber);
            //如果同一个机台存在两个生产牌号的情况下  退料的时候进行比较显示前一个生产牌号
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
                    string beforeProductName = string.Empty;
                    string productdockname = string.Empty;
                    for (int x = 0; x < twoProductNames.Tables[0].Rows.Count; x++)
                    {
                        beforeProductName = twoProductNames.Tables[0].Rows[x]["name"].ToString();
                        productdockname = twoProductNames.Tables[0].Rows[x]["dockname"].ToString();
                        //containerDetail1.Tables[0].Rows[0]["DOCKNAME"].ToString()
                        if (!string.IsNullOrEmpty(beforeProductName))
                        {
                            if (containerDetail1 != null && containerDetail1.Tables.Count > 0
                                && containerDetail1.Tables[0].Rows.Count > 0)
                            {
                                if (containerDetail1.Tables[0].Rows[0]["DOCKNAME"].ToString().Equals(productdockname))
                                {
                                    txtblk_Brand.Text = beforeProductName;
                                }
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
                        if (!onlyOne.Equals(ItemName))
                        {
                            this.txtblk_Brand.Text = onlyOne;
                        }
                    }
                }
            }
        }
    }
}