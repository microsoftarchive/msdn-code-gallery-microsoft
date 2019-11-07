// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Security.ExchangeActiveSyncProvisioning;
using SDKTemplateCS;

namespace SDKTemplateCS
{
	public sealed partial class ScenarioInput1 : Page
	{
		// A pointer back to the main page which is used to gain access to the input and output frames and their content.
		MainPage rootPage = null;

		public ScenarioInput1()
		{
			InitializeComponent();
		}

		#region Template-Related Code - Do not remove
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Get a pointer to our main page
			rootPage = e.Parameter as MainPage;

			// We want to be notified with the OutputFrame is loaded so we can get to the content.
			rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
		}

		#endregion

        #region Use this code if you need access to elements in the output frame - otherwise delete
        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            Page outputFrame = (Page)rootPage.OutputFrame.Content;

            // Go find the elements that we need for this scenario.
            // ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;

        }
        #endregion

		private void DebugPrint(String Trace)
        {
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            TextBox Scenario1DebugArea = outputFrame.FindName("Scenario1DebugArea") as TextBox;
            Scenario1DebugArea.Text += Trace + "\r\n";
        }

 

		private void Launch_Click(object sender, RoutedEventArgs e)
		{   


            try
            {
                Page outputFrame = (Page)rootPage.OutputFrame.Content;
                EasClientDeviceInformation CurrentDeviceInfor = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                TextBox DeviceID = outputFrame.FindName("DeviceID") as TextBox;
                DeviceID.Text = CurrentDeviceInfor.Id.ToString();
                TextBox OperatingSystem = outputFrame.FindName("OperatingSystem") as TextBox;
                OperatingSystem.Text = CurrentDeviceInfor.OperatingSystem;
                TextBox FriendlyName = outputFrame.FindName("FriendlyName") as TextBox;
                FriendlyName.Text = CurrentDeviceInfor.FriendlyName;
                TextBox SystemManufacturer = outputFrame.FindName("SystemManufacturer") as TextBox;
                SystemManufacturer.Text = CurrentDeviceInfor.SystemManufacturer;
                TextBox SystemProductName = outputFrame.FindName("SystemProductName") as TextBox;
                SystemProductName.Text = CurrentDeviceInfor.SystemProductName;
                TextBox SystemSku = outputFrame.FindName("SystemSku") as TextBox;
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
                Page outputFrame = (Page)rootPage.OutputFrame.Content;
                TextBox DeviceID = outputFrame.FindName("DeviceID") as TextBox;
                DeviceID.Text = "";
                TextBox OperatingSystem = outputFrame.FindName("OperatingSystem") as TextBox;
                OperatingSystem.Text = "";
                TextBox FriendlyName = outputFrame.FindName("FriendlyName") as TextBox;
                FriendlyName.Text = "";
                TextBox SystemManufacturer = outputFrame.FindName("SystemManufacturer") as TextBox;
                SystemManufacturer.Text = "";
                TextBox SystemProductName = outputFrame.FindName("SystemProductName") as TextBox;
                SystemProductName.Text = "";
                TextBox SystemSku = outputFrame.FindName("SystemSku") as TextBox;
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
