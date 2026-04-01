namespace CWCS.UI.FL.Tools
{
    partial class FormStationStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStationStatus));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.COL1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbFlowValue = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnReflash = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbFlowValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.panelControl3);
            this.panelControl1.Controls.Add(this.panelControl2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(800, 450);
            this.panelControl1.TabIndex = 0;
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.groupControl2);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(2, 168);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(796, 280);
            this.panelControl3.TabIndex = 1;
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.gridControl1);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.Location = new System.Drawing.Point(2, 2);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(792, 276);
            this.groupControl2.TabIndex = 2;
            this.groupControl2.Text = "信息显示";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 23);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.cmbFlowValue});
            this.gridControl1.Size = new System.Drawing.Size(788, 251);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.Lime;
            this.gridView1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.COL1,
            this.COL2,
            this.COL3,
            this.COL5,
            this.COL4});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            // 
            // COL1
            // 
            this.COL1.Caption = "站台编号";
            this.COL1.FieldName = "STATION_NO";
            this.COL1.Name = "COL1";
            this.COL1.OptionsColumn.AllowEdit = false;
            this.COL1.Visible = true;
            this.COL1.VisibleIndex = 0;
            this.COL1.Width = 127;
            // 
            // COL2
            // 
            this.COL2.Caption = "站台描述";
            this.COL2.FieldName = "STATION_DESCRIPTION";
            this.COL2.Name = "COL2";
            this.COL2.OptionsColumn.AllowEdit = false;
            this.COL2.Visible = true;
            this.COL2.VisibleIndex = 1;
            this.COL2.Width = 212;
            // 
            // COL3
            // 
            this.COL3.Caption = "是否有货";
            this.COL3.FieldName = "EXIST_PALLET";
            this.COL3.Name = "COL3";
            this.COL3.Visible = true;
            this.COL3.VisibleIndex = 2;
            this.COL3.Width = 212;
            // 
            // COL5
            // 
            this.COL5.Caption = "当前任务数";
            this.COL5.FieldName = "TASK_COUNT";
            this.COL5.Name = "COL5";
            this.COL5.Visible = true;
            this.COL5.VisibleIndex = 3;
            // 
            // COL4
            // 
            this.COL4.Caption = "创建时间";
            this.COL4.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            this.COL4.FieldName = "CREATE_TIME";
            this.COL4.Name = "COL4";
            this.COL4.OptionsColumn.AllowEdit = false;
            this.COL4.Visible = true;
            this.COL4.VisibleIndex = 4;
            this.COL4.Width = 214;
            // 
            // cmbFlowValue
            // 
            this.cmbFlowValue.Name = "cmbFlowValue";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.groupControl1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(2, 2);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(796, 166);
            this.panelControl2.TabIndex = 0;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.btnReflash);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(2, 2);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(792, 162);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "操作";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(27, 41);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(403, 28);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "备注：（1）1表示有货；0表示无货\r\n         （2）分配转向站台的时候，优先分配任务数最少的转向站台            ";
            // 
            // btnReflash
            // 
            this.btnReflash.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReflash.ImageOptions.Image")));
            this.btnReflash.Location = new System.Drawing.Point(634, 76);
            this.btnReflash.Name = "btnReflash";
            this.btnReflash.Size = new System.Drawing.Size(93, 35);
            this.btnReflash.TabIndex = 0;
            this.btnReflash.Text = "刷新";
            this.btnReflash.Click += new System.EventHandler(this.btnReflash_Click);
            // 
            // FormStationStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormStationStatus";
            this.Text = "转向站台维护";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbFlowValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnReflash;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn COL1;
        private DevExpress.XtraGrid.Columns.GridColumn COL2;
        private DevExpress.XtraGrid.Columns.GridColumn COL3;
        private DevExpress.XtraGrid.Columns.GridColumn COL4;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbFlowValue;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.Columns.GridColumn COL5;
    }
}