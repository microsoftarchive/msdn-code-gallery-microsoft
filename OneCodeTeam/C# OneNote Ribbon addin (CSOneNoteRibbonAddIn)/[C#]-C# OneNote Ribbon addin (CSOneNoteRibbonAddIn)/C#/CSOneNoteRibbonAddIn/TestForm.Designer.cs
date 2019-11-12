namespace CSOneNoteRibbonAddIn
{
    partial class TestForm
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
            this.btnGetPageTitle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGetPageTitle
            // 
            this.btnGetPageTitle.Location = new System.Drawing.Point(78, 91);
            this.btnGetPageTitle.Name = "btnGetPageTitle";
            this.btnGetPageTitle.Size = new System.Drawing.Size(133, 23);
            this.btnGetPageTitle.TabIndex = 0;
            this.btnGetPageTitle.Text = "GetPageTitle";
            this.btnGetPageTitle.UseVisualStyleBackColor = true;
            this.btnGetPageTitle.Click += new System.EventHandler(this.btnClick_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 254);
            this.Controls.Add(this.btnGetPageTitle);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetPageTitle;
    }
}