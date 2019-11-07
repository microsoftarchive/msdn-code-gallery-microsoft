using System;
using Windows.UI.Xaml.Controls;

namespace Navigation
{
    // This Page is used by  Scenario1 
    public sealed partial class SimplePage : Page
    {
        private static int _Id;
                
        public SimplePage()
        {
            this.InitializeComponent();
            _Id = _Id + 1;
            IDText.Text = "You are navigated to Page #" + _Id.ToString() + ".";
        }
    }
}
