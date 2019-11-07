namespace CSWinFormExAeroToClient
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
            if (disposing && (demoForm != null))
            {
                demoForm.Dispose();
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
            this.chkExtendFrame = new System.Windows.Forms.CheckBox();
            this.tbLeft = new System.Windows.Forms.TextBox();
            this.tbRight = new System.Windows.Forms.TextBox();
            this.tbTop = new System.Windows.Forms.TextBox();
            this.tbBottom = new System.Windows.Forms.TextBox();
            this.lbLeft = new System.Windows.Forms.Label();
            this.lbTop = new System.Windows.Forms.Label();
            this.lbRight = new System.Windows.Forms.Label();
            this.lbBottom = new System.Windows.Forms.Label();
            this.chkBlurBehindWindow = new System.Windows.Forms.CheckBox();
            this.tbX = new System.Windows.Forms.TextBox();
            this.tbWidth = new System.Windows.Forms.TextBox();
            this.tbY = new System.Windows.Forms.TextBox();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.lbX = new System.Windows.Forms.Label();
            this.lbWidth = new System.Windows.Forms.Label();
            this.lbY = new System.Windows.Forms.Label();
            this.lbHeight = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.lbAeroGlassStyleSupported = new System.Windows.Forms.Label();
            this.chkEntendToEntireClientArea = new System.Windows.Forms.CheckBox();
            this.chkEnableEntireFormBlurEffect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkExtendFrame
            // 
            this.chkExtendFrame.AutoSize = true;
            this.chkExtendFrame.Location = new System.Drawing.Point(11, 47);
            this.chkExtendFrame.Name = "chkExtendFrame";
            this.chkExtendFrame.Size = new System.Drawing.Size(91, 17);
            this.chkExtendFrame.TabIndex = 0;
            this.chkExtendFrame.Text = "Extend Frame";
            this.chkExtendFrame.UseVisualStyleBackColor = true;
            // 
            // tbLeft
            // 
            this.tbLeft.Location = new System.Drawing.Point(52, 70);
            this.tbLeft.Name = "tbLeft";
            this.tbLeft.Size = new System.Drawing.Size(100, 20);
            this.tbLeft.TabIndex = 1;
            this.tbLeft.Text = "30";
            // 
            // tbRight
            // 
            this.tbRight.Location = new System.Drawing.Point(214, 70);
            this.tbRight.Name = "tbRight";
            this.tbRight.Size = new System.Drawing.Size(100, 20);
            this.tbRight.TabIndex = 1;
            this.tbRight.Text = "60";
            // 
            // tbTop
            // 
            this.tbTop.Location = new System.Drawing.Point(52, 96);
            this.tbTop.Name = "tbTop";
            this.tbTop.Size = new System.Drawing.Size(100, 20);
            this.tbTop.TabIndex = 1;
            this.tbTop.Text = "40";
            // 
            // tbBottom
            // 
            this.tbBottom.Location = new System.Drawing.Point(214, 96);
            this.tbBottom.Name = "tbBottom";
            this.tbBottom.Size = new System.Drawing.Size(100, 20);
            this.tbBottom.TabIndex = 1;
            this.tbBottom.Text = "80";
            // 
            // lbLeft
            // 
            this.lbLeft.AutoSize = true;
            this.lbLeft.Location = new System.Drawing.Point(11, 70);
            this.lbLeft.Name = "lbLeft";
            this.lbLeft.Size = new System.Drawing.Size(25, 13);
            this.lbLeft.TabIndex = 2;
            this.lbLeft.Text = "Left";
            // 
            // lbTop
            // 
            this.lbTop.AutoSize = true;
            this.lbTop.Location = new System.Drawing.Point(10, 96);
            this.lbTop.Name = "lbTop";
            this.lbTop.Size = new System.Drawing.Size(26, 13);
            this.lbTop.TabIndex = 2;
            this.lbTop.Text = "Top";
            // 
            // lbRight
            // 
            this.lbRight.AutoSize = true;
            this.lbRight.Location = new System.Drawing.Point(174, 73);
            this.lbRight.Name = "lbRight";
            this.lbRight.Size = new System.Drawing.Size(32, 13);
            this.lbRight.TabIndex = 2;
            this.lbRight.Text = "Right";
            // 
            // lbBottom
            // 
            this.lbBottom.AutoSize = true;
            this.lbBottom.Location = new System.Drawing.Point(173, 99);
            this.lbBottom.Name = "lbBottom";
            this.lbBottom.Size = new System.Drawing.Size(40, 13);
            this.lbBottom.TabIndex = 2;
            this.lbBottom.Text = "Bottom";
            // 
            // chkBlurBehindWindow
            // 
            this.chkBlurBehindWindow.AutoSize = true;
            this.chkBlurBehindWindow.Location = new System.Drawing.Point(11, 143);
            this.chkBlurBehindWindow.Name = "chkBlurBehindWindow";
            this.chkBlurBehindWindow.Size = new System.Drawing.Size(233, 17);
            this.chkBlurBehindWindow.TabIndex = 0;
            this.chkBlurBehindWindow.Text = "Enable Blur Behind Window (Set the region)";
            this.chkBlurBehindWindow.UseVisualStyleBackColor = true;
            // 
            // tbX
            // 
            this.tbX.Location = new System.Drawing.Point(77, 166);
            this.tbX.Name = "tbX";
            this.tbX.Size = new System.Drawing.Size(75, 20);
            this.tbX.TabIndex = 1;
            this.tbX.Text = "100";
            // 
            // tbWidth
            // 
            this.tbWidth.Location = new System.Drawing.Point(77, 192);
            this.tbWidth.Name = "tbWidth";
            this.tbWidth.Size = new System.Drawing.Size(75, 20);
            this.tbWidth.TabIndex = 1;
            this.tbWidth.Text = "200";
            // 
            // tbY
            // 
            this.tbY.Location = new System.Drawing.Point(236, 166);
            this.tbY.Name = "tbY";
            this.tbY.Size = new System.Drawing.Size(78, 20);
            this.tbY.TabIndex = 1;
            this.tbY.Text = "100";
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(236, 192);
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(78, 20);
            this.tbHeight.TabIndex = 1;
            this.tbHeight.Text = "120";
            // 
            // lbX
            // 
            this.lbX.AutoSize = true;
            this.lbX.Location = new System.Drawing.Point(11, 166);
            this.lbX.Name = "lbX";
            this.lbX.Size = new System.Drawing.Size(41, 13);
            this.lbX.TabIndex = 2;
            this.lbX.Text = "Point.X";
            // 
            // lbWidth
            // 
            this.lbWidth.AutoSize = true;
            this.lbWidth.Location = new System.Drawing.Point(10, 192);
            this.lbWidth.Name = "lbWidth";
            this.lbWidth.Size = new System.Drawing.Size(58, 13);
            this.lbWidth.TabIndex = 2;
            this.lbWidth.Text = "Size.Width";
            // 
            // lbY
            // 
            this.lbY.AutoSize = true;
            this.lbY.Location = new System.Drawing.Point(174, 169);
            this.lbY.Name = "lbY";
            this.lbY.Size = new System.Drawing.Size(41, 13);
            this.lbY.TabIndex = 2;
            this.lbY.Text = "Point.Y";
            // 
            // lbHeight
            // 
            this.lbHeight.AutoSize = true;
            this.lbHeight.Location = new System.Drawing.Point(173, 195);
            this.lbHeight.Name = "lbHeight";
            this.lbHeight.Size = new System.Drawing.Size(61, 13);
            this.lbHeight.TabIndex = 2;
            this.lbHeight.Text = "Size.Height";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(236, 245);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lbAeroGlassStyleSupported
            // 
            this.lbAeroGlassStyleSupported.AutoSize = true;
            this.lbAeroGlassStyleSupported.Location = new System.Drawing.Point(11, 12);
            this.lbAeroGlassStyleSupported.Name = "lbAeroGlassStyleSupported";
            this.lbAeroGlassStyleSupported.Size = new System.Drawing.Size(136, 13);
            this.lbAeroGlassStyleSupported.TabIndex = 0;
            this.lbAeroGlassStyleSupported.Text = "Aero Glass Style Supported";
            // 
            // chkEntendToEntireClientArea
            // 
            this.chkEntendToEntireClientArea.AutoSize = true;
            this.chkEntendToEntireClientArea.Location = new System.Drawing.Point(52, 122);
            this.chkEntendToEntireClientArea.Name = "chkEntendToEntireClientArea";
            this.chkEntendToEntireClientArea.Size = new System.Drawing.Size(202, 17);
            this.chkEntendToEntireClientArea.TabIndex = 0;
            this.chkEntendToEntireClientArea.Text = "Extend Frame to the entire client area";
            this.chkEntendToEntireClientArea.UseVisualStyleBackColor = true;
            // 
            // chkEnableEntireFormBlurEffect
            // 
            this.chkEnableEntireFormBlurEffect.AutoSize = true;
            this.chkEnableEntireFormBlurEffect.Location = new System.Drawing.Point(48, 218);
            this.chkEnableEntireFormBlurEffect.Name = "chkEnableEntireFormBlurEffect";
            this.chkEnableEntireFormBlurEffect.Size = new System.Drawing.Size(243, 17);
            this.chkEnableEntireFormBlurEffect.TabIndex = 0;
            this.chkEnableEntireFormBlurEffect.Text = "Enable Blur Behind Window on the entire form";
            this.chkEnableEntireFormBlurEffect.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 280);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lbHeight);
            this.Controls.Add(this.lbBottom);
            this.Controls.Add(this.lbY);
            this.Controls.Add(this.lbRight);
            this.Controls.Add(this.lbWidth);
            this.Controls.Add(this.lbX);
            this.Controls.Add(this.lbTop);
            this.Controls.Add(this.tbHeight);
            this.Controls.Add(this.lbLeft);
            this.Controls.Add(this.tbY);
            this.Controls.Add(this.tbBottom);
            this.Controls.Add(this.tbWidth);
            this.Controls.Add(this.tbRight);
            this.Controls.Add(this.tbX);
            this.Controls.Add(this.tbTop);
            this.Controls.Add(this.chkEnableEntireFormBlurEffect);
            this.Controls.Add(this.chkBlurBehindWindow);
            this.Controls.Add(this.tbLeft);
            this.Controls.Add(this.lbAeroGlassStyleSupported);
            this.Controls.Add(this.chkEntendToEntireClientArea);
            this.Controls.Add(this.chkExtendFrame);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "CSWinFormExAeroToClient";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkExtendFrame;
        private System.Windows.Forms.TextBox tbLeft;
        private System.Windows.Forms.TextBox tbRight;
        private System.Windows.Forms.TextBox tbTop;
        private System.Windows.Forms.TextBox tbBottom;
        private System.Windows.Forms.Label lbLeft;
        private System.Windows.Forms.Label lbTop;
        private System.Windows.Forms.Label lbRight;
        private System.Windows.Forms.Label lbBottom;
        private System.Windows.Forms.CheckBox chkBlurBehindWindow;
        private System.Windows.Forms.TextBox tbX;
        private System.Windows.Forms.TextBox tbWidth;
        private System.Windows.Forms.TextBox tbY;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.Label lbX;
        private System.Windows.Forms.Label lbWidth;
        private System.Windows.Forms.Label lbY;
        private System.Windows.Forms.Label lbHeight;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lbAeroGlassStyleSupported;
        private System.Windows.Forms.CheckBox chkEntendToEntireClientArea;
        private System.Windows.Forms.CheckBox chkEnableEntireFormBlurEffect;

    }
}