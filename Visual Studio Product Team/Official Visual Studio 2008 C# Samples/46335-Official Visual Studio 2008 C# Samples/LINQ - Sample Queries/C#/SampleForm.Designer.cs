// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

namespace SampleSupport
{
    internal partial class SampleForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SampleForm));
            this.outerSplitContainer = new System.Windows.Forms.SplitContainer();
            this.samplesTreeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.samplesLabel = new System.Windows.Forms.Label();
            this.rightContainer = new System.Windows.Forms.SplitContainer();
            this.rightUpperSplitContainer = new System.Windows.Forms.SplitContainer();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.codeRichTextBox = new System.Windows.Forms.RichTextBox();
            this.codeLabel = new System.Windows.Forms.Label();
            this.runButton = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.outputLabel = new System.Windows.Forms.Label();
            this.outerSplitContainer.Panel1.SuspendLayout();
            this.outerSplitContainer.Panel2.SuspendLayout();
            this.outerSplitContainer.SuspendLayout();
            this.rightContainer.Panel1.SuspendLayout();
            this.rightContainer.Panel2.SuspendLayout();
            this.rightContainer.SuspendLayout();
            this.rightUpperSplitContainer.Panel1.SuspendLayout();
            this.rightUpperSplitContainer.Panel2.SuspendLayout();
            this.rightUpperSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // outerSplitContainer
            // 
            this.outerSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outerSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.outerSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.outerSplitContainer.Name = "outerSplitContainer";
            // 
            // outerSplitContainer.Panel1
            // 
            this.outerSplitContainer.Panel1.Controls.Add(this.samplesTreeView);
            this.outerSplitContainer.Panel1.Controls.Add(this.samplesLabel);
            // 
            // outerSplitContainer.Panel2
            // 
            this.outerSplitContainer.Panel2.Controls.Add(this.rightContainer);
            this.outerSplitContainer.Size = new System.Drawing.Size(952, 682);
            this.outerSplitContainer.SplitterDistance = 268;
            this.outerSplitContainer.TabIndex = 0;
            // 
            // samplesTreeView
            // 
            this.samplesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.samplesTreeView.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.samplesTreeView.HideSelection = false;
            this.samplesTreeView.ImageKey = "Item";
            this.samplesTreeView.ImageList = this.imageList;
            this.samplesTreeView.Location = new System.Drawing.Point(0, 28);
            this.samplesTreeView.Name = "samplesTreeView";
            this.samplesTreeView.SelectedImageKey = "Item";
            this.samplesTreeView.ShowNodeToolTips = true;
            this.samplesTreeView.ShowRootLines = false;
            this.samplesTreeView.Size = new System.Drawing.Size(266, 654);
            this.samplesTreeView.TabIndex = 1;
            this.samplesTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.samplesTreeView_AfterExpand);
            this.samplesTreeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.samplesTreeView_BeforeCollapse);
            this.samplesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.samplesTreeView_AfterSelect);
            this.samplesTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.samplesTreeView_AfterCollapse);
            this.samplesTreeView.DoubleClick += new System.EventHandler(this.samplesTreeView_DoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.Images.SetKeyName(0, "Help");
            this.imageList.Images.SetKeyName(1, "BookStack");
            this.imageList.Images.SetKeyName(2, "BookClosed");
            this.imageList.Images.SetKeyName(3, "BookOpen");
            this.imageList.Images.SetKeyName(4, "Item");
            this.imageList.Images.SetKeyName(5, "Run");
            // 
            // samplesLabel
            // 
            this.samplesLabel.AutoSize = true;
            this.samplesLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.samplesLabel.Location = new System.Drawing.Point(3, 9);
            this.samplesLabel.Name = "samplesLabel";
            this.samplesLabel.Size = new System.Drawing.Size(58, 16);
            this.samplesLabel.TabIndex = 0;
            this.samplesLabel.Text = "Samples:";
            // 
            // rightContainer
            // 
            this.rightContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightContainer.Location = new System.Drawing.Point(0, 0);
            this.rightContainer.Name = "rightContainer";
            this.rightContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // rightContainer.Panel1
            // 
            this.rightContainer.Panel1.Controls.Add(this.rightUpperSplitContainer);
            // 
            // rightContainer.Panel2
            // 
            this.rightContainer.Panel2.Controls.Add(this.runButton);
            this.rightContainer.Panel2.Controls.Add(this.outputTextBox);
            this.rightContainer.Panel2.Controls.Add(this.outputLabel);
            this.rightContainer.Size = new System.Drawing.Size(680, 682);
            this.rightContainer.SplitterDistance = 357;
            this.rightContainer.TabIndex = 0;
            // 
            // rightUpperSplitContainer
            // 
            this.rightUpperSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightUpperSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.rightUpperSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.rightUpperSplitContainer.Name = "rightUpperSplitContainer";
            this.rightUpperSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // rightUpperSplitContainer.Panel1
            // 
            this.rightUpperSplitContainer.Panel1.Controls.Add(this.descriptionTextBox);
            this.rightUpperSplitContainer.Panel1.Controls.Add(this.descriptionLabel);
            // 
            // rightUpperSplitContainer.Panel2
            // 
            this.rightUpperSplitContainer.Panel2.Controls.Add(this.codeRichTextBox);
            this.rightUpperSplitContainer.Panel2.Controls.Add(this.codeLabel);
            this.rightUpperSplitContainer.Size = new System.Drawing.Size(680, 357);
            this.rightUpperSplitContainer.SplitterDistance = 95;
            this.rightUpperSplitContainer.TabIndex = 0;
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.descriptionTextBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionTextBox.Location = new System.Drawing.Point(0, 28);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ReadOnly = true;
            this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.descriptionTextBox.Size = new System.Drawing.Size(680, 67);
            this.descriptionTextBox.TabIndex = 1;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLabel.Location = new System.Drawing.Point(3, 9);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(72, 16);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = "Description:";
            // 
            // codeRichTextBox
            // 
            this.codeRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.codeRichTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.codeRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.codeRichTextBox.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codeRichTextBox.Location = new System.Drawing.Point(0, 18);
            this.codeRichTextBox.Name = "codeRichTextBox";
            this.codeRichTextBox.ReadOnly = true;
            this.codeRichTextBox.Size = new System.Drawing.Size(680, 240);
            this.codeRichTextBox.TabIndex = 1;
            this.codeRichTextBox.Text = "";
            this.codeRichTextBox.WordWrap = false;
            // 
            // codeLabel
            // 
            this.codeLabel.AutoSize = true;
            this.codeLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codeLabel.Location = new System.Drawing.Point(3, -1);
            this.codeLabel.Name = "codeLabel";
            this.codeLabel.Size = new System.Drawing.Size(38, 16);
            this.codeLabel.TabIndex = 0;
            this.codeLabel.Text = "Code:";
            // 
            // runButton
            // 
            this.runButton.Enabled = false;
            this.runButton.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.runButton.ImageKey = "Run";
            this.runButton.ImageList = this.imageList;
            this.runButton.Location = new System.Drawing.Point(0, -1);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(119, 27);
            this.runButton.TabIndex = 0;
            this.runButton.Text = " Run Sample!";
            this.runButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.outputTextBox.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputTextBox.Location = new System.Drawing.Point(0, 48);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(680, 273);
            this.outputTextBox.TabIndex = 2;
            this.outputTextBox.WordWrap = false;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputLabel.Location = new System.Drawing.Point(3, 29);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(47, 16);
            this.outputLabel.TabIndex = 1;
            this.outputLabel.Text = "Output:";
            // 
            // SampleForm
            // 
            this.AcceptButton = this.runButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(952, 682);
            this.Controls.Add(this.outerSplitContainer);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SampleForm";
            this.Text = "Samples";
            this.outerSplitContainer.Panel1.ResumeLayout(false);
            this.outerSplitContainer.Panel1.PerformLayout();
            this.outerSplitContainer.Panel2.ResumeLayout(false);
            this.outerSplitContainer.ResumeLayout(false);
            this.rightContainer.Panel1.ResumeLayout(false);
            this.rightContainer.Panel2.ResumeLayout(false);
            this.rightContainer.Panel2.PerformLayout();
            this.rightContainer.ResumeLayout(false);
            this.rightUpperSplitContainer.Panel1.ResumeLayout(false);
            this.rightUpperSplitContainer.Panel1.PerformLayout();
            this.rightUpperSplitContainer.Panel2.ResumeLayout(false);
            this.rightUpperSplitContainer.Panel2.PerformLayout();
            this.rightUpperSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer outerSplitContainer;
        private System.Windows.Forms.Label samplesLabel;
        private System.Windows.Forms.SplitContainer rightContainer;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.SplitContainer rightUpperSplitContainer;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label codeLabel;
        private System.Windows.Forms.TreeView samplesTreeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.RichTextBox codeRichTextBox;
    }
}