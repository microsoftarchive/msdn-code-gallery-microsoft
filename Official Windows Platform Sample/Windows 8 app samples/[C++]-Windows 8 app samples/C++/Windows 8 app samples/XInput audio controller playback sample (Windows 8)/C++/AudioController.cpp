//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "AudioController.h"
#include "DirectXSample.h"


namespace
{
    const char16 APPLICATION_TITLE[]    = L"XInput audio controller playback sample";
    const char16 SOUND_FILE[]           = L"media\\wavs\\pow.wav";

    const char16    FONT_LOCAL[]        = L"en-US";
    const char16    FONT_NAME[]         = L"Segoe UI";
    const float     FONT_SIZE_TEXT      = 18.0f;

    const char16 MSG_NEED_CONTROLLER[]  = L"Please insert an Xbox 360 common controller device and attach a headset.";
    const char16 MSG_NEED_HEADSET[]     = L"Please attach a headset to the Xbox 360 common controller device.";
    const char16 MSG_INSTRUCTIONS[]     = L"Press the A button to play a sound.";

    const float INFORMATION_START_X     = 32.0f;
    const float INFORMATION_START_Y     = 150.0f;

    const uint64 XINPUT_ENUM_TIMEOUT_MS          = 2000;  // 2 seconds
    const uint64 XINPUT_HEADSET_CHECK_TIMEOUT_MS = 2000;  // 2 seconds
};

AudioController::AudioController() :
    m_audioEngine(nullptr),
    m_masteringVoice(nullptr),
    m_sourceVoice(nullptr),
    m_soundEffectBuffer(),
    m_isControllerConnected(false),
    m_isHeadsetConnected(false),
    m_aButtonWasPressed(false)
{
    // Want the first pass to perform ennumeration
    uint64 tickCount = ::GetTickCount64();
    m_lastEnumTime = tickCount - XINPUT_ENUM_TIMEOUT_MS;
    m_lastHeadsetCheckTime = tickCount - XINPUT_HEADSET_CHECK_TIMEOUT_MS;

}

AudioController::~AudioController()
{
    ReleaseAudioResources();
}

void AudioController::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    //
    // Create and initialize the default Sample graphics object
    //
    m_sampleOverlay = ref new SampleOverlay();
    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        ref new Platform::String(APPLICATION_TITLE)
        );

    //
    // Setup the local graphics objects
    //
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            FONT_NAME,
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            FONT_SIZE_TEXT,
            FONT_LOCAL,
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

void AudioController::CreateAudioVoices(
    _In_ const char16* deviceEndpointId
    )
{
    uint32 flags = 0;

    DX::ThrowIfFailed(
        XAudio2Create(&m_audioEngine, flags)
        );

    // Load all data for the sound effect into a single in-memory buffer
    MediaStreamer soundStream(ref new Platform::String(SOUND_FILE));
    m_soundEffectBuffer = soundStream.ReadAll();

    // Get the sample rate from the sound
    uint32 streamSampleRate = soundStream.GetOutputWaveFormatEx().nSamplesPerSec;

    // Create mastering voice for the stream, use stream's sample rate
    DX::ThrowIfFailed(
        m_audioEngine->CreateMasteringVoice(
            &m_masteringVoice,
            XAUDIO2_DEFAULT_CHANNELS,
            streamSampleRate,
            0,
            deviceEndpointId
            )
        );


    // Create a source voice for the sound effect using the stream's format
    DX::ThrowIfFailed(
        m_audioEngine->CreateSourceVoice(&m_sourceVoice, &(soundStream.GetOutputWaveFormatEx()))
        );
}

void AudioController::ReleaseAudioResources()
{
    if (m_sourceVoice != nullptr)
    {
        m_sourceVoice->DestroyVoice();
        m_sourceVoice = nullptr;
    }

    if (m_masteringVoice != nullptr)
    {
        m_masteringVoice->DestroyVoice();
        m_masteringVoice = nullptr;
    }

    if (m_audioEngine != nullptr)
    {
        m_audioEngine->Release();
        m_audioEngine = nullptr;
    }
}

void AudioController::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();
    m_sampleOverlay->UpdateForWindowSizeChange();
}

void AudioController::Render()
{
    // bind the render targets
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // clear both the render target and depth stencil to default values
    const float clearColor[4] = { 0.071f, 0.040f, 0.561f, 1.0f };

    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        clearColor
        );

    m_sampleOverlay->Render();

    D2D1_SIZE_F size = m_d2dContext->GetSize();
    D2D1_RECT_F pos = D2D1::RectF(INFORMATION_START_X, INFORMATION_START_Y, size.width, size.height);

    m_d2dContext->BeginDraw();

    if (!m_isControllerConnected)
    {
        // Display a message when controller is not connected
        m_d2dContext->DrawText(
            MSG_NEED_CONTROLLER,
            static_cast<uint32>(::wcslen(MSG_NEED_CONTROLLER)),
            m_dataTextFormat.Get(),
            pos,
            m_textBrush.Get()
            );
    }
    else if (!m_isHeadsetConnected)
    {
        // Display a message when controller is connected, but not a headset

        m_d2dContext->DrawText(
            MSG_NEED_HEADSET,
            static_cast<uint32>(::wcslen(MSG_NEED_HEADSET)),
            m_dataTextFormat.Get(),
            pos,
            m_textBrush.Get()
            );
    }
    else
    {
        // Display a message when all is connected explaining what to do
        m_d2dContext->DrawText(
            MSG_INSTRUCTIONS,
            static_cast<uint32>(::wcslen(MSG_INSTRUCTIONS)),
            m_dataTextFormat.Get(),
            pos,
            m_textBrush.Get()
            );
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

void AudioController::OnSuspending()
{
    if (m_audioEngine != nullptr)
    {
        m_audioEngine->StopEngine();
    }
}

void AudioController::OnResuming()
{
    if (m_audioEngine != nullptr)
    {
        DX::ThrowIfFailed(
            m_audioEngine->StartEngine()
            );
    }
}

void AudioController::FetchControllerInput()
{
    uint64 currentTime = ::GetTickCount64();

    if (!m_isControllerConnected)
    {
        //
        // Ennumerating for XInput devices takes 'time' on the order of milliseconds.
        // Any time a device is not currently known as connected (not yet called XInput, or calling
        // an XInput function after a failure) ennumeration happens.
        // An app should avoid repeatedly calling XInput functions if there are no known devices connected
        // as this can slow down application performance.
        // This sample takes the simple approach of not calling XInput functions after failure
        // until a specified timeout has passed.
        //
        if (currentTime - m_lastEnumTime < XINPUT_ENUM_TIMEOUT_MS)
        {
            return;
        }
        m_lastEnumTime = currentTime;

        // Check for controller connection by trying to get the capabilties
        uint32 capsResult = XInputGetCapabilities(0, XINPUT_FLAG_GAMEPAD, &m_xinputCaps);
        if (capsResult != ERROR_SUCCESS)
        {
            m_lastHeadsetCheckTime = currentTime;
            return;
        }

        // Device is connected
        m_isControllerConnected = true;

        //
        // Start with the assumption that the headset is not connected
        // and recheck the connection right away
        //
        m_isHeadsetConnected = false;
        m_lastHeadsetCheckTime = currentTime - XINPUT_HEADSET_CHECK_TIMEOUT_MS;
    }

    uint32 stateResult = XInputGetState(0, &m_xinputState);
    if (stateResult != ERROR_SUCCESS)
    {
        // Device is no longer connected
        m_isControllerConnected = false;
        m_isHeadsetConnected = false;
        m_lastEnumTime = currentTime;
        m_lastHeadsetCheckTime = currentTime;

        return;
    }

    if ((m_xinputCaps.Flags & XINPUT_CAPS_VOICE_SUPPORTED) == 0)
    {
        //
        // Controller does not support voice
        // We don't keep rechecking caps in this state as they shouldn't change without
        // the device first getting disconnected, then reconnected
        //
        return;
    }

    if (currentTime - m_lastHeadsetCheckTime < XINPUT_HEADSET_CHECK_TIMEOUT_MS)
    {
        // It is not time to test the headset connection
        return;
    }
    m_lastHeadsetCheckTime = currentTime;

    // Find the Audio Endpoint ID
    uint32 renderCount = 0;
    uint32 getEndpointResult = XInputGetAudioDeviceIds(0, nullptr, &renderCount, nullptr, nullptr);

    if ((getEndpointResult != ERROR_SUCCESS) || (renderCount == 0))
    {
        // The headset is not connected even if it was before
        m_isHeadsetConnected = false;
        return;
    }

    if (m_isHeadsetConnected == true)
    {
        // Was connected, still connected
        return;
    }

    // Newly connected, setup audio, get the full render ID
    char16* renderId = new char16[renderCount];
    getEndpointResult = XInputGetAudioDeviceIds(0, renderId, &renderCount, nullptr, nullptr);
    if ((getEndpointResult == ERROR_SUCCESS) && (renderId[0] != '\0'))
    {
        // Headset was there, and now is gone
        m_isHeadsetConnected = true;
        CreateAudioVoices(renderId);
    }
    delete[] renderId;
}

void AudioController::Update(float TimeTotal, float TimeDelta)
{
    FetchControllerInput();

    if (m_isHeadsetConnected)
    {
        if (m_xinputState.Gamepad.wButtons & XINPUT_GAMEPAD_A)
        {
            m_aButtonWasPressed = true;
        }
        else if (m_aButtonWasPressed)
        {
            // Trigger once, only on button release
            m_aButtonWasPressed = false;
            TriggerSoundEffect();
        }
    }
}

void AudioController::TriggerSoundEffect()
{
    XAUDIO2_BUFFER buf = {0};
    XAUDIO2_VOICE_STATE state = {0};

    // Interrupt sound effect if currently playing
    DX::ThrowIfFailed(
        m_sourceVoice->Stop()
        );
    DX::ThrowIfFailed(
        m_sourceVoice->FlushSourceBuffers()
        );

    // Queue in-memory buffer for playback and start the voice
    buf.AudioBytes = static_cast<uint32>(m_soundEffectBuffer.size());
    buf.pAudioData = &m_soundEffectBuffer[0];
    buf.Flags = XAUDIO2_END_OF_STREAM;

    DX::ThrowIfFailed(
        m_sourceVoice->SubmitSourceBuffer(&buf)
        );
    DX::ThrowIfFailed(
        m_sourceVoice->Start()
        );
}
