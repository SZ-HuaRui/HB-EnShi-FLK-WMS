using C1.Win.C1Ribbon;
using CWCS.BLL;
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
    public partial class FrmTaskEdit : C1RibbonForm
    {
        private string _tasktype;
        private string _taskno;
        private string _barcode;
        private int _taskstatus = 0;
        private int _runstatus = 0;
        private string _fromstation;
        private string _tostation;
        public static bool ischange = false;
        public OracleRepository _repository = new OracleRepository();
        public static string newdata = string.Empty;
        public FrmTaskEdit()
        {
            InitializeComponent();
        }

        public FrmTaskEdit(string tasktype, string taskno, string barcode,string fromstation, string tostation, string taskstatus, string runstatus)
        {
            InitializeComponent();
            _tasktype = tasktype;
            _taskno = taskno;
            _barcode = barcode;
            _fromstation = fromstation;
            _tostation = tostation;

            switch (taskstatus)
            {
                case "未开始": _taskstatus = 0; break;
                case "已完成": _taskstatus = 1; break;
                case "执行中": _taskstatus = 2; break;
            }
            if (runstatus == "可执行")
                _runstatus = 1;
            else if (runstatus == "不可执行")
                _runstatus = 0;
        }
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            bool flag = _repository.UpdateTaskStatus(_taskno.ToString(), _tasktype, cbb_taskstatus.SelectedIndex.ToString(), cbb_runstatus.SelectedIndex.ToString());
            if (flag)
            {
                ischange = true;
                newdata = string.Format(",新数据:任务号:{0},任务类型:{1},任务状态:{2},执行状态:{3},起始站台:{4},目标站台:{5}", _taskno.ToString(), _tasktype, cbb_taskstatus.SelectedItem.ToString(), cbb_runstatus.SelectedItem.ToString(), txt_fromstation.Text, txt_tostation.Text);
            }
            MessageBox.Show(string.Format("任务修改{0}", flag ? "成功" : "失败"), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmTaskEdit_Load(object sender, EventArgs e)
        {
            txt_taskno.Text = _taskno;
            txtBarCode.Text = _barcode;
            txt_tostation.Text = _tostation;
            txt_fromstation.Text = _fromstation;
            cbb_taskstatus.SelectedIndex = _taskstatus;
            cbb_runstatus.SelectedIndex = _runstatus;

        }
    }
}
