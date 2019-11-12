namespace O365_ManageUsers
{
    partial class LoginUser
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
            this.LastNameHeading = new System.Windows.Forms.Label();
            this.DisplayNameHeading = new System.Windows.Forms.Label();
            this.UserPrincipalName = new System.Windows.Forms.TextBox();
            this.UserPassword = new System.Windows.Forms.TextBox();
            this.Login = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LastNameHeading
            // 
            this.LastNameHeading.AutoSize = true;
            this.LastNameHeading.Location = new System.Drawing.Point(27, 21);
            this.LastNameHeading.Name = "LastNameHeading";
            this.LastNameHeading.Size = new System.Drawing.Size(60, 13);
            this.LastNameHeading.TabIndex = 0;
            this.LastNameHeading.Text = "User Name";
            // 
            // DisplayNameHeading
            // 
            this.DisplayNameHeading.AutoSize = true;
            this.DisplayNameHeading.Location = new System.Drawing.Point(27, 49);
            this.DisplayNameHeading.Name = "DisplayNameHeading";
            this.DisplayNameHeading.Size = new System.Drawing.Size(53, 13);
            this.DisplayNameHeading.TabIndex = 1;
            this.DisplayNameHeading.Text = "Password";
            // 
            // UserPrincipalName
            // 
            this.UserPrincipalName.Location = new System.Drawing.Point(95, 21);
            this.UserPrincipalName.Name = "UserPrincipalName";
            this.UserPrincipalName.Size = new System.Drawing.Size(185, 20);
            this.UserPrincipalName.TabIndex = 0;
            // 
            // UserPassword
            // 
            this.UserPassword.Location = new System.Drawing.Point(95, 49);
            this.UserPassword.Name = "UserPassword";
            this.UserPassword.PasswordChar = '*';
            this.UserPassword.Size = new System.Drawing.Size(185, 20);
            this.UserPassword.TabIndex = 1;
            // 
            // Login
            // 
            this.Login.Location = new System.Drawing.Point(95, 77);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(75, 23);
            this.Login.TabIndex = 2;
            this.Login.Text = "Login";
            this.Login.UseVisualStyleBackColor = true;
            this.Login.Click += new System.EventHandler(this.Login_Click);
            // 
            // LoginUser
            // 
            this.AcceptButton = this.Login;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 120);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.UserPassword);
            this.Controls.Add(this.UserPrincipalName);
            this.Controls.Add(this.DisplayNameHeading);
            this.Controls.Add(this.LastNameHeading);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Credentials";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LastNameHeading;
        private System.Windows.Forms.Label DisplayNameHeading;
        private System.Windows.Forms.TextBox UserPrincipalName;
        private System.Windows.Forms.TextBox UserPassword;
        private System.Windows.Forms.Button Login;
    }
}