namespace O365_ManageUsers
{
    partial class UserOperations
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
            this.UserIDHeading = new System.Windows.Forms.Label();
            this.FirstNameHeading = new System.Windows.Forms.Label();
            this.Header = new System.Windows.Forms.Label();
            this.UsageLocationHeading = new System.Windows.Forms.Label();
            this.UserPrincipalName = new System.Windows.Forms.TextBox();
            this.FirstName = new System.Windows.Forms.TextBox();
            this.UsageLocation = new System.Windows.Forms.TextBox();
            this.SaveUser = new System.Windows.Forms.Button();
            this.DepartmentHeading = new System.Windows.Forms.Label();
            this.Department = new System.Windows.Forms.TextBox();
            this.Country = new System.Windows.Forms.TextBox();
            this.CountryHeading = new System.Windows.Forms.Label();
            this.LastName = new System.Windows.Forms.TextBox();
            this.LastNameHeading = new System.Windows.Forms.Label();
            this.DisplayName = new System.Windows.Forms.TextBox();
            this.DisplayNameHeading = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UserIDHeading
            // 
            this.UserIDHeading.AutoSize = true;
            this.UserIDHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserIDHeading.Location = new System.Drawing.Point(25, 41);
            this.UserIDHeading.Name = "UserIDHeading";
            this.UserIDHeading.Size = new System.Drawing.Size(129, 15);
            this.UserIDHeading.TabIndex = 0;
            this.UserIDHeading.Text = "User Principal Name *";
            // 
            // FirstNameHeading
            // 
            this.FirstNameHeading.AutoSize = true;
            this.FirstNameHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FirstNameHeading.Location = new System.Drawing.Point(25, 67);
            this.FirstNameHeading.Name = "FirstNameHeading";
            this.FirstNameHeading.Size = new System.Drawing.Size(67, 15);
            this.FirstNameHeading.TabIndex = 1;
            this.FirstNameHeading.Text = "First Name";
            // 
            // Header
            // 
            this.Header.AutoSize = true;
            this.Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Header.Location = new System.Drawing.Point(25, 11);
            this.Header.Name = "Header";
            this.Header.Size = new System.Drawing.Size(294, 17);
            this.Header.TabIndex = 2;
            this.Header.Text = "Please enter below details to create new user";
            // 
            // UsageLocationHeading
            // 
            this.UsageLocationHeading.AutoSize = true;
            this.UsageLocationHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UsageLocationHeading.Location = new System.Drawing.Point(25, 197);
            this.UsageLocationHeading.Name = "UsageLocationHeading";
            this.UsageLocationHeading.Size = new System.Drawing.Size(101, 15);
            this.UsageLocationHeading.TabIndex = 4;
            this.UsageLocationHeading.Text = "Usage Location *";
            // 
            // UserPrincipalName
            // 
            this.UserPrincipalName.Location = new System.Drawing.Point(166, 41);
            this.UserPrincipalName.Name = "UserPrincipalName";
            this.UserPrincipalName.Size = new System.Drawing.Size(200, 20);
            this.UserPrincipalName.TabIndex = 0;
            // 
            // FirstName
            // 
            this.FirstName.Location = new System.Drawing.Point(166, 67);
            this.FirstName.Name = "FirstName";
            this.FirstName.Size = new System.Drawing.Size(200, 20);
            this.FirstName.TabIndex = 1;
            // 
            // UsageLocation
            // 
            this.UsageLocation.Location = new System.Drawing.Point(166, 197);
            this.UsageLocation.Name = "UsageLocation";
            this.UsageLocation.Size = new System.Drawing.Size(200, 20);
            this.UsageLocation.TabIndex = 6;
            // 
            // SaveUser
            // 
            this.SaveUser.Location = new System.Drawing.Point(166, 224);
            this.SaveUser.Name = "SaveUser";
            this.SaveUser.Size = new System.Drawing.Size(95, 23);
            this.SaveUser.TabIndex = 8;
            this.SaveUser.Text = "Add User";
            this.SaveUser.UseVisualStyleBackColor = true;
            this.SaveUser.Click += new System.EventHandler(this.SaveUser_Click);
            // 
            // DepartmentHeading
            // 
            this.DepartmentHeading.AutoSize = true;
            this.DepartmentHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DepartmentHeading.Location = new System.Drawing.Point(25, 145);
            this.DepartmentHeading.Name = "DepartmentHeading";
            this.DepartmentHeading.Size = new System.Drawing.Size(72, 15);
            this.DepartmentHeading.TabIndex = 24;
            this.DepartmentHeading.Text = "Department";
            // 
            // Department
            // 
            this.Department.Location = new System.Drawing.Point(166, 145);
            this.Department.Name = "Department";
            this.Department.Size = new System.Drawing.Size(200, 20);
            this.Department.TabIndex = 4;
            // 
            // Country
            // 
            this.Country.Location = new System.Drawing.Point(166, 171);
            this.Country.Name = "Country";
            this.Country.Size = new System.Drawing.Size(200, 20);
            this.Country.TabIndex = 5;
            // 
            // CountryHeading
            // 
            this.CountryHeading.AutoSize = true;
            this.CountryHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CountryHeading.Location = new System.Drawing.Point(25, 171);
            this.CountryHeading.Name = "CountryHeading";
            this.CountryHeading.Size = new System.Drawing.Size(48, 15);
            this.CountryHeading.TabIndex = 28;
            this.CountryHeading.Text = "Country";
            // 
            // LastName
            // 
            this.LastName.Location = new System.Drawing.Point(166, 93);
            this.LastName.Name = "LastName";
            this.LastName.Size = new System.Drawing.Size(200, 20);
            this.LastName.TabIndex = 2;
            // 
            // LastNameHeading
            // 
            this.LastNameHeading.AutoSize = true;
            this.LastNameHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastNameHeading.Location = new System.Drawing.Point(25, 93);
            this.LastNameHeading.Name = "LastNameHeading";
            this.LastNameHeading.Size = new System.Drawing.Size(67, 15);
            this.LastNameHeading.TabIndex = 30;
            this.LastNameHeading.Text = "Last Name";
            // 
            // DisplayName
            // 
            this.DisplayName.Location = new System.Drawing.Point(166, 119);
            this.DisplayName.Name = "DisplayName";
            this.DisplayName.Size = new System.Drawing.Size(200, 20);
            this.DisplayName.TabIndex = 3;
            // 
            // DisplayNameHeading
            // 
            this.DisplayNameHeading.AutoSize = true;
            this.DisplayNameHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DisplayNameHeading.Location = new System.Drawing.Point(25, 119);
            this.DisplayNameHeading.Name = "DisplayNameHeading";
            this.DisplayNameHeading.Size = new System.Drawing.Size(92, 15);
            this.DisplayNameHeading.TabIndex = 32;
            this.DisplayNameHeading.Text = "Display Name *";
            // 
            // UserOperations
            // 
            this.AcceptButton = this.SaveUser;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(391, 259);
            this.Controls.Add(this.DisplayName);
            this.Controls.Add(this.DisplayNameHeading);
            this.Controls.Add(this.LastName);
            this.Controls.Add(this.LastNameHeading);
            this.Controls.Add(this.Country);
            this.Controls.Add(this.CountryHeading);
            this.Controls.Add(this.Department);
            this.Controls.Add(this.DepartmentHeading);
            this.Controls.Add(this.SaveUser);
            this.Controls.Add(this.UsageLocation);
            this.Controls.Add(this.FirstName);
            this.Controls.Add(this.UserPrincipalName);
            this.Controls.Add(this.UsageLocationHeading);
            this.Controls.Add(this.Header);
            this.Controls.Add(this.FirstNameHeading);
            this.Controls.Add(this.UserIDHeading);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserOperations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add User";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label UserIDHeading;
        private System.Windows.Forms.Label FirstNameHeading;
        private System.Windows.Forms.Label Header;
        private System.Windows.Forms.Label UsageLocationHeading;
        private System.Windows.Forms.TextBox UserPrincipalName;
        private System.Windows.Forms.TextBox FirstName;
        private System.Windows.Forms.TextBox UsageLocation;
        private System.Windows.Forms.Button SaveUser;
        private System.Windows.Forms.Label DepartmentHeading;
        private System.Windows.Forms.TextBox Department;
        private System.Windows.Forms.TextBox Country;
        private System.Windows.Forms.Label CountryHeading;
        private System.Windows.Forms.TextBox LastName;
        private System.Windows.Forms.Label LastNameHeading;
        private System.Windows.Forms.TextBox DisplayName;
        private System.Windows.Forms.Label DisplayNameHeading;
    }
}