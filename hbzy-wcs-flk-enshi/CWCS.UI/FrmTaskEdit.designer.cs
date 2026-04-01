namespace CWCS.UI
{
    partial class FrmTaskEdit
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
            this.label2 = new System.Windows.Forms.Label();
            this.txt_taskno = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConfirm = new C1.Win.C1Input.C1Button();
            this.btnCancle = new C1.Win.C1Input.C1Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBarCode = new System.Windows.Forms.TextBox();
            this.cbb_taskstatus = new System.Windows.Forms.ComboBox();
            this.cbb_runstatus = new System.Windows.Forms.ComboBox();
            this.txt_tostation = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_fromstation = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(136, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "任务号：";
            // 
            // txt_taskno
            // 
            this.txt_taskno.Enabled = false;
            this.txt_taskno.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_taskno.ForeColor = System.Drawing.Color.Blue;
            this.txt_taskno.Location = new System.Drawing.Point(230, 24);
            this.txt_taskno.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_taskno.Name = "txt_taskno";
            this.txt_taskno.ReadOnly = true;
            this.txt_taskno.Size = new System.Drawing.Size(182, 35);
            this.txt_taskno.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(127, 253);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "任务状态：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(127, 315);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "执行状态：";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(51, 401);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(141, 41);
            this.btnConfirm.TabIndex = 8;
            this.btnConfirm.Text = "确定(&O)";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(321, 401);
            this.btnCancle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(141, 41);
            this.btnCancle.TabIndex = 9;
            this.btnCancle.Text = "取消(&C)";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(136, 85);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "托盘号：";
            // 
            // txtBarCode
            // 
            this.txtBarCode.Enabled = false;
            this.txtBarCode.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtBarCode.ForeColor = System.Drawing.Color.Blue;
            this.txtBarCode.Location = new System.Drawing.Point(230, 77);
            this.txtBarCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBarCode.Name = "txtBarCode";
            this.txtBarCode.ReadOnly = true;
            this.txtBarCode.Size = new System.Drawing.Size(182, 35);
            this.txtBarCode.TabIndex = 3;
            // 
            // cbb_taskstatus
            // 
            this.cbb_taskstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_taskstatus.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbb_taskstatus.ForeColor = System.Drawing.Color.Blue;
            this.cbb_taskstatus.FormattingEnabled = true;
            this.cbb_taskstatus.Items.AddRange(new object[] {
            "0(未开始)",
            "1(已完成)",
            "2(执行中)"});
            this.cbb_taskstatus.Location = new System.Drawing.Point(227, 246);
            this.cbb_taskstatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbb_taskstatus.Name = "cbb_taskstatus";
            this.cbb_taskstatus.Size = new System.Drawing.Size(185, 32);
            this.cbb_taskstatus.TabIndex = 6;
            // 
            // cbb_runstatus
            // 
            this.cbb_runstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_runstatus.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbb_runstatus.ForeColor = System.Drawing.Color.Blue;
            this.cbb_runstatus.FormattingEnabled = true;
            this.cbb_runstatus.Items.AddRange(new object[] {
            "不可执行",
            "可执行"});
            this.cbb_runstatus.Location = new System.Drawing.Point(227, 307);
            this.cbb_runstatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbb_runstatus.Name = "cbb_runstatus";
            this.cbb_runstatus.Size = new System.Drawing.Size(185, 32);
            this.cbb_runstatus.TabIndex = 8;
            // 
            // txt_tostation
            // 
            this.txt_tostation.Enabled = false;
            this.txt_tostation.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_tostation.ForeColor = System.Drawing.Color.Blue;
            this.txt_tostation.Location = new System.Drawing.Point(228, 185);
            this.txt_tostation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_tostation.Name = "txt_tostation";
            this.txt_tostation.Size = new System.Drawing.Size(184, 35);
            this.txt_tostation.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(127, 194);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 18);
            this.label9.TabIndex = 16;
            this.label9.Text = "目标站台：";
            // 
            // txt_fromstation
            // 
            this.txt_fromstation.Enabled = false;
            this.txt_fromstation.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_fromstation.ForeColor = System.Drawing.Color.Blue;
            this.txt_fromstation.Location = new System.Drawing.Point(229, 129);
            this.txt_fromstation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txt_fromstation.Name = "txt_fromstation";
            this.txt_fromstation.Size = new System.Drawing.Size(183, 35);
            this.txt_fromstation.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(127, 133);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 18);
            this.label5.TabIndex = 18;
            this.label5.Text = "起始站台：";
            // 
            // FrmTaskEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 455);
            this.Controls.Add(this.txt_fromstation);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_tostation);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cbb_runstatus);
            this.Controls.Add(this.cbb_taskstatus);
            this.Controls.Add(this.txtBarCode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_taskno);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(550, 486);
            this.MinimizeBox = false;
            this.Name = "FrmTaskEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "任务修改";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.Load += new System.EventHandler(this.FrmTaskEdit_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_taskno;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private C1.Win.C1Input.C1Button btnConfirm;
        private C1.Win.C1Input.C1Button btnCancle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBarCode;
        private System.Windows.Forms.ComboBox cbb_taskstatus;
        private System.Windows.Forms.ComboBox cbb_runstatus;
        private System.Windows.Forms.TextBox txt_tostation;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txt_fromstation;
        private System.Windows.Forms.Label label5;


    }
}