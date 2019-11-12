//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2_UseDirectXForVideoStream.xaml.cpp
// Implementation of the Scenario2_UseDirectXForVideoStream class
//

#include "pch.h"
#include "Scenario2_UseDirectXForVideoStream.xaml.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Media::Core;
using namespace Windows::Media::MediaProperties;
using namespace SDKSample;
using namespace SDKSample::MediaStreamSource;

static const int c_frameRateN = 30;
static const int c_frameRateD = 1;

Scenario2_UseDirectXForVideoStream::Scenario2_UseDirectXForVideoStream() :
    _videoDesc(nullptr),
    _mss(nullptr),
    _sampleGenerator(nullptr),
    _fHasSetMediaSource(false),
    _fMediaSourceIsLoaded(false),
    _fPlayRequestPending(false)
{
    InitializeComponent();
}

void Scenario2_UseDirectXForVideoStream::OnSampleRequested(Windows::Media::Core::MediaStreamSource ^ sender, MediaStreamSourceSampleRequestedEventArgs ^ args)
{
    _sampleGenerator->GenerateSample(args->Request);
}

void Scenario2_UseDirectXForVideoStream::OnStarting(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceStartingEventArgs ^args)
{
    _sampleGenerator->Initialize(sender, _videoDesc);
    TimeSpan startPos = {0};
    args->Request->SetActualStartPosition(startPos);
}

void Scenario2_UseDirectXForVideoStream::InitializeMediaPlayer()
{
    int iWidth = (int)Window::Current->Bounds.Width;
    int iHeight = (int)Window::Current->Bounds.Height;
    // Even frame size with a 16:9 ratio
    iWidth = min(iWidth, ((iHeight * 16 / 9) >> 1) * 2);
    iHeight = min(iHeight, ((iWidth * 9 / 16) >> 1) * 2);

    VideoEncodingProperties^ videoProperties = VideoEncodingProperties::CreateUncompressed(MediaEncodingSubtypes::Bgra8, iWidth, iHeight);
    _videoDesc = ref new VideoStreamDescriptor(videoProperties);
    _videoDesc->EncodingProperties->FrameRate->Numerator = c_frameRateN;
    _videoDesc->EncodingProperties->FrameRate->Denominator = c_frameRateD;
    _videoDesc->EncodingProperties->Bitrate = (unsigned int)(c_frameRateN * c_frameRateD * iWidth * iHeight * 4);

    _mss = ref new Windows::Media::Core::MediaStreamSource(_videoDesc);
    TimeSpan spanBuffer;
    spanBuffer.Duration = 2500000;
    _mss->BufferTime = spanBuffer;
    _mss->Starting += ref new Windows::Foundation::TypedEventHandler<Windows::Media::Core::MediaStreamSource ^, Windows::Media::Core::MediaStreamSourceStartingEventArgs ^>(this, &SDKSample::MediaStreamSource::Scenario2_UseDirectXForVideoStream::OnStarting);
    _mss->SampleRequested += ref new Windows::Foundation::TypedEventHandler<Windows::Media::Core::MediaStreamSource ^, Windows::Media::Core::MediaStreamSourceSampleRequestedEventArgs ^>(this, &SDKSample::MediaStreamSource::Scenario2_UseDirectXForVideoStream::OnSampleRequested);

    _sampleGenerator = ref new DXSurfaceGenerator::SampleGenerator();

    mediaPlayer->AutoPlay = false;
    mediaPlayer->CurrentStateChanged += ref new Windows::UI::Xaml::RoutedEventHandler(this, &SDKSample::MediaStreamSource::Scenario2_UseDirectXForVideoStream::OnCurrentStateChanged);
    mediaPlayer->SetMediaStreamSource(_mss);
    _fHasSetMediaSource = true;
}

void Scenario2_UseDirectXForVideoStream::playButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        if (!_fHasSetMediaSource)
        {
            InitializeMediaPlayer();
        }

        if (_fMediaSourceIsLoaded)
        {
            mediaPlayer->Play();
            _fPlayRequestPending = false;
        }
        else
        {
            _fPlayRequestPending = true;
        }
        MainPage::Current->NotifyUser("Playing the DirectX Media", NotifyType::StatusMessage);
    }
}

void Scenario2_UseDirectXForVideoStream::pauseButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        if (_fPlayRequestPending)
        {
            _fPlayRequestPending = false;
        }

        mediaPlayer->Pause();
        MainPage::Current->NotifyUser("Pausing the DirectX Media", NotifyType::StatusMessage);
    }
}

void Scenario2_UseDirectXForVideoStream::OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e)
{
    // As we are still playing we need to stop MSS from requesting for more samples otherwise we'll crash
    if (_mss)
    {
        _mss->NotifyError(Windows::Media::Core::MediaStreamSourceErrorStatus::Other);
        mediaPlayer->Stop();
    }
}

void SDKSample::MediaStreamSource::Scenario2_UseDirectXForVideoStream::OnCurrentStateChanged(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e)
{
    switch (mediaPlayer->CurrentState)
    {
    case MediaElementState::Paused:
    case MediaElementState::Stopped:
        _fMediaSourceIsLoaded = true;
        if (_fPlayRequestPending)
        {
            mediaPlayer->Play();
            _fPlayRequestPending = false;
        }
        break;
    }
}
