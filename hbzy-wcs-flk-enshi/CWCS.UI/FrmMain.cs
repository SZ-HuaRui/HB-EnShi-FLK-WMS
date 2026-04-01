using C1.Win.C1Command;
using C1.Win.C1Ribbon;
using CWCS.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WCS;
using CWCS.Repository;

namespace CWCS.UI
{
    public partial class FrmMain :C1RibbonForm
    {
        public const int WM_SYSCOMMAND = 0x112;
        public const int WM_MINIMIZE = 0xF020;
        public const int WM_MAXIMIZE = 0xF030;
        public const int WM_CLOSE = 0xF060;
        public const int WM_RESTORE = 0xF120;
        private OracleRepository repository = null;

        Thread P_thread;
        Thread ProcessBar_thread;
        public FrmMain()
        {
            InitializeComponent();
            MaximizedBounds = Screen.PrimaryScreen.WorkingArea;
            timer1.Enabled = false;
        }

        private void ShowTime(DateTime dt)
        {
            P_thread = new Thread(
                () =>
                {
                    while (true)
                    {
                        this.Invoke(
                            (MethodInvoker)delegate()
                            //在对控件的 Invoke 方法进行调用时或需要一个简单委托又不想自己定义时可以使用该委托。   
                            {
                                this.toolStripStatusLabel2.Text = "系统时间：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                                TimeSpan ts1 = new TimeSpan(dt.Ticks);
                                TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                                TimeSpan ts3 = ts1.Subtract(ts2).Duration();

                                this.toolStripStatusLabel3.Text = "运行时长：" + ts3.ToString("c");

                            });
                        Thread.Sleep(1000);
                    }
                });
            P_thread.IsBackground = true;
            P_thread.Start();
        }

        private void ShowProcessBar()
        {
            ProcessBar_thread = new Thread(
              () =>
              {
                  while (true)
                  {
                      this.Invoke(
                          (MethodInvoker)delegate()
                          //在对控件的 Invoke 方法进行调用时或需要一个简单委托又不想自己定义时可以使用该委托。   
                          {
                              if (this.toolStripStatusLabel1.Value < this.toolStripStatusLabel1.Maximum)
                              {
                                  int max = this.toolStripStatusLabel1.Value + 25;
                                  if (max > this.toolStripStatusLabel1.Maximum)
                                      this.toolStripStatusLabel1.Value = this.toolStripStatusLabel1.Maximum;
                                  else
                                      this.toolStripStatusLabel1.Value += 25;
                              }
                              else
                              {
                                  this.toolStripStatusLabel1.Value = 0;
                              }

                          });
                      Thread.Sleep(600);
                  }
              });
            ProcessBar_thread.IsBackground = true;
            ProcessBar_thread.Start();
        }

        /// <summary>
        /// 加载系统信息
        /// </summary>
        private void InitSysInfo()
        {
            DateTime dtStart = DateTime.Now;
            ShowTime(dtStart);
            ShowProcessBar();
            this.toolStripStatusLabel4.Text = "系统版本号：" + Application.ProductVersion.ToString();
            this.toolStripStatusLabel5.Text = "湖北中烟恩施卷烟厂辅料库";
            this.Text = "恩施卷烟厂辅料库WCS系统";

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                
                repository = new OracleRepository();
                LoadWcsParamConfig();
                MaximizedBounds = Screen.PrimaryScreen.WorkingArea;

                //ShowForm(new FormBDManager(), "FormBDManager", "BD管理");
                //FormAGVManager formAgv01 = new FormAGVManager("AGV01", "192.168.57.31", "8000");
                //ShowForm(formAgv01, "FormAGVManager01", "一楼AGV管理");
                //return;

                InitSysInfo();
                LoadTabControl();
             

            }
            catch (Exception ex)
            {
                MessageBox.Show("系统信息:" + ex.Message.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void LoadWcsParamConfig()
        {
            DataTable dt = repository.QueryTableData(new WCS_PARAMETER_CONFIG());
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WCS_PARAMETER_CONFIG item = new WCS_PARAMETER_CONFIG();
                    item.T_ID = dr["t_id"].ToString();
                    item.PARAM_CODE = dr["param_code"].ToString();
                    item.PARAM_NAME = dr["param_name"].ToString();
                    item.PARAM_VALUE = dr["param_value"].ToString();
                    item.PARAM_TYPE = dr["param_type"].ToString();
                    item.CAN_EDIT = dr["can_edit"].ToString();
                    StaticCommon.WcsParamConfigList.Add(item);
                }
            }

         }
            /// <summary>
            /// 加载TabControl
            /// </summary>
            private void LoadTabControl()
           {
            List<WCS_PARAMETER_CONFIG> listConfig = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.StartsWith("View")).ToList();
            foreach (WCS_PARAMETER_CONFIG param in listConfig)
            {
                if (param.PARAM_VALUE.Equals("1"))
                {
                    if (param.PARAM_CODE.Equals("ViewConveryManager"))
                    {
                        FrmConveryManager formsta = new FrmConveryManager();
                        ShowForm(formsta, "FrmConveryManager", "输送机管理");
                    }
                    if (param.PARAM_CODE.Equals("ViewBDManager"))
                    {
                        ShowForm(new FormBDManager(), "FormBDManager", "BD门管理");
                    }
                    if (param.PARAM_CODE.Equals("ViewAgv01Manager"))
                    {
                        FormAGVManager formAgv01 = new FormAGVManager("AGV01",
                        StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("NT8000IpFloor01")).ToList()[0].PARAM_VALUE,
                        StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("NT8000PortFloor01")).ToList()[0].PARAM_VALUE);
                        ShowForm(formAgv01, "FormAGVManager01", "一楼AGV管理");
                    }
                    if (param.PARAM_CODE.Equals("ViewAgv02Manager"))
                    {
                        FormAGVManager formAgv02 = new FormAGVManager("AGV02",
                        StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("NT8000IpFloor02")).ToList()[0].PARAM_VALUE,
                        StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals("NT8000PortFloor02")).ToList()[0].PARAM_VALUE);
                        ShowForm(formAgv02, "FormAGVManager02", "二楼AGV管理");
                    }
                    if (param.PARAM_CODE.Equals("ViewSCManager"))
                    {
                        FrmSRMManager formSC = new FrmSRMManager();
                        ShowForm(formSC, "FrmSRMManager", "堆垛机管理");
                    }
                    if (param.PARAM_CODE.Equals("ViewTaskManager"))
                    {

                        FormTaskManager TaskManager = new FormTaskManager();
                        ShowForm(TaskManager, "FormTaskManager", "任务管理");
                    }
                    if (param.PARAM_CODE.Equals("ViewLogManager"))
                    {
                        FormSystemLog eventlog = new FormSystemLog();
                        ShowForm(eventlog, "FormSystemLog", "系统日志");
                    }
                    if (param.PARAM_CODE.Equals("ViewToolBox"))
                    {
                        FormToolBox formToolBox = new FormToolBox();
                        ShowForm(formToolBox, "FormToolBox", "工具箱");
                    }
                }
            }
        }

        private C1DockingTabPage GetPage(string aKeyName)
        {
            C1DockingTabPage result;
            foreach (C1DockingTabPage c1DockingTabPage in this.tabMain.TabPages)
            {
                if (c1DockingTabPage.Name.Equals(aKeyName))
                {
                    result = c1DockingTabPage;
                    return result;
                }
            }
            result = null;
            return result;
        }



        public void ShowForm(Form aForm, string aKeyName, string aCaption)
        {

            C1DockingTabPage page = this.GetPage(aKeyName);
            if (page == null)
            {
                if (aForm != null)
                {
                    aForm.Text = aCaption;
                    aForm.TopLevel = false;
                    aForm.FormBorderStyle = FormBorderStyle.None;
                    aForm.Visible = true;
                    aForm.Dock = DockStyle.Fill;
                    C1DockingTabPage tabPage = new C1DockingTabPage();
                    tabPage.Controls.Add(aForm);
                    tabPage.Text = aCaption;
                    tabPage.Name = aKeyName;
                    this.tabMain.TabPages.Add(tabPage);
                    aForm.Size = tabPage.Size;
                    aForm.BackColor = this.tabMain.BackColor;
                }
            }
            else
            {
                this.tabMain.SelectedTab = page;
                this.tabMain.ScrollToSelectedTab();
                page.Show();
            }
        }

        private void ScanEquipmentStatus()
        {
            foreach (C1DockingTabPage tb in this.tabMain.TabPages)
            {
                if (!tb.Focused)
                {
                    foreach (Control c in tb.Controls)
                    {
                        if (c is FrmConveryManager)
                        {
                            FrmConveryManager frm = (FrmConveryManager)c;
                            if (frm.dicEquipStatus.ContainsValue(false))
                             {
                                    tb.TabBackColor = tb.TabBackColor == Color.Red ?Color.Transparent : Color.Red;
                                    tb.TabForeColor = Color.Red;
                                    break;
                             }
                        }
                        else if (c is FrmSRMManager)
                        {
                            FrmSRMManager frm = (FrmSRMManager)c;
                            if (frm.dicEquipStatus.ContainsValue(false))
                                tb.TabBackColor = tb.TabBackColor == Color.Red ? Color.Transparent : Color.Red;
                        }
                    }
                }
            }
        }

        #region 窗体事件
        protected override void WndProc(ref   Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                int wParam = (int)m.WParam;
                //拦截 除最小化 恢复 关闭 以外的消息
                if (wParam != WM_MINIMIZE && wParam != WM_CLOSE && wParam != WM_RESTORE && wParam != WM_MAXIMIZE)
                {
                    return;
                }
            }
            base.WndProc(ref   m);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.WindowState = FormWindowState.Minimized;
            e.Cancel = true;
            this.notifyWCS.Visible = true;
        }

        private void CloseCtiServer()
        {
            ProcessBar_thread.Abort();
            P_thread.Abort();
            this.notifyWCS.Dispose();
            Application.ExitThread();
            this.Close();
            Application.Exit();
            //"CWCS.UI"
            Process p =  Process.GetCurrentProcess();
            
            if(p != null) p.Kill();
        }

        private void HideCtiServer()
        {
            this.Hide();
        }

        private void ShowCtiServer()
        {
            this.Show();
            this.WindowState = FormWindowState.Maximized;
            this.Activate();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCtiServer();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HideCtiServer();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseCtiServer();
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            ScanEquipmentStatus();
        }

        private void notifyWCS_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (WindowState == FormWindowState.Minimized)
            {

                WindowState = FormWindowState.Maximized;
                this.Activate();
                this.ShowInTaskbar = true;
                this.ShowIcon = true;
                this.notifyWCS.Visible = true;

            }
        }

        //private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    System.Environment.Exit(0);
        //}
    }
}
