// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;

namespace RawNotificationsSampleCS
{
    public sealed partial class ScenarioInput2 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content
        MainPage rootPage = null;
        private CoreDispatcher _dispatcher;
        private bool eventAdded = false;

        public ScenarioInput2()
        {
            InitializeComponent();
            _dispatcher = Window.Current.Dispatcher;
        }

        #region Template-Related Code - Do not remove
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = e.Parameter as MainPage;

            // We want to be notified with the OutputFrame is loaded so we can get to the content
            rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
        }
        #endregion

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
            UpdateListener(false);
        }


        #region Use this code if you need access to elements in the output frame - otherwise delete
        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame

            // Get a pointer to the content within the OutputFrame
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
        }

        #endregion

        private void Scenario2AddListener_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateListener(true))
            {
                rootPage.NotifyUser("Now listening for raw notifications", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType.ErrorMessage);
            }
        }

        private void Scenario2RemoveListener_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateListener(false))
            {
                rootPage.NotifyUser("No longer listening for raw notifications", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType.ErrorMessage);
            }
        }

        private bool UpdateListener(bool add)
        {
            if (rootPage.Channel != null)
            {
                
                if (add && !eventAdded)
                {
                    rootPage.Channel.PushNotificationReceived += OnPushNotificationReceived;
                    eventAdded = true;
                }
                else if (!add && eventAdded)
                {
                    rootPage.Channel.PushNotificationReceived -= OnPushNotificationReceived;
                    eventAdded = false;
                }
                return true;
            }
	    return false;
        }

        private async void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            if (e.NotificationType == PushNotificationType.Raw)
            {
                e.Cancel = true;
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser("Raw notification received with content: " + e.RawNotification.Content, NotifyType.StatusMessage);
                });
            }
        }
    }
}
