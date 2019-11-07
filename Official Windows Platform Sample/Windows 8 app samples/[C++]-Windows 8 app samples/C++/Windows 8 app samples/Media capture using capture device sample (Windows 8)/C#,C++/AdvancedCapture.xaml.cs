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
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using SDKTemplate;
using System;
using System.Threading.Tasks;

namespace MediaCapture
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdvancedCapture : SDKTemplate.Common.LayoutAwarePage
    {
        private Windows.Media.Capture.MediaCapture m_mediaCaptureMgr;
        private Windows.Storage.StorageFile m_recordStorageFile;
        private DeviceInformationCollection m_devInfoCollection;
        private bool m_bRecording;
        private bool m_bSuspended;
        private bool m_bPreviewing;
        private bool m_bEffectAddedToRecord = false;
        private bool m_bEffectAddedToPhoto = false;
        private EventHandler<Object> m_soundLevelHandler;
        private bool m_bRotateVideoOnOrientationChange;
        private bool m_bReversePreviewRotation;

        private readonly String VIDEO_FILE_NAME = "video.mp4";
        private readonly String PHOTO_FILE_NAME = "photo.jpg";
        private readonly String TEMP_PHOTO_FILE_NAME = "photoTmp.jpg";

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public AdvancedCapture()
        {
            this.InitializeComponent();
            m_soundLevelHandler = new EventHandler<Object>(MediaControl_SoundLevelChanged);
            ScenarioInit();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Media.MediaControl.SoundLevelChanged += m_soundLevelHandler;
            Windows.Graphics.Display.DisplayProperties.OrientationChanged += DisplayProperties_OrientationChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Media.MediaControl.SoundLevelChanged -= m_soundLevelHandler;
            Windows.Graphics.Display.DisplayProperties.OrientationChanged -= DisplayProperties_OrientationChanged;
        }

        private void ScenarioInit()
        {
            btnStartDevice2.IsEnabled = false;
            btnStartPreview2.IsEnabled = false;
            btnStartStopRecord2.IsEnabled = false;
            m_bRecording = false;
            m_bPreviewing = false;
            btnStartStopRecord2.Content = "StartRecord";
            btnTakePhoto2.IsEnabled = false;
            previewElement2.Source = null;
            playbackElement2.Source = null;
            imageElement2.Source = null;
            ShowStatusMessage("");
            chkAddRemoveEffect.IsChecked = false;
            chkAddRemoveEffect.IsEnabled = false;
            previewCanvas2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            EnumerateWebcamsAsync();
            m_bSuspended = false;
        }

        private void ScenarioReset()
        {
            previewCanvas2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ScenarioInit();
        }

        private async void MediaControl_SoundLevelChanged(object sender, Object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
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
                        previewElement2.Source = null;
                    }
                }
            });
        }

        public async void RecordLimitationExceeded(Windows.Media.Capture.MediaCapture currentCaptureObject)
        {
            if (m_bRecording)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    ShowStatusMessage("Stopping Record on exceeding max record duration");
                    await m_mediaCaptureMgr.StopRecordAsync();
                    m_bRecording = false;
                    SwitchRecordButtonContent();
                    EnableButton(true, "StartStopRecord");
                    ShowStatusMessage("Stopped record on exceeding max record duration:" + m_recordStorageFile.Path);
                });
            }
        }

        public async void Failed(Windows.Media.Capture.MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ShowStatusMessage("Fatal error" + currentFailure.Message);
            });
        }

        internal async void btnStartDevice_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                EnableButton(false, "StartDevice");
                ShowStatusMessage("Starting device");
                m_mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                var chosenDevInfo = m_devInfoCollection[EnumedDeviceList2.SelectedIndex];
                settings.VideoDeviceId = chosenDevInfo.Id;

                if (chosenDevInfo.EnclosureLocation != null && chosenDevInfo.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back)
                {
                    m_bRotateVideoOnOrientationChange = true;
                    m_bReversePreviewRotation = false;
                }
                else if (chosenDevInfo.EnclosureLocation != null && chosenDevInfo.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front)
                {
                    m_bRotateVideoOnOrientationChange = true;
                    m_bReversePreviewRotation = true;
                }
                else
                {
                    m_bRotateVideoOnOrientationChange = false;
                }

                await m_mediaCaptureMgr.InitializeAsync(settings);

                DisplayProperties_OrientationChanged(null);

                EnableButton(true, "StartPreview");
                EnableButton(true, "StartStopRecord");
                EnableButton(true, "TakePhoto");
                ShowStatusMessage("Device initialized successful");
                chkAddRemoveEffect.IsEnabled = true;
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

                previewCanvas2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                previewElement2.Source = m_mediaCaptureMgr;
                await m_mediaCaptureMgr.StartPreviewAsync();
                m_bPreviewing = true;
                ShowStatusMessage("Start preview successful");
            }
            catch (Exception exception)
            {
                m_bPreviewing = false;
                previewElement2.Source = null;
                EnableButton(true, "StartPreview");
                ShowExceptionMessage(exception);
            }
        }

        internal async void btnTakePhoto_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                var currentRotation = GetCurrentPhotoRotation();

                ShowStatusMessage("Taking photo");
                EnableButton(false, "TakePhoto");

                var tempPhotoStorageFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(TEMP_PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                ShowStatusMessage("Create photo file successful");
                var imageProperties = ImageEncodingProperties.CreateJpeg();

                await m_mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, tempPhotoStorageFile);

                var photoStorageFile = await ReencodePhotoAsync(tempPhotoStorageFile, currentRotation);

                EnableButton(true, "TakePhoto");
                ShowStatusMessage("Photo taken");

                var photoStream = await photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                ShowStatusMessage("File open successful");
                var bmpimg = new BitmapImage();

                bmpimg.SetSource(photoStream);
                imageElement2.Source = bmpimg;
                ShowStatusMessage(photoStorageFile.Path);
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

                    PrepareForVideoRecording();

                    m_recordStorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                    ShowStatusMessage("Create record file successful");

                    var recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);
                    await m_mediaCaptureMgr.StartRecordToStorageFileAsync(recordProfile, this.m_recordStorageFile);
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
                        playbackElement2.AutoPlay = true;
                        playbackElement2.SetSource(stream, this.m_recordStorageFile.FileType);
                        playbackElement2.Play();
                    }
                }
            }
            catch (Exception exception)
            {
                EnableButton(true, "StartStopRecord");
                ShowExceptionMessage(exception);
                m_bRecording = false;
                SwitchRecordButtonContent();
            }
        }

        internal async void lstEnumedDevices_SelectionChanged(Object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (m_bPreviewing)
            {
                await m_mediaCaptureMgr.StopPreviewAsync();
                m_bPreviewing = false;
            }

            btnStartDevice2.IsEnabled = true;
            btnStartPreview2.IsEnabled = false;
            btnStartStopRecord2.IsEnabled = false;
            m_bRecording = false;
            btnStartStopRecord2.Content = "StartRecord";
            btnTakePhoto2.IsEnabled = false;
            previewElement2.Source = null;
            playbackElement2.Source = null;
            imageElement2.Source = null;
            chkAddRemoveEffect.IsEnabled = false;
            chkAddRemoveEffect.IsChecked = false;
            m_bEffectAddedToRecord = false;
            m_bEffectAddedToPhoto = false;
            ShowStatusMessage("");
        }

        private async void EnumerateWebcamsAsync()
        {
            try
            {
                ShowStatusMessage("Enumerating Webcams...");
                m_devInfoCollection = null;

                EnumedDeviceList2.Items.Clear();

                m_devInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
                if (m_devInfoCollection.Count == 0)
                {
                    ShowStatusMessage("No WebCams found.");
                }
                else
                {
                    for (int i = 0; i < m_devInfoCollection.Count; i++)
                    {
                        var devInfo = m_devInfoCollection[i];
                        EnumedDeviceList2.Items.Add(devInfo.Name);
                    }
                    EnumedDeviceList2.SelectedIndex = 0;
                    ShowStatusMessage("Enumerating Webcams completed successfully.");
                    btnStartDevice2.IsEnabled = true;
                }
            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }
        }

        internal async void addEffectToImageStream()
        {
            if ((m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic != Windows.Media.Capture.VideoDeviceCharacteristic.AllStreamsIdentical) &&
                                               (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic != Windows.Media.Capture.VideoDeviceCharacteristic.PreviewPhotoStreamsIdentical) &&
                                               (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic != Windows.Media.Capture.VideoDeviceCharacteristic.RecordPhotoStreamsIdentical))
            {
                IMediaEncodingProperties props2 = m_mediaCaptureMgr.VideoDeviceController.GetMediaStreamProperties(Windows.Media.Capture.MediaStreamType.Photo);
                if (props2.GetType().Equals("Image")) //Cant add an effect to an image type
                {
                    //Change the media type on the stream
                    System.Collections.Generic.IReadOnlyList<IMediaEncodingProperties> supportedPropsList = m_mediaCaptureMgr.VideoDeviceController.GetAvailableMediaStreamProperties(Windows.Media.Capture.MediaStreamType.Photo);
                    int i = 0;
                    while (i++ < supportedPropsList.Count)
                    {
                        if (supportedPropsList[i].Type.Equals("Video"))
                        {
                            await m_mediaCaptureMgr.VideoDeviceController.SetMediaStreamPropertiesAsync(Windows.Media.Capture.MediaStreamType.Photo, supportedPropsList[i]);
                            ShowStatusMessage("Change type on image pin successful");
                            await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.Photo, "GrayscaleTransform.GrayscaleEffect", null);
                            ShowStatusMessage("Add effect to photo successful");
                            m_bEffectAddedToPhoto = true;
                            chkAddRemoveEffect.IsEnabled = true;
                            break;
                        }
                    }
                    chkAddRemoveEffect.IsEnabled = true;
                }
                else
                {
                    await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.Photo, "GrayscaleTransform.GrayscaleEffect", null);
                    ShowStatusMessage("Add effect to photo successful");
                    chkAddRemoveEffect.IsEnabled = true;
                    m_bEffectAddedToPhoto = true;

                }
            }
            else
            {
                chkAddRemoveEffect.IsEnabled = true;
            }
        }

        internal async void chkAddRemoveEffect_Checked(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                chkAddRemoveEffect.IsEnabled = false;
                await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.VideoPreview, "GrayscaleTransform.GrayscaleEffect", null);
                ShowStatusMessage("Add effect to video preview successful");
                if ((m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic != Windows.Media.Capture.VideoDeviceCharacteristic.AllStreamsIdentical) &&
                            (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic != Windows.Media.Capture.VideoDeviceCharacteristic.PreviewRecordStreamsIdentical))
                {
                    IMediaEncodingProperties props = m_mediaCaptureMgr.VideoDeviceController.GetMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoRecord);
                    if (!props.GetType().Equals("H264")) //Cant add an effect to an H264 stream
                    {
                        await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.VideoRecord, "GrayscaleTransform.GrayscaleEffect", null);
                        ShowStatusMessage("Add effect to video record successful");
                        m_bEffectAddedToRecord = true;
                        addEffectToImageStream();
                    }
                    else
                    {
                        addEffectToImageStream();
                        chkAddRemoveEffect.IsEnabled = true;
                    }
                }
                else
                {
                    addEffectToImageStream();
                    chkAddRemoveEffect.IsEnabled = true;
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        internal async void chkAddRemoveEffect_Unchecked(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                chkAddRemoveEffect.IsEnabled = false;
                await m_mediaCaptureMgr.ClearEffectsAsync(Windows.Media.Capture.MediaStreamType.VideoPreview);
                ShowStatusMessage("Remove effect from preview successful");
                if (m_bEffectAddedToRecord)
                {
                    await m_mediaCaptureMgr.ClearEffectsAsync(Windows.Media.Capture.MediaStreamType.VideoRecord);
                    ShowStatusMessage("Remove effect from preview successful");
                    m_bEffectAddedToRecord = false;
                }
                if (m_bEffectAddedToPhoto)
                {
                    await m_mediaCaptureMgr.ClearEffectsAsync(Windows.Media.Capture.MediaStreamType.Photo);
                    ShowStatusMessage("Remove effect from preview successful");
                    m_bEffectAddedToRecord = false;
                }
                chkAddRemoveEffect.IsEnabled = true;
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
            if (m_bRecording)
            {
                btnStartStopRecord2.Content = "StopRecord";
            }
            else
            {
                btnStartStopRecord2.Content = "StartRecord";
            }
        }

        private void EnableButton(bool enabled, String name)
        {
            if (name.Equals("StartDevice"))
            {
                btnStartDevice2.IsEnabled = enabled;
            }
            else if (name.Equals("StartPreview"))
            {
                btnStartPreview2.IsEnabled = enabled;
            }
            else if (name.Equals("StartStopRecord"))
            {
                btnStartStopRecord2.IsEnabled = enabled;
            }
            else if (name.Equals("TakePhoto"))
            {
                btnTakePhoto2.IsEnabled = enabled;
            }
        }

        private async Task<Windows.Storage.StorageFile> ReencodePhotoAsync(
            Windows.Storage.StorageFile tempStorageFile,
            Windows.Storage.FileProperties.PhotoOrientation photoRotation)
        {
            Windows.Storage.Streams.IRandomAccessStream inputStream = null;
            Windows.Storage.Streams.IRandomAccessStream outputStream = null;
            Windows.Storage.StorageFile photoStorage = null;

            try
            {
                inputStream = await tempStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);

                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(inputStream);

                photoStorage = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);

                outputStream = await photoStorage.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                outputStream.Size = 0;

                var encoder = await Windows.Graphics.Imaging.BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                var properties = new Windows.Graphics.Imaging.BitmapPropertySet();
                properties.Add("System.Photo.Orientation",
                    new Windows.Graphics.Imaging.BitmapTypedValue(photoRotation, Windows.Foundation.PropertyType.UInt16));

                await encoder.BitmapProperties.SetPropertiesAsync(properties);

                await encoder.FlushAsync();
            }
            finally
            {
                if (inputStream != null)
                {
                    inputStream.Dispose();
                }

                if (outputStream != null)
                {
                    outputStream.Dispose();
                }

                var asyncAction = tempStorageFile.DeleteAsync(Windows.Storage.StorageDeleteOption.PermanentDelete);
            }

            return photoStorage;
        }

        private Windows.Storage.FileProperties.PhotoOrientation GetCurrentPhotoRotation()
        {
            bool counterclockwiseRotation = m_bReversePreviewRotation;

            if (m_bRotateVideoOnOrientationChange)
            {
                return PhotoRotationLookup(Windows.Graphics.Display.DisplayProperties.CurrentOrientation, counterclockwiseRotation);
            }
            else
            {
                return Windows.Storage.FileProperties.PhotoOrientation.Normal;
            }
        }

        private void PrepareForVideoRecording()
        {
            if (m_mediaCaptureMgr == null)
            {
                return;
            }

            bool counterclockwiseRotation = m_bReversePreviewRotation;

            if (m_bRotateVideoOnOrientationChange)
            {
                m_mediaCaptureMgr.SetRecordRotation(VideoRotationLookup(Windows.Graphics.Display.DisplayProperties.CurrentOrientation, counterclockwiseRotation));
            }
            else
            {
                m_mediaCaptureMgr.SetRecordRotation(Windows.Media.Capture.VideoRotation.None);
            }
        }

        private void DisplayProperties_OrientationChanged(object sender)
        {
            if (m_mediaCaptureMgr == null)
            {
                return;
            }

            bool previewMirroring = m_mediaCaptureMgr.GetPreviewMirroring();
            bool counterclockwiseRotation = (previewMirroring && !m_bReversePreviewRotation) ||
                (!previewMirroring && m_bReversePreviewRotation);

            if (m_bRotateVideoOnOrientationChange)
            {
                m_mediaCaptureMgr.SetPreviewRotation(VideoRotationLookup(Windows.Graphics.Display.DisplayProperties.CurrentOrientation, counterclockwiseRotation));
            }
            else
            {
                m_mediaCaptureMgr.SetPreviewRotation(Windows.Media.Capture.VideoRotation.None);
            }
        }

        private Windows.Storage.FileProperties.PhotoOrientation PhotoRotationLookup(
            Windows.Graphics.Display.DisplayOrientations displayOrientation,
            bool counterclockwise)
        {
            switch (displayOrientation)
            {
                case Windows.Graphics.Display.DisplayOrientations.Landscape:
                    return Windows.Storage.FileProperties.PhotoOrientation.Normal;

                case Windows.Graphics.Display.DisplayOrientations.Portrait:
                    return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.Rotate270:
                        Windows.Storage.FileProperties.PhotoOrientation.Rotate90;

                case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped:
                    return Windows.Storage.FileProperties.PhotoOrientation.Rotate180;

                case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped:
                    return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.Rotate90 :
                        Windows.Storage.FileProperties.PhotoOrientation.Rotate270;

                default:
                    return Windows.Storage.FileProperties.PhotoOrientation.Unspecified;
            }
        }

        private Windows.Media.Capture.VideoRotation VideoRotationLookup(
            Windows.Graphics.Display.DisplayOrientations displayOrientation,
            bool counterclockwise)
        {
            switch (displayOrientation)
            {
                case Windows.Graphics.Display.DisplayOrientations.Landscape:
                    return Windows.Media.Capture.VideoRotation.None;

                case Windows.Graphics.Display.DisplayOrientations.Portrait:
                    return (counterclockwise) ? Windows.Media.Capture.VideoRotation.Clockwise270Degrees : 
                        Windows.Media.Capture.VideoRotation.Clockwise90Degrees;

                case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped:
                    return Windows.Media.Capture.VideoRotation.Clockwise180Degrees;

                case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped:
                    return (counterclockwise) ? Windows.Media.Capture.VideoRotation.Clockwise90Degrees :
                        Windows.Media.Capture.VideoRotation.Clockwise270Degrees;

                default:
                    return Windows.Media.Capture.VideoRotation.None;
            }
        }
    }
}
