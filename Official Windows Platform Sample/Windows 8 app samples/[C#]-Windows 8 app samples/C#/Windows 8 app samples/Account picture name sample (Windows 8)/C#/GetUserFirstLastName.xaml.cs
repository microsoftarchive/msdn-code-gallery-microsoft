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
    public sealed partial class GetUserFirstLastName : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public GetUserFirstLastName()
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

        private async void GetFirstNameButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = await UserInformation.GetFirstNameAsync();
            if (string.IsNullOrEmpty(firstName))
            {
                rootPage.NotifyUser("No First Name was returned", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("First Name = " + firstName, NotifyType.StatusMessage);
            }
        }

        private async void GetLastNameButton_Click(object sender, RoutedEventArgs e)
        {
            string lastName = await UserInformation.GetLastNameAsync();
            if (string.IsNullOrEmpty(lastName))
            {
                rootPage.NotifyUser("No Last Name was returned", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Last Name = " + lastName, NotifyType.StatusMessage);
            }
        }
    }
}
