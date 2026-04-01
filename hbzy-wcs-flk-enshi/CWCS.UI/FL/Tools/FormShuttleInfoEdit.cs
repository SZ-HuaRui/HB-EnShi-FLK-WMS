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

namespace CWCS.UI.FL.Tools
{
    public partial class FormShuttleInfoEdit : Form
    {
        public bool IsShowed { get; set; } = false;

        public OracleRepository _repository = new OracleRepository();

        public Action<object,EventArgs> ReflashTable;
        public string t_id { get; set; }
        public FormShuttleInfoEdit()
        {
            InitializeComponent();
            Initialize();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (cmbCurrentStatus.Text.Equals("在货位中") && (string.IsNullOrEmpty(txtCurrentLayer.Text.Trim()) || string.IsNullOrEmpty(txtCurrentLine.Text.Trim()) || cmbCurrentRackSide.Text.Equals("置空")) )
            {
                MessageBox.Show("货位行、层必须填。货位侧不能置空","提示");
                return;
            }
            WCS_SHUTTLEINFO info = new WCS_SHUTTLEINFO();
            info.T_ID = t_id;
            if (cmbCurrentRackSide.Text.Equals("置空"))
            {
                info.CURRENT_RACK_SIDE = "";
            }else if (cmbCurrentRackSide.Text.Equals("左侧"))
            {
                info.CURRENT_RACK_SIDE = "Left";
            }
            else
            {
                info.CURRENT_RACK_SIDE = "Right";
            }

            if (cmbCurrentStatus.Text.Equals("在堆垛机上"))
            {
                info.CURRENT_STATUS = "In_Sc";
            }
            else if (cmbCurrentStatus.Text.Equals("在货位中"))
            {
                info.CURRENT_STATUS = "In_Line";
            }
            else
            {
                info.CURRENT_STATUS = "In_ChargingStation";
            }
            info.CURRENT_LOCATION_LINE = txtCurrentLine.Text.Trim();
            info.CURRENT_LOCATION_LAYER = txtCurrentLayer.Text.Trim();
            info.ENTER_LEAVE_CS_TIME =  DateTime.Parse(dateEnterLeaveTime.Text.Trim());
            if (_repository.UpdateShuttleInfo(info))
            {
                MessageBox.Show("修改成功","提示");
                Close();
                ReflashTable(null,null);
                return;
            }
            MessageBox.Show("修改失败","提示");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void Initialize()
        {
            cmbCurrentStatus.Properties.Items.Add("在堆垛机上");
            cmbCurrentStatus.Properties.Items.Add("在货位中");
            cmbCurrentStatus.Properties.Items.Add("在充电位");
            cmbCurrentRackSide.Properties.Items.Add("置空");
            cmbCurrentRackSide.Properties.Items.Add("左侧");
            cmbCurrentRackSide.Properties.Items.Add("右侧");
        }

        private void cmbCurrentStatus_Properties_EditValueChanged(object sender, EventArgs e)
        {
            if (IsShowed)
            {
                if (!cmbCurrentStatus.Text.Equals("在货位中"))
                {
                    txtCurrentLine.Text = string.Empty;
                    txtCurrentLayer.Text = string.Empty;
                    cmbCurrentRackSide.Text = "置空";
                }
            }
        }
    }
}
