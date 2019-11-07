namespace CSRichTextBoxSyntaxHighlighting 
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
            CSRichTextBoxSyntaxHighlighting .XMLViewerSettings xmlViewerSettings1 = new CSRichTextBoxSyntaxHighlighting .XMLViewerSettings();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.lbNote = new System.Windows.Forms.Label();
            this.btnProcess = new System.Windows.Forms.Button();
            this.viewer = new CSRichTextBoxSyntaxHighlighting .XMLViewer();
            this.pnlMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMenu
            // 
            this.pnlMenu.Controls.Add(this.lbNote);
            this.pnlMenu.Controls.Add(this.btnProcess);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(775, 79);
            this.pnlMenu.TabIndex = 1;
            // 
            // lbNote
            // 
            this.lbNote.AutoSize = true;
            this.lbNote.Location = new System.Drawing.Point(12, 9);
            this.lbNote.Name = "lbNote";
            this.lbNote.Size = new System.Drawing.Size(389, 65);
            this.lbNote.TabIndex = 2;
            this.lbNote.Text = resources.GetString("lbNote.Text");
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(420, 38);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 1;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // viewer
            // 
            this.viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewer.Location = new System.Drawing.Point(0, 79);
            this.viewer.Name = "viewer";
            xmlViewerSettings1.AttributeKey = System.Drawing.Color.Red;
            xmlViewerSettings1.AttributeValue = System.Drawing.Color.Blue;
            xmlViewerSettings1.Element = System.Drawing.Color.DarkRed;
            xmlViewerSettings1.Tag = System.Drawing.Color.Blue;
            xmlViewerSettings1.Value = System.Drawing.Color.Black;
            this.viewer.Settings = xmlViewerSettings1;
            this.viewer.Size = new System.Drawing.Size(775, 381);
            this.viewer.TabIndex = 0;
            this.viewer.Text = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><html><head><title>My home page</title></h" +
                "ead><body bgcolor=\"000000\" text=\"ff0000\">Hello World!</body></html>\n";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 460);
            this.Controls.Add(this.viewer);
            this.Controls.Add(this.pnlMenu);
            this.Name = "MainForm";
            this.Text = "SimpleXMLViewer";
            this.pnlMenu.ResumeLayout(false);
            this.pnlMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private XMLViewer viewer;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Label lbNote;
    }
}

