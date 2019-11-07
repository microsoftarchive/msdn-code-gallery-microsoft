using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CSUWPSignalRClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatRoom : Page
    {
        private string groupName { get; set; }
        private string userName { get; set; }

        private IDisposable receiveMessageHandler { get; set; }

        public ChatRoom()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }        

        /// <summary>
        /// get user name and group name from the mainpage
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            base.OnNavigatedTo(e);
            dynamic info = e.Parameter;
            groupName = info.groupName;
            userName = info.userName;
            this.tblGroupName.Text = groupName;
            this.tblUserName.Text = userName;
            App myApp = (Application.Current as App);
            receiveMessageHandler = myApp.MyHubProxy.On<string, string,DateTime>("receivemessage", ReceiveMessage);
        }

        private async void ReceiveMessage(string userName, string message,DateTime sendTime)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.lvwMessages.Items.Add($"{sendTime.ToString("MM-dd HH:mm:ss")}\n{userName}: {message}");
            });
        }

        async private void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }

        /// <summary>
        /// leave group and remove receive message handler
        /// navigate to mainpage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnExit_Click(object sender, RoutedEventArgs e)
        {
            App myApp = (Application.Current as App);
            if (myApp.MyHubConnection.State == ConnectionState.Connected)
            {
                await myApp.MyHubProxy.Invoke("LeaveGroup", groupName);
                receiveMessageHandler.Dispose();
            }
            Frame.Navigate(typeof(MainPage), new { groupName = groupName, userName = userName });
        }

        /// <summary>
        /// Invoke SendToGroup method 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            tblError.Text = string.Empty;
            App myApp = (Application.Current as App);
            if (myApp.MyHubConnection.State != ConnectionState.Connected)
            {
                tblError.Text = "Disconnected!";
                return;
            }

            string message = this.tbxMessage.Text.Trim();
            if (message.Length > 0)
            {
                myApp.MyHubProxy.Invoke("SendToGroup", groupName, userName, message, DateTime.Now);
            }
            this.tbxMessage.Text = string.Empty;
        }

    }
}
