namespace CWCS.UI
{
    partial class frmSystemInfo
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label2 = new System.Windows.Forms.Label();
            this.lblCPU = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTotalMemroy = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblAvaMemory = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblSign = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 45;
            this.label2.Text = "可用CPU：";
            // 
            // lblCPU
            // 
            this.lblCPU.BackColor = System.Drawing.Color.Transparent;
            this.lblCPU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCPU.ForeColor = System.Drawing.Color.Blue;
            this.lblCPU.Location = new System.Drawing.Point(106, 117);
            this.lblCPU.Name = "lblCPU";
            this.lblCPU.Size = new System.Drawing.Size(45, 19);
            this.lblCPU.TabIndex = 46;
            this.lblCPU.Text = "??";
            this.lblCPU.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMemory
            // 
            this.lblMemory.BackColor = System.Drawing.Color.Transparent;
            this.lblMemory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMemory.ForeColor = System.Drawing.Color.Blue;
            this.lblMemory.Location = new System.Drawing.Point(106, 164);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(45, 19);
            this.lblMemory.TabIndex = 48;
            this.lblMemory.Text = "??";
            this.lblMemory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 47;
            this.label4.Text = "可用物理内存：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(154, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 49;
            this.label5.Text = "%";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(154, 167);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 50;
            this.label6.Text = "MB";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(154, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 53;
            this.label3.Text = "MB";
            // 
            // lblCurrent
            // 
            this.lblCurrent.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCurrent.ForeColor = System.Drawing.Color.Blue;
            this.lblCurrent.Location = new System.Drawing.Point(106, 210);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(45, 19);
            this.lblCurrent.TabIndex = 52;
            this.lblCurrent.Text = "??";
            this.lblCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 213);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 12);
            this.label8.TabIndex = 51;
            this.label8.Text = "当前已分配内存：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(53, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 54;
            this.label7.Text = "CPU个数：";
            // 
            // lblCount
            // 
            this.lblCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCount.ForeColor = System.Drawing.Color.Blue;
            this.lblCount.Location = new System.Drawing.Point(106, 94);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(45, 19);
            this.lblCount.TabIndex = 55;
            this.lblCount.Text = "??";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(154, 97);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 12);
            this.label10.TabIndex = 56;
            this.label10.Text = "个";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(47, 144);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 57;
            this.label9.Text = "物理内存：";
            // 
            // lblTotalMemroy
            // 
            this.lblTotalMemroy.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalMemroy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalMemroy.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalMemroy.Location = new System.Drawing.Point(106, 141);
            this.lblTotalMemroy.Name = "lblTotalMemroy";
            this.lblTotalMemroy.Size = new System.Drawing.Size(45, 19);
            this.lblTotalMemroy.TabIndex = 58;
            this.lblTotalMemroy.Text = "??";
            this.lblTotalMemroy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(154, 144);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 12);
            this.label12.TabIndex = 59;
            this.label12.Text = "MB";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(154, 190);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(11, 12);
            this.label11.TabIndex = 62;
            this.label11.Text = "%";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(35, 191);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 12);
            this.label14.TabIndex = 60;
            this.label14.Text = "内存可用率：";
            // 
            // lblAvaMemory
            // 
            this.lblAvaMemory.BackColor = System.Drawing.Color.Transparent;
            this.lblAvaMemory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAvaMemory.ForeColor = System.Drawing.Color.Blue;
            this.lblAvaMemory.Location = new System.Drawing.Point(106, 187);
            this.lblAvaMemory.Name = "lblAvaMemory";
            this.lblAvaMemory.Size = new System.Drawing.Size(45, 19);
            this.lblAvaMemory.TabIndex = 63;
            this.lblAvaMemory.Text = "??";
            this.lblAvaMemory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(61, 246);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(101, 12);
            this.label13.TabIndex = 64;
            this.label13.Text = "系统繁忙程度评价";
            // 
            // lblSign
            // 
            this.lblSign.BackColor = System.Drawing.Color.Transparent;
            this.lblSign.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSign.Font = new System.Drawing.Font("Arial", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSign.ForeColor = System.Drawing.Color.Red;
            this.lblSign.Location = new System.Drawing.Point(13, 261);
            this.lblSign.Name = "lblSign";
            this.lblSign.Size = new System.Drawing.Size(192, 45);
            this.lblSign.TabIndex = 65;
            this.lblSign.Text = "??";
            this.lblSign.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.Red;
            this.label15.Location = new System.Drawing.Point(35, 313);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(149, 12);
            this.label15.TabIndex = 66;
            this.label15.Text = "该值越高，表示系统越繁忙";
            // 
            // frmSystemInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(215, 395);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.lblSign);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblAvaMemory);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lblTotalMemroy);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblCurrent);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblMemory);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblCPU);
            this.Controls.Add(this.label2);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "frmSystemInfo";
            this.Text = "系统性能";
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.lblCPU, 0);
            //this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.lblMemory, 0);
            this.Controls.SetChildIndex(this.label5, 0);
            this.Controls.SetChildIndex(this.label6, 0);
            this.Controls.SetChildIndex(this.label8, 0);
            this.Controls.SetChildIndex(this.lblCurrent, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.lblCount, 0);
            this.Controls.SetChildIndex(this.label10, 0);
            this.Controls.SetChildIndex(this.label9, 0);
            this.Controls.SetChildIndex(this.lblTotalMemroy, 0);
            this.Controls.SetChildIndex(this.label12, 0);
            this.Controls.SetChildIndex(this.label14, 0);
            this.Controls.SetChildIndex(this.label11, 0);
            this.Controls.SetChildIndex(this.lblAvaMemory, 0);
            this.Controls.SetChildIndex(this.label13, 0);
            this.Controls.SetChildIndex(this.lblSign, 0);
            this.Controls.SetChildIndex(this.label15, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblCPU;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTotalMemroy;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblAvaMemory;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblSign;
        private System.Windows.Forms.Label label15;
    }
}