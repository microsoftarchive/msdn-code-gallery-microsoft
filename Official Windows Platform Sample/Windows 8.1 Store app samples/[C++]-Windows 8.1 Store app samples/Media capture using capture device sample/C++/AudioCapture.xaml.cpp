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
// AudioCapture.xaml.cpp
// Implementation of the AudioCapture class
//

#include "pch.h"
#include "AudioCapture.xaml.h"
#include <ppl.h>
using namespace concurrency;

using namespace SDKSample::MediaCapture;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::Storage;
using namespace Windows::Media;
using namespace Windows::Media::MediaProperties;
using namespace Windows::Storage::Streams;
using namespace Windows::System;
using namespace Windows::UI::Xaml::Media::Imaging;


AudioCapture::AudioCapture()
{
    InitializeComponent();
    ScenarioInit();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void AudioCapture::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    SystemMediaTransportControls^ systemMediaControls = SystemMediaTransportControls::GetForCurrentView();
    m_eventRegistrationToken = systemMediaControls->PropertyChanged += ref new TypedEventHandler<SystemMediaTransportControls^, SystemMediaTransportControlsPropertyChangedEventArgs^>(this, &AudioCapture::SystemMediaControlsPropertyChanged);
}

void AudioCapture::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    SystemMediaTransportControls^ systemMediaControls = SystemMediaTransportControls::GetForCurrentView();
    systemMediaControls->PropertyChanged -= m_eventRegistrationToken;
    ScenarioClose();
}

void  AudioCapture::ScenarioInit()
{
    try
    {
        rootPage = MainPage::Current;
        btnStartDevice3->IsEnabled = true;
        btnStartStopRecord3->IsEnabled = false;
        m_bRecording = false;
        recordRawAudio->IsChecked = false;
        recordRawAudio->IsEnabled = false;
        m_bUserRequestedRaw = false;
        m_bRawAudioSupported = false;		
        playbackElement3->Source = nullptr;
        m_bSuspended = false;
        ShowStatusMessage("");

        //Read system's raw audio stream support
        String^ deviceId = Windows::Media::Devices::MediaDevice::GetDefaultAudioCaptureId(Windows::Media::Devices::AudioDeviceRole::Communications);
        auto PropertiesToRetrieve = ref new Platform::Collections::Vector<String^>();
        PropertiesToRetrieve->Append("System.Devices.AudioDevice.RawProcessingSupported");
        // read property store to see if the device supports a RAW processing mode and start audio recording accordingly
        create_task(Windows::Devices::Enumeration::DeviceInformation::CreateFromIdAsync(deviceId, PropertiesToRetrieve)).then([this] (Windows::Devices::Enumeration::DeviceInformation^ deviceInformation)
        {
            auto obj = deviceInformation->Properties->Lookup("System.Devices.AudioDevice.RawProcessingSupported");
            auto bDeviceSupportsRaw = false;
            if (obj) bDeviceSupportsRaw = obj->Equals(true);
            return bDeviceSupportsRaw;
        }).then([this](bool bDeviceSupportsRaw)
        {
            this->m_bRawAudioSupported = bDeviceSupportsRaw;
            if(this->m_bRawAudioSupported)
            {
                this->recordRawAudio->IsEnabled = true;
                this->ShowStatusMessage("Raw audio recording is supported");
            }
            else
            {
                this->ShowStatusMessage("Raw audio recording is not supported");
            }
        });

    }
    catch (Exception ^e)
    {
        ShowExceptionMessage(e);
    }

}

void AudioCapture::ScenarioClose()
{

    if (m_bRecording)
    {
        ShowStatusMessage("Stopping Record and Microphone");

        create_task(m_mediaCaptureMgr->StopRecordAsync()).then([this](task<void> recordTask)
        {
            try
            {
                recordTask.get();
                m_bRecording = false;

                delete(m_mediaCaptureMgr.Get());

            }catch (Exception ^e)
            {
                ShowExceptionMessage(e);
            }
        });
    }else
    {
        ShowStatusMessage("Stopping Microphone");
        try
        {
            if (m_mediaCaptureMgr.Get())
            {
                delete(m_mediaCaptureMgr.Get());
            }
        }
        catch (Exception ^e)
        {
            ShowExceptionMessage(e);
        }
    }

}


void AudioCapture::SystemMediaControlsPropertyChanged(SystemMediaTransportControls^ sender, SystemMediaTransportControlsPropertyChangedEventArgs^ e)
{
    switch (e->Property)
    {
    case SystemMediaTransportControlsProperty::SoundLevel:
        create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High, ref new Windows::UI::Core::DispatchedHandler([this, sender]()
        {

            if(sender->SoundLevel != SoundLevel::Muted)
            {
                ScenarioInit();
            }
            else
            {
                ScenarioClose();
            }
        })));
        break;

    default:
        break;
    }
}

void AudioCapture::RecordLimitationExceeded(Windows::Media::Capture::MediaCapture ^currentCaptureObject)
{
    try
    {
        if (m_bRecording)
        {
            create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High, ref new Windows::UI::Core::DispatchedHandler([this](){
                try
                {
                    ShowStatusMessage("Stopping Record on exceeding max record duration");
                    EnableButton(false, "StartStopRecord");
                    create_task(m_mediaCaptureMgr->StopRecordAsync()).then([this](task<void> recordTask)
                    {
                        try
                        {
                            recordTask.get();
                            m_bRecording = false;
                            SwitchRecordButtonContent();
                            EnableButton(true, "StartStopRecord");
                            ShowStatusMessage("Stopped record on exceeding max record duration:" + m_recordStorageFile->Path);
                        }
                        catch (Exception ^e)
                        {
                            ShowExceptionMessage(e);
                            m_bRecording = false;
                            SwitchRecordButtonContent();
                            EnableButton(true, "StartStopRecord");
                        }
                    });

                }
                catch (Exception ^e)
                {
                    m_bRecording = false;
                    SwitchRecordButtonContent();
                    EnableButton(true, "StartStopRecord");
                    ShowExceptionMessage(e);
                }

            })));
        }
    }
    catch (Exception ^e)
    {
        m_bRecording = false;
        SwitchRecordButtonContent();
        EnableButton(true, "StartStopRecord");
        ShowExceptionMessage(e);
    }
}

void AudioCapture::Failed(Windows::Media::Capture::MediaCapture ^currentCaptureObject, Windows::Media::Capture::MediaCaptureFailedEventArgs^ currentFailure)
{
    String ^message = "Fatal error: " + currentFailure->Message;
    create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High, 
        ref new Windows::UI::Core::DispatchedHandler([this, message]()
    {
        ShowStatusMessage(message);
    })));
}

void AudioCapture::startAudioCapture()
{
    auto mediaCapture = ref new Windows::Media::Capture::MediaCapture();
    m_mediaCaptureMgr = mediaCapture;
    auto settings = ref new Windows::Media::Capture::MediaCaptureInitializationSettings();
    settings->StreamingCaptureMode = Windows::Media::Capture::StreamingCaptureMode::Audio;
    settings->MediaCategory = Windows::Media::Capture::MediaCategory::Other;
    settings->AudioProcessing = (m_bRawAudioSupported && m_bUserRequestedRaw) ? Windows::Media::AudioProcessing::Raw : Windows::Media::AudioProcessing::Default;

    create_task(mediaCapture->InitializeAsync()).then([this](task<void> initTask)
    {
        try
        {
            initTask.get();
            auto mediaCapture = m_mediaCaptureMgr.Get();
            EnableButton(true, "StartPreview");
            EnableButton(true, "StartStopRecord");
            EnableButton(true, "TakePhoto");
            ShowStatusMessage("Device initialized successfully");
            mediaCapture->RecordLimitationExceeded += ref new Windows::Media::Capture::RecordLimitationExceededEventHandler(this, &AudioCapture::RecordLimitationExceeded);
            mediaCapture->Failed += ref new Windows::Media::Capture::MediaCaptureFailedEventHandler(this, &AudioCapture::Failed);
        }
        catch (Exception ^ e)
        {
            ShowExceptionMessage(e);
        }
    });
}

void AudioCapture::btnStartDevice_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    m_bUserRequestedRaw = recordRawAudio->IsChecked->Value ? true : false;
    recordRawAudio->IsEnabled = false;
    EnableButton(false, "StartDevice");
    ShowStatusMessage("Starting device");
    startAudioCapture();
}

void AudioCapture::btnStartStopRecord_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        String ^fileName;
        EnableButton(false, "StartStopRecord");

        if (!m_bRecording)
        {
            ShowStatusMessage("Starting Record");

            fileName = AUDIO_FILE_NAME;

            task<StorageFile^>(KnownFolders::VideosLibrary->CreateFileAsync(fileName, Windows::Storage::CreationCollisionOption::GenerateUniqueName)).then([this](task<StorageFile^> fileTask)
            {
                try
                {
                    this->m_recordStorageFile = fileTask.get();
                    ShowStatusMessage("Create record file successful");

                    MediaEncodingProfile^ recordProfile= nullptr;
                    recordProfile = MediaEncodingProfile::CreateM4a(Windows::Media::MediaProperties::AudioEncodingQuality::Auto);

                    create_task(m_mediaCaptureMgr->StartRecordToStorageFileAsync(recordProfile, this->m_recordStorageFile)).then([this](task<void> recordTask)
                    {
                        try
                        {
                            recordTask.get();
                            m_bRecording = true;
                            SwitchRecordButtonContent();
                            EnableButton(true, "StartStopRecord");

                            ShowStatusMessage("Start Record successful");


                        }catch (Exception ^e)
                        {
                            ShowExceptionMessage(e);
                            m_bRecording = false;
                            SwitchRecordButtonContent();
                            EnableButton(true, "StartStopRecord");
                        }
                    });
                }
                catch (Exception ^e)
                {
                    m_bRecording = false;
                    SwitchRecordButtonContent();
                    EnableButton(true, "StartStopRecord");
                    ShowExceptionMessage(e);
                }
            }
            );
        }
        else
        {
            ShowStatusMessage("Stopping Record");

            create_task(m_mediaCaptureMgr->StopRecordAsync()).then([this](task<void>)
            {
                try
                {
                    m_bRecording = false;
                    EnableButton(true, "StartStopRecord");
                    SwitchRecordButtonContent();

                    ShowStatusMessage("Stop record successful");
                    if (!m_bSuspended)
                    {
                        task<IRandomAccessStream^>(this->m_recordStorageFile->OpenAsync(FileAccessMode::Read)).then([this](task<IRandomAccessStream^> streamTask)
                        {
                            try
                            {
                                ShowStatusMessage("Record file opened");
                                auto stream = streamTask.get();
                                ShowStatusMessage(this->m_recordStorageFile->Path);
                                playbackElement3->AutoPlay = true;
                                playbackElement3->SetSource(stream, this->m_recordStorageFile->FileType);
                                playbackElement3->Play();
                            }
                            catch (Exception ^e)
                            {
                                ShowExceptionMessage(e);
                                m_bRecording = false;
                                SwitchRecordButtonContent();
                                EnableButton(true, "StartStopRecord");
                            }
                        });
                    }
                }
                catch (Exception ^e)
                {
                    m_bRecording = false;
                    SwitchRecordButtonContent();
                    EnableButton(true, "StartStopRecord");
                    ShowExceptionMessage(e);
                }
            });
        }
    }
    catch (Platform::Exception^ e)
    {
        EnableButton(true, "StartStopRecord");
        ShowExceptionMessage(e);
        m_bRecording = false;
        SwitchRecordButtonContent();
    }
}


void AudioCapture::ShowStatusMessage(Platform::String^ text)
{
    rootPage->NotifyUser(text, NotifyType::StatusMessage);
}

void AudioCapture::ShowExceptionMessage(Platform::Exception^ ex)
{
    rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
}

void AudioCapture::SwitchRecordButtonContent()
{
    {
        if (m_bRecording)
        {
            btnStartStopRecord3->Content="StopRecord";
        }
        else
        {
            btnStartStopRecord3->Content="StartRecord";
        }
    }
}
void AudioCapture::EnableButton(bool enabled, String^ name)
{
    if (name->Equals("StartDevice"))
    {
        btnStartDevice3->IsEnabled = enabled;
    }

    else if (name->Equals("StartStopRecord"))
    {
        btnStartStopRecord3->IsEnabled = enabled;
    }

}

