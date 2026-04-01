
using CWCS.Entity;
using CWCS.Repository;
using DevExpress.XtraGrid.Columns;
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
    public partial class FormAGVTaskUpdate : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormAGVTaskUpdate()
        {
            InitializeComponent();
        }


        private void btnSelect_Click(object sender, EventArgs e)
        {
            BindGridView(this.txtTaskNO.Text.Trim(), this.txtBarcode.Text.Trim());
        }

        public void BindGridView(string taskNO, string barcode)
        {
            int taskNoTemp = 0;
            WCS_SUBTASKINFO info = new WCS_SUBTASKINFO();
            info.EQUIPMENTTYPE = "AGV";
            if (!string.IsNullOrEmpty(this.txtBarcode.Text.Trim())) info.BARCODE = this.txtBarcode.Text.Trim();
            if (!string.IsNullOrEmpty(this.txtAgvIndex.Text.Trim())) info.AGVINDEX = this.txtAgvIndex.Text.Trim();
            if (int.TryParse(taskNO, out taskNoTemp)) info.TASKNO = taskNoTemp;
            DataTable dt = _repository.QueryTableData(info);
            gridControl1.DataSource = dt;
        }
       

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle < 0) return;
            e.Info.DisplayText = (e.RowHandle+1).ToString();
        }


        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            int[] rowCount = this.gridView1.GetSelectedRows();
            if (rowCount.Length != 1) return;
            string id = this.gridView1.GetRowCellValue(rowCount[0], "T_ID").ToString();
            string AGVIndex = this.gridView1.GetRowCellValue(rowCount[0], "AGVINDEX").ToString();
            string AGVCarNO = this.gridView1.GetRowCellValue(rowCount[0], "AGVCARNO").ToString();
            if (!_repository.UpdateAGVIndexById(id, AGVIndex, AGVCarNO))
            {
                MessageBox.Show("修改失败！", "提示");
                return;
            }
            MessageBox.Show("修改成功！", "提示");
            BindGridView(this.txtTaskNO.Text.Trim(), this.txtBarcode.Text.Trim());
        }

      
    }
}
