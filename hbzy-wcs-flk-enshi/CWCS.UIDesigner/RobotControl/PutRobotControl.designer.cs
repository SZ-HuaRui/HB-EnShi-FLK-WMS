namespace CWCS.UIDesigner.RobotControl
{
    partial class PutRobotControl
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gbTop = new System.Windows.Forms.GroupBox();
            this.txtBindCommand = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBindingCount = new System.Windows.Forms.TextBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.txtBindingTray = new System.Windows.Forms.TextBox();
            this.txtBindingStatus = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gbBottom = new System.Windows.Forms.GroupBox();
            this.c1DockingTab14 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage18 = new C1.Win.C1Command.C1DockingTabPage();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.trayControl1 = new CWCS.UIDesigner.RobotControl.TrayControl();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbTop.SuspendLayout();
            this.gbBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab14)).BeginInit();
            this.c1DockingTab14.SuspendLayout();
            this.c1DockingTabPage18.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(492, 769);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "组盘机械手";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.gbTop, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gbBottom, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.trayControl1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 23);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(486, 744);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // gbTop
            // 
            this.gbTop.Controls.Add(this.txtBindCommand);
            this.gbTop.Controls.Add(this.label8);
            this.gbTop.Controls.Add(this.txtBindingCount);
            this.gbTop.Controls.Add(this.lblCount);
            this.gbTop.Controls.Add(this.txtBindingTray);
            this.gbTop.Controls.Add(this.txtBindingStatus);
            this.gbTop.Controls.Add(this.label2);
            this.gbTop.Controls.Add(this.label3);
            this.gbTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTop.Location = new System.Drawing.Point(3, 4);
            this.gbTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbTop.Name = "gbTop";
            this.gbTop.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbTop.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gbTop.Size = new System.Drawing.Size(480, 140);
            this.gbTop.TabIndex = 8;
            this.gbTop.TabStop = false;
            // 
            // txtBindCommand
            // 
            this.txtBindCommand.Enabled = false;
            this.txtBindCommand.Location = new System.Drawing.Point(307, 84);
            this.txtBindCommand.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBindCommand.Name = "txtBindCommand";
            this.txtBindCommand.Size = new System.Drawing.Size(87, 28);
            this.txtBindCommand.TabIndex = 45;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(206, 88);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 18);
            this.label8.TabIndex = 44;
            this.label8.Text = "交互命令：";
            // 
            // txtBindingCount
            // 
            this.txtBindingCount.Location = new System.Drawing.Point(109, 84);
            this.txtBindingCount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBindingCount.Name = "txtBindingCount";
            this.txtBindingCount.Size = new System.Drawing.Size(87, 28);
            this.txtBindingCount.TabIndex = 35;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCount.Location = new System.Drawing.Point(8, 88);
            this.lblCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(98, 18);
            this.lblCount.TabIndex = 34;
            this.lblCount.Text = "当前数量：";
            // 
            // txtBindingTray
            // 
            this.txtBindingTray.Enabled = false;
            this.txtBindingTray.Location = new System.Drawing.Point(109, 35);
            this.txtBindingTray.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBindingTray.Name = "txtBindingTray";
            this.txtBindingTray.Size = new System.Drawing.Size(87, 28);
            this.txtBindingTray.TabIndex = 31;
            // 
            // txtBindingStatus
            // 
            this.txtBindingStatus.Enabled = false;
            this.txtBindingStatus.Location = new System.Drawing.Point(307, 35);
            this.txtBindingStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBindingStatus.Name = "txtBindingStatus";
            this.txtBindingStatus.Size = new System.Drawing.Size(87, 28);
            this.txtBindingStatus.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(42, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 30;
            this.label2.Text = "托盘：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(206, 38);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 18);
            this.label3.TabIndex = 32;
            this.label3.Text = "运行状态：";
            // 
            // gbBottom
            // 
            this.gbBottom.Controls.Add(this.c1DockingTab14);
            this.gbBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbBottom.Location = new System.Drawing.Point(3, 522);
            this.gbBottom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbBottom.Name = "gbBottom";
            this.gbBottom.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gbBottom.Size = new System.Drawing.Size(480, 220);
            this.gbBottom.TabIndex = 2;
            this.gbBottom.TabStop = false;
            // 
            // c1DockingTab14
            // 
            this.c1DockingTab14.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.c1DockingTab14.AutoHiding = true;
            this.c1DockingTab14.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.c1DockingTab14.CanAutoHide = true;
            this.c1DockingTab14.CanMoveTabs = true;
            this.c1DockingTab14.Controls.Add(this.c1DockingTabPage18);
            this.c1DockingTab14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1DockingTab14.KeepClosedPages = false;
            this.c1DockingTab14.Location = new System.Drawing.Point(3, 23);
            this.c1DockingTab14.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.c1DockingTab14.Name = "c1DockingTab14";
            this.c1DockingTab14.ShowCaption = true;
            this.c1DockingTab14.ShowToolTips = true;
            this.c1DockingTab14.Size = new System.Drawing.Size(474, 195);
            this.c1DockingTab14.TabIndex = 5;
            this.c1DockingTab14.TabSizeMode = C1.Win.C1Command.TabSizeModeEnum.Fit;
            this.c1DockingTab14.TabsSpacing = 5;
            this.c1DockingTab14.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2010;
            this.c1DockingTab14.TextDirection = C1.Win.C1Command.TabTextDirectionEnum.Horizontal;
            this.c1DockingTab14.VisualStyle = C1.Win.C1Command.VisualStyle.Office2010Blue;
            this.c1DockingTab14.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2010Blue;
            // 
            // c1DockingTabPage18
            // 
            this.c1DockingTabPage18.CaptionVisible = true;
            this.c1DockingTabPage18.Controls.Add(this.txtLog);
            this.c1DockingTabPage18.Location = new System.Drawing.Point(1, 1);
            this.c1DockingTabPage18.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.c1DockingTabPage18.Name = "c1DockingTabPage18";
            this.c1DockingTabPage18.Size = new System.Drawing.Size(472, 164);
            this.c1DockingTabPage18.TabIndex = 0;
            this.c1DockingTabPage18.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 26);
            this.txtLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(472, 138);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // trayControl1
            // 
            this.trayControl1.ContainerCount = 64;
            this.trayControl1.Location = new System.Drawing.Point(3, 151);
            this.trayControl1.Name = "trayControl1";
            this.trayControl1.Size = new System.Drawing.Size(480, 366);
            this.trayControl1.TabIndex = 9;
            this.trayControl1.Text = "trayControl1";
            this.trayControl1.TrayFontSize = 10;
            this.trayControl1.TrayHeight = 150;
            this.trayControl1.TrayWidth = 300;
            // 
            // PutRobotControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PutRobotControl";
            this.Size = new System.Drawing.Size(492, 769);
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gbTop.ResumeLayout(false);
            this.gbTop.PerformLayout();
            this.gbBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab14)).EndInit();
            this.c1DockingTab14.ResumeLayout(false);
            this.c1DockingTabPage18.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gbBottom;
        private System.Windows.Forms.GroupBox gbTop;
        private System.Windows.Forms.TextBox txtBindCommand;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBindingCount;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.TextBox txtBindingTray;
        private System.Windows.Forms.TextBox txtBindingStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private C1.Win.C1Command.C1DockingTab c1DockingTab14;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage18;
        private System.Windows.Forms.RichTextBox txtLog;
        private TrayControl trayControl1;
    }
}
