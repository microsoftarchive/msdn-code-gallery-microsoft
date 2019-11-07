using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Navigation
{
    //This page is used by the Scenario 3
    public sealed partial class CancelNavigationPage : Page
    {
        private static int _Id;

        public CancelNavigationPage()
        {
            this.InitializeComponent();
            _Id = _Id + 1;
            IDText.Text = "You are navigated to Page #" + _Id.ToString() + ".";
        }
        
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            e.Cancel = CancelNavigationSwitch.IsOn;

            base.OnNavigatingFrom(e);
        }
    }
}
