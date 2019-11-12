//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using StoreRoom.Entities;
using StoreRoom.Model;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;

namespace StoreRoom
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        private CheckBox checkedItem;

        public MainPage()
        {
            this.InitializeComponent();

            // Populate the sample title from the constant in the Constants.cs file.
            SetFeatureName(FEATURE_NAME);
        }

        public void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            if (e.NotificationType == PushNotificationType.Toast)
            {
                this.RefreshOrders();
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.CurrentChannel.PushNotificationReceived += this.OnPushNotificationReceived;

            this.RefreshOrders();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.CurrentChannel.PushNotificationReceived -= this.OnPushNotificationReceived;
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void SetFeatureName(string str)
        {
            FeatureName.Text = str;
        }

        private async void RefreshOrders()
        {
            await this.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    this.ShowProgress(true);
                });
         
            try 
            {
                var orderItems = await App.MobileService.GetTable<DeliveryOrder>().Where(o => !o.Delivered).OrderBy(o => o.Id).ToEnumerableAsync();

                var model = new OrderModel();
                foreach (var item in orderItems)
                {
                    model.Items.Add(new OrderItemModel
                    {
                        Id = item.Id,
                        Customer = item.Customer,
                        Delivered = item.Delivered,
                        Product = item.Product,
                        Quantity = item.Quantity
                    });
                }

                await this.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        this.ShowProgress(false);
                        this.DataContext = model;
                    });
            }

            // ensure that mobile service url and key is configured before starting the sample
            catch (HttpRequestException)
            {
                Debug.WriteLine("Mobile service url and key should be configured correctly");
            }
        }

        private void ShowProgress(bool state)
        {
            if (state)
            {
                this.LoadingLegend.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.OrdersGridView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                this.LoadingLegend.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.OrdersGridView.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.checkedItem = sender as CheckBox;

            await this.ShowMessage("Confirmation", "The selected order will be marked as delivered. Do you want to continue?");
        }

        private async Task ShowMessage(string title, string message)
        {
            var messageDialog = new MessageDialog(message, title);
            messageDialog.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(this.UpdateOrder)));
            messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CancelUpdate)));
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;
            await messageDialog.ShowAsync();
        }

        private void CancelUpdate(IUICommand command)
        {
            this.checkedItem.IsChecked = false;
        }

        private async void UpdateOrder(IUICommand command)
        {
            var orderTable = App.MobileService.GetTable<DeliveryOrder>();
            var orders = await orderTable.Where(o => o.Id == (this.checkedItem.DataContext as OrderItemModel).Id).ToListAsync();
            var orderToUpdate = orders[0];
            orderToUpdate.Delivered = true;
            await orderTable.UpdateAsync(orderToUpdate);
            this.RefreshOrders();
        }
    }
}
