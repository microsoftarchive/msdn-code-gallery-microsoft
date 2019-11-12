using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CSReceiveUri
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        ///When navigateed to this page from OnActivated of app.cs
        ///set the uri info to UI
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Uri uri = e.Parameter as Uri;
            if (uri != null)
            {
                this.Scheme.Text = uri.Scheme;
                this.Host.Text = uri.Host;
                this.LocalPath.Text = uri.LocalPath;
                this.Query.Text = uri.Query;
            }
        }
    }
}
