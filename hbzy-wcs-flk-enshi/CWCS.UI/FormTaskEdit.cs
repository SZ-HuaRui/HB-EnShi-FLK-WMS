using CWCS.Entity;
using CWCS.Repository;
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
    public partial class FormTaskEdit : Form
    {
        public string TaskType { get; set; }

        public OracleRepository _repository = new OracleRepository();
        public Action<object, EventArgs> RefreshData { get; set; }
        public FormTaskEdit()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            cmbRunStatus.Properties.Items.Add("可执行");
            cmbRunStatus.Properties.Items.Add("不可执行");
            cmbTaskStatus.Properties.Items.Add("未开始");
            cmbTaskStatus.Properties.Items.Add("执行中");
            cmbTaskStatus.Properties.Items.Add("已完成");
            cmbRunStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbTaskStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                string taskStaus = string.Empty;
                string runStatus = string.Empty;
                switch (cmbTaskStatus.Text)
                {
                    case "未开始": taskStaus = "0"; break;
                    case "执行中": taskStaus = "2"; break;
                    case "已完成": taskStaus = "1"; break;
                    default: break;
                }
                switch (cmbRunStatus.Text)
                {
                    case "可执行": runStatus = "1"; break;
                    case "不可执行": runStatus = "0"; break;
                    default: break;
                }
                if (TaskType.Equals("MAIN") && taskStaus.Equals("1"))
                {
                    MessageBox.Show("不允许直接完成主任务!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                bool isSuccess = _repository.DealTaskStatus(
                    new WCS_SUBTASKINFO()
                    {
                        TASKNO = Convert.ToInt32(txtTaskNo.Text.Trim()),
                        EQUIPMENTTYPE = TaskType,
                        FROMSTATION = txtFromAddress.Text.Trim()
                    },
                    new WCS_SUBTASKINFO()
                    {
                        RUNSTATUS = runStatus,
                        TASKSTATUS = taskStaus
                    }
                    );
                if (isSuccess)
                {
                    MessageBox.Show("修改成功！");
                    RefreshData?.Invoke(null, null);
                }
                else
                {
                    MessageBox.Show("修改失败！");
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
