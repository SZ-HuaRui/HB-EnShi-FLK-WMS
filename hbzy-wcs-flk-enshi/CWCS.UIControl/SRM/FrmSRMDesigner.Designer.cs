namespace CWCS.UIControl
{
    partial class FrmSRMDesigner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSRMDesigner));
            this.c1DockingTabPage2 = new C1.Win.C1Command.C1DockingTabPage();
            this.grdSendLog = new C1.Win.C1FlexGrid.C1FlexGrid();
            this.c1DockingTab2 = new C1.Win.C1Command.C1DockingTab();
            this.label6 = new System.Windows.Forms.Label();
            this.btnModity = new C1.Win.C1Input.C1Button();
            this.btnInsert = new C1.Win.C1Input.C1Button();
            this.txtWDBLength = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtWDBAddr = new System.Windows.Forms.TextBox();
            this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
            this.cmbProrocol = new C1.Win.C1List.C1Combo();
            this.txtPutPoint = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtPickPoint = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnSave = new C1.Win.C1Input.C1Button();
            this.btnDelete = new C1.Win.C1Input.C1Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtRDBLength = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRDBAddr = new System.Windows.Forms.TextBox();
            this.txtWritePort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtReadPort = new System.Windows.Forms.TextBox();
            this.txtEquipNum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtWriteIP = new System.Windows.Forms.TextBox();
            this.txtReadIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.c1DockingTab3 = new C1.Win.C1Command.C1DockingTab();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.c1CommandDock3 = new C1.Win.C1Command.C1CommandDock();
            this.startStep1 = new CWCS.UIDesigner.WizardBase.StartStep();
            this.finishStep1 = new CWCS.UIDesigner.WizardBase.FinishStep();
            this.intermediateStep2 = new CWCS.UIDesigner.WizardBase.IntermediateStep();
            this.intermediateStep1 = new CWCS.UIDesigner.WizardBase.IntermediateStep();
            this.startStep2 = new CWCS.UIDesigner.WizardBase.StartStep();
            this.finishStep2 = new CWCS.UIDesigner.WizardBase.FinishStep();
            this.intermediateStep3 = new CWCS.UIDesigner.WizardBase.IntermediateStep();
            this.finishStep3 = new CWCS.UIDesigner.WizardBase.FinishStep();
            this.c1DockingTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSendLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab2)).BeginInit();
            this.c1DockingTab2.SuspendLayout();
            this.c1DockingTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbProrocol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab3)).BeginInit();
            this.c1DockingTab3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandDock3)).BeginInit();
            this.c1CommandDock3.SuspendLayout();
            this.SuspendLayout();
            // 
            // c1DockingTabPage2
            // 
            this.c1DockingTabPage2.CaptionVisible = true;
            this.c1DockingTabPage2.Controls.Add(this.grdSendLog);
            this.c1DockingTabPage2.Location = new System.Drawing.Point(1, 1);
            this.c1DockingTabPage2.Name = "c1DockingTabPage2";
            this.c1DockingTabPage2.Size = new System.Drawing.Size(502, 599);
            this.c1DockingTabPage2.TabIndex = 0;
            this.c1DockingTabPage2.Text = "堆垛机配置";
            // 
            // grdSendLog
            // 
            this.grdSendLog.ColumnInfo = resources.GetString("grdSendLog.ColumnInfo");
            this.grdSendLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdSendLog.Location = new System.Drawing.Point(0, 19);
            this.grdSendLog.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.grdSendLog.Name = "grdSendLog";
            this.grdSendLog.Rows.Count = 1;
            this.grdSendLog.Rows.DefaultSize = 24;
            this.grdSendLog.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.ListBox;
            this.grdSendLog.ShowCursor = true;
            this.grdSendLog.Size = new System.Drawing.Size(502, 580);
            this.grdSendLog.TabIndex = 2;
            this.grdSendLog.VisualStyle = C1.Win.C1FlexGrid.VisualStyle.Office2010Blue;
            this.grdSendLog.MouseClick += new System.Windows.Forms.MouseEventHandler(this.grdSendLog_MouseClick);
            // 
            // c1DockingTab2
            // 
            this.c1DockingTab2.AcceptsCtrlTab = false;
            this.c1DockingTab2.Controls.Add(this.c1DockingTabPage2);
            this.c1DockingTab2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1DockingTab2.Location = new System.Drawing.Point(300, 0);
            this.c1DockingTab2.Name = "c1DockingTab2";
            this.c1DockingTab2.ShowCaption = true;
            this.c1DockingTab2.ShowSingleTab = false;
            this.c1DockingTab2.ShowTabList = true;
            this.c1DockingTab2.ShowToolTips = true;
            this.c1DockingTab2.Size = new System.Drawing.Size(504, 601);
            this.c1DockingTab2.TabIndex = 16;
            this.c1DockingTab2.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2007;
            this.c1DockingTab2.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2007Blue;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(31, 379);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 40;
            this.label6.Text = "读取DB长度：";
            // 
            // btnModity
            // 
            this.btnModity.Location = new System.Drawing.Point(159, 506);
            this.btnModity.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnModity.Name = "btnModity";
            this.btnModity.Size = new System.Drawing.Size(84, 18);
            this.btnModity.TabIndex = 37;
            this.btnModity.Text = "修改(&M)";
            this.btnModity.UseVisualStyleBackColor = true;
            this.btnModity.Click += new System.EventHandler(this.btnModity_Click);
            // 
            // btnInsert
            // 
            this.btnInsert.Location = new System.Drawing.Point(45, 506);
            this.btnInsert.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(84, 18);
            this.btnInsert.TabIndex = 36;
            this.btnInsert.Text = "添加(&I)";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // txtWDBLength
            // 
            this.txtWDBLength.Location = new System.Drawing.Point(110, 377);
            this.txtWDBLength.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWDBLength.Name = "txtWDBLength";
            this.txtWDBLength.Size = new System.Drawing.Size(122, 21);
            this.txtWDBLength.TabIndex = 35;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(20, 342);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 12);
            this.label10.TabIndex = 33;
            this.label10.Text = "读取DB块地址：";
            // 
            // txtWDBAddr
            // 
            this.txtWDBAddr.Location = new System.Drawing.Point(110, 340);
            this.txtWDBAddr.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWDBAddr.Name = "txtWDBAddr";
            this.txtWDBAddr.Size = new System.Drawing.Size(122, 21);
            this.txtWDBAddr.TabIndex = 32;
            // 
            // c1DockingTabPage1
            // 
            this.c1DockingTabPage1.CaptionVisible = true;
            this.c1DockingTabPage1.Controls.Add(this.cmbProrocol);
            this.c1DockingTabPage1.Controls.Add(this.txtPutPoint);
            this.c1DockingTabPage1.Controls.Add(this.label12);
            this.c1DockingTabPage1.Controls.Add(this.txtPickPoint);
            this.c1DockingTabPage1.Controls.Add(this.label11);
            this.c1DockingTabPage1.Controls.Add(this.btnSave);
            this.c1DockingTabPage1.Controls.Add(this.btnDelete);
            this.c1DockingTabPage1.Controls.Add(this.label5);
            this.c1DockingTabPage1.Controls.Add(this.label6);
            this.c1DockingTabPage1.Controls.Add(this.btnModity);
            this.c1DockingTabPage1.Controls.Add(this.btnInsert);
            this.c1DockingTabPage1.Controls.Add(this.txtWDBLength);
            this.c1DockingTabPage1.Controls.Add(this.label10);
            this.c1DockingTabPage1.Controls.Add(this.txtWDBAddr);
            this.c1DockingTabPage1.Controls.Add(this.txtRDBLength);
            this.c1DockingTabPage1.Controls.Add(this.label9);
            this.c1DockingTabPage1.Controls.Add(this.label8);
            this.c1DockingTabPage1.Controls.Add(this.txtRDBAddr);
            this.c1DockingTabPage1.Controls.Add(this.txtWritePort);
            this.c1DockingTabPage1.Controls.Add(this.label7);
            this.c1DockingTabPage1.Controls.Add(this.txtReadPort);
            this.c1DockingTabPage1.Controls.Add(this.txtEquipNum);
            this.c1DockingTabPage1.Controls.Add(this.label4);
            this.c1DockingTabPage1.Controls.Add(this.txtWriteIP);
            this.c1DockingTabPage1.Controls.Add(this.txtReadIP);
            this.c1DockingTabPage1.Controls.Add(this.label1);
            this.c1DockingTabPage1.Controls.Add(this.label2);
            this.c1DockingTabPage1.Controls.Add(this.label3);
            this.c1DockingTabPage1.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.c1DockingTabPage1.Name = "c1DockingTabPage1";
            this.c1DockingTabPage1.Size = new System.Drawing.Size(297, 576);
            this.c1DockingTabPage1.TabIndex = 0;
            this.c1DockingTabPage1.Text = "设备信息";
            // 
            // cmbProrocol
            // 
            this.cmbProrocol.AddItemSeparator = ';';
            this.cmbProrocol.Caption = "";
            this.cmbProrocol.CaptionHeight = 17;
            this.cmbProrocol.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cmbProrocol.ColumnCaptionHeight = 22;
            this.cmbProrocol.ColumnFooterHeight = 22;
            this.cmbProrocol.ContentHeight = 16;
            this.cmbProrocol.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cmbProrocol.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cmbProrocol.EditorFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbProrocol.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cmbProrocol.EditorHeight = 16;
            this.cmbProrocol.Images.Add(((System.Drawing.Image)(resources.GetObject("cmbProrocol.Images"))));
            this.cmbProrocol.ItemHeight = 15;
            this.cmbProrocol.Location = new System.Drawing.Point(110, 79);
            this.cmbProrocol.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbProrocol.MatchEntryTimeout = ((long)(2000));
            this.cmbProrocol.MaxDropDownItems = ((short)(5));
            this.cmbProrocol.MaxLength = 32767;
            this.cmbProrocol.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cmbProrocol.Name = "cmbProrocol";
            this.cmbProrocol.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cmbProrocol.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cmbProrocol.Size = new System.Drawing.Size(122, 22);
            this.cmbProrocol.TabIndex = 49;
            this.cmbProrocol.PropBag = resources.GetString("cmbProrocol.PropBag");
            // 
            // txtPutPoint
            // 
            this.txtPutPoint.Location = new System.Drawing.Point(110, 457);
            this.txtPutPoint.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPutPoint.Name = "txtPutPoint";
            this.txtPutPoint.Size = new System.Drawing.Size(122, 21);
            this.txtPutPoint.TabIndex = 48;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Location = new System.Drawing.Point(53, 459);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 12);
            this.label12.TabIndex = 47;
            this.label12.Text = "放货点：";
            // 
            // txtPickPoint
            // 
            this.txtPickPoint.Location = new System.Drawing.Point(110, 418);
            this.txtPickPoint.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPickPoint.Name = "txtPickPoint";
            this.txtPickPoint.Size = new System.Drawing.Size(122, 21);
            this.txtPickPoint.TabIndex = 46;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(54, 421);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 45;
            this.label11.Text = "取货点：";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(159, 542);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(84, 18);
            this.btnSave.TabIndex = 44;
            this.btnSave.Text = "保存(&S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(45, 542);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(84, 18);
            this.btnDelete.TabIndex = 43;
            this.btnDelete.Text = "删除(&D)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(43, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 41;
            this.label5.Text = "协议类型：";
            // 
            // txtRDBLength
            // 
            this.txtRDBLength.Location = new System.Drawing.Point(110, 302);
            this.txtRDBLength.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtRDBLength.Name = "txtRDBLength";
            this.txtRDBLength.Size = new System.Drawing.Size(122, 21);
            this.txtRDBLength.TabIndex = 31;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(31, 305);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 12);
            this.label9.TabIndex = 30;
            this.label9.Text = "写入DB长度：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(20, 268);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 12);
            this.label8.TabIndex = 29;
            this.label8.Text = "写入DB块地址：";
            // 
            // txtRDBAddr
            // 
            this.txtRDBAddr.Location = new System.Drawing.Point(110, 266);
            this.txtRDBAddr.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtRDBAddr.Name = "txtRDBAddr";
            this.txtRDBAddr.Size = new System.Drawing.Size(122, 21);
            this.txtRDBAddr.TabIndex = 28;
            // 
            // txtWritePort
            // 
            this.txtWritePort.Location = new System.Drawing.Point(110, 229);
            this.txtWritePort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWritePort.Name = "txtWritePort";
            this.txtWritePort.Size = new System.Drawing.Size(122, 21);
            this.txtWritePort.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(53, 194);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 12);
            this.label7.TabIndex = 26;
            this.label7.Text = "写入IP:";
            // 
            // txtReadPort
            // 
            this.txtReadPort.Location = new System.Drawing.Point(110, 157);
            this.txtReadPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtReadPort.Name = "txtReadPort";
            this.txtReadPort.Size = new System.Drawing.Size(122, 21);
            this.txtReadPort.TabIndex = 19;
            // 
            // txtEquipNum
            // 
            this.txtEquipNum.Location = new System.Drawing.Point(110, 44);
            this.txtEquipNum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtEquipNum.Name = "txtEquipNum";
            this.txtEquipNum.Size = new System.Drawing.Size(122, 21);
            this.txtEquipNum.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(43, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "读取端口：";
            // 
            // txtWriteIP
            // 
            this.txtWriteIP.Location = new System.Drawing.Point(110, 192);
            this.txtWriteIP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWriteIP.Name = "txtWriteIP";
            this.txtWriteIP.Size = new System.Drawing.Size(122, 21);
            this.txtWriteIP.TabIndex = 25;
            // 
            // txtReadIP
            // 
            this.txtReadIP.Location = new System.Drawing.Point(110, 121);
            this.txtReadIP.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtReadIP.Name = "txtReadIP";
            this.txtReadIP.Size = new System.Drawing.Size(122, 21);
            this.txtReadIP.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(43, 231);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 24;
            this.label1.Text = "写入端口：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(43, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "设备编号：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(53, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "读取IP：";
            // 
            // c1DockingTab3
            // 
            this.c1DockingTab3.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.c1DockingTab3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1DockingTab3.CanAutoHide = true;
            this.c1DockingTab3.CanCloseTabs = true;
            this.c1DockingTab3.CanMoveTabs = true;
            this.c1DockingTab3.Controls.Add(this.c1DockingTabPage1);
            this.c1DockingTab3.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.c1DockingTab3.Name = "c1DockingTab3";
            this.c1DockingTab3.SelectedTabBold = true;
            this.c1DockingTab3.ShowCaption = true;
            this.c1DockingTab3.Size = new System.Drawing.Size(300, 601);
            this.c1DockingTab3.TabIndex = 1;
            this.c1DockingTab3.TabSizeMode = C1.Win.C1Command.TabSizeModeEnum.Fit;
            this.c1DockingTab3.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2007;
            this.c1DockingTab3.VisualStyle = C1.Win.C1Command.VisualStyle.Office2007Blue;
            this.c1DockingTab3.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2007Blue;
            // 
            // c1CommandDock3
            // 
            this.c1CommandDock3.Controls.Add(this.c1DockingTab3);
            this.c1CommandDock3.Dock = System.Windows.Forms.DockStyle.Left;
            this.c1CommandDock3.Id = 8;
            this.c1CommandDock3.Location = new System.Drawing.Point(0, 0);
            this.c1CommandDock3.Name = "c1CommandDock3";
            this.c1CommandDock3.Size = new System.Drawing.Size(300, 601);
            // 
            // startStep1
            // 
            this.startStep1.BindingImage = ((System.Drawing.Image)(resources.GetObject("startStep1.BindingImage")));
            this.startStep1.Icon = ((System.Drawing.Image)(resources.GetObject("startStep1.Icon")));
            this.startStep1.Name = "startStep1";
            this.startStep1.Subtitle = "请输相关堆垛机信息信息";
            this.startStep1.SubtitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.startStep1.Title = "欢迎使用CWCS配置工具";
            this.startStep1.TitleFont = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            // 
            // finishStep1
            // 
            this.finishStep1.Name = "finishStep1";
            // 
            // intermediateStep2
            // 
            this.intermediateStep2.Name = "intermediateStep2";
            this.intermediateStep2.SubtitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.intermediateStep2.Title = "New WizardControl step.";
            this.intermediateStep2.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            // 
            // intermediateStep1
            // 
            this.intermediateStep1.Name = "intermediateStep1";
            this.intermediateStep1.SubtitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.intermediateStep1.Title = "New WizardControl step.";
            this.intermediateStep1.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            // 
            // startStep2
            // 
            this.startStep2.BindingImage = ((System.Drawing.Image)(resources.GetObject("startStep2.BindingImage")));
            this.startStep2.Icon = ((System.Drawing.Image)(resources.GetObject("startStep2.Icon")));
            this.startStep2.Name = "startStep2";
            this.startStep2.Subtitle = "请输相关堆垛机信息信息";
            this.startStep2.SubtitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.startStep2.Title = "欢迎使用CWCS配置工具";
            this.startStep2.TitleFont = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            // 
            // finishStep2
            // 
            this.finishStep2.Name = "finishStep2";
            // 
            // intermediateStep3
            // 
            this.intermediateStep3.Name = "intermediateStep3";
            this.intermediateStep3.SubtitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.intermediateStep3.Title = "New WizardControl step.";
            this.intermediateStep3.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            // 
            // finishStep3
            // 
            this.finishStep3.Name = "finishStep3";
            // 
            // FrmSRMDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 601);
            this.Controls.Add(this.c1DockingTab2);
            this.Controls.Add(this.c1CommandDock3);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FrmSRMDesigner";
            this.Text = "堆垛机配置";
            this.Load += new System.EventHandler(this.FrmSCDesigner_Load);
            this.c1DockingTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdSendLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab2)).EndInit();
            this.c1DockingTab2.ResumeLayout(false);
            this.c1DockingTabPage1.ResumeLayout(false);
            this.c1DockingTabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbProrocol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab3)).EndInit();
            this.c1DockingTab3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandDock3)).EndInit();
            this.c1CommandDock3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UIDesigner.WizardBase.StartStep startStep1;
        private UIDesigner.WizardBase.FinishStep finishStep1;
        private UIDesigner.WizardBase.IntermediateStep intermediateStep2;
        private UIDesigner.WizardBase.IntermediateStep intermediateStep1;
        private UIDesigner.WizardBase.StartStep startStep2;
        private UIDesigner.WizardBase.FinishStep finishStep2;
        private UIDesigner.WizardBase.IntermediateStep intermediateStep3;
        private UIDesigner.WizardBase.FinishStep finishStep3;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage2;
        private C1.Win.C1Command.C1DockingTab c1DockingTab2;
        private System.Windows.Forms.Label label6;
        private C1.Win.C1Input.C1Button btnModity;
        private C1.Win.C1Input.C1Button btnInsert;
        private System.Windows.Forms.TextBox txtWDBLength;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtWDBAddr;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        private System.Windows.Forms.TextBox txtRDBLength;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtRDBAddr;
        private System.Windows.Forms.TextBox txtWritePort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtReadPort;
        private System.Windows.Forms.TextBox txtEquipNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtWriteIP;
        private System.Windows.Forms.TextBox txtReadIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private C1.Win.C1Command.C1DockingTab c1DockingTab3;
        private System.Windows.Forms.ToolTip toolTip1;
        private C1.Win.C1Command.C1CommandDock c1CommandDock3;
        private C1.Win.C1FlexGrid.C1FlexGrid grdSendLog;
        private C1.Win.C1Input.C1Button btnSave;
        private C1.Win.C1Input.C1Button btnDelete;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPutPoint;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtPickPoint;
        private System.Windows.Forms.Label label11;
        private C1.Win.C1List.C1Combo cmbProrocol;


    }
}