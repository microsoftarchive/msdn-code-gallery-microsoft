namespace O365_SingleSignOn
{
    partial class DomainOperations
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
            this.AddNewDomain = new System.Windows.Forms.Button();
            this.FullDomainName = new System.Windows.Forms.TextBox();
            this.Header = new System.Windows.Forms.Label();
            this.DomainNameHeading = new System.Windows.Forms.Label();
            this.DomainTypeHeader = new System.Windows.Forms.Label();
            this.DomainType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // AddNewDomain
            // 
            this.AddNewDomain.Location = new System.Drawing.Point(121, 105);
            this.AddNewDomain.Name = "AddNewDomain";
            this.AddNewDomain.Size = new System.Drawing.Size(95, 23);
            this.AddNewDomain.TabIndex = 3;
            this.AddNewDomain.Text = "Add Domain";
            this.AddNewDomain.UseVisualStyleBackColor = true;
            this.AddNewDomain.Click += new System.EventHandler(this.AddDomain_Click);
            // 
            // FullDomainName
            // 
            this.FullDomainName.Location = new System.Drawing.Point(121, 46);
            this.FullDomainName.Name = "FullDomainName";
            this.FullDomainName.Size = new System.Drawing.Size(200, 20);
            this.FullDomainName.TabIndex = 1;
            // 
            // Header
            // 
            this.Header.AutoSize = true;
            this.Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Header.Location = new System.Drawing.Point(24, 12);
            this.Header.Name = "Header";
            this.Header.Size = new System.Drawing.Size(312, 17);
            this.Header.TabIndex = 38;
            this.Header.Text = "Please enter below details to create new domain";
            // 
            // DomainNameHeading
            // 
            this.DomainNameHeading.AutoSize = true;
            this.DomainNameHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DomainNameHeading.Location = new System.Drawing.Point(24, 46);
            this.DomainNameHeading.Name = "DomainNameHeading";
            this.DomainNameHeading.Size = new System.Drawing.Size(93, 15);
            this.DomainNameHeading.TabIndex = 34;
            this.DomainNameHeading.Text = "Domain Name*";
            // 
            // DomainTypeHeader
            // 
            this.DomainTypeHeader.AutoSize = true;
            this.DomainTypeHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DomainTypeHeader.Location = new System.Drawing.Point(24, 72);
            this.DomainTypeHeader.Name = "DomainTypeHeader";
            this.DomainTypeHeader.Size = new System.Drawing.Size(85, 15);
            this.DomainTypeHeader.TabIndex = 45;
            this.DomainTypeHeader.Text = "Domain Type*";
            // 
            // DomainType
            // 
            this.DomainType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DomainType.FormattingEnabled = true;
            this.DomainType.Items.AddRange(new object[] {
            "Standard(Managed) Domain",
            "Federated(SSO) Domain"});
            this.DomainType.Location = new System.Drawing.Point(121, 75);
            this.DomainType.Name = "DomainType";
            this.DomainType.Size = new System.Drawing.Size(200, 21);
            this.DomainType.TabIndex = 2;
            // 
            // DomainOperations
            // 
            this.AcceptButton = this.AddNewDomain;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 145);
            this.Controls.Add(this.DomainType);
            this.Controls.Add(this.DomainTypeHeader);
            this.Controls.Add(this.AddNewDomain);
            this.Controls.Add(this.FullDomainName);
            this.Controls.Add(this.Header);
            this.Controls.Add(this.DomainNameHeading);
            this.Name = "DomainOperations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Domain";
            this.Load += new System.EventHandler(this.DomainOperations_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AddNewDomain;
        private System.Windows.Forms.TextBox FullDomainName;
        private System.Windows.Forms.Label Header;
        private System.Windows.Forms.Label DomainNameHeading;
        private System.Windows.Forms.Label DomainTypeHeader;
        private System.Windows.Forms.ComboBox DomainType;
    }
}