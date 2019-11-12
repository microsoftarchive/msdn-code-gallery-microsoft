//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace CallControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PhoneCall : SDKTemplate.Common.LayoutAwarePage
    {
        CoreWindow _cw;
        Windows.Media.Devices.CallControl callControls = null;
        ulong callToken = 0;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public PhoneCall()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ButtonInitialize.Click += new RoutedEventHandler(initDevice);
            ButtonIncomingCall.Click += new RoutedEventHandler(newIncomingCall);
            ButtonHangUp.Click += new RoutedEventHandler(hangUp);

            _cw = Window.Current.CoreWindow;
        }

        #region Scenario Specific Code

        void initDevice(Object sender, RoutedEventArgs e)
        {
            if (callControls == null)
            {
                try
                {
                    callControls = Windows.Media.Devices.CallControl.GetDefault();

                    if (callControls != null)
                    {
                        // Add the event listener to listen for the various button presses
                        callControls.AnswerRequested += new Windows.Media.Devices.CallControlEventHandler(answerButton);
                        callControls.HangUpRequested += new Windows.Media.Devices.CallControlEventHandler(hangupButton);
                        callControls.AudioTransferRequested += new Windows.Media.Devices.CallControlEventHandler(audiotransferButton);
                        callControls.RedialRequested += new Windows.Media.Devices.RedialRequestedEventHandler(redialButton);
                        callControls.DialRequested += new Windows.Media.Devices.DialRequestedEventHandler(dialButton);
                        enableIncomingCallButton();
                        disableInitializeButton();
                        dispatchMessage("Call Controls Initialized");
                    }
                    else
                    {
                        dispatchMessage("Call Controls Failed to Initialize");
                    }
                }
                catch (Exception exception)
                {
                    dispatchMessage("Call Controls Failed to Initialized in Try Catch" + exception.Message.ToString());
                }

            }
        }

        void newIncomingCall(Object sender, RoutedEventArgs e)
        {
            // Indicate a new incoming call and ring the headset.
            callToken = callControls.IndicateNewIncomingCall(true, "5555555555");
            disableIncomingCallButton();
            dispatchMessage("Call Token: " + callToken.ToString());
        }

        void hangUp(object sender, RoutedEventArgs e)
        {
            // Hang up request received.  The application should end the active call and stop
            // streaming to the headset
            dispatchMessage("Hangup requested");
            callControls.EndCall(callToken);
            enableIncomingCallButton();
            disableHangUpButton();
            stopAudioElement();
            callToken = 0;
        }

        void answerButton(Windows.Media.Devices.CallControl sender)
        {
            // When the answer button is pressed indicate to the device that the call was answered
            // and start a song on the headset (this is done by streaming music to the bluetooth
            // device in this sample)
            dispatchMessage("Answer Requested: " + callToken.ToString());
            callControls.IndicateActiveCall(callToken);
            SetAudioSource();
            enableHangUpButton();
            playAudioElement();

        }

        async void SetAudioSource()
        {
            var AudioFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("folk_rock.mp3");
            var AudioStream = await AudioFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AudioElement.SetSource(AudioStream, AudioFile.ContentType);
            });
        }

        void hangupButton(Windows.Media.Devices.CallControl sender)
        {
            // Hang up request received.  The application should end the active call and stop
            // streaming to the headset
            dispatchMessage("Hangup requested");
            callControls.EndCall(callToken);
            enableIncomingCallButton();
            disableHangUpButton();
            stopAudioElement();
            callToken = 0;
        }

        void audiotransferButton(Windows.Media.Devices.CallControl sender)
        {
            // Handle the audio transfer request here
            enableHangUpButton();
            dispatchMessage("Audio Transfer Requested");
        }

        void redialButton(Windows.Media.Devices.CallControl sender, Windows.Media.Devices.RedialRequestedEventArgs redialRequestedEventArgs)
        {
            // Handle the redial request here.  Indicate to the device that the request was handled.
            dispatchMessage("Redial Requested");
            redialRequestedEventArgs.Handled();
        }

        void dialButton(Windows.Media.Devices.CallControl sender, Windows.Media.Devices.DialRequestedEventArgs dialRequestedEventArgs)
        {
            // A device may send a dial request by either sending a URI or if it is a speed dial,
            // an integer with the number to dial.
            if (dialRequestedEventArgs.Contact.GetType() == typeof(UInt32))
            {
                dispatchMessage("Dial requested: " + dialRequestedEventArgs.Contact.ToString());
                dialRequestedEventArgs.Handled();
            }
            else
            {
                // Dialing a URI
                Uri uri = (Uri)dialRequestedEventArgs.Contact;
                if (uri.Scheme.Equals("tel", StringComparison.OrdinalIgnoreCase))
                {
                    string host = uri.Host;
                    string path = uri.PathAndQuery;
                    dispatchMessage("Dial requested: " + path);
                }


            }
        }

        async void playAudioElement()
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AudioElement.Play();
            });
        }

        async void stopAudioElement()
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AudioElement.Stop();
            });
        }

        async void enableIncomingCallButton()
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ButtonIncomingCall.IsEnabled = true;
            });
        }

        async void enableHangUpButton()
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ButtonHangUp.IsEnabled = true;
            });
        }

        async void disableInitializeButton()
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ButtonInitialize.IsEnabled = false;
            });
        }

        async void disableIncomingCallButton()
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ButtonIncomingCall.IsEnabled = false;
            });
        }

        async void disableHangUpButton()
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ButtonHangUp.IsEnabled = false;
            });
        }

        void dispatchMessage(string message)
        {
            dispatchStatusMessage(message);
        }

        async void dispatchErrorMessage(string message)
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser(getTimeStampedMessage(message), NotifyType.ErrorMessage);
            });
        }

        async void dispatchStatusMessage(string message)
        {
            await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser(getTimeStampedMessage(message), NotifyType.StatusMessage);
            });
        }

        string getTimeStampedMessage(string eventText)
        {
            Windows.Globalization.DateTimeFormatting.DateTimeFormatter dateTimeFormat = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
            string dateTime = dateTimeFormat.Format(DateTime.Now);
            return eventText + "   " + dateTime;
        }

        #endregion
    }
}
