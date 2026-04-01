namespace CWCS.UI
{
    partial class FrmSRMManager
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
            this.c1DockingTab1 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
            this.scControl1 = new CWCS.UIDesigner.SRMControl.SCControl();
            this.c1DockingTab2 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage2 = new C1.Win.C1Command.C1DockingTabPage();
            this.rtbCommunication = new System.Windows.Forms.RichTextBox();
            this.c1DockingTab3 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage3 = new C1.Win.C1Command.C1DockingTabPage();
            this.c1CommandHolder1 = new C1.Win.C1Command.C1CommandHolder();
            this.c1ContextMenu1 = new C1.Win.C1Command.C1ContextMenu();
            this.c1CommandLink2 = new C1.Win.C1Command.C1CommandLink();
            this.cmdSendTask = new C1.Win.C1Command.C1Command();
            this.c1CommandLink4 = new C1.Win.C1Command.C1CommandLink();
            this.cmdTaskMode = new C1.Win.C1Command.C1Command();
            this.cmdChangeLoc = new C1.Win.C1Command.C1Command();
            this.c1CommandLink6 = new C1.Win.C1Command.C1CommandLink();
            this.rtbCommand = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).BeginInit();
            this.c1DockingTab1.SuspendLayout();
            this.c1DockingTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab2)).BeginInit();
            this.c1DockingTab2.SuspendLayout();
            this.c1DockingTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab3)).BeginInit();
            this.c1DockingTab3.SuspendLayout();
            this.c1DockingTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).BeginInit();
            this.SuspendLayout();
            // 
            // c1DockingTab1
            // 
            this.c1DockingTab1.AcceptsCtrlTab = false;
            this.c1DockingTab1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.c1DockingTab1.CanMoveTabs = true;
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage1);
            this.c1DockingTab1.Dock = System.Windows.Forms.DockStyle.Top;
            this.c1DockingTab1.Indent = 0;
            this.c1DockingTab1.KeepClosedPages = false;
            this.c1DockingTab1.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTab1.Name = "c1DockingTab1";
            this.c1DockingTab1.ShowCaption = true;
            this.c1DockingTab1.Size = new System.Drawing.Size(918, 503);
            this.c1DockingTab1.TabIndex = 0;
            this.c1DockingTab1.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2007;
            this.c1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2007Blue;
            // 
            // c1DockingTabPage1
            // 
            this.c1DockingTabPage1.CaptionVisible = true;
            this.c1DockingTabPage1.Controls.Add(this.scControl1);
            this.c1DockingTabPage1.Location = new System.Drawing.Point(1, 1);
            this.c1DockingTabPage1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTabPage1.Name = "c1DockingTabPage1";
            this.c1DockingTabPage1.Size = new System.Drawing.Size(916, 477);
            this.c1DockingTabPage1.TabIndex = 0;
            this.c1DockingTabPage1.Text = "堆垛机监控";
            // 
            // scControl1
            // 
            this.scControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scControl1.Location = new System.Drawing.Point(0, 19);
            this.scControl1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.scControl1.Name = "scControl1";
            this.scControl1.Size = new System.Drawing.Size(916, 458);
            this.scControl1.TabIndex = 0;
            this.scControl1.SCMouseClick += new System.Windows.Forms.MouseEventHandler(this.scControl1_SCMouseClick);
            this.scControl1.SCChangWCSWorMode += new System.EventHandler(this.scControl1_SCChangWCSWorMode);
            // 
            // c1DockingTab2
            // 
            this.c1DockingTab2.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.c1DockingTab2.Controls.Add(this.c1DockingTabPage2);
            this.c1DockingTab2.Dock = System.Windows.Forms.DockStyle.Left;
            this.c1DockingTab2.Location = new System.Drawing.Point(0, 503);
            this.c1DockingTab2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTab2.Name = "c1DockingTab2";
            this.c1DockingTab2.Size = new System.Drawing.Size(475, 207);
            this.c1DockingTab2.TabIndex = 7;
            this.c1DockingTab2.TabSizeMode = C1.Win.C1Command.TabSizeModeEnum.Fit;
            this.c1DockingTab2.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2007;
            this.c1DockingTab2.TextDirection = C1.Win.C1Command.TabTextDirectionEnum.VerticalRight;
            this.c1DockingTab2.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2007Blue;
            // 
            // c1DockingTabPage2
            // 
            this.c1DockingTabPage2.Controls.Add(this.rtbCommunication);
            this.c1DockingTabPage2.Location = new System.Drawing.Point(25, 1);
            this.c1DockingTabPage2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTabPage2.Name = "c1DockingTabPage2";
            this.c1DockingTabPage2.Size = new System.Drawing.Size(449, 205);
            this.c1DockingTabPage2.TabIndex = 0;
            this.c1DockingTabPage2.Text = "堆垛机通讯日志";
            // 
            // rtbCommunication
            // 
            this.rtbCommunication.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbCommunication.Location = new System.Drawing.Point(0, 0);
            this.rtbCommunication.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.rtbCommunication.Name = "rtbCommunication";
            this.rtbCommunication.Size = new System.Drawing.Size(449, 205);
            this.rtbCommunication.TabIndex = 0;
            this.rtbCommunication.Text = "";
            // 
            // c1DockingTab3
            // 
            this.c1DockingTab3.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.c1DockingTab3.Controls.Add(this.c1DockingTabPage3);
            this.c1DockingTab3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1DockingTab3.Location = new System.Drawing.Point(475, 503);
            this.c1DockingTab3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTab3.Name = "c1DockingTab3";
            this.c1DockingTab3.Size = new System.Drawing.Size(443, 207);
            this.c1DockingTab3.TabIndex = 8;
            this.c1DockingTab3.TabSizeMode = C1.Win.C1Command.TabSizeModeEnum.Fit;
            this.c1DockingTab3.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2007;
            this.c1DockingTab3.TextDirection = C1.Win.C1Command.TabTextDirectionEnum.VerticalRight;
            this.c1DockingTab3.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2007Blue;
            // 
            // c1DockingTabPage3
            // 
            this.c1DockingTabPage3.Controls.Add(this.rtbCommand);
            this.c1DockingTabPage3.Location = new System.Drawing.Point(25, 1);
            this.c1DockingTabPage3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTabPage3.Name = "c1DockingTabPage3";
            this.c1DockingTabPage3.Size = new System.Drawing.Size(417, 205);
            this.c1DockingTabPage3.TabIndex = 0;
            this.c1DockingTabPage3.Text = "堆垛机命令日志";
            // 
            // c1CommandHolder1
            // 
            this.c1CommandHolder1.Commands.Add(this.c1ContextMenu1);
            this.c1CommandHolder1.Commands.Add(this.cmdSendTask);
            this.c1CommandHolder1.Commands.Add(this.cmdTaskMode);
            this.c1CommandHolder1.Commands.Add(this.cmdChangeLoc);
            this.c1CommandHolder1.Owner = this;
            this.c1CommandHolder1.CommandClick += new C1.Win.C1Command.CommandClickEventHandler(this.c1CommandHolder1_CommandClick);
            // 
            // c1ContextMenu1
            // 
            this.c1ContextMenu1.CommandLinks.AddRange(new C1.Win.C1Command.C1CommandLink[] {
            this.c1CommandLink2,
            this.c1CommandLink4});
            this.c1ContextMenu1.Name = "c1ContextMenu1";
            this.c1ContextMenu1.VisualStyleBase = C1.Win.C1Command.VisualStyle.OfficeXP;
            // 
            // c1CommandLink2
            // 
            this.c1CommandLink2.Command = this.cmdSendTask;
            // 
            // cmdSendTask
            // 
            this.cmdSendTask.Name = "cmdSendTask";
            this.cmdSendTask.Text = "手动任务";
            // 
            // c1CommandLink4
            // 
            this.c1CommandLink4.Command = this.cmdTaskMode;
            this.c1CommandLink4.SortOrder = 1;
            this.c1CommandLink4.Text = "任务模式";
            // 
            // cmdTaskMode
            // 
            this.cmdTaskMode.Name = "cmdTaskMode";
            this.cmdTaskMode.Text = "编辑任务模式";
            // 
            // cmdChangeLoc
            // 
            this.cmdChangeLoc.Name = "cmdChangeLoc";
            this.cmdChangeLoc.Text = "更换货位";
            // 
            // c1CommandLink6
            // 
            this.c1CommandLink6.Command = this.cmdChangeLoc;
            this.c1CommandLink6.SortOrder = 2;
            this.c1CommandLink6.Text = "更换巷道";
            // 
            // rtbCommand
            // 
            this.rtbCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbCommand.Location = new System.Drawing.Point(0, 0);
            this.rtbCommand.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.rtbCommand.Name = "rtbCommand";
            this.rtbCommand.Size = new System.Drawing.Size(417, 205);
            this.rtbCommand.TabIndex = 1;
            this.rtbCommand.Text = "";
            // 
            // FrmSRMManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 710);
            this.Controls.Add(this.c1DockingTab3);
            this.Controls.Add(this.c1DockingTab2);
            this.Controls.Add(this.c1DockingTab1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "FrmSRMManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "堆垛机管理";
            this.Load += new System.EventHandler(this.FrmSRMManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).EndInit();
            this.c1DockingTab1.ResumeLayout(false);
            this.c1DockingTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab2)).EndInit();
            this.c1DockingTab2.ResumeLayout(false);
            this.c1DockingTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab3)).EndInit();
            this.c1DockingTab3.ResumeLayout(false);
            this.c1DockingTabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private C1.Win.C1Command.C1DockingTab c1DockingTab1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        private UIDesigner.SRMControl.SCControl scControl1;
        private C1.Win.C1Command.C1DockingTab c1DockingTab2;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage2;
        private System.Windows.Forms.RichTextBox rtbCommunication;
        private C1.Win.C1Command.C1DockingTab c1DockingTab3;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage3;
        private C1.Win.C1Command.C1CommandHolder c1CommandHolder1;
        private C1.Win.C1Command.C1ContextMenu c1ContextMenu1;
        private C1.Win.C1Command.C1CommandLink c1CommandLink2;
        private C1.Win.C1Command.C1Command cmdSendTask;
        private C1.Win.C1Command.C1CommandLink c1CommandLink4;
        private C1.Win.C1Command.C1Command cmdTaskMode;
        private C1.Win.C1Command.C1Command cmdChangeLoc;
        private C1.Win.C1Command.C1CommandLink c1CommandLink6;
        private System.Windows.Forms.RichTextBox rtbCommand;
    }
}