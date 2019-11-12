namespace CSWindowsHook
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
            this.btnLocalMouseHook = new System.Windows.Forms.Button();
            this.btnGlobalLLMouseHook = new System.Windows.Forms.Button();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btnLocalKeyboardHook = new System.Windows.Forms.Button();
            this.btnGlobalLLKeyboardHook = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLocalMouseHook
            // 
            this.btnLocalMouseHook.Location = new System.Drawing.Point(12, 12);
            this.btnLocalMouseHook.Name = "btnLocalMouseHook";
            this.btnLocalMouseHook.Size = new System.Drawing.Size(482, 39);
            this.btnLocalMouseHook.TabIndex = 0;
            this.btnLocalMouseHook.Text = "Set Local Mouse Hook";
            this.btnLocalMouseHook.UseVisualStyleBackColor = true;
            this.btnLocalMouseHook.Click += new System.EventHandler(this.btnLocalMouseHook_Click);
            // 
            // btnGlobalLLMouseHook
            // 
            this.btnGlobalLLMouseHook.Location = new System.Drawing.Point(12, 57);
            this.btnGlobalLLMouseHook.Name = "btnGlobalLLMouseHook";
            this.btnGlobalLLMouseHook.Size = new System.Drawing.Size(482, 39);
            this.btnGlobalLLMouseHook.TabIndex = 1;
            this.btnGlobalLLMouseHook.Text = "Set Global LL Mouse Hook";
            this.btnGlobalLLMouseHook.UseVisualStyleBackColor = true;
            this.btnGlobalLLMouseHook.Click += new System.EventHandler(this.btnGlobalLLMouseHook_Click);
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(12, 194);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(482, 394);
            this.tbLog.TabIndex = 2;
            // 
            // btnLocalKeyboardHook
            // 
            this.btnLocalKeyboardHook.Location = new System.Drawing.Point(12, 102);
            this.btnLocalKeyboardHook.Name = "btnLocalKeyboardHook";
            this.btnLocalKeyboardHook.Size = new System.Drawing.Size(482, 39);
            this.btnLocalKeyboardHook.TabIndex = 4;
            this.btnLocalKeyboardHook.Text = "Set Local Keyboard Hook";
            this.btnLocalKeyboardHook.UseVisualStyleBackColor = true;
            this.btnLocalKeyboardHook.Click += new System.EventHandler(this.btnLocalKeyboardHook_Click);
            // 
            // btnGlobalLLKeyboardHook
            // 
            this.btnGlobalLLKeyboardHook.Location = new System.Drawing.Point(12, 147);
            this.btnGlobalLLKeyboardHook.Name = "btnGlobalLLKeyboardHook";
            this.btnGlobalLLKeyboardHook.Size = new System.Drawing.Size(482, 39);
            this.btnGlobalLLKeyboardHook.TabIndex = 5;
            this.btnGlobalLLKeyboardHook.Text = "Set Global LL Keyboard Hook";
            this.btnGlobalLLKeyboardHook.UseVisualStyleBackColor = true;
            this.btnGlobalLLKeyboardHook.Click += new System.EventHandler(this.btnGlobalLLKeyboardHook_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 600);
            this.Controls.Add(this.btnGlobalLLKeyboardHook);
            this.Controls.Add(this.btnLocalKeyboardHook);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.btnGlobalLLMouseHook);
            this.Controls.Add(this.btnLocalMouseHook);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Text = "Windows Hook Example";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLocalMouseHook;
        private System.Windows.Forms.Button btnGlobalLLMouseHook;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button btnLocalKeyboardHook;
        private System.Windows.Forms.Button btnGlobalLLKeyboardHook;
    }
}

