using CWCS.Entity;
using CWCS.Repository;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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
    public partial class FormSystemConfig : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormSystemConfig()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = _repository.GetWcsParamConfig(txtParamName.Text.Trim());
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            int[] rowCount = this.gridView1.GetSelectedRows();
            if (rowCount.Length != 1) return;
            string paramCode = this.gridView1.GetRowCellValue(rowCount[0], "PARAM_CODE").ToString().Trim();
            string paramValue = this.gridView1.GetRowCellValue(rowCount[0], "PARAM_VALUE").ToString().Trim();
            string editValue = this.gridView1.GetRowCellValue(rowCount[0], "CAN_EDIT").ToString().Trim();
            if (string.IsNullOrEmpty(paramValue))
            {
                MessageBox.Show("参数值不能为空！", "提示");
                return;
            }
            if (editValue.Equals("0"))
            {
                MessageBox.Show("该值不可修改！", "提示");
                return;
            }
            if (!_repository.UpdateWcsParamConfig(paramCode, paramValue))
            {
                MessageBox.Show("修改失败！", "提示");
                return;
            }
            if (StaticCommon.WcsParamConfigList != null && StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals(paramCode)).FirstOrDefault() != null)
            {
                WCS_PARAMETER_CONFIG config = StaticCommon.WcsParamConfigList.Where(x => x.PARAM_CODE.Equals(paramCode)).FirstOrDefault();
                config.PARAM_VALUE = paramValue;
            }
            MessageBox.Show("修改成功！", "提示");
            //gridControl1.DataSource = dal.GetAllWcsParamConfig();
        }
    }
}
