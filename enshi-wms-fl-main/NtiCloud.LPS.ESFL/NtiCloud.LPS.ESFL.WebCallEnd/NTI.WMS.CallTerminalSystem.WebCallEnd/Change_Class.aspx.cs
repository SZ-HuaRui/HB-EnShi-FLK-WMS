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
    public partial class Change_Class : System.Web.UI.Page
    {
        private DataTable containerDetail;//static
        private DataTable initData, UsedMaterialData;//static
        private DataTable dt;//static
        private DataTable Dataset1;//static
        UsedMaterialsHead umh = new UsedMaterialsHead();
        List<UsedMaterialsDetail> useddetails = new List<UsedMaterialsDetail>();

        string Schemeid;

        //触发textbox的textchange事件后，修改contertainerDetail的数量  PEQ 20110405
        //private ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

        string EquipmentNumber
        {
            get { return Request.Cookies["MachineDockSetting"] == null ? "RF" : Convert.ToString(Request.Cookies["MachineDockSetting"].Values["EQUIPMENTID"]); }
        }

        private DataTable containerDetail1
        {
            set { ViewState["containerDetail1"] = value; }
            get { return ViewState["containerDetail1"] as DataTable; }
        }

        private DataTable initData1
        {
            set { ViewState["initData1"] = value; }
            get { return ViewState["initData1"] as DataTable; }
        }

        private DataTable dt1
        {
            set { ViewState["dt1"] = value; }
            get { return ViewState["dt1"] as DataTable; }
        }

        private DataTable Dataset11
        {
            set { ViewState["Dataset11"] = value; }
            get { return ViewState["Dataset11"] as DataTable; }
        }


        /// <summary>
        /// 交接班站台id
        /// </summary>
        private string ChangeClassDock
        {
            set { ViewState["ChangeClassDock"] = value; }
            get
            {
                if (ViewState["ChangeClassDock"] == null)
                    return null;
                return ViewState["ChangeClassDock"].ToString();
            }
        }

        //加载
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionKey"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }

            Response.AddHeader("P3P", "CP=CAO PSAOUR");

            if (Session["Rateto1"] == null || Session["Rateto1"].ToString() != "1")
            {
                Session["Rateto1"] = "0";
            }
            //Response.Expires = -1;
            //绑定机台的信息 
            if (string.IsNullOrEmpty(LoginUser.MachineNumber))
            {
                string page = "Set_Number.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('请先设置机台信息！');window.location='" + page + "';</script>");
                return;
            }
            if (!Page.IsPostBack)
            {
                string MachineNumber, MachineName, ItemName;
                MachineNumber = LoginUser.MachineNumber;
                ItemName = LoginUser.ItemName;
                MachineName = LoginUser.MachineName;

                label1.Text = (MachineNumber == "") ? "未设置" : MachineNumber;//MachineNumber;
                labdocknumber.Text = (MachineName == "") ? "未设置" : MachineName;
                label2.Text = (ItemName == "") ? "未设置" : ItemName;
                labbc.Text = LoginUser.RunsTypeCode;

                GetAndBindData(true);
            }

            //this.SetTowProductName();
        }

        private void GetAndBindData(bool isChangePersonReceive)
        {
            Dataset11 = GetDataSet(isChangePersonReceive);
            //GetUsedMaterialData();
            BindDataSet();
        }

        //检查当前机台当前班次是否已经确认过消耗
        private bool CheckHavedEnsure(string MachineNumber, string RunsTypeCode, string StartTime, string EndTime)
        {
            try
            {
                ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
                return rpb.CheckHavedEnsure(Properties.Settings.Default.WareHouseID.ToString(), MachineNumber, RunsTypeCode, StartTime, EndTime);
            }
            catch
            {
                return false;
            }
        }

        #region 加载界面数据
        /// <summary>
        /// 加载界面数据
        /// </summary>
        /// <returns></returns>
        private DataTable GetDataSet(bool isChangePersonReceive)
        {
            //  InitInfo();
            ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            DataSet dsBindData = new DataSet();
            initData = new DataTable();
            initData = rpb.GetMachineCurItemsInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.Token, isChangePersonReceive);
            if (initData == null || initData.Rows.Count == 0)
            {
                ckbSelect.Visible = false;
                btn_Ensure.Visible = false;
                return null;
            }

            DataTable dt = rpb.GetClassDockNumberByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.Token);
            if (dt == null)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('无法访问服务器！');location.href='Control_Panel.aspx';</script>");
                return null;
            }
            if (dt.Rows.Count == 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台未初始化交接班站台！');location.href='Control_Panel.aspx';</script>");
                return null;
            }
            if (dt.Rows.Count != 1)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台对应的交接班站台有多个，一个机台只能有一个交接班站台！');location.href='Control_Panel.aspx';</script>");
                return null;
            }
            ChangeClassDock = dt.Rows[0]["dock_id"].ToString();

            DataTable dtTaskItemInfo = rpb.GetTheNewItemNumByPlanOutId(LoginUser.Url,(isChangePersonReceive==true?"1":"0"), LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.Token);
            for (int i = 0; i < dtTaskItemInfo.Rows.Count; i++)
            {
                for (int k = 0; k < initData.Rows.Count; k++)
                {
                    if (dtTaskItemInfo.Rows[i]["container_d_id"].ToString() == initData.Rows[k]["containerDetailId"].ToString() && dtTaskItemInfo.Rows[i]["container_id"].ToString() == initData.Rows[k]["container_id"].ToString())
                    {
                        initData.Rows[k]["planQty"] = dtTaskItemInfo.Rows[i]["plan_qty"].ToString();
                    }
                }
            }
            Dataset11 = initData;
            return Dataset11;
        }
        #endregion

        private DataTable GetUsedMaterialData()
        {
            ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            UsedMaterialData = new DataTable();
            UsedMaterialData.Clear();
            UsedMaterialData = rpb.GetUsedMaterialsByRuns(LoginUser.MachineNumber, "", "").Tables[0];

            CommonBLL bll = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            DataSet ds = null; //  bll.GetMachineCurPlanByMachineNumber(MachineNumber);
            if (ds != null && ds.Tables != null)
            {
                Schemeid = ds.Tables[0].Rows[0]["Schemeid"].ToString().Trim();
            }
            if (UsedMaterialData == null)
            {
                GridViewReturnBill_ToNext.DataSource = null;
                GridViewReturnBill_ToNext.DataBind();
                return null;
            }
            DataTable dtt = new DataTable();
            dtt.Clear();
            dtt = UsedMaterialData;
            dtt.Columns.Add("USEDMATERIALSDETAILID");
            dtt.Columns.Add("Field1");
            dtt.Columns.Add("Field2");
            dtt.Columns.Add("Field3");
            dtt.Columns.Add("Schemeid");
            foreach (DataRow row in dtt.Rows)
            {
                row["USEDMATERIALSDETAILID"] = Guid.NewGuid().ToString("N");
                row["Field1"] = null;
                row["Field2"] = null;
                row["Field3"] = null;
                row["Schemeid"] = Schemeid;
            }
            //UsedDataTable = dtt;
            return dtt;
        }

        private void BindDataSet()
        {
            if (Dataset11 == null || Dataset11.Rows.Count <= 0)
            {
                GridViewReturnBill_ToNext.DataSource = null;
                GridViewReturnBill_ToNext.DataBind();
                return;
            }
            else
            {
                GridViewReturnBill_ToNext.DataSource = Dataset11;//测试  Dataset1
                GridViewReturnBill_ToNext.DataBind();
            }

            ////查询是否已经做交班,此处的方法需要测试
            //ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            //DataTable dtChangeClass = rbob.GetMachineReturnMaterialBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.SchemeId, ChangeClassDock, LoginUser.Token);

            //if (dtChangeClass == null || dtChangeClass.Rows.Count == 0)
            //{
            //    return;
            //}

            //string createdate = DateTime.Parse(dtChangeClass.Rows[0]["create_date"].ToString()).ToString("yyyy-MM-dd").Substring(0, 10);
            //string shiftId = dtChangeClass.Rows[0]["shift_id"].ToString().Trim();
            //string teamId = dtChangeClass.Rows[0]["team_id"].ToString().Trim();
            //if (LoginUser.RunsCodeId == shiftId && LoginUser.RunsGroupId == teamId && DateTime.Now.ToString("yyyy-MM-dd").Equals(createdate))
            //{
            //    btn_Ensure.Visible = false;
            //    btn_Modify.Visible = true;
            //    LblChangeMsg.Visible = true;
            //}
        }

        #region 翻页事件
        /// <summary>
        /// 翻页事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridViewReturnBill_ToNext_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewReturnBill_ToNext.PageIndex = e.NewPageIndex;
            CheckQty();
        }
        #endregion

        #region 检查退余料数量是否符合格式要求
        /// <summary>
        /// 检查退余料数量是否符合格式要求
        /// </summary>
        /// <returns>false：不符合；true：符合</returns>
        private bool CheckQty()
        {
            bool bFind = false;
            foreach (GridViewRow row in GridViewReturnBill_ToNext.Rows)
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

        #region 重新绑定退余料界面数据源
        /// <summary>
        /// 重新绑定退余料界面数据源
        /// </summary>
        /// <param name="tb">输入余料数量的控件</param>
        /// <param name="containerDetailId">容器明细id</param>
        private void UpdateContainerDetailsQty(TextBox tb, string containerDetailId)
        {
            if (string.IsNullOrEmpty(containerDetailId)) return;
            //否则直接修改containerdetail的数据
            //获取textbox中的数值
            string count = tb.Text;
            if (GridViewReturnBill_ToNext.Rows == null) return;
            Dataset11 = this.GetReturnItem(decimal.Parse(count), Dataset11, containerDetailId);//containerDetail
            this.GridViewReturnBill_ToNext.DataSource = Dataset11;//containerDetail
            this.GridViewReturnBill_ToNext.DataBind();
        }
        #endregion

        #region 根据容器明细id，修改退余料界面数据源中退余料中物料的数量
        /// <summary>
        /// 根据容器明细id，修改退余料界面数据源中退余料中物料的数量
        /// </summary>
        /// <param name="qty">退余料数量</param>
        /// <param name="dtContainerDetails">退余料界面数据源</param>
        /// <param name="containerDetailId">容器明细id</param>
        /// <returns></returns>
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
        }
        #endregion

        //提交
        protected void btn_Ensure_Click(object sender, EventArgs e)
        {
            ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            CheckQty();
            try
            {
                if (GridViewReturnBill_ToNext.Rows == null || GridViewReturnBill_ToNext.Rows.Count == 0)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('没有交接班记录！');location.href='Control_Panel.aspx';</script>");
                }
                if (Make_Task())
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('交班操作成功。');location.href='Control_Panel.aspx';</script>");
                    Session["Change_Class"] = "haved";
                }
            }
            catch { }

        }

        private bool Make_Task()
        {
            ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            bool isAllZero = true;
            DataTable deitems = Dataset11;//GetDataSet();

            try
            {
                #region
                foreach (DataRow row in deitems.Rows)
                {
                    decimal qty = decimal.Parse(row["QTY"].ToString());

                    if (qty > 0)
                    {
                        isAllZero = false;
                        break;
                    }
                }

                if (isAllZero == true)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('交班的所有数据为零,不能进行交接班!');</script>");
                    return false;
                }
                #endregion

                JsonObject itemList = new JsonObject();
                JsonArray details = new JsonArray();
                foreach (DataRow row in deitems.Rows)
                {
                    decimal qty = decimal.Parse(row["qty"].ToString());

                    if (qty >= 0)
                    {
                        decimal planQty = Convert.ToDecimal(row["planQty"]);

                        if (qty > planQty)
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('“" + row["itemName"] + "”的交班数量不能超出标准数量 " + planQty + " , 请重新输入!');</script>");
                            return false;
                        }
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('“" + row["itemName"] + "”的交班数量不能为负数, 请重新输入!');</script>");
                        return false;
                    }


                    JsonObject obj = new JsonObject();
                    obj.Add("containerDetailId", row["containerDetailId"].ToString());//容器明细id
                    obj.Add("itemId", row["itemId"].ToString());//物料id
                    obj.Add("itemName", row["itemName"].ToString());//物料名称
                    obj.Add("planQty", row["planQty"].ToString());//要料时物料的数量
                    obj.Add("qty", qty);//交班的数量
                    obj.Add("lotNo", row["lotNo"].ToString());//批次
                    obj.Add("ownerId", row["ownerId"].ToString());//供应商
                    details.Add(obj);
                }

                itemList.Add("ReturnItems", details);

                string msg = rpb.ClientReturnPallet_ToNext(LoginUser.Url,ckbSelect.Checked, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, itemList.ToString(), LoginUser.HasLGV.ToString(), LoginUser.Token);
                if (string.IsNullOrEmpty(msg))
                {
                    return true;
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('交班失败！原因：" + msg + "');</script>");
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('交班出现错误：" + ex.Message + "');</script>");
            }

            return false;
        }

        protected void btn_Return_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
            //ClientScript.RegisterStartupScript(Page.GetType(), "",
            //"<script>window.open('Control_Panel.aspx','aa','top=0,left=0,resizable=no,status=yes,menubar=no,scrollbars=yes');window.moveTo(0,0);window.resizeTo(screen.width,screen.height);</script>");
        }

        private void SetTowProductName()
        {
            ReturnPalletBLL rpb = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
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
                    // this.label3.Visible = this.Label4.Visible = true;
                    string beforeProductName = string.Empty;
                    for (int x = 0; x < twoProductNames.Tables[0].Rows.Count; x++)
                    {
                        beforeProductName = twoProductNames.Tables[0].Rows[x]["name"].ToString();
                        if (!string.IsNullOrEmpty(beforeProductName))
                        {
                            if (x == 0)
                            {
                                this.label2.Text = beforeProductName;
                            }
                            else
                            {
                                this.label3.Text = beforeProductName;
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
                                // this.label3.Visible = this.Label4.Visible = true;
                                this.label3.Text = onlyOne;
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
                            // this.label3.Visible = this.Label4.Visible = true;
                            this.label3.Text = onlyOne;
                        }
                    }
                }
            }
        }

        public bool CheckMachineNumber(String dockNumber, bool flag)
        {
            if (!LoginUser.MachineNumber.Equals(label1.Text))
            {
                String errorinfo = String.Format("机台：{0}与当前交班数据绑定机台号：{1}不一致,请重新登录！", label1.Text, LoginUser.MachineNumber);
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert( '" + errorinfo + "');</script>");
                return false;
            }
            if (flag)
            {
                String CheckDock = String.Empty;

                #region Old
                //switch (MachineNumber.Substring(0, 4))
                //{
                //case "0218":
                //    CheckDock = "J";
                //    break;
                //case "0219":
                //    CheckDock = "M";
                //    break;
                //case "0220":
                //    CheckDock = "C";
                //    break;
                //case "0221":
                //    CheckDock = "F";
                //    break;
                //}
                #endregion

                #region New 通用版 （Web.config 文件中配置节点 CheckMachineNumberSet）
                //格式：截取机台编码长度 + | + 匹配1 + 冒号 + 值1 + | + 匹配2 + 冒号 + 值2 + | ...
                //例：  4|0218:J|0219:M|0220:C|0221:F
                //注：  标点符号为英文半角，无空格，最后无需分割符“|”。
                string value = Properties.Settings.Default.CheckMachineNumberSet;
                try
                {
                    if (string.IsNullOrEmpty(value)) { throw new Exception(); }
                    string[] config = value.Split('|');
                    int length = Convert.ToInt32(config[0]);
                    string code = LoginUser.MachineNumber.Substring(0, length);
                    int index = 1;
                    while (index < config.Length)
                    {
                        string[] checks = config[index].Split(':');
                        if (code.Equals(checks[0]))
                        {
                            CheckDock = checks[1];
                            break;
                        }
                        index++;
                    }
                }
                catch (Exception)
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert( 'Web.config文件配置有误！');</script>");
                    return false;
                }
                #endregion

                if (!CheckDock.Equals(dockNumber.Substring(0, 1)))
                {
                    String errorinfo = String.Format("机台：{0},绑定站台：{1},当前机台交班数据绑定站台信息有误,请重新登录！", LoginUser.MachineNumber, dockNumber);
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert( '" + errorinfo + "');</script>");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 需要交接人工领料多选框选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ckbSelect_CheckedChanged(object sender, EventArgs e)
        {
            GetAndBindData(ckbSelect.Checked);
        }

        //修改交班数据
        protected void btn_Modify_Click(object sender, EventArgs e)
        {
            JsonObject jo = new JsonObject();
            JsonArray details = new JsonArray();

            bool isAllZero = true;
            foreach (DataRow row in initData1.Rows)
            {
                decimal qty = decimal.Parse(row["qty"].ToString());

                if (qty > 0)
                {
                    isAllZero = false;
                    decimal planQty = Convert.ToDecimal(row["planQty"]);

                    if (qty > planQty)
                    {
                        ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('“" + row["itemName"] + "”，修改的物料数量不能超出标准数量 " + planQty + " , 请重新输入!');</script>");
                        return;
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('“" + row["itemName"] + "”，修改的物料数量不能为负数, 请重新输入!');</script>");
                    return;
                }

                JsonObject obj = new JsonObject();
                obj.Add("containerDetailId", row["containerDetailId"].ToString());//容器明细id
                obj.Add("itemId", row["itemId"].ToString());//物料id
                obj.Add("itemName", row["itemName"].ToString());//物料名称
                obj.Add("planQty", row["planQty"].ToString());//要料时物料的数量
                obj.Add("qty", qty);//修改的物料数量
                obj.Add("lotNo", row["lotNo"].ToString());//批次
                obj.Add("ownerId", row["ownerId"].ToString());//供应商
                //obj.Add("isChangePersonReceive", ckbSelect.Checked);//是否交接人工领料数据（true:交；false:不交）
                details.Add(obj);
            }

            jo.Add("ItemList", details);

            ReturnPalletBLL bll = new ReturnPalletBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
            string msg = string.Empty;
            try
            {
                //msg = bll.ClientModifyReturnBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.SchemeId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, jo.ToString(), LoginUser.HasLGV.ToString(), LoginUser.Token);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            if (string.IsNullOrEmpty(msg))
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('数据更新成功。');</script>");
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('修改失败！" + msg + "');</script>");
            }
        }
    }
}