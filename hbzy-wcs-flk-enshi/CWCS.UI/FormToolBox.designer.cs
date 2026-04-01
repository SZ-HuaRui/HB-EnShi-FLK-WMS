namespace WCS
{
    partial class FormToolBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormToolBox));
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.btnShuttleInfoUpdate = new DevExpress.XtraBars.BarButtonItem();
            this.btnAgvTaskInfoUpdate = new DevExpress.XtraBars.BarButtonItem();
            this.btnStationStatusUpdate = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.btnSystemConfig = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.btnReceiveTask = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnResidualTaskClean = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeader;
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 101);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.Size = new System.Drawing.Size(800, 349);
            this.xtraTabControl1.TabIndex = 1;
            this.xtraTabControl1.CloseButtonClick += new System.EventHandler(this.xtraTabControl1_CloseButtonClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2,
            this.ribbonPageGroup1,
            this.ribbonPageGroup3});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "功能页一";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.btnShuttleInfoUpdate);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnAgvTaskInfoUpdate);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnStationStatusUpdate);
            this.ribbonPageGroup2.ItemLinks.Add(this.btnResidualTaskClean);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "WCS信息维护";
            // 
            // btnShuttleInfoUpdate
            // 
            this.btnShuttleInfoUpdate.Caption = "穿梭板位置信息维护";
            this.btnShuttleInfoUpdate.Id = 25;
            this.btnShuttleInfoUpdate.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnShuttleInfoUpdate.ImageOptions.Image")));
            this.btnShuttleInfoUpdate.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnShuttleInfoUpdate.ImageOptions.LargeImage")));
            this.btnShuttleInfoUpdate.Name = "btnShuttleInfoUpdate";
            this.btnShuttleInfoUpdate.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btnShuttleInfoUpdate.VisibleInSearchMenu = false;
            // 
            // btnAgvTaskInfoUpdate
            // 
            this.btnAgvTaskInfoUpdate.Caption = "AGV任务信息维护";
            this.btnAgvTaskInfoUpdate.Id = 28;
            this.btnAgvTaskInfoUpdate.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAgvTaskInfoUpdate.ImageOptions.Image")));
            this.btnAgvTaskInfoUpdate.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnAgvTaskInfoUpdate.ImageOptions.LargeImage")));
            this.btnAgvTaskInfoUpdate.Name = "btnAgvTaskInfoUpdate";
            // 
            // btnStationStatusUpdate
            // 
            this.btnStationStatusUpdate.Caption = "转向站台维护";
            this.btnStationStatusUpdate.Id = 29;
            this.btnStationStatusUpdate.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnStationStatusUpdate.ImageOptions.Image")));
            this.btnStationStatusUpdate.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnStationStatusUpdate.ImageOptions.LargeImage")));
            this.btnStationStatusUpdate.Name = "btnStationStatusUpdate";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSystemConfig);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.State = DevExpress.XtraBars.Ribbon.RibbonPageGroupState.Expanded;
            this.ribbonPageGroup1.Text = "系统配置";
            // 
            // btnSystemConfig
            // 
            this.btnSystemConfig.Caption = "系统配置";
            this.btnSystemConfig.Id = 27;
            this.btnSystemConfig.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSystemConfig.ImageOptions.Image")));
            this.btnSystemConfig.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSystemConfig.ImageOptions.LargeImage")));
            this.btnSystemConfig.Name = "btnSystemConfig";
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.btnReceiveTask);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "接口信息";
            // 
            // btnReceiveTask
            // 
            this.btnReceiveTask.Caption = "WMS下发任务";
            this.btnReceiveTask.Id = 30;
            this.btnReceiveTask.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReceiveTask.ImageOptions.Image")));
            this.btnReceiveTask.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnReceiveTask.ImageOptions.LargeImage")));
            this.btnReceiveTask.Name = "btnReceiveTask";
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.AllowMdiChildButtons = false;
            this.ribbonControl1.AllowMinimizeRibbon = false;
            this.ribbonControl1.AllowTrimPageText = false;
            this.ribbonControl1.AutoHideEmptyItems = true;
            this.ribbonControl1.AutoSaveLayoutToXml = true;
            this.ribbonControl1.BackColor = System.Drawing.Color.White;
            this.ribbonControl1.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.True;
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.ribbonControl1.SearchEditItem,
            this.btnShuttleInfoUpdate,
            this.btnSystemConfig,
            this.btnAgvTaskInfoUpdate,
            this.btnStationStatusUpdate,
            this.btnReceiveTask,
            this.btnResidualTaskClean});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 32;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.OptionsPageCategories.ShowCaptions = false;
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
            this.ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ShowPageHeadersInFormCaption = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
            this.ribbonControl1.ShowQatLocationSelector = false;
            this.ribbonControl1.ShowToolbarCustomizeItem = false;
            this.ribbonControl1.Size = new System.Drawing.Size(800, 101);
            this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
            this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            this.ribbonControl1.TransparentEditorsMode = DevExpress.Utils.DefaultBoolean.True;
            // 
            // btnResidualTaskClean
            // 
            this.btnResidualTaskClean.Caption = "残留任务清除";
            this.btnResidualTaskClean.Id = 31;
            this.btnResidualTaskClean.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.btnResidualTaskClean.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.btnResidualTaskClean.Name = "btnResidualTaskClean";
            // 
            // FormToolBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "FormToolBox";
            this.Text = "工具箱";
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem btnShuttleInfoUpdate;
        private DevExpress.XtraBars.BarButtonItem btnSystemConfig;
        private DevExpress.XtraBars.BarButtonItem btnAgvTaskInfoUpdate;
        private DevExpress.XtraBars.BarButtonItem btnStationStatusUpdate;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
        private DevExpress.XtraBars.BarButtonItem btnReceiveTask;
        private DevExpress.XtraBars.BarButtonItem btnResidualTaskClean;
    }
}