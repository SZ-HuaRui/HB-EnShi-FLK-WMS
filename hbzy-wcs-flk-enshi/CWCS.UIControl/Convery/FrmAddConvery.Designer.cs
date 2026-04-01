namespace CWCS.UIControl.Convery
{
    partial class FrmAddConvery
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
            this.txtEndNum = new C1.Win.C1Input.C1TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFromNum = new C1.Win.C1Input.C1TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancle = new C1.Win.C1Input.C1Button();
            this.btnGeneral = new C1.Win.C1Input.C1Button();
            this.txtDB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.progressBarEx1 = new CWCS.UIDesigner.ProgressBarEx();
            this.tb_areaCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_zoneCode = new System.Windows.Forms.TextBox();
            this.tb_startValue = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromNum)).BeginInit();
            this.SuspendLayout();
            // 
            // txtEndNum
            // 
            this.txtEndNum.Label = this.label3;
            this.txtEndNum.Location = new System.Drawing.Point(169, 64);
            this.txtEndNum.Name = "txtEndNum";
            this.txtEndNum.Size = new System.Drawing.Size(123, 21);
            this.txtEndNum.TabIndex = 30;
            this.txtEndNum.Tag = null;
            this.txtEndNum.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2007Blue;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(98, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 37;
            this.label3.Text = "目标站台：";
            // 
            // txtFromNum
            // 
            this.txtFromNum.Label = this.label2;
            this.txtFromNum.Location = new System.Drawing.Point(169, 27);
            this.txtFromNum.Name = "txtFromNum";
            this.txtFromNum.Size = new System.Drawing.Size(123, 21);
            this.txtFromNum.TabIndex = 29;
            this.txtFromNum.Tag = null;
            this.txtFromNum.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2007Blue;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(98, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 36;
            this.label2.Text = "起始站台：";
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(217, 300);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(91, 23);
            this.btnCancle.TabIndex = 44;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.VisualStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.btnCancle.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // btnGeneral
            // 
            this.btnGeneral.Location = new System.Drawing.Point(101, 300);
            this.btnGeneral.Name = "btnGeneral";
            this.btnGeneral.Size = new System.Drawing.Size(89, 23);
            this.btnGeneral.TabIndex = 43;
            this.btnGeneral.Text = "生成站台信息";
            this.btnGeneral.UseVisualStyleBackColor = true;
            this.btnGeneral.VisualStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.btnGeneral.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.btnGeneral.Click += new System.EventHandler(this.btnGeneral_Click);
            // 
            // txtDB
            // 
            this.txtDB.Location = new System.Drawing.Point(169, 101);
            this.txtDB.Name = "txtDB";
            this.txtDB.Size = new System.Drawing.Size(123, 21);
            this.txtDB.TabIndex = 31;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(74, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 39;
            this.label5.Text = "DB块单位长度：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(122, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 38;
            this.label4.Text = "DB块：";
            // 
            // txtLength
            // 
            this.txtLength.Location = new System.Drawing.Point(169, 140);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(123, 21);
            this.txtLength.TabIndex = 32;
            // 
            // progressBarEx1
            // 
            this.progressBarEx1.BackColor = System.Drawing.Color.LimeGreen;
            this.progressBarEx1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBarEx1.Font = new System.Drawing.Font("Consolas", 10.25F);
            this.progressBarEx1.ForeColor = System.Drawing.Color.DarkOrange;
            this.progressBarEx1.Location = new System.Drawing.Point(0, 340);
            this.progressBarEx1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.progressBarEx1.Name = "progressBarEx1";
            this.progressBarEx1.Size = new System.Drawing.Size(405, 19);
            this.progressBarEx1.TabIndex = 39;
            this.progressBarEx1.TextColor = System.Drawing.Color.Honeydew;
            this.progressBarEx1.Value = 0;
            // 
            // tb_areaCode
            // 
            this.tb_areaCode.Location = new System.Drawing.Point(171, 211);
            this.tb_areaCode.Name = "tb_areaCode";
            this.tb_areaCode.Size = new System.Drawing.Size(123, 21);
            this.tb_areaCode.TabIndex = 34;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(123, 253);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 42;
            this.label1.Text = "库区：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(100, 214);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 41;
            this.label6.Text = "区域编码：";
            // 
            // tb_zoneCode
            // 
            this.tb_zoneCode.Location = new System.Drawing.Point(171, 251);
            this.tb_zoneCode.Name = "tb_zoneCode";
            this.tb_zoneCode.Size = new System.Drawing.Size(123, 21);
            this.tb_zoneCode.TabIndex = 35;
            // 
            // tb_startValue
            // 
            this.tb_startValue.Location = new System.Drawing.Point(170, 174);
            this.tb_startValue.Name = "tb_startValue";
            this.tb_startValue.Size = new System.Drawing.Size(123, 21);
            this.tb_startValue.TabIndex = 33;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(63, 177);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 12);
            this.label7.TabIndex = 40;
            this.label7.Text = "DB块起始偏移量：";
            // 
            // FrmAddConvery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 359);
            this.Controls.Add(this.tb_startValue);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tb_areaCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tb_zoneCode);
            this.Controls.Add(this.progressBarEx1);
            this.Controls.Add(this.txtEndNum);
            this.Controls.Add(this.txtFromNum);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnGeneral);
            this.Controls.Add(this.txtDB);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "FrmAddConvery";
            this.Text = "批量添加";
            this.Load += new System.EventHandler(this.FrmAddConvery_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtEndNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private C1.Win.C1Input.C1TextBox txtEndNum;
        private System.Windows.Forms.Label label3;
        private C1.Win.C1Input.C1TextBox txtFromNum;
        private System.Windows.Forms.Label label2;
        private C1.Win.C1Input.C1Button btnCancle;
        private C1.Win.C1Input.C1Button btnGeneral;
        private System.Windows.Forms.TextBox txtDB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLength;
        private UIDesigner.ProgressBarEx progressBarEx1;
        private System.Windows.Forms.TextBox tb_areaCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_zoneCode;
        private System.Windows.Forms.TextBox tb_startValue;
        private System.Windows.Forms.Label label7;
    }
}