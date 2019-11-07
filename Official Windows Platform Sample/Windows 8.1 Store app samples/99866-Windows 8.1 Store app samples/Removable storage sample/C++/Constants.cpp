//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;

using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "List removable storage devices", "SDKSample.RemovableStorageCPP.S1_ListStorages" },
    { "Send file to storage device", "SDKSample.RemovableStorageCPP.S2_SendToStorage" },
    { "Get image file from storage device", "SDKSample.RemovableStorageCPP.S3_GetFromStorage" },
    { "Get image file from camera or camera memory (Autoplay)", "SDKSample.RemovableStorageCPP.S4_Autoplay" }
};

int MainPage::autoplayScenarioIndex = 3;

/// <summary>
/// Invoked when the application is launched to open a specific file or to access
/// specific content. This is the entry point for Content Autoplay when camera
/// memory is attached to the PC.
/// </summary>
/// <param name="args">Details about the file activation request.</param>
void App::OnFileActivated(Windows::ApplicationModel::Activation::FileActivatedEventArgs^ args)
{
    Frame^ rootFrame = nullptr;
    if (Window::Current->Content == nullptr)
    {
        rootFrame = ref new Frame();
        TypeName pageType = { "SDKSample.MainPage", TypeKind::Custom };
        rootFrame->Navigate(pageType);
        Window::Current->Content = rootFrame;
    }
    else
    {
        rootFrame = safe_cast<Frame^>(Window::Current->Content);
    }
    Window::Current->Activate();
    MainPage^ mainPage = safe_cast<MainPage^>(rootFrame->Content);

    // Clear any device id so we always use the latest connected device
    mainPage->AutoplayNonFileSystemDeviceId = nullptr;

    if (args->Verb == "storageDevice")
    {
        // Launched from Autoplay for content. This will return a single storage folder
        // representing that file system device.
        mainPage->AutoplayFileSystemDeviceFolder = safe_cast<StorageFolder^>(args->Files->GetAt(0));
        mainPage->FileActivationFiles = nullptr;
    }
    else
    {
        // Launched to handle a file type.  This will return a list of image files that the user
        // requests for this application to handle.
        mainPage->FileActivationFiles = args->Files;
        mainPage->AutoplayFileSystemDeviceFolder = nullptr;
    }

    // Select the Autoplay scenario
    mainPage->LoadAutoplayScenario();
}

/// <summary>
/// Invoked when the application is activated.
/// This is the entry point for Device Autoplay when a device is attached to the PC.
/// Other activation kinds (such as search and protocol activation) may also be handled here.
/// </summary>
/// <param name="args">Details about the activation request.</param>
void App::OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ args)
{
    if (args->Kind == ActivationKind::Device)
    {
        Frame^ rootFrame = nullptr;
        if (Window::Current->Content == nullptr)
        {
            rootFrame = ref new Frame();
            TypeName pageType = { "SDKSample.MainPage", TypeKind::Custom };
            rootFrame->Navigate(pageType);
            Window::Current->Content = rootFrame;
        }
        else
        {
            rootFrame = safe_cast<Frame^>(Window::Current->Content);
        }
        Window::Current->Activate();
        MainPage^ mainPage = safe_cast<MainPage^>(rootFrame->Content);

        // Launched from Autoplay for device, receiving the device information identifier.
        DeviceActivatedEventArgs^ deviceArgs = dynamic_cast<DeviceActivatedEventArgs^>(args);
        if (deviceArgs != nullptr)
        {
            mainPage->AutoplayNonFileSystemDeviceId = deviceArgs->DeviceInformationId;

            // Clear any saved drive or file so we always use the latest connected device
            mainPage->AutoplayFileSystemDeviceFolder = nullptr;
            mainPage->FileActivationFiles = nullptr;

            // Select the Autoplay scenario
            mainPage->LoadAutoplayScenario();
        }
    }
}
