using C1.Win.C1Ribbon;
using CWCS.BLL;
using CWCS.Entity.SRM;
using CWCS.UIDesigner.SRMControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CWCS.UI
{
    public partial class FrmSRMSend : C1RibbonForm
    {
        RGVObject SCNum ;
        private BLL.SRMBLL scbll;
        private String SCType = "";
        private String OldLocationNum = "";
        private String BarCode = "";
        private String FromStation = "";
        private String TaskNum = "";
        private String Num = "";
        private String Withboard = "";
        private bool isLineBlock;
        public static  bool isChange=false;
        public FrmSRMSend()
        {
            InitializeComponent();
        }
        public FrmSRMSend(string SCNum, string TaskNum,string BarCode,string OldLocationNum,string Type,bool isLineBlock)
        {
            InitializeComponent();
            this.Num = SCNum;
            this.SCType = Type;
            this.BarCode = BarCode;
            this.TaskNum = TaskNum;
            this.OldLocationNum = OldLocationNum;
            this.isLineBlock = isLineBlock;
            
        
        }

        public FrmSRMSend(RGVObject SCNum, BLL.SRMBLL scbll, string Type)
        {
            InitializeComponent();
            this.SCNum = SCNum;
        
            this.TaskNum = SCNum.M_WorkTaskno;
            this.scbll = scbll;
            this.SCType = Type;
            this.cmbCheckOut.Text = "Normal";
            this.cmbTaskMode.Text = "可出可入";
            DataTable dt = scbll.GetTaskInfo(SCNum.M_WorkTaskno);
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                this.BarCode = dr["barcode"].ToString();
                this.OldLocationNum = dr["tostation"].ToString().Replace("A","");
                this.FromStation = dr["fromstation"].ToString().Replace("A", ""); ;
                this.Withboard = dr["goodstype"].ToString();
            }
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (SCType == "cmdSendTask")
            {
                SRMCommand cmdType = SRMCommand.None;
                string strCommandType = cmbCommandType.Text.Trim();
                if (strCommandType == "--请选择--")
                {
                    MessageBox.Show("请选择命令！");
                    return;
                }
                else if (strCommandType == "取货指令")
                {
                    cmdType = SRMCommand.CR_GETC;
                }
                else if (strCommandType == "放货指令")
                {
                    cmdType = SRMCommand.CR_PUTC;
                }
                else if (strCommandType == "移动指令")
                {
                    cmdType = SRMCommand.CR_POS_GET;
                }
                else if (strCommandType == "收回穿梭板指令")
                {
                    cmdType = SRMCommand.CR_PICKSH_PUTC;
                }
                else if (strCommandType == "卸下穿梭板指令")
                {
                    cmdType = SRMCommand.CR_DROPSH_PUTC;
                }
                else if (strCommandType == "清除指令")
                {
                    cmdType = SRMCommand.CR_NO_FUNC;
                }
                if (cmdType == SRMCommand.None)
                    return;
                if (txt_taskno.Text.ToString().Trim().Length >= 5)
                {
                    MessageBox.Show("手动指令，任务号须小于10000","提示",MessageBoxButtons.OK);
                    txt_taskno.Text = string.Empty;
                    return;
                }
                SendTask(txtSCNum.Text.Trim(), txt_taskno.Text.Trim(), txt_from.Text.Trim(), txt_to.Text.Trim(), cmdType);

            }
            else if (SCType == "cmdTaskMode")
            {
                bool checkOut = true;
                SCTaskMode taskmode = SCTaskMode.Pause;
                string strCommandType = cmbTaskMode.Text.Trim();
                if (strCommandType == "--请选择--")
                {
                    MessageBox.Show("请选择命令！");
                    return;
                }
                else if (strCommandType == "可出可入")
                {
                    taskmode = SCTaskMode.All;
                }
                else if (strCommandType == "只入不出")
                {
                    taskmode = SCTaskMode.OnlyInFeed;
                }
                else if (strCommandType == "只出不入")
                {
                    taskmode = SCTaskMode.OnlyOutFeed;
                }
                ChangeTaskMode(SCNum.M_SCNum, taskmode, checkOut);
            }
            Close();
        }

        private void ShowToAddress()
        {
            lb_from.Visible = false;
            txt_from.Visible = false;
            txt_from.Text = string.Empty;
            txt_to.Location = new System.Drawing.Point(168, 137);
            lb_to.Location = new System.Drawing.Point(87, 140);
            lb_to.Visible = true;
            txt_to.Visible = true;
           
        }

        private void ShowFromAddress()
        {
            lb_from.Visible = true;
            txt_from.Visible = true;
            lb_to.Visible = false;
            txt_to.Visible = false;
            txt_to.Text = string.Empty;
        }
        private void ShowAllAddress()
        {
            lb_from.Visible = true;
            txt_from.Visible = true;
            lb_to.Visible = true;
            txt_to.Visible = true;
        }
        private void HideAllAddress()
        {
            lb_from.Visible = false;
            txt_from.Visible = false;
            lb_to.Visible = false;
            txt_to.Visible = false;
            txt_from.Text = string.Empty;
            txt_to.Text = string.Empty;
        }


        private void FrmSRMSend_Load(object sender, EventArgs e)
        {
            if (SCType == "cmdSendTask")
            {
                txtSCNum.Text = SCNum.M_SCNum;
                cmbCommandType.Focus();
                cmbCommandType.SelectedIndex = 0;
                btnSend.Text = "发送(&S)";
                c1DockingTab1.SelectedTab = c1DockingTabPage1;
                c1DockingTab1.TabPages.Remove(c1DockingTabPage2);
                c1DockingTab1.TabPages.Remove(c1DockingTabPage3);
            }
            if (SCType == "cmdTaskMode")
            {
                txtSCNO.Text = SCNum.M_SCNum;
                cmbTaskMode.Focus();
                cmbTaskMode.SelectedItem = SCNum.M_WCSTaskMode.ToString();
                c1DockingTab1.SelectedTab = c1DockingTabPage2;
                c1DockingTab1.TabPages.Remove(c1DockingTabPage1);
                c1DockingTab1.TabPages.Remove(c1DockingTabPage3);
                btnSend.Text = "修改(&M)";
            }
            if (SCType == "cmdChangeLoc")
            {
                txtSRMNO.Text = Num;
                txtSCTaskNu.Text = this.TaskNum;
                txtSCBarCode.Text = this.BarCode;
                txtOldLoc.Text = this.OldLocationNum;
                

                c1DockingTab1.SelectedTab = c1DockingTabPage3;
                c1DockingTab1.TabPages.Remove(c1DockingTabPage1);
                c1DockingTab1.TabPages.Remove(c1DockingTabPage2);
                btnSend.Text = "更换(&U)";
            }
        }

        private void SendTask(string scno, string taskno, string sourceAddress, string targetAddress, SRMCommand cmdType)
        {
            if (scbll.SendTaskByMan(scno, taskno,  sourceAddress, targetAddress, cmdType, false))
            {
                MessageBox.Show(string.Format("{0},手动指令{1}下发成功",scno,taskno));
            }
            else
            {
                MessageBox.Show(string.Format("{0},手动指令{1}下发失败！", scno, taskno));
            }
        }

        private void ChangeTaskMode(string scno, SCTaskMode taskmode, bool checkout)
        {
            if (!checkout)
            {
                if (MessageBox.Show("快速出库模式为慎用模式，请确认是否继续切换？", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            if (scbll.ChanageWCSTaskMode(scno, taskmode, checkout))
            {
                MessageBox.Show("修改任务模式成功");
            }
            else
            {
                MessageBox.Show("修改任务模式失败");
            }
        }

        private void cmbCommandType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCommandType.Text.Equals("清除指令"))
            {
                this.txt_taskno.Text = "1000";
                HideAllAddress();
            }
            if (cmbCommandType.Text.Equals("取货指令"))
            {
                ShowFromAddress();
            }
            if (cmbCommandType.Text.Equals("放货指令"))
            {
                ShowToAddress();
            }
            if (cmbCommandType.Text.Equals("移动指令"))
            {
                ShowToAddress();
            }
            if (cmbCommandType.Text.Equals("收回穿梭板指令"))
            {
                ShowToAddress();
            }
            if (cmbCommandType.Text.Equals("卸下穿梭板指令"))
            {
                ShowToAddress();
            }
        }
    }
}
