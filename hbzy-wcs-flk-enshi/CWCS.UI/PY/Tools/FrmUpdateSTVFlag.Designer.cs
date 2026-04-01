namespace CWCS.UI
{
    partial class FrmUpdateSTVFlag
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.textBoxTasKNO = new System.Windows.Forms.TextBox();
            this.textBoxBarcode = new System.Windows.Forms.TextBox();
            this.textBoxGroupNO = new System.Windows.Forms.TextBox();
            this.textBoxSequenceNO = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "任务号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "托盘号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(92, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "出库组号：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(81, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "出库顺序号：";
            // 
            // buttonConfirm
            // 
            this.buttonConfirm.Location = new System.Drawing.Point(202, 192);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(75, 23);
            this.buttonConfirm.TabIndex = 4;
            this.buttonConfirm.Text = "确认接货";
            this.buttonConfirm.UseVisualStyleBackColor = true;
            this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
            // 
            // textBoxTasKNO
            // 
            this.textBoxTasKNO.Location = new System.Drawing.Point(177, 38);
            this.textBoxTasKNO.Name = "textBoxTasKNO";
            this.textBoxTasKNO.ReadOnly = true;
            this.textBoxTasKNO.Size = new System.Drawing.Size(100, 21);
            this.textBoxTasKNO.TabIndex = 5;
            // 
            // textBoxBarcode
            // 
            this.textBoxBarcode.Location = new System.Drawing.Point(177, 72);
            this.textBoxBarcode.Name = "textBoxBarcode";
            this.textBoxBarcode.ReadOnly = true;
            this.textBoxBarcode.Size = new System.Drawing.Size(100, 21);
            this.textBoxBarcode.TabIndex = 6;
            // 
            // textBoxGroupNO
            // 
            this.textBoxGroupNO.Location = new System.Drawing.Point(177, 108);
            this.textBoxGroupNO.Name = "textBoxGroupNO";
            this.textBoxGroupNO.ReadOnly = true;
            this.textBoxGroupNO.Size = new System.Drawing.Size(100, 21);
            this.textBoxGroupNO.TabIndex = 7;
            // 
            // textBoxSequenceNO
            // 
            this.textBoxSequenceNO.Location = new System.Drawing.Point(177, 143);
            this.textBoxSequenceNO.Name = "textBoxSequenceNO";
            this.textBoxSequenceNO.ReadOnly = true;
            this.textBoxSequenceNO.Size = new System.Drawing.Size(100, 21);
            this.textBoxSequenceNO.TabIndex = 8;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(83, 192);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FrmUpdateSTVFlag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 259);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxSequenceNO);
            this.Controls.Add(this.textBoxGroupNO);
            this.Controls.Add(this.textBoxBarcode);
            this.Controls.Add(this.textBoxTasKNO);
            this.Controls.Add(this.buttonConfirm);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FrmUpdateSTVFlag";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "确认接货";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonConfirm;
        private System.Windows.Forms.TextBox textBoxTasKNO;
        private System.Windows.Forms.TextBox textBoxBarcode;
        private System.Windows.Forms.TextBox textBoxGroupNO;
        private System.Windows.Forms.TextBox textBoxSequenceNO;
        private System.Windows.Forms.Button buttonCancel;
    }
}