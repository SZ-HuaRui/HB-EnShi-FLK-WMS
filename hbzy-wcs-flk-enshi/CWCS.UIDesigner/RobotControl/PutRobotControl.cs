using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWCS.Entity;
using CWCS.Entity.Robot;


namespace CWCS.UIDesigner.RobotControl
{
    public partial class PutRobotControl : RobotBaseControl
    {
        public event MouseEventHandler RobotMouseClick;

        public string robotnum;
        public string equipmenttype;
        public List<BatteryInfo> batterylst = new List<BatteryInfo>();
        public int ContainerCount = 16;//装盘数量

        public PutRobotControl()
        {
            InitializeComponent();
            this.Resize += new EventHandler(PutRobotControl_Resize);
            SetTag(this);
            winX = this.Width;
            winY = this.Height;
            trayControl1.ContainerCount = ContainerCount;
        }

        void PutRobotControl_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / winX;
            float newy = (this.Height) / winY;
            setControls(newx, newy, this);
        }
    

        public void SetTextData(string robonum, string equipmenttype)
        {
            this.robotnum = robonum;
            this.equipmenttype = equipmenttype;
            if (equipmenttype == "PutRobot")
            {
                this.lblCount.Text = "装盘数量:";
                this.gbTop.Text = "装盘机械手-" + robonum;
            }
            else
            {
                this.lblCount.Text = "拆盘数量";
                this.gbTop.Text = "拆盘机械手-" + robonum;
            }

        }

        #region 委托事务
        private delegate void RichBoxCallBack(RichTextBox rb, string msg);

        private delegate void LabelCallBack(Label lb, bool b_ConnectStatus);

        private delegate void TextBoxCallBack(TextBox txtbox, string status);


        public void ShowLog(string msg)
        {
            ShowText(txtLog, msg);
        }
        private void ShowText(RichTextBox rb, string msg)
        {
            if (rb.InvokeRequired)
            {
                RichBoxCallBack rbcb = new RichBoxCallBack(ShowText);
                this.Invoke(rbcb, new object[] { rb, msg });
            }
            else
            {
                if (rb.TextLength > 5000)
                    rb.Clear();
                rb.AppendText(System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + ": " + msg + "\r\n");
            }
        }

        private void ShowTime(Label lb, bool b_ConnectStatus)
        {
            if (lb.InvokeRequired)
            {
                LabelCallBack rbcb = new LabelCallBack(ShowTime);
                this.Invoke(rbcb, new object[] { lb, b_ConnectStatus });
            }
            else
            {
                if (b_ConnectStatus)
                    lb.BackColor = lb.BackColor == Color.Lime ? Color.DarkGray : Color.Lime;
                else
                    lb.BackColor = Color.Red;
                lb.Text = b_ConnectStatus ? "已连接" : "断开";
            }
        }

        private void ShowRunStatus(TextBox tb, string runstatus)
        {
            if (tb.InvokeRequired)
            {
                TextBoxCallBack tbcb = new TextBoxCallBack(ShowRunStatus);
                this.Invoke(tbcb, new object[] { tb, runstatus });
            }
            else
            {
                tb.Text = runstatus;
            }
        }

        private string AnalyticalRunStatus(int code)
        {
            string str = string.Empty;
            switch (code)
            {
                case 0: str = "未启动"; break;
                case 1: str = "自动"; break;
                case 2: str = "手动"; break;
                case 4: str = "上电"; break;
                case 8: str = "运行"; break;
                case 16: str = "急停"; break;
                case 32: str = "故障"; break;
                default: str = ""; break;
            }
            return str;
        }
        private string AnalyticalCommand(int code)
        {

            string str = string.Empty;
            switch (code)
            { //0，无任务；1，装盘中；2，装盘完成(RB清0)

                case 0: str = "无"; break;
                case 1: str = "抓取完成"; break;
              
                default: str = ""; break;
            }
            return str;
        }

        #endregion

        public void ShowRobotStatus(RobotInfo rb)
        {
            ShowRunStatus(txtBindingStatus, AnalyticalRunStatus(rb.rbstate));
            ShowRunStatus(txtBindingCount, rb.rbnumber.ToString());
            ShowRunStatus(txtBindingTray,rb.workstation==1?rb.L_traycode:rb.R_traycode);
            ShowRunStatus(txtBindCommand, AnalyticalCommand(rb.l_barcodereading));
            trayControl1.ConnectStatus = rb.m_ConnectStatus?TrayConnect.Connected:TrayConnect.DisConnected;
            if (rb.LstBattery != null)
            {
                if (rb.LstBattery.Count > 0)
                {
                    batterylst = rb.LstBattery;
                    foreach (var item in rb.LstBattery)
                    {
                        if (!string.IsNullOrEmpty(item.BatteryCode))
                            this.trayControl1.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Fill;
                        else
                            this.trayControl1.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Empty;
                    }
                }
                else
                {
                    batterylst = new List<BatteryInfo>();
                }
            }
            else
            {
                batterylst = new List<BatteryInfo>();
            }
        }

        private void trayControl1_DoubleClick(object sender, EventArgs e)
        {
            BatteryControl battery = new BatteryControl(batterylst, robotnum, "");
            battery.ShowData();
            battery.ShowDialog();
        }

        private void trayControl1_MouseClick(object sender, MouseEventArgs e)
        {
            BatteryControl battery = new BatteryControl(batterylst, robotnum, "");
            battery.ShowData();
            battery.ShowDialog();
        }
    }
}
