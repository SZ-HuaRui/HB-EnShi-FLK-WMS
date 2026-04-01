namespace CWCS.UI
{
    partial class FormScanOneCodeSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScanOneCodeSelect));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.chkNoRead = new DevExpress.XtraEditors.CheckEdit();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtOneCode = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtEndTime = new DevExpress.XtraEditors.DateEdit();
            this.txtStartTime = new DevExpress.XtraEditors.DateEdit();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.paginationControl1 = new CWCS.CommonControl.UIControl.PaginationControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkNoRead.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOneCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.groupControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1067, 182);
            this.panelControl1.TabIndex = 0;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.chkNoRead);
            this.groupControl1.Controls.Add(this.btnSelect);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.txtOneCode);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.txtEndTime);
            this.groupControl1.Controls.Add(this.txtStartTime);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(2, 2);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(1063, 178);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "查询条件";
            // 
            // chkNoRead
            // 
            this.chkNoRead.Location = new System.Drawing.Point(487, 77);
            this.chkNoRead.Name = "chkNoRead";
            this.chkNoRead.Properties.Caption = "仅显示NoRead条码";
            this.chkNoRead.Size = new System.Drawing.Size(146, 19);
            this.chkNoRead.TabIndex = 7;
            this.chkNoRead.CheckedChanged += new System.EventHandler(this.chkNoRead_CheckedChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.ImageOptions.Image")));
            this.btnSelect.Location = new System.Drawing.Point(714, 64);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 34);
            this.btnSelect.TabIndex = 6;
            this.btnSelect.Text = "查询";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(22, 81);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(137, 14);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "一号工程码/质量追溯码：";
            // 
            // txtOneCode
            // 
            this.txtOneCode.Location = new System.Drawing.Point(165, 78);
            this.txtOneCode.Name = "txtOneCode";
            this.txtOneCode.Size = new System.Drawing.Size(279, 20);
            this.txtOneCode.TabIndex = 4;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(259, 35);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(12, 14);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "---";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(22, 35);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 14);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "时间范围：";
            // 
            // txtEndTime
            // 
            this.txtEndTime.EditValue = null;
            this.txtEndTime.Location = new System.Drawing.Point(284, 32);
            this.txtEndTime.Name = "txtEndTime";
            this.txtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtEndTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtEndTime.Properties.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            this.txtEndTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtEndTime.Properties.EditFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            this.txtEndTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtEndTime.Properties.Mask.EditMask = "yyyy/MM/dd HH:mm:ss";
            this.txtEndTime.Size = new System.Drawing.Size(160, 20);
            this.txtEndTime.TabIndex = 1;
            // 
            // txtStartTime
            // 
            this.txtStartTime.EditValue = null;
            this.txtStartTime.Location = new System.Drawing.Point(88, 32);
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtStartTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtStartTime.Properties.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            this.txtStartTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtStartTime.Properties.EditFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            this.txtStartTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.txtStartTime.Properties.Mask.EditMask = "yyyy/MM/dd HH:mm:ss";
            this.txtStartTime.Size = new System.Drawing.Size(160, 20);
            this.txtStartTime.TabIndex = 0;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.groupControl2);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(0, 182);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1067, 498);
            this.panelControl2.TabIndex = 1;
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.gridControl1);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.Location = new System.Drawing.Point(2, 2);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(1063, 494);
            this.groupControl2.TabIndex = 0;
            this.groupControl2.Text = "信息显示";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 21);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1059, 471);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn4,
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn2});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.IndicatorWidth = 40;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator_1);
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "扫码时间";
            this.gridColumn4.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            this.gridColumn4.FieldName = "CREATETIME";
            this.gridColumn4.MaxWidth = 200;
            this.gridColumn4.MinWidth = 200;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 0;
            this.gridColumn4.Width = 200;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "一号工程码/质量追溯码";
            this.gridColumn1.FieldName = "ONECODE";
            this.gridColumn1.MinWidth = 200;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 200;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "识别码";
            this.gridColumn3.FieldName = "BRANDCODE";
            this.gridColumn3.MaxWidth = 200;
            this.gridColumn3.MinWidth = 200;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 200;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "封箱机";
            this.gridColumn2.FieldName = "FXNUMBER";
            this.gridColumn2.MaxWidth = 100;
            this.gridColumn2.MinWidth = 100;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 3;
            this.gridColumn2.Width = 100;
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.paginationControl1);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(0, 680);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(1067, 44);
            this.panelControl3.TabIndex = 2;
            // 
            // paginationControl1
            // 
            this.paginationControl1.CurrentPageIndex = 1;
            this.paginationControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paginationControl1.Location = new System.Drawing.Point(2, 2);
            this.paginationControl1.Name = "paginationControl1";
            this.paginationControl1.Size = new System.Drawing.Size(1063, 40);
            this.paginationControl1.TabIndex = 0;
            this.paginationControl1.TotalCount = 0;
            // 
            // FormScanOneCodeSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 724);
            this.Controls.Add(this.panelControl3);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormScanOneCodeSelect";
            this.Text = "FormScanOneCodeSelect";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkNoRead.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOneCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.DateEdit txtStartTime;
        private DevExpress.XtraEditors.DateEdit txtEndTime;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtOneCode;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnSelect;
        private DevExpress.XtraEditors.CheckEdit chkNoRead;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private CWCS.CommonControl.UIControl.PaginationControl paginationControl1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
    }
}