using CWCS.Common.Uitl;
using CWCS.Entity;
using CWCS.Repository;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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
    public partial class FormSystemLog : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public DataTable SourceData = new DataTable();
        private string StartTime { get; set; }
        private string EndTime { get; set; }
        public FormSystemLog()
        {
            InitializeComponent();
            dateStartTime.Text = DateTime.Now.ToString("yyyy/MM/dd") + " 08:00:00";
            dateEndTime.Text = DateTime.Now.AddMonths(1).ToString("yyyy/MM/dd") + " 23:59:59";
            paginationControl1.OnPageChanged += PaginationControl1_OnPageChanged;
           

        }



        private void PaginationControl1_OnPageChanged(object sender, EventArgs e)
        {
            this.gridControl1.DataSource = DataTableUtil.ListRowToDataTable(SourceData.Select().Skip(paginationControl1.PageSize * (paginationControl1.CurrentPageIndex - 1)).Take(this.paginationControl1.PageSize).ToList());
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                StartTime = dateStartTime.Text.Trim();
                EndTime = dateEndTime.Text.Trim();
                SourceData = _repository.GetSystemLog(dateStartTime.Text, dateEndTime.Text, txtKeyword.Text.Trim());
                paginationControl1.TotalCount = SourceData.Rows.Count;
                paginationControl1.CurrentPageIndex = 1;
                gridControl1.DataSource = DataTableUtil.ListRowToDataTable(SourceData.Select().Skip(0).Take(paginationControl1.PageSize).ToList());
            }
            catch
            {

            }
          
        }

      

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle < 0) return;
            e.Info.DisplayText = (e.RowHandle + 1).ToString();
            
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                MouseEventArgs mouseArgs = e as MouseEventArgs;
                GridHitInfo info = gridView1.CalcHitInfo(mouseArgs.X, mouseArgs.Y);
                if (info != null && info.InRowCell && !info.InGroupPanel)
                {
                    rtbLog.Text = gridView1.GetRowCellValue(info.RowHandle, "CREATETIME").ToString()
                                  + "  "
                                  + gridView1.GetRowCellValue(info.RowHandle, "LOGTEXT").ToString();
                }
            }
            catch
            {

            }
            
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (gridView1.GetRowCellValue(e.RowHandle, "LOGTEXT").ToString().Contains("失败") || gridView1.GetRowCellValue(e.RowHandle, "LOGTEXT").ToString().Contains("异常"))
                    {
                        e.Appearance.BackColor = Color.Red;
                    }
                }
            }
            catch
            {

            }
            
        }
    }
}
