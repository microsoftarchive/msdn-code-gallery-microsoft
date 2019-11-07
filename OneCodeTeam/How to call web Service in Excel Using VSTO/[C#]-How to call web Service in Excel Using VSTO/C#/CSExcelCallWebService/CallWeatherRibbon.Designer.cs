namespace CSExcelCallWebService
{
    partial class CallWeatherRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public CallWeatherRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.citybox = this.Factory.CreateRibbonEditBox();
            this.countrybox = this.Factory.CreateRibbonEditBox();
            this.btnGetWeather = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group1.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group1);
            this.tab1.Label = "Call WebService Ribbon";
            this.tab1.Name = "tab1";
            // 
            // group1
            // 
            this.group1.Items.Add(this.citybox);
            this.group1.Items.Add(this.countrybox);
            this.group1.Items.Add(this.btnGetWeather);
            this.group1.Label = "Weather Web Service";
            this.group1.Name = "group1";
            // 
            // citybox
            // 
            this.citybox.Label = "City Name:      ";
            this.citybox.Name = "citybox";
            this.citybox.Text = null;
            // 
            // countrybox
            // 
            this.countrybox.Label = "Country Name:";
            this.countrybox.Name = "countrybox";
            this.countrybox.Text = null;
            // 
            // btnGetWeather
            // 
            this.btnGetWeather.Label = "Get Weather";
            this.btnGetWeather.Name = "btnGetWeather";
            this.btnGetWeather.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnGetWeather_Click);
            // 
            // CallWeatherRibbon
            // 
            this.Name = "CallWeatherRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.CallWeatherRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonEditBox citybox;
        internal Microsoft.Office.Tools.Ribbon.RibbonEditBox countrybox;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnGetWeather;
    }

    partial class ThisRibbonCollection
    {
        internal CallWeatherRibbon CallWeatherRibbon
        {
            get { return this.GetRibbon<CallWeatherRibbon>(); }
        }
    }
}
