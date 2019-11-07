//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Devices.Enumeration;
using Windows.Media;
using Windows.Media.Capture;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace CameraOptions
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowOptionsUI : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private MediaCapture mediaCaptureMgr = null;
        private bool previewStarted = false;
        private CoreDispatcher dispatcher = Window.Current.Dispatcher;

        public ShowOptionsUI()
        {
            this.InitializeComponent();
            ShowSettings.Visibility = Visibility.Collapsed;
            // Using the SoundLevelChanged event to determine when the app can stream from the webcam
            SystemMediaTransportControls systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
            systemMediaControls.PropertyChanged += SystemMediaControls_PropertyChanged;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// This is the click handler for the 'StartPreview' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the machine has a webcam
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                if (devices.Count > 0)
                {
                    rootPage.NotifyUser("", NotifyType.ErrorMessage);

                    if (mediaCaptureMgr == null)
                    {
                        // Using Windows.Media.Capture.MediaCapture APIs to stream from webcam
                        mediaCaptureMgr = new MediaCapture();
                        await mediaCaptureMgr.InitializeAsync();

                        VideoStream.Source = mediaCaptureMgr;
                        await mediaCaptureMgr.StartPreviewAsync();
                        previewStarted = true;

                        ShowSettings.Visibility = Visibility.Visible;
                        StartPreview.IsEnabled = false;
                    }
                }
                else
                {
                    rootPage.NotifyUser("A webcam is required to run this sample.", NotifyType.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                mediaCaptureMgr = null;
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This is the click handler for the 'ShowSettings' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mediaCaptureMgr != null)
                {
                    // Using Windows.Media.Capture.CameraOptionsUI API to show webcam settings
                    CameraOptionsUI.Show(mediaCaptureMgr);
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
            }
        }

        private async void SystemMediaControls_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs e)
        {
            switch (e.Property)
            {
                case SystemMediaTransportControlsProperty.SoundLevel:
                    // The callbacks for MediaControl_SoundLevelChanged and StopPreviewAsync may be invoked on threads other
                    // than the UI thread, so to ensure there's no synchronization issue, the Dispatcher is used here to
                    // ensure all operations run on the UI thread.
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            if (sender.SoundLevel == SoundLevel.Muted)
                            {
                                if (previewStarted)
                                {
                                    await mediaCaptureMgr.StopPreviewAsync();
                                    mediaCaptureMgr = null;
                                    previewStarted = false;
                                    VideoStream.Source = null;
                                }
                            }
                            else
                            {
                                if (!previewStarted)
                                {
                                    ShowSettings.Visibility = Visibility.Collapsed;
                                    StartPreview.IsEnabled = true;
                                }
                            }
                        });
                    break;

                default:
                    break;
            }
        }
    }
}
