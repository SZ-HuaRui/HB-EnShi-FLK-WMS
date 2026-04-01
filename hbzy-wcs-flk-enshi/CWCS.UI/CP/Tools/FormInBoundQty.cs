using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWCS.Repository;

namespace CWCS.UI
{
    public partial class FormInBoundQty : Form
    {
        public OracleRepository _repository = new OracleRepository();
        private string StartTime { get; set; }
        private string EndTime { get; set; }
        public FormInBoundQty()
        {
            InitializeComponent();
            this.txtStartTime.Text = DateTime.Now.ToString("yyyy/MM/dd") + " 08:00:00";
            this.txtEndTime.Text = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle < 0) return;
            e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                StartTime = this.txtStartTime.Text.Trim();
                EndTime = this.txtEndTime.Text.Trim();
                DataTable dt = _repository.GetInBoundQty(StartTime, EndTime);
                int qty = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        qty += Convert.ToInt32(dr["Qty"]);
                    }
                    DataRow item = dt.NewRow();
                    item["item_code"] = "合计";
                    item["qty"] = qty.ToString();
                    dt.Rows.Add(item);
                }

                this.gridControl1.DataSource = dt;
            }
            catch
            {

            }
           
        }

        private void btnToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                    options.SheetName = "入库量统计";
                    gridControl1.ExportToXls(saveFileDialog.FileName, options);
                    DevExpress.XtraEditors.XtraMessageBox.Show("导出Excel成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {

            }
            
        }
    }
}
