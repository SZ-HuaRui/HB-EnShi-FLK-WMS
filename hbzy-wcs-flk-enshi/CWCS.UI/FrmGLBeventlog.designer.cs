namespace CWCS.UI
{
    partial class FrmGLBeventlog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmGLBeventlog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rb_error = new System.Windows.Forms.RadioButton();
            this.rb_normal = new System.Windows.Forms.RadioButton();
            this.txt_barcode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_taskno = new System.Windows.Forms.TextBox();
            this.lb_taskno = new System.Windows.Forms.Label();
            this.txt_key = new System.Windows.Forms.TextBox();
            this.btn_search = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.dtp_updateend = new System.Windows.Forms.DateTimePicker();
            this.dtp_updatestart = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.bdnInfo = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.txtCurrentPage = new System.Windows.Forms.ToolStripTextBox();
            this.lblPageCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.c1DockingTab1 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
            this.c1DockingTab2 = new C1.Win.C1Command.C1DockingTab();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gv_logmanager = new C1.Win.C1FlexGrid.C1FlexGrid();
            this.rtb_msg = new System.Windows.Forms.RichTextBox();
            this.bdsInfo = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdnInfo)).BeginInit();
            this.bdnInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).BeginInit();
            this.c1DockingTab1.SuspendLayout();
            this.c1DockingTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gv_logmanager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(195)))), ((int)(((byte)(235)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.txt_barcode);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txt_taskno);
            this.panel1.Controls.Add(this.lb_taskno);
            this.panel1.Controls.Add(this.txt_key);
            this.panel1.Controls.Add(this.btn_search);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.dtp_updateend);
            this.panel1.Controls.Add(this.dtp_updatestart);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1326, 87);
            this.panel1.TabIndex = 42;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rb_error);
            this.groupBox3.Controls.Add(this.rb_normal);
            this.groupBox3.Location = new System.Drawing.Point(1034, -2);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(126, 81);
            this.groupBox3.TabIndex = 62;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "日志类型";
            // 
            // rb_error
            // 
            this.rb_error.AutoSize = true;
            this.rb_error.Location = new System.Drawing.Point(9, 52);
            this.rb_error.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rb_error.Name = "rb_error";
            this.rb_error.Size = new System.Drawing.Size(105, 22);
            this.rb_error.TabIndex = 0;
            this.rb_error.Text = "异常日志";
            this.rb_error.UseVisualStyleBackColor = true;
            // 
            // rb_normal
            // 
            this.rb_normal.AutoSize = true;
            this.rb_normal.Checked = true;
            this.rb_normal.Location = new System.Drawing.Point(9, 27);
            this.rb_normal.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rb_normal.Name = "rb_normal";
            this.rb_normal.Size = new System.Drawing.Size(105, 22);
            this.rb_normal.TabIndex = 0;
            this.rb_normal.TabStop = true;
            this.rb_normal.Text = "所有日志";
            this.rb_normal.UseVisualStyleBackColor = true;
            // 
            // txt_barcode
            // 
            this.txt_barcode.ForeColor = System.Drawing.Color.Blue;
            this.txt_barcode.Location = new System.Drawing.Point(768, 50);
            this.txt_barcode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_barcode.Name = "txt_barcode";
            this.txt_barcode.Size = new System.Drawing.Size(240, 28);
            this.txt_barcode.TabIndex = 57;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(664, 52);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 18);
            this.label4.TabIndex = 58;
            this.label4.Text = "托盘条码:";
            // 
            // txt_taskno
            // 
            this.txt_taskno.ForeColor = System.Drawing.Color.Blue;
            this.txt_taskno.Location = new System.Drawing.Point(766, 10);
            this.txt_taskno.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_taskno.Name = "txt_taskno";
            this.txt_taskno.Size = new System.Drawing.Size(240, 28);
            this.txt_taskno.TabIndex = 54;
            // 
            // lb_taskno
            // 
            this.lb_taskno.AutoSize = true;
            this.lb_taskno.Location = new System.Drawing.Point(657, 20);
            this.lb_taskno.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_taskno.Name = "lb_taskno";
            this.lb_taskno.Size = new System.Drawing.Size(98, 18);
            this.lb_taskno.TabIndex = 55;
            this.lb_taskno.Text = "WCS任务号:";
            // 
            // txt_key
            // 
            this.txt_key.Location = new System.Drawing.Point(102, 48);
            this.txt_key.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_key.Name = "txt_key";
            this.txt_key.Size = new System.Drawing.Size(529, 28);
            this.txt_key.TabIndex = 2;
            // 
            // btn_search
            // 
            this.btn_search.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_search.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_search.ForeColor = System.Drawing.Color.Black;
            this.btn_search.Location = new System.Drawing.Point(1178, 16);
            this.btn_search.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(116, 52);
            this.btn_search.TabIndex = 3;
            this.btn_search.Text = "查找";
            this.btn_search.UseVisualStyleBackColor = true;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 54);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 18);
            this.label1.TabIndex = 24;
            this.label1.Text = "关键字:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 14);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 18);
            this.label9.TabIndex = 21;
            this.label9.Text = "时间范围:";
            // 
            // dtp_updateend
            // 
            this.dtp_updateend.CalendarForeColor = System.Drawing.Color.Blue;
            this.dtp_updateend.CalendarTitleBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dtp_updateend.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.dtp_updateend.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp_updateend.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_updateend.Location = new System.Drawing.Point(390, 10);
            this.dtp_updateend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtp_updateend.Name = "dtp_updateend";
            this.dtp_updateend.Size = new System.Drawing.Size(241, 28);
            this.dtp_updateend.TabIndex = 1;
            this.dtp_updateend.Value = new System.DateTime(2049, 1, 1, 0, 0, 0, 0);
            this.dtp_updateend.ValueChanged += new System.EventHandler(this.dtp_updateend_ValueChanged);
            // 
            // dtp_updatestart
            // 
            this.dtp_updatestart.CalendarForeColor = System.Drawing.Color.Blue;
            this.dtp_updatestart.CalendarTitleBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dtp_updatestart.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.dtp_updatestart.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp_updatestart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_updatestart.Location = new System.Drawing.Point(102, 9);
            this.dtp_updatestart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtp_updatestart.Name = "dtp_updatestart";
            this.dtp_updatestart.Size = new System.Drawing.Size(242, 28);
            this.dtp_updatestart.TabIndex = 0;
            this.dtp_updatestart.Value = new System.DateTime(2009, 6, 3, 0, 0, 0, 0);
            this.dtp_updatestart.ValueChanged += new System.EventHandler(this.dtp_updatestart_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(356, 16);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 18);
            this.label8.TabIndex = 22;
            this.label8.Text = "至";
            // 
            // bdnInfo
            // 
            this.bdnInfo.AddNewItem = null;
            this.bdnInfo.CountItem = this.bindingNavigatorCountItem;
            this.bdnInfo.DeleteItem = null;
            this.bdnInfo.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.bdnInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.toolStripButton4,
            this.toolStripSeparator4,
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.txtCurrentPage,
            this.lblPageCount,
            this.toolStripSeparator2,
            this.toolStripButton5,
            this.toolStripSeparator5,
            this.toolStripButton2,
            this.toolStripSeparator3,
            this.toolStripButton3,
            this.toolStripSeparator6});
            this.bdnInfo.Location = new System.Drawing.Point(0, 87);
            this.bdnInfo.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bdnInfo.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bdnInfo.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bdnInfo.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bdnInfo.Name = "bdnInfo";
            this.bdnInfo.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.bdnInfo.PositionItem = this.bindingNavigatorPositionItem;
            this.bdnInfo.Size = new System.Drawing.Size(1326, 38);
            this.bdnInfo.TabIndex = 43;
            this.bdnInfo.Text = "bindingNavigator1";
            this.bdnInfo.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.bdnInfo_ItemClicked_1);
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(46, 33);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "总项数";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveFirstItem.Text = "移到第一条记录";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMovePreviousItem.Text = "移到上一条记录";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 38);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "位置";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(73, 30);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "当前位置";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveNextItem.Text = "移到下一条记录";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveLastItem.Text = "移到最后一条记录";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.RightToLeftAutoMirrorImage = true;
            this.toolStripButton4.Size = new System.Drawing.Size(50, 33);
            this.toolStripButton4.Text = "首页";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.RightToLeftAutoMirrorImage = true;
            this.toolStripButton1.Size = new System.Drawing.Size(68, 33);
            this.toolStripButton1.Text = "上一页";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // txtCurrentPage
            // 
            this.txtCurrentPage.AccessibleName = "位置";
            this.txtCurrentPage.AutoSize = false;
            this.txtCurrentPage.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.txtCurrentPage.Name = "txtCurrentPage";
            this.txtCurrentPage.Size = new System.Drawing.Size(73, 30);
            this.txtCurrentPage.Text = "0";
            this.txtCurrentPage.ToolTipText = "当前页";
            // 
            // lblPageCount
            // 
            this.lblPageCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblPageCount.Name = "lblPageCount";
            this.lblPageCount.Size = new System.Drawing.Size(46, 33);
            this.lblPageCount.Text = "/ {0}";
            this.lblPageCount.ToolTipText = "总项数";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.RightToLeftAutoMirrorImage = true;
            this.toolStripButton5.Size = new System.Drawing.Size(68, 33);
            this.toolStripButton5.Text = "下一页";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.RightToLeftAutoMirrorImage = true;
            this.toolStripButton2.Size = new System.Drawing.Size(50, 33);
            this.toolStripButton2.Text = "末页";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.RightToLeftAutoMirrorImage = true;
            this.toolStripButton3.Size = new System.Drawing.Size(50, 33);
            this.toolStripButton3.Text = "刷新";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 38);
            // 
            // c1DockingTab1
            // 
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage1);
            this.c1DockingTab1.Dock = System.Windows.Forms.DockStyle.Top;
            this.c1DockingTab1.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab1.Name = "c1DockingTab1";
            this.c1DockingTab1.Size = new System.Drawing.Size(1328, 166);
            this.c1DockingTab1.TabIndex = 46;
            this.c1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.OfficeXP;
            // 
            // c1DockingTabPage1
            // 
            this.c1DockingTabPage1.Controls.Add(this.bdnInfo);
            this.c1DockingTabPage1.Controls.Add(this.panel1);
            this.c1DockingTabPage1.Location = new System.Drawing.Point(1, 33);
            this.c1DockingTabPage1.Name = "c1DockingTabPage1";
            this.c1DockingTabPage1.Size = new System.Drawing.Size(1326, 132);
            this.c1DockingTabPage1.TabIndex = 0;
            this.c1DockingTabPage1.Text = "查询条件";
            // 
            // c1DockingTab2
            // 
            this.c1DockingTab2.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.c1DockingTab2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1DockingTab2.Location = new System.Drawing.Point(0, 166);
            this.c1DockingTab2.Name = "c1DockingTab2";
            this.c1DockingTab2.Size = new System.Drawing.Size(1328, 438);
            this.c1DockingTab2.TabIndex = 47;
            this.c1DockingTab2.VisualStyleBase = C1.Win.C1Command.VisualStyle.OfficeXP;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 166);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gv_logmanager);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtb_msg);
            this.splitContainer1.Size = new System.Drawing.Size(1328, 438);
            this.splitContainer1.SplitterDistance = 349;
            this.splitContainer1.TabIndex = 51;
            // 
            // gv_logmanager
            // 
            this.gv_logmanager.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.gv_logmanager.AllowEditing = false;
            this.gv_logmanager.AllowFiltering = true;
            this.gv_logmanager.AutoGenerateColumns = false;
            this.gv_logmanager.ColumnInfo = resources.GetString("gv_logmanager.ColumnInfo");
            this.gv_logmanager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gv_logmanager.Location = new System.Drawing.Point(0, 0);
            this.gv_logmanager.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gv_logmanager.Name = "gv_logmanager";
            this.gv_logmanager.Rows.DefaultSize = 24;
            this.gv_logmanager.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.ListBox;
            this.gv_logmanager.ShowCursor = true;
            this.gv_logmanager.Size = new System.Drawing.Size(1328, 349);
            this.gv_logmanager.StyleInfo = resources.GetString("gv_logmanager.StyleInfo");
            this.gv_logmanager.TabIndex = 43;
            this.gv_logmanager.VisualStyle = C1.Win.C1FlexGrid.VisualStyle.Office2010Blue;
            this.gv_logmanager.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gv_logmanager_MouseClick);
            // 
            // rtb_msg
            // 
            this.rtb_msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_msg.Location = new System.Drawing.Point(0, 0);
            this.rtb_msg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtb_msg.Name = "rtb_msg";
            this.rtb_msg.Size = new System.Drawing.Size(1328, 85);
            this.rtb_msg.TabIndex = 2;
            this.rtb_msg.Text = "";
            // 
            // FrmGLBeventlog
            // 
            this.AcceptButton = this.btn_search;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1328, 604);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.c1DockingTab2);
            this.Controls.Add(this.c1DockingTab1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmGLBeventlog";
            this.Text = "总体监控_数据库日志";
            this.Load += new System.EventHandler(this.formGLBeventlog_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bdnInfo)).EndInit();
            this.bdnInfo.ResumeLayout(false);
            this.bdnInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).EndInit();
            this.c1DockingTab1.ResumeLayout(false);
            this.c1DockingTabPage1.ResumeLayout(false);
            this.c1DockingTabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab2)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gv_logmanager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bdsInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txt_key;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dtp_updateend;
        private System.Windows.Forms.DateTimePicker dtp_updatestart;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.BindingNavigator bdnInfo;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripTextBox txtCurrentPage;
        private System.Windows.Forms.ToolStripLabel lblPageCount;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.BindingSource bdsInfo;
        private System.Windows.Forms.TextBox txt_taskno;
        private System.Windows.Forms.Label lb_taskno;
        private System.Windows.Forms.TextBox txt_barcode;
        private System.Windows.Forms.Label label4;
        private C1.Win.C1Command.C1DockingTab c1DockingTab1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        private C1.Win.C1Command.C1DockingTab c1DockingTab2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox rtb_msg;
        private C1.Win.C1FlexGrid.C1FlexGrid gv_logmanager;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rb_error;
        private System.Windows.Forms.RadioButton rb_normal;
    }
}