namespace WCS
{
    partial class FormBDManager
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dv_BDView = new System.Windows.Forms.DataGridView();
            this.taskno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReleationMachine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wmstaskno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agvindex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtb_info = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lb_connect = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbl_PZ = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_JZ = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bdsInfo = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dv_BDView)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdsInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(195)))), ((int)(((byte)(235)))));
            this.groupBox1.Controls.Add(this.dv_BDView);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1097, 406);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BD机台列表";
            // 
            // dv_BDView
            // 
            this.dv_BDView.AllowUserToAddRows = false;
            this.dv_BDView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.dv_BDView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dv_BDView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dv_BDView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dv_BDView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(241)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dv_BDView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dv_BDView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dv_BDView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.taskno,
            this.ReleationMachine,
            this.wmstaskno,
            this.agvindex});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dv_BDView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dv_BDView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dv_BDView.Location = new System.Drawing.Point(3, 17);
            this.dv_BDView.MultiSelect = false;
            this.dv_BDView.Name = "dv_BDView";
            this.dv_BDView.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dv_BDView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dv_BDView.RowHeadersWidth = 20;
            this.dv_BDView.RowTemplate.Height = 23;
            this.dv_BDView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dv_BDView.Size = new System.Drawing.Size(1091, 386);
            this.dv_BDView.TabIndex = 41;
            this.dv_BDView.SelectionChanged += new System.EventHandler(this.dv_BDView_SelectionChanged);
            // 
            // taskno
            // 
            this.taskno.DataPropertyName = "taskno";
            this.taskno.HeaderText = "BD门编号";
            this.taskno.MinimumWidth = 70;
            this.taskno.Name = "taskno";
            this.taskno.ReadOnly = true;
            this.taskno.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.taskno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.taskno.Width = 78;
            // 
            // ReleationMachine
            // 
            this.ReleationMachine.HeaderText = "关联机台";
            this.ReleationMachine.Name = "ReleationMachine";
            this.ReleationMachine.ReadOnly = true;
            this.ReleationMachine.Width = 78;
            // 
            // wmstaskno
            // 
            this.wmstaskno.DataPropertyName = "wmstaskno";
            this.wmstaskno.HeaderText = "IP地址";
            this.wmstaskno.MinimumWidth = 100;
            this.wmstaskno.Name = "wmstaskno";
            this.wmstaskno.ReadOnly = true;
            this.wmstaskno.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.wmstaskno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // agvindex
            // 
            this.agvindex.DataPropertyName = "agvindex";
            this.agvindex.HeaderText = "连接状态";
            this.agvindex.Name = "agvindex";
            this.agvindex.ReadOnly = true;
            this.agvindex.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.agvindex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.agvindex.Width = 78;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(195)))), ((int)(((byte)(235)))));
            this.groupBox2.Controls.Add(this.rtb_info);
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 406);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1097, 374);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "信息";
            // 
            // rtb_info
            // 
            this.rtb_info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(241)))), ((int)(((byte)(250)))));
            this.rtb_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_info.Location = new System.Drawing.Point(3, 17);
            this.rtb_info.Name = "rtb_info";
            this.rtb_info.Size = new System.Drawing.Size(763, 354);
            this.rtb_info.TabIndex = 63;
            this.rtb_info.Text = "";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(766, 17);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(328, 354);
            this.panel2.TabIndex = 62;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lb_connect);
            this.groupBox3.Controls.Add(this.panel3);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(326, 352);
            this.groupBox3.TabIndex = 45;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "状态";
            // 
            // lb_connect
            // 
            this.lb_connect.BackColor = System.Drawing.Color.Red;
            this.lb_connect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_connect.Location = new System.Drawing.Point(26, 29);
            this.lb_connect.Name = "lb_connect";
            this.lb_connect.Size = new System.Drawing.Size(119, 29);
            this.lb_connect.TabIndex = 4;
            this.lb_connect.Text = "断线";
            this.lb_connect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbl_PZ);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Location = new System.Drawing.Point(29, 198);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(260, 47);
            this.panel3.TabIndex = 3;
            // 
            // lbl_PZ
            // 
            this.lbl_PZ.AutoSize = true;
            this.lbl_PZ.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_PZ.Location = new System.Drawing.Point(74, 20);
            this.lbl_PZ.Name = "lbl_PZ";
            this.lbl_PZ.Size = new System.Drawing.Size(87, 16);
            this.lbl_PZ.TabIndex = 1;
            this.lbl_PZ.Text = "未收到状态";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(13, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "盘纸门：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbl_JZ);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(29, 111);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 47);
            this.panel1.TabIndex = 2;
            // 
            // lbl_JZ
            // 
            this.lbl_JZ.AutoSize = true;
            this.lbl_JZ.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_JZ.Location = new System.Drawing.Point(74, 20);
            this.lbl_JZ.Name = "lbl_JZ";
            this.lbl_JZ.Size = new System.Drawing.Size(87, 16);
            this.lbl_JZ.TabIndex = 1;
            this.lbl_JZ.Text = "未收到状态";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "卷纸门：";
            // 
            // FormBDManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1097, 780);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormBDManager";
            this.Text = "BD门";
            this.Load += new System.EventHandler(this.FormBDEquip_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dv_BDView)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdsInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.BindingSource bdsInfo;
        private System.Windows.Forms.DataGridView dv_BDView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox rtb_info;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridViewTextBoxColumn taskno;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReleationMachine;
        private System.Windows.Forms.DataGridViewTextBoxColumn wmstaskno;
        private System.Windows.Forms.DataGridViewTextBoxColumn agvindex;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_JZ;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lbl_PZ;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lb_connect;
    }
}