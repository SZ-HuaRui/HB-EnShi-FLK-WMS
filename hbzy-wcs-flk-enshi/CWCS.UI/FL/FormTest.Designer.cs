namespace CWCS.UI.FL
{
    partial class FormTest
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
            this.btnCreateEntity = new DevExpress.XtraEditors.SimpleButton();
            this.btnTest = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // btnCreateEntity
            // 
            this.btnCreateEntity.Location = new System.Drawing.Point(50, 32);
            this.btnCreateEntity.Name = "btnCreateEntity";
            this.btnCreateEntity.Size = new System.Drawing.Size(99, 37);
            this.btnCreateEntity.TabIndex = 0;
            this.btnCreateEntity.Text = "生成实体类";
            this.btnCreateEntity.Click += new System.EventHandler(this.btnCreateEntity_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(195, 32);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(99, 37);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "测试";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // FormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnCreateEntity);
            this.Name = "FormTest";
            this.Text = "测试";
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnCreateEntity;
        private DevExpress.XtraEditors.SimpleButton btnTest;
    }
}