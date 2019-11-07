//Copyright (C) Microsoft Corporation.  All rights reserved.

namespace GuiHost
{
    partial class Form1
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
            this.showVisualizerButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // showVisualizerButton
            // 
            this.showVisualizerButton.Location = new System.Drawing.Point(12, 28);
            this.showVisualizerButton.Name = "showVisualizerButton";
            this.showVisualizerButton.Size = new System.Drawing.Size(177, 23);
            this.showVisualizerButton.TabIndex = 0;
            this.showVisualizerButton.Text = "Show Visualizer";
            this.showVisualizerButton.UseVisualStyleBackColor = true;
            this.showVisualizerButton.Click += new System.EventHandler(this.showVisualizerButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 79);
            this.Controls.Add(this.showVisualizerButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button showVisualizerButton;
    }
}

