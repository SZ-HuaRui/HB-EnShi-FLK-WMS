namespace CWCS.UI
{
    partial class FormSystemConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSystemConfig));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.COL_CAN_EDIT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL_PARAM_CODE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL_PARAM_NAME = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL_PARAM_TYPE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL_PARAM_VALUE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.COL_T_ID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbFlowValue = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtParamName = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbFlowValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParamName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(566, 176, 650, 400);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(800, 450);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.groupControl2);
            this.panelControl1.Controls.Add(this.groupControl1);
            this.panelControl1.Location = new System.Drawing.Point(12, 12);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(776, 426);
            this.panelControl1.TabIndex = 4;
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.gridControl1);
            this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl2.Location = new System.Drawing.Point(2, 158);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(772, 266);
            this.groupControl2.TabIndex = 1;
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
            this.gridControl1.Size = new System.Drawing.Size(768, 241);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.Lime;
            this.gridView1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.COL_CAN_EDIT,
            this.COL_PARAM_CODE,
            this.COL_PARAM_NAME,
            this.COL_PARAM_TYPE,
            this.COL_PARAM_VALUE,
            this.COL_T_ID});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            // 
            // COL_CAN_EDIT
            // 
            this.COL_CAN_EDIT.Caption = "是否可编辑";
            this.COL_CAN_EDIT.FieldName = "CAN_EDIT";
            this.COL_CAN_EDIT.Name = "COL_CAN_EDIT";
            this.COL_CAN_EDIT.OptionsColumn.AllowEdit = false;
            this.COL_CAN_EDIT.Visible = true;
            this.COL_CAN_EDIT.VisibleIndex = 4;
            // 
            // COL_PARAM_CODE
            // 
            this.COL_PARAM_CODE.Caption = "参数编码";
            this.COL_PARAM_CODE.FieldName = "PARAM_CODE";
            this.COL_PARAM_CODE.Name = "COL_PARAM_CODE";
            this.COL_PARAM_CODE.OptionsColumn.AllowEdit = false;
            this.COL_PARAM_CODE.Visible = true;
            this.COL_PARAM_CODE.VisibleIndex = 0;
            // 
            // COL_PARAM_NAME
            // 
            this.COL_PARAM_NAME.Caption = "参数名称";
            this.COL_PARAM_NAME.FieldName = "PARAM_NAME";
            this.COL_PARAM_NAME.Name = "COL_PARAM_NAME";
            this.COL_PARAM_NAME.OptionsColumn.AllowEdit = false;
            this.COL_PARAM_NAME.Visible = true;
            this.COL_PARAM_NAME.VisibleIndex = 1;
            // 
            // COL_PARAM_TYPE
            // 
            this.COL_PARAM_TYPE.Caption = "参数类型";
            this.COL_PARAM_TYPE.FieldName = "PARAM_TYPE";
            this.COL_PARAM_TYPE.Name = "COL_PARAM_TYPE";
            this.COL_PARAM_TYPE.OptionsColumn.AllowEdit = false;
            this.COL_PARAM_TYPE.Visible = true;
            this.COL_PARAM_TYPE.VisibleIndex = 2;
            // 
            // COL_PARAM_VALUE
            // 
            this.COL_PARAM_VALUE.Caption = "参数值";
            this.COL_PARAM_VALUE.FieldName = "PARAM_VALUE";
            this.COL_PARAM_VALUE.Name = "COL_PARAM_VALUE";
            this.COL_PARAM_VALUE.Visible = true;
            this.COL_PARAM_VALUE.VisibleIndex = 3;
            // 
            // COL_T_ID
            // 
            this.COL_T_ID.Caption = "PARAMID";
            this.COL_T_ID.FieldName = "T_ID";
            this.COL_T_ID.Name = "COL_T_ID";
            // 
            // cmbFlowValue
            // 
            this.cmbFlowValue.Name = "cmbFlowValue";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.txtParamName);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.btnSelect);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.Location = new System.Drawing.Point(2, 2);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(772, 156);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "查询条件";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.BackColor = System.Drawing.Color.Red;
            this.labelControl1.Appearance.Options.UseBackColor = true;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Location = new System.Drawing.Point(25, 53);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(350, 25);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "备注：请谨慎修改系统配置参数，修改错误可能导致系统异常";
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.ImageOptions.Image")));
            this.btnSelect.Location = new System.Drawing.Point(490, 87);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(79, 39);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "查询";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(800, 450);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(780, 430);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(25, 99);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 14);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "参数名称";
            // 
            // txtParamName
            // 
            this.txtParamName.Location = new System.Drawing.Point(89, 96);
            this.txtParamName.Name = "txtParamName";
            this.txtParamName.Size = new System.Drawing.Size(286, 20);
            this.txtParamName.TabIndex = 3;
            // 
            // FormSystemConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSystemConfig";
            this.Text = "FormFlowConfig";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbFlowValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParamName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.SimpleButton btnSelect;
        private DevExpress.XtraGrid.Columns.GridColumn COL_PARAM_CODE;
        private DevExpress.XtraGrid.Columns.GridColumn COL_PARAM_NAME;
        private DevExpress.XtraGrid.Columns.GridColumn COL_PARAM_TYPE;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbFlowValue;
        private DevExpress.XtraGrid.Columns.GridColumn COL_T_ID;
        private DevExpress.XtraGrid.Columns.GridColumn COL_PARAM_VALUE;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.Columns.GridColumn COL_CAN_EDIT;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtParamName;
    }
}