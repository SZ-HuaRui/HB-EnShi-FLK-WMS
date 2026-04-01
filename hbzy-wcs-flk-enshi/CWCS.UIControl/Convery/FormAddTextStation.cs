using CWCS.Entity;
using CWCS.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWCS.UIControl.Convery
{
    public partial class FormAddTextStation : Form
    {
        private OracleRepository _repository = new OracleRepository();
        public FormAddTextStation()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtItemId.Text.Trim()))
            {
                MessageBox.Show("站台ID不能为空","提示");
            }
            WCS_OPCITEM item = new WCS_OPCITEM();
            item.Itemid = txtItemId.Text.Trim();
            item.Itemname = txtItemId.Text.Trim();
            item.Stationno = txtItemId.Text.Trim();
            item.Locationx = txtX.Text.Trim();
            item.Locationy = txtY.Text.Trim();
            item.Width = txtWidth.Text.Trim();
            item.Height = txtHeight.Text.Trim();
            item.TextDescription = txtTextDescription.Text.Trim();
            item.Groupname = txtGroupName.Text.Trim();
            item.AreaCode = txtZoneCode.Text.Trim();
            item.ZoneCode = txtZoneCode.Text.Trim();
            item.Status = "1";
            if (!_repository.InsertTableData(item))
            {
                MessageBox.Show("添加站台失败！", "提示");
                return;
            }
            MessageBox.Show("添加站台成功！", "提示");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
