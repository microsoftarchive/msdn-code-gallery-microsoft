namespace DataContentControls
{
    [System.ComponentModel.ToolboxItemAttribute(false)]
    partial class ActionsPaneControl1
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Button4 = new System.Windows.Forms.Button();
            this.Button3 = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Button4
            // 
            this.Button4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Button4.Location = new System.Drawing.Point(51, 404);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(194, 38);
            this.Button4.TabIndex = 17;
            this.Button4.Text = "Submit Change to Title";
            this.Button4.UseVisualStyleBackColor = false;
            this.Button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // Button3
            // 
            this.Button3.Location = new System.Drawing.Point(51, 329);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(194, 38);
            this.Button3.TabIndex = 16;
            this.Button3.Text = "Search by ID";
            this.Button3.UseVisualStyleBackColor = true;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(48, 284);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(52, 13);
            this.Label1.TabIndex = 15;
            this.Label1.Text = "Enter ID: ";
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(121, 267);
            this.TextBox1.Multiline = true;
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(124, 34);
            this.TextBox1.TabIndex = 14;
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(74, 202);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(128, 38);
            this.Button2.TabIndex = 13;
            this.Button2.Text = "Next";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(74, 129);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(128, 38);
            this.Button1.TabIndex = 12;
            this.Button1.Text = "Previous";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // ActionsPaneControl1
            // 
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.Controls.Add(this.Button4);
            this.Controls.Add(this.Button3);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.TextBox1);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Button1);
            this.Name = "ActionsPaneControl1";
            this.Size = new System.Drawing.Size(292, 558);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button Button4;
        internal System.Windows.Forms.Button Button3;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox TextBox1;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Button Button1;
    }
}
