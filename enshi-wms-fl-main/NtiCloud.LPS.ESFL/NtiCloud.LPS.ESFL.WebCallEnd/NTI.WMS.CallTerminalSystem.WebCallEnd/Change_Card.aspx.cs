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
    public partial class Change_Card : System.Web.UI.Page
    {
        private string target_Area_Des = string.Empty;//static
        private string targe_Area_Number = string.Empty;//static
        //private DataSet machineCurContainInfo;//static
        private ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        private DataSet target;//static
        //private DataSet ds_ContainInfo;//static
        DataTable dtChangeCardFor;//static

        #region

        private int CurRowIndex
        {
            set { ViewState["CurRowIndex"] = value; }
            get { return ViewState["CurRowIndex"] == null ? 0 : Convert.ToInt32(ViewState["CurRowIndex"]); }
        }
        //站台号
        private string dockNumber;
        //区段
        private string modelCode;

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

        private string modelCode1
        {
            set { ViewState["modelCode1"] = value; }
            get { return ViewState["modelCode1"] as string; }
        }

        private DataTable dt_ContainInfo1
        {
            set { ViewState["dt_ContainInfo1"] = value; }
            get { return ViewState["dt_ContainInfo1"] as DataTable; }
        }

        private DataTable machineCurContainInfo1
        {
            set { ViewState["machineCurContainInfo1"] = value; }
            get { return ViewState["machineCurContainInfo1"] as DataTable; }
        }

        private DataSet target1
        {
            set { ViewState["target1"] = value; }
            get { return ViewState["target1"] as DataSet; }
        }//ds_ContainInfo

        private DataSet ds_ContainInfo1
        {
            set { ViewState["ds_ContainInfo1"] = value; }
            get { return ViewState["ds_ContainInfo1"] as DataSet; }
        }//dtChangeCardFor

        private DataTable dtChangeCardFor1
        {
            set { ViewState["dtChangeCardFor1"] = value; }
            get { return ViewState["dtChangeCardFor1"] as DataTable; }
        }

        private string HasLGV
        {
            set { ViewState["HasLGV"] = value; }
            get { return ViewState["HasLGV"] == null ? string.Empty : Convert.ToString(ViewState["HasLGV"]); }
        }
        #endregion

        //加载
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionKey"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            Response.AddHeader("P3P", "CP=CAO PSAOUR");

            if (LoginUser.MachineNumber == "")
            {
                Session["IsToCall"] = "Change_Card.aspx";
                string Url = "Set_Number.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请先设置机台！');window.location='" + Url + "';</script>");
                return;
            }

            Response.Expires = -1;

            if (!IsPostBack)
            {
                string machineNumber = Session["MachineNumber"].ToString();
                string machineName = LoginUser.MachineName;
                string itemName = LoginUser.ItemName;

                Label1.Text = (machineNumber == null) ? "未设置" : machineNumber;//机台编号
                labdocknumber.Text = (machineName == null) ? "未设置" : machineName;//机台名称
                Label2.Text = (itemName == null) ? "未设置" : itemName;//生产牌号
                labbc.Text = LoginUser.RunsTypeCode;

                if (Properties.Settings.Default.AllowODD)
                    hfAllowODD.Value = "1";
                else
                    hfAllowODD.Value = "2";

                InitControl(Properties.Settings.Default.AllowODD);
                BindingComboBox();
                InitDataSet();
                InitTarget();
            }
        }

        /// <summary>
        /// 加载换牌数据
        /// </summary>
        private void InitDataSet()
        {
            //根据机台id获取机台所有站台位的容器信息(换牌退料)
            DataTable machineCurContainInfo = rpb.GetMachineCurContainInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId,LoginUser.Token);
            //target = rpb.GetReturnPalletDestination();
            ////将target放到viewstate中
            //target1 = target;
            
            //设置默认的退料的数量为0  
            dt_ContainInfo1 = this.GetReturnItem(0, machineCurContainInfo, "");//containerDetail
            if (dt_ContainInfo1==null)
            {
                btn_Ensure.Enabled = false;
                TextBox1.Enabled = false;
                target_Area.Enabled = false;
                gvChangeItem.DataSource = null;
                gvChangeItem.DataBind();
                return;
            }
            DataTable dtTaskItemInfo = rpb.GetTheNewItemNumByPlanOutId(LoginUser.Url,"0", LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.Token);
            for (int i = 0; i < dtTaskItemInfo.Rows.Count; i++)
            {
                for (int k = 0; k < dt_ContainInfo1.Rows.Count; k++)
                {
                    if (dtTaskItemInfo.Rows[i]["container_d_id"].ToString() == dt_ContainInfo1.Rows[k]["containerDetailId"].ToString())
                    {
                        dt_ContainInfo1.Rows[k]["planQty"] = dtTaskItemInfo.Rows[i]["plan_qty"].ToString();
                    }
                }
            }
            gvChangeItem.DataSource = dt_ContainInfo1;
            gvChangeItem.DataBind();

            if (machineCurContainInfo == null || machineCurContainInfo.Rows.Count <= 0)
            {
                this.labtarget_area.Visible = false;
                this.target_Area.Visible = false;
                btn_Ensure.Visible = false;
                TextBox1.Visible = false;
                LblMsg.Visible = false;
                // ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没有用于换牌的物料!');window.location='Control_Panel.aspx';</script>");
                return;
            }

            //将machineCurContainInfo放入viewstate中
            machineCurContainInfo1 = machineCurContainInfo;
        }

        private void BindingListview()
        {
            string ddProductName = string.Empty;
            int twoProducts = 0;

            DataSet twoProductNames = rpb.GetTwoProductName(LoginUser.WarehouseID, LoginUser.MachineNumber);
            if (dt_ContainInfo1 != null && dt_ContainInfo1.Rows.Count > 0)
            {
                for (int y = 0; y < dt_ContainInfo1.Rows.Count; y++)
                {
                    if (twoProductNames != null && twoProductNames.Tables.Count > 0 && twoProductNames.Tables[0].Rows.Count > 0)
                    {
                        for (int z = 0; z < twoProductNames.Tables[0].Rows.Count; z++)
                        {
                            if (dt_ContainInfo1.Rows[y]["dockname"].ToString().Equals(twoProductNames.Tables[0].Rows[z]["dockname"].ToString()))
                            {
                                dt_ContainInfo1.Rows[y]["field41"] = twoProductNames.Tables[0].Rows[z]["name"].ToString();
                                break;
                            }
                        }
                    }
                }
            }

            if (dt_ContainInfo1 == null || dt_ContainInfo1.Rows.Count <= 0)//dt_ContainInfo
            {
                gvChangeItem.DataSource = null;
                gvChangeItem.DataBind();
                return;
            }

            if (dt_ContainInfo1 != null && dt_ContainInfo1.Rows.Count > 0)//dt_ContainInfo
            {
                gvChangeItem.DataSource = dt_ContainInfo1;//dt_ContainInfo
                gvChangeItem.DataBind();
            }
            else
            {
                if (gvChangeItem.Rows.Count == 0)
                {
                    this.labtarget_area.Visible = false;
                    this.target_Area.Visible = false;
                    btn_Ensure.Visible = false;
                    //string Url = "Control_Panel.aspx";
                    //ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没有用于换牌的物料!');window.location='" + Url + "';</script>");
                    return;
                }
            }
            if (gvChangeItem.Rows != null)
            {
                foreach (GridViewRow grv in gvChangeItem.Rows)
                {
                    TextBox txtQty = (TextBox)grv.FindControl("txtQty");
                    txtQty.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd1 = (TextBox)grv.FindControl("ODD1");
                    txtodd1.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd2 = (TextBox)grv.FindControl("ODD2");
                    txtodd2.Attributes.Add("onclick", "showNumberInput(this)");

                    TextBox txtodd3 = (TextBox)grv.FindControl("ODD3");
                    txtodd3.Attributes.Add("onclick", "showNumberInput(this)");
                }
            }
            foreach (GridViewRow grv in gvChangeItem.Rows)
            {
                CheckBox cbk = (CheckBox)grv.FindControl("ckbSelect");
                string temp_DockNumber = ((TextBox)grv.FindControl("DOCKNUMBER")).Text.ToString();
                foreach (DataRow drow in dt_ContainInfo1.Rows)
                {
                    if (temp_DockNumber.Equals(drow["DOCKNUMBER"].ToString()))
                    {
                        cbk.Checked = drow["FIELD4"].ToString() == "true" ? true : false;//true
                    }
                }
            }
        }

        private void BindingComboBox()
        {
            try
            {
                //target1.Tables[0].DefaultView.Sort = "EQUIPMENTNUMBER desc";//target
                //DataTable dtDestination = target1.Tables[0].DefaultView.ToTable();//target

                //target_Area.DataSource = target1.Tables[0].DefaultView;//target
                //target_Area.DataTextField = "DESCRIPTION";
                //target_Area.DataValueField = "EQUIPMENTNUMBER";
                //target_Area.DataBind();
                //if (!string.IsNullOrEmpty(Properties.Settings.Default.UnSetReturnItemPlace) && (Properties.Settings.Default.UnSetReturnItemPlace.Equals("true")))
                //{
                //    this.target_Area.Enabled = false;
                //}

                labtarget_area.Visible = true;
                target_Area.Visible = true;
                ListItem myListItem = new ListItem("平衡库", Properties.Settings.Default.ReturnPallet_TargetAddress);
                target_Area.Items.Add(myListItem);
                target_Area.SelectedIndex = 0;
            }
            catch
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没能获取到目的区域!');</script>");
            }
        }

        public DataTable GetReturnItem(decimal qty, DataTable dtContainerDetails, string containerDetailId)
        {
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
                        if (dr["containerDetailId"] != null && dr["containerDetailId"].ToString().Equals(containerDetailId))
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

            }
            catch { return null; }
            return dtItem;
            //DataSet dsItem = new DataSet();
            //DataTable dtItem = new DataTable();

            //try
            //{
            //    if (dtContainerDetails.Rows.Count <= 0) return null;
            //    dtItem = dtContainerDetails.Clone();
            //    DataRow drItem = null;

            //    //如果containerdetailid为空则修改所有的containerdetail的数据
            //    if (string.IsNullOrEmpty(containerDetailId))
            //    {
            //        foreach (DataRow dr in dtContainerDetails.Rows)
            //        {
            //            dr.BeginEdit();
            //            dr["QTY"] = qty;
            //            dr.EndEdit();
            //            drItem = dtItem.NewRow();
            //            drItem = dr;
            //            dtItem.Rows.Add(drItem.ItemArray);
            //        }
            //    }
            //    else
            //    {
            //        foreach (DataRow dr in dtContainerDetails.Rows)
            //        {
            //            if (dr["CONTAINERDETAILID"] != null && dr["CONTAINERDETAILID"].ToString().Equals(containerDetailId))
            //            {
            //                dr.BeginEdit();
            //                dr["QTY"] = qty;
            //                dr.EndEdit();

            //            }
            //            drItem = dtItem.NewRow();
            //            drItem = dr;
            //            dtItem.Rows.Add(drItem.ItemArray);
            //        }
            //    }
            //    dtChangeCardFor = new DataTable();
            //    dtChangeCardFor = dtItem.Clone();

            //    dtChangeCardFor = dtItem.Copy();

            //    //dtChangeCardFor放到viewstate中
            //    dtChangeCardFor1 = dtChangeCardFor;

            //    dsItem.Tables.Add(dtItem);
            //}
            //catch { return null; }
            //if (dsItem == null || dsItem.Tables.Count == 0 || dsItem.Tables[0].Rows == null)
            //{
            //    dsItem.Tables.Add(new DataTable());
            //}
            //return dsItem;
        }


        #region 根据配置，是否允许零头来控制显示
        /// </summary>
        /// <param name="AllowOdd">true 允许零头； false 不允许零头</param>
        private void InitControl(bool AllowOdd)
        {
            if (!AllowOdd)
            {
                gvChangeItem.Columns[7].Visible = false;
                gvChangeItem.Columns[8].Visible = false;
                gvChangeItem.Columns[9].Visible = false;
            }
        }
        #endregion

        #region
        protected void ckbSelect_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb = (CheckBox)sender;
            GridViewRow row = (GridViewRow)ckb.Parent.NamingContainer;
            string temp_DockNumber = ((TextBox)row.FindControl("DOCKNUMBER")).Text.ToString();
            foreach (DataRow drow in dt_ContainInfo1.Rows)//dt_ContainInfo
            {
                if (temp_DockNumber.Equals(drow["DOCKNUMBER"].ToString()))
                {
                    drow["FIELD1"] = (ckb.Checked == true) ? "true" : "false";
                }
            }
            if (ckb.Checked == true)
            {
                CheckSameModelCode(true, temp_DockNumber);
            }
            else
            {
                CheckSameModelCode(false, temp_DockNumber);
            }
            //}
        }
        #endregion

        #region 同一站台的记录同时选中
        private void CheckSameModelCode(bool isCheck, string temp_DockNumber)
        {
            //DataTable dt = dt_ContainInfo;
            //for (int i = 0; i <= gvChangeItem.Rows.Count - 1; i++)
            //{
            //    GridViewRow row = gvChangeItem.Rows[i];
            //    CheckBox ckbSelect = row.FindControl("ckbSelect") as CheckBox;
            //    Label lblDocNumber = row.FindControl("lblDocNumber") as Label;
            //    if (temp_DockNumber.Equals(lblDocNumber.Text.Trim()))
            //    {
            //        ckbSelect.Checked = isCheck;
            //        dt_ContainInfo.Rows[i]["FIELD4"] = isCheck == true ? "true" : "false";
            //    }
            //}
            for (int i = 0; i < dt_ContainInfo1.Rows.Count; i++)//dt_ContainInfo
            {
                if (temp_DockNumber.Equals(dt_ContainInfo1.Rows[i]["DOCKNUMBER"]))//dt_ContainInfo
                {
                    dt_ContainInfo1.Rows[i]["FIELD4"] = isCheck == true ? "true" : "false";//dt_ContainInfo
                }
            }
            //dt_ContainInfo = dt;
            BindingListview();
        }
        #endregion

        protected void gvChangeItem_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvChangeItem.PageIndex = e.NewPageIndex;
            //bool bFind = false;
            //foreach (GridViewRow row in gvChangeItem.Rows)
            //{
            //    if (bFind)
            //    {
            //        break;
            //    }
            //    TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
            //    string ContainerDetailID = txtContainerDetailID.Text.Trim();
            //    TextBox textbox = (TextBox)row.FindControl("txtQty");
            //    //CheckBox checkbox = (CheckBox)row.FindControl("ckbSelect");
            //    if (!string.IsNullOrEmpty(textbox.Text))
            //    {
            //        if (CommonFuctions.CheckDecemal(textbox.Text))
            //        {
            //            string count = textbox.Text;
            //            UpdateContainerDetailsQty(textbox, ContainerDetailID);
            //        }
            //        else
            //        {
            //            bFind = true;
            //            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确!');</script>");
            //            break;
            //        }
            //    }
            //    decimal qty = decimal.Parse(((TextBox)row.FindControl("txtQty")).Text.ToString()) * decimal.Parse(((TextBox)row.FindControl("RATE")).Text.ToString());
            //    string itemid = ((TextBox)row.FindControl("ITEMID")).Text.ToString();
            //    string bomid = ((TextBox)row.FindControl("BOMID")).Text.ToString();
            //    if (!rpb.CompareItemQtyToBom(itemid, qty, 0, 0, 0, bomid))
            //    {
            //        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料总量不能超出BOM的数量,请重新输入!');</script>");
            //        return;
            //    }
            //}
            //if (bFind)
            //{
            //    return;
            //}
            //BindingListview();
            CheckQty();
        }

        #region 检查退余料数量是否符合格式要求
        /// <summary>
        /// 检查退余料数量是否符合格式要求
        /// </summary>
        /// <returns>false：不符合；true：符合</returns>
        private bool CheckQty()
        {
            bool bFind = false;
            foreach (GridViewRow row in gvChangeItem.Rows)
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
        #endregion

        private void InitTarget()
        {
            if (target_Area.Items.Count <= 0 || ds_ContainInfo1 == null || ds_ContainInfo1.Tables.Count <= 0 || ds_ContainInfo1.Tables[0].Rows.Count <= 0)//ds_ContainInfo
            {
                return;
            }
            target_Area_Des = target_Area.SelectedItem.Text.ToString();
            targe_Area_Number = target_Area.SelectedItem.Value.ToString();

            //将target_Area_Des和targe_Area_Number放到viewstate中
            target_Area_Des1 = target_Area_Des;
            targe_Area_Number1 = targe_Area_Number;

            foreach (DataRow row in ds_ContainInfo1.Tables[0].Rows)//ds_ContainInfo
            {
                if (row["MODELCODE"].ToString().Equals(Properties.Settings.Default.ContainerTypeCode_Handle))
                {
                    row["FIELD2"] = Properties.Settings.Default.Transient_Area_Dock_Number;
                    row["FIELD3"] = "暂存区退料接收站台";
                }
                else
                {
                    row["FIELD2"] = targe_Area_Number;
                    row["FIELD3"] = target_Area_Des;
                }
            }
        }

        protected void target_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = ds_ContainInfo1.Tables[0];//ds_ContainInfo
            for (int i = CurRowIndex; i <= ds_ContainInfo1.Tables[0].Rows.Count - 1; i++)//ds_ContainInfo
            {
                if (ds_ContainInfo1.Tables[0].Rows[i]["MODELCODE"].ToString() == Properties.Settings.Default.ContainerTypeCode_Handle)
                {
                    ds_ContainInfo1.Tables[0].Rows[i]["FIELD2"] = Properties.Settings.Default.EquipmentNumber_HandWorkReturnTarget;//EQUIPMENTNUMBER  
                    ds_ContainInfo1.Tables[0].Rows[i]["FIELD3"] = "暂存区退料接收站台";//DESCRIPTION
                }
            }
        }

        private void UpdateContainerDetailsQty(TextBox tb, string containerDetailId)
        {
            if (string.IsNullOrEmpty(containerDetailId)) return;

            string count = tb.Text.Trim();
            if (dt_ContainInfo1 == null || dt_ContainInfo1.Rows.Count <= 0) return;//ds_ContainInfo
            dt_ContainInfo1 = this.GetReturnItem(decimal.Parse(count), dt_ContainInfo1, containerDetailId);
            this.gvChangeItem.DataSource = dt_ContainInfo1;//dt_ContainInfo
            gvChangeItem.DataBind();
        }

        private void CompareItemQtyToBom(TextBox tb, DataTable dt, string ContainerDetailID)
        {
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["CONTAINERDETAILID"].ToString() == ContainerDetailID)
                    {
                        if (tb.Text.Length > 10)
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('您输入的数值位数太长,请重新输入!');</script>");
                            return;
                        }

                        row[tb.ID] = tb.Text;     //写入修改数据到DataSet
                        if (row["MODELCODE"].ToString().Equals(Properties.Settings.Default.ContainerTypeCode_Handle))
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Ensure_Click(object sender, EventArgs e)
        {
            try
            {
                if (TextBox1.Text.ToString().Trim() == "" || TextBox1.Text.ToString().Trim() == null)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请输入密码！');</script>");
                    TextBox1.Visible = true;
                    Label1.Visible = true;
                    return;
                }

                if (!TextBox1.Text.ToString().Trim().Equals(Properties.Settings.Default.AdminPassword))
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('密码错误，请重新输入！');</script>");
                    return;
                }
                if (Make_Task())
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料成功。');window.location='Control_Panel.aspx';</script>");
                }

            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('" + ex.Message + "');</script>");
            }
        }

        //退料处理
        private bool Make_Task()
        {
            CheckQty();

            #region 数据获取和验证
            foreach (GridViewRow row in gvChangeItem.Rows)
            {
                TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
                string ContainerDetailID = txtContainerDetailID.Text.Trim();
                TextBox textbox = (TextBox)row.FindControl("txtQty");
                Label lblPlanQty = (Label)row.FindControl("txtPlanQty");

                if (textbox.Text == "" || textbox.Text == null || textbox.Text == String.Empty)
                {
                    textbox.Text = "0.00";
                }
                Decimal returnqty = 0;
                if (CommonFuctions.CheckDecemal(textbox.Text.Trim()))
                {
                    returnqty = Convert.ToDecimal(textbox.Text.Trim());
                    //for (int i = 0; i < machineCurContainInfo1.Rows.Count; i++)
                    //{
                    //    if (machineCurContainInfo1.Rows[i]["containerDetailId"].Equals(ContainerDetailID))
                    //    {
                    //        machineCurContainInfo1.Rows[i]["qty"] = returnqty;
                    //    }
                    //}

                    decimal planQty = Convert.ToDecimal(lblPlanQty.Text);
                    if (returnqty > planQty)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料量不能大于标准计量！');</script>");
                        return false;
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确！');</script>");
                    return false;
                }
            }
            #endregion

            string dockNumber_Succeed = string.Empty;
            string dockNumber_Error = string.Empty;

            bool flag = false;
            string target_Number = string.Empty;
            string hasLGV = LoginUser.HasLGV.ToString();
            DataTable change_DataTable = dt_ContainInfo1;

            DataView dv = new DataView(change_DataTable);
            if (dv.Count > 0)
            {
                DataTable dt = dv.ToTable();

                JsonObject bomInfo = new JsonObject();
                JsonArray details = new JsonArray();
                foreach (DataRow row in dt.Rows)
                {
                    decimal qty = decimal.Parse(row["qty"].ToString());
                    //decimal qty_Rate = qty * decimal.Parse(row["ITEM_RATE"].ToString());
                    JsonObject obj = new JsonObject();
                    obj.Add("containerDetailId", row["containerDetailId"].ToString());//容器明细id
                    obj.Add("dock_id", row["dock_id"].ToString());//容器明细id
                    obj.Add("itemId", row["item_id"].ToString());//物料id
                    obj.Add("itemName", row["itemName"].ToString());//物料名称
                    obj.Add("planQty", row["planQty"].ToString());//要料时物料的数量
                    obj.Add("qty", qty);//退余料的数量
                    obj.Add("lotNo", row["lot_no"].ToString());//批次
                    obj.Add("ownerId", row["supplier_id"].ToString());//供应商
                    details.Add(obj);

                }

                bomInfo.Add("ReturnItems", details);
                //bomInfo.Add("FullPallet", fullPallet);

                //下面的内容，在换牌时再进行修改
                string msg = rpb.ChangeCard(LoginUser.Url, LoginUser.SessionKey, IsCkbCanBeCallOut.Checked, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, bomInfo.ToString(), LoginUser.HasLGV.ToString(), LoginUser.Token);
                if (!string.IsNullOrEmpty(msg))
                {
                    dockNumber_Error += "退余料失败！原因：" + msg + "  ";
                }
            }

            if (string.IsNullOrEmpty(dockNumber_Error))
            {
                return true;
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('" + dockNumber_Error + "');</script>");
            }

            return flag;

            //#region 数据获取和验证
            //foreach (GridViewRow row in gvChangeItem.Rows)
            //{
            //    TextBox txtContainerDetailID = (TextBox)row.FindControl("txtContainerDetailID");
            //    string ContainerDetailID = txtContainerDetailID.Text.Trim();
            //    TextBox textbox = (TextBox)row.FindControl("txtQty");
            //    HiddenField hid = (HiddenField)row.FindControl("hidBomQty");

            //    if (textbox.Text == "" || textbox.Text == null || textbox.Text == String.Empty)
            //    {
            //        textbox.Text = "0.00";
            //    }
            //    Decimal returnqty = 0;
            //    if (CommonFuctions.CheckDecemal(textbox.Text.Trim()))
            //    {
            //        returnqty = Convert.ToDecimal(textbox.Text.Trim());
            //        for (int i = 0; i < machineCurContainInfo1.Rows.Count; i++)
            //        {
            //            if (machineCurContainInfo1.Rows[i]["containerDetailId"].Equals(ContainerDetailID))
            //            {
            //                machineCurContainInfo1.Rows[i]["Qty"] = returnqty;
            //            }
            //        }

            //        decimal qtyBom = Convert.ToDecimal(hid.Value);
            //        if (returnqty > qtyBom)
            //        {
            //            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退料量不能大于配盘单标准量！');</script>");
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据格式不正确！');</script>");
            //        return false;
            //    }
            //}
            //#endregion

            //string dockNumber_Succeed = string.Empty;
            //string dockNumber_Error = string.Empty;

            //bool flag = false;
            //string target_Number = string.Empty;
            //string hasLGV = LoginUser.HasLGV.ToString();
            //DataTable change_DataTable = machineCurContainInfo1;

            ////按站台分组
            //var q = from t in change_DataTable.AsEnumerable()
            //        group t by new { f1 = t.Field<string>("DOCK_NUMBER") } into m
            //        select new
            //        {
            //            DockNumber = m.Key.f1
            //        };

            //foreach (var dock in q)
            //{
            //    string dockNumber = dock.DockNumber;
            //    #region

            //    DataView dv = new DataView(change_DataTable);
            //    dv.RowFilter = "DOCK_NUMBER='" + dockNumber + "'";
            //    string fullPallet = "1";

            //    if (dv.Count > 0)
            //    {
            //        DataTable dt = dv.ToTable();

            //        JsonObject bomInfo = new JsonObject();
            //        DataRow rowItem = dt.Rows[0];
            //        bomInfo.Add("BomId", rowItem["BOM_ID"].ToString());
            //        bomInfo.Add("SchemeId", LoginUser.SchemeId);
            //        bomInfo.Add("PalletCode", rowItem["CONTAINER_BARCODE"].ToString());
            //        bomInfo.Add("Modelcode", rowItem["MODEL_CODE"].ToString());
            //        bomInfo.Add("ContainerId", rowItem["CONTAINER_ID"].ToString());
            //        bomInfo.Add("CanCallOut", "1");

            //        JsonArray details = new JsonArray();

            //        string returnModelCode = string.Empty;
            //        foreach (DataRow row in dt.Rows)
            //        {
            //            decimal qty = decimal.Parse(row["QTY"].ToString());
            //            if (qty == 0 || qty < decimal.Parse(row["BOMQTY"].ToString()))
            //            {
            //                fullPallet = "0";
            //            }

            //            decimal qty_Rate = qty * decimal.Parse(row["ITEM_RATE"].ToString());

            //            JsonObject obj = new JsonObject();
            //            obj.Add("containerDetailId", row["containerDetailId"].ToString());//容器明细id
            //            obj.Add("dock_id", row["dock_id"].ToString());//容器明细id
            //            obj.Add("itemId", row["itemId"].ToString());//物料id
            //            obj.Add("itemName", row["itemName"].ToString());//物料名称
            //            obj.Add("planQty", row["planQty"].ToString());//要料时物料的数量
            //            obj.Add("qty", qty);//退余料的数量
            //            obj.Add("lotNo", row["lotNo"].ToString());//批次
            //            obj.Add("ownerId", row["ownerId"].ToString());//供应商
            //            details.Add(obj);

            //        }

            //        bomInfo.Add("ReturnItems", details);
            //        //bomInfo.Add("FullPallet", fullPallet);

            //        //下面的内容，在换牌时再进行修改
            //        //string msg = rpb.ClientReturnPallet(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, dockNumber,
            //        //    bomInfo.ToString(), LoginUser.HasLGV.ToString(), LoginUser.RunsTypeCode);
            //        //if (!string.IsNullOrEmpty(msg))
            //        //{
            //        //    dockNumber_Error += dockNumber + "退余料失败！原因：" + msg + "  ";
            //        //}
            //    }
            //    #endregion
            //}

            //if (string.IsNullOrEmpty(dockNumber_Error))
            //{
            //    return true;
            //}
            //else
            //{
            //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('" + dockNumber_Error + "');</script>");
            //}

            //return flag;
        }

        //返回
        protected void btn_Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        private void SetTowProductName()
        {
            int twoProducts = 0;
            string ddProductName = string.Empty;
            string onlyOne = string.Empty;

            DataSet twoProductNames = rpb.GetTwoProductName(LoginUser.WarehouseID, LoginUser.MachineNumber);
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
                    this.Label3.Visible = this.Label4.Visible = true;
                    string beforeProductName = string.Empty;
                    for (int x = 0; x < twoProductNames.Tables[0].Rows.Count; x++)
                    {
                        beforeProductName = twoProductNames.Tables[0].Rows[x]["name"].ToString();
                        if (!string.IsNullOrEmpty(beforeProductName))
                        {
                            if (x == 0)
                            {
                                this.Label2.Text = beforeProductName;
                            }
                            else
                            {
                                this.Label4.Text = beforeProductName;
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
                            if (!onlyOne.Equals(LoginUser.ItemName))
                            {
                                this.Label3.Visible = this.Label4.Visible = true;
                                this.Label4.Text = onlyOne;
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
                        if (!onlyOne.Equals(LoginUser.ItemName))
                        {
                            this.Label3.Visible = this.Label4.Visible = true;
                            this.Label4.Text = onlyOne;
                        }
                    }
                }
            }
        }
    }
}
