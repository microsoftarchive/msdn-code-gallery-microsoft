namespace DocumentProtectionExcel
{
	partial class TechniqueUserControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.technique1Label = new System.Windows.Forms.Label();
			this.dataLabel = new System.Windows.Forms.Label();
			this.dataDataGridView = new System.Windows.Forms.DataGridView();
			this.technique2Label = new System.Windows.Forms.Label();
			this.dateLabel = new System.Windows.Forms.Label();
			this.dateDateTimePicker = new System.Windows.Forms.DateTimePicker();
			((System.ComponentModel.ISupportInitialize)(this.dataDataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// technique1Label
			// 
			this.technique1Label.AutoSize = true;
			this.technique1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.technique1Label.Location = new System.Drawing.Point(13, 13);
			this.technique1Label.Name = "technique1Label";
			this.technique1Label.Size = new System.Drawing.Size(74, 13);
			this.technique1Label.TabIndex = 0;
			this.technique1Label.Text = "Technique 1";
			// 
			// dataLabel
			// 
			this.dataLabel.AutoSize = true;
			this.dataLabel.Location = new System.Drawing.Point(13, 40);
			this.dataLabel.Name = "dataLabel";
			this.dataLabel.Size = new System.Drawing.Size(128, 13);
			this.dataLabel.TabIndex = 1;
			this.dataLabel.Text = "Modify customer data here";
			// 
			// dataDataGridView
			// 
			this.dataDataGridView.Location = new System.Drawing.Point(13, 70);
			this.dataDataGridView.Name = "dataDataGridView";
			this.dataDataGridView.Size = new System.Drawing.Size(183, 87);
			this.dataDataGridView.TabIndex = 2;
			this.dataDataGridView.Text = "dataGridView1";
			// 
			// technique2Label
			// 
			this.technique2Label.AutoSize = true;
			this.technique2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.technique2Label.Location = new System.Drawing.Point(13, 184);
			this.technique2Label.Name = "technique2Label";
			this.technique2Label.Size = new System.Drawing.Size(74, 13);
			this.technique2Label.TabIndex = 3;
			this.technique2Label.Text = "Technique 2";
			// 
			// dateLabel
			// 
			this.dateLabel.AutoSize = true;
			this.dateLabel.Location = new System.Drawing.Point(13, 214);
			this.dateLabel.Name = "dateLabel";
			this.dateLabel.Size = new System.Drawing.Size(66, 13);
			this.dateLabel.TabIndex = 4;
			this.dateLabel.Text = "Select a date";
			// 
			// dateDateTimePicker
			// 
			this.dateDateTimePicker.Location = new System.Drawing.Point(13, 244);
			this.dateDateTimePicker.Name = "dateDateTimePicker";
			this.dateDateTimePicker.Size = new System.Drawing.Size(183, 20);
			this.dateDateTimePicker.TabIndex = 5;
			this.dateDateTimePicker.ValueChanged += new System.EventHandler(this.dateDateTimePicker_ValueChanged);
			// 
			// TechniqueUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dateDateTimePicker);
			this.Controls.Add(this.dateLabel);
			this.Controls.Add(this.technique2Label);
			this.Controls.Add(this.dataDataGridView);
			this.Controls.Add(this.dataLabel);
			this.Controls.Add(this.technique1Label);
			this.MinimumSize = new System.Drawing.Size(210, 313);
			this.Name = "TechniqueUserControl";
			this.Size = new System.Drawing.Size(210, 313);
			((System.ComponentModel.ISupportInitialize)(this.dataDataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label technique1Label;
		private System.Windows.Forms.Label dataLabel;
		private System.Windows.Forms.DataGridView dataDataGridView;
		private System.Windows.Forms.Label technique2Label;
		private System.Windows.Forms.Label dateLabel;
		private System.Windows.Forms.DateTimePicker dateDateTimePicker;
	}
}
