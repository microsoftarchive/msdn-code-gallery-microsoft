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
    public sealed partial class PhotoSequence : SDKTemplate.Common.LayoutAwarePage
    {
        private Windows.Media.Capture.MediaCapture m_capture;
        private Windows.Media.Capture.LowLagPhotoSequenceCapture m_photoSequenceCapture;
        private Windows.Media.Capture.CapturedFrame[] m_framePtr;

        private Windows.Storage.StorageFile m_photoStorageFile;
        private TypedEventHandler<SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs> m_mediaPropertyChanged;

        private bool m_bPreviewing;
        private bool m_bPhotoSequence;
        private bool m_highLighted;

        private int m_selectedIndex;
        private uint m_frameNum;
        private uint m_ThumbnailNum;
        private uint m_pastFrame;
        private uint m_futureFrame;

        private readonly String PHOTOSEQ_FILE_NAME = "photoSequence.jpg";

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public PhotoSequence()
        {
            this.InitializeComponent();
            ScenarioInit();
            m_mediaPropertyChanged += new TypedEventHandler<SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs>(SystemMediaControls_PropertyChanged);
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

        private void ShowStatusMessage(String text)
        {
            rootPage.NotifyUser(text, NotifyType.StatusMessage);
        }

        private void ShowExceptionMessage(Exception ex)
        {
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
        }

        private void ScenarioInit()
        {
            try
            {
                btnStartDevice4.IsEnabled = true;
                btnStartPreview4.IsEnabled = false;
                btnStartStopPhotoSequence.IsEnabled = false;
                btnStartStopPhotoSequence.Content = "Prepare PhotoSequence";
                btnSaveToFile.IsEnabled = false;

                m_bPreviewing = false;
                m_bPhotoSequence = false;

                previewElement4.Source = null;

                m_photoSequenceCapture = null;
                Clear();

                //user can set the maximum history frame number
                m_pastFrame = 5;
                ////user can set the maximum future frame number
                m_futureFrame = 5;

                m_framePtr = new Windows.Media.Capture.CapturedFrame[m_pastFrame + m_futureFrame];

                m_frameNum = 0;
                m_ThumbnailNum = 0;
                m_selectedIndex = -1;
                m_highLighted = false;
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }

        }

        private async void ScenarioClose()
        {
            try
            {
                if (m_bPhotoSequence)
                {
                    ShowStatusMessage("Stopping PhotoSequence");
                    await m_photoSequenceCapture.FinishAsync();
                    m_photoSequenceCapture = null;
                    m_bPhotoSequence = false;
                    m_framePtr = null;
                }
                if (m_bPreviewing)
                {
                    await m_capture.StopPreviewAsync();
                    m_bPreviewing = false;
                }

                if (m_capture != null)
                {
                    previewElement4.Source = null;
                    m_capture.Dispose();
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }

        }

        private async void Failed(Windows.Media.Capture.MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
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



        private async void btnStartDevice_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                btnStartDevice4.IsEnabled = false;
                ShowStatusMessage("Starting device");
                m_capture = new Windows.Media.Capture.MediaCapture();

                await m_capture.InitializeAsync();

                if (m_capture.MediaCaptureSettings.VideoDeviceId != "")
                {

                    btnStartPreview4.IsEnabled = true;

                    ShowStatusMessage("Device initialized successful");

                    m_capture.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(Failed);
                }
                else
                {
                    btnStartDevice4.IsEnabled = true;
                    ShowStatusMessage("No Video Device Found");
                }
            }
            catch (Exception ex)
            {
                btnStartPreview4.IsEnabled = false;
                btnStartDevice4.IsEnabled = true;
                ShowExceptionMessage(ex);
            }
        }



        private async void btnStartPreview_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            try
            {
                ShowStatusMessage("Starting preview");
                btnStartPreview4.IsEnabled = false;
                btnStartStopPhotoSequence.IsEnabled = true;
                m_bPreviewing = true;

                previewCanvas4.Visibility = Windows.UI.Xaml.Visibility.Visible;
                previewElement4.Source = m_capture;
                await m_capture.StartPreviewAsync();

                ShowStatusMessage("Start preview successful");
            }
            catch (Exception ex)
            {
                previewElement4.Source = null;
                btnStartPreview4.IsEnabled = true;
                btnStartStopPhotoSequence.IsEnabled = false;
                m_bPreviewing = false;
                ShowExceptionMessage(ex);
            }
        }



        private void Clear()
        {
            PhotoGrid.Source = null;
            ThumbnailGrid.Items.Clear();
        }


        private async void btnStartStopPhotoSequence_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (btnStartStopPhotoSequence.Content.ToString() == "Prepare PhotoSequence")
                {

                    if (!m_capture.VideoDeviceController.LowLagPhotoSequence.Supported)
                    {
                        rootPage.NotifyUser("Photo-sequence is not supported", NotifyType.ErrorMessage);
                    }
                    else
                    {
                        if (m_capture.VideoDeviceController.LowLagPhotoSequence.MaxPastPhotos < m_pastFrame)
                        {
                            m_pastFrame = m_capture.VideoDeviceController.LowLagPhotoSequence.MaxPastPhotos;
                            rootPage.NotifyUser("pastFrame number is past limit, reset passFrame number", NotifyType.ErrorMessage);
                        }

                        btnStartStopPhotoSequence.IsEnabled = false;

                        m_capture.VideoDeviceController.LowLagPhotoSequence.ThumbnailEnabled = true;
                        m_capture.VideoDeviceController.LowLagPhotoSequence.DesiredThumbnailSize = 300;

                        m_capture.VideoDeviceController.LowLagPhotoSequence.PhotosPerSecondLimit = 4;
                        m_capture.VideoDeviceController.LowLagPhotoSequence.PastPhotoLimit = m_pastFrame;

                        LowLagPhotoSequenceCapture photoCapture = await m_capture.PrepareLowLagPhotoSequenceCaptureAsync(ImageEncodingProperties.CreateJpeg());

                        photoCapture.PhotoCaptured += new Windows.Foundation.TypedEventHandler<LowLagPhotoSequenceCapture, PhotoCapturedEventArgs>(this.photoCapturedEventHandler);

                        m_photoSequenceCapture = photoCapture;

                        btnStartStopPhotoSequence.Content = "Take PhotoSequence";
                        btnStartStopPhotoSequence.IsEnabled = true;
                        m_bPhotoSequence = true;
                    }
                }
                else if (btnStartStopPhotoSequence.Content.ToString() == "Take PhotoSequence")
                {
                    btnSaveToFile.IsEnabled = false;
                    m_frameNum = 0;
                    m_ThumbnailNum = 0;
                    m_selectedIndex = -1;
                    m_highLighted = false;
                    Clear();

                    btnStartStopPhotoSequence.IsEnabled = false;
                    await m_photoSequenceCapture.StartAsync();
                }
                else
                {
                    rootPage.NotifyUser("Bad photo-sequence state", NotifyType.ErrorMessage);
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }






        private async void photoCapturedEventHandler(LowLagPhotoSequenceCapture senders, PhotoCapturedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {

                try
                {
                    if (m_frameNum == (m_pastFrame + m_futureFrame))
                    {
                        btnStartStopPhotoSequence.IsEnabled = false;

                        await m_photoSequenceCapture.StopAsync();

                        btnStartStopPhotoSequence.IsEnabled = true;
                        btnSaveToFile.IsEnabled = true;
                        ThumbnailGrid.SelectedIndex = m_selectedIndex;
                    }
                    else if (m_frameNum < (m_pastFrame + m_futureFrame))
                    {
                        var bitmap = new BitmapImage();

                        m_framePtr[m_frameNum] = args.Frame;
               
                        bitmap.SetSource(args.Thumbnail);

                        var image = new Image();
                        image.Source = bitmap;

                        image.Width = 160;
                        image.Height = 120;

                        var ThumbnailItem = new Windows.UI.Xaml.Controls.GridViewItem();
                        ThumbnailItem.Content = image;
                        ThumbnailGrid.Items.Add(ThumbnailItem);

                        if ((!m_highLighted) && (args.CaptureTimeOffset.Ticks > 0))
                        {
                            //first picture with timeSpam > 0 get highlighted 
                            m_highLighted = true;

                            ThumbnailItem.BorderThickness = new Thickness(1);
                            ThumbnailItem.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                            m_selectedIndex = (int)m_ThumbnailNum;
                        }
                        m_ThumbnailNum++;
                    }
                    m_frameNum++;
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                }
            });

        }



        private async void ItemSelected_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            m_selectedIndex = ThumbnailGrid.SelectedIndex;
            if (m_selectedIndex > -1)
            {
                var bitmap = new BitmapImage();
                try
                {
                    await bitmap.SetSourceAsync(m_framePtr[m_selectedIndex].CloneStream());
                    PhotoGrid.Source = bitmap;
                }
                catch (Exception ex)
                {
                    ShowExceptionMessage(ex);
                    rootPage.NotifyUser("Display selected photo fail", NotifyType.ErrorMessage);
                }
            }
        }




        private async void btnSaveToFile_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                m_selectedIndex = ThumbnailGrid.SelectedIndex;

                if (m_selectedIndex > -1)
                {
                    m_photoStorageFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTOSEQ_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);

                    if (null == m_photoStorageFile)
                        rootPage.NotifyUser("PhotoFile creation fails", NotifyType.ErrorMessage);

                    var OutStream = await m_photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                    if (null == OutStream)
                        rootPage.NotifyUser("PhotoStream creation fails", NotifyType.ErrorMessage);

                    var ContentStream = m_framePtr[m_selectedIndex].CloneStream();

                    await Windows.Storage.Streams.RandomAccessStream.CopyAndCloseAsync(ContentStream.GetInputStreamAt(0), OutStream.GetOutputStreamAt(0));

                    ShowStatusMessage("Photo save complete");
                }
                else
                {
                    rootPage.NotifyUser("Please select a photo to display", NotifyType.ErrorMessage);
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

    }

}