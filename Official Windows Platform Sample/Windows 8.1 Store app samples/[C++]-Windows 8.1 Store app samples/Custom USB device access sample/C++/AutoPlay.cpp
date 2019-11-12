//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "AutoPlay.h"

using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace SDKSample;
using namespace SDKSample::CustomUsbDeviceAccess;

/// <summary>
/// Invoked when the application is activated.
/// This is the entry point for Device Autoplay when a device is attached to the PC.
/// Other activation kinds (such as search and protocol activation) may also be handled here.
///
/// This code was adapted from the "Removable storage sample" on MSDN.
/// </summary>
/// <param name="args">Details about the activation request.</param>
void App::OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ args)
{
    if (args->Kind == ActivationKind::Device)
    {
        // Load the UI
        if (Window::Current->Content == nullptr)
        {
            Frame^ rootFrame = ref new Frame();
            TypeName pageType = { "SDKSample.MainPage", TypeKind::Custom };
            rootFrame->Navigate(pageType);

            // Place the frame in the current Window
            Window::Current->Content = rootFrame;
        }

        // Ensure the current window is active or else the app will freeze at the splash screen
        Window::Current->Activate();

        // Launched from Autoplay for device, notify the app what device launched this app
        DeviceActivatedEventArgs^ deviceArgs = static_cast<DeviceActivatedEventArgs^>(args);
        if (deviceArgs != nullptr)
        {
            // The DeviceInformationId is the same id found in a DeviceInformation object, so it can be used
            // with UsbDevice.FromIdAsync()
            // The deviceArgs->Verb is the verb that is provided in the appxmanifest for this specific device
            MainPage::Current->NotifyUser(
                "The app was launched by device id: " + deviceArgs->DeviceInformationId
                + "\nVerb: " + deviceArgs->Verb,
                NotifyType::StatusMessage);
        }
    }
}
