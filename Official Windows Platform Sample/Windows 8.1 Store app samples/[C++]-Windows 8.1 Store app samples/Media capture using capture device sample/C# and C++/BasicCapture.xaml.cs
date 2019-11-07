//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Foundation;

namespace MediaCapture
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BasicCapture : SDKTemplate.Common.LayoutAwarePage
    {
        private Windows.Media.Capture.MediaCapture m_mediaCaptureMgr;
        private Windows.Storage.StorageFile m_photoStorageFile;
        private Windows.Storage.StorageFile m_recordStorageFile;
        private bool m_bRecording;
        private bool m_bSuspended;
        private bool m_bPreviewing;
        private TypedEventHandler<SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs> m_mediaPropertyChanged;

        private readonly String PHOTO_FILE_NAME = "photo.jpg";
        private readonly String VIDEO_FILE_NAME = "video.mp4";



        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public BasicCapture()
        {
            this.InitializeComponent();
            ScenarioInit();
            m_mediaPropertyChanged += new TypedEventHandler<SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs>(SystemMediaControls_PropertyChanged);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemMediaTransportControls systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
            systemMediaControls.PropertyChanged += m_mediaPropertyChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SystemMediaTransportControls systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
            systemMediaControls.PropertyChanged -= m_mediaPropertyChanged;
            ScenarioClose();
        }

        private void ScenarioInit()
        {
            btnStartDevice1.IsEnabled = true;
            btnStartPreview1.IsEnabled = false;
            btnStartStopRecord1.IsEnabled = false;
            btnStartStopRecord1.Content = "StartRecord";
            btnTakePhoto1.IsEnabled = false;
            btnTakePhoto1.Content = "TakePhoto";

            m_bRecording = false;
            m_bPreviewing = false;
            m_bSuspended = false;

            previewElement1.Source = null;
            playbackElement1.Source = null;
            imageElement1.Source = null;
            sldBrightness.IsEnabled = false;
            sldContrast.IsEnabled = false;

            ShowStatusMessage("");
        }

        private async void ScenarioClose()
        {
            try
            {
                if (m_bRecording)
                {
                    ShowStatusMessage("Stopping Record");

                    await m_mediaCaptureMgr.StopRecordAsync();
                    m_bRecording = false;
                }
                if (m_bPreviewing)
                {
                    ShowStatusMessage("Stopping preview");
                    await m_mediaCaptureMgr.StopPreviewAsync();
                    m_bPreviewing = false;
                }

                if (m_mediaCaptureMgr != null)
                {
                    ShowStatusMessage("Stopping Camera");
                    previewElement1.Source = null;
                    m_mediaCaptureMgr.Dispose();
                }
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }
        }

        private async void SystemMediaControls_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs e)
        {
            switch (e.Property)
            {
                case SystemMediaTransportControlsProperty.SoundLevel:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (sender.SoundLevel != Windows.Media.SoundLevel.Muted)
                        {
                            ScenarioInit();
                        }
                        else
                        {
                            ScenarioClose();
                        }
                    });
                    break;

                default:
                    break;
            }
        }

        public async void RecordLimitationExceeded(Windows.Media.Capture.MediaCapture currentCaptureObject)
        {

            if (m_bRecording)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        ShowStatusMessage("Stopping Record on exceeding max record duration");
                        await m_mediaCaptureMgr.StopRecordAsync();
                        m_bRecording = false;
                        btnStartStopRecord1.Content = "StartRecord";
                        btnStartStopRecord1.IsEnabled = true;
                        ShowStatusMessage("Stopped record on exceeding max record duration:" + m_recordStorageFile.Path);

                        if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                        {
                            //if camera does not support record and Takephoto at the same time
                            //enable TakePhoto button again, after record finished
                            btnTakePhoto1.Content = "TakePhoto";
                            btnTakePhoto1.IsEnabled = true;
                        }
                    }
                    catch (Exception e)
                    {
                        ShowExceptionMessage(e);
                    }
                });
            }
        }

        public async void Failed(Windows.Media.Capture.MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ShowStatusMessage("Fatal error" + currentFailure.Message);
                });
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }
        }

        internal async void btnStartDevice_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                btnStartDevice1.IsEnabled = false;
                ShowStatusMessage("Starting device");
                m_mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                await m_mediaCaptureMgr.InitializeAsync();

                if (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceId != "" && m_mediaCaptureMgr.MediaCaptureSettings.AudioDeviceId != "")
                {

                    btnStartPreview1.IsEnabled = true;
                    btnStartStopRecord1.IsEnabled = true;
                    btnTakePhoto1.IsEnabled = true;

                    ShowStatusMessage("Device initialized successful");

                    m_mediaCaptureMgr.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(RecordLimitationExceeded);
                    m_mediaCaptureMgr.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(Failed);
                }
                else
                {
                    btnStartDevice1.IsEnabled = true;
                    ShowStatusMessage("No VideoDevice/AudioDevice Found");
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        internal async void btnStartPreview_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            m_bPreviewing = false;
            try
            {
                ShowStatusMessage("Starting preview");
                btnStartPreview1.IsEnabled = false;

                previewCanvas1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                previewElement1.Source = m_mediaCaptureMgr;
                await m_mediaCaptureMgr.StartPreviewAsync();
                if ((m_mediaCaptureMgr.VideoDeviceController.Brightness != null) && m_mediaCaptureMgr.VideoDeviceController.Brightness.Capabilities.Supported)
                {
                    SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Brightness, sldBrightness);
                }
                if ((m_mediaCaptureMgr.VideoDeviceController.Contrast != null) && m_mediaCaptureMgr.VideoDeviceController.Contrast.Capabilities.Supported)
                {
                    SetupVideoDeviceControl(m_mediaCaptureMgr.VideoDeviceController.Contrast, sldContrast);
                }
                m_bPreviewing = true;
                ShowStatusMessage("Start preview successful");

            }
            catch (Exception exception)
            {
                m_bPreviewing = false;
                previewElement1.Source = null;
                btnStartPreview1.IsEnabled = true;
                ShowExceptionMessage(exception);
            }
        }

        internal async void btnTakePhoto_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            try
            {
                ShowStatusMessage("Taking photo");
                btnTakePhoto1.IsEnabled = false;

                if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                {
                    //if camera does not support record and Takephoto at the same time
                    //disable Record button when taking photo
                    btnStartStopRecord1.IsEnabled = false;
                }

                m_photoStorageFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);

                ShowStatusMessage("Create photo file successful");
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

                await m_mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile);

                btnTakePhoto1.IsEnabled = true;
                ShowStatusMessage("Photo taken");

                if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                {
                    //if camera does not support record and Takephoto at the same time
                    //enable Record button after taking photo
                    btnStartStopRecord1.IsEnabled = true;
                }

                var photoStream = await m_photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);

                ShowStatusMessage("File open successful");
                var bmpimg = new BitmapImage();

                bmpimg.SetSource(photoStream);
                imageElement1.Source = bmpimg;
                ShowStatusMessage(this.m_photoStorageFile.Path);

            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
                btnTakePhoto1.IsEnabled = true;
            }
        }

        private async void StartRecord()
        {
            try
            {
                ShowStatusMessage("Starting Record");
                String fileName;
                fileName = VIDEO_FILE_NAME;

                m_recordStorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);

                ShowStatusMessage("Create record file successful");

                MediaEncodingProfile recordProfile = null;
                recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);

                await m_mediaCaptureMgr.StartRecordToStorageFileAsync(recordProfile, m_recordStorageFile);
                m_bRecording = true;
                btnStartStopRecord1.IsEnabled = true;
                btnStartStopRecord1.Content = "StopRecord";

                ShowStatusMessage("Start Record successful");
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }


        internal async void btnStartStopRecord_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                btnStartStopRecord1.IsEnabled = false;
                playbackElement1.Source = null;

                if (btnStartStopRecord1.Content.ToString() == "StartRecord")
                {
                    if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                    {
                        //if camera does not support record and Takephoto at the same time
                        //disable TakePhoto button when recording
                        btnTakePhoto1.IsEnabled = false;
                    }

                    StartRecord();

                }
                else //(btnStartStopRecord1.Content.ToString() == "StopRecord")
                {
                    ShowStatusMessage("Stopping Record");

                    await m_mediaCaptureMgr.StopRecordAsync();

                    m_bRecording = false;
                    btnStartStopRecord1.IsEnabled = true;
                    btnStartStopRecord1.Content = "StartRecord";

                    if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                    {
                        //if camera does not support lowlag record and lowlag photo at the same time
                        //enable TakePhoto button after recording
                        btnTakePhoto1.IsEnabled = true;
                    }

                    ShowStatusMessage("Stop record successful");
                    if (!m_bSuspended)
                    {
                        var stream = await m_recordStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);

                        ShowStatusMessage("Record file opened");
                        ShowStatusMessage(this.m_recordStorageFile.Path);
                        playbackElement1.AutoPlay = true;
                        playbackElement1.SetSource(stream, this.m_recordStorageFile.FileType);
                        playbackElement1.Play();
                    }

                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
                m_bRecording = false;
                btnStartStopRecord1.IsEnabled = true;
                btnStartStopRecord1.Content = "StartRecord";
            }

        }

        private void SetupVideoDeviceControl(Windows.Media.Devices.MediaDeviceControl videoDeviceControl, Slider slider)
        {
            try
            {
                if ((videoDeviceControl.Capabilities).Supported)
                {
                    slider.IsEnabled = true;
                    slider.Maximum = videoDeviceControl.Capabilities.Max;
                    slider.Minimum = videoDeviceControl.Capabilities.Min;
                    slider.StepFrequency = videoDeviceControl.Capabilities.Step;
                    double controlValue = 0;
                    if (videoDeviceControl.TryGetValue(out controlValue))
                    {
                        slider.Value = controlValue;
                    }
                }
                else
                {
                    slider.IsEnabled = false;
                }
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }
        }

        // VideoDeviceControllers
        internal void sldBrightness_ValueChanged(Object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            try
            {
                bool succeeded = m_mediaCaptureMgr.VideoDeviceController.Brightness.TrySetValue(sldBrightness.Value);
                if (!succeeded)
                {
                    ShowStatusMessage("Set Brightness failed");
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        internal void sldContrast_ValueChanged(Object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            try
            {
                bool succeeded = m_mediaCaptureMgr.VideoDeviceController.Contrast.TrySetValue(sldContrast.Value);
                if (!succeeded)
                {
                    ShowStatusMessage("Set Contrast failed");
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        private void ShowStatusMessage(String text)
        {
            rootPage.NotifyUser(text, NotifyType.StatusMessage);
        }

        private void ShowExceptionMessage(Exception ex)
        {
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
        }
    }
}
