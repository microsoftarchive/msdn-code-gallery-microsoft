//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;

// New additions for media work

using Windows.Media.MediaProperties;
using Windows.Media.Capture;

// New additions for thread-pool timer
using Windows.System.Threading;

namespace HidInfraredSensor
{
    /// <summary>
    /// This scenario demonstrates how to monitor a HID motion-sensor and initiate
    /// video captures when motion is detected.
    /// </summary>
    public sealed partial class SensorTriggeredVideoCapture : SDKTemplate.Common.LayoutAwarePage
    {
        private TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs> interruptEventHandler;

        //media variables
        private Windows.Media.Capture.MediaCapture CaptureMgr;
        private Windows.Storage.StorageFile StorageFile;
        private readonly String VIDEO_FILE_NAME = "video.mp4";
        private Boolean Recording = false;
        Boolean Capture = false;

        public async void RecordLimitationExceeded(Windows.Media.Capture.MediaCapture currentCaptureObject)
        {
                if (Recording)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                            rootPage.NotifyUser(
                                "Stopping Record on exceeding max record duration",
                                 NotifyType.StatusMessage);
                     });
                }
        }

        public async void Failed(Windows.Media.Capture.MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser(
                        "Fatal error" + currentFailure.Message,
                        NotifyType.StatusMessage);
                });
 
        }

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        private MainPage rootPage = MainPage.Current;

        public SensorTriggeredVideoCapture()
        {
            interruptEventHandler = null;

            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        ///
        /// We will enable/disable parts of the UI if the device doesn't support it.
        /// 
        /// The IR_Sensor requires an output report to tell the device to start sending interrupts, so we will start it
        /// just in case we register for the events. We want this to be transparent because it doesn't demonstrate how 
        /// to register for events.
        /// </summary>
        /// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            // We will disable the scenario that is not supported by the device.
            // If no devices are connected, none of the scenarios will be shown and an error will be displayed
            Dictionary<DeviceModel, UIElement> deviceScenarios = new Dictionary<DeviceModel, UIElement>();
            deviceScenarios.Add(DeviceModel.IR_Sensor, IR_SensorScenario);

            DeviceList.Current.SetUpDeviceScenarios(deviceScenarios, DeviceScenarioContainer);


            UpdateRegisterEventButton();
        }

        /// <summary>
        /// Reset the device into a known state by undoing everything we did to the device (unregister interrupts and
        /// stop the device from creating interrupts)
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
            UnregisterFromInterruptEvent();
        }

        private void RegisterInterruptEvent_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs eventArgs)
        {
            if (DeviceList.Current.IsDeviceConnected)
            {
                TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs> interruptEventHandler =
                    new TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs>(this.OnGeneralInterruptEvent);

                RegisterForInterruptEvent(interruptEventHandler);
            }
            else
            {
                DeviceList.Current.NotifyDeviceNotConnected();
            }

        }

        private void UnregisterInterruptEvent_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs eventArgs)
        {
            if (DeviceList.Current.IsDeviceConnected)
            {
                UnregisterFromInterruptEvent();
            }
            else
            {
                DeviceList.Current.NotifyDeviceNotConnected();
            }
        }
  
        /// <summary>
        /// This method is called each time the motion sensor forwards data to the host. 
        /// 
        /// Note that reading is relatively straight forward and follows the general WinRT paradigm of using events args
        /// and reading from buffers.
        /// </summary>
        /// <param name="sender">The hidDevice that raised the event (the one that received the interrupt)</param>
        /// <param name="eventArgs">Contains the HidInputReport that caused the interrupt</param> 
        private async void OnGeneralInterruptEvent(HidDevice sender, HidInputReportReceivedEventArgs eventArgs)
        {
            // Retrieve the sensor data
            HidInputReport inputReport = eventArgs.Report;
            IBuffer buffer = inputReport.Data;
            DataReader dr = DataReader.FromBuffer(buffer);
            byte[] bytes = new byte[inputReport.Data.Length];
            dr.ReadBytes(bytes);

            // Set the video length and delay values.
            TimeSpan length = TimeSpan.FromSeconds(5); // Video length 5 seconds
            TimeSpan delay = TimeSpan.FromSeconds(15); // Pause or delay 15 seconds
            TimeSpan radioDelay = TimeSpan.FromMilliseconds(250); // Duration of radio-button highlight
 
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        // Briefly check the radio button to show that an event was fired
                        radio1.IsChecked = true; 
                    });

                // Create a threadpool timer which toggles the radio button on for the radioDelay interval
                ThreadPoolTimer RadioButtonTimer = ThreadPoolTimer.CreateTimer(
                     async (source) =>
                     {
                         await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {
                             // Radio button is unchecked once duration expires
                             radio1.IsChecked = false; 
                         });
                     }, radioDelay);

                // The first byte contains the motion data
                if ((bytes[1] == 1) && !Capture) 
                {
                    Capture = true;

                    // Create a threadpool timer which stops the video capture after "length" seconds
                    ThreadPoolTimer VideoStopTimer = ThreadPoolTimer.CreateTimer(
                         async (source) =>
                         {
                             await CaptureMgr.StopRecordAsync();

                             await rootPage.Dispatcher.RunAsync(
                                 CoreDispatcherPriority.Normal,
                                 new DispatchedHandler(() =>
                                 {
                                      rootPage.NotifyUser("Video capture concluded.",  NotifyType.StatusMessage);
                                 }));
                         }, length);

                    // Create a threadpool timer which prevents false captures by pausing detection for "delay" seconds
                    ThreadPoolTimer CapturePauseTimer = ThreadPoolTimer.CreateTimer(
                         async (source) =>
                         {
                              Capture = false;
                              await rootPage.Dispatcher.RunAsync(
                                CoreDispatcherPriority.Normal,
                                new DispatchedHandler(() =>
                                {
                                    rootPage.NotifyUser("Presence sensor enabled.", NotifyType.StatusMessage);
                                }));
                         }, delay);

                    await rootPage.Dispatcher.RunAsync(
                      CoreDispatcherPriority.Normal,
                      new DispatchedHandler(() =>
                      {
                          rootPage.NotifyUser("Video capture started.", NotifyType.StatusMessage);
                      }));

                    String fileName;
                    fileName = VIDEO_FILE_NAME;                 
                    StorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                    MediaEncodingProfile recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);
                    await CaptureMgr.StartRecordToStorageFileAsync(recordProfile, StorageFile);
                }
        }

        /// <summary>
        /// Register for the interrupt that is triggered when the device sends an interrupt to us.
        /// 
        /// All interrupts happen on HidDevice, so once we register the event, all interrupts (regardless of the HidInputReport) will
        /// raise the event. Read the comment above OnGeneralInterruptEvent for more information on how to distinguish input reports.
        ///
        /// The function also saves the event token so that we can unregister from the even later on.
        /// </summary>
        /// <param name="eventHandler">Event handler that will be called when the event is raised</param>
        private async void RegisterForInterruptEvent(TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs> eventHandler)
        {

                if (interruptEventHandler == null)
                {
                    // Save the interrupt handler so we can use it to unregister
                    interruptEventHandler = eventHandler;


                    DeviceList.Current.CurrentDevice.InputReportReceived += interruptEventHandler;

                    UpdateRegisterEventButton();

                    // Prepare for media captures

                    CaptureMgr = new Windows.Media.Capture.MediaCapture();
                    await CaptureMgr.InitializeAsync();

                    CaptureMgr.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(RecordLimitationExceeded); ;
                    CaptureMgr.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(Failed); ;

                    rootPage.NotifyUser("Video capture enabled." , NotifyType.StatusMessage);

                }
        }

        /// <summary>
        /// Unregisters from the interrupt event that was registered for in the RegisterForInterruptEvent();
        /// </summary>
        private async void UnregisterFromInterruptEvent()
        {
                if (interruptEventHandler != null)
                {
                    DeviceList.Current.CurrentDevice.InputReportReceived -= interruptEventHandler;

                    interruptEventHandler = null;
  
                    UpdateRegisterEventButton();
                }
         }

        private void UpdateRegisterEventButton()
        {
            ButtonRegisterInterruptEvent.IsEnabled = (interruptEventHandler == null);
            ButtonUnregisterInterruptEvent.IsEnabled = (interruptEventHandler != null);
        }
        
    }
}
