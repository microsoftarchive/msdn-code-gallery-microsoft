namespace DataAnalysisExcel
{
    partial class UnscheduledOrderControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// ComboBox with the whole list of flavors.
        /// </summary>
        private System.Windows.Forms.ComboBox flavorComboBox;

        /// <summary>
        /// list of high inventory items.
        /// </summary>
        private System.Windows.Forms.ListBox highList;

        /// <summary>
        /// list of low inventory items.
        /// </summary>
        private System.Windows.Forms.ListBox lowList;

        /// <summary>
        ///  Data view based on the Sales table, representing the current day.
        /// </summary>
        private OperationsData.OperationsView view;

        private System.Windows.Forms.Label selectorLabel;

        private System.Windows.Forms.Label highLabel;

        private System.Windows.Forms.Label lowLabel;

        private System.Windows.Forms.GroupBox recommendationGroup;

        /// <summary>
        /// This displays recommendation as to whether an unscheduled
        /// order should be placed. 
        /// </summary>
        private System.Windows.Forms.Label recommendationLabel;

        private System.Windows.Forms.Label orderLabel;

        /// <summary>
        /// The button to create an unscheduled order.
        /// </summary>
        private System.Windows.Forms.Button createOrderButton;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnscheduledOrderControl));
            this.selectorLabel = new System.Windows.Forms.Label();
            this.flavorComboBox = new System.Windows.Forms.ComboBox();
            this.highLabel = new System.Windows.Forms.Label();
            this.lowLabel = new System.Windows.Forms.Label();
            this.highList = new System.Windows.Forms.ListBox();
            this.lowList = new System.Windows.Forms.ListBox();
            this.recommendationGroup = new System.Windows.Forms.GroupBox();
            this.createOrderButton = new System.Windows.Forms.Button();
            this.orderLabel = new System.Windows.Forms.Label();
            this.recommendationLabel = new System.Windows.Forms.Label();
            this.recommendationGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectorLabel
            // 
            resources.ApplyResources(this.selectorLabel, "selectorLabel");
            this.selectorLabel.Name = "selectorLabel";
            // 
            // flavorComboBox
            // 
            this.flavorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flavorComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.flavorComboBox, "flavorComboBox");
            this.flavorComboBox.Name = "flavorComboBox";
            this.flavorComboBox.SelectedIndexChanged += new System.EventHandler(this.flavorComboBox_SelectedIndexChanged);
            // 
            // highLabel
            // 
            resources.ApplyResources(this.highLabel, "highLabel");
            this.highLabel.Name = "highLabel";
            // 
            // lowLabel
            // 
            resources.ApplyResources(this.lowLabel, "lowLabel");
            this.lowLabel.Name = "lowLabel";
            // 
            // highList
            // 
            this.highList.FormattingEnabled = true;
            resources.ApplyResources(this.highList, "highList");
            this.highList.Name = "highList";
            this.highList.Click += new System.EventHandler(this.inventoryList_Click);
            // 
            // lowList
            // 
            this.lowList.FormattingEnabled = true;
            resources.ApplyResources(this.lowList, "lowList");
            this.lowList.Name = "lowList";
            this.lowList.Click += new System.EventHandler(this.inventoryList_Click);
            // 
            // recommendationGroup
            // 
            this.recommendationGroup.Controls.Add(this.createOrderButton);
            this.recommendationGroup.Controls.Add(this.orderLabel);
            this.recommendationGroup.Controls.Add(this.recommendationLabel);
            resources.ApplyResources(this.recommendationGroup, "recommendationGroup");
            this.recommendationGroup.Name = "recommendationGroup";
            this.recommendationGroup.TabStop = false;
            // 
            // createOrderButton
            // 
            resources.ApplyResources(this.createOrderButton, "createOrderButton");
            this.createOrderButton.Name = "createOrderButton";
            this.createOrderButton.Click += new System.EventHandler(this.CreateOrderButton_Click);
            // 
            // orderLabel
            // 
            resources.ApplyResources(this.orderLabel, "orderLabel");
            this.orderLabel.Name = "orderLabel";
            // 
            // recommendationLabel
            // 
            resources.ApplyResources(this.recommendationLabel, "recommendationLabel");
            this.recommendationLabel.Name = "recommendationLabel";
            // 
            // UnscheduledOrderControl
            // 
            this.Controls.Add(this.recommendationGroup);
            this.Controls.Add(this.lowList);
            this.Controls.Add(this.highList);
            this.Controls.Add(this.lowLabel);
            this.Controls.Add(this.highLabel);
            this.Controls.Add(this.flavorComboBox);
            this.Controls.Add(this.selectorLabel);
            this.Name = "UnscheduledOrderControl";
            resources.ApplyResources(this, "$this");
            this.recommendationGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
