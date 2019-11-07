namespace CSOfficeRibbonAccessibility
{
    partial class RibbonInfoForm
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
            this.lbTabs = new System.Windows.Forms.ListBox();
            this.lbGroups = new System.Windows.Forms.ListBox();
            this.lbControls = new System.Windows.Forms.ListBox();
            this.btnListChildGroups = new System.Windows.Forms.Button();
            this.btnListChildControls = new System.Windows.Forms.Button();
            this.btnExecuteControl = new System.Windows.Forms.Button();
            this.labelForm = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbTabs
            // 
            this.lbTabs.FormattingEnabled = true;
            this.lbTabs.Location = new System.Drawing.Point(21, 77);
            this.lbTabs.Name = "lbTabs";
            this.lbTabs.Size = new System.Drawing.Size(133, 212);
            this.lbTabs.TabIndex = 1;
            this.lbTabs.SelectedValueChanged += new System.EventHandler(this.lbTabs_SelectedValueChanged);
            // 
            // lbGroups
            // 
            this.lbGroups.FormattingEnabled = true;
            this.lbGroups.Location = new System.Drawing.Point(211, 77);
            this.lbGroups.Name = "lbGroups";
            this.lbGroups.Size = new System.Drawing.Size(133, 212);
            this.lbGroups.TabIndex = 2;
            // 
            // lbControls
            // 
            this.lbControls.FormattingEnabled = true;
            this.lbControls.Location = new System.Drawing.Point(401, 77);
            this.lbControls.Name = "lbControls";
            this.lbControls.Size = new System.Drawing.Size(133, 212);
            this.lbControls.TabIndex = 3;
            // 
            // btnListChildGroups
            // 
            this.btnListChildGroups.Location = new System.Drawing.Point(21, 300);
            this.btnListChildGroups.Name = "btnListChildGroups";
            this.btnListChildGroups.Size = new System.Drawing.Size(133, 23);
            this.btnListChildGroups.TabIndex = 4;
            this.btnListChildGroups.Text = "List child groups";
            this.btnListChildGroups.UseVisualStyleBackColor = true;
            this.btnListChildGroups.Click += new System.EventHandler(this.btnListChildGroups_Click);
            // 
            // btnListChildControls
            // 
            this.btnListChildControls.Location = new System.Drawing.Point(211, 300);
            this.btnListChildControls.Name = "btnListChildControls";
            this.btnListChildControls.Size = new System.Drawing.Size(133, 23);
            this.btnListChildControls.TabIndex = 5;
            this.btnListChildControls.Text = "List child controls";
            this.btnListChildControls.UseVisualStyleBackColor = true;
            this.btnListChildControls.Click += new System.EventHandler(this.btnListChildControls_Click);
            // 
            // btnExecuteControl
            // 
            this.btnExecuteControl.Location = new System.Drawing.Point(401, 300);
            this.btnExecuteControl.Name = "btnExecuteControl";
            this.btnExecuteControl.Size = new System.Drawing.Size(133, 23);
            this.btnExecuteControl.TabIndex = 6;
            this.btnExecuteControl.Text = "Execute selected control";
            this.btnExecuteControl.UseVisualStyleBackColor = true;
            this.btnExecuteControl.Click += new System.EventHandler(this.btnExecuteControl_Click);
            // 
            // labelForm
            // 
            this.labelForm.AutoSize = true;
            this.labelForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelForm.Location = new System.Drawing.Point(13, 17);
            this.labelForm.Name = "labelForm";
            this.labelForm.Size = new System.Drawing.Size(460, 20);
            this.labelForm.TabIndex = 9;
            this.labelForm.Text = "Ribbon Information via Microsoft Active Accessibility API";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(399, 59);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(48, 13);
            this.Label3.TabIndex = 21;
            this.Label3.Text = "Controls:";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(210, 59);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(44, 13);
            this.Label2.TabIndex = 20;
            this.Label2.Text = "Groups:";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(18, 59);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(34, 13);
            this.Label1.TabIndex = 19;
            this.Label1.Text = "Tabs:";
            // 
            // RibbonInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 347);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.labelForm);
            this.Controls.Add(this.btnExecuteControl);
            this.Controls.Add(this.btnListChildControls);
            this.Controls.Add(this.btnListChildGroups);
            this.Controls.Add(this.lbControls);
            this.Controls.Add(this.lbGroups);
            this.Controls.Add(this.lbTabs);
            this.Name = "RibbonInfoForm";
            this.Text = "Ribbon Information";
            this.Load += new System.EventHandler(this.RibbonInfoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbTabs;
        private System.Windows.Forms.ListBox lbGroups;
        private System.Windows.Forms.ListBox lbControls;
        private System.Windows.Forms.Button btnListChildGroups;
        private System.Windows.Forms.Button btnListChildControls;
        private System.Windows.Forms.Button btnExecuteControl;
        private System.Windows.Forms.Label labelForm;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label1;
    }
}