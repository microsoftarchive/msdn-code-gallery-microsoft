namespace CSWinFormLayeredWindow
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
            this.btnShowAlphaWindow = new System.Windows.Forms.Button();
            this.trackBarOpacity = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lbOpacity = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOpacity)).BeginInit();
            this.SuspendLayout();
            // 
            // btnShowAlphaWindow
            // 
            this.btnShowAlphaWindow.Location = new System.Drawing.Point(15, 14);
            this.btnShowAlphaWindow.Name = "btnShowAlphaWindow";
            this.btnShowAlphaWindow.Size = new System.Drawing.Size(157, 25);
            this.btnShowAlphaWindow.TabIndex = 0;
            this.btnShowAlphaWindow.Text = "Show Alpha Window";
            this.btnShowAlphaWindow.UseVisualStyleBackColor = true;
            this.btnShowAlphaWindow.Click += new System.EventHandler(this.btnShowAlphaWindow_Click);
            // 
            // trackBarOpacity
            // 
            this.trackBarOpacity.LargeChange = 15;
            this.trackBarOpacity.Location = new System.Drawing.Point(9, 72);
            this.trackBarOpacity.Maximum = 255;
            this.trackBarOpacity.Name = "trackBarOpacity";
            this.trackBarOpacity.Size = new System.Drawing.Size(171, 45);
            this.trackBarOpacity.TabIndex = 1;
            this.trackBarOpacity.TickFrequency = 10;
            this.trackBarOpacity.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarOpacity.Value = 255;
            this.trackBarOpacity.ValueChanged += new System.EventHandler(this.trackBarOpacity_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Opacity level [0..255]:";
            // 
            // lbOpacity
            // 
            this.lbOpacity.AutoSize = true;
            this.lbOpacity.Location = new System.Drawing.Point(132, 53);
            this.lbOpacity.Name = "lbOpacity";
            this.lbOpacity.Size = new System.Drawing.Size(25, 13);
            this.lbOpacity.TabIndex = 3;
            this.lbOpacity.Text = "255";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 115);
            this.Controls.Add(this.lbOpacity);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBarOpacity);
            this.Controls.Add(this.btnShowAlphaWindow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "CSWinFormLayeredWindow";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOpacity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnShowAlphaWindow;
        private System.Windows.Forms.TrackBar trackBarOpacity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbOpacity;
    }
}

