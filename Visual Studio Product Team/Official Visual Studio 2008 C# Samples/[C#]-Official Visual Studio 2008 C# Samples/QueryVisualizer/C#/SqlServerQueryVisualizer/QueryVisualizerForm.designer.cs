namespace LinqToSqlQueryVisualizer {
    partial class QueryVisualizerForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.txtSql = new System.Windows.Forms.TextBox();
            this.btnQuery = new System.Windows.Forms.Button();
            this.chkUseOriginal = new System.Windows.Forms.CheckBox();
            this.radioQuery1 = new System.Windows.Forms.RadioButton();
            this.radioQuery2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txtExpression
            // 
            this.txtExpression.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExpression.Location = new System.Drawing.Point(12, 11);
            this.txtExpression.Multiline = true;
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.ReadOnly = true;
            this.txtExpression.Size = new System.Drawing.Size(661, 69);
            this.txtExpression.TabIndex = 0;
            // 
            // txtSql
            // 
            this.txtSql.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSql.Location = new System.Drawing.Point(13, 86);
            this.txtSql.Multiline = true;
            this.txtSql.Name = "txtSql";
            this.txtSql.Size = new System.Drawing.Size(660, 192);
            this.txtSql.TabIndex = 1;
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.Location = new System.Drawing.Point(609, 284);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(64, 23);
            this.btnQuery.TabIndex = 3;
            this.btnQuery.Text = "Execute";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // chkUseOriginal
            // 
            this.chkUseOriginal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkUseOriginal.AutoSize = true;
            this.chkUseOriginal.Location = new System.Drawing.Point(13, 286);
            this.chkUseOriginal.Name = "chkUseOriginal";
            this.chkUseOriginal.Size = new System.Drawing.Size(90, 17);
            this.chkUseOriginal.TabIndex = 4;
            this.chkUseOriginal.Text = "Original query";
            this.chkUseOriginal.UseVisualStyleBackColor = true;
            this.chkUseOriginal.CheckedChanged += new System.EventHandler(this.chkIncludeParams_CheckedChanged);
            // 
            // radioQuery1
            // 
            this.radioQuery1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioQuery1.AutoSize = true;
            this.radioQuery1.Location = new System.Drawing.Point(150, 286);
            this.radioQuery1.Name = "radioQuery1";
            this.radioQuery1.Size = new System.Drawing.Size(62, 17);
            this.radioQuery1.TabIndex = 5;
            this.radioQuery1.TabStop = true;
            this.radioQuery1.Text = "Query 1";
            this.radioQuery1.UseVisualStyleBackColor = true;
            this.radioQuery1.Visible = false;
            this.radioQuery1.CheckedChanged += new System.EventHandler(this.radioQuery1_CheckedChanged);
            // 
            // radioQuery2
            // 
            this.radioQuery2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioQuery2.AutoSize = true;
            this.radioQuery2.Location = new System.Drawing.Point(218, 285);
            this.radioQuery2.Name = "radioQuery2";
            this.radioQuery2.Size = new System.Drawing.Size(62, 17);
            this.radioQuery2.TabIndex = 6;
            this.radioQuery2.TabStop = true;
            this.radioQuery2.Text = "Query 2";
            this.radioQuery2.UseVisualStyleBackColor = true;
            this.radioQuery2.Visible = false;
            // 
            // QueryVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 308);
            this.Controls.Add(this.radioQuery2);
            this.Controls.Add(this.radioQuery1);
            this.Controls.Add(this.chkUseOriginal);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.txtSql);
            this.Controls.Add(this.txtExpression);
            this.Name = "QueryVisualizerForm";
            this.Text = "Linq to Sql Query Visualizer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.TextBox txtSql;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.CheckBox chkUseOriginal;
        private System.Windows.Forms.RadioButton radioQuery1;
        private System.Windows.Forms.RadioButton radioQuery2;
    }
}