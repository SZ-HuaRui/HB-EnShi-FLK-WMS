using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using C1.Win.C1Command;
using CWCS.Common.Uitl;
using CWCS.Entity;
using CWCS.Repository;
using DevExpress.Services.Implementation;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace CWCS.UI
{
    public partial class FormTaskManager : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormTaskManager()
        {
            InitializeComponent();
            Initialize();
            InitializeEvents();
        }

        private void Initialize()
        {
            dateStartTime.Text = $"{DateTime.Now:yyyy/MM/dd} 08:00:00";
            dateEndTime.Text = $"{DateTime.Now.AddMonths(1):yyyy/MM/dd} 23:59:59";
            rbMainTask.Checked = true;
            rbTaskNoStart.Checked = true;
        }

        private void InitializeEvents()
        {
            rbMainTask.CheckedChanged += radioBox_CheckedChanged;
            rbConveyorTask.CheckedChanged += radioBox_CheckedChanged;
            rbAgvTask.CheckedChanged += radioBox_CheckedChanged;
            rbScTask.CheckedChanged += radioBox_CheckedChanged;
            rbTaskNoStart.CheckedChanged += radioBox_CheckedChanged;
            rbTaskExecuting.CheckedChanged += radioBox_CheckedChanged;
            rbTaskFinished.CheckedChanged += radioBox_CheckedChanged;
            gridView1.KeyDown += new KeyEventHandler(this.gridView1_KeyDown);

        }

       
        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Point point = this.gridView1.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo info = this.gridView1.CalcHitInfo(point);
                if (info.InRow && info.InRowCell)
                {
                    if (gridView1.FocusedValue != null)
                    {
                        Clipboard.SetText(gridView1.FocusedValue.ToString());
                    }
                }
             
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string wcsTaskType = "MAIN";
            string taskStatus = "0";
            //任务状态
            if (rbMainTask.Checked)
            {
                wcsTaskType = "MAIN";
            }
            if (rbConveyorTask.Checked)
            {
                wcsTaskType = "STA";
            }
            if (rbScTask.Checked)
            {
                wcsTaskType = "SC";
            }
            if (rbAgvTask.Checked)
            {
                wcsTaskType = "AGV";
            }
            //执行状态
            if (rbTaskNoStart.Checked)
            {
                taskStatus = "0";
            }
            if (rbTaskExecuting.Checked)
            {
                taskStatus = "2";
            }
            if (rbTaskFinished.Checked)
            {
                taskStatus = "1";
            }
            gridControl1.DataSource = _repository.GetTaskInfoView(string.Empty,
                                                          wcsTaskType,
                                                          taskStatus,
                                                          dateStartTime.Text,
                                                          dateEndTime.Text,
                                                          txtWcsTaskNo.Text.Trim(),
                                                          txtWmsTaskNo.Text.Trim(),
                                                          txtFromAddress.Text.Trim(),
                                                          txtToAddress.Text.Trim(),
                                                          txtBarcode.Text.Trim());
        }

        private void radioBox_CheckedChanged(object sender, EventArgs e)
        {
            btnSelect_Click(sender,e);
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = $"{e.RowHandle+1}";
            }
        }

      

        private void btnFinishMany_Click(object sender, EventArgs e)
        {
           
            string taskType = string.Empty;

            if (rbMainTask.Checked)
            {
                taskType = "MAIN";
            }
            if (rbConveyorTask.Checked)
            {
                taskType = "STA";
            }
            if (rbScTask.Checked)
            {
                taskType = "SC";
            }
            if (rbAgvTask.Checked)
            {
                taskType = "AGV";
            }
           
            if (taskType.Equals("MAIN"))
            {
                MessageBox.Show("不允许直接完成主任务","提示");
                return;
            }
            int[] rowArray = gridView1.GetSelectedRows();
            if (rowArray == null || rowArray.Length == 0)
            {
                MessageBox.Show("请选择至少一行数据","提示");
                return;
            }

            FormUserCheck userCheck = new FormUserCheck();
            userCheck.ShowDialog();
            if (userCheck.IsChecked)
            {
                foreach (int rowHandle in rowArray)
                {
                    _repository.DealTaskStatus(
                    new WCS_SUBTASKINFO()
                    {
                        TASKNO = Convert.ToInt32(gridView1.GetRowCellValue(rowHandle, "TASKNO").ToString()),
                        EQUIPMENTTYPE = taskType,
                        FROMSTATION = gridView1.GetRowCellValue(rowHandle, "FROMSTATION").ToString()
                    },
                    new WCS_SUBTASKINFO()
                    {
                        RUNSTATUS = "1",
                        TASKSTATUS = "1"
                    }
                    );
                }
                btnSelect_Click(sender, e);
            }
            


        }

        private void btnEditTask_Click(object sender, EventArgs e)
        {
            int[] rows = gridView1.GetSelectedRows();
            if (rows == null || rows.Length != 1)
            {
                MessageBox.Show("请选择一行数据", "提示");
                return;
            }
            FormTaskEdit formTaskEdit = new FormTaskEdit();
            formTaskEdit.RefreshData = btnSelect_Click;
            int rowHandle = rows[0];
            formTaskEdit.txtTaskNo.Text = gridView1.GetRowCellValue(rowHandle, "TASKNO").ToString();
            formTaskEdit.txtBarcode.Text = gridView1.GetRowCellValue(rowHandle, "BARCODE").ToString();
            formTaskEdit.txtFromAddress.Text = gridView1.GetRowCellValue(rowHandle, "FROMSTATION").ToString();
            formTaskEdit.txtToAddress.Text = gridView1.GetRowCellValue(rowHandle, "TOSTATION").ToString();
            formTaskEdit.cmbRunStatus.Text = gridView1.GetRowCellValue(rowHandle, "RUNSTATUS").ToString();
            formTaskEdit.cmbTaskStatus.Text = gridView1.GetRowCellValue(rowHandle, "TASKSTATUS").ToString();
            string taskType = gridView1.GetRowCellValue(rowHandle, "EQUIPMENTTYPE").ToString();
            if (!string.IsNullOrEmpty(taskType))
            {
                formTaskEdit.TaskType = taskType;
            }
            else
            {
                if (rbMainTask.Checked)
                {
                    taskType = "MAIN";
                }
                if (rbConveyorTask.Checked)
                {
                    taskType = "STA";
                }
                if (rbScTask.Checked)
                {
                    taskType = "SC";
                }
                if (rbAgvTask.Checked)
                {
                    taskType = "AGV";
                }
                formTaskEdit.TaskType = taskType;
            }
            if (taskType.Equals("MAIN"))
            {
                if (MessageBox.Show("确定修改【主任务】吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                {
                    return;
                }
                formTaskEdit.cmbRunStatus.ReadOnly = true;
            }
            formTaskEdit.ShowDialog();
        }

        /// <summary>
        /// 主任务批量未开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNoStartMany_Click(object sender, EventArgs e)
        {
            if (!rbMainTask.Checked)
            {
                MessageBox.Show("只允许批量未开始主任务", "提示");
                return;
            }
            string taskType = "MAIN";
            int[] rowArray = gridView1.GetSelectedRows();
            if (rowArray == null || rowArray.Length == 0)
            {
                MessageBox.Show("请选择至少一行数据", "提示");
                return;
            }
            FormUserCheck userCheck = new FormUserCheck();
            userCheck.ShowDialog();
            if (userCheck.IsChecked)
            {
                foreach (int rowHandle in rowArray)
                {
                    _repository.DealTaskStatus(
                    new WCS_SUBTASKINFO()
                    {
                        TASKNO = Convert.ToInt32(gridView1.GetRowCellValue(rowHandle, "TASKNO").ToString()),
                        EQUIPMENTTYPE = taskType,
                        FROMSTATION = gridView1.GetRowCellValue(rowHandle, "FROMSTATION").ToString()
                    },
                    new WCS_SUBTASKINFO()
                    {
                        RUNSTATUS = "1",
                        TASKSTATUS = "0"
                    }
                    );
                }
                btnSelect_Click(sender, e);
            }
        }

        /// <summary>
        /// 子任务批量未开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubNoStartMany_Click(object sender, EventArgs e)
        {
            if (rbMainTask.Checked)
            {
                MessageBox.Show("不允许批量未开始主任务", "提示");
                return;
            }
            int[] rowArray = gridView1.GetSelectedRows();
            if (rowArray == null || rowArray.Length == 0)
            {
                MessageBox.Show("请选择至少一行数据", "提示");
                return;
            }
            FormUserCheck userCheck = new FormUserCheck();
            userCheck.ShowDialog();
            if (userCheck.IsChecked)
            {
                foreach (int rowHandle in rowArray)
                {
                    _repository.DealTaskStatus(
                    new WCS_SUBTASKINFO()
                    {
                        TASKNO = Convert.ToInt32(gridView1.GetRowCellValue(rowHandle, "TASKNO").ToString()),
                        EQUIPMENTTYPE = gridView1.GetRowCellValue(rowHandle, "EQUIPMENTTYPE").ToString(),
                        FROMSTATION = gridView1.GetRowCellValue(rowHandle, "FROMSTATION").ToString()
                    },
                    new WCS_SUBTASKINFO()
                    {
                        TASKSTATUS = "0"
                    }
                    );
                }
                btnSelect_Click(sender, e);
            }

        }

        private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (rbAgvTask.Checked)
            {
                int[] rowCount = this.gridView1.GetSelectedRows();
                if (rowCount.Length != 1) return;
                string taskNO = this.gridView1.GetRowCellValue(rowCount[0], "TASKNO").ToString();
                string fromStation = this.gridView1.GetRowCellValue(rowCount[0], "FROMSTATION").ToString();
                string equipmentType = this.gridView1.GetRowCellValue(rowCount[0], "EQUIPMENTTYPE").ToString();
                string AGVIndex = this.gridView1.GetRowCellValue(rowCount[0], "AGVINDEX").ToString();
                string AGVCarNO = this.gridView1.GetRowCellValue(rowCount[0], "AGVCARNO").ToString();

                FormUserCheck userCheck = new FormUserCheck();
                userCheck.ShowDialog();
                if (userCheck.IsChecked)
                {
                    if (!string.IsNullOrEmpty(taskNO))
                    {
                        if (!_repository.UpdateTableData(
                            new WCS_SUBTASKINFO()
                            {
                                TASKNO = Convert.ToInt32(taskNO),
                                FROMSTATION = fromStation,
                                EQUIPMENTTYPE = equipmentType
                            },
                            new WCS_SUBTASKINFO()
                            {
                                AGVINDEX = AGVIndex,
                                AGVCARNO = AGVCarNO
                            }))
                        {
                            MessageBox.Show("修改失败！", "提示");
                            return;
                        }
                        MessageBox.Show("修改成功！", "提示");
                    }
                }
            }
        }
    }
}
