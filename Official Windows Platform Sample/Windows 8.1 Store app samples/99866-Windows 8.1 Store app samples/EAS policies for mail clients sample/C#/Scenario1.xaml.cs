//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Security.ExchangeActiveSyncProvisioning;


namespace EAS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
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

        private void DebugPrint(String Trace)
        {
            TextBox Scenario1DebugArea = rootPage.FindName("Scenario1DebugArea") as TextBox;
            Scenario1DebugArea.Text += Trace + "\r\n";
        }



        private void Launch_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                EasClientDeviceInformation CurrentDeviceInfor = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                TextBox DeviceID = rootPage.FindName("DeviceID") as TextBox;
                DeviceID.Text = CurrentDeviceInfor.Id.ToString();
                TextBox OperatingSystem = rootPage.FindName("OperatingSystem") as TextBox;
                OperatingSystem.Text = CurrentDeviceInfor.OperatingSystem;
                TextBox FriendlyName = rootPage.FindName("FriendlyName") as TextBox;
                FriendlyName.Text = CurrentDeviceInfor.FriendlyName;
                TextBox SystemManufacturer = rootPage.FindName("SystemManufacturer") as TextBox;
                SystemManufacturer.Text = CurrentDeviceInfor.SystemManufacturer;
                TextBox SystemProductName = rootPage.FindName("SystemProductName") as TextBox;
                SystemProductName.Text = CurrentDeviceInfor.SystemProductName;
                TextBox SystemSku = rootPage.FindName("SystemSku") as TextBox;
                SystemSku.Text = CurrentDeviceInfor.SystemSku;
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, Machine infor Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                TextBox DeviceID = rootPage.FindName("DeviceID") as TextBox;
                DeviceID.Text = "";
                TextBox OperatingSystem = rootPage.FindName("OperatingSystem") as TextBox;
                OperatingSystem.Text = "";
                TextBox FriendlyName = rootPage.FindName("FriendlyName") as TextBox;
                FriendlyName.Text = "";
                TextBox SystemManufacturer = rootPage.FindName("SystemManufacturer") as TextBox;
                SystemManufacturer.Text = "";
                TextBox SystemProductName = rootPage.FindName("SystemProductName") as TextBox;
                SystemProductName.Text = "";
                TextBox SystemSku = rootPage.FindName("SystemSku") as TextBox;
                SystemSku.Text = "";
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, Machine infor Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }
        }		        

    }
}
