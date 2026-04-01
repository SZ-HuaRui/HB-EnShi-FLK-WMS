
using CWCS.Repository;
using DevExpress.XtraEditors.Controls;
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
    public partial class FormLocationInfoUpdate : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormLocationInfoUpdate()
        {
            InitializeComponent();
            BindBrandCmb();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = _repository.GetAllLocationInfo(this.txtLocationNumber.Text.Trim(),this.txtItemCode.Text.Trim(),this.txtBarcode.Text.Trim(),this.txtLotNumber.Text.Trim());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                string[] result = new string[2];
                if (MessageBox.Show("确定清空货位信息吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    int[] rowsCount = this.gridView1.GetSelectedRows();
                    if (rowsCount == null || rowsCount.Length != 1)
                    {
                        MessageBox.Show("请选择一行数据", "提示");
                        return;
                    }
                    string location_number = this.gridView1.GetRowCellValue(rowsCount[0], "LOCATION_NUMBER").ToString();
                    result = _repository.Wcs_LocationInfo_Update("Clear", location_number, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                    if (result[0].Equals("0"))
                    {
                        MessageBox.Show($"{result[1].Trim()}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    MessageBox.Show($"货位清空成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.gridControl1.DataSource = _repository.GetAllLocationInfo(this.txtLocationNumber.Text.Trim(), this.txtItemCode.Text.Trim(), this.txtBarcode.Text.Trim(), this.txtLotNumber.Text.Trim());
                }
            }
            catch
            {

            }
           
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                string[] result = new string[2];
                if (MessageBox.Show("确定修改货位信息吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    int[] rowsCount = this.gridView1.GetSelectedRows();
                    if (rowsCount == null || rowsCount.Length != 1)
                    {
                        MessageBox.Show("请选择一行数据", "提示");
                        return;
                    }
                    string location_number = this.gridView1.GetRowCellValue(rowsCount[0], "LOCATION_NUMBER").ToString();
                    string barcode = this.txtBarcodeUpdate.Text.Trim();
                    string weight = this.txtWeight.Text.Trim();
                    string item_code = this.cmbItem.SelectedValue.ToString();
                    string batch_no = this.txtLotNumberUpdate.Text.Trim();
                    string firstin_time = this.txtInBoxTime.Text.ToString();
                    string[] itemFilter = new string[3] { "99999999", "88888888", "11111111" };
                    if (string.IsNullOrEmpty(item_code))
                    {
                        MessageBox.Show("牌号必须选！", "提示");
                        return;
                    }
                    if (string.IsNullOrEmpty(barcode))
                    {
                        MessageBox.Show("箱号必须填！", "提示");
                        return;
                    }
                    if (!itemFilter.Contains(item_code))
                    {
                        if (string.IsNullOrEmpty(weight))
                        {
                            MessageBox.Show("重量必须填！", "提示");
                            return;
                        }
                        if (string.IsNullOrEmpty(batch_no))
                        {
                            MessageBox.Show("批次必须填！", "提示");
                            return;
                        }
                        if (string.IsNullOrEmpty(firstin_time))
                        {
                            MessageBox.Show("进柜时间必须填！", "提示");
                            return;
                        }
                    }

                    result = _repository.Wcs_LocationInfo_Update("Update", location_number,item_code, barcode, batch_no, firstin_time, weight);
                    if (result[0].Equals("0"))
                    {
                        MessageBox.Show($"{result[1].Trim()}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    MessageBox.Show($"货位修改成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.gridControl1.DataSource = _repository.GetAllLocationInfo(this.txtLocationNumber.Text.Trim(), this.txtItemCode.Text.Trim(), this.txtBarcode.Text.Trim(), this.txtLotNumber.Text.Trim());
                }
            }
            catch
            {

            }
            
        }


        public void BindBrandCmb()
        {
            this.cmbItem.ValueMember = "item_code";
            this.cmbItem.DisplayMember = "item_name";
            this.cmbItem.DataSource = _repository.GetBaseItemInfo();
        }
       
    }
}
