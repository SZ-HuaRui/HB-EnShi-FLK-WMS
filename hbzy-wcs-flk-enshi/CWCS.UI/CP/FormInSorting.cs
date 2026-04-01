using CWCS.Entity;
using CWCS.NetWork;
using CWCS.Service;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CWCS.UI
{
    public partial class FormInSorting : Form
    {
        public InSortingService service = new InSortingService();
        public delegate void  DelegateShowString(string message);
        public delegate void DelegateShowTable(DataTable dt);
        public delegate void DelegateShowDock(Dictionary<string,string> dic);
        private bool InitFinished = false;
        private int CurrentFloorIndex = -1;
        private bool ChangedEventActived = true;
        public FormInSorting()
        {
            InitializeComponent();
        }

        private void FormInSorting_Load(object sender, EventArgs e)
        {
            service.ShowWCSClientEvent += ShowTextWCSClient;
            service.ShowWCSServerEvent += ShowTextWCSServer;
            service.ShowConnectEvent += ShowConnect;
            service.ShowDockStatusEvent += ShowDockStatus;
            service.ShowRunStatus += ShowRunStatus;
            service.ShowSetDockTable += ShowSetDockTable;
            service.StartServer();
            this.gridControl1.DataSource = service.GetAllDockInfo();
            BindDockStautsEvent();
            BindFloor();
            InitFinished = true;
        }

        private void BindDockStautsEvent()
        {
            foreach (Control control in this.panelDockStatus.Controls)
            {
                if (control is LabelControl)
                {
                    LabelControl label = control as LabelControl;
                    label.Click += DockStatus_Click;
                    label.MouseEnter += handCursor_MouseEnter;
                }
            }
        }
        private void BindFloor()
        {
            string flag =  service.GetSwitchFloorValue();
            switch (flag)
            {
                case "0":this.radioGroup1.SelectedIndex = 0; break;
                case "1": this.radioGroup1.SelectedIndex = 2; break;
                case "2": this.radioGroup1.SelectedIndex = 1; break;
                default:break;
            }
        }

        private void handCursor_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void DockStatus_Click(object sender, EventArgs e)
        {
            try
            {
                LabelControl label =  sender as LabelControl;
                string dockDesription = string.Empty;
                string dockNO = string.Empty;
                FormUserCheck form = new FormUserCheck();
                form.ShowDialog();
                if (form.IsChecked)
                {
                    dockDesription = label.Text.Contains("正常") ? $"{label.Text.Replace("正常", "堵塞")}" : label.Text.Contains("堵塞") ? $"{label.Text.Replace("堵塞", "正常")}" : $"{label.Text.Replace("故障", "正常")}";
                    switch (label.Name)
                    {
                        case "lblDockStatus01": dockNO = "1"; break;
                        case "lblDockStatus02": dockNO = "2"; break;
                        case "lblDockStatus03": dockNO = "3"; break;
                        case "lblDockStatus04": dockNO = "4"; break;
                        case "lblDockStatus05": dockNO = "5"; break;
                        case "lblDockStatus06": dockNO = "6"; break;
                        case "lblDockStatus07": dockNO = "7"; break;
                        case "lblDockStatus08": dockNO = "8"; break;
                        case "lblDockStatus09": dockNO = "9"; break;
                        case "lblDockStatus10": dockNO = "10"; break;
                        case "lblDockStatus11": dockNO = "11"; break;
                        default: break;
                    }
                    if (StaticCommon.DockStatusDic.Keys.Contains(dockNO))
                    {
                        StaticCommon.DockStatusDic[dockNO] = dockDesription;
                        label.Text = dockDesription;
                        if (dockDesription.Contains("正常"))
                        {
                            label.BackColor = Color.Lime;
                        }
                        else
                        {
                            label.BackColor = Color.Red;
                        }
                    }
                }
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public void ShowTextWCSClient(string message)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                DelegateShowString callBack = new DelegateShowString(ShowTextWCSClient);
                this.richTextBox1.Invoke(callBack,new object[] {message});
            }
            else
            {
                this.richTextBox1.SelectionColor = Color.Black;
                if (message.Contains("失败") || message.Contains("剔除") || message.Contains("异常"))
                {
                    this.richTextBox1.SelectionColor = Color.Red;
                }
                this.richTextBox1?.AppendText(message);
                if (this.richTextBox1.TextLength > 5000)
                {
                    this.richTextBox1.Text = string.Empty;
                }
            }
        }

        public void ShowTextWCSServer(string message)
        {
            if (this.richTextBox2.InvokeRequired)
            {
                DelegateShowString callBack = new DelegateShowString(ShowTextWCSServer);
                this.richTextBox2.Invoke(callBack, new object[] { message });
            }
            else
            {
                this.richTextBox2.SelectionColor = Color.Black;
                if (message.Contains("失败") || message.Contains("剔除") || message.Contains("异常"))
                {
                    this.richTextBox2.SelectionColor = Color.Red;
                }
                this.richTextBox2?.AppendText(message);
                if (this.richTextBox2.TextLength > 5000)
                {
                    this.richTextBox2.Text = string.Empty;
                }

            }
        }

        public void ShowConnect(string message)
        {
            if (this.lblConnect.InvokeRequired)
            {
                DelegateShowString callBack = new DelegateShowString(ShowConnect);
                this.lblConnect.Invoke(callBack, new object[] { message });
            }
            else
            {
                if (message.Contains("正常"))
                {
                    this.lblConnect.Text = message;
                    this.lblConnect.BackColor = Color.Lime;
                }
                else
                {
                    this.lblConnect.Text = message;
                    this.lblConnect.BackColor = Color.Red;
                }
            }
        }

        public void ShowRunStatus(string message)
        {
            if (this.lblRunMode.InvokeRequired)
            {
                DelegateShowString callBack = new DelegateShowString(ShowRunStatus);
                this.lblRunMode.Invoke(callBack, new object[] { message });
            }
            else
            {
                if (message.Contains("自动"))
                {
                    this.lblRunMode.Text = message;
                    this.lblRunMode.BackColor = Color.Lime;
                }
                else
                {
                    this.lblRunMode.Text = message;
                    this.lblRunMode.BackColor = Color.Red;
                }
            }
        }

        public void ShowDockStatus(Dictionary<string,string> dic)
        {
            if (this.panelDockStatus.InvokeRequired)
            {
                DelegateShowDock callBack = new DelegateShowDock(ShowDockStatus);
                this.panelDockStatus.Invoke(callBack, new object[] { dic });
            }
            else
            {
                foreach (KeyValuePair<string,string> pair in dic)
                {
                    switch (pair.Key)
                    {
                        case "1": this.lblDockStatus01.Text = pair.Value; this.lblDockStatus01.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "2": this.lblDockStatus02.Text = pair.Value; this.lblDockStatus02.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "3": this.lblDockStatus03.Text = pair.Value; this.lblDockStatus03.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "4": this.lblDockStatus04.Text = pair.Value; this.lblDockStatus04.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "5": this.lblDockStatus05.Text = pair.Value; this.lblDockStatus05.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "6": this.lblDockStatus06.Text = pair.Value; this.lblDockStatus06.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "7": this.lblDockStatus07.Text = pair.Value; this.lblDockStatus07.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "8": this.lblDockStatus08.Text = pair.Value; this.lblDockStatus08.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "9": this.lblDockStatus09.Text = pair.Value; this.lblDockStatus09.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "10":this.lblDockStatus10.Text = pair.Value; this.lblDockStatus10.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        case "11":this.lblDockStatus11.Text = pair.Value; this.lblDockStatus11.BackColor = pair.Value.Contains("正常") ? Color.Lime : Color.Red; break;
                        default:break;
                    }
                }
            }
        }

        public void ShowSetDockTable(DataTable dt)
        {
            if (this.gridControl1.InvokeRequired)
            {
                DelegateShowTable callBack = new DelegateShowTable(ShowSetDockTable);
                this.gridControl1.Invoke(callBack, new object[] { dt });
            }
            else
            {
                if (dt != null)
                {
                    this.gridControl1.DataSource = dt;
                }
            }
        }


        private void btnReflash_Click(object sender, EventArgs e)
        {
            this.gridControl1.DataSource = service.GetAllDockInfo();
        }

        #region 导致界面变卡，不使用 
        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            this.gridView1.FormatConditions.Add(setCellColorCondition(this.gridView1.Columns["ISWCSCLEAN"], Color.Lime, "ISWCSCLEAN = '清道完成'"));
            this.gridView1.FormatConditions.Add(setCellColorCondition(this.gridView1.Columns["ISWCSCLEAN"], Color.Orange, "ISWCSCLEAN = '清道中'"));
            this.gridView1.FormatConditions.Add(setCellColorCondition(this.gridView1.Columns["PLCRESPONSE"], Color.Red, "PLCRESPONSE = '失败'"));
            this.gridView1.FormatConditions.Add(setCellColorCondition(this.gridView1.Columns["PLCRESPONSE"], Color.Lime, "PLCRESPONSE = '成功'"));
        }

        public StyleFormatCondition setCellColorCondition(GridColumn column,Color color,string express)
        {

            StyleFormatCondition condition = new StyleFormatCondition();
            condition.Column = column;
            condition.Appearance.ForeColor = color;
            condition.Appearance.Options.UseForeColor = true;
            condition.Appearance.Options.UseTextOptions = true;
            condition.Condition = FormatConditionEnum.Expression;
            condition.Expression = express;
            return condition;
        }
        #endregion

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GridHitInfo info = this.gridView1.CalcHitInfo(e.X,e.Y);
                if (info != null && info.InRow && info.Column != null && info.Column.Name == "colClean")
                {
                    string dockNO = this.gridView1.GetRowCellValue(info.RowHandle, "DOCKNO").ToString();
                    string stationNO = this.gridView1.GetRowCellValue(info.RowHandle, "STATIONNO").ToString();
                    if (MessageBox.Show($"确定对道口【{dockNO}】清道吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        if (!service.CleanDockToSorting(dockNO))
                        {
                            MessageBox.Show($"清道失败！\n原因：道口{dockNO}，下发清道指令给分拣线失败！");
                            return;
                        }
                        if (!service.SetCleanFlag(dockNO, 0))
                        {
                            MessageBox.Show($"清道指令下发失败！\n原因：道口{dockNO}，设置WMS清道标记失败！");
                            return;
                        }
                        service.SetPLCFlag(dockNO,0);
                        MessageBox.Show("清道指令下发成功", "提示");
                        this.gridControl1.DataSource = service.GetAllDockInfo();
                    }
                }
                if (info != null && info.InRow && info.Column != null && info.Column.Name == "colSetDock")
                {
                    string dockNO = this.gridView1.GetRowCellValue(info.RowHandle, "DOCKNO").ToString();
                    string cleanFlag = this.gridView1.GetRowCellValue(info.RowHandle, "ISWCSCLEAN").ToString();
                    string plcFlag = this.gridView1.GetRowCellValue(info.RowHandle, "PLCRESPONSE").ToString();
                    string itemCode = this.gridView1.GetRowCellValue(info.RowHandle, "ITEMCODE").ToString();
                    if (cleanFlag.Equals("设道完成"))
                    {
                        MessageBox.Show("请先清道，再设道！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        return;
                    }
                    if (cleanFlag.Equals("清道中") && !(dockNO.Equals("4") || dockNO.Equals("5")))
                    {
                        MessageBox.Show("当前状态不是【清道完成】，无法设道！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                       
                    }
                    FormSetDock from = new FormSetDock(dockNO, itemCode);
                    from.ReflashTable += ShowSetDockTable;
                    from.ShowDialog();
                }
            }
        }

        private void btnRequestSystemStutas_Click(object sender, EventArgs e)
        {
            service.RequestSystemStatus();
        }

        private void btnCleanAll_Click(object sender, EventArgs e)
        {
            try
            {
                FormUserCheck form = new FormUserCheck();
                form.ShowDialog();
                if (form.IsChecked)
                {
                    service.InsertWcsLog($"{form.UserName},清除所有道口");
                    service.CleanAllDock();
                    this.gridControl1.DataSource = service.GetAllDockInfo();
                }
            }
            catch (Exception ex)
            {
                this.richTextBox2.AppendText($"清道出现异常！{ex.Message},{ex.StackTrace}");
            }
        }

        private void toggleSwitch1_EditValueChanged(object sender, EventArgs e)
        {
            if (this.toggleSwitch1.IsOn)
            {
                this.radioGroup1.Enabled = true;
                this.toggleSwitch1.ForeColor = Color.Lime;
            }
            else
            {
                this.radioGroup1.Enabled = false;
                this.toggleSwitch1.ForeColor = Color.Black;
            }

        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (InitFinished && ChangedEventActived)
                {

                    string text = this.radioGroup1.Properties.Items[this.radioGroup1.SelectedIndex].Description;
                    //1：二楼禁用；2：三楼禁用
                    string flag = text.Contains("全部") ? "0" : text.Contains("二楼") ? "2" : "1";
                    if (!service.SwitchFloor(flag))
                    {
                        MessageBox.Show("楼层切换失败！", "提示", MessageBoxButtons.OK,MessageBoxIcon.Information);
                        ChangedEventActived = false;
                        this.radioGroup1.SelectedIndex = CurrentFloorIndex;
                        return;
                    }
                    MessageBox.Show("楼层切换成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {

            }
            
           

        }

        private void radioGroup1_MouseEnter(object sender, EventArgs e)
        {
            CurrentFloorIndex = radioGroup1.SelectedIndex;
            ChangedEventActived = true;
        }
    }
}
