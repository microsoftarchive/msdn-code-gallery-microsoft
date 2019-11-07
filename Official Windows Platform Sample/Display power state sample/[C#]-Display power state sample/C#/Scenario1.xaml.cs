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
using Windows.System.Display;

namespace DisplayRequests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        private const long LongMax = 2147483647L;
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private DisplayRequest AppDisplayRequest;
        private long DisplayRequestRefCount = 0;

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

        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null) {
                if (AppDisplayRequest == null) {
                    
                    // This call creates an instance of the displayRequest object
                    AppDisplayRequest = new DisplayRequest();
                }
                
                // This call activates a display-required request. If successful, 
                // the screen is guaranteed not to turn off automatically due to user inactivity.	
                if (DisplayRequestRefCount < LongMax)
                {
                    AppDisplayRequest.RequestActive();
                    DisplayRequestRefCount++;
                    rootPage.NotifyUser("Display request activated (" + DisplayRequestRefCount + ")", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Error: Exceeded maximum display request active instant count (" + DisplayRequestRefCount + ")", NotifyType.ErrorMessage);
                }
            }
        }

        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Release_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null) {
                if (AppDisplayRequest != null && DisplayRequestRefCount > 0) {
                
                    // This call de-activates the display-required request. If successful, the screen
                    // might be turned off automatically due to a user inactivity, depending on the
                    // power policy settings of the system. The requestRelease method throws an exception 
                    // if it is called before a successful requestActive call on this object.
                    AppDisplayRequest.RequestRelease();
                    DisplayRequestRefCount--;
                    rootPage.NotifyUser("Display request released (" + DisplayRequestRefCount + ")", NotifyType.StatusMessage);                  
                }
                else
                {
                    rootPage.NotifyUser("No existing active display request instance to be released", NotifyType.ErrorMessage);				
                }
            }
        }
    }
}
