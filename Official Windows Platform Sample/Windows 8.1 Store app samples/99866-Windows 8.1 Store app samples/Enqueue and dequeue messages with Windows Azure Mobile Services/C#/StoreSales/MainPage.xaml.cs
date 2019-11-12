//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;

namespace StoreSales
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Populate the sample title from the constant in the Constants.cs file.
            SetFeatureName(FEATURE_NAME);

            var productList = new List<string>
            {
                "Lumia 920",
                "Surface RT",
                "LCD 40",
                "LCD 32",
                "Xbox 360 4gb",
                "Xbox 360 256gb"
            };

            var amountList = new List<int> { 1, 2, 3, 4, 5, 6 };

            this.DataContext = new OrderViewModel(productList, amountList);
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private static async Task<bool> IsValid(OrderViewModel orderViewModel)
        {
            if (string.IsNullOrWhiteSpace(orderViewModel.Customer))
            {
                var messageBox = new Windows.UI.Popups.MessageDialog("You must specify a customer name.", "Warning!");
                await messageBox.ShowAsync();
                return false;
            }

            if (string.IsNullOrWhiteSpace(orderViewModel.SelectedProduct))
            {
                var messageBox = new Windows.UI.Popups.MessageDialog("You must select a product.", "Warning!");
                await messageBox.ShowAsync();
                return false;
            }

            if (orderViewModel.SelectedAmount == -1)
            {
                var messageBox = new Windows.UI.Popups.MessageDialog("You must select an amount.", "Warning!");
                await messageBox.ShowAsync();
                return false;
            }

            return true;
        }

        private async void BuyClick(object sender, RoutedEventArgs e)
        {
            var orderViewModel = this.DataContext as OrderViewModel;

            if (await IsValid(orderViewModel))
            {
                IMobileServiceTable<Order> albumTable = App.MobileService.GetTable<Order>();

                var order = new Order
                {
                    Customer = orderViewModel.Customer,
                    Product = orderViewModel.SelectedProduct,
                    Quantity = orderViewModel.SelectedAmount,
                };

                this.ProgressRing.IsActive = true;
                this.PurchaseButton.IsEnabled = false;
                await albumTable.InsertAsync(order);
                this.ProgressRing.IsActive = false;
                this.PurchaseButton.IsEnabled = true;
                var messageBox = new Windows.UI.Popups.MessageDialog("A message has been sent to the storeroom.", "Done!");
                await messageBox.ShowAsync();

                orderViewModel.CleanUp();
            }
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void SetFeatureName(string str)
        {
            FeatureName.Text = str;
        }
    }

    public class MainPageSizeChangedEventArgs : EventArgs
    {
        private double width;

        public double Width
        {
            get { return width; }
            set { width = value; }
        }
    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
}
