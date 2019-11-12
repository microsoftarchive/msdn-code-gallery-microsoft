//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once


#include "SampleOverlay.h"
#include "DirectXSample.h"
#include "DirectXBase.h"
#include "MediaStreamer.h"

ref class StreamEffect : public DirectXBase
{
public:
//    static const size_t BUFFER_COUNT = 3;
internal:
    StreamEffect();

    // Overrides from DirectXBase
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void OnSuspending();
    void OnResuming();

    void Update(float TimeTotal, float TimeDelta);

    // Audio Functionality
    void CreateAudioResources();
    void ReleaseAudioResources();
    void RenderAudio();
    bool IsAudioSetup();

private:
    // Overlay for Graphics
    ~StreamEffect();
    SampleOverlay^                  m_sampleOverlay;

    // Audio Related Members
    bool                            m_isAudioStarted;
    IXAudio2*                       m_musicEngine;
    IXAudio2MasteringVoice*         m_musicMasteringVoice;
    IXAudio2SourceVoice*            m_musicSourceVoice;
    std::vector<byte>               m_audioBuffers[3];
    std::unique_ptr<MediaStreamer>  m_musicStreamer;
    size_t                          m_currentBuffer;

    // Members for drawing text messages
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_dataTextFormat;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_textBrush;
};
