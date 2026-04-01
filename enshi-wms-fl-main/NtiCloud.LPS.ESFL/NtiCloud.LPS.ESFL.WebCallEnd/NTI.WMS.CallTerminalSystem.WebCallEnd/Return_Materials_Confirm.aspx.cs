using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using System.Text;
using WMSInterface;
using NTI.WMS.CallTerminalSystem.WebCallEnd.Common;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Return_Materials_Confirm : System.Web.UI.Page
    {
        private ReturnBillOperatorBLL rbob =
            new ReturnBillOperatorBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private ReturnPalletBLL rpb =
                        new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private static DataSet returnMaterialBill;
        private static DataTable returnBillDT;
        private static DataTable materialDT;
        private DataChange dc = new DataChange();
        private static DataTable machineNumbers;  //含在退料单的机台
        private static DataTable dockNumbers;       //含有退料单的站台(指定机台号)
        private static DataTable dockNumber_Binded;    //用于绑定ComboBox的站台信息
        private static DataTable dockBills;
        private static string MachineNumber;
        private static int selectcount;
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["Equipment"] == null || Session["UserName"] == null || Session["WarehouseID"] == null || Session["RunsGroup"] == null)
            //{
            //    Response.Redirect("pass.aspx");
            //}
            //else
            //{
                //EquipmentNumber = LoginUser.EquipmentNumber;
                //UserName = Session["UserName"].ToString();
                //UserPassword = Session["PassWord"].ToString();
                //WareHouseID = Session["WarehouseID"].ToString();
                //Equipment = Session["Equipment"].ToString();
                //WorkShop = Session["WorkShop"].ToString(); //车间 
                //Run = Session["Runs"].ToString(); //班组 
                //MachineNumber = Session["MachineNumber"].ToString();
                //RunsGroup = Session["RunsGroup"].ToString();
                //hasLGV = Session["HasLGV"].ToString();
                Response.Expires = -1;
                
                //if (Session["MachineName"] != null)
                //{
                //    MachineName = Session["MachineName"].ToString();
                //}
                //else
                //{
                //    MachineName = LoginUser.MachineName;
                //}
          //  }
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
            if (!IsPostBack)
            {
                GetReturnBillDataSet();
                GetMachineNumbers();
                Set_Default_MachineNumber();
                btn_Query_Click(null, null);
            }
        }

        public void MsgBox(string Message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "", "<script type='text/javascript'>alert('" + Message + "');</script>");
            return;
        }

        private void GetReturnBillDataSet()
        {
            returnMaterialBill = rbob.GetAllReturnMaterialBill();
            SaveData();
        }
        /// <summary>
        /// 复制DataSet的部分字段值,用于记录未修改前的数据
        /// </summary>
        private void SaveData()
        {
            if (returnMaterialBill == null || returnMaterialBill.Tables[0].Rows.Count == 0)
            {
                return;
            }
            foreach (DataRow row in returnMaterialBill.Tables[0].Rows)
            {
                row["FIELD1"] = row["QTY1"];
                row["FIELD2"] = row["ODD1"];
                row["FIELD3"] = row["ODD2"];
                row["FIELD4"] = row["ODD3"];
                if (OperType.CLIENT_CALLTERMINAL_RETURNBADPALLET.Equals(row["TYPECODE"].ToString()))
                {
                    row["FIELD11"] = "true";
                    row["FIELD21"] = "false";
                }
                else
                {
                    row["FIELD11"] = "false";
                }
            }
        }

        private void GetMachineNumbers()
        {
            machineNumbers = new DataTable();
            machineNumbers.Columns.Add("MACHINENUMBER");
            machineNumbers.Rows.Add("全部");

            foreach (DataRow row in returnMaterialBill.Tables[0].Rows)
            {
                if (machineNumbers == null || machineNumbers.Rows.Count <= 0)
                {
                    machineNumbers.Rows.Add(row["MACHINENUMBER"]);
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow machineRow in machineNumbers.Rows)
                    {
                        if ((machineRow["MACHINENUMBER"].ToString()).Equals(row["MACHINENUMBER"].ToString()))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        machineNumbers.Rows.Add(row["MACHINENUMBER"]);
                    }
                }
            }
            if (machineNumbers == null || machineNumbers.Rows.Count <= 0)
            {
                cb_MachineNumber.DataSource = null;
                cb_MachineNumber.DataBind();
                return;
            }
            else
            {
                cb_MachineNumber.DataSource = machineNumbers.DefaultView;
                cb_MachineNumber.DataValueField = "MACHINENUMBER";
                cb_MachineNumber.DataTextField = "MACHINENUMBER";
                cb_MachineNumber.DataBind();
            }
            cb_MachineNumber_SelectedIndexChanged(null, null);
        }

        private void Set_Default_MachineNumber()
        {
            string _machineNumber = MachineNumber;
            int _index = 0;
            foreach (DataRow row in machineNumbers.Rows)
            {
                if (_machineNumber.Equals(row["MACHINENUMBER"].ToString()))
                {
                    cb_MachineNumber.SelectedIndex = _index;
                }
                else
                {
                    _index++;
                }
            }
            //cb_DockNumber.SelectedIndex =0;
        }

        protected void btn_Query_Click(object sender, EventArgs e)
        {
            string machineNum =cb_MachineNumber.SelectedItem.Value.ToString();
            string dockNum = cb_DockNumber.SelectedItem.Value.ToString();
            string filterString = string.Empty;

            if (machineNum.Equals("全部") && dockNum.Equals("全部站台"))
            {
                filterString = "";
            }
            else
            {
                if (machineNum.Equals("全部"))
                {
                    filterString = string.Format("DOCKNUMBER='" + dockNum + "'");

                }
                else if (dockNum.Equals("全部站台"))
                {
                    filterString = string.Format("MACHINENUMBER='" + machineNum + "'");
                }
                else
                {
                    filterString = string.Format("MACHINENUMBER='" + machineNum + "'" + " AND DOCKNUMBER='" + dockNum + "'");
                }
            }
            dockBills = dc.RowFilterFromDataset(dockNumbers, filterString);
            BindingLVReturnBill();      //刷新数据
        }

        private void BindingLVReturnBill()
        {
            LVData();
            //lv_Return_Bill.ItemsSource = returnBill.Tables[0].DefaultView;
            if (returnBillDT == null || returnBillDT.Rows.Count < 1)
            {
                lv_Return_Bill.DataSource = null;
                lv_Return_Bill.DataBind();
                return;
            }
            lv_Return_Bill.DataSource = returnBillDT.DefaultView;
            lv_Return_Bill.DataBind();
            CheckAll(null, null);
        }
        private void LVData()
        {
            //bool isSetted = false;
            string billid = string.Empty;

            returnBillDT = new DataTable();
            returnBillDT.Columns.Add("BILLID");
            returnBillDT.Columns.Add("MACHINENUMBER");
            returnBillDT.Columns.Add("DOCKNUMBER");
            returnBillDT.Columns.Add("BILLTYPENAME");
            returnBillDT.Columns.Add("CREATETIME");
            returnBillDT.Columns.Add("CONTAINERTYPE");
            returnBillDT.Columns.Add("BOMCODE");
            returnBillDT.Columns.Add("PALLETNUMBER");
            returnBillDT.Columns.Add("CONTAINERID");

            if (dockBills == null || dockBills.Rows.Count <= 0)
            {
                return;
            }
            foreach (DataRow row in dockBills.Rows)
            {
                DataRow dataRow = returnBillDT.NewRow();
                if (!billid.Equals(row["BILLID"].ToString()))
                {
                    dataRow["BILLID"] = row["BILLID"];
                    dataRow["MACHINENUMBER"] = row["MACHINENUMBER"];
                    dataRow["DOCKNUMBER"] = row["DOCKNUMBER"];
                    dataRow["BILLTYPENAME"] = row["BILLTYPENAME"];
                    dataRow["CREATETIME"] = row["CREATETIME"];
                    dataRow["CONTAINERTYPE"] = row["CONTAINERTYPE"];
                    dataRow["BOMCODE"] = row["BOMCODE"];
                    dataRow["PALLETNUMBER"] = row["PALLETNUMBER"];
                    dataRow["CONTAINERID"] = row["CONTAINERID"];
                    billid = row["BILLID"].ToString();
                    returnBillDT.Rows.Add(dataRow);
                }
            }
        }
        #region TextBox单击事件
        
        private void CompareItemQtyToBOM(TextBox tb, string containerDetailID)
        {
            string value;
            if (!string.IsNullOrEmpty(containerDetailID))
            {
                foreach (DataRow row in materialDT.Rows)
                {
                    if (containerDetailID.Equals(row["CONTAINERDETAILID"].ToString()))
                    {
                        string itemID = row["ITEMID"].ToString();
                        string bomID = row["BOMID"].ToString();
                        value = row[tb.ID].ToString();

                        if (tb.Text.Length > 10)
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('您输入的数值位数太长,请重新输入!');</script>");
                            tb.Text = value;
                            return;
                        }

                        row[tb.ID] = tb.Text;
                        decimal qty = decimal.Parse(row["QTY1"].ToString()) * decimal.Parse(row["RATE"].ToString());

                        if ((row["CONTAINERTYPE"].ToString()).Equals(Properties.Settings.Default.ContainerTypeCode_Handle))
                        {
                            return;
                        }

                        if (!rpb.CompareItemQtyToBom(itemID, qty, decimal.Parse(row["ODD1"].ToString()), decimal.Parse(row["ODD2"].ToString()), decimal.Parse(row["ODD3"].ToString()), bomID))
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量不能超出BOM的数量,请重新输入!');</script>");                         
                        }
                    }
                }
            }
        }
        #endregion
        protected void cb_MachineNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ComboBox cb = sender as ComboBox;
            string machineNumber = cb_MachineNumber.SelectedItem.Value.ToString();
            string filterString = string.Format("MACHINENUMBER='" + machineNumber + "'");
            if (machineNumber.Equals("全部"))
            {
                dockNumbers = dc.RowFilterFromDataset(returnMaterialBill, "");
            }
            else
            {
                dockNumbers = dc.RowFilterFromDataset(returnMaterialBill, filterString);
            }

            InitDockNumbers(dockNumbers);

            if (dockNumber_Binded == null || dockNumber_Binded.Rows.Count <= 0)
            {
                cb_DockNumber.DataSource = null;
                cb_DockNumber.DataBind();
                return;
            }
            else
            {
                cb_DockNumber.DataSource = dockNumber_Binded.DefaultView;
                cb_DockNumber.DataTextField = "DOCKNUMBER";
                cb_DockNumber.DataValueField = "DOCKNUMBER";
                cb_DockNumber.DataBind();
            }
        }
        private void InitDockNumbers(DataTable dt)
        {
            dockNumber_Binded = new DataTable();
            dockNumber_Binded.Columns.Add("DOCKNUMBER");

            dockNumber_Binded.Rows.Add("全部站台");

            if (dt == null || dt.Rows.Count <= 0)
            {
                return;
            }
            foreach (DataRow row in dt.Rows)
            {
                if (dockNumber_Binded == null || dockNumber_Binded.Rows.Count <= 0)
                {
                    dockNumber_Binded.Rows.Add(row["DOCKNUMBER"]);
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow dockRow in dockNumber_Binded.Rows)
                    {
                        if ((dockRow["DOCKNUMBER"].ToString()).Equals(row["DOCKNUMBER"].ToString()))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        dockNumber_Binded.Rows.Add(row["DOCKNUMBER"]);
                    }
                }
            }
        }

        protected void CheckAll(object sender, EventArgs e)
        {
            //foreach (GridViewRow gv in lv_Return_Bill.Rows)
            //{
            //    CheckBox cbk = (CheckBox)gv.FindControl("chkSelect");
            //    cbk.Checked = true;
            //}
            selectcount = lv_Return_Bill.Rows.Count;
            BindList(selectcount);
        }

        protected void ChkSelect_select(object sender, EventArgs e)
        {
            CheckBox cbk = sender as CheckBox;
            if (cbk.Checked == true)
            {
                selectcount--;
            }
            else
            {
                selectcount++;
                
            }
            BindList(selectcount);
        }
        private void BindList(int selectcoun)
        {
            StringBuilder billID_SB = new StringBuilder("(");
            string rowFilterString = string.Empty;
            if (lv_Return_Bill != null&&selectcount>1)
            {
                foreach (GridViewRow gv in lv_Return_Bill.Rows)
                {
                    CheckBox cbkl = (CheckBox)gv.FindControl("chkSelect");
                    if (cbkl.Checked == true)
                    {
                        string billID = string.Format("'" + ((TextBox)gv.FindControl("BILLID")).Text.ToString() + "'" + ",");
                        billID_SB.Append(billID);
                    }
                }
                billID_SB.Remove(billID_SB.Length - 1, 1);
                billID_SB.Append(")");
                rowFilterString = string.Format("BILLID in " + billID_SB.ToString());
            }
            else if (lv_Return_Bill != null && selectcount > 0)
            {
                foreach (GridViewRow gv in lv_Return_Bill.Rows)
                {
                    CheckBox cbkl = (CheckBox)gv.FindControl("chkSelect");
                    if (cbkl.Checked == true)
                    {
                        string billID = ((TextBox)gv.FindControl("BILLID")).Text.ToString();
                        rowFilterString = string.Format("BILLID='" + billID + "'");
                    }
                }
            }
            materialDT = new DataChange().RowFilterFromDataset(returnMaterialBill, rowFilterString);

            if (materialDT != null)
            {
                materialDT.Columns.Add("ENABLEDODD");
                foreach (DataRow row in materialDT.Rows)
                {
                    row["ENABLEDODD"] = Properties.Settings.Default.EnabledODD;
                }
            }
            Binding_lv_Return_Materials_list();
        }
        private void Binding_lv_Return_Materials_list()
        {
            if (returnBillDT == null || returnBillDT.Rows.Count == 0)
            {
                lv_Return_Materials_list.DataSource = null;
                lv_Return_Materials_list.DataBind();
                return;
            }
            if (materialDT == null)
            {
                lv_Return_Materials_list.DataSource = null;
                lv_Return_Materials_list.DataBind();
                return;
            }
            lv_Return_Materials_list.DataSource = materialDT.DefaultView;
            lv_Return_Materials_list.DataBind();
            if (lv_Return_Materials_list.Rows != null)
            {
                foreach (GridViewRow grv in lv_Return_Materials_list.Rows)
                {
                    TextBox txtQty = (TextBox)grv.FindControl("QTY1");
                    txtQty.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd1 = (TextBox)grv.FindControl("ODD1");
                    txtodd1.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd2 = (TextBox)grv.FindControl("ODD2");
                    txtodd2.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd3 = (TextBox)grv.FindControl("ODD3");
                    txtodd3.Attributes.Add("onclick", "showNumberInput(this)");
                }
            }
        }

        protected void lv_Return_Materials_list_SelectionChanged(object sender, EventArgs e)
        {
            GridViewRow grv=(GridViewRow)((CheckBox)sender).Parent.Parent;
            //DataRowView drv = (DataRowView)lv_Return_Materials_list.SelectedItem;
        }

        /// <summary>
        /// 确认退料单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Confirm_Item_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            GridViewRow grv = (GridViewRow)((Button)sender).Parent.Parent;
            string _billID = ((TextBox)grv.FindControl("BILLID")).Text.ToString();
            int index = 0;
            string _dockNumber = string.Empty;

            foreach (DataRow row in returnBillDT.Rows)
            {
                if (_billID.Equals(row["BILLID"].ToString()))
                {
                    lv_Return_Bill.SelectedIndex = index;
                    _dockNumber = row["DOCKNUMBER"].ToString();
                    break;
                }
                else
                {
                    index++;
                }
            }

            if (MakeTask() > 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('确认退料单操作执行成功!');</script>");
                btn_requery_Click(null, null);
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('确认退料单操作执行失败!');</script>");
            }
        }

        protected void btn_Return_Item_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            GridViewRow grv=(GridViewRow)btn.NamingContainer;
            string _billID = ((TextBox)grv.FindControl("BILLID")).Text.ToString();
            string _palletNumber = string.Empty;
            string _containerID = string.Empty;
            string _machineNumber = string.Empty;
            string _dockNumber = string.Empty;
            foreach (DataRow row in returnBillDT.Rows)
            {
                if (_billID.Equals(row["BILLID"]))
                {
                    _palletNumber = row["PALLETNUMBER"].ToString();
                    _containerID = row["CONTAINERID"].ToString();
                    _machineNumber = row["MACHINENUMBER"].ToString();
                    _dockNumber = row["DOCKNUMBER"].ToString();
                    break;
                }
            }

            if (rbob.Return_MaterialBillByBillID(_billID, _palletNumber, _containerID, _machineNumber, _dockNumber) > 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退单操作成功!');</script>");
                Refresh();
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料操作失败!');</script>");
            }
        }

        private int MakeTask()
        {
            ContainerEntity ce = new ContainerEntity();
            bool isInitCE = false;

            foreach (DataRow row in materialDT.Rows)
            {
                string itemID = row["ITEMID"].ToString();
                string containerDetailID = row["CONTAINERDETAILID"].ToString();
                string itemName = row["ITEMNAME"].ToString();
                string lotNumber = row["LOTNUMBER"].ToString();
                decimal qty = decimal.Parse(row["QTY1"].ToString()) * decimal.Parse(row["RATE"].ToString());
                decimal odd1 = decimal.Parse(row["ODD1"].ToString());
                decimal odd2 = decimal.Parse(row["ODD2"].ToString());
                decimal odd3 = decimal.Parse(row["ODD3"].ToString());
                bool isLock = row["FIELD11"].ToString().Equals("true") ? bool.Parse(row["FIELD21"].ToString()) : false;

                if (!isInitCE)
                {
                    ce.BillID = row["BILLID"].ToString();
                    ce.Bomid = row["BOMID"].ToString();
                    ce.Containerbarcode = row["PALLETNUMBER"].ToString();
                    ce.CONTAINERID = row["CONTAINERID"].ToString();
                    //ce.FirstInTime = row["CREATETIME"].ToString();
                    ce.WAREHOUSEID = row["WAREHOUSEID"].ToString();
                    ce.Modelcode = row["CONTAINERTYPE"].ToString();
                    isInitCE = true;
                }
                ce.AddContainerDetail_ItemData(containerDetailID, itemID, itemName, qty, null, lotNumber, odd1, odd2, odd3, isLock);
            }
            return rbob.ClientConfirmReturnBill_Warehouse(ce, ce.BillID);
        }

        #region 修改退料量
        
        protected void btn_Change_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            GridViewRow grv = (GridViewRow)((Button)sender).Parent.Parent;
            string _containDetailID = ((TextBox)grv.FindControl("txtContainerDetailID")).Text.ToString();
            TextBox textbox = (TextBox)grv.FindControl("QTY1");
            TextBox txtodd1 = (TextBox)grv.FindControl("ODD1");
            TextBox txtodd2 = (TextBox)grv.FindControl("ODD2");
            TextBox txtodd3 = (TextBox)grv.FindControl("ODD3");
            HashSet<TextBox> oddCollection = new HashSet<TextBox>();
            oddCollection.Add(textbox);
            oddCollection.Add(txtodd1);
            oddCollection.Add(txtodd2);
            oddCollection.Add(txtodd3);
            foreach (TextBox txt in oddCollection)
            {
                if (txt.Text != "")
                {
                    CompareItemQtyToBOM(txt, _containDetailID);
                }
            }
            //lv_Return_Materials_list.SelectedIndex = DataChanged.GetLine(materialDT, textbox, "CONTAINERDETAILID", _containDetailID);

            decimal qty_n = 0, qty = 0, odd1 = 0, odd2 = 0, odd3 = 0;
            string _containerType = null, _itemID = null, _bomID = null, _itemName = null;

            foreach (DataRow row in materialDT.Rows)
            {
                if (_containDetailID.Equals(row["CONTAINERDETAILID"].ToString()))
                {
                    //qty_n = decimal.Parse(textbox.Text.ToString());
                    qty_n = decimal.Parse(row["QTY1"].ToString());
                    qty = qty_n * decimal.Parse(row["RATE"].ToString());
                    //odd1 = Convert.ToDecimal(txtodd1.Text);
                    //odd2 = Convert.ToDecimal(txtodd2.Text);
                    //odd3 = Convert.ToDecimal(txtodd3.Text);
                    odd1 = decimal.Parse(row["ODD1"].ToString());
                    odd2 = decimal.Parse(row["ODD2"].ToString());
                    odd3 = decimal.Parse(row["ODD3"].ToString());
                    _containerType = row["CONTAINERTYPE"].ToString();
                    _itemID = row["ITEMID"].ToString();
                    _bomID = row["BOMID"].ToString();
                    _itemName = row["ITEMNAME"].ToString();
                    break;
                }
            }

            if (!_containerType.Equals(Properties.Settings.Default.ContainerTypeCode_Handle))
            {
                if (!rpb.CompareItemQtyToBom(_itemID, qty, odd1, odd2, odd3, _bomID))
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量不能超出BOM的数量,请重新输入!');</script>");
                    return;
                }
            }
            //bool ff = true;
            //if (ff)
            //{
            //    string msg = NTIMessage.CHANGE_COUNT_HINT + "\n" + _itemName + "\n实发数量: " + qty_n + "\t零头1: " + odd1 +
            //    "\n零头2: " + odd2 + "\t零头3: " + odd3;
            //    MsgBox(msg);
            //}

            if (rbob.UpdateReturnItemQty(_containDetailID, qty, odd1, odd2, odd3) > 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('修改退料量操作执行成功!');</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('修改退料量操作执行失败!');</script>");
                foreach (DataRow row in materialDT.Rows)
                {
                    if (_containDetailID.Equals(row["CONTAINERDETAILID"].ToString()))
                    {
                        row["QTY1"] = row["FIELD1"];
                        row["ODD1"] = row["FIELD2"];
                        row["ODD2"] = row["FIELD3"];
                        row["ODD3"] = row["FIELD4"];
                    }
                }
            }
            btn_requery_Click(null, null);
        }
        #endregion

        protected void btn_requery_Click(object sender, EventArgs e)
        {
            Refresh();
        }
        private void Refresh()
        {
            GetReturnBillDataSet();
            GetMachineNumbers();
            btn_Query_Click(null, null);
        }

        protected void lv_Create_Bill_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_Return_Bill.PageIndex = e.NewPageIndex;
            BindingLVReturnBill();
        }

        //protected void lv_Return_Materials_list_PageIndexChanging(object sender, GridViewPageEventArgs e)
        //{
        //    lv_Return_Materials_list.PageIndex = e.NewPageIndex;

        //}

        protected void CheckBox_Checked(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            GridViewRow grv=(GridViewRow)cb.NamingContainer;
            string containerDetailID = ((TextBox)grv.FindControl("txtContainerDetailID")).Text.ToString();
            foreach (DataRow row in materialDT.Rows)
            {
                if (containerDetailID.Equals(row["CONTAINERDETAILID"].ToString()))
                {
                    row["FIELD21"] = "true";
                }
            }
        }

        protected void btn_All_confirm_Click(object sender, EventArgs e)
        {
            int i;
            int errors = 0, succ = 0;
            if (lv_Return_Bill == null || lv_Return_Bill.Rows.Count <= 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没有可退的物料！');</script>");
                return;
            }
            if (selectcount <= 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请选择确认退料的退料单！');</script>");
                return;
            }
            //foreach (GridView grv in lv_Return_Bill.Rows)
            //{
            //    CheckBox cb = (CheckBox)grv.FindControl("chkSelect");
            //    if (cb.Checked == true)
            //    {

            //    }
            //}
            for (i = 0; i < lv_Return_Bill.Rows.Count; i++)
            {
                lv_Return_Bill.SelectedIndex = i;

                if (MakeTask() > 0)
                {
                    succ++;
                }
                else
                {
                    errors++;
                }
            }
            if (succ == lv_Return_Bill.Rows.Count)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('确认退料单操作执行成功!');</script>");
            }
            else if (errors == lv_Return_Bill.Rows.Count)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('确认退料单操作执行失败!');</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('确认退料单操作部分成功!');</script>");
            }
            Refresh();
        }

        protected void btn_Quit_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }
    }
}
