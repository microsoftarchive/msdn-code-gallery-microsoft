//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// ShowOptionsUI.xaml.cpp
// Implementation of the ShowOptionsUI class
//

#include "pch.h"
#include "ShowOptionsUI.xaml.h"

using namespace SDKSample::CameraOptions;

using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Media;
using namespace Windows::Media::Capture;
using namespace Windows::Foundation;
using namespace Windows::Devices::Enumeration;

ShowOptionsUI::ShowOptionsUI()
{
    InitializeComponent();
    previewStarted = false;
    ShowSettings->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    mediaCaptureMgr = nullptr;
    dispatcher = Window::Current->Dispatcher;
    // Using the SoundLevelChanged event to determine when the app can stream from the webcam
    MediaControl::SoundLevelChanged += ref new EventHandler<Object^>(this, &ShowOptionsUI::MediaControl_SoundLevelChanged);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ShowOptionsUI::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CameraOptions::ShowOptionsUI::StartPreview_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Check if the machine has a webcam
    concurrency::task<DeviceInformationCollection^>(DeviceInformation::FindAllAsync(DeviceClass::VideoCapture))
        .then([this](concurrency::task<DeviceInformationCollection^> getDevicesTask)
    {
        try
        {
            DeviceInformationCollection^ devices = getDevicesTask.get();
            if (devices->Size > 0)
            {
                rootPage->NotifyUser("", NotifyType::ErrorMessage);

                if (mediaCaptureMgr == nullptr)
                {
                    // Using Windows::Media::Capture::MediaCapture APIs to stream from Webcam
                    mediaCaptureMgr = ref new Windows::Media::Capture::MediaCapture();

                    concurrency::task<void>(mediaCaptureMgr->InitializeAsync()).then([this] ()
                    {
                        VideoStream->Source = mediaCaptureMgr.Get();
                        return mediaCaptureMgr->StartPreviewAsync();
                    })
                        .then([this](concurrency::task<void> previewTask)
                    {
                        try
                        {
                            previewTask.get();
                            previewStarted = true;
                            ShowSettings->Visibility = Windows::UI::Xaml::Visibility::Visible;
                            StartPreview->IsEnabled = false;
                        }
                        catch (Platform::Exception^ ex)
                        {
                            mediaCaptureMgr = nullptr;
                            rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
                        }
                    });
                }
            }
            else
            {
                rootPage->NotifyUser("A webcam is required to run this sample.", NotifyType::ErrorMessage);
            }
        }
        catch (Platform::Exception^ ex)
        {
            mediaCaptureMgr = nullptr;
            rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
        }
    });
}

void SDKSample::CameraOptions::ShowOptionsUI::ShowSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        if (mediaCaptureMgr.Get() != nullptr)
        {
            // Using Windows::Media::Capture::MediaCapture APIs to show webcam settings
            CameraOptionsUI::Show(mediaCaptureMgr.Get());
        }
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
    }
}

void SDKSample::CameraOptions::ShowOptionsUI::MediaControl_SoundLevelChanged(Object^ sender, Object^ e)
{
    // The callbacks for MediaControl_SoundLevelChanged and StopPreviewAsync may be invoked on threads other
    // than the UI thread, so to ensure there's no synchronization issue, the Dispatcher is used here to
    // ensure all operations run on the UI thread.
    dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this] ()
    {
        if (MediaControl::SoundLevel == Windows::Media::SoundLevel::Muted)
        {
            if (previewStarted)
            {
                concurrency::task<void>(mediaCaptureMgr->StopPreviewAsync()).then([this] ()
                {
                    mediaCaptureMgr = nullptr;
                    previewStarted = false;
                    VideoStream->Source = nullptr;
                });
            }
        }
        else
        {
            if (!previewStarted)
            {
                ShowSettings->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                StartPreview->IsEnabled = true;
            }
        }           
    }));
}
