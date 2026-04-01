namespace CWCS.UIDesigner.RobotControl
{
    partial class BatteryControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatteryControl));
            this.c1DockingTab17 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage21 = new C1.Win.C1Command.C1DockingTabPage();
            this.c1FlexGrid3 = new C1.Win.C1FlexGrid.C1FlexGrid();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab17)).BeginInit();
            this.c1DockingTab17.SuspendLayout();
            this.c1DockingTabPage21.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid3)).BeginInit();
            this.SuspendLayout();
            // 
            // c1DockingTab17
            // 
            this.c1DockingTab17.Controls.Add(this.c1DockingTabPage21);
            this.c1DockingTab17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1DockingTab17.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab17.Name = "c1DockingTab17";
            this.c1DockingTab17.Size = new System.Drawing.Size(740, 584);
            this.c1DockingTab17.TabIndex = 6;
            this.c1DockingTab17.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2010;
            this.c1DockingTab17.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2010Blue;
            // 
            // c1DockingTabPage21
            // 
            this.c1DockingTabPage21.Controls.Add(this.c1FlexGrid3);
            this.c1DockingTabPage21.Location = new System.Drawing.Point(1, 27);
            this.c1DockingTabPage21.Name = "c1DockingTabPage21";
            this.c1DockingTabPage21.Size = new System.Drawing.Size(738, 556);
            this.c1DockingTabPage21.TabIndex = 0;
            this.c1DockingTabPage21.Text = "BatteryInfo";
            // 
            // c1FlexGrid3
            // 
            this.c1FlexGrid3.ColumnInfo = resources.GetString("c1FlexGrid3.ColumnInfo");
            this.c1FlexGrid3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1FlexGrid3.Location = new System.Drawing.Point(0, 0);
            this.c1FlexGrid3.Name = "c1FlexGrid3";
            this.c1FlexGrid3.Rows.Count = 1;
            this.c1FlexGrid3.Rows.DefaultSize = 24;
            this.c1FlexGrid3.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.ListBox;
            this.c1FlexGrid3.ShowCursor = true;
            this.c1FlexGrid3.Size = new System.Drawing.Size(738, 556);
            this.c1FlexGrid3.TabIndex = 2;
            this.c1FlexGrid3.VisualStyle = C1.Win.C1FlexGrid.VisualStyle.Office2010Blue;
            // 
            // BatteryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 584);
            this.Controls.Add(this.c1DockingTab17);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BatteryControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "电芯信息";
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab17)).EndInit();
            this.c1DockingTab17.ResumeLayout(false);
            this.c1DockingTabPage21.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private C1.Win.C1Command.C1DockingTab c1DockingTab17;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage21;
        private C1.Win.C1FlexGrid.C1FlexGrid c1FlexGrid3;
    }
}
