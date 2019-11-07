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
using Windows.Media.Capture;

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
        private EventHandler<Object> m_soundLevelHandler;

        private readonly String PHOTO_FILE_NAME = "photo.jpg";
        private readonly String VIDEO_FILE_NAME = "video.mp4";

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public BasicCapture()
        {
            this.InitializeComponent();
            ScenarioInit();
            m_soundLevelHandler = new EventHandler<Object>(MediaControl_SoundLevelChanged);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Media.MediaControl.SoundLevelChanged += m_soundLevelHandler;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Media.MediaControl.SoundLevelChanged -= m_soundLevelHandler;
        }

        private void ScenarioInit()
        {
            btnStartDevice1.IsEnabled = true;
            btnStartPreview1.IsEnabled = false;
            btnStartStopRecord1.IsEnabled = false;
            m_bRecording = false;
            m_bPreviewing = false;
            btnStartStopRecord1.Content = "StartRecord";
            btnTakePhoto1.IsEnabled = false;
            previewElement1.Source = null;
            playbackElement1.Source = null;
            imageElement1.Source = null;
            sldBrightness.IsEnabled = false;
            sldContrast.IsEnabled = false;
            m_bSuspended = false;
            previewCanvas1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            ShowStatusMessage("");

        }

        private async void MediaControl_SoundLevelChanged(object sender, Object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    if (Windows.Media.MediaControl.SoundLevel != Windows.Media.SoundLevel.Muted)
                    {
                        ScenarioReset();
                    }
                    else
                    {
                        if (m_bRecording)
                        {
                            ShowStatusMessage("Stopping Record on invisibility");

                            await m_mediaCaptureMgr.StopRecordAsync();
                            m_bRecording = false;
                            EnableButton(true, "StartStopRecord");
                        }
                        if (m_bPreviewing)
                        {
                            ShowStatusMessage("Stopping Preview on invisibility");

                            await m_mediaCaptureMgr.StopPreviewAsync();
                            m_bPreviewing = false;
                            EnableButton(true, "StartPreview");
                            previewElement1.Source = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                }
            });
        }

        public async void RecordLimitationExceeded(Windows.Media.Capture.MediaCapture currentCaptureObject)
        {
            try
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
                            SwitchRecordButtonContent();
                            EnableButton(true, "StartStopRecord");
                            ShowStatusMessage("Stopped record on exceeding max record duration:" + m_recordStorageFile.Path);
                        }
                        catch (Exception e)
                        {
                            ShowExceptionMessage(e);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
                m_bRecording = false;
                SwitchRecordButtonContent();
                EnableButton(true, "StartStopRecord");
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

        private void ScenarioReset()
        {
            previewCanvas1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ScenarioInit();
        }



        internal async void btnStartDevice_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                EnableButton(false, "StartDevice");
                ShowStatusMessage("Starting device");
                m_mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                await m_mediaCaptureMgr.InitializeAsync();

                EnableButton(true, "StartPreview");
                EnableButton(true, "StartStopRecord");
                EnableButton(true, "TakePhoto");
                ShowStatusMessage("Device initialized successful");
               
                m_mediaCaptureMgr.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(RecordLimitationExceeded); ;
                m_mediaCaptureMgr.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(Failed); ;

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
                EnableButton(false, "StartPreview");

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
                EnableButton(true, "StartPreview");
                ShowExceptionMessage(exception);
            }
        }

        internal async void btnTakePhoto_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                ShowStatusMessage("Taking photo");
                EnableButton(false, "TakePhoto");

                m_photoStorageFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);

                ShowStatusMessage("Create photo file successful");
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

                await m_mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile);

                EnableButton(true, "TakePhoto");
                ShowStatusMessage("Photo taken");

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
                EnableButton(true, "TakePhoto");
            }
        }

        internal async void btnStartStopRecord_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                String fileName;
                EnableButton(false, "StartStopRecord");

                if (!m_bRecording)
                {
                    ShowStatusMessage("Starting Record");

                    fileName = VIDEO_FILE_NAME;

                    m_recordStorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);

                    ShowStatusMessage("Create record file successful");

                    MediaEncodingProfile recordProfile = null;
                    recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);

                    await m_mediaCaptureMgr.StartRecordToStorageFileAsync(recordProfile, m_recordStorageFile);
                    m_bRecording = true;
                    SwitchRecordButtonContent();
                    EnableButton(true, "StartStopRecord");

                    ShowStatusMessage("Start Record successful");



                }
                else
                {
                    ShowStatusMessage("Stopping Record");

                    await m_mediaCaptureMgr.StopRecordAsync();

                    m_bRecording = false;
                    EnableButton(true, "StartStopRecord");
                    SwitchRecordButtonContent();

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
                SwitchRecordButtonContent();
                EnableButton(true, "StartStopRecord");
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

        private void SwitchRecordButtonContent()
        {
            {
                if (m_bRecording)
                {
                    btnStartStopRecord1.Content = "StopRecord";
                }
                else
                {
                    btnStartStopRecord1.Content = "StartRecord";
                }
            }
        }

        private void EnableButton(bool enabled, String name)
        {
            if (name.Equals("StartDevice"))
            {
                btnStartDevice1.IsEnabled = enabled;
            }
            else if (name.Equals("StartPreview"))
            {
                btnStartPreview1.IsEnabled = enabled;
            }
            else if (name.Equals("StartStopRecord"))
            {
                btnStartStopRecord1.IsEnabled = enabled;
            }
            else if (name.Equals("TakePhoto"))
            {
                btnTakePhoto1.IsEnabled = enabled;
            }
        }
    }
}
