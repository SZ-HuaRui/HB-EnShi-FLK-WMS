using CWCS.UI;
using CWCS.UI.FL.Tools;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraTab;
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
    public partial class FormToolBox : Form
    {
        public FormToolBox()
        {
            InitializeComponent();
            btnShuttleInfoUpdate.ItemClick += Btn_ItemClick;
            btnSystemConfig.ItemClick += Btn_ItemClick;
            btnAgvTaskInfoUpdate.ItemClick += Btn_ItemClick;
            btnStationStatusUpdate.ItemClick += Btn_ItemClick;
            btnReceiveTask.ItemClick += Btn_ItemClick;
            btnResidualTaskClean.ItemClick += Btn_ItemClick;
        }

        private void Btn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (IsExistPage(e.Item.Caption))
            {
                this.xtraTabControl1.SelectedTabPage = this.xtraTabControl1.TabPages.Where(i => i.Text.Equals(e.Item.Caption)).ToList<XtraTabPage>()[0];
            }
            else
            {
                AddPage(e.Item.Caption);
                this.xtraTabControl1.SelectedTabPage = this.xtraTabControl1.TabPages.Where(i => i.Text.Equals(e.Item.Caption)).ToList<XtraTabPage>()[0];
            }
        }

        public bool IsExistPage(string pageName)
        {
            bool IsExists = false;
            if (this.xtraTabControl1.TabPages.Count == 0) return IsExists;
            foreach (XtraTabPage page in this.xtraTabControl1.TabPages)
            {
                if (page.Text.Equals(pageName))
                {
                    IsExists = true;
                    break;
                }
            }
            return IsExists;
        }


        public void AddPage(string pageName)
        {
            XtraTabPage page = new XtraTabPage();
            page.Text = pageName;
            Form form = null;
            switch (pageName)
            {
                
                case "穿梭板位置信息维护": form = new FormShuttleInfo(); break;
                case "AGV任务信息维护": form = new FormAGVTaskUpdate(); break;
                case "转向站台维护": form = new FormStationStatus(); break;
                case "系统配置": form = new FormSystemConfig(); break;
                case "WMS下发任务": form = new FormReceiveTask(); break;
                case "残留任务清除": form = new FormResidualTask(); break;
                default:break;
            }
            form.Dock = DockStyle.Fill;
            form.TopLevel = false;
            page.Controls.Add(form);
            this.xtraTabControl1.TabPages.Add(page);
            form.Show();

        }

        private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
        {
            this.xtraTabControl1.TabPages.Remove(this.xtraTabControl1.SelectedTabPage);
        }
       
    }

}
