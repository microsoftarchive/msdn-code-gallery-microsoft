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
using Windows.System.UserProfile;

namespace AccountPictureName
{
    public sealed partial class GetUserDisplayName : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public GetUserDisplayName()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void GetDisplayNameButton_Click(object sender, RoutedEventArgs e)
        {
            string displayName = await UserInformation.GetDisplayNameAsync();
            if (string.IsNullOrEmpty(displayName))
            {
                rootPage.NotifyUser("No Display Name was returned", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Display Name = \"" + displayName + "\"", NotifyType.StatusMessage);
            }
        }
    }
}
