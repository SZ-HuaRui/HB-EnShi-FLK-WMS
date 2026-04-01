namespace CWCS.CommonControl.UIControl
{
    partial class PaginationControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPageIndex = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblFirst = new System.Windows.Forms.Label();
            this.lblPrevious = new System.Windows.Forms.Label();
            this.lblNext = new System.Windows.Forms.Label();
            this.lblLast = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSkip = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbPageSize = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblGO = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblTotalPageCount = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(135, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "共";
            // 
            // lblTotalCount
            // 
            this.lblTotalCount.AutoSize = true;
            this.lblTotalCount.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTotalCount.ForeColor = System.Drawing.Color.Red;
            this.lblTotalCount.Location = new System.Drawing.Point(165, 10);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(15, 17);
            this.lblTotalCount.TabIndex = 1;
            this.lblTotalCount.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(193, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "条记录，";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(324, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "当前第";
            // 
            // lblPageIndex
            // 
            this.lblPageIndex.AutoSize = true;
            this.lblPageIndex.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPageIndex.ForeColor = System.Drawing.Color.Red;
            this.lblPageIndex.Location = new System.Drawing.Point(377, 10);
            this.lblPageIndex.Name = "lblPageIndex";
            this.lblPageIndex.Size = new System.Drawing.Size(15, 17);
            this.lblPageIndex.TabIndex = 4;
            this.lblPageIndex.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(403, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 17);
            this.label4.TabIndex = 5;
            this.label4.Text = "页。";
            // 
            // lblFirst
            // 
            this.lblFirst.AutoSize = true;
            this.lblFirst.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFirst.Location = new System.Drawing.Point(425, 10);
            this.lblFirst.Name = "lblFirst";
            this.lblFirst.Size = new System.Drawing.Size(32, 17);
            this.lblFirst.TabIndex = 6;
            this.lblFirst.Text = "首页";
            this.lblFirst.Click += new System.EventHandler(this.lblFirst_Click);
            this.lblFirst.MouseEnter += new System.EventHandler(this.lbl_CursorEvent);
            // 
            // lblPrevious
            // 
            this.lblPrevious.AutoSize = true;
            this.lblPrevious.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPrevious.Location = new System.Drawing.Point(460, 10);
            this.lblPrevious.Name = "lblPrevious";
            this.lblPrevious.Size = new System.Drawing.Size(44, 17);
            this.lblPrevious.TabIndex = 7;
            this.lblPrevious.Text = "上一页";
            this.lblPrevious.Click += new System.EventHandler(this.lblPrevious_Click);
            this.lblPrevious.MouseEnter += new System.EventHandler(this.lbl_CursorEvent);
            // 
            // lblNext
            // 
            this.lblNext.AutoSize = true;
            this.lblNext.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblNext.Location = new System.Drawing.Point(507, 10);
            this.lblNext.Name = "lblNext";
            this.lblNext.Size = new System.Drawing.Size(44, 17);
            this.lblNext.TabIndex = 8;
            this.lblNext.Text = "下一页";
            this.lblNext.Click += new System.EventHandler(this.lblNext_Click);
            this.lblNext.MouseEnter += new System.EventHandler(this.lbl_CursorEvent);
            // 
            // lblLast
            // 
            this.lblLast.AutoSize = true;
            this.lblLast.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLast.Location = new System.Drawing.Point(554, 10);
            this.lblLast.Name = "lblLast";
            this.lblLast.Size = new System.Drawing.Size(32, 17);
            this.lblLast.TabIndex = 9;
            this.lblLast.Text = "尾页";
            this.lblLast.Click += new System.EventHandler(this.lblLast_Click);
            this.lblLast.MouseEnter += new System.EventHandler(this.lbl_CursorEvent);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(589, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "跳转到：";
            // 
            // txtSkip
            // 
            this.txtSkip.Location = new System.Drawing.Point(648, 8);
            this.txtSkip.Name = "txtSkip";
            this.txtSkip.Size = new System.Drawing.Size(61, 21);
            this.txtSkip.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(712, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "页";
            // 
            // cmbPageSize
            // 
            this.cmbPageSize.FormattingEnabled = true;
            this.cmbPageSize.Location = new System.Drawing.Point(70, 8);
            this.cmbPageSize.Name = "cmbPageSize";
            this.cmbPageSize.Size = new System.Drawing.Size(56, 20);
            this.cmbPageSize.TabIndex = 13;
            this.cmbPageSize.SelectedIndexChanged += new System.EventHandler(this.cmbPageSize_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(1, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "页面大小：";
            // 
            // lblGO
            // 
            this.lblGO.AutoSize = true;
            this.lblGO.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblGO.Location = new System.Drawing.Point(736, 10);
            this.lblGO.Name = "lblGO";
            this.lblGO.Size = new System.Drawing.Size(32, 17);
            this.lblGO.TabIndex = 15;
            this.lblGO.Text = "前往";
            this.lblGO.Click += new System.EventHandler(this.lblGO_Click);
            this.lblGO.MouseEnter += new System.EventHandler(this.lbl_CursorEvent);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(241, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 17);
            this.label8.TabIndex = 16;
            this.label8.Text = "共";
            // 
            // lblTotalPageCount
            // 
            this.lblTotalPageCount.AutoSize = true;
            this.lblTotalPageCount.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTotalPageCount.ForeColor = System.Drawing.Color.Red;
            this.lblTotalPageCount.Location = new System.Drawing.Point(270, 10);
            this.lblTotalPageCount.Name = "lblTotalPageCount";
            this.lblTotalPageCount.Size = new System.Drawing.Size(15, 17);
            this.lblTotalPageCount.TabIndex = 17;
            this.lblTotalPageCount.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(295, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 17);
            this.label10.TabIndex = 18;
            this.label10.Text = "页，";
            // 
            // PaginationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblTotalPageCount);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblGO);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmbPageSize);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSkip);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblLast);
            this.Controls.Add(this.lblNext);
            this.Controls.Add(this.lblPrevious);
            this.Controls.Add(this.lblFirst);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblPageIndex);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblTotalCount);
            this.Controls.Add(this.label1);
            this.Name = "PaginationControl";
            this.Size = new System.Drawing.Size(918, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTotalCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblPageIndex;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblFirst;
        private System.Windows.Forms.Label lblPrevious;
        private System.Windows.Forms.Label lblNext;
        private System.Windows.Forms.Label lblLast;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSkip;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbPageSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblGO;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblTotalPageCount;
        private System.Windows.Forms.Label label10;
    }
}
