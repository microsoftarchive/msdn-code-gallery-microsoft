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
    /// A page for 'Handling an activation to map an address' scenario that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapAddressScenario
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public MapAddressScenario()
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
                ContactMapActivatedEventArgs mapArgs = rootPage.ContactEvent as ContactMapActivatedEventArgs;
                if (mapArgs != null)
                {
                    Windows.ApplicationModel.Contacts.ContactAddress address = mapArgs.Address;
                    rootPage.NotifyUser(
                        String.Format("Map address activation was received. The street address to map is {0}.",
                            String.IsNullOrEmpty(address.StreetAddress) ? "unspecified" : address.StreetAddress),
                        NotifyType.StatusMessage);
                }
            }
        }
    }
}
