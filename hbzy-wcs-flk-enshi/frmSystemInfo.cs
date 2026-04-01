using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Baf.ServerComponents;

namespace CWCS.UI
{
    public partial class frmSystemInfo :Form
    {
        private SystemHelper mHelper = new SystemHelper();
        private ulong mTotal = 0;
        public frmSystemInfo()
        {
            InitializeComponent();
            VisibleChanged += new EventHandler(frmSystemInfo_VisibleChanged);
            lblCount.Text = Environment.ProcessorCount.ToString();
            mTotal = mHelper.TotalPhysicalMemory();
            lblTotalMemroy.Text = mTotal.ToString();
        }

        void frmSystemInfo_VisibleChanged(object sender, EventArgs e)
        {
            timer1.Enabled = Visible;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblCPU.Text = mHelper.GetAvailableProcessor().ToString();
            ulong useMemroy = mHelper.GetAvailableMemory();
            lblMemory.Text = useMemroy.ToString();
            lblCurrent.Text = (Environment.WorkingSet/1024/1024).ToString();
            float ava = (float)useMemroy / (float)mTotal;
            lblAvaMemory.Text = Math.Round((ava * 100),2).ToString();
            lblSign.Text = Math.Round(mHelper.GetComputerScope(),4).ToString();
        }


    }
}