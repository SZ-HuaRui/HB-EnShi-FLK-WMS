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
    public partial class FileRobotControl:RobotBaseControl
    {
        private delegate void RichBoxCallBack(RichTextBox rb, string msg);

        private delegate void LabelCallBack(Label lb, bool b_ConnectStatus);

        private delegate void TextBoxCallBack(TextBox txtbox, string status);



        public FileRobotControl()
        {
            InitializeComponent();

            this.Resize += new EventHandler(FileRobotControl_Resize);
            SetTag(this);
            winX = this.Width;
            winY = this.Height;
        }
        void FileRobotControl_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / winX;
            float newy = (this.Height) / winY;
            setControls(newx, newy, this);
        }
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
        private string AnalyticalRobotCommand(int code)
        {

            string str = string.Empty;
            switch (code)
            { //0，无任务；1，装盘中；2，装盘完成(RB清0)

                case 0: str = "无任务"; break;
                case 1: str = "分拣中"; break;
              
                default: str = ""; break;
            }
            return str;
        }

        private string AnalyticalCommand(int code)
        {

            string str = string.Empty;
            switch (code)
            { //0，无指令；1，申请读码；2，品规下发完成(WCS);3.分拣完成

                case 0: str = "无指令"; break;
                case 1: str = "申请读码"; break;
                case 2: str = "下发完成"; break;
                case 3: str = "分拣完成"; break;
                default: str = ""; break;
            }
            return str;
        }


        public void ShowRobotStatus(RobotInfo rb)
        {
            if (rb != null)
            {
                //ShowRunStatus(txtFile1Status, AnalyticalRunStatus(rb.rbstate));
                //ShowRunStatus(txtFile1Command, AnalyticalCommand(rb.f));
                if (rb.LstBattery != null)
                {
                    if (rb.LstBattery.Count > 0)
                    {
                        ShowRunStatus(txtbarcode1, rb.LstBattery[0].BatteryCode.ToString());
                        ShowRunStatus(txtbarcode2, rb.LstBattery[1].BatteryCode.ToString());
                    }
                }
            }
        }
    }
}
