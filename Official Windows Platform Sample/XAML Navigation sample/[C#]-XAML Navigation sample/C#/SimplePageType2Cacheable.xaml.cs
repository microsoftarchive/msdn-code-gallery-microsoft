using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Navigation
{
    //This page is used by Scenario 4
    public sealed partial class SimplePageType2Cacheable : Page
    {
        private bool _newPage;
        private static int _Id;

        public SimplePageType2Cacheable()
        {
            this.InitializeComponent();
            _Id = _Id + 1;
            _newPage = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IDText.Text = "You are navigated to Page Type 2 #" + _Id + ".";
            if (_newPage)
            {
                IDText.Text += "\nThis is a new instance of the page.";
                IDText.Text += "\nIt has been added to the cache.";
                _newPage = false;
            }
            else
            {
                IDText.Text += "\nThis is cached instance.";
            }

            if (e.Parameter != null)
            {
                MessageText.Text = e.Parameter.ToString();
            }
            base.OnNavigatedFrom(e);
        }
    }
}
