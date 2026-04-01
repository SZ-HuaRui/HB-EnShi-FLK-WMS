using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CWCS.BLL;

namespace CWCS.UI
{
    public partial class FrmUpdateSTVFlag : Form
    {
        public FrmUpdateSTVFlag(string taskNO,string barcode,string groupNO,string sequenceNO)
        {
            InitializeComponent();
            this.textBoxTasKNO.Text = taskNO;
            this.textBoxBarcode.Text = barcode;
            this.textBoxGroupNO.Text = groupNO;
            this.textBoxSequenceNO.Text = sequenceNO;
        }

        //确认
        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            //string[] strArray = bll.UpdateSTVFlag(this.textBoxTasKNO.Text,this.textBoxBarcode.Text);
            //if (strArray[1].Equals("0"))
            //{
            //    MessageBox.Show("确认接货失败！描述："+strArray[0], "提示", MessageBoxButtons.OK);
            //    this.Close();
            //    return;

            //}
            MessageBox.Show("确认接货成功！","提示",MessageBoxButtons.OK);
            this.Close();
        }

        //取消
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
