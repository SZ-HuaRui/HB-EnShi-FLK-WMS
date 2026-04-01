using CWCS.Entity;
using CWCS.Repository;
using DataAccess;
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
    public partial class FormReceiveTask : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormReceiveTask()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = _repository.GetRecevieTask();
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

            if (e.RowHandle < 0) return;
            e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
                    _repository.DeleteTableData(
                    new WCS_RECEIVE_TASK()
                    {
                        TASKNUM = gridView1.GetRowCellValue(rowHandle, "TASKNUM").ToString()
                    }
                    );
                }
                btnSelect_Click(sender, e);
            }
        }

        private void btnItemInventoryCompute_Click(object sender, EventArgs e)
        {
            FormUserCheck userCheck = new FormUserCheck();
            userCheck.ShowDialog();
            if (userCheck.IsChecked)
            {
              
                if (!_repository.ItemInventoryCompute())
                {
                    MessageBox.Show("结存失败！", "提示");
                    return;
                }
                MessageBox.Show("结存成功", "提示");
            }

        }

        private void btnPeiPanInventoryCompute_Click(object sender, EventArgs e)
        {
            FormUserCheck userCheck = new FormUserCheck();
            userCheck.ShowDialog();
            if (userCheck.IsChecked)
            {
                 
                if (!_repository.PeiPanInventoryCompute())
                {
                    MessageBox.Show("结存失败！", "提示");
                    return;
                }
                MessageBox.Show("结存成功", "提示");
            }
        }
    }
}
