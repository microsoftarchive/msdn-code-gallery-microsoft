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

namespace UserDomainName
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GetPrincipalName : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public GetPrincipalName()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This is the click handler for the 'GetName' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void GetPrincipalName_Click(object sender, RoutedEventArgs e)
        {
            if (!Windows.System.UserProfile.UserInformation.NameAccessAllowed)
            {
                PrincipalResultText.Text = "Access to user information is disabled by the user or administrator";
            }
            else
            {
                PrincipalResultText.Text = "Beginning asynchronous operation.";
                String upn = await Windows.System.UserProfile.UserInformation.GetPrincipalNameAsync();
                if (String.IsNullOrEmpty(upn))
                {
                    // NULL may be returned in any of the following circumstances:
                    // The information can not be retrieved from the directory
                    // The calling user is not a member of a domain
                    // The user or administrator has disabled the privacy setting
                    PrincipalResultText.Text = "No principal name returned for the current user.";
                }
                else
                {
                    PrincipalResultText.Text = upn;
                }
            }
        }
    }
}
