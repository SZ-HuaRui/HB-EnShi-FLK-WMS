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
    public partial class FormUserCheck : Form
    {
        public OracleRepository _repository = new OracleRepository();
        public bool IsChecked = false;
        public string UserName { get; set; }
        public string Password { get; set; }
        public FormUserCheck()
        {
            InitializeComponent();
            txtUser.Text = "admin";
            btnCancel.MouseEnter += btn_MouserEnter;
            btnConfirm.MouseEnter += btn_MouserEnter;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            UserName = txtUser.Text.Trim();
            Password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("用户名不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("密码不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!_repository.IsExistUser(UserName, Password))
            {
                MessageBox.Show("用户或密码错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txtPassword.Text = string.Empty;
                return;
            }
            IsChecked = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        

        private void FormUserCheck_Shown(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        private void btn_MouserEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
        }
    }
}
