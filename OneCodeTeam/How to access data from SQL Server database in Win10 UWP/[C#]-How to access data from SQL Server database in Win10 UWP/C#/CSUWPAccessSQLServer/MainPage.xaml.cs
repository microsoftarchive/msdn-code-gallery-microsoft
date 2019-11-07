using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CSUWPAccessSQLServer.AccessSQLService;

namespace CSUWPAccessSQLServer
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        //Get data from SQL Server by WCF Service
        private async void GetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceClient client = new ServiceClient();
                IList<Article> articleList = await client.QueryArticleAsync();

                //set the result to UI
                lvDataTemplates.ItemsSource = articleList;
            }
            catch (Exception ex)
            {
                NotifyUser(ex.Message);
            }
        }

        // The event handler for the click event of the link in the footer. 
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://blogs.msdn.com/b/onecode"));
        }

        private void NotifyUser(string message)
        {
            StatusBlock.Text = message;
        }
    }
}
