//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace ContactActions
{
    /// <summary>
    /// A page for 'Handling an activation to send a message' scenario that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SendMessageScenario
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public SendMessageScenario()
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
            if (rootPage.ContactEvent != null)
            {
                ContactMessageActivatedEventArgs messageArgs = rootPage.ContactEvent as ContactMessageActivatedEventArgs;
                if (messageArgs != null)
                {
                    rootPage.NotifyUser(
                        String.Format("Send message activation was received. The service to use is {0}. The user ID to message is {1}.", messageArgs.ServiceId, messageArgs.ServiceUserId),
                        NotifyType.StatusMessage);
                }
            }
        }
    }
}
