/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSWindowsStoreCustomMessageHeader
 * Copyright (c) Microsoft Corporation.
 * 
 *  This is the MainPage of the app.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using CSWindowsStoreCustomMessageHeader.CalculatorService;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CSWindowsStoreCustomMessageHeader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;
        }

        void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <= 500)
            {
                VisualStateManager.GoToState(this, "MinimalLayout", true);
            }
            else if (e.NewSize.Width < e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "PortraitLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            }
        }

        /// <summary>
        /// Handle the button's click event.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Click</param>
        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a client instance of the CustomMessageHeaderService.
                CalculatorServiceClient client = new CalculatorServiceClient();
                using (new OperationContextScope(client.InnerChannel))
                {
                    // We will use an instance of the custom class called UserInfo as a MessageHeader.
                    UserInfo userInfo = new UserInfo();
                    userInfo.FirstName = "John";
                    userInfo.LastName = "Doe";
                    userInfo.Age = 30;

                    // Add a SOAP Header to an outgoing request
                    MessageHeader aMessageHeader = MessageHeader.CreateHeader("UserInfo", "http://tempuri.org", userInfo);
                    OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                    // Add a HTTP Header to an outgoing request
                    HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                    requestMessage.Headers["MyHttpHeader"] = "MyHttpHeaderValue";
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

                    // Add the two numbers and get the result.
                    double result = await client.AddAsync(20, 40);
                    txtOut.Text = "Add result: " + result.ToString();
                }
            }
            catch (Exception oEx)
            {
                txtOut.Text = "Exception: " + oEx.Message;
            }
        }

        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }
    } 
}
