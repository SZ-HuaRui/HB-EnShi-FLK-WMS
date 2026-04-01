using CWCS.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CWCS.UI
{
    public partial class FormItemInfo : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public FormItemInfo()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                string itemName = this.txtItemName_Select.Text.Trim();
                this.gridControl1.DataSource = _repository.GetItemInfo(itemName);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
          
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                string itemCode = this.txtItemCode_Insert.Text.Trim();
                string itemName = this.txtItemName_Insert.Text.Trim();
                string boxType = this.txtBoxType_Insert.Text.Trim();
                if (string.IsNullOrEmpty(itemCode))
                {
                    MessageBox.Show("品牌编码必须填！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (string.IsNullOrEmpty(itemName))
                {
                    MessageBox.Show("品牌名称必须填！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (string.IsNullOrEmpty(boxType))
                {
                    MessageBox.Show("烟箱类型必须填！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Regex regex = new Regex(@"\D");
                if (regex.IsMatch(boxType))
                {
                    MessageBox.Show("烟箱类型必须是数字！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string[] result;
                if (MessageBox.Show("确定添加该品牌吗！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    result = _repository.Wcs_InsertItemInfo(itemCode, itemName, boxType);
                    if (!result[0].Equals("1"))
                    {
                        MessageBox.Show($"品牌添加失败！\n{result[1]}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    MessageBox.Show("品牌添加成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.gridControl1.DataSource = _repository.GetItemInfo(itemName);
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
          

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int[] rowsCount =  this.gridView1.GetSelectedRows();
                if (rowsCount == null || rowsCount.Length != 1)
                {
                    MessageBox.Show($"请选择一行数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                bool result;
                if (MessageBox.Show("确定删除该品牌吗！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    result = _repository.Wcs_DeleteItemInfo(this.gridView1.GetRowCellValue(rowsCount[0],"T_ID").ToString());
                    if (!result)
                    {
                        MessageBox.Show($"删除失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    MessageBox.Show($"删除成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.gridControl1.DataSource = _repository.GetItemInfo(this.txtItemName_Select.Text.Trim());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.RowHandle < 0) return;
            e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                int index = this.gridView1.GetSelectedRows()[0];
                string itemId = this.gridView1.GetRowCellValue(index,"T_ID").ToString();
                string itemName = this.gridView1.GetRowCellValue(index, "ITEM_NAME").ToString();
                string exteriorCode = this.gridView1.GetRowCellValue(index, "EXTERIOR_CODE").ToString();
                string boxType = this.gridView1.GetRowCellValue(index, "BOX_TYPE").ToString();
                bool result = _repository.Wcs_UpdateItemInfo(itemId,itemName,exteriorCode,boxType);
                if (!result)
                {
                    MessageBox.Show($"修改失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                MessageBox.Show($"修改成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
