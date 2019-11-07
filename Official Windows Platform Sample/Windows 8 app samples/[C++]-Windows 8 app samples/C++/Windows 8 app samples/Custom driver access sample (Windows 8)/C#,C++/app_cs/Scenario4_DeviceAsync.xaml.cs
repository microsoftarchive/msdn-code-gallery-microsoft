//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace CustomDeviceAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceAsync : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DeviceAsync()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void deviceAsyncSet_Click_1(object sender, RoutedEventArgs e)
        {
            if (DeviceList.Current.Fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            // Get the selector element
            var val = barGraphInput.SelectedIndex;
            bool[] barGraphArray = new bool[8];

            for (var i = 0; i < barGraphArray.Length; i += 1)
            {
                barGraphArray[i] = i < val;
            }

            try
            {
                DeviceList.Current.Fx2Device.SetBarGraphDisplay(barGraphArray);
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.ToString(), NotifyType.ErrorMessage);
                return;
            }

            ClearBarGraphTable();
            rootPage.NotifyUser("Bar Graph Set to " + val, NotifyType.StatusMessage);
        }


        private async void deviceAsyncGet_Click_1(object sender, RoutedEventArgs e)
        {
            if (DeviceList.Current.Fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                rootPage.NotifyUser("Getting Fx2 bars", NotifyType.StatusMessage);

                var result = await DeviceList.Current.Fx2Device.GetBarGraphDisplayAsync();
                rootPage.NotifyUser("Got bars value", NotifyType.StatusMessage);
                UpdateBarGraphTable(result.BarGraphDisplay);
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
        }

        private void ClearBarGraphTable()
        {
            barGraphOutput.Inlines.Clear();
        }

        private void UpdateBarGraphTable(bool[] barGraphArray)
        {
            var output = barGraphOutput;

            DeviceList.CreateBooleanTable(
                output.Inlines,
                barGraphArray,
                null,
                "Bar Number",
                "Bar State",
                "Lit",
                "Not Lit"
                );
        }


    }
}
