//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"

#include <xaudio2.h>
#include <xaudio2fx.h>
#include "MediaStreamer.h"

ref class AudioController : public DirectXBase
{
internal:
    AudioController();

    // Overrides from DirectXBase
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void OnSuspending();
    void OnResuming();

    void Update(float TimeTotal, float TimeDelta);

    // Audio Functionality
    void CreateAudioVoices(_In_ const char16* deviceEndpointId);
    void ReleaseAudioResources();
    void TriggerSoundEffect();

    // Input functionality
    void FetchControllerInput();

private:
    ~AudioController();

    // Overlay for Graphics
    SampleOverlay^              m_sampleOverlay;

    // Audio Related Members
    IXAudio2*                   m_audioEngine;
    IXAudio2MasteringVoice*     m_masteringVoice;
    IXAudio2SourceVoice*        m_sourceVoice;
    std::vector<byte>           m_soundEffectBuffer;

    // Input related members
    bool                    m_isControllerConnected;  // Do we have a controller connected
    bool                    m_isHeadsetConnected;     // Does the controller have a headset
    XINPUT_CAPABILITIES     m_xinputCaps;             // Capabilites of the controller
    XINPUT_STATE            m_xinputState;            // The current state of the controller
    uint64                  m_lastEnumTime;           // Last time a new controller connection was checked
    uint64                  m_lastHeadsetCheckTime;   // Last time the headset was re-checked for connectivity
    bool                    m_aButtonWasPressed;

    // Members for drawing
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_dataTextFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_textBrush;
};
