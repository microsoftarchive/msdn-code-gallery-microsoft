namespace CSWinFormSearchAndHighlightText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblSearch = new System.Windows.Forms.Label();
            this.cboSearch = new System.Windows.Forms.ComboBox();
            this.btnSearchAndHighlight = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panelColor = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkMatchWholeWord = new System.Windows.Forms.CheckBox();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.rtbSource = new CSWinFormSearchAndHighlightText.CustomRichTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSearch
            // 
            resources.ApplyResources(this.lblSearch, "lblSearch");
            this.lblSearch.Name = "lblSearch";
            // 
            // cboSearch
            // 
            resources.ApplyResources(this.cboSearch, "cboSearch");
            this.cboSearch.FormattingEnabled = true;
            this.cboSearch.Name = "cboSearch";
            this.cboSearch.TextChanged += new System.EventHandler(this.cboSearch_TextChanged);
            this.cboSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboSearch_KeyPress);
            // 
            // btnSearchAndHighlight
            // 
            resources.ApplyResources(this.btnSearchAndHighlight, "btnSearchAndHighlight");
            this.btnSearchAndHighlight.Name = "btnSearchAndHighlight";
            this.btnSearchAndHighlight.UseVisualStyleBackColor = true;
            this.btnSearchAndHighlight.Click += new System.EventHandler(this.btnSearchAndHighlight_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panelColor
            // 
            resources.ApplyResources(this.panelColor, "panelColor");
            this.panelColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.panelColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelColor.Cursor = System.Windows.Forms.Cursors.Default;
            this.panelColor.Name = "panelColor";
            this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkMatchWholeWord);
            this.groupBox1.Controls.Add(this.chkMatchCase);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // chkMatchWholeWord
            // 
            resources.ApplyResources(this.chkMatchWholeWord, "chkMatchWholeWord");
            this.chkMatchWholeWord.Name = "chkMatchWholeWord";
            this.chkMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase
            // 
            resources.ApplyResources(this.chkMatchCase, "chkMatchCase");
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // rtbSource
            // 
            resources.ApplyResources(this.rtbSource, "rtbSource");
            this.rtbSource.Name = "rtbSource";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtbSource);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelColor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearchAndHighlight);
            this.Controls.Add(this.cboSearch);
            this.Controls.Add(this.lblSearch);
            this.Name = "MainForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.ComboBox cboSearch;
        private System.Windows.Forms.Button btnSearchAndHighlight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelColor;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkMatchWholeWord;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private CustomRichTextBox rtbSource;
    }
}

