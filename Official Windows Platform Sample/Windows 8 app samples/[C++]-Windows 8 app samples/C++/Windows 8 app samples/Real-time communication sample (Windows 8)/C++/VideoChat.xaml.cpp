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
// Videochat.xaml.cpp
// Implementation of the Videochat class
//

#include "pch.h"
#include "VideoChat.xaml.h"
#include <wrl\wrappers\corewrappers.h>

using namespace SimpleCommunication;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

VideoChat::VideoChat()
    : _device(nullptr)
    , _active(false)
    , _initialization(false)
    , _isTerminator(false)
    , _role(Role::uninitialized)
    , _dispatcher(nullptr)
{
    InitializeComponent();

    _sdkSample.Initialize(StatusMessage, Window::Current->Dispatcher);
    _dispatcher = Window::Current->Dispatcher;
}

VideoChat::~VideoChat()
{
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void VideoChat::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    _rootPage = MainPage::Current;

    _rootPage->LockScenarioChange(); // Lock

    Initialize().then([this](task<void> asyncInfo)
    {
        Windows::Foundation::HResult hr;
        try
        {
            asyncInfo.get();
            hr.Value = S_OK;
        }
        catch (Platform::Exception^ e)
        {
            hr.Value = e->HResult;
        }

        return OnInitializeCompleted(hr).then([this]()
        {
            _rootPage->UnlockScenarioChange(); // Unlock
        });
    }); 
}

void
    VideoChat::OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e)
{
    if (_rootPage->NavigatingPrevented())
    {
        e->Cancel = true;
    }
    else
    {
        RemoteVideo->MediaFailed::remove(_mediaElementFailedEventRegistrationToken);
        _mediaElementFailedEventRegistrationToken.Value = 0;
        Deactivate();
    }
}

void
    VideoChat::OnNavigatedFrom(NavigationEventArgs^ e)
{
    _sdkSample.Deinitialize();
}

void
    VideoChat::CallButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto address = IpAddressTextbox->Text;
    if (address)
    {
        _role = Role::active;
        RemoteVideo->Source = ref new Windows::Foundation::Uri("stsp://" + address);
        RemoteVideo->Play();
        RemoteVideoBorder->Visibility = Windows::UI::Xaml::Visibility::Visible;
        RemoteVideoPosterBorder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        IpAddressTextbox->IsEnabled = false;
        CallButton->IsEnabled = false;
        _sdkSample.DisplayStatus("Initiating connection... Please wait!");
    }
}

void
    VideoChat::EndCallButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    bool callEndCall = false;

    if (!_isTerminator && _active)
    {
        _isTerminator = true;
        callEndCall = true;
        _rootPage->LockScenarioChange(); // Lock
    }

    if (callEndCall)
    {
        EndCall().then([this](task<void>)
        {
            _isTerminator = false;
        });
    }
}

task<void>
    VideoChat::Initialize()
{
    if (_initialization || _device)
    {
        return create_task([](){});
    }
    _initialization = true;
    _active = true;

    return task<DeviceInformationCollection^>(DeviceInformation::FindAllAsync(DeviceClass::VideoCapture)).then([this](DeviceInformationCollection^ devices)
    {
        auto videoCaptures = devices->Size;
        return task<DeviceInformationCollection^>(DeviceInformation::FindAllAsync(DeviceClass::AudioCapture)).then([this,videoCaptures](DeviceInformationCollection^ devices)
        {
            auto audioCaptures = devices->Size;
            return ((videoCaptures > 0) && (audioCaptures) > 0);
        });
    }).then([this](task<bool> asyncInfo)
    {
        bool succeed;
        try
        {
            succeed = asyncInfo.get();
        }
        catch (Platform::Exception^ e)
        {
            Windows::Foundation::HResult hr;
            hr.Value = e->HResult;
            _sdkSample.FormatError(hr, "SimpleCommunication.NoCamera", "A machine with a camera and a microphone is required to run this sample.");
            throw;
        }

        if (succeed)
        {
            CaptureDevice ^device = ref new CaptureDevice();

            return device->InitializeAsync().then([this, device]()
            {
                _device = device;
                _mediaCaptureFailedEventRegistrationToken = _device->Failed::add(ref new CaptureFailedHandler(this, &VideoChat::OnMediaCaptureFailed));
                _incomingConnectionEventRegistrationToken = _device->Incomingconnectionevent::add(ref new IncomingconnectioneventHandler(this, &VideoChat::OnIncomingConnection));
                return StartRecordingToCustomSinkAsync();
            });
        }
        else
        {
            _sdkSample.DisplayError("A machine with a camera and a microphone is required to run this sample.");
            _rootPage->UnlockScenarioChange();
            cancel_current_task();
        }
    });
}

task<void>
    VideoChat::OnInitializeCompleted(Windows::Foundation::HResult hr)
{
    return create_task([this, hr]()
    {        
        if (FAILED(hr.Value))
        {
            _rootPage->UnlockScenarioChange();
            _initialization = false;

            Deactivate();

            _sdkSample.FormatError(hr, "", ref new Platform::String(L"Initialization") + " error: ");
            throw ref new Platform::Exception(hr.Value);
        }

        return task<void>(_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
        {
            CallButton->IsEnabled = true;
            IpAddressTextbox->IsEnabled = true;
            EndCallButton->IsEnabled = false;

            CallButton->IsEnabled = true;
            EndCallButton->IsEnabled = true;
            if (_mediaElementFailedEventRegistrationToken.Value == 0)
            {
                _mediaElementFailedEventRegistrationToken = RemoteVideo->MediaFailed::add(ref new Windows::UI::Xaml::ExceptionRoutedEventHandler(this, &VideoChat::VideoPlaybackErrorHandler));
            }

            // Both side start out as passive
            _role = Role::passive;
            _isTerminator = false;
            _initialization = false;

            _sdkSample.DisplayStatus("Tap 'Call' button to start call");
            _rootPage->SetCurrentScenario("SimpleCommunication::VideoChat");
        }
        )));
    });
}

task<void>
    VideoChat::EndCall()
{
    return CleanUp().then([this]()
    {

        if (_active)
        {
            return Initialize().then([this](task<void> asyncInfo)
            {
                Windows::Foundation::HResult hr;
                try
                {
                    asyncInfo.get();
                    hr.Value = S_OK;
                }
                catch (Platform::Exception^ e)
                {
                    hr.Value = e->HResult;
                }

                return OnInitializeCompleted(hr).then([this](task<void> asyncInfo){});
            });   
        }
        return create_task([](){});
    });
}

task<void>
    VideoChat::Deactivate()
{
    _active = false;

    return CleanUp();
}

task<void>
    VideoChat::CleanUp()
{
    if (_device != nullptr)
    {
        _device->Failed::remove(_mediaCaptureFailedEventRegistrationToken);
        _device->Incomingconnectionevent::remove(_incomingConnectionEventRegistrationToken);
    }
    _mediaCaptureFailedEventRegistrationToken.Value = 0;
    _incomingConnectionEventRegistrationToken.Value = 0;

    return task<void>(_dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        CallButton->IsEnabled = false;
        EndCallButton->IsEnabled = false;
    }
    ))).then([this]()
    {
        if (_device != nullptr)
        {
            auto device = _device;
            _device = nullptr;

            return device->CleanupAsync();
        }

        return create_task([](){});
    }).then([this](task<void>)
    {
        if (!_active)
        {
            return;
        }
        _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
        {
            RemoteVideo->Source = nullptr;
            RemoteVideoBorder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            RemoteVideoPosterBorder->Visibility = Windows::UI::Xaml::Visibility::Visible;
        }));
    });
}

task<void>
    VideoChat::StartRecordingToCustomSinkAsync()
{
    auto mediaEncodingProfile = Windows::Media::MediaProperties::MediaEncodingProfile::CreateMp4(Windows::Media::MediaProperties::VideoEncodingQuality::Qvga);

    mediaEncodingProfile->Video->FrameRate->Numerator = 15;
    mediaEncodingProfile->Video->FrameRate->Denominator = 1;
    mediaEncodingProfile->Container = nullptr;

    return _device->StartRecordingAsync(mediaEncodingProfile);
}

void
    VideoChat::OnIncomingConnection(Microsoft::Samples::SimpleCommunication::StspMediaSink^ sender, Microsoft::Samples::SimpleCommunication::IncomingConnectionEventArgs^ args)
{
    args->Accept();

    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, args]()
    {
        EndCallButton->IsEnabled = true;
        _isTerminator = false;

        Microsoft::WRL::Wrappers::HString url_, header_;
        header_.Set(L"stsp://");
        WindowsTrimStringStart(reinterpret_cast<HSTRING>(args->RemoteUrl), header_.Get(), url_.GetAddressOf());
        auto url = reinterpret_cast<Platform::String^>(url_.Get());

        switch (_role)
        {
        case Role::active:
            _sdkSample.DisplayStatus("Connected. Remote machine address: " + url);
            break;
        case Role::passive:
            {
                auto address = args->RemoteUrl;
                RemoteVideo->Source = ref new Windows::Foundation::Uri(address);
                RemoteVideo->Play();
                RemoteVideoBorder->Visibility = Windows::UI::Xaml::Visibility::Visible;
                RemoteVideoPosterBorder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                _sdkSample.DisplayStatus("Connected. Remote machine address: " + url);
            }
            IpAddressTextbox->IsEnabled = false;
            CallButton->IsEnabled = false;
            break;
        default:
            break;
        }
    }
    ));
}

void
    VideoChat::OnMediaCaptureFailed(CaptureDevice^ sender, CaptureFailedEventArgs^ errorEventArgs)
{
    _sdkSample.FormatError(errorEventArgs, "Capture error code: ");

    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        if (!_isTerminator && _active)
        {
            _isTerminator = true;

            EndCall().then([this](task<void> previousTask)
            {
                _isTerminator = false;
            });
        }
    }));
}

void
    VideoChat::VideoPlaybackErrorHandler(Platform::Object^ sender, Windows::UI::Xaml::ExceptionRoutedEventArgs^ e)
{
    // See MF_MEDIA_ENGINE_ERR
    if (e->ErrorMessage != "MF_MEDIA_ENGINE_ERR_NOERROR")
    {
        _sdkSample.DisplayError(e->ErrorMessage);

        _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
        {
            if (!_isTerminator && _active)
            {
                _isTerminator = true;

                EndCall().then([this](task<void> previousTask)
                {
                    _isTerminator = false;
                });
            }
        }));
    }
}

