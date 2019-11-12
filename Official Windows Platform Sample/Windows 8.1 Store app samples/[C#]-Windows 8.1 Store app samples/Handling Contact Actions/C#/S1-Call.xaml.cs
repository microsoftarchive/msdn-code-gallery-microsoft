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
    /// A page for 'Handling an activation to make a call' scenario that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CallScenario
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public CallScenario()
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
                IContactCallActivatedEventArgs callArgs = rootPage.ContactEvent as IContactCallActivatedEventArgs;
                if (callArgs != null)
                {
                    if (callArgs.ServiceId == "telephone")
                    {
                        rootPage.NotifyUser(
                            String.Format("Call activation was received. The phone number to call is {0}.", callArgs.ServiceUserId),
                            NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser(
                           String.Format("This app doesn't support calling by using the {0} service.", callArgs.ServiceId),
                           NotifyType.ErrorMessage);
                    }
                }
            }
            else if (rootPage.ProtocolEvent != null)
            {
                Uri protocolUri = rootPage.ProtocolEvent.Uri;
                if (protocolUri.Scheme == "tel")
                {
                    rootPage.NotifyUser(
                        String.Format("Tel: activation was received. The phone number to call is {0}.", protocolUri.AbsolutePath),
                        NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser(
                        String.Format("This app doesn't support the {0} protocol.", protocolUri.Scheme),
                        NotifyType.ErrorMessage);
                }
            }
        }
    }
}
