using DevExpress.XtraEditors.Popup;

namespace CWCS.UI
{
    partial class FrmSRMSend
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
            this.btnSend = new C1.Win.C1Input.C1Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancle = new C1.Win.C1Input.C1Button();
            this.txtSCBarCode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSRMNO = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSCTaskNu = new System.Windows.Forms.TextBox();
            this.txtOldLoc = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.c1DockingTabPage3 = new C1.Win.C1Command.C1DockingTabPage();
            this.cmbTaskMode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.c1DockingTab1 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
            this.txtSCNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_to = new System.Windows.Forms.TextBox();
            this.txt_taskno = new System.Windows.Forms.TextBox();
            this.txt_from = new System.Windows.Forms.TextBox();
            this.cmbCommandType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lb_to = new System.Windows.Forms.Label();
            this.lb_taskno = new System.Windows.Forms.Label();
            this.lb_from = new System.Windows.Forms.Label();
            this.c1DockingTabPage2 = new C1.Win.C1Command.C1DockingTabPage();
            this.cmbCheckOut = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSCNO = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.c1DockingTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).BeginInit();
            this.c1DockingTab1.SuspendLayout();
            this.c1DockingTabPage1.SuspendLayout();
            this.c1DockingTabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(124, 19);
            this.btnSend.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(72, 25);
            this.btnSend.TabIndex = 66;
            this.btnSend.Text = "发送(&S)";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(253)))));
            this.panel1.Controls.Add(this.btnCancle);
            this.panel1.Controls.Add(this.btnSend);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 287);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(390, 82);
            this.panel1.TabIndex = 3;
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(206, 19);
            this.btnCancle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(72, 25);
            this.btnCancle.TabIndex = 67;
            this.btnCancle.Text = "取消(&C)";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // txtSCBarCode
            // 
            this.txtSCBarCode.Enabled = false;
            this.txtSCBarCode.Location = new System.Drawing.Point(161, 85);
            this.txtSCBarCode.Name = "txtSCBarCode";
            this.txtSCBarCode.Size = new System.Drawing.Size(137, 21);
            this.txtSCBarCode.TabIndex = 81;
            this.txtSCBarCode.Tag = "4";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(109, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 12);
            this.label7.TabIndex = 80;
            this.label7.Text = "托盘号:";
            // 
            // txtSRMNO
            // 
            this.txtSRMNO.Enabled = false;
            this.txtSRMNO.Location = new System.Drawing.Point(161, 33);
            this.txtSRMNO.Name = "txtSRMNO";
            this.txtSRMNO.Size = new System.Drawing.Size(137, 21);
            this.txtSRMNO.TabIndex = 79;
            this.txtSRMNO.Tag = "4";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(87, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 12);
            this.label8.TabIndex = 78;
            this.label8.Text = "堆垛机编号:";
            // 
            // txtSCTaskNu
            // 
            this.txtSCTaskNu.Enabled = false;
            this.txtSCTaskNu.Location = new System.Drawing.Point(161, 59);
            this.txtSCTaskNu.Name = "txtSCTaskNu";
            this.txtSCTaskNu.Size = new System.Drawing.Size(137, 21);
            this.txtSCTaskNu.TabIndex = 73;
            this.txtSCTaskNu.Tag = "3";
            // 
            // txtOldLoc
            // 
            this.txtOldLoc.Enabled = false;
            this.txtOldLoc.Location = new System.Drawing.Point(161, 111);
            this.txtOldLoc.Name = "txtOldLoc";
            this.txtOldLoc.Size = new System.Drawing.Size(137, 21);
            this.txtOldLoc.TabIndex = 74;
            this.txtOldLoc.Tag = "4";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(109, 61);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 12);
            this.label11.TabIndex = 70;
            this.label11.Text = "任务号:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Location = new System.Drawing.Point(109, 113);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 12);
            this.label12.TabIndex = 71;
            this.label12.Text = "原货位:";
            // 
            // c1DockingTabPage3
            // 
            this.c1DockingTabPage3.Controls.Add(this.txtSCBarCode);
            this.c1DockingTabPage3.Controls.Add(this.label7);
            this.c1DockingTabPage3.Controls.Add(this.txtSRMNO);
            this.c1DockingTabPage3.Controls.Add(this.label8);
            this.c1DockingTabPage3.Controls.Add(this.txtSCTaskNu);
            this.c1DockingTabPage3.Controls.Add(this.txtOldLoc);
            this.c1DockingTabPage3.Controls.Add(this.label11);
            this.c1DockingTabPage3.Controls.Add(this.label12);
            this.c1DockingTabPage3.Location = new System.Drawing.Point(1, 25);
            this.c1DockingTabPage3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTabPage3.Name = "c1DockingTabPage3";
            this.c1DockingTabPage3.Size = new System.Drawing.Size(388, 261);
            this.c1DockingTabPage3.TabIndex = 2;
            this.c1DockingTabPage3.Text = "更换货位";
            // 
            // cmbTaskMode
            // 
            this.cmbTaskMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTaskMode.FormattingEnabled = true;
            this.cmbTaskMode.Items.AddRange(new object[] {
            "可出可入",
            "只入不出",
            "只出不入",
            "暂停"});
            this.cmbTaskMode.Location = new System.Drawing.Point(167, 79);
            this.cmbTaskMode.Name = "cmbTaskMode";
            this.cmbTaskMode.Size = new System.Drawing.Size(137, 20);
            this.cmbTaskMode.TabIndex = 71;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(104, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 70;
            this.label6.Text = "任务模式:";
            // 
            // c1DockingTab1
            // 
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage1);
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage2);
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage3);
            this.c1DockingTab1.Dock = System.Windows.Forms.DockStyle.Top;
            this.c1DockingTab1.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTab1.Name = "c1DockingTab1";
            this.c1DockingTab1.SelectedIndex = 2;
            this.c1DockingTab1.Size = new System.Drawing.Size(390, 287);
            this.c1DockingTab1.TabIndex = 2;
            this.c1DockingTab1.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2007;
            this.c1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2007Blue;
            // 
            // c1DockingTabPage1
            // 
            this.c1DockingTabPage1.Controls.Add(this.txtSCNum);
            this.c1DockingTabPage1.Controls.Add(this.label2);
            this.c1DockingTabPage1.Controls.Add(this.txt_to);
            this.c1DockingTabPage1.Controls.Add(this.txt_taskno);
            this.c1DockingTabPage1.Controls.Add(this.txt_from);
            this.c1DockingTabPage1.Controls.Add(this.cmbCommandType);
            this.c1DockingTabPage1.Controls.Add(this.label3);
            this.c1DockingTabPage1.Controls.Add(this.lb_to);
            this.c1DockingTabPage1.Controls.Add(this.lb_taskno);
            this.c1DockingTabPage1.Controls.Add(this.lb_from);
            this.c1DockingTabPage1.Location = new System.Drawing.Point(1, 25);
            this.c1DockingTabPage1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTabPage1.Name = "c1DockingTabPage1";
            this.c1DockingTabPage1.Size = new System.Drawing.Size(388, 261);
            this.c1DockingTabPage1.TabIndex = 0;
            this.c1DockingTabPage1.Text = "手动任务";
            // 
            // txtSCNum
            // 
            this.txtSCNum.Enabled = false;
            this.txtSCNum.Location = new System.Drawing.Point(167, 33);
            this.txtSCNum.Name = "txtSCNum";
            this.txtSCNum.Size = new System.Drawing.Size(137, 21);
            this.txtSCNum.TabIndex = 67;
            this.txtSCNum.Tag = "4";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(75, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 66;
            this.label2.Text = "堆垛机编号:";
            // 
            // txt_to
            // 
            this.txt_to.Location = new System.Drawing.Point(168, 172);
            this.txt_to.Name = "txt_to";
            this.txt_to.Size = new System.Drawing.Size(137, 21);
            this.txt_to.TabIndex = 61;
            this.txt_to.Tag = "5";
            this.txt_to.Visible = false;
            // 
            // txt_taskno
            // 
            this.txt_taskno.Location = new System.Drawing.Point(167, 102);
            this.txt_taskno.Name = "txt_taskno";
            this.txt_taskno.Size = new System.Drawing.Size(137, 21);
            this.txt_taskno.TabIndex = 59;
            this.txt_taskno.Tag = "3";
            // 
            // txt_from
            // 
            this.txt_from.Location = new System.Drawing.Point(168, 137);
            this.txt_from.Name = "txt_from";
            this.txt_from.Size = new System.Drawing.Size(137, 21);
            this.txt_from.TabIndex = 60;
            this.txt_from.Tag = "4";
            this.txt_from.Visible = false;
            // 
            // cmbCommandType
            // 
            this.cmbCommandType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCommandType.FormattingEnabled = true;
            this.cmbCommandType.Items.AddRange(new object[] {
            "--请选择--",
            "取货指令",
            "放货指令",
            "移动指令",
            "清除指令"});
            this.cmbCommandType.Location = new System.Drawing.Point(167, 68);
            this.cmbCommandType.Name = "cmbCommandType";
            this.cmbCommandType.Size = new System.Drawing.Size(137, 20);
            this.cmbCommandType.TabIndex = 63;
            this.cmbCommandType.SelectedIndexChanged += new System.EventHandler(this.cmbCommandType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(87, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 62;
            this.label3.Text = "命令类型:";
            // 
            // lb_to
            // 
            this.lb_to.AutoSize = true;
            this.lb_to.BackColor = System.Drawing.Color.Transparent;
            this.lb_to.Location = new System.Drawing.Point(87, 175);
            this.lb_to.Name = "lb_to";
            this.lb_to.Size = new System.Drawing.Size(59, 12);
            this.lb_to.TabIndex = 58;
            this.lb_to.Text = "目标地址:";
            this.lb_to.Visible = false;
            // 
            // lb_taskno
            // 
            this.lb_taskno.AutoSize = true;
            this.lb_taskno.BackColor = System.Drawing.Color.Transparent;
            this.lb_taskno.Location = new System.Drawing.Point(99, 105);
            this.lb_taskno.Name = "lb_taskno";
            this.lb_taskno.Size = new System.Drawing.Size(47, 12);
            this.lb_taskno.TabIndex = 56;
            this.lb_taskno.Text = "任务号:";
            // 
            // lb_from
            // 
            this.lb_from.AutoSize = true;
            this.lb_from.BackColor = System.Drawing.Color.Transparent;
            this.lb_from.Location = new System.Drawing.Point(87, 140);
            this.lb_from.Name = "lb_from";
            this.lb_from.Size = new System.Drawing.Size(59, 12);
            this.lb_from.TabIndex = 57;
            this.lb_from.Text = "起始地址:";
            this.lb_from.Visible = false;
            // 
            // c1DockingTabPage2
            // 
            this.c1DockingTabPage2.Controls.Add(this.cmbCheckOut);
            this.c1DockingTabPage2.Controls.Add(this.label4);
            this.c1DockingTabPage2.Controls.Add(this.cmbTaskMode);
            this.c1DockingTabPage2.Controls.Add(this.label6);
            this.c1DockingTabPage2.Controls.Add(this.txtSCNO);
            this.c1DockingTabPage2.Controls.Add(this.label5);
            this.c1DockingTabPage2.Location = new System.Drawing.Point(1, 25);
            this.c1DockingTabPage2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.c1DockingTabPage2.Name = "c1DockingTabPage2";
            this.c1DockingTabPage2.Size = new System.Drawing.Size(388, 261);
            this.c1DockingTabPage2.TabIndex = 1;
            this.c1DockingTabPage2.Text = "任务模式";
            // 
            // cmbCheckOut
            // 
            this.cmbCheckOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCheckOut.FormattingEnabled = true;
            this.cmbCheckOut.Items.AddRange(new object[] {
            "正常出库",
            "快速出库"});
            this.cmbCheckOut.Location = new System.Drawing.Point(167, 119);
            this.cmbCheckOut.Name = "cmbCheckOut";
            this.cmbCheckOut.Size = new System.Drawing.Size(137, 20);
            this.cmbCheckOut.TabIndex = 73;
            this.cmbCheckOut.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(104, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 72;
            this.label4.Text = "出库模式:";
            this.label4.Visible = false;
            // 
            // txtSCNO
            // 
            this.txtSCNO.Enabled = false;
            this.txtSCNO.Location = new System.Drawing.Point(167, 39);
            this.txtSCNO.Name = "txtSCNO";
            this.txtSCNO.Size = new System.Drawing.Size(137, 21);
            this.txtSCNO.TabIndex = 69;
            this.txtSCNO.Tag = "4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(93, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 12);
            this.label5.TabIndex = 68;
            this.label5.Text = "堆垛机编号:";
            // 
            // FrmSRMSend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 369);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.c1DockingTab1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "FrmSRMSend";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.FrmSRMSend_Load);
            this.panel1.ResumeLayout(false);
            this.c1DockingTabPage3.ResumeLayout(false);
            this.c1DockingTabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).EndInit();
            this.c1DockingTab1.ResumeLayout(false);
            this.c1DockingTabPage1.ResumeLayout(false);
            this.c1DockingTabPage1.PerformLayout();
            this.c1DockingTabPage2.ResumeLayout(false);
            this.c1DockingTabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private C1.Win.C1Input.C1Button btnSend;
        private System.Windows.Forms.Panel panel1;
        private C1.Win.C1Input.C1Button btnCancle;
        private System.Windows.Forms.TextBox txtSCBarCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSRMNO;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSCTaskNu;
        private System.Windows.Forms.TextBox txtOldLoc;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage3;
        private System.Windows.Forms.ComboBox cmbTaskMode;
        private System.Windows.Forms.Label label6;
        private C1.Win.C1Command.C1DockingTab c1DockingTab1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        private System.Windows.Forms.TextBox txtSCNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_to;
        private System.Windows.Forms.TextBox txt_taskno;
        private System.Windows.Forms.TextBox txt_from;
        private System.Windows.Forms.ComboBox cmbCommandType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lb_to;
        private System.Windows.Forms.Label lb_taskno;
        private System.Windows.Forms.Label lb_from;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage2;
        private System.Windows.Forms.TextBox txtSCNO;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbCheckOut;
        private System.Windows.Forms.Label label4;
    }
}