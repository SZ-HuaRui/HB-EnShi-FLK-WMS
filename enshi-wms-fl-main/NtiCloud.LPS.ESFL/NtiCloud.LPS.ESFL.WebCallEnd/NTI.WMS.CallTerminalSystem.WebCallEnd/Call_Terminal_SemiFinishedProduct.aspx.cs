using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using NTI.WMS.CallTerminalSystem.BLL;
using System.Data;
using NTI.WMS.CallTerminalSystem.WebCallEnd.Common;
using Newtonsoft.Json;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public partial class Call_Terminal_SemiFinishedProduct : System.Web.UI.Page
    {
        string UserName, WareHouseID, ItemName, MachineNumber, MachineName, hasLGV, EquipmentNumber;
        private static string operateName = "要料";//"要料";
        private static IList dockList;
        private static DateTime lastTime = new DateTime(2009, 06, 18);
        private static DataTable dtTask = new DataTable();
        private static Hashtable ButtonTable;
        private static Hashtable TextTable;
        /// <summary>
        /// 键：站台code(值为设备表中的字段equipment_code的值)   值：区段code
        /// </summary>
        private static Dictionary<string, string> LBdictionary = new Dictionary<string, string>();
        private static Dictionary<string, DockInfo> TBdictionary;
        private static Dictionary<string, Command> RBdictionary = new Dictionary<string, Command>();
        private static Command m_Command;
        private Command_GetData cg;
        private CommonBLL cb = new CommonBLL(LoginUser.EquipmentNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);

        //加载
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSAOUR");
            if (Session["MachineNumber"] == null)
            {
                Response.Redirect("Call_Terminal_Log.aspx");
                return;
            }
            else if (Session["RunsGroup"] == null)
            {
                Response.Redirect("Set_Number.aspx");
                return;
            }
            else
            {
                ////查询是否已经做交班
                //ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
                ////获取机台未完成的交接单信息
                //DataTable change_Class_Task = rbob.GetNoFinishMachineReturnMaterialBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.Token);
                //if (change_Class_Task == null)
                //{
                //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('连接服务器失败！');</script>");
                //    return;
                //}
                //if (change_Class_Task != null && change_Class_Task.Rows.Count > 0)
                //{
                //    //ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前班已经交班，无法进行操作！');window.location='Control_Panel.aspx';</script>");
                //    Response.Redirect("Control_Panel.aspx");
                //    return;
                //}

                ////EquipmentNumber = Session["Equipment"].ToString();
                //UserName = Session["UserName"].ToString();
                ////UserPassword = Session["PassWord"].ToString();
                //WareHouseID = LoginUser.WarehouseID;
                ////WorkShop = Session["WorkShop"].ToString(); //车间 
                //Run = Session["RunsGroup"].ToString(); //班组 

                //if (Session["MachineNumber"] != null)
                //{
                //    MachineNumber = Session["MachineNumber"].ToString();
                //}
                //RunsGroup = Session["RunsGroup"].ToString();

                //Response.Expires = -1;

                ////MachineName = LoginUser.MachineName;
                //if (Session["MachineName"] != null)
                //{
                //    MachineName = Session["MachineName"].ToString();

                //}
                ////if (Session["productitemname"] != null)
                ////{
                ////    LoginUser.ItemName = Session["productitemname"].ToString();
                ////    ItemName = Session["productitemname"].ToString();
                ////}

                ////  }
                //ItemName = LoginUser.ItemName;
                //if (Session["HasLGV"] == null) Session["HasLGV"] = "1";
                //hasLGV = Session["HasLGV"].ToString();
                //LoginUser.HasLGV = Convert.ToInt32(hasLGV);
                //txtHavLGV.Text = hasLGV.ToString();

                ////rb_ReturnNull_Call.Visible = false;
                //this.rb_ReturnRemnantsMaterials.Visible = true;//= this.btn_Switch.Visible = true;
                if (!Page.IsPostBack)
                {
                    ////查询是否已经做交班
                    //ReturnBillOperatorBLL rbob = new ReturnBillOperatorBLL(LoginUser.MachineNumber, LoginUser.UserName, LoginUser.Password, LoginUser.WarehouseID);
                    ////获取机台未完成的交接单信息
                    //DataTable change_Class_Task = rbob.GetNoFinishMachineReturnMaterialBill(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.Token);
                    //if (change_Class_Task == null)
                    //{
                    //    string url = "Call_Terminal_Log.aspx";
                    //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('连接服务器失败,请重新登录！');window.location='" + url + "';</script>");
                    //    return;
                    //}
                    //if (change_Class_Task != null && change_Class_Task.Rows.Count > 0)
                    //{
                    //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('存在交班记录，无法进行车间操作！');</script>");
                    //    Response.Redirect("Control_Panel.aspx");
                    //    return;
                    //}

                    if (Session["MachineNumber"] != null)
                    {
                        MachineNumber = Session["MachineNumber"].ToString();
                    }

                    Response.Expires = -1;

                    if (Session["MachineName"] != null)
                    {
                        MachineName = Session["MachineName"].ToString();

                    }
                    ItemName = LoginUser.ItemName;
                    if (Session["HasLGV"] == null) Session["HasLGV"] = "1";
                    hasLGV = Session["HasLGV"].ToString();
                    LoginUser.HasLGV = Convert.ToInt32(hasLGV);
                    txtHavLGV.Text = hasLGV.ToString();

                    //rb_ReturnNull_Call.Visible = false;
                    //this.rb_ReturnRemnantsMaterials.Visible = true;//= this.btn_Switch.Visible = true;
                    TBdictionary = new Dictionary<string, DockInfo>();
                    ButtonTableLoad();
                    InitRadioButton();
                    SetButtonVisibleToFalse();
                    if (!string.IsNullOrEmpty(MachineNumber))
                    {
                        ReFreshMachineNumber();
                    }

                    lb_MachineNumber.Text = (MachineNumber == null) ? "未设置" : MachineNumber;
                    lb_MachineName.Text = (MachineName == null) ? "未设置" : MachineName;
                    txtblock_UserName.Text = LoginUser.UserName;
                    lb_ProductName.Text = (ItemName == String.Empty) ? "未设置" : ItemName;

                    Label3.Text = LoginUser.RunsTypeCode;
                    //if (rb_CallMaterials.Checked == true)
                    //{
                    //    radioButton_Click(rb_CallMaterials, e);
                    //}
                    //rb_CallMaterials.Visible = true;
                    //rb_CallMaterials.Enabled = true;
                    //rb_ReturnNull_Call.Enabled = true;
                    //rb_ReturnNull_Call.Visible = true;
                    //rb_CallMaterials.Enabled = false;
                    //rb_CallMaterials.Visible = false;
                    ////井冈山项目需要屏蔽退空托盘加要料按钮
                    //rb_ReturnNull_Call.Enabled = false;
                    //rb_ReturnNull_Call.Visible = false;

                    //if (LoginUser.MachineNumber.ToLower().Contains("bzr"))
                    //{
                    //    rb_ReturnNull_Call.Enabled = false;
                    //}

                    ItemDetailBind();
                    DateTime dtNow = DateTime.Now;
                    DateTime dtStart = Convert.ToDateTime(dtNow.ToString("yyyy-MM-dd" + " 07:00:00"));
                    DateTime dtCen = Convert.ToDateTime(dtNow.ToString("yyyy-MM-dd" + " 17:00:00"));
                    DateTime dtEnd = Convert.ToDateTime(dtNow.AddDays(1).ToString("yyyy-MM-dd" + " 07:00:00"));
                    if (DateTime.Compare(dtNow, dtStart) >= 0 && DateTime.Compare(dtNow, dtCen) < 0)
                    {
                        Label4.Text = "当前为早班（7点至17点）";
                    }
                    else if (DateTime.Compare(dtNow, dtCen) >= 0 && DateTime.Compare(dtNow, dtEnd) < 0)
                    {
                        Label4.Text = "当前为中班（17点至第二天7点）";
                    }
                    else
                    {
                        Label4.Text = "";
                    }
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

            //区段类型
            TextTable.Add("tb1", textblock_Section1);
            TextTable.Add("tb2", textblock_Section2);
            TextTable.Add("tb3", textblock_Section3);
            TextTable.Add("tb4", textblock_Section4);
            TextTable.Add("tb5", textblock_Section5);
            TextTable.Add("tb6", textblock_Section6);
            TextTable.Add("tb7", textblock_Section7);
            TextTable.Add("tb8", textblock_Section8);
            TextTable.Add("tb9", textblock_Section9);
            TextTable.Add("tb10", textblock_Section10);
            TextTable.Add("tb11", textblock_Section11);
            TextTable.Add("tb12", textblock_Section12);
            TextTable.Add("tb13", textblock_Section13);
            TextTable.Add("tb14", textblock_Section14);

            //站台
            ButtonTable.Add("button1", btn_Platform1);
            ButtonTable.Add("button2", btn_Platform2);
            ButtonTable.Add("button3", btn_Platform3);
            ButtonTable.Add("button4", btn_Platform4);
            ButtonTable.Add("button5", btn_Platform5);
            ButtonTable.Add("button6", btn_Platform6);
            ButtonTable.Add("button7", btn_Platform7);
            ButtonTable.Add("button8", btn_Platform8);
            ButtonTable.Add("button9", btn_Platform9);
            ButtonTable.Add("button10", btn_Platform10);
            ButtonTable.Add("button11", btn_Platform11);
            ButtonTable.Add("button12", btn_Platform12);
            ButtonTable.Add("button13", btn_Platform13);
            ButtonTable.Add("button14", btn_Platform14);
        }

        /// <summary>
        /// 设定Button为不可用
        /// </summary>
        private void SetButtonVisibleToFalse()
        {
            //区段
            for (int i = 1; i <= TextTable.Count; i++)
            {
                Button btn = (Button)TextTable["tb" + i];
                if (btn != null)
                {
                    btn.Visible = false;
                }
            }

            //站台
            for (int i = 0; i < ButtonTable.Count; i++)
            {
                Button btn = (Button)ButtonTable["button" + (i + 1)];
                if (btn == null) continue;
                btn.Visible = false;
                btn.Text = "0" + (i + 1).ToString();

                #region Enabled
                if (btn_Platform1.ID == btn.ID)
                {
                    btn_Platform1.Enabled = false;
                    SetButtonStyle(btn_Platform1);
                }
                else if (btn_Platform2.ID == btn.ID)
                {
                    btn_Platform2.Enabled = false;
                    SetButtonStyle(btn_Platform2);
                }
                else if (btn_Platform3.ID == btn.ID)
                {
                    btn_Platform3.Enabled = false;
                    SetButtonStyle(btn_Platform3);
                }
                else if (btn_Platform4.ID == btn.ID)
                {
                    btn_Platform4.Enabled = false;
                    SetButtonStyle(btn_Platform4);
                }
                else if (btn_Platform5.ID == btn.ID)
                {
                    btn_Platform5.Enabled = false;
                    SetButtonStyle(btn_Platform5);
                }
                else if (btn_Platform6.ID == btn.ID)
                {
                    btn_Platform6.Enabled = false;
                    SetButtonStyle(btn_Platform6);
                }
                else if (btn_Platform7.ID == btn.ID)
                {
                    btn_Platform7.Enabled = false;
                    SetButtonStyle(btn_Platform7);
                }
                else if (btn_Platform8.ID == btn.ID)
                {
                    btn_Platform8.Enabled = false;
                    SetButtonStyle(btn_Platform8);
                }
                else if (btn_Platform9.ID == btn.ID)
                {
                    btn_Platform9.Enabled = false;
                    SetButtonStyle(btn_Platform9);
                }
                else if (btn_Platform10.ID == btn.ID)
                {
                    btn_Platform10.Enabled = false;
                    SetButtonStyle(btn_Platform10);
                }
                else if (btn_Platform11.ID == btn.ID)
                {
                    btn_Platform11.Enabled = false;
                    SetButtonStyle(btn_Platform11);
                }
                else if (btn_Platform12.ID == btn.ID)
                {
                    btn_Platform12.Enabled = false;
                    SetButtonStyle(btn_Platform12);
                }
                else if (btn_Platform13.ID == btn.ID)
                {
                    btn_Platform13.Enabled = false;
                    SetButtonStyle(btn_Platform13);
                }
                else if (btn_Platform14.ID == btn.ID)
                {
                    btn_Platform14.Enabled = false;
                    SetButtonStyle(btn_Platform14);
                }
                #endregion
            }
        }

        /// <summary>
        /// 刷新。包括按钮组，listview的重新加载
        /// </summary>
        private void ReFreshMachineNumber()
        {
            InitButtonInfo();
            if (dockList != null && dockList.Count > 0)
            {
                Dock_Distributing(DockState.DOCK_STATUS_OK, dockList);
            }
            RefreshListView();  //当前任务
            ItemDetailBind();   //站台数据
        }

        /// <summary>
        /// 获取本机台站台位
        /// </summary>
        /// <param name="status"></param>
        private void InitButtonInfo()
        {
            cg = new Command_GetData();
            this.SetButtonVisibleToFalse();

            //获取站台位数据
            dockList = cg.GetMachineDockInfo(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.Token);

            string showlabeltext = string.Empty;
            string showdockname = string.Empty;
            string showmodelcode = string.Empty;

            if (dockList == null || dockList.Count <= 0)
            {
                string url = "Call_Terminal_Log.aspx";//Set_Number_new.aspx
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台没有设置站台,请设置站台后，再重新登录！');window.location='" + url + "';</script>");
                //ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('该机台还没有设置站台位!');</script>");
                return;
            }
            dtTask = cb.GetMachineCurTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.Token);
            List<string> dockNameList = new List<string>();//目的是存储不重复的站台编码
            for (int x = 0; x < dtTask.Rows.Count; x++)
            {
                if (dockNameList.Contains(dtTask.Rows[x]["dock_name"].ToString()))
                {
                    continue;
                }
                dockNameList.Add(dtTask.Rows[x]["dock_name"].ToString());
                showdockname = dtTask.Rows[x]["dock_name"].ToString();
                showmodelcode = dtTask.Rows[x]["section_code"].ToString();
                showlabeltext += "  " + showdockname + ":" + showmodelcode + ";  ";
            }
            if (!string.IsNullOrEmpty(showlabeltext))
            {
                //this.Label4.Text = showlabeltext;
            }
            else
            {
                //this.Label4.Text = string.Empty;
            }
        }

        #region 当前机台任务
        /// <summary>
        /// 刷新机台操作记录
        /// </summary>
        private void RefreshListView()
        {
            if (LoginUser.SchemeId == "")
            {
                lv_ClientCallTask.DataSource = null;
                lv_ClientCallTask.DataBind();
                return;
            }
            //任务包含退废料
            DataTable dtTasks = cb.GetMachineTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.SchemeId, LoginUser.WarehouseID, LoginUser.MachineId, Properties.Settings.Default.TaskTypes, LoginUser.Token,"1");
            if (dtTasks == null || dtTasks.Rows.Count == 0)
            {
                lv_ClientCallTask.DataSource = null;
                lv_ClientCallTask.DataBind();
            }
            else if (dtTasks.Rows.Count > 0)
            {
                //dtTasks.DefaultView.RowFilter = "Itemsource = 1";
                lv_ClientCallTask.DataSource = dtTasks.DefaultView;
                lv_ClientCallTask.DataBind();
            }
        }

        //加载站台物料信息
        private void ItemDetailBind()
        {
            //dtTask = cb.GetMachineCurTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, LoginUser.MachineId, LoginUser.Token);
            //if (dtTask == null || dtTask.Rows.Count <= 0)
            //{
            //    ItemDetail.DataSource = null;
            //    ItemDetail.DataBind();
            //}
            //else if (dtTask.Rows.Count > 0)
            //{
            //    //dtTask.DefaultView.RowFilter = "Itemsource = 1 and qty > 0";
            //    dtTask.DefaultView.RowFilter = "qty > 0";
            //    ItemDetail.DataSource = dtTask.DefaultView;
            //    ItemDetail.DataBind();
            //}
            DateTime dtNow = DateTime.Now;
            string startDatetime = string.Empty;
            string morDateTime = string.Empty;
            string endDatetime = string.Empty;

            //获取当前系统时间并判断是否为服务时间
            TimeSpan nowDt = dtNow.TimeOfDay;
            TimeSpan workStartDT = DateTime.Parse("07:00").TimeOfDay;
            TimeSpan workEndDT = DateTime.Parse("23:59").TimeOfDay;
            TimeSpan nextWorkStartDT = DateTime.Parse("00:00").TimeOfDay;
            if ((nowDt >= workStartDT && nowDt <= workEndDT) )//当前时刻在当天的早晨7点到凌晨23点59分之间
            {
                startDatetime = dtNow.ToString("yyyy/MM/dd" + " 07:00:00");
                morDateTime = dtNow.ToString("yyyy/MM/dd" + " 17:00:00");
                endDatetime = dtNow.AddDays(1).ToString("yyyy/MM/dd" + " 07:00:00");
            }
            else if(nowDt < workStartDT && nowDt >= nextWorkStartDT)//当前时刻在当天的凌晨0点到早晨7点之间
            {
                startDatetime = dtNow.AddDays(-1).ToString("yyyy/MM/dd" + " 07:00:00");
                morDateTime = dtNow.AddDays(-1).ToString("yyyy/MM/dd" + " 17:00:00");
                endDatetime = dtNow.ToString("yyyy/MM/dd" + " 07:00:00");
            }
            else//其他时刻
            {
                startDatetime = dtNow.ToString("yyyy/MM/dd" + " 07:00:00");
                morDateTime = dtNow.ToString("yyyy/MM/dd" + " 17:00:00");
                endDatetime = dtNow.AddDays(1).ToString("yyyy/MM/dd" + " 07:00:00");
            }


            //获取已完成的任务，包括要料、退余料、退坏料
            DataTable dtTasks = cb.GetMachineFinishTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.SchemeId, LoginUser.WarehouseID, LoginUser.MachineId, Properties.Settings.Default.TaskTypes, LoginUser.Token, startDatetime, morDateTime);
            if (dtTasks == null || dtTasks.Rows.Count == 0)
            {
                FinsishTask.DataSource = null;
                FinsishTask.DataBind();
            }
            else if (dtTasks.Rows.Count > 0)
            {
                //dtTasks.DefaultView.RowFilter = "Itemsource = 1";
                FinsishTask.DataSource = dtTasks.DefaultView;
                FinsishTask.DataBind();
            }
            //获取已完成的任务，包括要料、退余料、退坏料
            DataTable _dtTasks = cb.GetMachineFinishTaskByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.SchemeId, LoginUser.WarehouseID, LoginUser.MachineId, Properties.Settings.Default.TaskTypes, LoginUser.Token, morDateTime, endDatetime);
            if (_dtTasks == null || _dtTasks.Rows.Count == 0)
            {
                MorFinsishTask.DataSource = null;
                MorFinsishTask.DataBind();
            }
            else if (_dtTasks.Rows.Count > 0)
            {
                //dtTasks.DefaultView.RowFilter = "Itemsource = 1";
                MorFinsishTask.DataSource = _dtTasks.DefaultView;
                MorFinsishTask.DataBind();
            }
        }
        #endregion

        /// <summary>
        /// 将界面按钮与站台,区段绑定,控制按钮的可用状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="dockList"></param>
        /// <param name="isReturnWasteMaterials">是否为退废料（0：否；1：是）</param>
        private void Dock_Distributing(string status, IList dockList, string isReturnWasteMaterials = "0")
        {
            if (dockList == null)
            {
                return;
            }
            for (int index = 0, i = 0; i < dockList.Count && i < 14 && index < 14; i++)
            {
                DockInfo dockInfo = (DockInfo)dockList[i];
                string _modelCode = dockInfo.ModelCode;
                string _dockNumber = dockInfo.DockNumber;
                string _dockStatus = dockInfo.DockStatus;//Equipment_Using


                string textblock_Name = "tb" + (index + 1).ToString();  //区段
                Button btnModel = TextTable[textblock_Name] as Button;
                btnModel.Text = _modelCode;
                btnModel.Attributes.Add("DataSource", JsonConvert.SerializeObject(dockInfo));
                btnModel.Visible = true;

                string bt_Name = "button" + (index + 1).ToString();     //站台
                Button btnDock = new Button();
                btnDock = (Button)ButtonTable[bt_Name];
                btnDock.Visible = true;

                //if (LBdictionary.Keys.Contains(btnModel.Text))
                //{
                //    LBdictionary.Remove(btnModel.Text);
                //    LBdictionary.Add(btnModel.Text, dockInfo.DockNumber);
                //}
                if (LBdictionary.Keys.Contains(dockInfo.DockNumber))
                {
                    LBdictionary.Remove(dockInfo.DockNumber);
                    LBdictionary.Add(dockInfo.DockNumber, btnModel.Text);
                }
                else
                {
                    LBdictionary.Add(dockInfo.DockNumber, btnModel.Text);
                }

                if (_dockStatus == status && isReturnWasteMaterials == "0")
                {
                    string btnID = btnDock.ID;
                    if (this.btn_Platform1.ID.ToString() == btnID)
                    {
                        this.btn_Platform1.Enabled = true;
                        SetButtonStyle(btn_Platform1);
                    }
                    else if (this.btn_Platform2.ID.ToString() == btnID)
                    {
                        this.btn_Platform2.Enabled = true;
                        SetButtonStyle(btn_Platform2);
                    }
                    else if (this.btn_Platform3.ID.ToString() == btnID)
                    {
                        this.btn_Platform3.Enabled = true;
                        SetButtonStyle(btn_Platform3);
                    }
                    else if (this.btn_Platform4.ID.ToString() == btnID)
                    {
                        this.btn_Platform4.Enabled = true;
                        SetButtonStyle(btn_Platform4);
                    }
                    else if (this.btn_Platform5.ID.ToString() == btnID)
                    {
                        this.btn_Platform5.Enabled = true;
                        SetButtonStyle(btn_Platform5);
                    }
                    else if (this.btn_Platform6.ID.ToString() == btnID)
                    {
                        this.btn_Platform6.Enabled = true;
                        SetButtonStyle(btn_Platform6);
                    }
                    else if (this.btn_Platform7.ID.ToString() == btnID)
                    {
                        this.btn_Platform7.Enabled = true;
                        SetButtonStyle(btn_Platform7);
                    }
                    else if (btn_Platform8.ID.ToString() == btnID)
                    {
                        btn_Platform8.Enabled = true;
                        SetButtonStyle(btn_Platform8);
                    }
                    else if (btn_Platform9.ID.ToString() == btnID)
                    {
                        btn_Platform9.Enabled = true;
                        SetButtonStyle(btn_Platform9);
                    }
                    else if (btn_Platform10.ID.ToString() == btnID)
                    {
                        btn_Platform10.Enabled = true;
                        SetButtonStyle(btn_Platform10);
                    }
                    else if (btn_Platform11.ID.ToString() == btnID)
                    {
                        btn_Platform11.Enabled = true;
                        SetButtonStyle(btn_Platform11);
                    }
                    else if (btn_Platform12.ID.ToString() == btnID)
                    {
                        btn_Platform12.Enabled = true;
                        SetButtonStyle(btn_Platform12);
                    }
                    else if (btn_Platform13.ID.ToString() == btnID)
                    {
                        btn_Platform13.Enabled = true;
                        SetButtonStyle(btn_Platform13);
                    }
                    else if (btn_Platform14.ID.ToString() == btnID)
                    {
                        btn_Platform14.Enabled = true;
                        SetButtonStyle(btn_Platform14);
                    }
                }

                if (isReturnWasteMaterials == "1")
                {
                    string btnID = btnDock.ID;
                    if (this.btn_Platform1.ID.ToString() == btnID)
                    {
                        this.btn_Platform1.Enabled = true;
                        SetButtonStyle(btn_Platform1);
                    }
                    else if (this.btn_Platform2.ID.ToString() == btnID)
                    {
                        this.btn_Platform2.Enabled = true;
                        SetButtonStyle(btn_Platform2);
                    }
                    else if (this.btn_Platform3.ID.ToString() == btnID)
                    {
                        this.btn_Platform3.Enabled = true;
                        SetButtonStyle(btn_Platform3);
                    }
                    else if (this.btn_Platform4.ID.ToString() == btnID)
                    {
                        this.btn_Platform4.Enabled = true;
                        SetButtonStyle(btn_Platform4);
                    }
                    else if (this.btn_Platform5.ID.ToString() == btnID)
                    {
                        this.btn_Platform5.Enabled = true;
                        SetButtonStyle(btn_Platform5);
                    }
                    else if (this.btn_Platform6.ID.ToString() == btnID)
                    {
                        this.btn_Platform6.Enabled = true;
                        SetButtonStyle(btn_Platform6);
                    }
                    else if (this.btn_Platform7.ID.ToString() == btnID)
                    {
                        this.btn_Platform7.Enabled = true;
                        SetButtonStyle(btn_Platform7);
                    }
                    else if (btn_Platform8.ID.ToString() == btnID)
                    {
                        btn_Platform8.Enabled = true;
                        SetButtonStyle(btn_Platform8);
                    }
                    else if (btn_Platform9.ID.ToString() == btnID)
                    {
                        btn_Platform9.Enabled = true;
                        SetButtonStyle(btn_Platform9);
                    }
                    else if (btn_Platform10.ID.ToString() == btnID)
                    {
                        btn_Platform10.Enabled = true;
                        SetButtonStyle(btn_Platform10);
                    }
                    else if (btn_Platform11.ID.ToString() == btnID)
                    {
                        btn_Platform11.Enabled = true;
                        SetButtonStyle(btn_Platform11);
                    }
                    else if (btn_Platform12.ID.ToString() == btnID)
                    {
                        btn_Platform12.Enabled = true;
                        SetButtonStyle(btn_Platform12);
                    }
                    else if (btn_Platform13.ID.ToString() == btnID)
                    {
                        btn_Platform13.Enabled = true;
                        SetButtonStyle(btn_Platform13);
                    }
                    else if (btn_Platform14.ID.ToString() == btnID)
                    {
                        btn_Platform14.Enabled = true;
                        SetButtonStyle(btn_Platform14);
                    }
                }

                SetButtonInfo(btnDock, dockInfo);

                index++;

                //string bt_Name = "button" + (index + 1).ToString();  //站台
                //string textblock_Name = "tb" + (index + 1).ToString();  //区段

                //Button button = new Button();
                //button = (Button)ButtonTable[bt_Name];
                //button.Visible = true;

                //Button tb = TextTable[textblock_Name] as Button;
                //tb.Visible = true;
                //tb.Text = _modelCode;
                ////tb.Text = _dockNumber;
                //if (LBdictionary.Keys.Contains(tb.Text))
                //{
                //    LBdictionary.Remove(tb.Text);
                //    LBdictionary.Add(tb.Text, dockInfo.DockNumber);
                //}
                //else
                //{
                //    LBdictionary.Add(tb.Text, dockInfo.DockNumber);
                //}
                //if (_dockStatus == status)
                //{
                //    string btnId = button.ID;

                //    #region Enabled
                //    if (this.btn_Platform1.ID.ToString() == btnId)
                //    {
                //        this.btn_Platform1.Enabled = true;
                //        SetButtonStyle(btn_Platform1);
                //    }
                //    else if (this.btn_Platform2.ID.ToString() == btnId)
                //    {
                //        this.btn_Platform2.Enabled = true;
                //        SetButtonStyle(btn_Platform2);
                //    }
                //    else if (this.btn_Platform3.ID.ToString() == btnId)
                //    {
                //        this.btn_Platform3.Enabled = true;
                //        SetButtonStyle(btn_Platform3);
                //    }
                //    else if (this.btn_Platform4.ID.ToString() == btnId)
                //    {
                //        this.btn_Platform4.Enabled = true;
                //        SetButtonStyle(btn_Platform4);
                //    }
                //    else if (this.btn_Platform5.ID.ToString() == btnId)
                //    {
                //        this.btn_Platform5.Enabled = true;
                //        SetButtonStyle(btn_Platform5);
                //    }
                //    else if (this.btn_Platform6.ID.ToString() == btnId)
                //    {
                //        this.btn_Platform6.Enabled = true;
                //        SetButtonStyle(btn_Platform6);
                //    }
                //    else if (this.btn_Platform7.ID.ToString() == btnId)
                //    {
                //        this.btn_Platform7.Enabled = true;
                //        SetButtonStyle(btn_Platform7);
                //    }
                //    #endregion
                //}

                //SetButtonInfo(button, dockInfo);

                //index++;
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
        /// <summary>
        /// 初始化RadioButton
        /// </summary>
        private void InitRadioButton()
        {
            RBdictionary.Clear();
            Command command = new Command_CallMaterials();//要料
            //RBdictionary.Add(rb_CallMaterials.Text, command);  //暂屏蔽
            RBdictionary.Add(rb_OnlyCallMaterials.Text, command);
            m_Command = (Command)command;
            //operateName = rb_CallMaterials.Text;

            command = new Command_ReturnNullPallet();//退空托盘
            RBdictionary.Add(rb_ReturnNullPallt.Text, command);

            command = new Command_ReturnRemnantsMaterials();//退余料
            ////rb_ReturnRemnantsMaterials.Tag = command;
            RBdictionary.Add(rb_ReturnRemnantsMaterials.Text, command);

            command = new Command_ReturnBadMaterials();//退坏料
            //rb_ReturnBadMaterials.Tag = command;
            RBdictionary.Add(rb_ReturnBadMaterials.Text, command);

            command = new Command_ReturnNullPalletAndCallMaterials();//退空托盘+要料
            ////rb_ReturnNull_Call.Tag = command;
            RBdictionary.Add(rb_ReturnNull_Call.Text, command);

            command = new Command_ReturnWasteMaterials();//退废料
            RBdictionary.Add(rb_ReturnWasteMaterials.Text, command);
        }

        protected void radioButton_Click(object sender, EventArgs e)
        {
            try
            {
                RadioButton rdb = sender as RadioButton;
                m_Command = RBdictionary[rdb.Text] as Command;
                operateName = (string)rdb.Text;
                switch (operateName)
                {
                    case RadioButtonOperator.CallPallet:
                        InitButtonInfo();
                        Dock_Distributing(DockState.DOCK_STATUS_OK, dockList);
                        operateName = "要料+退空托盘";
                        break;
                    case RadioButtonOperator.OnlyCallPallet:
                        InitButtonInfo();
                        Dock_Distributing(DockState.DOCK_STATUS_OK, dockList);
                        operateName = "要料";
                        break;
                    case RadioButtonOperator.ReturnWasteMaterials:
                        InitButtonInfo();
                        Dock_Distributing("", dockList, "1");
                        break;
                    default:
                        InitButtonInfo();
                        Dock_Distributing(DockState.DOCK_STATUS_USING, dockList);
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        DockInfo info;

        //要料/退空托盘/退余料
        protected void button_Click(object sender, EventArgs e)
        {
            //LoginUser.UserName = UserName;
            txtblock_UserName.Text = LoginUser.UserName;
            try
            {
                //去掉提示空格  modified by pengenqi 2012/6/12
                operateName = operateName.Replace(" ", "");

                if (m_Command == null) return;
                Button button = sender as Button;
                if (button.Enabled == false || button.Text == "")
                {
                    return;
                }
                else
                {
                    info = (DockInfo)TBdictionary[button.Text];
                    m_Command.Info = info;
                    Session["DockNumber"] = info.DockNumber;
                    LoginUser.DockNumber = info.DockNumber;
                    string modelCode = info.ModelCode;

                    #region 退余料
                    if (this.rb_ReturnRemnantsMaterials.Checked == true)
                    {
                        bool res = false;

                        try
                        {
                            button.Enabled = false;
                            res = m_Command.Check();
                        }
                        catch (Exception ex)
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('" + ex.Message + "');</script>");
                            return;
                        }
                        finally
                        {
                            button.Enabled = true;
                        }
                        if (res == false)
                        {
                            string machineCX = info.MachineNumber;
                            Session["rb_ReturnRemnantsMaterials"] = false;
                            Response.Redirect("Return_Remnant_Material.aspx");
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('对不起,当前站台存在未完成的任务，您不能进行退余料');</script>");
                        }
                    }
                    #endregion

                    #region 退坏料
                    if (this.rb_ReturnBadMaterials.Checked == true)
                    {
                        bool res = false;
                        try
                        {
                            button.Enabled = false;
                            res = m_Command.Check();
                        }
                        catch (Exception ex)
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('" + ex.Message + "');</script>");
                            return;
                        }
                        finally
                        {
                            button.Enabled = true;
                        }

                        if (res == false)
                        {
                            string machineCX = info.MachineNumber;
                            Session["rb_ReturnBadMaterials"] = false;
                            Response.Redirect("Return_Bad_Materials_new.aspx");
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('对不起,当前站台存在未完成的任务，您不能进行退坏料');</script>");
                        }
                    }
                    #endregion

                    #region 退空托盘+要料
                    else if (this.rb_ReturnNull_Call.Checked == true)
                    {
                        string msg = string.Empty;
                        string errcode = string.Empty;
                        try
                        {
                            button.Enabled = false;
                            //msg = m_Command.Execute();
                            msg = cb.ReturnNullAndCallPallet(LoginUser.Url, out errcode, LoginUser.SessionKey, LoginUser.WarehouseID, info.MachineDockID, LoginUser.HasLGV.ToString(), LoginUser.RunsCodeId, LoginUser.RunsGroupId, LoginUser.SchemeId, LoginUser.MachineId, LoginUser.Token);
                        }
                        catch (Exception ex)
                        {
                            msg = ex.Message;
                        }
                        finally
                        {
                            button.Enabled = true;
                        }
                        //if (string.IsNullOrEmpty(msg))
                        //{
                        //    button.Enabled = false;
                        //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退空托盘任务已产生！');</script>");
                        //    if (operateName == RadioButtonOperator.CallPallet)
                        //    {
                        //        InitButtonInfo();
                        //        Dock_Distributing(DockState.DOCK_STATUS_OK, dockList);
                        //    }
                        //    RefreshListView();
                        //    ItemDetailBind();
                        //}
                        //else
                        //{
                        //    ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作失败！原因：" + msg + "');</script>");
                        //}

                        if (string.Equals(errcode,"0"))
                        {
                            button.Enabled = false;
                            if (string.IsNullOrEmpty(msg))
                            {
                                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作成功。');</script>");
                                
                            }
                            else
                            {
                                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退空盘成功，但要料失败，原因：" + msg + "，请进入呼叫终端进行要料');");
                            }
                            if (operateName == RadioButtonOperator.CallPallet)
                            {
                                InitButtonInfo();
                                Dock_Distributing(DockState.DOCK_STATUS_OK, dockList);
                            }
                            RefreshListView();
                            ItemDetailBind();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作失败！原因：" + msg + "');</script>");
                        }
                    }
                    #endregion

                    #region 退空托盘
                    else if (rb_ReturnNullPallt.Checked == true)
                    {
                        string msg = string.Empty;
                        try
                        {
                            button.Enabled = false;
                            msg = m_Command.Execute();
                        }
                        catch (Exception ex)
                        {
                            msg = ex.Message;
                        }
                        finally
                        {
                            button.Enabled = true;
                        }
                        if (string.IsNullOrEmpty(msg))
                        {
                            button.Enabled = false;
                            //ReFreshMachineNumber();
                            //ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退空托盘任务已产生！');</script>");
                            //if (operateName == RadioButtonOperator.ReturnNullPallet)
                            //{
                            //    InitButtonInfo();
                            //    Dock_Distributing(DockState.DOCK_STATUS_OK, dockList);
                            //}
                            //RefreshListView();
                            //ItemDetailBind();
                            //btn_Refresh_Click(null, null);

                            string url = "Call_Terminal_SemiFinishedProduct.aspx";
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退空托盘任务已产生！');location.href='" + url + "';</script>");

                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作失败！原因：" + msg + "');</script>");
                        }
                    }
                    #endregion

                    #region 退废料
                    else if (rb_ReturnWasteMaterials.Checked == true)
                    {
                        string msg = string.Empty;
                        try
                        {
                            button.Enabled = false;
                            msg = m_Command.Execute();
                            //msg = cb.ReturnWasteMaterials(LoginUser.Url, LoginUser.SessionKey, LoginUser.WarehouseID, info.MachineDockID, LoginUser.HasLGV.ToString(), LoginUser.RunsCodeId, LoginUser.RunsGroupId, LoginUser.SchemeId, LoginUser.MachineId, LoginUser.Token);
                        }
                        catch (Exception ex)
                        {
                            msg = ex.Message;
                        }
                        finally
                        {
                            button.Enabled = true;
                        }
                        if (string.IsNullOrEmpty(msg))
                        {
                            //button.Enabled = false;
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('退废料任务已产生！');</script>");
                            if (operateName == RadioButtonOperator.ReturnWasteMaterials)
                            {
                                InitButtonInfo();
                                Dock_Distributing(DockState.DOCK_STATUS_USING, dockList,"1");
                                //Dock_Distributing("", dockList);
                            }
                            RefreshListView();
                            ItemDetailBind();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作失败！原因：" + msg + "');</script>");
                        }
                    }
                    #endregion

                    #region 要料
                    else if (rb_OnlyCallMaterials.Checked == true)
                    {
                        string machineCX = info.MachineNumber;
                        string msg = string.Empty;
                        try
                        {
                            button.Enabled = false;
                            msg = m_Command.ExecuteCallMaterials(LoginUser.Url, LoginUser.MachineId, LoginUser.HasLGV.ToString(), info.MachineDockID, LoginUser.SchemeId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, LoginUser.Token);
                        }
                        catch (Exception ex)
                        {
                            msg = ex.Message;
                        }
                        finally
                        {
                            button.Enabled = true;
                        }
                        if (string.IsNullOrEmpty(msg))
                        {
                            button.Enabled = false;
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('要料成功。');</script>");

                            if (operateName == RadioButtonOperator.OnlyCallPallet)
                            {
                                InitButtonInfo();
                                Dock_Distributing(DockState.DOCK_STATUS_OK, dockList);
                                LoginUser.Recycle = true;
                            }
                            RefreshListView();
                            ItemDetailBind();
                            //string url = "Call_Terminal_SemiFinishedProduct.aspx";
                            //ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('要料成功！');location.href='" + url + "';</script>");
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('操作失败！原因：" + msg + "');</script>");
                        }
                        //1029
                        if (LoginUser.Recycle == false)
                        {
                            return;
                        }
                        ItemDetailBind();
                    }
                    #endregion

                    SetButtonStyle(button);
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

        //刷新
        protected void btn_Refresh_Click(object sender, EventArgs e)
        {
            CommonBLL bll = new CommonBLL();
            DataTable dt = bll.GetMachineCurPlanByMachineNumber(LoginUser.Url, LoginUser.SessionKey, LoginUser.MachineId, LoginUser.RunsCodeId, LoginUser.RunsGroupId, LoginUser.Token);
            if (dt == null)
            {
                string url = "Call_Terminal_Log.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('获取生产牌号失败！');window.location='" + url + "';</script>");
                return;
            }
            if (dt.Rows.Count == 0)
            {
                string url = "Call_Terminal_Log.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台未设定生产牌号！');window.location='" + url + "';</script>");
                return;
            }
            if (dt.Rows.Count != 1)
            {
                string url = "Call_Terminal_Log.aspx";
                ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>alert('当前机台设定了多个生产工单！');window.location='" + url + "';</script>");
                return;
            }

            //StateCode = dt.Rows[0]["STATE_CODE"].ToString();
            LoginUser.ItemID = dt.Rows[0]["item_id"].ToString();
            LoginUser.ItemName = dt.Rows[0]["item_name"].ToString();
            LoginUser.MachineId = dt.Rows[0]["equipment_id"].ToString();
            LoginUser.MachineNumber = dt.Rows[0]["equipment_code"].ToString();
            LoginUser.MachineName = dt.Rows[0]["equipment_name"].ToString();
            LoginUser.RunsCode = dt.Rows[0]["shift_name"].ToString();//shift_name
            LoginUser.RunsCodeId = dt.Rows[0]["shift_id"].ToString();//shift_id
            LoginUser.RunsGroup = dt.Rows[0]["team_name"].ToString();//team_name
            LoginUser.RunsGroupId = dt.Rows[0]["team_id"].ToString();//team_id
            LoginUser.SchemeId = dt.Rows[0]["t_id"].ToString();
            LoginUser.RunsTypeCode = LoginUser.RunsGroup + "-" + LoginUser.RunsCode;
            Session["productitemname"] = LoginUser.ItemName;
            Response.Redirect("Call_Terminal_SemiFinishedProduct.aspx");
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

        //查看配盘单明细
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
                DockInfo _di = JsonConvert.DeserializeObject<DockInfo>(tb.Attributes["DataSource"]);
                string dockcode = _di.DockNumber;
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "", " <script language='javascript'>window.showModalDialog('Dock_Materials.aspx?dockNumber=" + modelCode_dockNumber + "',window,'dialogHeight: 400px; dialogWidth: 427px; dialogTop: 200px; dialogLeft: 233px;resizable:yes;scroll:yes;center:yes;');window.location=this.location;</script>");
                string pagepath = "Dock_Materials.aspx?dockcode=" + dockcode;
                openModelWin(pagepath, 300, 527);
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
            FinsishTask.PageIndex = e.NewPageIndex;
            ReFreshMachineNumber();
        }
        //退余料
        protected void rb_ReturnRemnantsMaterials_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ReturnRemnantsMaterials.Checked == true)
            {
                //btnReturn.Visible = true;
                //btnReturn.Font.Bold = true;
                //btnReturn.Font.Size = new FontUnit(11);
                m_Command = new Command_ReturnRemnantsMaterials();
                operateName = (string)rb_ReturnRemnantsMaterials.Text;
                InitButtonInfo();
                Dock_Distributing(DockState.DOCK_STATUS_USING, dockList);
                ItemDetailBind();
            }
        }

        protected void btn_PalletType_Click(object sender, EventArgs e)
        {
            Response.Redirect("PalletType.aspx");
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            radioButton_Click(rb_ReturnNullPallt, e);
            //rb_CallMaterials.Checked = false;  //暂屏蔽
            rb_ReturnNullPallt.Checked = true;
            operateName = rb_ReturnNullPallt.Text.ToString();
            string docknumber = info.DockNumber;
            string flag1 = string.Empty;
            if (docknumber.StartsWith("J"))
            {
                if (Convert.ToInt32(docknumber.Substring(docknumber.Length - 1, 1)) % 2 == 1)
                {
                    int a = Convert.ToInt32(docknumber.Substring(docknumber.Length - 1, 1)) + 1;
                    Button btn = (Button)ButtonTable["button" + a.ToString()];
                    for (int i = 0; i < ButtonTable.Count; i++)
                    {
                        if (btn_Platform1.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(1); </script>");
                            break;
                        }
                        else if (btn_Platform2.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(2); </script>");
                            break;
                        }
                        else if (btn_Platform3.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(3); </script>");
                            break;
                        }
                        else if (btn_Platform4.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(4); </script>");
                            break;
                        }
                        else if (btn_Platform5.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(5); </script>");
                            break;
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(6); </script>");
                            break;
                        }
                    }
                }
                else if ((Convert.ToInt32(docknumber.Substring(docknumber.Length - 1, 1)) % 2 == 0))
                {
                    int a = Convert.ToInt32(docknumber.Substring(docknumber.Length - 1, 1)) - 1;
                    Button btn = (Button)ButtonTable["button" + a.ToString()];
                    for (int i = 0; i < ButtonTable.Count; i++)
                    {
                        if (btn_Platform1.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(1); </script>");
                            break;
                        }
                        else if (btn_Platform2.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(2); </script>");
                            break;
                        }
                        else if (btn_Platform3.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(3); </script>");
                            break;
                        }
                        else if (btn_Platform4.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(4); </script>");
                            break;
                        }
                        else if (btn_Platform5.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(5); </script>");
                            break;
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(6); </script>");
                            break;
                        }
                    }
                }
            }
            else if (docknumber.StartsWith("B"))
            {
                if (Convert.ToInt32(docknumber.Substring(docknumber.Length - 1, 1)) % 2 == 1)
                {
                    int a = Convert.ToInt32(docknumber.Substring(docknumber.Length - 1, 1)) - 1;
                    Button btn = (Button)ButtonTable["button" + a.ToString()];
                    for (int i = 0; i < ButtonTable.Count; i++)
                    {
                        if (btn_Platform1.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(1); </script>");
                            break;
                        }
                        else if (btn_Platform2.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(2); </script>");
                            break;
                        }
                        else if (btn_Platform3.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(3); </script>");
                            break;
                        }
                        else if (btn_Platform4.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(4); </script>");
                            break;
                        }
                        else if (btn_Platform5.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(5); </script>");
                            break;
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(6); </script>");
                            break;
                        }
                    }
                }
                else if ((Convert.ToInt32(docknumber.Substring(docknumber.Length - 1, 1)) % 2 == 0))
                {
                    int a = Convert.ToInt32(docknumber.Substring(docknumber.Length - 1, 1)) - 3;
                    Button btn = (Button)ButtonTable["button" + a.ToString()];
                    for (int i = 0; i < ButtonTable.Count; i++)
                    {
                        if (btn_Platform1.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(1); </script>");
                            break;
                        }
                        else if (btn_Platform2.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(2); </script>");
                            break;
                        }
                        else if (btn_Platform3.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(3); </script>");
                            break;
                        }
                        else if (btn_Platform4.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(4); </script>");
                            break;
                        }
                        else if (btn_Platform5.ID == btn.ID)
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(5); </script>");
                            break;
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(ClientScript.GetType(), "", @"<script>firm1(6); </script>");
                            break;
                        }
                    }
                }
            }
            RefreshListView();
            ItemDetailBind();
            LoginUser.Recycle = false;
        }

        protected void ItemDetail_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            ItemDetail.PageIndex = e.NewSelectedIndex;
            ReFreshMachineNumber();
        }

        private void SetButtonStyle(Button btn)
        {
            if (btn != null)
            {
                btn.Font.Bold = btn.Enabled;
                btn.Font.Size = new FontUnit(btn.Enabled ? 16 : 12);
            }
        }
    }
}