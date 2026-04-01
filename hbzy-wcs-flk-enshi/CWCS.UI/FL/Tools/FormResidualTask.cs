using CWCS.Entity;
using CWCS.Repository;
using DevExpress.ClipboardSource.SpreadsheetML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWCS.UI.FL.Tools
{
    public partial class FormResidualTask : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormResidualTask()
        {
            InitializeComponent();
            rbMainTask.Checked = true;
            rbMainTask.CheckedChanged += radioBox_CheckedChanged;
            rbSubTask.CheckedChanged += radioBox_CheckedChanged;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
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
                    if (rbMainTask.Checked)
                    {
                       _repository.DeleteTableData(
                       new WCS_SUBTASKINFO()
                       {
                           SUBTASKNO = gridView1.GetRowCellValue(rowHandle, "T_ID").ToString()
                       }
                       );
                       _repository.DeleteTableData(
                           new WCS_TASKINFO()
                           {
                               T_ID = gridView1.GetRowCellValue(rowHandle, "T_ID").ToString()
                           }
                       );
                    }
                    if (rbSubTask.Checked)
                    {
                        _repository.DeleteTableData(
                          new WCS_SUBTASKINFO()
                          {
                              T_ID = gridView1.GetRowCellValue(rowHandle, "T_ID").ToString()
                          }
                      );
                    }
                   
                }
                btnSelect_Click(sender, e);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (rbMainTask.Checked)
            {
               gridControl1.DataSource =  _repository.QueryTableData(new WCS_TASKINFO());
            }

            if (rbSubTask.Checked)
            {
                DataTable dt = _repository.QueryTableData(new WCS_SUBTASKINFO());
                dt.Columns["CREATE_DATE"].ColumnName = "CREATETIME";
                gridControl1.DataSource = dt;
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = $"{e.RowHandle + 1}";
            }
        }

        private void radioBox_CheckedChanged(object sender, EventArgs e)
        {
            btnSelect_Click(sender, e);
        }
    }
}
