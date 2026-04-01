using CWCS.Repository;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CWCS.UI.FL.Tools
{
    public partial class FormShuttleInfo : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormShuttleInfo()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = _repository.GetAllShuttleInfo();
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            MouseEventArgs @event = e as MouseEventArgs;
            if (@event.Button.IsLeft())
            {
                GridHitInfo info = gridView1.CalcHitInfo(new Point(@event.X, @event.Y));
                if (info != null && info.InRowCell && info.Column.Name.Equals("colOperation"))
                {
                    FormShuttleInfoEdit form = new FormShuttleInfoEdit();
                    form.ReflashTable = btnSelect_Click;
                    form.t_id = gridView1.GetRowCellValue(info.RowHandle, "T_ID").ToString();
                    form.txtShuttleNo.Text = gridView1.GetRowCellValue(info.RowHandle, "EQUIPMENT_NO").ToString();
                    form.txtSCNo.Text = gridView1.GetRowCellValue(info.RowHandle, "BELONG_EQUIPMENT_NO").ToString();
                    form.txtCurrentLine.Text =  gridView1.GetRowCellValue(info.RowHandle, "CURRENT_LOCATION_LINE").ToString();
                    form.cmbCurrentStatus.Text = form.cmbCurrentStatus.Text = gridView1.GetRowCellValue(info.RowHandle, "CURRENT_STATUS_DES").ToString();
                    form.txtCurrentLayer.Text = gridView1.GetRowCellValue(info.RowHandle, "CURRENT_LOCATION_LAYER").ToString();
                    form.cmbCurrentRackSide.Text = gridView1.GetRowCellValue(info.RowHandle, "CURRENT_RACK_SIDE_DES").ToString();
                    form.dateEnterLeaveTime.Text = gridView1.GetRowCellValue(info.RowHandle, "ENTER_LEAVE_CS_TIME").ToString();
                    form.IsShowed = true;
                    form.ShowDialog();
                    form.txtCurrentLayer.Focus();
                }
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                e.Info.DisplayText = $"{e.RowHandle + 1}";
            }
           
        }
    }
}
