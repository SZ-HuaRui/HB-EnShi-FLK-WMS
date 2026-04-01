namespace CWCS.UI
{
    partial class FormAGVTaskUpdate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAGVTaskUpdate));
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTaskNO = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFromStation = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colToStation = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCanExe = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTaskStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAGVIndex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAGVCarNO = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colT_ID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBarcode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL_EQUIPMENTNUMBER = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grpCondition = new DevExpress.XtraEditors.GroupControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtBarcode = new DevExpress.XtraEditors.TextEdit();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtTaskNO = new DevExpress.XtraEditors.TextEdit();
            this.simpleLabelItem1 = new DevExpress.XtraLayout.SimpleLabelItem();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtAgvIndex = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpCondition)).BeginInit();
            this.grpCondition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaskNO.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgvIndex.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.gridControl1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(2, 2);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(1087, 272);
            this.groupControl1.TabIndex = 5;
            this.groupControl1.Text = "任务显示";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(2, 23);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1083, 247);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.Lime;
            this.gridView1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTaskNO,
            this.colFromStation,
            this.colToStation,
            this.colCanExe,
            this.colTaskStatus,
            this.colAGVIndex,
            this.colAGVCarNO,
            this.colT_ID,
            this.colBarcode,
            this.COL_EQUIPMENTNUMBER});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.IndicatorWidth = 40;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            // 
            // colTaskNO
            // 
            this.colTaskNO.Caption = "任务号";
            this.colTaskNO.FieldName = "TASKNO";
            this.colTaskNO.Name = "colTaskNO";
            this.colTaskNO.OptionsColumn.AllowEdit = false;
            this.colTaskNO.Visible = true;
            this.colTaskNO.VisibleIndex = 0;
            // 
            // colFromStation
            // 
            this.colFromStation.Caption = "起始地址";
            this.colFromStation.FieldName = "FROMSTATION";
            this.colFromStation.Name = "colFromStation";
            this.colFromStation.OptionsColumn.AllowEdit = false;
            this.colFromStation.Visible = true;
            this.colFromStation.VisibleIndex = 1;
            // 
            // colToStation
            // 
            this.colToStation.Caption = "目标地址";
            this.colToStation.FieldName = "TOSTATION";
            this.colToStation.Name = "colToStation";
            this.colToStation.OptionsColumn.AllowEdit = false;
            this.colToStation.Visible = true;
            this.colToStation.VisibleIndex = 2;
            // 
            // colCanExe
            // 
            this.colCanExe.Caption = "是否可执行";
            this.colCanExe.FieldName = "RUNSTATUS";
            this.colCanExe.Name = "colCanExe";
            this.colCanExe.OptionsColumn.AllowEdit = false;
            this.colCanExe.Visible = true;
            this.colCanExe.VisibleIndex = 3;
            // 
            // colTaskStatus
            // 
            this.colTaskStatus.Caption = "任务状态";
            this.colTaskStatus.FieldName = "TASKSTATUS";
            this.colTaskStatus.Name = "colTaskStatus";
            this.colTaskStatus.OptionsColumn.AllowEdit = false;
            this.colTaskStatus.Visible = true;
            this.colTaskStatus.VisibleIndex = 4;
            // 
            // colAGVIndex
            // 
            this.colAGVIndex.Caption = "AGV索引号";
            this.colAGVIndex.FieldName = "AGVINDEX";
            this.colAGVIndex.Name = "colAGVIndex";
            this.colAGVIndex.Visible = true;
            this.colAGVIndex.VisibleIndex = 5;
            // 
            // colAGVCarNO
            // 
            this.colAGVCarNO.Caption = "AGV车号";
            this.colAGVCarNO.FieldName = "AGVCARNO";
            this.colAGVCarNO.Name = "colAGVCarNO";
            this.colAGVCarNO.Visible = true;
            this.colAGVCarNO.VisibleIndex = 6;
            // 
            // colT_ID
            // 
            this.colT_ID.Caption = "唯一值";
            this.colT_ID.FieldName = "T_ID";
            this.colT_ID.Name = "colT_ID";
            // 
            // colBarcode
            // 
            this.colBarcode.Caption = "托盘号";
            this.colBarcode.FieldName = "BARCODE";
            this.colBarcode.Name = "colBarcode";
            this.colBarcode.OptionsColumn.AllowEdit = false;
            this.colBarcode.Visible = true;
            this.colBarcode.VisibleIndex = 7;
            // 
            // COL_EQUIPMENTNUMBER
            // 
            this.COL_EQUIPMENTNUMBER.Caption = "设备号";
            this.COL_EQUIPMENTNUMBER.FieldName = "EQUIPMENTNUMBER";
            this.COL_EQUIPMENTNUMBER.Name = "COL_EQUIPMENTNUMBER";
            this.COL_EQUIPMENTNUMBER.OptionsColumn.AllowEdit = false;
            this.COL_EQUIPMENTNUMBER.Visible = true;
            this.COL_EQUIPMENTNUMBER.VisibleIndex = 8;
            // 
            // grpCondition
            // 
            this.grpCondition.Controls.Add(this.labelControl3);
            this.grpCondition.Controls.Add(this.txtAgvIndex);
            this.grpCondition.Controls.Add(this.labelControl2);
            this.grpCondition.Controls.Add(this.txtBarcode);
            this.grpCondition.Controls.Add(this.btnSelect);
            this.grpCondition.Controls.Add(this.labelControl1);
            this.grpCondition.Controls.Add(this.txtTaskNO);
            this.grpCondition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpCondition.Location = new System.Drawing.Point(2, 2);
            this.grpCondition.Name = "grpCondition";
            this.grpCondition.Size = new System.Drawing.Size(1087, 170);
            this.grpCondition.TabIndex = 4;
            this.grpCondition.Text = "查询条件";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(250, 91);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 14);
            this.labelControl2.TabIndex = 6;
            this.labelControl2.Text = "托盘号：";
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(304, 88);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(99, 20);
            this.txtBarcode.TabIndex = 5;
            // 
            // btnSelect
            // 
            this.btnSelect.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.ImageOptions.Image")));
            this.btnSelect.Location = new System.Drawing.Point(663, 77);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 32);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "查询";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(52, 92);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 14);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "任务号：";
            // 
            // txtTaskNO
            // 
            this.txtTaskNO.Location = new System.Drawing.Point(106, 89);
            this.txtTaskNO.Name = "txtTaskNO";
            this.txtTaskNO.Size = new System.Drawing.Size(99, 20);
            this.txtTaskNO.TabIndex = 0;
            // 
            // simpleLabelItem1
            // 
            this.simpleLabelItem1.AllowHotTrack = false;
            this.simpleLabelItem1.Location = new System.Drawing.Point(0, 0);
            this.simpleLabelItem1.Name = "simpleLabelItem1";
            this.simpleLabelItem1.Size = new System.Drawing.Size(780, 430);
            this.simpleLabelItem1.TextSize = new System.Drawing.Size(122, 14);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.grpCondition);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1091, 174);
            this.panelControl1.TabIndex = 6;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.groupControl1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 174);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1091, 276);
            this.panelControl2.TabIndex = 7;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(448, 91);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(72, 14);
            this.labelControl3.TabIndex = 8;
            this.labelControl3.Text = "AGV索引号：";
            // 
            // txtAgvIndex
            // 
            this.txtAgvIndex.Location = new System.Drawing.Point(526, 88);
            this.txtAgvIndex.Name = "txtAgvIndex";
            this.txtAgvIndex.Size = new System.Drawing.Size(99, 20);
            this.txtAgvIndex.TabIndex = 7;
            // 
            // FormAGVTaskUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 450);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormAGVTaskUpdate";
            this.Text = "FormAGVTaskUpdate";
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpCondition)).EndInit();
            this.grpCondition.ResumeLayout(false);
            this.grpCondition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaskNO.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleLabelItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAgvIndex.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraLayout.SimpleLabelItem simpleLabelItem1;
        private DevExpress.XtraEditors.GroupControl grpCondition;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtTaskNO;
        private DevExpress.XtraEditors.SimpleButton btnSelect;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colTaskNO;
        private DevExpress.XtraGrid.Columns.GridColumn colFromStation;
        private DevExpress.XtraGrid.Columns.GridColumn colToStation;
        private DevExpress.XtraGrid.Columns.GridColumn colCanExe;
        private DevExpress.XtraGrid.Columns.GridColumn colTaskStatus;
        private DevExpress.XtraGrid.Columns.GridColumn colAGVIndex;
        private DevExpress.XtraGrid.Columns.GridColumn colAGVCarNO;
        private DevExpress.XtraGrid.Columns.GridColumn colT_ID;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtBarcode;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraGrid.Columns.GridColumn colBarcode;
        private DevExpress.XtraGrid.Columns.GridColumn COL_EQUIPMENTNUMBER;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtAgvIndex;
    }
}