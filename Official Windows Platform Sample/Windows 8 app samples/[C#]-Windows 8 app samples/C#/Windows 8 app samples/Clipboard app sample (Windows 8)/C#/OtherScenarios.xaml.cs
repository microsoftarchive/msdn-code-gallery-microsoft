//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using SDKTemplate;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Clipboard
{
    public sealed partial class OtherScenarios : SDKTemplate.Common.LayoutAwarePage
    {
        MainPage rootPage = MainPage.Current;

        public OtherScenarios()
        {
            this.InitializeComponent();
            this.Init();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (rootPage.isClipboardContentChangeChecked)
            {
                RegisterClipboardContentChange.IsChecked = true;
            }

            if (rootPage.needToPrintClipboardFormat)
            {
                this.DisplayFormats(Windows.ApplicationModel.DataTransfer.Clipboard.GetContent());
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.needToPrintClipboardFormat = false;
        }

        #region Scenario Specific Code

        void Init()
        {
            ShowFormatButton.Click += new RoutedEventHandler(ShowFormatButton_Click);
            EmptyClipboardButton.Click += new RoutedEventHandler(EmptyClipboardButton_Click);
            RegisterClipboardContentChange.Checked += new RoutedEventHandler(RegisterClipboardContentChange_Check);
            RegisterClipboardContentChange.Unchecked += new RoutedEventHandler(RegisterClipboardContentChange_UnCheck);
            ClearOutputButton.Click += new RoutedEventHandler(ClearOutputButton_Click);
        }

        #endregion

        #region Button Click

        void ShowFormatButton_Click(object sender, RoutedEventArgs e)
        {
            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            this.DisplayFormats(dataPackageView);
        }

        void EmptyClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayFormatOutputText.Text = "";
            try
            {
                Windows.ApplicationModel.DataTransfer.Clipboard.Clear();
                OutputText.Text = "Clipboard has been emptied.";
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Error emptying Clipboard: " + ex.Message + ". Try again", NotifyType.ErrorMessage);
            }
        }

        void RegisterClipboardContentChange_Check(object sender, RoutedEventArgs e)
        {
            if (!rootPage.isClipboardContentChangeChecked)
            {
                rootPage.isClipboardContentChangeChecked = true;
                rootPage.clipboardContentChanged = new EventHandler<object>(this.RegisterClipboardChangeEventHandler);
                Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged += rootPage.clipboardContentChanged;
                Window.Current.CoreWindow.VisibilityChanged += new TypedEventHandler<CoreWindow, VisibilityChangedEventArgs>(CoreWindow_VisibilityChanged);
                OutputText.Text = "Successfully registered for clipboard update notification.";
            }
        }

        void RegisterClipboardContentChange_UnCheck(object sender, RoutedEventArgs e)
        {
            this.ClearOutput();
            rootPage.isClipboardContentChangeChecked = false;
            Windows.ApplicationModel.DataTransfer.Clipboard.ContentChanged -= rootPage.clipboardContentChanged;
            Window.Current.CoreWindow.VisibilityChanged -= new TypedEventHandler<CoreWindow, VisibilityChangedEventArgs>(CoreWindow_VisibilityChanged);
            OutputText.Text = "Successfully un-registered for clipboard update notification.";
        }

        void ClearOutputButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClearOutput();
        }

        #endregion

        #region Private helper methods

        private void ClearOutput()
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);
            OutputText.Text = "";
            DisplayFormatOutputText.Text = "";
        }

        private void DisplayFormats(DataPackageView dataPackageView)
        {
            if (dataPackageView != null && dataPackageView.AvailableFormats.Count > 0)
            {
                DisplayFormatOutputText.Text = "Available formats in the clipboard: ";
                var availableFormats = dataPackageView.AvailableFormats.GetEnumerator();
                while (availableFormats.MoveNext())
                {
                    DisplayFormatOutputText.Text += Environment.NewLine + availableFormats.Current;
                }
            }
            else
            {
                OutputText.Text = "Clipboard is empty";
            }
        }

        private void RegisterClipboardChangeEventHandler(object sender, object e)
        {
            // If user is not in  when clipboard content is changed, this flag will ensure the clipboard format gets printed out when user navigates to it
            rootPage.needToPrintClipboardFormat = true;
            if (this.isApplicationWindowActive)
            {
                rootPage.NotifyUser(String.Format("Clipboard content has been changed. Please select 'other clipboard operations' scenario to see the list of available formats."), NotifyType.StatusMessage);
                this.DisplayFormats(Windows.ApplicationModel.DataTransfer.Clipboard.GetContent());
            }
            else
            {
                rootPage.NotifyUser(String.Format("Clipboard content has been changed while the app was at background. Please select 'other clipboard operations' scenario to see the list of available formats."), NotifyType.StatusMessage);

                // Background applications can't access clipboard
                // Deferring processing of update notification till later
                this.containsUnprocessedNotifications = true;
            }
        }

        private void CoreWindow_VisibilityChanged(CoreWindow sender, VisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                // Application's main window has been activated (received focus), and therefore the application can now access clipboard
                this.isApplicationWindowActive = true;
                if (this.containsUnprocessedNotifications)
                {
                    this.DisplayFormats(Windows.ApplicationModel.DataTransfer.Clipboard.GetContent());
                    this.containsUnprocessedNotifications = false;
                }
            }
            else
            {
                // Application's main window has been deactivated (lost focus), and therefore the application can no longer access Clipboard
                this.isApplicationWindowActive = false;
            }
        }

        #endregion

        #region private member variables

        private bool isApplicationWindowActive = true;
        private bool containsUnprocessedNotifications = false;

        #endregion
    }
}
