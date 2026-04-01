using CWCS.Entity;
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

namespace CWCS.UI.FL.Tools
{
    public partial class FormStationStatus : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormStationStatus()
        {
            InitializeComponent();
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                int[] rowCount = this.gridView1.GetSelectedRows();
                if (rowCount.Length != 1) return;
                string stationNo = this.gridView1.GetRowCellValue(rowCount[0], "STATION_NO").ToString().Trim();
                string exsitPallet = this.gridView1.GetRowCellValue(rowCount[0], "EXIST_PALLET").ToString().Trim();
                int taskCount = Convert.ToInt32(this.gridView1.GetRowCellValue(rowCount[0], "TASK_COUNT").ToString().Trim());
                if (string.IsNullOrEmpty(exsitPallet))
                {
                    MessageBox.Show("参数值不能为空！", "提示");
                    return;
                }
                if (!_repository.UpdateTableData(
                    new WCS_STATION_STATUS()
                    {
                        STATION_NO = stationNo
                    },
                    new WCS_STATION_STATUS()
                    {
                        EXIST_PALLET = exsitPallet,
                        TASK_COUNT = taskCount
                    }
                ))
                {
                    MessageBox.Show("修改失败！", "提示");
                    return;
                }
                MessageBox.Show("修改成功！", "提示");
                gridControl1.DataSource = _repository.QueryTableData(new WCS_STATION_STATUS());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改失败！{ex.Message}", "提示");
            }
           
        }

        private void btnReflash_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = _repository.QueryTableData(new WCS_STATION_STATUS());
        }
    }
}
