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
// LowLatency.xaml.cpp
// Implementation of the LowLatency class
//

#include "pch.h"
#include "LowLatency.xaml.h"

using namespace SimpleCommunication;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace concurrency;

enum Action
{
    previewButtonClicked,
    localClientButtonClicked,
    previewSetupCompleted,
    recordingStarted,
    latencyModeToggled,
};

LowLatency::LowLatency()
    : _device(nullptr)
    , _currentState(ScenarioOneStates::uninitialized)
    , _previousState(ScenarioOneStates::uninitialized)
    , _latencyMode(LatencyModes::uninitialized)
    , _initialized(false)
    , _videoSettingsFilter(nullptr)
{
    InitializeComponent();

    _sdkSample.Initialize(StatusMessage, Window::Current->Dispatcher);
    _dispatcher = Window::Current->Dispatcher;

    _localHostVideo[LatencyModes::highLatency] = PlaybackVideo;
    _localHostVideo[LatencyModes::lowLatency] = RealTimePlaybackVideo;

    _streamFilteringCriteria.aspectRatio = "1.333333333333333";
    _streamFilteringCriteria.horizontalResolution = 640;
    _streamFilteringCriteria.subType = "YUY2";

    actionTransitionFunctions[ScenarioOneStates::initializing][Action::previewSetupCompleted] = ref new ActionTransition(this, &LowLatency::initializing_previewSetupCompleted);
    actionTransitionFunctions[ScenarioOneStates::waiting][Action::previewButtonClicked] = ref new ActionTransition(this, &LowLatency::waiting_previewButtonClicked);
    actionTransitionFunctions[ScenarioOneStates::previewing][Action::previewButtonClicked] = ref new ActionTransition(this, &LowLatency::previewing_previewButtonClicked);
    actionTransitionFunctions[ScenarioOneStates::previewing][Action::localClientButtonClicked] = ref new ActionTransition(this, &LowLatency::previewing_localClientButtonClicked);
    actionTransitionFunctions[ScenarioOneStates::previewing][Action::recordingStarted] = ref new ActionTransition(this, &LowLatency::previewing_recordingStarted);
    actionTransitionFunctions[ScenarioOneStates::streaming][Action::previewButtonClicked] = ref new ActionTransition(this, &LowLatency::streaming_previewButtonClicked);
    actionTransitionFunctions[ScenarioOneStates::streaming][Action::latencyModeToggled] = ref new ActionTransition(this, &LowLatency::streaming_latencyModeToggled);
    actionTransitionFunctions[ScenarioOneStates::streaming][Action::recordingStarted] = ref new ActionTransition(this, &LowLatency::streaming_recordingStarted);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void
    LowLatency::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    _rootPage = MainPage::Current;

    if (_initialized)
    {
        InitializeUIControls();
        _rootPage->SetCurrentScenario("SimpleCommunication::LowLatency");
        return;
    }
    else
        if (_currentState == ScenarioOneStates::initializing || _device)
        {
            // We are already in intializing state.
            return;
        }

        _rootPage->LockScenarioChange();

        _sdkSample.DisplayStatus("Loading...Please wait.");

        // Check for camera and init scenario if one is found
        task<DeviceInformationCollection^>(DeviceInformation::FindAllAsync(DeviceClass::VideoCapture)).then([this](DeviceInformationCollection^ devices)
        {
            return devices->Size > 0;
        }).then([this](bool success)
        {
            if (success)
            {
                if (_currentState == ScenarioOneStates::initializing || _device)
                {
                    // We are already in intializing state.
                    cancel_current_task();
                }
                else
                {
                    return Initialize().then([this](task<void> result)
                    {
                        try
                        {
                            result.get();
                            return task<void>(_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
                            {
                                InitializeUIControls();
                                _rootPage->UnlockScenarioChange();
                                _initialized = true;
                                _rootPage->SetCurrentScenario("SimpleCommunication::LowLatency");

                                m_visibilityEventRegistrationToken = Window::Current->VisibilityChanged += ref new Windows::UI::Xaml::WindowVisibilityChangedEventHandler(this, &LowLatency::VisibilityChanged);
                            })));
                        }
                        catch (Platform::Exception^ e)
                        {
                            Windows::Foundation::HResult hr;
                            hr.Value = e->HResult;
                            _sdkSample.FormatError(hr, "Initialization error: ", "");
                            _rootPage->UnlockScenarioChange();
                            throw e;
                        }
                    });
                }
            }
            else
            {
                _sdkSample.DisplayError("A machine with a camera is required to run this sample.");
                _rootPage->UnlockScenarioChange();
                cancel_current_task();
            }
        });
}

void
    LowLatency::InitializeUIControls()
{
    PreviewVideo->FlowDirection = Windows::UI::Xaml::FlowDirection::RightToLeft;

    PlaybackVideo->AutoPlay = true;
    PlaybackVideo->FlowDirection = Windows::UI::Xaml::FlowDirection::RightToLeft;

    RealTimePlaybackVideo->AutoPlay = true;
    RealTimePlaybackVideo->FlowDirection = Windows::UI::Xaml::FlowDirection::RightToLeft;

    _latencyMode = LatencyModes::highLatency;
    LatencyModeToggleSwitch->IsOn = (_latencyMode == LatencyModes::lowLatency);
};

void
    LowLatency::OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e)
{
    if (_rootPage->NavigatingPrevented())
    {
        e->Cancel = true;
    }

    Window::Current->VisibilityChanged -= m_visibilityEventRegistrationToken;
}

void
    LowLatency::OnNavigatedFrom(NavigationEventArgs^ e)
{
    _sdkSample.Deinitialize();
    this->Cleanup();
    for (auto iter = _localHostVideo.begin(); iter != _localHostVideo.end(); iter ++)
    {
        auto video = iter->second;
        if (video)
        {
            video->Source = nullptr;
        }
    }
    Reset();
}

/// <summary>Handler for the camera 'failed' event</summary>
void
    LowLatency::OnMediaCaptureFailed(CaptureDevice^ sender, CaptureFailedEventArgs^ errorEventArgs)
{
    _sdkSample.FormatError(errorEventArgs, "Capture engine error code: ");
}

void
    LowLatency::OnIncomingConnection(Microsoft::Samples::SimpleCommunication::StspMediaSink^ sender, Microsoft::Samples::SimpleCommunication::IncomingConnectionEventArgs^ args)
{
    args->Accept();
}

void LowLatency::VisibilityChanged(Object^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ e)
{
    if (e->Visible == true)
    {
        Initialize();
    }
    else
    {
        Cleanup();
    }
}


/// <summary>Initializes the scenario</summary>
task<void>
    LowLatency::Initialize()
{
    _ASSERT(_currentState != ScenarioOneStates::initializing);

    _currentState = ScenarioOneStates::initializing;
    CaptureDevice ^device = ref new CaptureDevice();


    return device->InitializeAsync().then([this, device](task<void> asyncInfo)
    {
        try
        {
            asyncInfo.get();
        }
        catch (Platform::Exception^ e)
        {
            _rootPage->UnlockScenarioChange();
            Windows::Foundation::HResult hr;
            hr.Value = e->HResult;
            _sdkSample.FormatError(hr, "Initialization error: ", "CaptureManager::InitializeAsync");
            _currentState = ScenarioOneStates::end;
            if (_device)
            {
                _device->CleanupAsync().then([this](){});
                _device = nullptr;
            }
            throw e;
        }

        _device = device;
        if (!_videoSettingsFilter)
        {
            _videoSettingsFilter = ref new VideoSettingsFilter([this](Windows::Media::MediaProperties::IMediaEncodingProperties^ encodingProp, unsigned int index)
            {
                if (encodingProp->Type != L"Video")
                {
                    return false;
                }

                auto videoProp = static_cast<Windows::Media::MediaProperties::IVideoEncodingProperties^>(encodingProp);
                if (videoProp->Height != 0 && videoProp->Width != 0)
                {
                    return (videoProp->Width == _streamFilteringCriteria.horizontalResolution &&
                        videoProp->Subtype == _streamFilteringCriteria.subType);
                }
                return false;
            });
        }

        return device->SelectPreferredCameraStreamSettingAsync(Windows::Media::Capture::MediaStreamType::VideoPreview, _videoSettingsFilter).then([this](task<void> asyncInfo)
        {
            try
            {
                asyncInfo.get();
            }
            catch (Platform::Exception^ e)
            {
                Windows::Foundation::HResult hr;
                hr.Value = e->HResult;
                _sdkSample.FormatError(hr, "Initialization error: ", "CaptureDevice.SelectPreferredCameraStreamSettingAsync(): ");
                _currentState = ScenarioOneStates::end;
                if (_device)
                {
                    _device->CleanupAsync().then([this](){});
                    _device = nullptr;
                }
                throw e;
            }

            return task<void>(actionTransitionFunctions[ScenarioOneStates::initializing][Action::previewSetupCompleted]());
        });
    });
}

task<void>
    LowLatency::StartRecordingToCustomSink()
{
    //// Use the MP4 preset to an obtain H.264 video encoding profile
    auto mediaEncodingProfile = Windows::Media::MediaProperties::MediaEncodingProfile::CreateMp4(Windows::Media::MediaProperties::VideoEncodingQuality::Vga);
    mediaEncodingProfile->Audio = nullptr;
    mediaEncodingProfile->Container = nullptr;

    return _device->StartRecordingAsync(mediaEncodingProfile).then([this](task<void> asyncInfo)
    {
        try
        {
            asyncInfo.get();
        }
        catch (Platform::Exception^ e)
        {
            auto prefix = _currentState.ToString();
            Windows::Foundation::HResult hr;
            hr.Value = e->HResult;
            _sdkSample.FormatError(hr, ": Local streaming initialization error: ", prefix);
            _currentState = ScenarioOneStates::end;
            throw e;
        }

        return task<void>(actionTransitionFunctions[_currentState][Action::recordingStarted]());
    });
}

/// <summary>Cleans up.</summary>
task<void>
    LowLatency::Cleanup()
{
    _previousState = _currentState;

    if (!_device)
    {
        return create_task([]()
        {
        });
    }

    Reset();
    PreviewVideo->Source = nullptr; 

    _device->Failed::remove(_mediaCaptureFailedEventRegistrationToken);
    _device->Incomingconnectionevent::remove(_incomingConnectionEventRegistrationToken);
    return _device->CleanupAsync().then([this]()
    {
        _device = nullptr;
        _currentState = ScenarioOneStates::end;
    });
}

void
    LowLatency::PreviewButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    _rootPage->LockScenarioChange(); // Lock

    _sdkSample.DisplayStatus("");
    task<void>(actionTransitionFunctions[_currentState][Action::previewButtonClicked]()).then([this]()
    {
        _rootPage->UnlockScenarioChange(); // Unlock
    }); 
}

void
    LowLatency::LocalClientButtonClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    _rootPage->LockScenarioChange(); // Lock

    _sdkSample.DisplayStatus("");
    PreviewButton->IsEnabled = false;

    task<void>(actionTransitionFunctions[ScenarioOneStates::previewing][Action::localClientButtonClicked]()).then([this]()
    {
        _dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
        {
            UpdateLocalClientWindow();
            _rootPage->UnlockScenarioChange(); // Unlock
        }
        ));
    }); 
}

void
    LowLatency::LatencyModeToggled(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (!LatencyModeToggleSwitch->IsEnabled)
    {
        // This is not a user operation.
        return;
    }

    _rootPage->LockScenarioChange(); // Lock

    _sdkSample.DisplayStatus("");
    PreviewButton->IsEnabled = false;
    LatencyModeToggleSwitch->IsEnabled = false;

    task<void>(actionTransitionFunctions[ScenarioOneStates::streaming][Action::latencyModeToggled]()).then([this]()
    {
        _dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
        {
            UpdateLocalClientWindow();
            _rootPage->UnlockScenarioChange(); // Unlock
        }));
    }); 
}

void
    LowLatency::UpdateLocalClientWindow()
{
    PreviewButton->IsEnabled = true;
    ClientButton->IsEnabled = false;
    LatencyModeToggleSwitch->IsEnabled = true;

    WebcamPlaybackPoster->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

    if (_latencyMode == LatencyModes::highLatency)
    {
        RealTimePlaybackVideo->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        PlaybackVideo->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
    else
    {
        PlaybackVideo->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        RealTimePlaybackVideo->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
}

void
    SimpleCommunication::LowLatency::Reset()
{
    _sdkSample.ClearLastStatus();
    PreviewVideo->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    RealTimePlaybackVideo->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    WebcamPreviewPoster->Visibility = Windows::UI::Xaml::Visibility::Visible;
    WebcamPlaybackPoster->Visibility = Windows::UI::Xaml::Visibility::Visible;

    PreviewButton->Content = "Start Preview";
    ClientButton->IsEnabled = false;
    _latencyMode = LatencyModes::highLatency;
    LatencyModeToggleSwitch->IsEnabled = false;
    LatencyModeToggleSwitch->IsOn = (_latencyMode == LatencyModes::lowLatency);

    if (_currentState == ScenarioOneStates::end)
    {
        PreviewButton->IsEnabled = false;
    }
}

//initializing
IAsyncAction^
    LowLatency::initializing_previewSetupCompleted()
{
    return create_async([this]() -> task<void>
    {      
        _mediaCaptureFailedEventRegistrationToken = _device->Failed::add(ref new CaptureFailedHandler(this, &LowLatency::OnMediaCaptureFailed));
        _incomingConnectionEventRegistrationToken = _device->Incomingconnectionevent::add(ref new IncomingconnectioneventHandler(this, &LowLatency::OnIncomingConnection));
        _previousState = _currentState;
        _currentState = ScenarioOneStates::waiting; 

        return task<void>(_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
        {    
            PreviewButton->IsEnabled = true;
            _sdkSample.DisplayStatus("Done. Tap or click 'Start Preview' button to start webcam preview");
        })));
    });
}

//waiting
/// <summary>
/// Logic when preview button is clicked and the scenario is in "waiting" state.
/// </summary>
IAsyncAction^
    LowLatency::waiting_previewButtonClicked()
{
    return create_async([this]() -> task<void>
    {    
        return task<void>(_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
        {
            if (_previousState == ScenarioOneStates::initializing)
            {
                PreviewVideo->Source = _device->MediaCapture.Get();
            }
        }
        ))).then([this]()
        {
            return task<void>(_device->MediaCapture.Get()->StartPreviewAsync()).then([this]()
            {
                _previousState = _currentState;
                _currentState = ScenarioOneStates::previewing;
                return _dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
                {
                    WebcamPreviewPoster->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                    PreviewVideo->Visibility = Windows::UI::Xaml::Visibility::Visible;
                    PreviewButton->Content = "Stop Preview";
                    ClientButton->IsEnabled = true;
                }));
            });
        });
    });
}

//previewing
/// <summary>
/// The logic when the "stop" preview button is clicked while the scenario is in "previewing" state.
/// </summary>
IAsyncAction^
    LowLatency::previewing_previewButtonClicked()
{
    return create_async([this]() -> task<void>
    {    
        return task<void>(_device->MediaCapture->StopPreviewAsync()).then([this]()
        {
            if (ScenarioOneStates::previewing == _currentState)
            {
                _previousState = _currentState;
                _currentState = ScenarioOneStates::waiting;
            }
            return _dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
            {
                Reset();
            }));
        });
    });
}

IAsyncAction^
    LowLatency::previewing_localClientButtonClicked()
{
    return create_async([this]() -> task<void>
    {   
        return StartRecordingToCustomSink();
    });
}

IAsyncAction^
    LowLatency::previewing_recordingStarted()
{
    return create_async([this]() -> task<void>
    {
        return task<void>(_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
        {
            _localHostVideo[_latencyMode]->Source = ref new Uri("stsp://localhost");
            _previousState = _currentState;
            _currentState = ScenarioOneStates::streaming;
        })));
    });
}

//streaming
/// <summary>
/// The logic when the "stop" preview button is clicked while the scenario is in "streaming" state.
/// </summary>
IAsyncAction^
    LowLatency::streaming_previewButtonClicked()
{
    return create_async([this]() -> task<void>
    {
        return task<void>(_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
        {
            //// stop localhost client video
            auto currLocalHostVideo = _localHostVideo[_latencyMode];
            currLocalHostVideo->Pause();
        }
        ))).then([this]()
        {
            LatencyModes latencyMode = _latencyMode;

            //// stop streaming to network and close network sink
            return _device->StopRecordingAsync().then([this, latencyMode](task<void> asyncInfo)
            {
                try
                {
                    asyncInfo.get();
                }
                catch (Platform::Exception^ e)
                {
                    Windows::Foundation::HResult hr;
                    hr.Value = e->HResult;
                    _sdkSample.FormatError(hr, "StopRecordAsync error: ", "");
                    _currentState = ScenarioOneStates::end;
                    throw e;
                }

                return task<void>(_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, asyncInfo, latencyMode]()
                {
                    _localHostVideo[latencyMode]->Source = nullptr;
                    _latencyMode = LatencyModes::highLatency;
                }
                ))).then([this]()
                {
                    return task<void>(actionTransitionFunctions[ScenarioOneStates::previewing][Action::previewButtonClicked]()).then([this]()
                    {
                        _previousState = _currentState;
                        _currentState = ScenarioOneStates::waiting;
                    });                        
                });
            });                   
        });
    });
}

IAsyncAction^
    LowLatency::streaming_latencyModeToggled()
{
    return create_async([this]() -> task<void>
    {
        if (!_device)
        {
            throw ref new Platform::Exception(E_NOINTERFACE);
        }

        return _device->StopRecordingAsync().then([this](task<void> asyncInfo)
        {
            try
            {
                asyncInfo.get();
            }
            catch (Platform::Exception^ e)
            {
                Windows::Foundation::HResult hr;
                hr.Value = e->HResult;
                _sdkSample.FormatError(hr, "Latency toggle error: ", "StopRecordingAsync: ");
                _currentState = ScenarioOneStates::end;
                throw e;
            }

            return task<void>(_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
            {
                _localHostVideo[_latencyMode]->Source = nullptr;
            }
            )));
        }
        ).then([this]()
        {
            _latencyMode = (_latencyMode == LatencyModes::highLatency) ?
                LatencyModes::lowLatency : LatencyModes::highLatency;

            return StartRecordingToCustomSink();                          
        });
    });
}

/// <summary>
/// Logic when the scenario is in "streaming" state and "recordingStarted"
/// event is triggered after toggling latency mode.
/// </summary>
IAsyncAction^
    LowLatency::streaming_recordingStarted()
{
    return create_async([this]() -> task<void>
    {
        return task<void>(_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
        {
            _localHostVideo[_latencyMode]->Source = ref new Uri("stsp://localhost");
            _localHostVideo[_latencyMode]->Play();
        })));
    });
}
