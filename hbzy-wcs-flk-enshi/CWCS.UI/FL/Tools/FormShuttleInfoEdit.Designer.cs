namespace CWCS.UI.FL.Tools
{
    partial class FormShuttleInfoEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        public System.ComponentModel.IContainer components = null;

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
        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShuttleInfoEdit));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.dateEnterLeaveTime = new DevExpress.XtraEditors.DateEdit();
            this.cmbCurrentStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtCurrentLayer = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtCurrentLine = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtSCNo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtShuttleNo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.cmbCurrentRackSide = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnterLeaveTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnterLeaveTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrentStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentLayer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentLine.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSCNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShuttleNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrentRackSide.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.groupControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(800, 450);
            this.panelControl1.TabIndex = 0;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.panelControl3);
            this.groupControl1.Controls.Add(this.panelControl2);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(2, 2);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(796, 446);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "详细信息";
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.btnCancel);
            this.panelControl3.Controls.Add(this.btnConfirm);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(2, 397);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(792, 47);
            this.panelControl3.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(573, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 36);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConfirm.ImageOptions.Image")));
            this.btnConfirm.Location = new System.Drawing.Point(688, 6);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(95, 36);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "确定修改";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.cmbCurrentRackSide);
            this.panelControl2.Controls.Add(this.dateEnterLeaveTime);
            this.panelControl2.Controls.Add(this.cmbCurrentStatus);
            this.panelControl2.Controls.Add(this.labelControl7);
            this.panelControl2.Controls.Add(this.labelControl6);
            this.panelControl2.Controls.Add(this.labelControl5);
            this.panelControl2.Controls.Add(this.txtCurrentLayer);
            this.panelControl2.Controls.Add(this.labelControl4);
            this.panelControl2.Controls.Add(this.txtCurrentLine);
            this.panelControl2.Controls.Add(this.labelControl3);
            this.panelControl2.Controls.Add(this.txtSCNo);
            this.panelControl2.Controls.Add(this.labelControl2);
            this.panelControl2.Controls.Add(this.txtShuttleNo);
            this.panelControl2.Controls.Add(this.labelControl1);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(2, 23);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(792, 374);
            this.panelControl2.TabIndex = 0;
            // 
            // dateEnterLeaveTime
            // 
            this.dateEnterLeaveTime.EditValue = null;
            this.dateEnterLeaveTime.Location = new System.Drawing.Point(181, 168);
            this.dateEnterLeaveTime.Name = "dateEnterLeaveTime";
            this.dateEnterLeaveTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEnterLeaveTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateEnterLeaveTime.Properties.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            this.dateEnterLeaveTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateEnterLeaveTime.Properties.EditFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            this.dateEnterLeaveTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dateEnterLeaveTime.Properties.Mask.EditMask = "yyyy/MM/dd HH:mm:ss";
            this.dateEnterLeaveTime.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.dateEnterLeaveTime.Size = new System.Drawing.Size(172, 20);
            this.dateEnterLeaveTime.TabIndex = 15;
            // 
            // cmbCurrentStatus
            // 
            this.cmbCurrentStatus.EditValue = "";
            this.cmbCurrentStatus.Location = new System.Drawing.Point(626, 41);
            this.cmbCurrentStatus.Name = "cmbCurrentStatus";
            this.cmbCurrentStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbCurrentStatus.Properties.EditValueChanged += new System.EventHandler(this.cmbCurrentStatus_Properties_EditValueChanged);
            this.cmbCurrentStatus.Size = new System.Drawing.Size(100, 20);
            this.cmbCurrentStatus.TabIndex = 14;
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(39, 171);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(120, 14);
            this.labelControl7.TabIndex = 12;
            this.labelControl7.Text = "进入或离开充电位时间";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(548, 44);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(72, 14);
            this.labelControl6.TabIndex = 10;
            this.labelControl6.Text = "当前位置状态";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(548, 103);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(60, 14);
            this.labelControl5.TabIndex = 8;
            this.labelControl5.Text = "当前货位侧";
            // 
            // txtCurrentLayer
            // 
            this.txtCurrentLayer.Location = new System.Drawing.Point(391, 100);
            this.txtCurrentLayer.Name = "txtCurrentLayer";
            this.txtCurrentLayer.Size = new System.Drawing.Size(100, 20);
            this.txtCurrentLayer.TabIndex = 7;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(315, 103);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(60, 14);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "当前货位层";
            // 
            // txtCurrentLine
            // 
            this.txtCurrentLine.Location = new System.Drawing.Point(115, 100);
            this.txtCurrentLine.Name = "txtCurrentLine";
            this.txtCurrentLine.Size = new System.Drawing.Size(100, 20);
            this.txtCurrentLine.TabIndex = 5;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(39, 103);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(60, 14);
            this.labelControl3.TabIndex = 4;
            this.labelControl3.Text = "当前货位行";
            // 
            // txtSCNo
            // 
            this.txtSCNo.Location = new System.Drawing.Point(391, 41);
            this.txtSCNo.Name = "txtSCNo";
            this.txtSCNo.Properties.ReadOnly = true;
            this.txtSCNo.Size = new System.Drawing.Size(100, 20);
            this.txtSCNo.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(315, 44);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 14);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "堆垛机编号";
            // 
            // txtShuttleNo
            // 
            this.txtShuttleNo.Enabled = false;
            this.txtShuttleNo.Location = new System.Drawing.Point(115, 41);
            this.txtShuttleNo.Name = "txtShuttleNo";
            this.txtShuttleNo.Properties.ReadOnly = true;
            this.txtShuttleNo.Size = new System.Drawing.Size(100, 20);
            this.txtShuttleNo.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(39, 44);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "穿梭板编号";
            // 
            // cmbCurrentRackSide
            // 
            this.cmbCurrentRackSide.EditValue = "";
            this.cmbCurrentRackSide.Location = new System.Drawing.Point(626, 100);
            this.cmbCurrentRackSide.Name = "cmbCurrentRackSide";
            this.cmbCurrentRackSide.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbCurrentRackSide.Size = new System.Drawing.Size(100, 20);
            this.cmbCurrentRackSide.TabIndex = 16;
            // 
            // FormShuttleInfoUpdatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelControl1);
            this.Name = "FormShuttleInfoUpdatePanel";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "穿梭板信息维护";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnterLeaveTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEnterLeaveTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrentStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentLayer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentLine.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSCNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShuttleNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCurrentRackSide.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraEditors.PanelControl panelControl1;
        public DevExpress.XtraEditors.GroupControl groupControl1;
        public DevExpress.XtraEditors.PanelControl panelControl3;
        public DevExpress.XtraEditors.PanelControl panelControl2;
        public DevExpress.XtraEditors.TextEdit txtShuttleNo;
        public DevExpress.XtraEditors.LabelControl labelControl1;
        public DevExpress.XtraEditors.TextEdit txtSCNo;
        public DevExpress.XtraEditors.LabelControl labelControl2;
        public DevExpress.XtraEditors.TextEdit txtCurrentLine;
        public DevExpress.XtraEditors.LabelControl labelControl3;
        public DevExpress.XtraEditors.TextEdit txtCurrentLayer;
        public DevExpress.XtraEditors.LabelControl labelControl4;
        public DevExpress.XtraEditors.LabelControl labelControl5;
        public DevExpress.XtraEditors.LabelControl labelControl6;
        public DevExpress.XtraEditors.LabelControl labelControl7;
        public DevExpress.XtraEditors.SimpleButton btnConfirm;
        public DevExpress.XtraEditors.SimpleButton btnCancel;
        public DevExpress.XtraEditors.ComboBoxEdit cmbCurrentStatus;
        public DevExpress.XtraEditors.DateEdit dateEnterLeaveTime;
        public DevExpress.XtraEditors.ComboBoxEdit cmbCurrentRackSide;
    }
}