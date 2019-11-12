namespace CSWinFormSingleInstanceApp
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
            this.labelWelcomeMsg = new System.Windows.Forms.Label();
            this.buttonLogoff = new System.Windows.Forms.Button();
            this.labelUserStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelWelcomeMsg
            // 
            this.labelWelcomeMsg.AutoSize = true;
            this.labelWelcomeMsg.Location = new System.Drawing.Point(12, 9);
            this.labelWelcomeMsg.Name = "labelWelcomeMsg";
            this.labelWelcomeMsg.Size = new System.Drawing.Size(0, 13);
            this.labelWelcomeMsg.TabIndex = 0;
            // 
            // buttonLogoff
            // 
            this.buttonLogoff.Location = new System.Drawing.Point(197, 201);
            this.buttonLogoff.Name = "buttonLogoff";
            this.buttonLogoff.Size = new System.Drawing.Size(75, 23);
            this.buttonLogoff.TabIndex = 1;
            this.buttonLogoff.Text = "Logoff";
            this.buttonLogoff.UseVisualStyleBackColor = true;
            this.buttonLogoff.Click += new System.EventHandler(this.buttonLogoff_Click);
            // 
            // labelUserStatus
            // 
            this.labelUserStatus.AutoSize = true;
            this.labelUserStatus.Location = new System.Drawing.Point(12, 33);
            this.labelUserStatus.Name = "labelUserStatus";
            this.labelUserStatus.Size = new System.Drawing.Size(35, 13);
            this.labelUserStatus.TabIndex = 2;
            this.labelUserStatus.Text = "label1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.labelUserStatus);
            this.Controls.Add(this.buttonLogoff);
            this.Controls.Add(this.labelWelcomeMsg);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWelcomeMsg;
        private System.Windows.Forms.Button buttonLogoff;
        private System.Windows.Forms.Label labelUserStatus;
    }
}

