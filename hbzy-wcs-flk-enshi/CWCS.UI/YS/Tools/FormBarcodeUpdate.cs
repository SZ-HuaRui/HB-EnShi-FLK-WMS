using CWCS.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WCS
{
    /// <summary>
    /// Description:update barcode in the location
    /// </summary>
    public partial class FormBarcodeUpdate : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormBarcodeUpdate()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DataTable dt = _repository.GetLocationInfo(this.txtBarcode.Text);
            gridControl1.DataSource = dt;
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            int[] rowsCount =  this.gridView1.GetSelectedRows();
            if (rowsCount is null || rowsCount.Length != 1) return;
            string barcode = this.gridView1.GetRowCellValue(rowsCount[0], "CONTAINER_BARCODE").ToString();
            string locationNumber = this.gridView1.GetRowCellValue(rowsCount[0], "LOCATION_NUMBER").ToString();
            string zoneId = this.gridView1.GetRowCellValue(rowsCount[0], "ZONE_ID").ToString();
            Dictionary<bool,string> dic = _repository.UpdateLocationBarcode(locationNumber, zoneId, barcode);
            if (dic.Keys.Contains(false))
            {
                MessageBox.Show(dic[false],"提示");
                return;
            }
            MessageBox.Show("托盘号维护成功！", "提示");
            return;
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle < 0) return;
            e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.C)
            {
               Point point = this.gridView1.GridControl.PointToClient(Control.MousePosition);
               GridHitInfo info = this.gridView1.CalcHitInfo(point);
                if (info.InRow && info.InRowCell)
                {
                  string text =  this.gridView1.GetRowCellValue(info.RowHandle,info.Column.FieldName).ToString();
                  Clipboard.SetText(text);     
                }

            }
        }
    }
}
