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
using Windows.Media;
using Windows.Media.Capture;
using Windows.Foundation;
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
        private Windows.Media.Capture.MediaCapture m_mediaCaptureMgr = null;
        private Windows.Media.Capture.LowLagPhotoCapture m_lowLagPhoto = null;
        private Windows.Media.Capture.LowLagMediaRecording m_lowLagRecord = null;
        private Windows.Storage.StorageFile m_recordStorageFile;
        private DeviceInformationCollection m_devInfoCollection;
        private DeviceInformationCollection m_microPhoneInfoCollection;
        private bool m_bLowLagPrepared;
        private bool m_bRecording;
        private bool m_bSuspended;
        private bool m_bPreviewing;
        private bool m_bEffectAddedToRecord = false;
        private bool m_bEffectAddedToPhoto = false;
        private TypedEventHandler<SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs> m_mediaPropertyChanged;
        private bool m_bRotateVideoOnOrientationChange;
        private bool m_bReversePreviewRotation;
        private Windows.Graphics.Display.DisplayOrientations m_displayOrientation;

        private readonly String VIDEO_FILE_NAME = "video.mp4";
        private readonly String PHOTO_FILE_NAME = "photo.jpg";

        private double m_rotHeight;
        private double m_rotWidth;
        static Guid rotGUID = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public AdvancedCapture()
        {
            this.InitializeComponent();
            m_mediaPropertyChanged = new TypedEventHandler<SystemMediaTransportControls, SystemMediaTransportControlsPropertyChangedEventArgs>(SystemMediaControls_PropertyChanged);
            ScenarioInit();
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
            Windows.Graphics.Display.DisplayInformation displayInfo = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            m_displayOrientation = displayInfo.CurrentOrientation;
            displayInfo.OrientationChanged += DisplayInfo_OrientationChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SystemMediaTransportControls systemMediaControls = SystemMediaTransportControls.GetForCurrentView();
            systemMediaControls.PropertyChanged -= m_mediaPropertyChanged;
            Windows.Graphics.Display.DisplayInformation.GetForCurrentView().OrientationChanged -= DisplayInfo_OrientationChanged;

            ScenarioClose();

        }

        private void ScenarioInit()
        {
            btnStartDevice2.IsEnabled = false;
            btnStartPreview2.IsEnabled = false;
            btnStartStopRecord2.IsEnabled = false;
            btnStartStopRecord2.Content = "StartRecord";
            btnTakePhoto2.IsEnabled = false;
            btnTakePhoto2.Content = "TakePhoto";

            m_bRecording = false;
            m_bPreviewing = false;
            m_bSuspended = false;
            m_bLowLagPrepared = false;
            chkAddRemoveEffect.IsChecked = false;
            chkAddRemoveEffect.IsEnabled = false;
            radTakePhoto.IsEnabled = false;
            radRecord.IsEnabled = false;
            radTakePhoto.IsChecked = false;
            radRecord.IsChecked = false;

            previewElement2.Source = null;
            playbackElement2.Source = null;
            imageElement2.Source = null;
            ShowStatusMessage("");

            EnumerateWebcamsAsync();
            EnumerateMicrophonesAsync();

            SceneModeList2.SelectedIndex = -1;
            SceneModeList2.Items.Clear();

            m_rotHeight = previewElement2.Width;
            m_rotWidth = previewElement2.Height;
        }

        private async void ScenarioClose()
        {
            try
            {
                if (m_bLowLagPrepared)
                {
                    ShowStatusMessage("Stopping LowLagPhoto");
                    await m_lowLagPhoto.FinishAsync();
                    m_bLowLagPrepared = false;
                }

                if (m_bRecording)
                {
                    ShowStatusMessage("Stopping LowLag Record");
                    await m_lowLagRecord.FinishAsync();
                    m_bRecording = false;
                    if (btnStartStopRecord2.Content.ToString() == "StartRecord")
                    {
                        await m_recordStorageFile.DeleteAsync();
                    }
                }
                if (m_bPreviewing)
                {
                    ShowStatusMessage("Stopping Preview");

                    await m_mediaCaptureMgr.StopPreviewAsync();
                    m_bPreviewing = false;
                    previewElement2.Source = null;
                }

                if (m_mediaCaptureMgr != null)
                {
                    ShowStatusMessage("Stopping Camera");
                    previewElement2.Source = null;
                    m_mediaCaptureMgr.Dispose();
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
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
            if (btnStartStopRecord2.Content.ToString() == "StopRecord")
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        ShowStatusMessage("Stopping Record on exceeding max record duration");
                        btnStartStopRecord2.IsEnabled = false;
                        await m_lowLagRecord.FinishAsync();
                        m_bRecording = false;
                        ShowStatusMessage("Stopped record on exceeding max record duration:" + m_recordStorageFile.Path);

                        if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                        {
                            //if camera does not support lowlag record and lowlag photo at the same time
                            //enable the checkbox
                            radTakePhoto.IsEnabled = true;
                            radRecord.IsEnabled = true;
                        }
                        //Prepare lowlag record for next round;
                        PrepareLowLagRecordAsync();
                    }
                    catch (Exception exception)
                    {
                        ShowExceptionMessage(exception);
                    }
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

        private async void PrepareLowLagRecordAsync()
        {
            try
            {
                PrepareForVideoRecording();

                m_recordStorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(VIDEO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                ShowStatusMessage("Create record file successful");
                MediaEncodingProfile recordProfile = null;
                recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);
                m_lowLagRecord = await m_mediaCaptureMgr.PrepareLowLagRecordToStorageFileAsync(recordProfile, m_recordStorageFile);
                m_bRecording = true;
                btnStartStopRecord2.Content = "StartRecord";
                btnStartStopRecord2.IsEnabled = true;
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        internal async void btnStartDevice_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {

                btnStartDevice2.IsEnabled = false;
                m_bReversePreviewRotation = false;
                ShowStatusMessage("Starting device");


                m_mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();



                var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                var chosenDevInfo = m_devInfoCollection[EnumedDeviceList2.SelectedIndex];
                settings.VideoDeviceId = chosenDevInfo.Id;

                if (EnumedMicrophonesList2.SelectedIndex >= 0 && m_microPhoneInfoCollection.Count > 0)
                {
                    var chosenMicrophoneInfo = m_microPhoneInfoCollection[EnumedMicrophonesList2.SelectedIndex];
                    settings.AudioDeviceId = chosenMicrophoneInfo.Id;
                }

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

                if (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceId != "" && m_mediaCaptureMgr.MediaCaptureSettings.AudioDeviceId != "")
                {
                    btnStartPreview2.IsEnabled = true;
                    ShowStatusMessage("Device initialized successful");
                    chkAddRemoveEffect.IsEnabled = true;
                    chkAddRemoveEffect.IsChecked = false;
                    m_mediaCaptureMgr.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(RecordLimitationExceeded);
                    m_mediaCaptureMgr.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(Failed);

                    if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                    {
                        radTakePhoto.IsEnabled = true;
                        radRecord.IsEnabled = true;
                        //choose TakePhoto Mode as defaul
                        radTakePhoto.IsChecked = true;
                    }
                    else
                    {
                        //prepare lowlag photo, then prepare lowlag record
                        m_lowLagPhoto = await m_mediaCaptureMgr.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg());

                        btnTakePhoto2.IsEnabled = true;
                        m_bLowLagPrepared = true;
                        PrepareLowLagRecordAsync();
                        //disable check options
                        radTakePhoto.IsEnabled = false;
                        radRecord.IsEnabled = false;
                    }

                    EnumerateSceneModeAsync();
                }
                else
                {
                    btnStartDevice2.IsEnabled = true;
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
                btnStartPreview2.IsEnabled = false;

                previewCanvas2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                previewCanvas2.Background.Opacity = 0;
                previewElement2.Source = m_mediaCaptureMgr;
                await m_mediaCaptureMgr.StartPreviewAsync();
                m_bPreviewing = true;
                OrientationChanged();
                ShowStatusMessage("Start preview successful");
            }
            catch (Exception exception)
            {
                m_bPreviewing = false;
                previewElement2.Source = null;
                btnStartPreview2.IsEnabled = true;
                ShowExceptionMessage(exception);
            }
        }

        internal async void btnTakePhoto_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            try
            {
                ShowStatusMessage("Taking photo");
                btnTakePhoto2.IsEnabled = false;

                if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                {
                    //disable check box while taking photo
                    radTakePhoto.IsEnabled = false;
                    radRecord.IsEnabled = false;
                }

                var photo = await m_lowLagPhoto.CaptureAsync();

                var currentRotation = GetCurrentPhotoRotation();
                var photoStorageFile = await ReencodePhotoAsync(photo.Frame.CloneStream(), currentRotation);

                btnTakePhoto2.IsEnabled = true;
                ShowStatusMessage("Photo taken");

                var photoStream = await photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                ShowStatusMessage("File open successful");
                var bmpimg = new BitmapImage();

                bmpimg.SetSource(photoStream);
                imageElement2.Source = bmpimg;
                ShowStatusMessage(photoStorageFile.Path);

                if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                {
                    //reset check options 
                    radTakePhoto.IsEnabled = true;
                    radTakePhoto.IsChecked = true;
                    radRecord.IsEnabled = true;
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
                btnTakePhoto2.IsEnabled = true;
                if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                {
                    //reset check options 
                    radTakePhoto.IsEnabled = true;
                    radRecord.IsEnabled = true;
                }
            }

        }

        internal async void btnStartStopRecord_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            try
            {
                if (btnStartStopRecord2.Content.ToString() == "StartRecord")
                {
                    if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                    {
                        //disable check box while recording
                        radTakePhoto.IsEnabled = false;
                        radRecord.IsEnabled = false;
                    }
                    btnStartStopRecord2.IsEnabled = false;
                    ShowStatusMessage("Starting Record");
                    await m_lowLagRecord.StartAsync();

                    btnStartStopRecord2.Content = "StopRecord";
                    btnStartStopRecord2.IsEnabled = true;
                    playbackElement2.Source = null;

                }
                else
                {
                    ShowStatusMessage("Stopping Record");
                    btnStartStopRecord2.IsEnabled = false;

                    await m_lowLagRecord.FinishAsync();
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

                    if (!m_mediaCaptureMgr.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
                    {
                        //reset check options 
                        radTakePhoto.IsEnabled = true;
                        radRecord.IsEnabled = true;
                        radRecord.IsChecked = true;
                    }
                    //prepare lowlag record for next round
                    m_bRecording = false;
                    PrepareLowLagRecordAsync();
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        internal void lstEnumedDevices_SelectionChanged(Object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            ScenarioClose();

            btnStartDevice2.IsEnabled = true;
            btnStartPreview2.IsEnabled = false;
            btnStartStopRecord2.IsEnabled = false;
            btnStartStopRecord2.Content = "StartRecord";
            btnTakePhoto2.IsEnabled = false;

            m_bRecording = false;
            m_bPreviewing = false;
            m_bSuspended = false;
            m_bLowLagPrepared = false;

            chkAddRemoveEffect.IsEnabled = false;

            radTakePhoto.IsEnabled = false;
            radRecord.IsEnabled = false;
            radTakePhoto.IsChecked = false;
            radRecord.IsChecked = false;

            previewCanvas2.Background.Opacity = 100;
            previewElement2.Source = null;
            playbackElement2.Source = null;
            imageElement2.Source = null;


            m_bEffectAddedToRecord = false;
            m_bEffectAddedToPhoto = false;
            SceneModeList2.SelectedIndex = -1;
            SceneModeList2.Items.Clear();
            ShowStatusMessage("Device changed, Initialize");
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
                        var location = devInfo.EnclosureLocation;

                        if (location != null)
                        {

                            if (location.Panel == Windows.Devices.Enumeration.Panel.Front)
                            {
                                EnumedDeviceList2.Items.Add(devInfo.Name + "-Front");
                            }
                            else if (location.Panel == Windows.Devices.Enumeration.Panel.Back)
                            {
                                EnumedDeviceList2.Items.Add(devInfo.Name + "-Back");
                            }
                            else
                            {
                                EnumedDeviceList2.Items.Add(devInfo.Name);
                            }
                        }
                        else
                        {
                            EnumedDeviceList2.Items.Add(devInfo.Name);
                        }
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

        private async void EnumerateMicrophonesAsync()
        {
            try
            {
                ShowStatusMessage("Enumerating Microphones...");
                m_microPhoneInfoCollection = null;

                EnumedMicrophonesList2.Items.Clear();

                m_microPhoneInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);

                try
                {
                    if (m_microPhoneInfoCollection == null || m_microPhoneInfoCollection.Count == 0)
                    {
                        ShowStatusMessage("No Microphones found.");
                    }
                    else
                    {
                        for (int i = 0; i < m_microPhoneInfoCollection.Count; i++)
                        {
                            var devInfo = m_microPhoneInfoCollection[i];
                            var location = devInfo.EnclosureLocation;
                            if (location != null)
                            {
                                if (location.Panel == Windows.Devices.Enumeration.Panel.Front)
                                {
                                    EnumedMicrophonesList2.Items.Add(devInfo.Name + "-Front");
                                }
                                else if (location.Panel == Windows.Devices.Enumeration.Panel.Back)
                                {
                                    EnumedMicrophonesList2.Items.Add(devInfo.Name + "-Back");

                                }
                                else
                                {
                                    EnumedMicrophonesList2.Items.Add(devInfo.Name);
                                }

                            }
                            else
                            {
                                EnumedMicrophonesList2.Items.Add(devInfo.Name);
                            }
                        }
                        EnumedMicrophonesList2.SelectedIndex = 0;
                        ShowStatusMessage("Enumerating Microphones completed successfully.");
                    }
                }
                catch (Exception e)
                {
                    ShowExceptionMessage(e);
                }

            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }
        }

        private void EnumerateSceneModeAsync()
        {
            try
            {
                ShowStatusMessage("Enumerating SceneMode...");

                SceneModeList2.Items.Clear();

                var sceneModes = m_mediaCaptureMgr.VideoDeviceController.SceneModeControl.SupportedModes;

                foreach (var mode in sceneModes)
                {
                    String modeName = null;

                    switch (mode)
                    {
                        case Windows.Media.Devices.CaptureSceneMode.Auto:
                            modeName = "Auto";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Macro:
                            modeName = "Macro";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Portrait:
                            modeName = "Portrait";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Sport:
                            modeName = "Sport";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Snow:
                            modeName = "Snow";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Night:
                            modeName = "Night";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Beach:
                            modeName = "Beach";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Sunset:
                            modeName = "Sunset";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Candlelight:
                            modeName = "Candlelight";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Landscape:
                            modeName = "Landscape";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.NightPortrait:
                            modeName = "Night portrait";
                            break;
                        case Windows.Media.Devices.CaptureSceneMode.Backlit:
                            modeName = "Backlit";
                            break;
                    }
                    if (modeName != null)
                    {
                        SceneModeList2.Items.Add(modeName);
                    }
                }

                if (sceneModes.Count > 0)
                {
                    SceneModeList2.SelectedIndex = 0;
                }

            }
            catch (Exception e)
            {
                ShowExceptionMessage(e);
            }

        }

        internal async void SceneMode_SelectionChanged(Object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (SceneModeList2.SelectedIndex > -1)
                {
                    var modeName = SceneModeList2.Items.IndexOf(SceneModeList2.SelectedIndex).ToString();

                    var mode = Windows.Media.Devices.CaptureSceneMode.Auto;

                    if (modeName == "Macro")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Macro;
                    }
                    else if (modeName == "Portrait")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Portrait;
                    }
                    else if (modeName == "Sport")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Sport;
                    }
                    else if (modeName == "Snow")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Snow;
                    }
                    else if (modeName == "Night")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Night;
                    }
                    else if (modeName == "Beach")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Beach;
                    }
                    else if (modeName == "Sunset")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Sunset;
                    }
                    else if (modeName == "Candlelight")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Candlelight;
                    }
                    else if (modeName == "Landscape")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Landscape;
                    }
                    else if (modeName == "Night portrait")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.NightPortrait;
                    }
                    else if (modeName == "Backlight")
                    {
                        mode = Windows.Media.Devices.CaptureSceneMode.Backlit;
                    }

                    await m_mediaCaptureMgr.VideoDeviceController.SceneModeControl.SetValueAsync(mode);
                    var message = "SceneMode is set to " + modeName;
                    ShowStatusMessage(message);
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }

        }

        internal async void addEffectToImageStream()
        {
            try
            {
                if ((m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic != Windows.Media.Capture.VideoDeviceCharacteristic.AllStreamsIdentical) &&
                                                   (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic != Windows.Media.Capture.VideoDeviceCharacteristic.PreviewPhotoStreamsIdentical) &&
                                                   (m_mediaCaptureMgr.MediaCaptureSettings.VideoDeviceCharacteristic != Windows.Media.Capture.VideoDeviceCharacteristic.RecordPhotoStreamsIdentical))
                {
                    IMediaEncodingProperties props2 = m_mediaCaptureMgr.VideoDeviceController.GetMediaStreamProperties(Windows.Media.Capture.MediaStreamType.Photo);
                    if (props2.Type.Equals("Image")) //Cant add an effect to an image type
                    {
                        //Change the media type on the stream
                        System.Collections.Generic.IReadOnlyList<IMediaEncodingProperties> supportedPropsList = m_mediaCaptureMgr.VideoDeviceController.GetAvailableMediaStreamProperties(Windows.Media.Capture.MediaStreamType.Photo);
                        int i = 0;
                        while (i++ < supportedPropsList.Count)
                        {
                            if (supportedPropsList[i].Type.Equals("Video"))
                            {
                                var bLowLagPrepare_tmp = m_bLowLagPrepared;

                                //it is necessary to un-prepare the lowlag photo before adding effect, since adding effect needs to change mediaType if it was not "Video" type;   
                                if (m_bLowLagPrepared)
                                {
                                    ShowStatusMessage("Stopping LowLagPhoto");
                                    await m_lowLagPhoto.FinishAsync();
                                    btnTakePhoto2.IsEnabled = false;
                                    m_bLowLagPrepared = false;
                                }
                                await m_mediaCaptureMgr.VideoDeviceController.SetMediaStreamPropertiesAsync(Windows.Media.Capture.MediaStreamType.Photo, supportedPropsList[i]);
                                ShowStatusMessage("Change type on image pin successful");
                                await m_mediaCaptureMgr.AddEffectAsync(Windows.Media.Capture.MediaStreamType.Photo, "GrayscaleTransform.GrayscaleEffect", null);
                                ShowStatusMessage("Add effect to photo successful");
                                m_bEffectAddedToPhoto = true;
                                chkAddRemoveEffect.IsEnabled = true;
                                //Prepare LowLag Photo again if LowLag Photo was prepared before adding effect;
                                if (bLowLagPrepare_tmp)
                                {
                                    m_lowLagPhoto = await m_mediaCaptureMgr.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg());
                                    btnTakePhoto2.IsEnabled = true;
                                    m_bLowLagPrepared = true;
                                }
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
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
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
                    if (!props.Type.Equals("H264")) //Cant add an effect to an H264 stream
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

        internal async void radTakePhoto_Checked(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (!m_bLowLagPrepared)
                {
                    //if camera does not support lowlag record and lowlag photo at the same time
                    //disable all buttons while preparing lowlag photo
                    btnStartStopRecord2.IsEnabled = false;
                    btnTakePhoto2.IsEnabled = false;
                    //uncheck record Mode
                    radRecord.IsChecked = false;
                    //disable check option while preparing lowlag photo
                    radTakePhoto.IsEnabled = false;
                    radRecord.IsEnabled = false;

                    if (m_bRecording)
                    {
                        //if camera does not support lowlag record and lowlag photo at the same time
                        //but lowlag record is already prepared, un-prepare lowlag record first, before preparing lowlag photo 
                        m_bRecording = false;
                        await m_lowLagRecord.FinishAsync();

                        ShowStatusMessage("Stopped record on preparing lowlag Photo:" + m_recordStorageFile.Path);

                        m_lowLagPhoto = await m_mediaCaptureMgr.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg());
                        btnTakePhoto2.IsEnabled = true;
                        m_bLowLagPrepared = true;
                        //re-enable check option, after lowlag record finish preparing
                        radTakePhoto.IsEnabled = true;
                        radRecord.IsEnabled = true;
                    }
                    else //(!m_bRecording)
                    {
                        //if camera does not support lowlag record and lowlag photo at the same time
                        //lowlag record is not prepared, go ahead to prepare lowlag photo 
                        m_lowLagPhoto = await m_mediaCaptureMgr.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg());
                        btnTakePhoto2.IsEnabled = true;
                        m_bLowLagPrepared = true;
                        //re-enable check option, after lowlag record finish preparing
                        radTakePhoto.IsEnabled = true;
                        radRecord.IsEnabled = true;
                    }
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        internal async void radRecord_Checked(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (!m_bRecording)
                {
                    //if camera does not support lowlag record and lowlag photo at the same time
                    //disable all buttons while preparing lowlag record
                    btnTakePhoto2.IsEnabled = false;
                    btnStartStopRecord2.IsEnabled = false;
                    //uncheck TakePhoto Mode
                    radTakePhoto.IsChecked = false;
                    //disable check option while preparing lowlag record
                    radTakePhoto.IsEnabled = false;
                    radRecord.IsEnabled = false;

                    if (m_bLowLagPrepared)
                    {
                        //if camera does not support lowlag record and lowlag photo at the same time
                        //but lowlag photo is already prepared, un-prepare lowlag photo first, before preparing lowlag record 
                        await m_lowLagPhoto.FinishAsync();

                        m_bLowLagPrepared = false;
                        //prepare lowlag record
                        PrepareForVideoRecording();

                        m_recordStorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(VIDEO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                        ShowStatusMessage("Create record file successful");
                        MediaEncodingProfile recordProfile = null;
                        recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);
                        m_lowLagRecord = await m_mediaCaptureMgr.PrepareLowLagRecordToStorageFileAsync(recordProfile, m_recordStorageFile);
                        btnStartStopRecord2.IsEnabled = true;
                        m_bRecording = true;
                        btnStartStopRecord2.Content = "StartRecord";

                        //re-enable check option, after lowlag record finish preparing
                        radTakePhoto.IsEnabled = true;
                        radRecord.IsEnabled = true;

                    }
                    else //(!m_bLowLagPrepared)
                    {
                        //if camera does not support lowlag record and lowlag photo at the same time
                        //lowlag photo is not prepared, go ahead to prepare lowlag record 
                        PrepareForVideoRecording();

                        m_recordStorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(VIDEO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                        ShowStatusMessage("Create record file successful");
                        MediaEncodingProfile recordProfile = null;
                        recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);
                        m_lowLagRecord = await m_mediaCaptureMgr.PrepareLowLagRecordToStorageFileAsync(recordProfile, m_recordStorageFile);
                        btnStartStopRecord2.IsEnabled = true;
                        m_bRecording = true;
                        btnStartStopRecord2.Content = "StartRecord";

                        //re-enable check option, after lowlag record finish preparing
                        radTakePhoto.IsEnabled = true;
                        radRecord.IsEnabled = true;
                    }
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

        private async Task<Windows.Storage.StorageFile> ReencodePhotoAsync(
            Windows.Storage.Streams.IRandomAccessStream stream,
            Windows.Storage.FileProperties.PhotoOrientation photoRotation)
        {
            Windows.Storage.Streams.IRandomAccessStream inputStream = null;
            Windows.Storage.Streams.IRandomAccessStream outputStream = null;
            Windows.Storage.StorageFile photoStorage = null;

            try
            {
                inputStream = stream;

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
            }

            return photoStorage;
        }

        private Windows.Storage.FileProperties.PhotoOrientation GetCurrentPhotoRotation()
        {
            bool counterclockwiseRotation = m_bReversePreviewRotation;

            if (m_bRotateVideoOnOrientationChange)
            {
                return PhotoRotationLookup(m_displayOrientation, counterclockwiseRotation);
            }
            else
            {
                return Windows.Storage.FileProperties.PhotoOrientation.Normal;
            }
        }

        private void PrepareForVideoRecording()
        {
            try
            {
                if (m_mediaCaptureMgr == null)
                {
                    return;
                }

                bool counterclockwiseRotation = m_bReversePreviewRotation;

                if (m_bRotateVideoOnOrientationChange)
                {
                    m_mediaCaptureMgr.SetRecordRotation(VideoRotationLookup(m_displayOrientation, counterclockwiseRotation));
                }
                else
                {
                    m_mediaCaptureMgr.SetRecordRotation(Windows.Media.Capture.VideoRotation.None);
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        private async void OrientationChanged()
        {
            try
            {
                if (m_mediaCaptureMgr == null)
                {
                    return;
                }

                var videoEncodingProperties = m_mediaCaptureMgr.VideoDeviceController.GetMediaStreamProperties(Windows.Media.Capture.MediaStreamType.VideoPreview);

                bool previewMirroring = m_mediaCaptureMgr.GetPreviewMirroring();
                bool counterclockwiseRotation = (previewMirroring && !m_bReversePreviewRotation) ||
                    (!previewMirroring && m_bReversePreviewRotation);

                if (m_bRotateVideoOnOrientationChange && m_bPreviewing)
                {
                    var rotDegree = VideoPreviewRotationLookup(m_displayOrientation, counterclockwiseRotation);
                    videoEncodingProperties.Properties.Add(rotGUID, rotDegree);
                    await m_mediaCaptureMgr.SetEncodingPropertiesAsync(Windows.Media.Capture.MediaStreamType.VideoPreview, videoEncodingProperties, null);
                    if (rotDegree == 90 || rotDegree == 270)
                    {
                        previewElement2.Height = m_rotHeight;
                        previewElement2.Width = m_rotWidth;
                    }
                    else
                    {
                        previewElement2.Height = m_rotWidth;
                        previewElement2.Width = m_rotHeight;
                    }
                }
            }
            catch (Exception exception)
            {
                ShowExceptionMessage(exception);
            }
        }

        private void DisplayInfo_OrientationChanged(Windows.Graphics.Display.DisplayInformation sender, object args)
        {
            m_displayOrientation = sender.CurrentOrientation;
            OrientationChanged();
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
                    return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.Rotate90 :
                        Windows.Storage.FileProperties.PhotoOrientation.Rotate270;

                case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped:
                    return Windows.Storage.FileProperties.PhotoOrientation.Rotate180;

                case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped:
                    return (counterclockwise) ? Windows.Storage.FileProperties.PhotoOrientation.Rotate270 :
                        Windows.Storage.FileProperties.PhotoOrientation.Rotate90;

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

        private uint VideoPreviewRotationLookup(
            Windows.Graphics.Display.DisplayOrientations displayOrientation, bool counterclockwise)
        {
            switch (displayOrientation)
            {
                case Windows.Graphics.Display.DisplayOrientations.Landscape:
                    return 0;

                case Windows.Graphics.Display.DisplayOrientations.Portrait:
                    {
                        if (counterclockwise)
                        {
                            return 270;
                        }
                        else
                        {
                            return 90;
                        }
                    }

                case Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped:
                    return 180;

                case Windows.Graphics.Display.DisplayOrientations.PortraitFlipped:
                    {
                        if (counterclockwise)
                        {
                            return 90;
                        }
                        else
                        {
                            return 270;
                        }
                    }

                default:
                    return 0;
            }
        }
    }

}
