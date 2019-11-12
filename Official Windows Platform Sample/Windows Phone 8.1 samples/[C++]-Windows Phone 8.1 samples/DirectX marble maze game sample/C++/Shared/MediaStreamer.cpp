//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXSample.h"
#include "MediaStreamer.h"

MediaStreamer::MediaStreamer()
{
    m_reader = nullptr;
    m_audioType = nullptr;
    ZeroMemory(&m_waveFormat, sizeof(m_waveFormat));
}

MediaStreamer::~MediaStreamer()
{
}

void MediaStreamer::Initialize(_In_ const WCHAR* url)
{
    Microsoft::WRL::ComPtr<IMFMediaType> outputMediaType;
    Microsoft::WRL::ComPtr<IMFMediaType> mediaType;

    DX::ThrowIfFailed(
        MFStartup(MF_VERSION)
        );

    DX::ThrowIfFailed(
        MFCreateSourceReaderFromURL(url, nullptr, &m_reader)
        );

    // Set the decoded output format as PCM.
    // XAudio2 on Windows can process PCM and ADPCM-encoded buffers.
    // When this sample uses Media Foundation, it always decodes into PCM.

    DX::ThrowIfFailed(
        MFCreateMediaType(&mediaType)
        );

    DX::ThrowIfFailed(
        mediaType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio)
        );

    DX::ThrowIfFailed(
        mediaType->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM)
        );

    DX::ThrowIfFailed(
        m_reader->SetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, mediaType.Get())
        );

    // Get the complete WAVEFORMAT from the Media Type.
    DX::ThrowIfFailed(
        m_reader->GetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, &outputMediaType)
        );

    uint32 formatSize = 0;
    WAVEFORMATEX* waveFormat;
    DX::ThrowIfFailed(
        MFCreateWaveFormatExFromMFMediaType(outputMediaType.Get(), &waveFormat, &formatSize)
        );
    CopyMemory(&m_waveFormat, waveFormat, sizeof(m_waveFormat));
    CoTaskMemFree(waveFormat);

    // Get the total length of the stream, in bytes.
    PROPVARIANT var;
    DX::ThrowIfFailed(
        m_reader->GetPresentationAttribute(MF_SOURCE_READER_MEDIASOURCE, MF_PD_DURATION, &var)
        );

    // duration is in 100ns units; convert to seconds, and round up
    // to the nearest whole byte.
    ULONGLONG duration = var.uhVal.QuadPart;
    m_maxStreamLengthInBytes =
        static_cast<unsigned int>(
            ((duration * static_cast<ULONGLONG>(m_waveFormat.nAvgBytesPerSec)) + 10000000) /
            10000000
            );
}

bool MediaStreamer::GetNextBuffer(uint8* buffer, uint32 maxBufferSize, uint32* bufferLength)
{
    Microsoft::WRL::ComPtr<IMFSample> sample;
    Microsoft::WRL::ComPtr<IMFMediaBuffer> mediaBuffer;
    BYTE *audioData = nullptr;
    DWORD sampleBufferLength = 0;
    DWORD flags = 0;

    *bufferLength = 0;

    if (m_reader == nullptr)
    {
        return false;
    }

    DX::ThrowIfFailed(
        m_reader->ReadSample(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, nullptr, &flags, nullptr, &sample)
        );

    if (sample == nullptr)
    {
        if (flags & MF_SOURCE_READERF_ENDOFSTREAM)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    DX::ThrowIfFailed(
        sample->ConvertToContiguousBuffer(&mediaBuffer)
        );

    DX::ThrowIfFailed(
        mediaBuffer->Lock(&audioData, nullptr, &sampleBufferLength)
        );

    // Only copy the sample if the remaining buffer is large enough.
    if (sampleBufferLength <= maxBufferSize)
    {
        CopyMemory(buffer, audioData, sampleBufferLength);
        *bufferLength = sampleBufferLength;
    }

    if (flags & MF_SOURCE_READERF_ENDOFSTREAM)
    {
        return true;
    }
    else
    {
        return false;
    }
}

void MediaStreamer::ReadAll(uint8* buffer, uint32 maxBufferSize, uint32* bufferLength)
{
    uint32 valuesWritten = 0;
    uint32 sampleBufferLength = 0;

    if (m_reader == nullptr)
    {
        return;
    }

    *bufferLength = 0;
    // If buffer isn't large enough, return
    if (maxBufferSize < m_maxStreamLengthInBytes)
    {
        return;
    }

    while (!GetNextBuffer(buffer + valuesWritten, maxBufferSize - valuesWritten, &sampleBufferLength))
    {
        valuesWritten += sampleBufferLength;
    }

    *bufferLength = valuesWritten + sampleBufferLength;
}

void MediaStreamer::Restart()
{
    if (m_reader == nullptr)
    {
        return;
    }

    PROPVARIANT var = {0};
    var.vt = VT_I8;

    DX::ThrowIfFailed(
        m_reader->SetCurrentPosition(GUID_NULL, var)
        );
}
