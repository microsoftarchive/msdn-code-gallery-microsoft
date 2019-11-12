namespace CSExtractAudioFile
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
            this.lbStartpoint = new System.Windows.Forms.Label();
            this.tbStartpoint = new System.Windows.Forms.TextBox();
            this.lbBeginMillisecs = new System.Windows.Forms.Label();
            this.btnSetBeginEndPoints = new System.Windows.Forms.Button();
            this.lbEndpoint = new System.Windows.Forms.Label();
            this.tbEndpoint = new System.Windows.Forms.TextBox();
            this.lbEndMillisecs = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.cmbOutputAudioType = new System.Windows.Forms.ComboBox();
            this.lbOutputFileType = new System.Windows.Forms.Label();
            this.lbOutputDirectory = new System.Windows.Forms.Label();
            this.tbOutputDirectory = new System.Windows.Forms.TextBox();
            this.btnChooseOutputDirectory = new System.Windows.Forms.Button();
            this.player = new AxWMPLib.AxWindowsMediaPlayer();
            this.btnChooseSourceFile = new System.Windows.Forms.Button();
            this.outputFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openAudioFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.player)).BeginInit();
            this.SuspendLayout();
            // 
            // lbStartpoint
            // 
            this.lbStartpoint.AutoSize = true;
            this.lbStartpoint.Location = new System.Drawing.Point(8, 128);
            this.lbStartpoint.Name = "lbStartpoint";
            this.lbStartpoint.Size = new System.Drawing.Size(55, 13);
            this.lbStartpoint.TabIndex = 10;
            this.lbStartpoint.Text = "Startpoint:";
            // 
            // tbStartpoint
            // 
            this.tbStartpoint.Enabled = false;
            this.tbStartpoint.Location = new System.Drawing.Point(68, 125);
            this.tbStartpoint.Name = "tbStartpoint";
            this.tbStartpoint.ReadOnly = true;
            this.tbStartpoint.Size = new System.Drawing.Size(100, 20);
            this.tbStartpoint.TabIndex = 11;
            // 
            // lbBeginMillisecs
            // 
            this.lbBeginMillisecs.AutoSize = true;
            this.lbBeginMillisecs.Location = new System.Drawing.Point(174, 128);
            this.lbBeginMillisecs.Name = "lbBeginMillisecs";
            this.lbBeginMillisecs.Size = new System.Drawing.Size(63, 13);
            this.lbBeginMillisecs.TabIndex = 12;
            this.lbBeginMillisecs.Text = "milliseconds";
            // 
            // btnSetBeginEndPoints
            // 
            this.btnSetBeginEndPoints.Location = new System.Drawing.Point(12, 94);
            this.btnSetBeginEndPoints.Name = "btnSetBeginEndPoints";
            this.btnSetBeginEndPoints.Size = new System.Drawing.Size(129, 23);
            this.btnSetBeginEndPoints.TabIndex = 13;
            this.btnSetBeginEndPoints.Tag = "SetStartPoint";
            this.btnSetBeginEndPoints.Text = "Set Start Point";
            this.btnSetBeginEndPoints.UseVisualStyleBackColor = true;
            this.btnSetBeginEndPoints.Click += new System.EventHandler(this.btnSetBeginEndPoints_Click);
            // 
            // lbEndpoint
            // 
            this.lbEndpoint.AutoSize = true;
            this.lbEndpoint.Location = new System.Drawing.Point(8, 155);
            this.lbEndpoint.Name = "lbEndpoint";
            this.lbEndpoint.Size = new System.Drawing.Size(52, 13);
            this.lbEndpoint.TabIndex = 14;
            this.lbEndpoint.Text = "Endpoint:";
            // 
            // tbEndpoint
            // 
            this.tbEndpoint.Enabled = false;
            this.tbEndpoint.Location = new System.Drawing.Point(68, 152);
            this.tbEndpoint.Name = "tbEndpoint";
            this.tbEndpoint.ReadOnly = true;
            this.tbEndpoint.Size = new System.Drawing.Size(100, 20);
            this.tbEndpoint.TabIndex = 15;
            // 
            // lbEndMillisecs
            // 
            this.lbEndMillisecs.AutoSize = true;
            this.lbEndMillisecs.Location = new System.Drawing.Point(174, 155);
            this.lbEndMillisecs.Name = "lbEndMillisecs";
            this.lbEndMillisecs.Size = new System.Drawing.Size(63, 13);
            this.lbEndMillisecs.TabIndex = 16;
            this.lbEndMillisecs.Text = "milliseconds";
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(12, 244);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(75, 23);
            this.btnExtract.TabIndex = 17;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // cmbOutputAudioType
            // 
            this.cmbOutputAudioType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutputAudioType.FormattingEnabled = true;
            this.cmbOutputAudioType.Location = new System.Drawing.Point(142, 184);
            this.cmbOutputAudioType.Name = "cmbOutputAudioType";
            this.cmbOutputAudioType.Size = new System.Drawing.Size(121, 21);
            this.cmbOutputAudioType.TabIndex = 22;
            // 
            // lbOutputFileType
            // 
            this.lbOutputFileType.AutoSize = true;
            this.lbOutputFileType.Location = new System.Drawing.Point(9, 187);
            this.lbOutputFileType.Name = "lbOutputFileType";
            this.lbOutputFileType.Size = new System.Drawing.Size(88, 13);
            this.lbOutputFileType.TabIndex = 23;
            this.lbOutputFileType.Text = "Output File Type:";
            // 
            // lbOutputDirectory
            // 
            this.lbOutputDirectory.AutoSize = true;
            this.lbOutputDirectory.Location = new System.Drawing.Point(9, 213);
            this.lbOutputDirectory.Name = "lbOutputDirectory";
            this.lbOutputDirectory.Size = new System.Drawing.Size(87, 13);
            this.lbOutputDirectory.TabIndex = 24;
            this.lbOutputDirectory.Text = "Output Directory:";
            // 
            // tbOutputDirectory
            // 
            this.tbOutputDirectory.Location = new System.Drawing.Point(142, 213);
            this.tbOutputDirectory.Name = "tbOutputDirectory";
            this.tbOutputDirectory.ReadOnly = true;
            this.tbOutputDirectory.Size = new System.Drawing.Size(340, 20);
            this.tbOutputDirectory.TabIndex = 25;
            // 
            // btnChooseOutputDirectory
            // 
            this.btnChooseOutputDirectory.Location = new System.Drawing.Point(488, 211);
            this.btnChooseOutputDirectory.Name = "btnChooseOutputDirectory";
            this.btnChooseOutputDirectory.Size = new System.Drawing.Size(77, 23);
            this.btnChooseOutputDirectory.TabIndex = 26;
            this.btnChooseOutputDirectory.Text = "Browse...";
            this.btnChooseOutputDirectory.UseVisualStyleBackColor = true;
            this.btnChooseOutputDirectory.Click += new System.EventHandler(this.btnChooseOutputDirectory_Click);
            // 
            // player
            // 
            this.player.Enabled = true;
            this.player.Location = new System.Drawing.Point(12, 41);
            this.player.Name = "player";
            this.player.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("player.OcxState")));
            this.player.Size = new System.Drawing.Size(612, 45);
            this.player.TabIndex = 28;
            // 
            // btnChooseSourceFile
            // 
            this.btnChooseSourceFile.Location = new System.Drawing.Point(11, 12);
            this.btnChooseSourceFile.Name = "btnChooseSourceFile";
            this.btnChooseSourceFile.Size = new System.Drawing.Size(613, 23);
            this.btnChooseSourceFile.TabIndex = 27;
            this.btnChooseSourceFile.Text = "Choose Audio File...";
            this.btnChooseSourceFile.UseVisualStyleBackColor = true;
            this.btnChooseSourceFile.Click += new System.EventHandler(this.btnChooseSourceFile_Click);
            // 
            // openAudioFileDialog
            // 
            this.openAudioFileDialog.Filter = "Audio Files (*.wav;*.mp3;*.mp4)|*.wav;*.mp3;*.mp4";
            // 
            // CSExtractAudioFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 279);
            this.Controls.Add(this.player);
            this.Controls.Add(this.btnChooseSourceFile);
            this.Controls.Add(this.btnChooseOutputDirectory);
            this.Controls.Add(this.tbOutputDirectory);
            this.Controls.Add(this.lbOutputDirectory);
            this.Controls.Add(this.lbOutputFileType);
            this.Controls.Add(this.cmbOutputAudioType);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.lbEndMillisecs);
            this.Controls.Add(this.tbEndpoint);
            this.Controls.Add(this.lbEndpoint);
            this.Controls.Add(this.btnSetBeginEndPoints);
            this.Controls.Add(this.lbBeginMillisecs);
            this.Controls.Add(this.tbStartpoint);
            this.Controls.Add(this.lbStartpoint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CSExtractAudioFile";
            this.Text = "CSExtractAudioFile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.player)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbStartpoint;
        private System.Windows.Forms.TextBox tbStartpoint;
        private System.Windows.Forms.Label lbBeginMillisecs;
        private System.Windows.Forms.Button btnSetBeginEndPoints;
        private System.Windows.Forms.Label lbEndpoint;
        private System.Windows.Forms.TextBox tbEndpoint;
        private System.Windows.Forms.Label lbEndMillisecs;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.ComboBox cmbOutputAudioType;
        private System.Windows.Forms.Label lbOutputFileType;
        private System.Windows.Forms.Label lbOutputDirectory;
        private System.Windows.Forms.TextBox tbOutputDirectory;
        private System.Windows.Forms.Button btnChooseOutputDirectory;
        private AxWMPLib.AxWindowsMediaPlayer player;
        private System.Windows.Forms.Button btnChooseSourceFile;
        private System.Windows.Forms.FolderBrowserDialog outputFolderBrowserDialog;
        private System.Windows.Forms.OpenFileDialog openAudioFileDialog;
    }
}

