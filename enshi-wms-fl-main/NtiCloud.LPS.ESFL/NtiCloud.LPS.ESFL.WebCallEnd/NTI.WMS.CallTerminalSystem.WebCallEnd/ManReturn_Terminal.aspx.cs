using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NTI.WMS.CallTerminalSystem.WebCallEnd.Common;
using System.Data;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Collections;
using WMSInterface.Entity50;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class ManReturn_Terminal : System.Web.UI.Page
    {
        string UserName, UserPassword;
        string WareHouseID, ItemName, ItemCode;
        string Equipment, WorkShop, Run, MachineNumber, RunsGroup, MachineName, hasLGV, EquipmentNumber, RunsTypeCode;
        private static int count_Model;
        private static string operateName = string.Empty;
        private static IList dockList;
        private static DateTime lastTime = new DateTime(2009, 06, 18);
        private static Hashtable ButtonTable;
        private static Hashtable TextTable;
        private static Dictionary<string, string> LBdictionary = new Dictionary<string, string>();
        private static Dictionary<string, DockInfo> TBdictionary;
        private static Dictionary<string, Command> RBdictionary = new Dictionary<string, Command>();
        private static Command m_Command;
        private Command_GetData cg;
        private CommonBLL cb = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSAOUR");
            if (Session["productitemname"] == null || Session["WorkShop"] == null || Session["Equipment"] == null || Session["UserName"] == null || Session["WarehouseID"] == null || Session["RunsGroup"] == null || Session["MachineNumber"] == null)
            {
                Response.Redirect("pass.aspx");
            }
            else
            {
                EquipmentNumber = Session["Equipment"].ToString();
                UserName = Session["UserName"].ToString();
                //UserPassword = Session["PassWord"].ToString();
                WareHouseID = Session["WarehouseID"].ToString();
                WorkShop = Session["WorkShop"].ToString(); //车间 
                Run = Session["Runs"].ToString(); //班组 

                if (Session["MachineNumber"] != null)
                {
                    MachineNumber = Session["MachineNumber"].ToString();
                }
                RunsGroup = Session["RunsGroup"].ToString();

                Response.Expires = -1;

                //MachineName = LoginUser.MachineName;
                if (Session["MachineName"] != null)
                {
                    MachineName = Session["MachineName"].ToString();

                }
                if (Session["productitemname"] != null)
                {
                    LoginUser.ItemName = Session["productitemname"].ToString();
                    ItemName = Session["productitemname"].ToString();
                }
                hasLGV = Session["HasLGV"].ToString();
                //Session["HasLGV"] = "1";
                //hasLGV = "1";
                //LoginUser.HasLGV = 1;

                if (!Page.IsPostBack)
                {
                    TBdictionary = new Dictionary<string, DockInfo>();
                    ButtonTableLoad();
                    InitRadioButton();
                    SetButtonEnabledToFalse();
                    if (!string.IsNullOrEmpty(MachineNumber))
                    {
                        ReFreshMachineNumber();
                    }

                    lb_MachineNumber.Text = (MachineNumber == null) ? "未设置" : MachineNumber;
                    lb_MachineName.Text = (MachineName == null) ? "未设置" : MachineName;
                    txtblock_UserName.Text = UserName;
                    //当前班组  peq 20110506
                    lb_ProductName.Text = (ItemName == String.Empty) ? "未设置" : ItemName;
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
                            break; ;
                        case "4":
                            RunsGroup = "丁班";
                            break;
                    }
                    LoginUser.RunsCode = Run;
                    RunsTypeCode = RunsGroup + "-" + Run;
                    LoginUser.RunsTypeCode = RunsGroup + "-" + Run;
                    Label3.Text = Run;
                    if (rb_ManPowerReturnMaterials.Checked)
                    {
                        radioButton_Click(rb_ManPowerReturnMaterials, e);
                    }
                    //将卷接包车间退空托盘+要料禁用1029
                    ItemDetailBind();
                }
            }
        }
        /// <summary>
        /// 将Button加载至ButtonTable
        /// </summary>
        private void ButtonTableLoad()
        {
            ButtonTable = new Hashtable();
            if (ButtonTable != null)
            {
                ButtonTable.Clear();
            }
            TextTable = new Hashtable();
            if (TextTable != null)
            {
                TextTable.Clear();
            }

            ButtonTable.Add("button1", btn_Platform1);
            ButtonTable.Add("button2", btn_Platform2);
            ButtonTable.Add("button3", btn_Platform3);
            ButtonTable.Add("button4", btn_Platform4);
            ButtonTable.Add("button5", btn_Platform5);
            ButtonTable.Add("button6", btn_Platform6);

            TextTable.Add("tb1", textblock_Section1);
            TextTable.Add("tb2", textblock_Section2);
            TextTable.Add("tb3", textblock_Section3);
            TextTable.Add("tb4", textblock_Section4);
            TextTable.Add("tb5", textblock_Section5);
            TextTable.Add("tb6", textblock_Section6);

        }
        /// <summary>
        /// 设定Button为不可用
        /// </summary>
        private void SetButtonEnabledToFalse()
        {
            for (int i = 0; i < ButtonTable.Count; i++)
            {
                Button btn = (Button)ButtonTable["button" + (i + 1).ToString()];
                if (btn_Platform1.ID == btn.ID)
                {
                    btn_Platform1.Enabled = false;
                }
                else if (btn_Platform2.ID == btn.ID)
                {
                    btn_Platform2.Enabled = false;
                }
                else if (btn_Platform3.ID == btn.ID)
                {
                    btn_Platform3.Enabled = false;
                }
                else if (btn_Platform4.ID == btn.ID)
                {
                    btn_Platform4.Enabled = false;
                }
                else if (btn_Platform5.ID == btn.ID)
                {
                    btn_Platform5.Enabled = false;
                }
                else
                {
                    btn_Platform6.Enabled = false;
                }
                //btn.Enabled = false;
                btn.Text = "0" + (i + 1).ToString();
                //TBdictionary.Add(btn.Text, null);
            }
        }

        private void InitRadioButton()
        {
            RBdictionary.Clear();
            Command command = new Command_ReturnNullPallet();//退空托盘
            RBdictionary.Add(rb_ReturnNullPallt.Text, command);
        }
        /// <summary>
        /// 刷新。包括按钮组，listview的重新加载
        /// </summary>
        private void ReFreshMachineNumber()
        {
            InitButtonInfo();
            if (dockList != null)
            {
                if (dockList.Count == 4 && count_Model <= 2)
                {
                    Interface_1();
                    Dock_Distributing(DockState.DOCK_STATUS_OK, dockList);
                }
                else if (dockList.Count < 4 && count_Model <= 2)
                {
                    if (HavingPermissions.EstimatePermissions(LoginUser.tempermission, PermissionString.WorkShop_ReturnBill))
                    {
                        Interface_3();
                        if (dockList.Count == 2 && count_Model == 2)
                        {
                            btn_Platform3.Visible = false;
                            textblock_Section5.Visible = false;
                            textblock_Section4.Visible = false;
                            textblock_Section6.Visible = false;

                            Dock_Distributing_High(DockState.DOCK_STATUS_OK, dockList);
                        }
                        else if (dockList.Count == 2 && count_Model == 1)
                        {
                            textblock_Section2.Visible = false;
                            btn_Platform3.Visible = false;

                            Dock_Distributing_Low(DockState.DOCK_STATUS_OK, dockList);
                        }
                        else if (dockList.Count == 1 && count_Model == 1)
                        {
                            btn_Platform3.Visible = false;
                            textblock_Section5.Visible = false;
                            textblock_Section4.Visible = false;
                            textblock_Section6.Visible = false;

                            Dock_Distributing_High(DockState.DOCK_STATUS_OK, dockList);
                        }
                    }
                }
                else if (dockList.Count == 6 && count_Model == 6)
                {
                    Dock_Distributing_High(DockState.DOCK_STATUS_OK, dockList);
                }
            }
            RefreshListView();
            ItemDetailBind();
        }

        private void Dock_Distributing_High(string status, IList dockList)
        {
            if (dockList == null)
            {
                return;
            }

            for (int index = 0; index < dockList.Count; index++)
            {
                DockInfo dockInfo = (DockInfo)dockList[index];
                string _modelCode = dockInfo.ModelCode;
                string _dockNumber = dockInfo.DockNumber;
                string _dockStatus = dockInfo.DockStatus;
                Button tb;

                string bt_Name = "button" + (index + 1).ToString();
                string textblock_Name = "tb" + (index + 1).ToString();

                Button button = new Button();
                button = (Button)ButtonTable[bt_Name];
                tb = TextTable[textblock_Name] as Button;
                tb.Text = _modelCode;
                //tb.Tag = dockInfo.DockNumber;
                if (LBdictionary.Keys.Contains(_modelCode))
                {
                    LBdictionary.Remove(_modelCode);
                    LBdictionary.Add(_modelCode, dockInfo.DockNumber);
                }
                else
                {
                    LBdictionary.Add(_modelCode, dockInfo.DockNumber);
                }
                SetButtonInfo(button, dockInfo);
                if (_dockStatus == status)
                {
                    string btnID = button.ID;
                    if (this.btn_Platform1.ID.ToString() == btnID)
                    {
                        this.btn_Platform1.Enabled = true;
                    }
                    else if (this.btn_Platform2.ID.ToString() == btnID)
                    {
                        if (dockInfo.DockNumber.StartsWith("C"))
                        {
                            this.btn_Platform2.Enabled = false;
                        }
                        else
                        {
                            this.btn_Platform2.Enabled = true;
                        }
                    }
                    else if (this.btn_Platform3.ID.ToString() == btnID)
                    {
                        this.btn_Platform3.Enabled = true;
                    }
                    else if (this.btn_Platform4.ID.ToString() == btnID)
                    {
                        this.btn_Platform4.Enabled = true;
                    }
                    else if (this.btn_Platform5.ID.ToString() == btnID)
                    {
                        this.btn_Platform5.Enabled = true;
                    }
                    else if (this.btn_Platform6.ID.ToString() == btnID)
                    {
                        this.btn_Platform6.Enabled = true;
                    }
                }
            }
        }

        private void Dock_Distributing_Low(string status, IList dockList)
        {
            if (dockList == null)
            {
                return;
            }
            for (int index = 0; index < dockList.Count; index++)
            {
                DockInfo dockInfo = (DockInfo)dockList[index];
                string _modelCode = dockInfo.ModelCode;
                string _dockNumber = dockInfo.DockNumber;
                string _dockStatus = dockInfo.DockStatus;
                Button tb;

                string bt_Name = "button" + (index + 1).ToString();
                string textblock_Name = "tb" + (index + 1).ToString();

                Button button = new Button();
                button = (Button)ButtonTable[bt_Name];
                //if (index == 1)
                //{
                //    tb = textblock_Section4;
                //}
                //else
                //{
                //    tb = TextTable[textblock_Name] as Button;
                //}
                tb = TextTable[textblock_Name] as Button;
                tb.Text = _modelCode;
                //tb.Tag = dockInfo.DockNumber;
                if (LBdictionary.Keys.Contains(_modelCode))
                {
                    LBdictionary.Remove(_modelCode);
                    LBdictionary.Add(_modelCode, dockInfo.DockNumber);
                }
                else
                {
                    LBdictionary.Add(_modelCode, dockInfo.DockNumber);
                }
                SetButtonInfo(button, dockInfo);
                if (_dockStatus == status)
                {
                    string btnID = button.ID;
                    if (this.btn_Platform1.ID.ToString() == btnID)
                    {
                        this.btn_Platform1.Enabled = true;
                    }
                    else if (this.btn_Platform2.ID.ToString() == btnID)
                    {
                        if (dockInfo.DockNumber.StartsWith("C"))
                        {
                            this.btn_Platform2.Enabled = false;
                        }
                        else
                        {
                            this.btn_Platform2.Enabled = true;
                        }
                    }
                    else if (this.btn_Platform3.ID.ToString() == btnID)
                    {
                        this.btn_Platform3.Enabled = true;
                    }
                    else if (this.btn_Platform4.ID.ToString() == btnID)
                    {
                        this.btn_Platform4.Enabled = true;
                    }
                    else if (this.btn_Platform5.ID.ToString() == btnID)
                    {
                        this.btn_Platform5.Enabled = true;
                    }
                    else if (this.btn_Platform6.ID.ToString() == btnID)
                    {
                        this.btn_Platform6.Enabled = true;
                    }
                    //button.Enabled = true;
                }
            }
        }
        /// <summary>
        /// 获取本机台站台位,并对站台位进行过滤(去掉人工站台,退残烟,退废料站台,以及一区段多于二个站台的其他站台)
        /// </summary>
        /// <param name="status"></param>
        private void InitButtonInfo()
        {
            int flag = 0;
            string _modelCode_temp = null;
            cg = new Command_GetData();
            this.SetButtonEnabledToFalse();
            dockList = cg.GetManMachineDockInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, MachineNumber);
            count_Model = 0;
            string showlabeltext = string.Empty;
            string showdockname = string.Empty;
            string showmodelcode = string.Empty;

            if (dockList == null || dockList.Count <= 0)
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台还没有设置站台位!');</script>");
                return;
            }
            else
            {
                for (int i = 0; i < dockList.Count; i++)
                {
                    DockInfo dock = (DockInfo)dockList[i];

                    if (dock.ModelCode == Properties.Settings.Default.ContainerTypeCode_Handle || dock.ModelCode == Properties.Settings.Default.ContainerTypeCode_Reject || dock.ModelCode == Properties.Settings.Default.ContainerTypeCode_Waste)
                    {
                        dockList.Remove(dock);
                        i--;
                    }
                    else if (dock.ModelCode != null && dock.ModelCode.Equals(_modelCode_temp))
                    {
                        flag++;
                        if (flag > 1)
                        {
                            dockList.Remove(dock);
                            i--;
                        }
                    }
                    else
                    {
                        count_Model++;
                        flag = 0;
                        _modelCode_temp = dock.ModelCode;//
                    }
                }
            }

            DataTable showds = cb.GetMachineDockInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, MachineNumber);
            if (showds != null && showds.Rows.Count > 0 && operateName.Equals("人工退料"))
            {
                showds.DefaultView.RowFilter = "ItemSource=0";
                showds.DefaultView.Sort = "dockname asc";
                DataTable showdt = showds.DefaultView.ToTable();
                for (int x = 0; x < showdt.Rows.Count; x++)
                {
                    showdockname = showdt.Rows[x]["dockname"].ToString();
                    showmodelcode = showdt.Rows[x]["modelcode"].ToString();
                    showlabeltext += "  " + showdockname + ":" + showmodelcode + ";  ";
                }
            }
            if (!string.IsNullOrEmpty(showlabeltext))
            {
                this.Label4.Text = showlabeltext;
            }
            else
            {
                this.Label4.Text = string.Empty;
            }
        }

        #region RefreshListView
        /// <summary>
        /// 刷新listview
        /// </summary>
        private void RefreshListView()
        {
            DataTable dtTasks = cb.GetMachineTaskByMachineNumber(LoginUser.EquipmentNumber, UserName, LoginUser.Password, MachineNumber, null);
            if (dtTasks == null)
            {
                lv_ClientCallTask.DataSource = null;
                lv_ClientCallTask.DataBind();
                return;
            }
            else
            {
                DataTable dt = dtTasks;
                dt.DefaultView.RowFilter = "Itemsource=0";
                lv_ClientCallTask.DataSource = dt.DefaultView;
                lv_ClientCallTask.DataBind();
            }
        }
        private void ItemDetailBind()
        {
            DataTable dtTask = cb.GetMachineCurTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey,
               LoginUser.WarehouseID, EquipmentNumber);
            if (dtTask == null)
            {
                ItemDetail.DataSource = null;
                ItemDetail.DataBind();
            }
            else
            {
                if (dtTask.Rows.Count > 0)
                {
                    dtTask.DefaultView.RowFilter = "Itemsource = 1 and qty > 0";
                    ItemDetail.DataSource = dtTask.DefaultView;
                    ItemDetail.DataBind();
                }
            }
        }
        #endregion

        /// <summary>
        /// 将界面按钮与站台,区段绑定
        /// </summary>
        /// <param name="status"></param>
        /// <param name="dockList"></param>
        private void Dock_Distributing(string status, System.Collections.IList dockList)
        {
            if (dockList == null)
            {
                return;
            }
            for (int index = 0, i = 0; i < dockList.Count && i < 6 && index < 6; i++)
            {
                DockInfo dockInfo = (DockInfo)dockList[i];
                string _modelCode = dockInfo.ModelCode;
                string _dockNumber = dockInfo.DockNumber;
                string _dockStatus = dockInfo.DockStatus;//Equipment_Using

                if (index % 2 == 1)
                {
                    string temp_Name = "tb" + ((index + 2) / 2).ToString();
                    Button temp_TB = TextTable[temp_Name] as Button;

                    if (_modelCode != null && !_modelCode.Equals(temp_TB.Text))
                    {
                        index++;
                        if (index > 5)
                        {
                            break;
                        }
                    }
                }

                string bt_Name = "button" + (index + 1).ToString();
                string textblock_Name = "tb" + ((index + 2) / 2).ToString();

                Button button = new Button();
                button = (Button)ButtonTable[bt_Name];
                Button tb = TextTable[textblock_Name] as Button;
                tb.Text = _modelCode;
                //tb.Text = _dockNumber;
                if (LBdictionary.Keys.Contains(tb.Text))
                {
                    LBdictionary.Remove(tb.Text);
                    LBdictionary.Add(tb.Text, dockInfo.DockNumber);
                }
                else
                {
                    LBdictionary.Add(tb.Text, dockInfo.DockNumber);
                }
                if (_dockStatus == status)
                {
                    string btnID = button.ID;
                    if (this.btn_Platform1.ID.ToString() == btnID)
                    {
                        this.btn_Platform1.Enabled = true;
                    }
                    else if (this.btn_Platform2.ID.ToString() == btnID)
                    {
                        if (dockInfo.DockNumber.StartsWith("C"))
                        {
                            this.btn_Platform2.Enabled = false;
                        }
                        else
                        {
                            this.btn_Platform2.Enabled = true;
                        }
                    }
                    else if (this.btn_Platform3.ID.ToString() == btnID)
                    {
                        this.btn_Platform3.Enabled = true;
                    }
                    else if (this.btn_Platform4.ID.ToString() == btnID)
                    {
                        this.btn_Platform4.Enabled = true;
                    }
                    else if (this.btn_Platform5.ID.ToString() == btnID)
                    {
                        this.btn_Platform5.Enabled = true;
                    }
                    else if (this.btn_Platform6.ID.ToString() == btnID)
                    {
                        this.btn_Platform6.Enabled = true;
                    }
                }
                SetButtonInfo(button, dockInfo);

                index++;
            }
        }

        private void SetButtonInfo(Button button, DockInfo info)
        {
            //button.Tag = info;
            string _DockNumber = info.DockNumber.Replace("编号", "");
            //button.Text = info.DockName;
            button.Text = _DockNumber;
            string dockId = info.MachineDockID;
            if (TBdictionary.Keys.Contains(button.Text))
            {
                TBdictionary.Remove(button.Text);
                TBdictionary.Add(button.Text, info);
            }
            else
            {
                TBdictionary.Add(_DockNumber, info);
            }
        }

        protected void radioButton_Click(object sender, EventArgs e)
        {
            try
            {
                RadioButton rdb = sender as RadioButton;
                m_Command = (Command)RBdictionary[rdb.Text];
                operateName = (string)rdb.Text;

                switch (operateName)
                {
                    case RadioButtonOperator.ManPowerReturnMaterials: InitButtonInfo();
                        //Dock_Distributing(DockState.DOCK_STATUS_USING, dockList);
                        Dock_Distributing_High(DockState.DOCK_STATUS_USING, dockList);
                        break;
                    case RadioButtonOperator.ReturnNullPallet: InitButtonInfo();
                        //Dock_Distributing(DockState.DOCK_STATUS_USING, dockList);
                        Dock_Distributing_High(DockState.DOCK_STATUS_USING, dockList);
                        break;
                    case RadioButtonOperator.ManPowerCallMaterials: InitButtonInfo();
                        Dock_Distributing_High(DockState.DOCK_STATUS_OK, dockList);
                        break;
                }
            }
            catch { }
        }

        DockInfo info;

        protected void button_Click(object sender, EventArgs e)
        {
            LoginUser.UserName = UserName;
            try
            {
                //去掉提示空格  modified by pengenqi 2012/6/12
                operateName = operateName.Replace(" ", "");
                // if (m_Command == null) return;
                Button button = sender as Button;
                if (button.Enabled == false || button.Text == "")
                {
                    return;
                }
                else
                {
                    info = (DockInfo)TBdictionary[button.Text];
                    //m_Command.Info = info;
                    Session["DockNumber"] = info.DockNumber;
                    LoginUser.DockNumber = info.DockNumber;
                    string modelCode = info.ModelCode;
                    #region 人工领料
                    if (this.rb_ManPowerCallMaterials.Checked == true)
                    {
                        string machineCX = info.MachineNumber;
                        Session["rb_ManPowerCallMaterials"] = false;
                        Response.Redirect("ManPowerCallMaterials.aspx");
                    }
                    #endregion
                    #region 人工退料
                    if (this.rb_ManPowerReturnMaterials.Checked == true)
                    {
                        string machineCX = info.MachineNumber;
                        Session["rb_ManPowerReturnMaterials"] = false;
                        Response.Redirect("ManPowerReturnMaterials.aspx");
                    }
                    #endregion
                    #region 退空托盘
                    else if (rb_ReturnNullPallt.Checked == true)
                    {

                        if (true)
                        {
                            ReturnPalletBLL rpb = new ReturnPalletBLL(EquipmentNumber, UserName, UserPassword, WareHouseID);
                            string machineCX = info.MachineNumber;
                            bool flag = false;
                            string tasknumber = rpb.CreateManPowerReturnNullPallet(EquipmentNumber, UserName, UserPassword, WareHouseID, MachineNumber, info.DockNumber);
                            if (!String.IsNullOrEmpty(tasknumber))
                            {
                                flag = true;
                            }
                            button.Enabled = flag ? false : true;
                            if (flag)
                            {
                                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('" + operateName + "操作执行成功！');</script>");
                                if (operateName == RadioButtonOperator.CallPallet)
                                {
                                    InitButtonInfo();
                                    Dock_Distributing_High(DockState.DOCK_STATUS_OK, dockList);
                                }
                                RefreshListView();
                                ItemDetailBind();
                            }
                            else
                            {
                                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('" + operateName + "操作执行失败！');</script>");
                            }
                        }
                    }
                    #endregion
                }
            }
            catch { }
        }

        private void OpenDialogForButton(Button button, string message)
        {
            ScriptManager.RegisterClientScriptBlock(
             button,
             typeof(Button),
             DateTime.Now.ToString().Replace(":", " "), ///使用当前时间作为标识
             "alert('" + message + "')",
             true);
        }

        /// <summary>
        /// 弹出控制父窗口的模态窗口
        /// </summary>
        /// <param name="pagePath">页面路径</param>
        /// <param name="winWidth">窗口宽度</param>
        /// <param name="winHeight">窗口高度</param>
        /// <param name="frameshref">控制父窗口路径</param>
        public void openModelWin(string pagepath, int width, int height, string frameshref)
        {
            string temp = @"<script language='javascript'>";
            temp += (@"   var sPath ='" + pagepath + "';");
            temp += (@"         strFeatures ='dialogWidth=" + width + "px;dialogHeight=" + height + "px;center=yes;resizable=yes;scroll=yes;help=no;status=no';");
            temp += (@"         var returnvalue = window.showModalDialog(sPath,'brower',strFeatures);");
            temp += (@"         window.location.href = this.location;");
            temp += (@"</script>");
            ClientScript.RegisterStartupScript(this.GetType(), "message", temp);
        }

        protected void btn_Exit_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        protected void btn_Returns_Click(object sender, EventArgs e)
        {
            Response.Redirect("Control_Panel.aspx");
        }

        /// <summary>
        /// 界面1,四个按钮,两个区段
        /// </summary>
        private void Interface_1()
        {
            btn_Platform5.Visible = false;
            btn_Platform6.Visible = false;
            textblock_Section3.Visible = false;
        }
        /// <summary>
        /// 界面2,二种操作类型,两个(或一个)按钮,两个区段
        /// </summary>
        private void Interface_2()
        {
            btn_Platform3.Visible = false;
            btn_Platform4.Visible = false;
            btn_Platform5.Visible = false;
            btn_Platform6.Visible = false;
            textblock_Section2.Visible = false;
            textblock_Section3.Visible = false;
            textblock_Section4.Visible = false;
        }
        /// <summary>
        /// 界面3,五种操作类型,两个站台,两个区段
        /// </summary>
        private void Interface_3()
        {
            //btn_Platform2.Visible = false;
            btn_Platform4.Visible = false;
            btn_Platform5.Visible = false;
            btn_Platform6.Visible = false;
            textblock_Section3.Visible = false;
            textblock_Section4.Visible = true;
        }

        protected void btn_Refresh_Click(object sender, EventArgs e)
        {
            DateTime nowTime = DateTime.Now;
            TimeSpan ts1 = new TimeSpan(lastTime.Ticks);
            TimeSpan ts2 = new TimeSpan(nowTime.Ticks);
            TimeSpan ts = ts2.Subtract(ts1);
            if (ts.Days > 0 || ts.Hours > 0 || ts.Minutes > 0 || ts.Seconds > 5)
            {
                lastTime = nowTime;
                RefreshListView();
                ItemDetailBind();
                RefreshProductName();
                //刷新界面按钮状态   
                Page_Load(sender, e);
            }
            else
            {
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('对不起,您的刷新请求不能过频繁!');</script>");
            }
        }
        private void RefreshProductName()
        {
            GetData.GetMachineProductPlan(cb, MachineNumber);

            //lb_ProductName.Content = (LoginUser.ItemName == null) ? "未设置" : LoginUser.ItemName+ "  班次:" + ClassGroup
            try
            {
                string ClassGroup = XmlReader.GetClassGroup();
                lb_ProductName.Text = (ItemName == null) ? "未设置" : ItemName;
            }
            catch { }
        }

        protected void textblock_click(object sender, EventArgs e)
        {
            Button tb = sender as Button;
            //Dock_Materials dm = new Dock_Materials();
            if (tb.Text == "")
            {
                return;
            }
            else
            {
                string modelCode_dockNumber = LBdictionary[tb.Text].ToString();
                string pagepath = "Dock_Materials.aspx?dockNumber=" + modelCode_dockNumber;
                openModelWin(pagepath, 500, 527);
            }
        }

        public void openModelWin(string pagePath, int winWidth, int winHeight)
        {
            string temp;
            temp = @"<script language='javascript'>";
            temp += (@"   var sPath ='" + pagePath + "';");
            temp += (@"         strFeatures ='dialogWidth=" + winWidth + "px;dialogHeight=" + winHeight + "px;center=yes;scroll=yes;help=no;status: yes';");//No
            temp += (@"         var returnvalue = window.open(sPath,'click',strFeatures);");
            temp += (@"</script>");
            ClientScript.RegisterStartupScript(this.GetType(), "message", temp);//newwindow click  showModalDialog
        }

        protected void btn_Switch_Click(object sender, EventArgs e)
        {
            Session["IsToCall"] = "tocall";
            Response.Redirect("Set_Number.aspx");
        }

        protected void GridViewReturnBill_ToNext_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            lv_ClientCallTask.PageIndex = e.NewPageIndex;
            ReFreshMachineNumber();
        }

        protected void rb_ManPowerReturnMaterials_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ManPowerReturnMaterials.Checked == true)
            {
                operateName = (string)rb_ManPowerReturnMaterials.Text;
                InitButtonInfo();
                Dock_Distributing_High(DockState.DOCK_STATUS_USING, dockList);
                //this.SetButtonEnabledToTrue();
                ItemDetailBind();
            }
        }

        private void SetButtonEnabledToTrue()
        {
            for (int i = 0; i < ButtonTable.Count; i++)
            {
                Button btn = (Button)ButtonTable["button" + (i + 1).ToString()];
                if (btn_Platform1.ID == btn.ID)
                {
                    btn_Platform1.Enabled = true;
                }
                else if (btn_Platform2.ID == btn.ID)
                {
                    btn_Platform2.Enabled = true;
                }
                else if (btn_Platform3.ID == btn.ID)
                {
                    btn_Platform3.Enabled = true;
                }
                else if (btn_Platform4.ID == btn.ID)
                {
                    btn_Platform4.Enabled = true;
                }
                else if (btn_Platform5.ID == btn.ID)
                {
                    btn_Platform5.Enabled = true;
                }
                else
                {
                    btn_Platform6.Enabled = true;
                }
                //btn.Enabled = false;
                btn.Text = "0" + (i + 1).ToString();
                //TBdictionary.Add(btn.Text, null);
            }
        }

        protected void btn_PalletType_Click(object sender, EventArgs e)
        {
            Response.Redirect("PalletType.aspx");
        }
        protected void ItemDetail_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            ItemDetail.PageIndex = e.NewSelectedIndex;
            ReFreshMachineNumber();
        }

        protected void rb_ManPowerCallMaterials_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ManPowerCallMaterials.Checked == true)
            {
                operateName = (string)rb_ManPowerCallMaterials.Text;
                InitButtonInfo();
                Dock_Distributing_High(DockState.DOCK_STATUS_OK, dockList);
                ItemDetailBind();
            }
        }
    }
}