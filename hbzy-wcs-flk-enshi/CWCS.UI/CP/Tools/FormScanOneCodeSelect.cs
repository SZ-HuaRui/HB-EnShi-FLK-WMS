using CWCS.Repository;
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
    public partial class FormScanOneCodeSelect : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public DataTable SourceData = new DataTable();
        private string StartTime { get; set; }
        private string EndTime { get; set; }
        private string OneCode { get; set; }
        private bool IsChecked = false;
        public FormScanOneCodeSelect()
        {
            InitializeComponent();
            this.txtStartTime.Text = DateTime.Now.ToString("yyyy/MM/dd") + " 08:00:00";
            this.txtEndTime.Text = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";
            this.paginationControl1.OnPageChanged += PaginationControl1_OnPageChanged;
        }

        private void PaginationControl1_OnPageChanged(object sender, EventArgs e)
        {
            this.gridControl1.DataSource = ListRowToDataTable(SourceData.Select().Skip(paginationControl1.PageSize * (paginationControl1.CurrentPageIndex - 1)).Take(this.paginationControl1.PageSize).ToList());
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            StartTime = this.txtStartTime.Text.Trim();
            EndTime = this.txtEndTime.Text.Trim();
            OneCode = this.txtOneCode.Text.Trim();
            SourceData = _repository.GetWcsOneCodeToItemCode(StartTime, EndTime,OneCode,IsChecked);
            this.paginationControl1.TotalCount = SourceData.Rows.Count;
            this.paginationControl1.CurrentPageIndex = 1;
            this.gridControl1.DataSource = ListRowToDataTable(SourceData.Select().Skip(0).Take(this.paginationControl1.PageSize).ToList());
        }

        public DataTable ListRowToDataTable(List<DataRow> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            DataTable dt = list[0].Table.Clone();
            foreach (DataRow dr in list)
            {
                dt.ImportRow(dr);
            }
            return dt;
        }


        private void gridView1_CustomDrawRowIndicator_1(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {

            if (e.RowHandle < 0) return;
            e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void chkNoRead_CheckedChanged(object sender, EventArgs e)
        {
            IsChecked = this.chkNoRead.Checked;
            SourceData = _repository.GetWcsOneCodeToItemCode(StartTime, EndTime, OneCode, IsChecked);
            this.paginationControl1.TotalCount = SourceData.Rows.Count;
            this.paginationControl1.CurrentPageIndex = 1;
            this.gridControl1.DataSource = ListRowToDataTable(SourceData.Select().Skip(0).Take(this.paginationControl1.PageSize).ToList());
        }

      
    }
}
