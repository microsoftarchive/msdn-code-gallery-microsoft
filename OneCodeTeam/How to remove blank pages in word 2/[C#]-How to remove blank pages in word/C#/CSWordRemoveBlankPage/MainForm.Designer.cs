namespace CSWordRemoveBlankPage
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
            this.txbWordPath = new System.Windows.Forms.TextBox();
            this.btnOpenWord = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Word Document:";
            // 
            // txbWordPath
            // 
            this.txbWordPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbWordPath.Location = new System.Drawing.Point(107, 10);
            this.txbWordPath.Name = "txbWordPath";
            this.txbWordPath.Size = new System.Drawing.Size(219, 20);
            this.txbWordPath.TabIndex = 1;
            // 
            // btnOpenWord
            // 
            this.btnOpenWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenWord.Location = new System.Drawing.Point(332, 8);
            this.btnOpenWord.Name = "btnOpenWord";
            this.btnOpenWord.Size = new System.Drawing.Size(75, 23);
            this.btnOpenWord.TabIndex = 2;
            this.btnOpenWord.Text = "Select";
            this.btnOpenWord.UseVisualStyleBackColor = true;
            this.btnOpenWord.Click += new System.EventHandler(this.btnOpenWord_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(13, 48);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(130, 23);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "Remove Blank Page";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 125);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnOpenWord);
            this.Controls.Add(this.txbWordPath);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(400, 120);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RemoveBlankPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbWordPath;
        private System.Windows.Forms.Button btnOpenWord;
        private System.Windows.Forms.Button btnRemove;
    }
}

