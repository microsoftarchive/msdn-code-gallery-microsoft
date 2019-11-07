namespace CSCreateLowIntegrityProcess
{
    partial class MainForm
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
            this.lbIntegrityLevel = new System.Windows.Forms.Label();
            this.btnCreateLowProcess = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnWriteLocalAppData = new System.Windows.Forms.Button();
            this.btnWriteLocalAppDataLow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Curent Integrity Level:";
            // 
            // lbIntegrityLevel
            // 
            this.lbIntegrityLevel.AutoSize = true;
            this.lbIntegrityLevel.Location = new System.Drawing.Point(127, 13);
            this.lbIntegrityLevel.Name = "lbIntegrityLevel";
            this.lbIntegrityLevel.Size = new System.Drawing.Size(0, 13);
            this.lbIntegrityLevel.TabIndex = 1;
            // 
            // btnCreateLowProcess
            // 
            this.btnCreateLowProcess.Location = new System.Drawing.Point(12, 34);
            this.btnCreateLowProcess.Name = "btnCreateLowProcess";
            this.btnCreateLowProcess.Size = new System.Drawing.Size(196, 23);
            this.btnCreateLowProcess.TabIndex = 2;
            this.btnCreateLowProcess.Text = "Launch myself at low integrity level";
            this.btnCreateLowProcess.UseVisualStyleBackColor = true;
            this.btnCreateLowProcess.Click += new System.EventHandler(this.btnCreateLowProcess_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tests:";
            // 
            // btnWriteLocalAppData
            // 
            this.btnWriteLocalAppData.Location = new System.Drawing.Point(12, 91);
            this.btnWriteLocalAppData.Name = "btnWriteLocalAppData";
            this.btnWriteLocalAppData.Size = new System.Drawing.Size(196, 23);
            this.btnWriteLocalAppData.TabIndex = 4;
            this.btnWriteLocalAppData.Text = "Write to the LocalAppData folder";
            this.btnWriteLocalAppData.UseVisualStyleBackColor = true;
            this.btnWriteLocalAppData.Click += new System.EventHandler(this.btnWriteLocalAppData_Click);
            // 
            // btnWriteLocalAppDataLow
            // 
            this.btnWriteLocalAppDataLow.Location = new System.Drawing.Point(12, 121);
            this.btnWriteLocalAppDataLow.Name = "btnWriteLocalAppDataLow";
            this.btnWriteLocalAppDataLow.Size = new System.Drawing.Size(196, 23);
            this.btnWriteLocalAppDataLow.TabIndex = 5;
            this.btnWriteLocalAppDataLow.Text = "Write to the LocalAppDataLow folder";
            this.btnWriteLocalAppDataLow.UseVisualStyleBackColor = true;
            this.btnWriteLocalAppDataLow.Click += new System.EventHandler(this.btnWriteLocalAppDataLow_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 157);
            this.Controls.Add(this.btnWriteLocalAppDataLow);
            this.Controls.Add(this.btnWriteLocalAppData);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCreateLowProcess);
            this.Controls.Add(this.lbIntegrityLevel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "CSCreateLowIntegrityProcess";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbIntegrityLevel;
        private System.Windows.Forms.Button btnCreateLowProcess;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnWriteLocalAppData;
        private System.Windows.Forms.Button btnWriteLocalAppDataLow;
    }
}

