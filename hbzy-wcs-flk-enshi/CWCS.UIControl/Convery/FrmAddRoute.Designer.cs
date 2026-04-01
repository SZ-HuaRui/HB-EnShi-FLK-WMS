namespace CWCS.UIControl.Convery
{
    partial class FrmAddRoute
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAddRoute));
            this.cmbEquipment = new C1.Win.C1List.C1Combo();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbType = new C1.Win.C1List.C1Combo();
            this.label1 = new System.Windows.Forms.Label();
            this.c1Button1 = new C1.Win.C1Input.C1Button();
            this.btnGeneral = new C1.Win.C1Input.C1Button();
            this.txtEndNum = new C1.Win.C1Input.C1TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFromNum = new C1.Win.C1Input.C1TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cmbEquipment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromNum)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbEquipment
            // 
            this.cmbEquipment.AddItemSeparator = ';';
            this.cmbEquipment.AlternatingRows = true;
            this.cmbEquipment.Caption = "";
            this.cmbEquipment.CaptionHeight = 17;
            this.cmbEquipment.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cmbEquipment.ColumnCaptionHeight = 22;
            this.cmbEquipment.ColumnFooterHeight = 22;
            this.cmbEquipment.ContentHeight = 20;
            this.cmbEquipment.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cmbEquipment.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cmbEquipment.EditorFont = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbEquipment.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cmbEquipment.EditorHeight = 20;
            this.cmbEquipment.FlatStyle = C1.Win.C1List.FlatModeEnum.Standard;
            this.cmbEquipment.Images.Add(((System.Drawing.Image)(resources.GetObject("cmbEquipment.Images"))));
            this.cmbEquipment.ItemHeight = 15;
            this.cmbEquipment.Location = new System.Drawing.Point(180, 207);
            this.cmbEquipment.MatchEntryTimeout = ((long)(2000));
            this.cmbEquipment.MaxDropDownItems = ((short)(5));
            this.cmbEquipment.MaxLength = 32767;
            this.cmbEquipment.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cmbEquipment.Name = "cmbEquipment";
            this.cmbEquipment.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cmbEquipment.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cmbEquipment.Size = new System.Drawing.Size(164, 26);
            this.cmbEquipment.TabIndex = 44;
            this.cmbEquipment.VisualStyle = C1.Win.C1List.VisualStyle.Office2007Blue;
            this.cmbEquipment.PropBag = resources.GetString("cmbEquipment.PropBag");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(70, 218);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 15);
            this.label4.TabIndex = 43;
            this.label4.Text = "所属堆垛机：";
            // 
            // cmbType
            // 
            this.cmbType.AddItemSeparator = ';';
            this.cmbType.AlternatingRows = true;
            this.cmbType.Caption = "";
            this.cmbType.CaptionHeight = 17;
            this.cmbType.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cmbType.ColumnCaptionHeight = 22;
            this.cmbType.ColumnFooterHeight = 22;
            this.cmbType.ContentHeight = 20;
            this.cmbType.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cmbType.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cmbType.EditorFont = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbType.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cmbType.EditorHeight = 20;
            this.cmbType.FlatStyle = C1.Win.C1List.FlatModeEnum.Standard;
            this.cmbType.Images.Add(((System.Drawing.Image)(resources.GetObject("cmbType.Images"))));
            this.cmbType.ItemHeight = 15;
            this.cmbType.Location = new System.Drawing.Point(180, 156);
            this.cmbType.MatchEntryTimeout = ((long)(2000));
            this.cmbType.MaxDropDownItems = ((short)(5));
            this.cmbType.MaxLength = 32767;
            this.cmbType.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cmbType.Name = "cmbType";
            this.cmbType.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cmbType.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cmbType.Size = new System.Drawing.Size(164, 26);
            this.cmbType.TabIndex = 42;
            this.cmbType.VisualStyle = C1.Win.C1List.VisualStyle.Office2007Blue;
            this.cmbType.PropBag = resources.GetString("cmbType.PropBag");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(107, 167);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 41;
            this.label1.Text = "类型：";
            // 
            // c1Button1
            // 
            this.c1Button1.Location = new System.Drawing.Point(240, 317);
            this.c1Button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.c1Button1.Name = "c1Button1";
            this.c1Button1.Size = new System.Drawing.Size(119, 28);
            this.c1Button1.TabIndex = 40;
            this.c1Button1.Text = "取消(&C)";
            this.c1Button1.UseVisualStyleBackColor = true;
            this.c1Button1.VisualStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.c1Button1.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            // 
            // btnGeneral
            // 
            this.btnGeneral.Location = new System.Drawing.Point(88, 317);
            this.btnGeneral.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGeneral.Name = "btnGeneral";
            this.btnGeneral.Size = new System.Drawing.Size(119, 28);
            this.btnGeneral.TabIndex = 39;
            this.btnGeneral.Text = "生成路径";
            this.btnGeneral.UseVisualStyleBackColor = true;
            this.btnGeneral.VisualStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.btnGeneral.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.btnGeneral.Click += new System.EventHandler(this.btnGeneral_Click);
            // 
            // txtEndNum
            // 
            this.txtEndNum.Label = this.label3;
            this.txtEndNum.Location = new System.Drawing.Point(180, 110);
            this.txtEndNum.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtEndNum.Name = "txtEndNum";
            this.txtEndNum.Size = new System.Drawing.Size(164, 25);
            this.txtEndNum.TabIndex = 36;
            this.txtEndNum.Tag = null;
            this.txtEndNum.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2007Blue;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(77, 115);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 38;
            this.label3.Text = "目标站台：";
            // 
            // txtFromNum
            // 
            this.txtFromNum.Label = this.label2;
            this.txtFromNum.Location = new System.Drawing.Point(180, 63);
            this.txtFromNum.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFromNum.Name = "txtFromNum";
            this.txtFromNum.Size = new System.Drawing.Size(164, 25);
            this.txtFromNum.TabIndex = 35;
            this.txtFromNum.Tag = null;
            this.txtFromNum.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2007Blue;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(77, 68);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 37;
            this.label2.Text = "起始站台：";
            // 
            // FrmAddRoute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 409);
            this.Controls.Add(this.cmbEquipment);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.c1Button1);
            this.Controls.Add(this.btnGeneral);
            this.Controls.Add(this.txtEndNum);
            this.Controls.Add(this.txtFromNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Name = "FrmAddRoute";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加路径";
            this.Load += new System.EventHandler(this.FrmAddRoute_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cmbEquipment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private C1.Win.C1List.C1Combo cmbEquipment;
        private System.Windows.Forms.Label label4;
        private C1.Win.C1List.C1Combo cmbType;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1Button c1Button1;
        private C1.Win.C1Input.C1Button btnGeneral;
        private C1.Win.C1Input.C1TextBox txtEndNum;
        private System.Windows.Forms.Label label3;
        private C1.Win.C1Input.C1TextBox txtFromNum;
        private System.Windows.Forms.Label label2;

    }
}