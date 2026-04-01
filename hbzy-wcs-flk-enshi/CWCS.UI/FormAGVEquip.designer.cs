namespace WCS
{
    partial class FormAGVEquip
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
            this.gv_taskmanager = new System.Windows.Forms.DataGridView();
            this.taskno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wmstaskno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agvindex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agvcarno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agvikey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AGVMagic = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fromstation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tostation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tasktype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.taskstatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.runstatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.equipmentnumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.barcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createtime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updatetime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtb_info = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSendLayer = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAGVIndex = new System.Windows.Forms.TextBox();
            this.txtLayer = new System.Windows.Forms.TextBox();
            this.txtTargetAddress = new System.Windows.Forms.TextBox();
            this.txtSourceAddress = new System.Windows.Forms.TextBox();
            this.sendtask = new System.Windows.Forms.Button();
            this.txt_end = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_start = new System.Windows.Forms.TextBox();
            this.start = new System.Windows.Forms.Label();
            this.btn_auto = new System.Windows.Forms.Button();
            this.txt_AGVIndex = new System.Windows.Forms.TextBox();
            this.lb_connect = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bdsInfo = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gv_taskmanager)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdsInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(195)))), ((int)(((byte)(235)))));
            this.groupBox1.Controls.Add(this.gv_taskmanager);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1097, 406);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "任务";
            // 
            // gv_taskmanager
            // 
            this.gv_taskmanager.AllowUserToAddRows = false;
            this.gv_taskmanager.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.gv_taskmanager.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gv_taskmanager.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gv_taskmanager.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gv_taskmanager.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(241)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gv_taskmanager.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gv_taskmanager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gv_taskmanager.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.taskno,
            this.wmstaskno,
            this.agvindex,
            this.agvcarno,
            this.agvikey,
            this.AGVMagic,
            this.fromstation,
            this.tostation,
            this.tasktype,
            this.taskstatus,
            this.runstatus,
            this.equipmentnumber,
            this.barcode,
            this.createtime,
            this.updatetime});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gv_taskmanager.DefaultCellStyle = dataGridViewCellStyle3;
            this.gv_taskmanager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gv_taskmanager.Location = new System.Drawing.Point(3, 17);
            this.gv_taskmanager.MultiSelect = false;
            this.gv_taskmanager.Name = "gv_taskmanager";
            this.gv_taskmanager.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gv_taskmanager.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gv_taskmanager.RowHeadersWidth = 20;
            this.gv_taskmanager.RowTemplate.Height = 23;
            this.gv_taskmanager.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gv_taskmanager.Size = new System.Drawing.Size(1091, 386);
            this.gv_taskmanager.TabIndex = 41;
            // 
            // taskno
            // 
            this.taskno.DataPropertyName = "taskno";
            this.taskno.HeaderText = "任务编号";
            this.taskno.MinimumWidth = 70;
            this.taskno.Name = "taskno";
            this.taskno.ReadOnly = true;
            this.taskno.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.taskno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.taskno.Width = 78;
            // 
            // wmstaskno
            // 
            this.wmstaskno.DataPropertyName = "wmstaskno";
            this.wmstaskno.HeaderText = "WMS任务号";
            this.wmstaskno.MinimumWidth = 100;
            this.wmstaskno.Name = "wmstaskno";
            this.wmstaskno.ReadOnly = true;
            this.wmstaskno.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.wmstaskno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // agvindex
            // 
            this.agvindex.DataPropertyName = "agvindex";
            this.agvindex.HeaderText = "AGV索引号";
            this.agvindex.Name = "agvindex";
            this.agvindex.ReadOnly = true;
            this.agvindex.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.agvindex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.agvindex.Width = 84;
            // 
            // agvcarno
            // 
            this.agvcarno.DataPropertyName = "agvcarno";
            this.agvcarno.HeaderText = "AGV车号";
            this.agvcarno.Name = "agvcarno";
            this.agvcarno.ReadOnly = true;
            this.agvcarno.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.agvcarno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.agvcarno.Width = 72;
            // 
            // agvikey
            // 
            this.agvikey.DataPropertyName = "agvikey";
            this.agvikey.HeaderText = "AGVKey";
            this.agvikey.Name = "agvikey";
            this.agvikey.ReadOnly = true;
            this.agvikey.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.agvikey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.agvikey.Width = 66;
            // 
            // AGVMagic
            // 
            this.AGVMagic.DataPropertyName = "agvmagic";
            this.AGVMagic.HeaderText = "AGVMagic";
            this.AGVMagic.Name = "AGVMagic";
            this.AGVMagic.ReadOnly = true;
            this.AGVMagic.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.AGVMagic.Width = 78;
            // 
            // fromstation
            // 
            this.fromstation.DataPropertyName = "fromstation";
            this.fromstation.HeaderText = "起始地址";
            this.fromstation.Name = "fromstation";
            this.fromstation.ReadOnly = true;
            this.fromstation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.fromstation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.fromstation.Width = 78;
            // 
            // tostation
            // 
            this.tostation.DataPropertyName = "tostation";
            this.tostation.HeaderText = "目标地址";
            this.tostation.Name = "tostation";
            this.tostation.ReadOnly = true;
            this.tostation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.tostation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.tostation.Width = 78;
            // 
            // tasktype
            // 
            this.tasktype.DataPropertyName = "tasktype";
            this.tasktype.HeaderText = "任务类型";
            this.tasktype.Name = "tasktype";
            this.tasktype.ReadOnly = true;
            this.tasktype.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.tasktype.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.tasktype.Width = 78;
            // 
            // taskstatus
            // 
            this.taskstatus.DataPropertyName = "taskstatus";
            this.taskstatus.HeaderText = "任务状态";
            this.taskstatus.Name = "taskstatus";
            this.taskstatus.ReadOnly = true;
            this.taskstatus.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.taskstatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.taskstatus.Width = 78;
            // 
            // runstatus
            // 
            this.runstatus.DataPropertyName = "runstatus";
            this.runstatus.HeaderText = "执行状态";
            this.runstatus.Name = "runstatus";
            this.runstatus.ReadOnly = true;
            this.runstatus.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.runstatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.runstatus.Width = 78;
            // 
            // equipmentnumber
            // 
            this.equipmentnumber.DataPropertyName = "equipmentnumber";
            this.equipmentnumber.HeaderText = "设备号";
            this.equipmentnumber.Name = "equipmentnumber";
            this.equipmentnumber.ReadOnly = true;
            this.equipmentnumber.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.equipmentnumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.equipmentnumber.Width = 66;
            // 
            // barcode
            // 
            this.barcode.DataPropertyName = "barcode";
            this.barcode.HeaderText = "托盘号";
            this.barcode.Name = "barcode";
            this.barcode.ReadOnly = true;
            this.barcode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.barcode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.barcode.Width = 66;
            // 
            // createtime
            // 
            this.createtime.DataPropertyName = "CREATE_DATE";
            this.createtime.HeaderText = "创建时间";
            this.createtime.Name = "createtime";
            this.createtime.ReadOnly = true;
            this.createtime.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.createtime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.createtime.Width = 78;
            // 
            // updatetime
            // 
            this.updatetime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.updatetime.DataPropertyName = "MODIFY_DATE";
            this.updatetime.HeaderText = "更新时间";
            this.updatetime.MinimumWidth = 80;
            this.updatetime.Name = "updatetime";
            this.updatetime.ReadOnly = true;
            this.updatetime.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.updatetime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
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
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtSendLayer);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.txtAGVIndex);
            this.groupBox3.Controls.Add(this.txtLayer);
            this.groupBox3.Controls.Add(this.txtTargetAddress);
            this.groupBox3.Controls.Add(this.txtSourceAddress);
            this.groupBox3.Controls.Add(this.sendtask);
            this.groupBox3.Controls.Add(this.txt_end);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.txt_start);
            this.groupBox3.Controls.Add(this.start);
            this.groupBox3.Controls.Add(this.btn_auto);
            this.groupBox3.Controls.Add(this.txt_AGVIndex);
            this.groupBox3.Controls.Add(this.lb_connect);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(326, 238);
            this.groupBox3.TabIndex = 45;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "状态";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Lime;
            this.label8.Location = new System.Drawing.Point(177, 65);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 63;
            this.label8.Text = "层指令：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Lime;
            this.label7.Location = new System.Drawing.Point(25, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 62;
            this.label7.Text = "列指令：";
            // 
            // txtSendLayer
            // 
            this.txtSendLayer.Location = new System.Drawing.Point(236, 209);
            this.txtSendLayer.Name = "txtSendLayer";
            this.txtSendLayer.Size = new System.Drawing.Size(68, 22);
            this.txtSendLayer.TabIndex = 61;
            this.txtSendLayer.Text = "发送";
            this.txtSendLayer.UseVisualStyleBackColor = true;
            this.txtSendLayer.Click += new System.EventHandler(this.txtSendLayer_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(159, 178);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 60;
            this.label6.Text = "AGV索引号：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(189, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 59;
            this.label5.Text = "层数：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(165, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 58;
            this.label4.Text = "目标地址：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(165, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 57;
            this.label1.Text = "起始地址：";
            // 
            // txtAGVIndex
            // 
            this.txtAGVIndex.Location = new System.Drawing.Point(236, 173);
            this.txtAGVIndex.Name = "txtAGVIndex";
            this.txtAGVIndex.Size = new System.Drawing.Size(68, 21);
            this.txtAGVIndex.TabIndex = 56;
            // 
            // txtLayer
            // 
            this.txtLayer.Location = new System.Drawing.Point(236, 146);
            this.txtLayer.Name = "txtLayer";
            this.txtLayer.Size = new System.Drawing.Size(68, 21);
            this.txtLayer.TabIndex = 55;
            // 
            // txtTargetAddress
            // 
            this.txtTargetAddress.Location = new System.Drawing.Point(236, 118);
            this.txtTargetAddress.Name = "txtTargetAddress";
            this.txtTargetAddress.Size = new System.Drawing.Size(68, 21);
            this.txtTargetAddress.TabIndex = 54;
            // 
            // txtSourceAddress
            // 
            this.txtSourceAddress.Location = new System.Drawing.Point(236, 87);
            this.txtSourceAddress.Name = "txtSourceAddress";
            this.txtSourceAddress.Size = new System.Drawing.Size(68, 21);
            this.txtSourceAddress.TabIndex = 53;
            // 
            // sendtask
            // 
            this.sendtask.Location = new System.Drawing.Point(78, 187);
            this.sendtask.Name = "sendtask";
            this.sendtask.Size = new System.Drawing.Size(68, 22);
            this.sendtask.TabIndex = 50;
            this.sendtask.Text = "发送";
            this.sendtask.UseVisualStyleBackColor = true;
            this.sendtask.Click += new System.EventHandler(this.sendtask_Click);
            // 
            // txt_end
            // 
            this.txt_end.Location = new System.Drawing.Point(78, 120);
            this.txt_end.Name = "txt_end";
            this.txt_end.Size = new System.Drawing.Size(68, 21);
            this.txt_end.TabIndex = 48;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 49;
            this.label2.Text = "目标地址：";
            // 
            // txt_start
            // 
            this.txt_start.Location = new System.Drawing.Point(78, 89);
            this.txt_start.Name = "txt_start";
            this.txt_start.Size = new System.Drawing.Size(68, 21);
            this.txt_start.TabIndex = 46;
            // 
            // start
            // 
            this.start.AutoSize = true;
            this.start.Location = new System.Drawing.Point(13, 93);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(65, 12);
            this.start.TabIndex = 47;
            this.start.Text = "起始地址：";
            // 
            // btn_auto
            // 
            this.btn_auto.Location = new System.Drawing.Point(176, 16);
            this.btn_auto.Name = "btn_auto";
            this.btn_auto.Size = new System.Drawing.Size(119, 30);
            this.btn_auto.TabIndex = 45;
            this.btn_auto.Text = "自动(&A)";
            this.btn_auto.UseVisualStyleBackColor = true;
            this.btn_auto.Click += new System.EventHandler(this.btn_auto_Click);
            // 
            // txt_AGVIndex
            // 
            this.txt_AGVIndex.Location = new System.Drawing.Point(78, 151);
            this.txt_AGVIndex.Name = "txt_AGVIndex";
            this.txt_AGVIndex.Size = new System.Drawing.Size(68, 21);
            this.txt_AGVIndex.TabIndex = 41;
            // 
            // lb_connect
            // 
            this.lb_connect.BackColor = System.Drawing.Color.Lime;
            this.lb_connect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_connect.Location = new System.Drawing.Point(27, 17);
            this.lb_connect.Name = "lb_connect";
            this.lb_connect.Size = new System.Drawing.Size(119, 29);
            this.lb_connect.TabIndex = 0;
            this.lb_connect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 42;
            this.label3.Text = "小车任务号：";
            // 
            // FormAGVEquip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1097, 780);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormAGVEquip";
            this.Text = "AGV小车";
            this.Load += new System.EventHandler(this.FormAGVEquip_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gv_taskmanager)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdsInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.BindingSource bdsInfo;
        private System.Windows.Forms.DataGridView gv_taskmanager;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox rtb_info;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lb_connect;
        private System.Windows.Forms.TextBox txt_AGVIndex;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_auto;
        private System.Windows.Forms.Button sendtask;
        private System.Windows.Forms.TextBox txt_end;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_start;
        private System.Windows.Forms.Label start;
        private System.Windows.Forms.TextBox txtAGVIndex;
        private System.Windows.Forms.TextBox txtLayer;
        private System.Windows.Forms.TextBox txtTargetAddress;
        private System.Windows.Forms.TextBox txtSourceAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button txtSendLayer;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridViewTextBoxColumn taskno;
        private System.Windows.Forms.DataGridViewTextBoxColumn wmstaskno;
        private System.Windows.Forms.DataGridViewTextBoxColumn agvindex;
        private System.Windows.Forms.DataGridViewTextBoxColumn agvcarno;
        private System.Windows.Forms.DataGridViewTextBoxColumn agvikey;
        private System.Windows.Forms.DataGridViewTextBoxColumn AGVMagic;
        private System.Windows.Forms.DataGridViewTextBoxColumn fromstation;
        private System.Windows.Forms.DataGridViewTextBoxColumn tostation;
        private System.Windows.Forms.DataGridViewTextBoxColumn tasktype;
        private System.Windows.Forms.DataGridViewTextBoxColumn taskstatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn runstatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn equipmentnumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn barcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn createtime;
        private System.Windows.Forms.DataGridViewTextBoxColumn updatetime;
    }
}