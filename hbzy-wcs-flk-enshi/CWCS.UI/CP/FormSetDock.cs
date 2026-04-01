using CWCS.Service;
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
    public partial class FormSetDock : Form
    {
        private SetDockService service = new SetDockService();
        public delegate void DelegateShowTable(DataTable dt);
        public event DelegateShowTable ReflashTable;
        public FormSetDock(string dockNO,string itemCode)
        {
            InitializeComponent();
            this.lblDock.Text = dockNO;
            BindBrandCmb(itemCode);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.cmbBrand.Text) || this.cmbBrand.Text.Equals("-请选择-") || this.cmbBrand.SelectedValue.Equals("99999999"))
                {
                    MessageBox.Show("请先选择牌号");
                    return;
                }
                if (service.IsExistsExceptionBrand(this.cmbBrand.SelectedValue.ToString()))
                {
                    MessageBox.Show("设道失败！存在相同外部编码的、已设道的牌号，请重新设定！");
                    return;
                }
                if (!service.WriteBoxType(this.lblDock.Text, this.cmbBrand.SelectedValue.ToString()))
                {
                    MessageBox.Show("设道失败！下发烟箱类型给PLC失败！");
                    return;
                }
                if (!service.SetDockBrand(this.lblDock.Text, this.cmbBrand.SelectedValue.ToString()))
                {
                    MessageBox.Show("设道失败！WCS数据库牌号更新失败！");
                    return;
                }
                MessageBox.Show($"道口{this.lblDock.Text}设道成功!");
                service.InsertWcsLog($"道口{this.lblDock.Text}设道成功！");
                this.Close();
                ReflashTable(service.GetAllDockInfo());
                return;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"道口{this.lblDock.Text}设道失败！{ex.Message}");
                return;
            }
            
        }

        private void handCursor_MouserEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void BindBrandCmb(string selectedCode)
        {
            this.cmbBrand.ValueMember = "item_code";
            this.cmbBrand.DisplayMember = "item_name";
            this.cmbBrand.DataSource =  service.GetAllBrandInfo();
            this.cmbBrand.SelectedValue = selectedCode;
            
        }

        

        private void cmbBrand_DropDown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtBrandCode.Text))
            {
                this.cmbBrand.DataSource = service.GetAllBrandInfo();
            }
            else
            {
                this.cmbBrand.DataSource = service.GetSomeBrandInfo(this.txtBrandCode.Text.Trim());
            }
        }
    }
}
