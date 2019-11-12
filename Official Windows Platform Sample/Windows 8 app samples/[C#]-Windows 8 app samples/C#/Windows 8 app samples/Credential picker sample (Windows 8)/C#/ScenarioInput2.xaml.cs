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
using Windows.Security.Credentials.UI;

namespace CredentialPickerCS
{
    public sealed partial class ScenarioInput2 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        public ScenarioInput2()
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
        void SetError(string err)
        {
            rootPage.NotifyUser(err, NotifyType.ErrorMessage);
        }

        void SetResult(CredentialPickerResults res)
        {
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            TextBox status = outputFrame.FindName("Status") as TextBox;
            if (res.ErrorCode == 0)
            {
                status.Text = String.Format("OK");
            }
            else
            {
                status.Text = String.Format("Error returned from CredPicker API: {0}", res.ErrorCode);
            }
            TextBox domain = outputFrame.FindName("Domain") as TextBox;
            domain.Text = res.CredentialDomainName;
            TextBox username = outputFrame.FindName("Username") as TextBox;
            username.Text = res.CredentialUserName;
            TextBox password = outputFrame.FindName("Password") as TextBox;
            password.Text = res.CredentialPassword;
            TextBox credsaved = outputFrame.FindName("CredentialSaved") as TextBox;
            credsaved.Text = (res.CredentialSaved ? "true" : "false");
            TextBox checkboxState = outputFrame.FindName("CheckboxState") as TextBox;
            switch (res.CredentialSaveOption)
            {
                case CredentialSaveOption.Hidden:
                    checkboxState.Text = "Hidden";
                    break;
                case CredentialSaveOption.Selected:
                    checkboxState.Text = "Selected";
                    break;
                case CredentialSaveOption.Unselected:
                    checkboxState.Text = "Unselected";
                    break;
            }
        }

        #endregion

        #region Click Handlers
        private async void Launch_Click(object sender, RoutedEventArgs e)
        {
            if ((Target.Text != "") && (Message.Text != "") && (Caption.Text != ""))
            {
                var credPickerResults = await CredentialPicker.PickAsync(Target.Text, Message.Text, Caption.Text);
                SetResult(credPickerResults);
            }
        }

        #endregion
    }
}
