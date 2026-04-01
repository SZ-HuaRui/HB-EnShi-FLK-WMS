using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using C1.Win.C1Ribbon;
using BLL;
using C1.Win.C1FlexGrid;
using CWCS.UI;
using CWCS.Repository;

namespace WCS
{
    public partial class FrmTaskManager : C1RibbonForm
    {
        private int pageSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["pagesize"]);    //每页显示行数
        private int nMax = 0;         //总记录数
        private int pageCount = 1;    //页数＝总记录数/每页显示行数
        private int pageCurrent = 1;   //当前页号
        private int nCurrent = 0;      //当前记录行
        private DataSet ds = new DataSet();
        private DataTable dtInfo = new DataTable();
        private DataTable dtInfo2 = new DataTable();
        public OracleRepository _repository = new OracleRepository();

        public FrmTaskManager()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            BindData(1);
        }

        private void FrmTaskManager_Load(object sender, EventArgs e)
        {
            dtp_updatestart.Value = DateTime.ParseExact("00:00:00", "HH:mm:ss", null);
            dtp_updateend.Value = DateTime.ParseExact("23:59:59", "HH:mm:ss", null);
            BindData(1);
        }
        private void ScanTask(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (!rb_glbtask.Checked && !rb_agv.Checked && rb_excuting.Checked)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        BindData(1);
                    }));

                }
            }
            catch
            {
            }
        }
        private void BindData(int load)
        {
            if (load == 1)
            {
                int tasktype, taskstatus;
                if (rb_glbtask.Checked)
                    tasktype = 0;
                else if (rb_sta.Checked)
                    tasktype = 1;
                else if (rb_sc.Checked)
                    tasktype = 2;
                else if (rb_agv.Checked)
                    tasktype = 3;
                else
                    tasktype = 0;


                if (rb_unstart.Checked)
                    taskstatus = 0;
                else if (rb_excuting.Checked)
                    taskstatus = 2;
                else
                    taskstatus = 1;

                dtInfo = _repository.GetTaskInfo(tasktype, taskstatus, dtp_updatestart.Value.ToString(), dtp_updateend.Value.ToString().Trim(), txt_wmstaskno.Text.Trim(), txt_taskno.Text.Trim(), txt_fromstation.Text.Trim(), txt_tostation.Text.Trim(), txt_barcode.Text.Trim(), "");

                if (dtInfo != null)
                {
                    nMax = dtInfo.Rows.Count;
                    pageCount = (nMax / pageSize);
                    if ((nMax % pageSize) > 0)
                    { pageCount++; }
                    pageCurrent = 1;
                    nCurrent = 0;
                }

            }
            if (dtInfo != null)
            {
                int nStartPos = 1;
                int nEndPos = 0;
                DataTable dtTemp = dtInfo.Clone();
                if (nMax > 0)
                {
                    if (pageCurrent == pageCount)
                    {
                        nEndPos = nMax;
                    }
                    else
                    {
                        nEndPos = pageSize * pageCurrent;
                    }
                    txtCurrentPage.Text = Convert.ToString(pageCurrent);
                }
                else
                {
                    nEndPos = 0;
                    txtCurrentPage.Text = "0";
                }
                nCurrent = pageSize * (pageCurrent - 1);
                nStartPos = nCurrent;
                lblPageCount.Text = "/ " + pageCount.ToString();

                //从元数据源复制记录行
                for (int i = nStartPos; i < nEndPos; i++)
                {
                    dtTemp.ImportRow(dtInfo.Rows[i]);
                    nCurrent++;
                }
                bindingNavigatorCountItem.Text = "/ " + nCurrent;
                dtTemp.TableName = "Test";
                bdsInfo.DataSource = dtTemp;
                //bdnInfo.BindingSource = bdsInfo;
                //gv_taskmanager.SetDataBinding(dtTemp, "Test");
                gv_taskmanager.AutoGenerateColumns = false;
                gv_taskmanager.DataSource = dtTemp;
                gv_taskmanager.Refresh();
                SortSerial();
            }
            else
            {
                gv_taskmanager.DataSource = dtInfo;
                SortSerial();
            }
        }

        private void gv_taskmanager_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = this.gv_taskmanager.RowSel;
                if (this.gv_taskmanager.RowSel > 0)
                {
                    c1ContextMenu1.ShowContextMenu(gv_taskmanager, new Point(e.X, e.Y));
                }
            }
        }

        private void c1CommandHolder1_CommandClick(object sender, C1.Win.C1Command.CommandClickEventArgs e)
        {
            if (e.Command.Name == "c1Command2")
            {
                if (c1DockingTab1.SelectedIndex == 0)
                {
                    if (gv_taskmanager.Rows.Selected.Count <= 0)
                        MessageBox.Show("请选择行");
                    if (rb_unstart.Checked || rb_excuting.Checked)
                    {

                        if (rb_glbtask.Checked)
                        {
                            MessageBox.Show("请确认是否需要对主任务进行修改!", "警告！");
                        }
                        if (gv_taskmanager.Rows[gv_taskmanager.Row].DataSource == null)
                            return;
                        DataRow row = ((DataRowView)gv_taskmanager.Rows[gv_taskmanager.Row].DataSource).Row;
                        if (row == null)
                            return;
                        string tasktype = row["tasktype"].ToString().Trim();
                        string barcode = row["barcode"].ToString().Trim();
                        string taskno = row["taskno"].ToString().Trim();
                        string taskstatus = row["taskstatus"].ToString().Trim();
                        string runstatus = row["runstatus"].ToString().Trim();
                        if (rb_glbtask.Checked)
                            tasktype = "GLB";
                        else if (rb_sta.Checked)
                            tasktype = "STA";
                        //else if (rb_sc.Checked)
                        //    tasktype = "SC";
                        else if (rb_agv.Checked)
                            tasktype = "AGV";
                        FrmTaskEdit formtaskedit = new FrmTaskEdit();
                        formtaskedit.ShowDialog();
                        if (FrmTaskEdit.ischange)
                        {
                            BindData(1);
                            FrmTaskEdit.ischange = false;
                        }
                    }
                }
            }
            else if (e.Command.Name == "c1Command1")
            {
                if (c1DockingTab1.SelectedIndex == 0)
                {
                    DataRow row = ((DataRowView)gv_taskmanager.Rows[gv_taskmanager.Row].DataSource).Row;
                    Clipboard.SetDataObject(this.gv_taskmanager[gv_taskmanager.Row, gv_taskmanager.Col].ToString());
                }
            }
            else if (e.Command.Name == "c1Command3")
            {
                if (!rb_glbtask.Checked)
                {
                    MessageBox.Show($"请选择主任务进行回收！", "提示");
                    return;
                }
                if (MessageBox.Show("确定回收此任务？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    DataRow dr = ((DataRowView)gv_taskmanager.Rows[gv_taskmanager.Row].DataSource).Row;
                    string taskNO = dr["TASKNO"].ToString().Trim();
                    Dictionary<bool, string> dic = new Dictionary<bool, string>();
                    //Dictionary<bool,string> dic = dal.RollbackTask(taskNO);
                    if (dic.Keys.Contains(false))
                    {
                        MessageBox.Show($"回收失败!\n{dic[false].ToString()}", "提示");
                        return;
                    }
                    MessageBox.Show($"回收成功！", "提示");
                    BindData(1);
                }

            }
        }

        private void rb_glbtask_CheckedChanged(object sender, EventArgs e)
        {
            BindData(1);
        }

        private void bdnInfo_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (pageCount > 1)
                {
                    if (e.ClickedItem.Text == "上一页")
                    {
                        pageCurrent--;
                        if (pageCurrent <= 0)
                        {
                            pageCurrent++;
                            return;
                        }
                        else
                        {

                            BindData(0);
                        }
                    }
                    if (e.ClickedItem.Text == "下一页")
                    {
                        pageCurrent++;
                        if (pageCurrent > pageCount)
                        {
                            pageCurrent--;
                            return;
                        }
                        else
                        {
                            BindData(0);
                        }
                    }
                    if (e.ClickedItem.Text == "首页")
                    {
                        pageCurrent = 1;

                        BindData(0);
                    }
                    if (e.ClickedItem.Text == "末页")
                    {
                        pageCurrent = pageCount;
                        BindData(0);
                    }
                }
                if (e.ClickedItem.Text == "刷新")
                {
                    BindData(1);
                }
            }
            catch { }
        }

        public void SortSerial()
        {
            try
            {
                for (int i = 1; i < gv_taskmanager.Rows.Count; i++)
                {
                    gv_taskmanager.Rows[i]["Serial"] = i;

                }
            }
            catch
            {
            }
        }

        //private void toolStripButton7_Click(object sender, EventArgs e)
        //{
        //    SaveFileDialog saveFile = new SaveFileDialog();
        //    saveFile.FileName = "任务列表" + DateTime.Now.ToString("yyyyMMddHHmmss");
        //    saveFile.Filter = "Excel文件|*.xls|Excel文件|*.xlsx";
        //    saveFile.FilterIndex = 1;
        //    if (saveFile.ShowDialog() == DialogResult.Cancel)
        //    {
        //        return;
        //    }
        //    string fileName = saveFile.FileName;
        //    string ext = saveFile.FilterIndex == 1 ? ".xls" : ".xlsx";  //扩展名
        //    System.Diagnostics.Stopwatch timeWatch = System.Diagnostics.Stopwatch.StartNew();
        //    NPOIExcelHelper.DataTableToExcel(dtInfo, "完成任务信息", fileName);
        //    timeWatch.Stop();
        //    MessageBox.Show("总共耗时" + timeWatch.Elapsed);
        //}

    }
}
