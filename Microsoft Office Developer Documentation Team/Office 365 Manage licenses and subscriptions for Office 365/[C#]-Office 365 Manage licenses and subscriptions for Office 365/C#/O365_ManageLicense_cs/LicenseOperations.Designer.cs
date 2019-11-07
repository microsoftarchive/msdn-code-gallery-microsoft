namespace O365_ManageLicenses
{
    partial class LicenseOperations
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
            this.UserHeading = new System.Windows.Forms.Label();
            this.eading = new System.Windows.Forms.Label();
            this.HeaderText = new System.Windows.Forms.Label();
            this.ManageLicenseOperations = new System.Windows.Forms.Button();
            this.SelectUser = new System.Windows.Forms.ComboBox();
            this.SelectLicense = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // UserHeading
            // 
            this.UserHeading.AutoSize = true;
            this.UserHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.UserHeading.Location = new System.Drawing.Point(24, 44);
            this.UserHeading.Name = "UserHeading";
            this.UserHeading.Size = new System.Drawing.Size(78, 15);
            this.UserHeading.TabIndex = 0;
            this.UserHeading.Text = "Select User *";
            // 
            // eading
            // 
            this.eading.AutoSize = true;
            this.eading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.eading.Location = new System.Drawing.Point(24, 71);
            this.eading.Name = "eading";
            this.eading.Size = new System.Drawing.Size(95, 15);
            this.eading.TabIndex = 1;
            this.eading.Text = "Select License *";
            // 
            // HeaderText
            // 
            this.HeaderText.AutoSize = true;
            this.HeaderText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderText.Location = new System.Drawing.Point(24, 15);
            this.HeaderText.Name = "HeaderText";
            this.HeaderText.Size = new System.Drawing.Size(294, 17);
            this.HeaderText.TabIndex = 2;
            this.HeaderText.Text = "Please enter below details to create new user";
            // 
            // ManageLicenseOperations
            // 
            this.ManageLicenseOperations.Location = new System.Drawing.Point(123, 99);
            this.ManageLicenseOperations.Name = "ManageLicenseOperations";
            this.ManageLicenseOperations.Size = new System.Drawing.Size(95, 23);
            this.ManageLicenseOperations.TabIndex = 3;
            this.ManageLicenseOperations.Text = "Assign License";
            this.ManageLicenseOperations.UseVisualStyleBackColor = true;
            this.ManageLicenseOperations.Click += new System.EventHandler(this.ManageLicenseOperations_Click);
            // 
            // SelectUser
            // 
            this.SelectUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectUser.FormattingEnabled = true;
            this.SelectUser.Location = new System.Drawing.Point(123, 44);
            this.SelectUser.Name = "SelectUser";
            this.SelectUser.Size = new System.Drawing.Size(231, 21);
            this.SelectUser.TabIndex = 1;
            // 
            // SelectLicense
            // 
            this.SelectLicense.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectLicense.FormattingEnabled = true;
            this.SelectLicense.Location = new System.Drawing.Point(123, 71);
            this.SelectLicense.Name = "SelectLicense";
            this.SelectLicense.Size = new System.Drawing.Size(231, 21);
            this.SelectLicense.TabIndex = 2;
            // 
            // LicenseOperations
            // 
            this.AcceptButton = this.ManageLicenseOperations;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(379, 136);
            this.Controls.Add(this.SelectLicense);
            this.Controls.Add(this.SelectUser);
            this.Controls.Add(this.ManageLicenseOperations);
            this.Controls.Add(this.HeaderText);
            this.Controls.Add(this.eading);
            this.Controls.Add(this.UserHeading);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseOperations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage License";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label UserHeading;
        private System.Windows.Forms.Label eading;
        private System.Windows.Forms.Label HeaderText;
        private System.Windows.Forms.Button ManageLicenseOperations;
        private System.Windows.Forms.ComboBox SelectUser;
        private System.Windows.Forms.ComboBox SelectLicense;
    }
}