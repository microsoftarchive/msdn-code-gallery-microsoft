//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "StreamEffect.h"
#include "DirectXSample.h"

namespace
{
    const char16 APPLICATION_TITLE[] = L"XAudio2 audio stream playback sample";
    const char16 MUSIC_FILE[]        = L"Media\\Wavs\\becky.wma";

    const size_t BUFFER_COUNT = 3;

    const char16 MSG_NEED_AUDIO[]       = L"No usable audio devices were found; install an audio device and restart the sample.";
    const float INFORMATION_START_X     = 32.0f;
    const float INFORMATION_START_Y     = 150.0f;

    const char16    FONT_LOCALE[]       = L"en-US";
    const char16    FONT_NAME[]         = L"Segoe UI";
    const float     FONT_SIZE_TEXT      = 18.0f;
};

StreamEffect::StreamEffect() :
    m_isAudioStarted(false),
    m_currentBuffer(0),
    m_musicEngine(nullptr),
    m_musicMasteringVoice(nullptr),
    m_musicSourceVoice(nullptr),
    m_audioBuffers()
{
}

StreamEffect::~StreamEffect()
{
    ReleaseAudioResources();
}

void StreamEffect::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    CreateAudioResources();
}

void StreamEffect::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        ref new Platform::String(APPLICATION_TITLE)
        );

    //
    // Setup the graphics objects related to drawing text
    //
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            FONT_NAME,
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            FONT_SIZE_TEXT,
            FONT_LOCALE,
            &m_dataTextFormat
            )
        );

    DX::ThrowIfFailed(
        m_dataTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING)
        );
    DX::ThrowIfFailed(
        m_dataTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::White), &m_textBrush)
        );

}

void StreamEffect::CreateAudioResources()
{
    UINT32 flags = 0;

    // Load the wave into a buffer using Media Foundation.
    // Media Foundation is a convenient way to get both file I/O and format decode for audio assets.
    // Games are welcome to replace the streamer in this sample with their own file I/O and decode routines

    m_musicStreamer.reset(new MediaStreamer(ref new Platform::String(MUSIC_FILE)));
    uint32 streamSampleRate = m_musicStreamer->GetOutputWaveFormatEx().nSamplesPerSec;

    // Create XAudio2 and a single mastering voice on the default audio device

    // NOTE: The method for creating XAudio2 is changing for Win8 (and will be different than
    // what you can read on MSDN today.) Changes include:
    //    * XAudio2Create is a flat Win32 API and does not require CoCreateInstance
    //    * There is no device enumeration on IXAudio2 - GetDeviceCount and GetDeviceDetails are removed
    //    * CreateMasteringVoice now takes a device id string instead of index, requiring you to enumerate devices using
    //      the new Metro style device enumeration APIs in Windows or the desktop MMDevice enumeration APIs
    //      (Only the default device is supported for //BUILD.)
    //    * CreateMasteringVoice has an additional parameter to tag the category of the output stream

    DX::ThrowIfFailed(
        XAudio2Create(&m_musicEngine, flags)
        );

    // This sample plays the equivalent of background music, which we tag on the mastering voice as AudioCategory_GameMedia.
    // In ordinary usage, if we were playing the music track with no effects, we could route it entirely through
    // Media Foundation. Here we are using XAudio2 to apply a reverb effect to the music, so we use Media Foundation to
    // decode the data then we feed it through the XAudio2 pipeline as a separate Mastering Voice, so that we can tag it
    // as Game Media.

    HRESULT hr = m_musicEngine->CreateMasteringVoice(
        &m_musicMasteringVoice,
        XAUDIO2_DEFAULT_CHANNELS,
        streamSampleRate,
        0,
        nullptr,    // Use the default audio device
        nullptr,    // No effect chain on the mastering voice
        AudioCategory_GameMedia
        );
    if (FAILED(hr))
    {
        //
        // Unable to create the mastering voice
        // This happens if there are no audio devices
        //
        m_musicMasteringVoice = nullptr;
        return;
    }

    // Set up Reverb effect

    const WAVEFORMATEX& waveFormat = m_musicStreamer->GetOutputWaveFormatEx();

    Microsoft::WRL::ComPtr<IUnknown> pXAPO;
    DX::ThrowIfFailed(
        XAudio2CreateReverb(&pXAPO)
        );
    XAUDIO2_EFFECT_DESCRIPTOR descriptor;
    descriptor.InitialState = true;
    descriptor.OutputChannels = waveFormat.nChannels;
    descriptor.pEffect = pXAPO.Get();
    XAUDIO2_EFFECT_CHAIN chain;
    chain.EffectCount = 1;
    chain.pEffectDescriptors = &descriptor;

    // Create the source voice and start it
    DX::ThrowIfFailed(
        m_musicEngine->CreateSourceVoice(&m_musicSourceVoice, &waveFormat, 0, 1.0f, nullptr, nullptr, &chain)
        );

    XAUDIO2FX_REVERB_PARAMETERS reverbParameters;
    reverbParameters.ReflectionsDelay = XAUDIO2FX_REVERB_DEFAULT_REFLECTIONS_DELAY;
    reverbParameters.ReverbDelay = XAUDIO2FX_REVERB_DEFAULT_REVERB_DELAY;
    reverbParameters.RearDelay = XAUDIO2FX_REVERB_DEFAULT_REAR_DELAY;
    reverbParameters.PositionLeft = XAUDIO2FX_REVERB_DEFAULT_POSITION;
    reverbParameters.PositionRight = XAUDIO2FX_REVERB_DEFAULT_POSITION;
    reverbParameters.PositionMatrixLeft = XAUDIO2FX_REVERB_DEFAULT_POSITION_MATRIX;
    reverbParameters.PositionMatrixRight = XAUDIO2FX_REVERB_DEFAULT_POSITION_MATRIX;
    reverbParameters.EarlyDiffusion = XAUDIO2FX_REVERB_DEFAULT_EARLY_DIFFUSION;
    reverbParameters.LateDiffusion = XAUDIO2FX_REVERB_DEFAULT_LATE_DIFFUSION;
    reverbParameters.LowEQGain = XAUDIO2FX_REVERB_DEFAULT_LOW_EQ_GAIN;
    reverbParameters.LowEQCutoff = XAUDIO2FX_REVERB_DEFAULT_LOW_EQ_CUTOFF;
    reverbParameters.HighEQGain = XAUDIO2FX_REVERB_DEFAULT_HIGH_EQ_GAIN;
    reverbParameters.HighEQCutoff = XAUDIO2FX_REVERB_DEFAULT_HIGH_EQ_CUTOFF;
    reverbParameters.RoomFilterFreq = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_FREQ;
    reverbParameters.RoomFilterMain = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_MAIN;
    reverbParameters.RoomFilterHF = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_HF;
    reverbParameters.ReflectionsGain = XAUDIO2FX_REVERB_DEFAULT_REFLECTIONS_GAIN;
    reverbParameters.ReverbGain = XAUDIO2FX_REVERB_DEFAULT_REVERB_GAIN;
    reverbParameters.DecayTime = XAUDIO2FX_REVERB_DEFAULT_DECAY_TIME;
    reverbParameters.Density = XAUDIO2FX_REVERB_DEFAULT_DENSITY;
    reverbParameters.RoomSize = XAUDIO2FX_REVERB_DEFAULT_ROOM_SIZE;
    reverbParameters.WetDryMix = XAUDIO2FX_REVERB_DEFAULT_WET_DRY_MIX;
    reverbParameters.DisableLateField = FALSE;

    DX::ThrowIfFailed(
        m_musicSourceVoice->SetEffectParameters(0, &reverbParameters, sizeof(reverbParameters))
        );

    DX::ThrowIfFailed(
        m_musicSourceVoice->EnableEffect(0)
        );
}

void StreamEffect::ReleaseAudioResources()
{
    if (m_musicSourceVoice != nullptr)
    {
        m_musicSourceVoice->DestroyVoice();
        m_musicSourceVoice    = nullptr;
    }
    if (m_musicMasteringVoice != nullptr)
    {
        m_musicMasteringVoice->DestroyVoice();
        m_musicMasteringVoice = nullptr;
    }
    if (m_musicEngine != nullptr)
    {
        m_musicEngine->Release();
        m_musicEngine = nullptr;
    }
}

void StreamEffect::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();
    m_sampleOverlay->UpdateForWindowSizeChange();
}

void StreamEffect::Render()
{
    // bind the render targets
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // clear both the render target and depth stencil to default values
    const float ClearColor[4] = { 0.071f, 0.040f, 0.561f, 1.0f };

    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        ClearColor
        );

    m_sampleOverlay->Render();

    if (IsAudioSetup())
    {
        RenderAudio();
    }
    else
    {
        //
        // No audio, render a message indicating setup failure
        //
        D2D1_SIZE_F size = m_d2dContext->GetSize();
        D2D1_RECT_F pos = D2D1::RectF(INFORMATION_START_X, INFORMATION_START_Y, size.width, size.height);

        // Display a message when controller is not connected
        m_d2dContext->BeginDraw();
        m_d2dContext->DrawText(
            MSG_NEED_AUDIO,
            static_cast<uint32>(::wcslen(MSG_NEED_AUDIO)),
            m_dataTextFormat.Get(),
            pos,
            m_textBrush.Get()
            );

        // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
        // is lost. It will be handled during the next call to Present.
        HRESULT hr = m_d2dContext->EndDraw();
        if (hr != D2DERR_RECREATE_TARGET)
        {
            DX::ThrowIfFailed(hr);
        }
    }
}

void StreamEffect::OnSuspending()
{
    m_musicEngine->StopEngine();
}

void StreamEffect::OnResuming()
{
    DX::ThrowIfFailed(
        m_musicEngine->StartEngine()
        );
}

// This sample processes audio buffers during the render cycle of the application.
// As long as the sample maintains a high-enough frame rate, this approach should
// not glitch audio. In game code, audio buffers should be processed on a separate thread
// that is not synched to the main render loop of the game.

void StreamEffect::RenderAudio()
{
    if (IsAudioSetup() == false)
    {
        return;
    }

    // Find the current state of the playing buffers
    XAUDIO2_VOICE_STATE state;
    m_musicSourceVoice->GetState(&state);

    // Have any of the buffer completed
    while (state.BuffersQueued < BUFFER_COUNT)
    {
        // Retreive the next buffer to stream from MF Music Streamer
        m_audioBuffers[m_currentBuffer] = m_musicStreamer->GetNextBuffer();
        if (m_audioBuffers[m_currentBuffer].size() == 0)
        {
            // Audio file has been fully read, restart the reader to re-loop the audio
            m_musicStreamer->Restart();
            break;
        }

        //
        // Submit the new buffer
        //
        XAUDIO2_BUFFER buf = {0};
        buf.AudioBytes = static_cast<uint32>(m_audioBuffers[m_currentBuffer].size());
        buf.pAudioData = &m_audioBuffers[m_currentBuffer][0];
        DX::ThrowIfFailed(
            m_musicSourceVoice->SubmitSourceBuffer(&buf)
            );

        // Advance the buffer index
        m_currentBuffer = ++m_currentBuffer % BUFFER_COUNT;


        // Get the updated state
        m_musicSourceVoice->GetState(&state);
    }
}

bool StreamEffect::IsAudioSetup()
{
    return ((m_musicEngine != nullptr) &&
            (m_musicMasteringVoice != nullptr) &&
            (m_musicSourceVoice != nullptr)
            );
}


void StreamEffect::Update(float TimeTotal, float TimeDelta)
{
    if (!m_isAudioStarted && (m_musicSourceVoice != nullptr))
    {
        DX::ThrowIfFailed(
            m_musicSourceVoice->Start(0)
            );

        m_isAudioStarted = true;
    }
}
