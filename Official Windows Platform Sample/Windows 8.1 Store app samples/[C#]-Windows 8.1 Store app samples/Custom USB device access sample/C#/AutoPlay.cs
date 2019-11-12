//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CustomUsbDeviceAccess;

namespace SDKTemplate
{
    public partial class App : Application
    {
        /// <summary>
        /// Invoked when the application is activated.
        /// This is the entry point for Device Autoplay when a device is attached to the PC.
        /// Other activation kinds (such as search and protocol activation) may also be handled here.
        ///
        /// This code was adapted from the "Removable storage sample" on MSDN.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Device)
            {
                // Load the UI
                if (Window.Current.Content == null)
                {
                    Frame rootFrame = new Frame();
                    rootFrame.Navigate(typeof(MainPage));

                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;
                }

                // Ensure the current window is active or else the app will freeze at the splash screen
                Window.Current.Activate();

                // Launched from Autoplay for device, notify the app what device launched this app
                DeviceActivatedEventArgs deviceArgs = (DeviceActivatedEventArgs)args;
                if (deviceArgs != null)
                {
                    // The DeviceInformationId is the same id found in a DeviceInformation object, so it can potentially be used
                    // with UsbDevice.FromIdAsync()
                    // The deviceArgs->Verb is the verb that is provided in the appxmanifest for this specific device
                    MainPage.Current.NotifyUser(
                        "The app was launched by device id: " + deviceArgs.DeviceInformationId
                        + "\nVerb: " + deviceArgs.Verb,
                        NotifyType.StatusMessage);
                }
            }
        }
    }
}
