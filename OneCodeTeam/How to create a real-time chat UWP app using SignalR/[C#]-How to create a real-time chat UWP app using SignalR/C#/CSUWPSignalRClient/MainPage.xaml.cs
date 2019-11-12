using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CSUWPSignalRClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handle the parameters from chatroom page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);            
            if (e.Parameter!=null&& !e.Parameter.Equals(string.Empty))
            {
                dynamic info = e.Parameter;
                tbxGroup.Text = info.groupName;
                tbxName.Text = info.userName;
            }
        }

        async private void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }

        private async void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            btnJoin.IsEnabled = false;
            tbxError.Text = string.Empty;
            string groupName = tbxGroup.Text.Trim();
            string userName = tbxName.Text.Trim();
            if (groupName.Length == 0 || userName.Length == 0)
            {
                tbxError.Text = "Group & user name can't be empty.";
                btnJoin.IsEnabled = true;
                return;
            }
            //Connect to hub
            App myApp = (Application.Current as App);
            if (myApp.MyHubConnection.State != ConnectionState.Connected)
            {
                try
                {
                    await myApp.MyHubConnection.Start();
                }
                catch
                {
                    tbxError.Text = $"Can't connect to server {myApp.MyHubConnection.Url}";
                    btnJoin.IsEnabled = true;
                    return;
                }
            }
            //join to group
            if (myApp.MyHubConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                await myApp.MyHubProxy.Invoke("JoinGroup", groupName);
                dynamic info = new { groupName = groupName, userName = userName };
                Frame.Navigate(typeof(ChatRoom), info);
            }
            else
            {
                tbxError.Text = $"Can't connect to server {myApp.MyHubConnection.Url}";
            }
            btnJoin.IsEnabled = true;
        }

    }
}
