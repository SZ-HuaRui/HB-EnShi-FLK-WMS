using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using CWCS.Repository;
using DataGridViewAutoFilter;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Configuration;

namespace WCS
{
    public partial class FormAGVEquip : Form
    {
        private OracleRepository _repository = new OracleRepository();
        private BLLAGV agv;
        private delegate void RichBoxCallBack(string msg);
        private delegate void GridViewCallBack(DataTable dt);
        private string AgvNo = string.Empty;
     
        public FormAGVEquip(string AgvNo,string Ip,string Port)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            this.AgvNo = AgvNo;
            agv = new BLLAGV(AgvNo,Ip,Port);
            if (AgvNo.Contains("1"))
            {
                txtLayer.Text = "1";
                txtLayer.Enabled = false;
            }
        }

        private void FormAGVEquip_Load(object sender, EventArgs e)
        {
            try
            {
                agv.GetConnectStatus += new BLLAGV.Sys_conn(agv_GetConnectStatus);
                agv.GetTask += new BLLAGV.NotifyTask(agv_GetTask);
                agv.AgvRun();
            }
            catch (Exception ex)
            {
                agv.WriteLog(ex.StackTrace + ex.Source + ex.TargetSite);
            }
        }
        
        private void agv_GetConnectStatus(string command, object state)
        {
            if (command == "Connect")
            {
                lb_connect.Text = $"{AgvNo}联机";
                lb_connect.BackColor = Color.Lime;
            }
            else if (command == "Break")
            {
                lb_connect.Text = $"{AgvNo}脱机";
                lb_connect.BackColor = Color.Gray;
            }
            else if (command == "Show")
            {
                ShowText((string)state);
            }
        }
        private void ShowText(string msg)
        {
            try
            {
                if (rtb_info.InvokeRequired)
                {
                    RichBoxCallBack rb = new RichBoxCallBack(ShowText);
                    this.Invoke(rb, new object[] { msg });
                }
                else
                {
                    if (rtb_info.TextLength >= 5000)
                        rtb_info.Clear();
                    if (!System.String.IsNullOrEmpty(msg))
                        rtb_info.AppendText(System.DateTime.Now + ":"+msg + "\r\n");
                }
            }
            catch (Exception ex)
            {
                agv.WriteLog(ex.StackTrace + ex.Source + ex.TargetSite);
            }
        }
        private void agv_GetTask(DataTable dt)
        {
            ShowData(dt);
            btn_auto.BackColor = (btn_auto.BackColor == Color.Lime ? Color.Silver : Color.Lime);
        }

        private void ShowData(DataTable dt)
        {
            if (gv_taskmanager.InvokeRequired)
            {
                GridViewCallBack gvcb = new GridViewCallBack(ShowData);
                this.Invoke(gvcb, new object[] { dt });
            }
            else
            {
                bdsInfo.DataSource = dt;
                gv_taskmanager.DataSource = bdsInfo;
                gv_taskmanager.Refresh();
            }
            GC.Collect();
        }


        //private void gv_taskmanager_BindingContextChanged(object sender, EventArgs e)
        //{
        //    if (gv_taskmanager.DataSource == null)
        //    { return; }
        //    foreach (DataGridViewColumn col in gv_taskmanager.Columns)
        //    {
        //        col.HeaderCell = new DataGridViewAutoFilterColumnHeaderCell(col.HeaderCell);
        //    }
        //    gv_taskmanager.AutoResizeColumns();
        //}

        //private void gv_taskmanager_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        //{
        //    if (e.RowIndex == -1)
        //    {
        //        bool mouseOver = e.CellBounds.Contains(gv_taskmanager.PointToClient(Cursor.Position));
        //        LinearGradientBrush brush = new LinearGradientBrush(
        //           e.CellBounds,
        //           mouseOver ? Color.PeachPuff : Color.LightGray,
        //           Color.LightBlue,
        //           LinearGradientMode.Vertical);

        //        using (brush)
        //        {
        //            e.Graphics.FillRectangle(brush, e.CellBounds);
        //            Rectangle border = e.CellBounds;
        //            border.Width -= 1;
        //            e.Graphics.DrawRectangle(Pens.Gray, border);
                    
        //        }

        //        e.PaintContent(e.CellBounds);
        //        e.Handled = true;
        //        if (e.Graphics != null)
        //            e.Graphics.Dispose();
        //    }
        //}

        //private void gv_taskmanager_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        //        e.RowBounds.Location.Y,
        //        gv_taskmanager.RowHeadersWidth - 4,
        //        e.RowBounds.Height);

        //    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
        //        gv_taskmanager.RowHeadersDefaultCellStyle.Font,
        //        rectangle,
        //        gv_taskmanager.RowHeadersDefaultCellStyle.ForeColor,
        //        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        //    if (e.Graphics != null)
        //        e.Graphics.Dispose();
        //    if (gv_taskmanager.RowHeadersDefaultCellStyle.Font != null)
        //        gv_taskmanager.RowHeadersDefaultCellStyle.Font.Dispose();
        //}

        private void btn_auto_Click(object sender, EventArgs e)
        {
            if (btn_auto.Text == "自动(&A)")
            {
                btn_auto.Text = "手动(&H)";
                btn_auto.BackColor = Color.Silver;
                agv.SetTimer(true);
            }
            else
            {
                btn_auto.Text = "自动(&A)";
                agv.SetTimer(true);
            }
        }

        //private void btn_sendcode_Click(object sender, EventArgs e)
        //{
        //    if (MessageBox.Show("确定要修改小车任务" + txt_AGVIndex.Text + "吗?", "修改", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //    {
        //        if (dal.Upadte_Agv_Status(0, txt_AGVIndex.Text, 3, "0", 0))
        //            MessageBox.Show("修改成功!", "提示");
        //        else
        //            MessageBox.Show("修改失败!", "提示");
        //    }
        //}

        private void sendtask_Click(object sender, EventArgs e)
        {
            if (agv.agvclass.SendTask(int.Parse(txt_AGVIndex.Text), int.Parse(txt_start.Text), int.Parse(txt_end.Text),string.Empty))
                MessageBox.Show("发送AGV任务成功!", "提示");
            
            else
                MessageBox.Show("发送AGV任务失败!", "提示");


           
        }

        private void txtSendLayer_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtAGVIndex.Text) || string.IsNullOrEmpty(this.txtSourceAddress.Text) || string.IsNullOrEmpty(this.txtTargetAddress.Text) || string.IsNullOrEmpty(this.txtLayer.Text))
                {
                    MessageBox.Show("信息不全，请填写完整！", "提示");
                    return;
                }
                if (agv.agvclass.SendmMessage_UpdateLayer_Manual(Convert.ToInt32(this.txtAGVIndex.Text), Convert.ToInt32(this.txtSourceAddress.Text), Convert.ToInt32(this.txtTargetAddress.Text), Convert.ToInt32(this.txtLayer.Text)))
                {
                    MessageBox.Show("发送成功！", "提示");
                    return;
                }
                MessageBox.Show("发送失败！", "提示");
            }
            catch(Exception ex)
            {
                MessageBox.Show("发送失败！", "提示");
            }
            
        }
    }
}
