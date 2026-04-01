using BLL;
using CWCS.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWCS.UI
{
    public partial class FormAGVManager : Form
    {
        private OracleRepository _repository = new OracleRepository();
        private BLLAGV agv;
        private delegate void RichBoxCallBack(string msg);
        private delegate void GridViewCallBack(DataTable dt);
        private string AgvNo = string.Empty;

        public FormAGVManager(string AgvNo, string Ip, string Port)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            this.AgvNo = AgvNo;
            agv = new BLLAGV(AgvNo, Ip, Port);
            if (AgvNo.Contains("1"))
            {
                txtLayer.Text = "1";
                txtLayer.Enabled = false;
            }
        }

        private void FormAGVManager_Load(object sender, EventArgs e)
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
                btnConnect.Text = $"         {AgvNo}联机";
                btnConnect.BackColor = Color.Lime;
            }
            else if (command == "Break")
            {
                btnConnect.Text = $"         {AgvNo}脱机";
                btnConnect.BackColor = Color.Red;
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
                if (rtbCommunication.InvokeRequired)
                {
                    RichBoxCallBack rb = new RichBoxCallBack(ShowText);
                    this.Invoke(rb, new object[] { msg });
                }
                else
                {
                    if (rtbCommunication.TextLength >= 5000)
                        rtbCommunication.Clear();
                    if (!string.IsNullOrEmpty(msg))
                        rtbCommunication.AppendText(DateTime.Now + ":" + msg + "\r\n");
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
        }

        private void ShowData(DataTable dt)
        {
            if (gridControl1.InvokeRequired)
            {
                GridViewCallBack gvcb = new GridViewCallBack(ShowData);
                this.Invoke(gvcb, new object[] { dt });
            }
            else
            {
                gridControl1.DataSource = dt;
            }
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            if (btnAuto.Text == "   自动")
            {
                btnAuto.Text = "   手动";
                btnAuto.BackColor = Color.Red;
                agv.SetTimer(false);
            }
            else
            {
                btnAuto.Text = "   自动";
                btnAuto.BackColor = Color.Lime;
                agv.SetTimer(true);
            }
        }

       

        private void btnSend1_Click(object sender, EventArgs e)
        {
            try
            {
                Random random = new Random();
                if (agv.agvclass.SendTask(random.Next(1, 9999), int.Parse(txtFromAddress1.Text), int.Parse(txtToAddress1.Text), string.Empty))
                    MessageBox.Show("发送AGV任务成功!", "提示");

                else
                    MessageBox.Show("发送AGV任务失败!", "提示");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"发送AGV任务失败!{ex.Message}", "提示");
            }
          
        }

        private void btnSend2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtAGVIndex.Text) || string.IsNullOrEmpty(txtFromAddress2.Text) || string.IsNullOrEmpty(txtToAddress2.Text) || string.IsNullOrEmpty(txtLayer.Text))
                {
                    MessageBox.Show("信息不全，请填写完整！", "提示");
                    return;
                }
                if (agv.agvclass.SendmMessage_UpdateLayer_Manual(Convert.ToInt32(txtAGVIndex.Text), Convert.ToInt32(txtFromAddress2.Text), Convert.ToInt32(txtToAddress2.Text), Convert.ToInt32(txtLayer.Text)))
                {
                    MessageBox.Show("发送成功！", "提示");
                    return;
                }
                MessageBox.Show("发送失败！", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送失败！", "提示");
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = $"{e.RowHandle + 1}";
            }
        }

       
    }
}
