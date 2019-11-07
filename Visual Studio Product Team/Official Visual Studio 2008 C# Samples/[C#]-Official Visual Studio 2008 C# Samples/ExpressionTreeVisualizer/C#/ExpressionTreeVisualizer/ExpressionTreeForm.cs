using System;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.Collections;
using System.Linq.Expressions;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ExpressionVisualizer
{
    public class TreeWindow : Form
    {
        private System.Windows.Forms.TextBox errorMessageBox;
        private TreeBrowser browser;
        private string errors;

        private void InitializeComponent()
        {
            this.errorMessageBox = new System.Windows.Forms.TextBox();


            this.SuspendLayout();

            // 
            // errorMessageBox
            // 
            this.errorMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.errorMessageBox.Location = new System.Drawing.Point(8, 8);
            this.errorMessageBox.Multiline = true;
            this.errorMessageBox.Name = "errorMessageBox";
            this.errorMessageBox.ReadOnly = true;
            this.errorMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.errorMessageBox.Size = new System.Drawing.Size(280, 56);
            this.errorMessageBox.TabIndex = 1;
            this.errorMessageBox.TabStop = false;
            this.errorMessageBox.Text = this.errors;

            // 
            // browser
            // 
            this.browser.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.browser.Location = new System.Drawing.Point(8, 72);
            this.browser.Size = new System.Drawing.Size(280, 192);
            this.browser.TabIndex = 2;
            this.browser.ExpandAll();

            // 
            // TreeWindow
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                      this.browser,
                                                                      this.errorMessageBox,
                                                                     });
            this.Name = "TreeWindow";
            this.Text = "Expression Tree Viewer";
            this.ResumeLayout(false);

            this.Size = new Size(600, 800);
        }

        public TreeWindow(TreeBrowser browser, string expression)
        {
            this.browser = browser;
            this.errors = expression;
            InitializeComponent();
        }
    }


    public class TreeBrowser : TreeView
    {
        public TreeBrowser() { }

        public void Add(TreeNode root)
        {
            Nodes.Add(root);
        }
    }
}