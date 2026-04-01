namespace CWCS.UI
{
    partial class FrmOtherSetting
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSmallSet = new System.Windows.Forms.Button();
            this.btnSmallReflash = new System.Windows.Forms.Button();
            this.textBoxSmallLeft = new System.Windows.Forms.TextBox();
            this.textBoxSmallTotal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.btnReflash = new System.Windows.Forms.Button();
            this.textBoxLeft = new System.Windows.Forms.TextBox();
            this.textBoxTotal = new System.Windows.Forms.TextBox();
            this.labelLeft = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(291, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 450);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(291, 450);
            this.panel2.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSmallSet);
            this.groupBox2.Controls.Add(this.btnSmallReflash);
            this.groupBox2.Controls.Add(this.textBoxSmallLeft);
            this.groupBox2.Controls.Add(this.textBoxSmallTotal);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 238);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(262, 171);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "薄片线缓存站台设置";
            // 
            // btnSmallSet
            // 
            this.btnSmallSet.Location = new System.Drawing.Point(141, 115);
            this.btnSmallSet.Name = "btnSmallSet";
            this.btnSmallSet.Size = new System.Drawing.Size(75, 23);
            this.btnSmallSet.TabIndex = 5;
            this.btnSmallSet.Text = "设置";
            this.btnSmallSet.UseVisualStyleBackColor = true;
            // 
            // btnSmallReflash
            // 
            this.btnSmallReflash.Location = new System.Drawing.Point(45, 115);
            this.btnSmallReflash.Name = "btnSmallReflash";
            this.btnSmallReflash.Size = new System.Drawing.Size(75, 23);
            this.btnSmallReflash.TabIndex = 4;
            this.btnSmallReflash.Text = "刷新";
            this.btnSmallReflash.UseVisualStyleBackColor = true;
            // 
            // textBoxSmallLeft
            // 
            this.textBoxSmallLeft.Location = new System.Drawing.Point(116, 67);
            this.textBoxSmallLeft.Name = "textBoxSmallLeft";
            this.textBoxSmallLeft.Size = new System.Drawing.Size(100, 21);
            this.textBoxSmallLeft.TabIndex = 3;
            // 
            // textBoxSmallTotal
            // 
            this.textBoxSmallTotal.Location = new System.Drawing.Point(116, 36);
            this.textBoxSmallTotal.Name = "textBoxSmallTotal";
            this.textBoxSmallTotal.Size = new System.Drawing.Size(100, 21);
            this.textBoxSmallTotal.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "剩余缓存：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "缓存总量初始值：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSet);
            this.groupBox1.Controls.Add(this.btnReflash);
            this.groupBox1.Controls.Add(this.textBoxLeft);
            this.groupBox1.Controls.Add(this.textBoxTotal);
            this.groupBox1.Controls.Add(this.labelLeft);
            this.groupBox1.Controls.Add(this.labelTotal);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(262, 171);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "片烟线缓存站台设置";
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(148, 117);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(75, 23);
            this.btnSet.TabIndex = 5;
            this.btnSet.Text = "设置";
            this.btnSet.UseVisualStyleBackColor = true;
            // 
            // btnReflash
            // 
            this.btnReflash.Location = new System.Drawing.Point(46, 117);
            this.btnReflash.Name = "btnReflash";
            this.btnReflash.Size = new System.Drawing.Size(75, 23);
            this.btnReflash.TabIndex = 4;
            this.btnReflash.Text = "刷新";
            this.btnReflash.UseVisualStyleBackColor = true;
            // 
            // textBoxLeft
            // 
            this.textBoxLeft.Location = new System.Drawing.Point(116, 69);
            this.textBoxLeft.Name = "textBoxLeft";
            this.textBoxLeft.Size = new System.Drawing.Size(107, 21);
            this.textBoxLeft.TabIndex = 3;
            // 
            // textBoxTotal
            // 
            this.textBoxTotal.Location = new System.Drawing.Point(116, 41);
            this.textBoxTotal.Name = "textBoxTotal";
            this.textBoxTotal.Size = new System.Drawing.Size(107, 21);
            this.textBoxTotal.TabIndex = 2;
            // 
            // labelLeft
            // 
            this.labelLeft.AutoSize = true;
            this.labelLeft.Location = new System.Drawing.Point(44, 73);
            this.labelLeft.Name = "labelLeft";
            this.labelLeft.Size = new System.Drawing.Size(65, 12);
            this.labelLeft.TabIndex = 1;
            this.labelLeft.Text = "剩余缓存：";
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(8, 41);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(101, 12);
            this.labelTotal.TabIndex = 0;
            this.labelTotal.Text = "缓存总量初始值：";
            // 
            // FrmOtherSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "FrmOtherSetting";
            this.Text = "其他设置";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Label labelLeft;
        private System.Windows.Forms.TextBox textBoxLeft;
        private System.Windows.Forms.TextBox textBoxTotal;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Button btnReflash;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSmallSet;
        private System.Windows.Forms.Button btnSmallReflash;
        private System.Windows.Forms.TextBox textBoxSmallLeft;
        private System.Windows.Forms.TextBox textBoxSmallTotal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}