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
    public partial class BindingRobotControl:RobotBaseControl
    {
        public event MouseEventHandler RobotMouseClick;

        public List<BatteryInfo> Putbatterylst = new List<BatteryInfo>();
        //public List<BatteryInfo> Pickbatterylst = new List<BatteryInfo>();
        string putrobotnumber;
        string pickrobotnumber;
        string typecode;//加拘束还是解拘束
        public BindingRobotControl()
        {
            InitializeComponent();

            this.Resize += new EventHandler(BindingRobotControl_Resize);
            SetTag(this);
           winX = this.Width;
            winY = this.Height;
        }

        void BindingRobotControl_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / winX;
           float newy = (this.Height) / winY;
            setControls(newx, newy, this);
        }

        public void SetText(string strName1, string strName2, string putrobotnumber, string pickrobotnumber, string typecode)
        {
            this.groupBoxBinding.Text = strName1;
            this.groupBoxCommon.Text = strName2;
            this.putrobotnumber = putrobotnumber;
            this.pickrobotnumber = pickrobotnumber;
            this.typecode = typecode;
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
            {//0，未启动；1，自动；2，手动；3，故障

                case 0: str = "未启动"; break;
                case 1: str = "自动"; break;
                case 2: str = "手动"; break;
                case 3: str = "故障"; break;
                default: str = ""; break;
            }
            return str;
        }

        #endregion

        public void ShowRobotStatus(RobotInfo rb)
        {
            if (typecode == "OutFeed")
            {
                //ShowTime(lblPLCConnect, rb.m_ConnectStatus);
                if (rb.stationtype == 1)//组盘
                {
                   // ShowRunStatus(txtCommonStatus, AnalyticalRunStatus(rb.rbstate));
                    //ShowRunStatus(txtCommonCount, rb.rbnumber.ToString());
                   // ShowRunStatus(txtCommonTray, rb.traycode);
                    if (rb.LstBattery.Count > 0)
                    {
                        Putbatterylst = rb.LstBattery;


                        foreach (var item in rb.LstBattery)
                        {
                            if (!string.IsNullOrEmpty(item.BatteryCode))
                                this.trayControl2.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Fill;
                            else
                                this.trayControl2.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Empty;
                        }
                    }
                    if (rb.m_ConnectStatus)
                        this.trayControl2.ConnectStatus = TrayConnect.Connected;
                    else
                        this.trayControl2.ConnectStatus = TrayConnect.DisConnected;
                }
                else//解盘
                {
                   //ShowRunStatus(txtBindingStatus, AnalyticalRunStatus(rb.rbstate));
                   // ShowRunStatus(txtBindingCount, rb.rbnumber.ToString());
                   // ShowRunStatus(txtBindingTray, rb.traycode);
                    if (rb.LstBattery != null)
                    {
                        if (rb.LstBattery.Count > 0)
                        {
                            Putbatterylst = rb.LstBattery;

                            foreach (var item in rb.LstBattery)
                            {
                                if (!string.IsNullOrEmpty(item.BatteryCode))
                                    this.trayControl1.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Fill;
                                else
                                    this.trayControl1.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Empty;
                            }
                        }
                    }
                    if (rb.m_ConnectStatus)
                        this.trayControl1.ConnectStatus = TrayConnect.Connected;
                    else
                        this.trayControl1.ConnectStatus = TrayConnect.DisConnected;
                }
            }
            else
            {
                //ShowTime(lblPLCConnect, rb.m_ConnectStatus);
                if (rb.stationtype == 1)//组盘
                {

                    //ShowRunStatus(txtBindingStatus, AnalyticalRunStatus(rb.rbstate));
                  //  ShowRunStatus(txtBindingCount, rb.rbnumber.ToString());
                   // ShowRunStatus(txtBindingTray, rb.traycode);
                    if (rb.LstBattery != null)
                    {
                        if (rb.LstBattery.Count > 0)
                        {
                            Putbatterylst = rb.LstBattery;

                            foreach (var item in rb.LstBattery)
                            {
                                if (!string.IsNullOrEmpty(item.BatteryCode))
                                    this.trayControl1.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Fill;
                                else
                                    this.trayControl1.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Empty;
                            }
                        }
                    }
                    if (rb.m_ConnectStatus)
                        this.trayControl1.ConnectStatus = TrayConnect.Connected;
                    else
                        this.trayControl1.ConnectStatus = TrayConnect.DisConnected;
                }
                else//解盘
                {
                  //  ShowRunStatus(txtCommonStatus, AnalyticalRunStatus(rb.rbstate));
                  //  ShowRunStatus(txtCommonCount, rb.rbnumber.ToString());
                  //  ShowRunStatus(txtCommonTray, rb.traycode);
                    if (rb.LstBattery.Count > 0)
                    {
                        Putbatterylst = rb.LstBattery;


                        foreach (var item in rb.LstBattery)
                        {
                            if (!string.IsNullOrEmpty(item.BatteryCode))
                                this.trayControl2.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Fill;
                            else
                                this.trayControl2.LstUnitStatus[item.Position - 1] = TrayUnitStatus.Empty;
                        }
                    }
                    if (rb.m_ConnectStatus)
                        this.trayControl2.ConnectStatus = TrayConnect.Connected;
                    else
                        this.trayControl2.ConnectStatus = TrayConnect.DisConnected;
                }
            }



        }

        private void trayControl2_DoubleClick(object sender, EventArgs e)
        {
            //BatteryControl battery = new BatteryControl(Putbatterylst);
            //battery.ShowData();
            //battery.ShowDialog();
        }


        private void trayControl2_MouseClick(object sender, MouseEventArgs e)
        {
            RobotMouse_Click((object)trayControl2, e);
        }



        private void trayControl1_MouseClick(object sender, MouseEventArgs e)
        {
            RobotMouse_Click((object)trayControl1, e);
        }

        private void RobotMouse_Click(object sender, MouseEventArgs e)
        {
            if (RobotMouseClick != null)
            {
                string rbnumber = "";
                if (typecode == "InFeed")
                {
                    if ((TrayControl)sender == trayControl2)//加拘束
                    {
                        rbnumber = pickrobotnumber;
                    }
                    else
                    {
                        rbnumber = putrobotnumber;

                    }
                }
                else
                {
                    if ((TrayControl)sender == trayControl2)//解拘束
                    {
                        rbnumber = putrobotnumber;
                    }
                    else
                    {
                        rbnumber = pickrobotnumber;
                    }
                }
                RobotMouseClick(rbnumber, e);
            }
        }

        public void ShowBatteryInfo(List<BatteryInfo> lst, string rbnumber)
        {
            BatteryControl battery = new BatteryControl(lst, rbnumber, "");
            battery.ShowData();
            battery.ShowDialog();
        }
    }
}
