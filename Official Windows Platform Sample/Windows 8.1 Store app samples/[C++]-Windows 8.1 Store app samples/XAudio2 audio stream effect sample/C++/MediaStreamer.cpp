//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXBase.h"
#include "directxsample.h"

#include <initguid.h>
#include "MediaStreamer.h"

MediaStreamer::MediaStreamer(_In_ Platform::String^ url)
{
    Microsoft::WRL::ComPtr<IMFMediaType> outputMediaType;
    Microsoft::WRL::ComPtr<IMFMediaType> mediaType;

    DX::ThrowIfFailed(
        MFStartup(MF_VERSION)
        );

    //
    // Build an attribute object to set MF_LOW_LATENCY
    // Each title should evaluate whether the latency v. power trade-off is appropriate
    // Longer running music tracks that are not latency sensitive may need this flag less than shorter sounds.
    // This sample is using the flag on a music track for demonstrative purposes.
    //
    Microsoft::WRL::ComPtr<IMFAttributes> lowLatencyAttribute;
    DX::ThrowIfFailed(
        MFCreateAttributes(&lowLatencyAttribute, 1)
        );

    DX::ThrowIfFailed(
        lowLatencyAttribute->SetUINT32(MF_LOW_LATENCY, TRUE)
        );

    //
    // Create the source reader on the url (file) with the low latency attributes
    //
    DX::ThrowIfFailed(
        MFCreateSourceReaderFromURL(url->Data(), lowLatencyAttribute.Get(), &m_reader)
        );

    // Set the decoded output format as PCM
    // XAudio2 on Windows can process PCM and ADPCM-encoded buffers.
    // When using MF, this sample always decodes into PCM.

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

    // Get the complete WAVEFORMAT from the Media Type
    DX::ThrowIfFailed(
        m_reader->GetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, &outputMediaType)
        );

    uint32 formatByteCount = 0;
    WAVEFORMATEX* waveFormat;
    DX::ThrowIfFailed(
        MFCreateWaveFormatExFromMFMediaType(outputMediaType.Get(), &waveFormat, &formatByteCount)
        );
    CopyMemory(&m_waveFormat, waveFormat, sizeof(m_waveFormat));
    CoTaskMemFree(waveFormat);

    // Get the total length of the stream in bytes
    PROPVARIANT var;
    DX::ThrowIfFailed(
        m_reader->GetPresentationAttribute(MF_SOURCE_READER_MEDIASOURCE, MF_PD_DURATION, &var)
        );
    LONGLONG duration = var.uhVal.QuadPart;
    float64 durationInSeconds = (duration / (float64)(10000 * 1000));
    m_maxStreamLengthInBytes = (uint32)(durationInSeconds * m_waveFormat.nAvgBytesPerSec);
}

std::vector<byte> MediaStreamer::GetNextBuffer()
{
    std::vector<byte> resultData;

    Microsoft::WRL::ComPtr<IMFSample> sample;
    Microsoft::WRL::ComPtr<IMFMediaBuffer> mediaBuffer;

    uint8* audioData = nullptr;
    uint32 sampleBufferLength = 0;
    uint32 flags = 0;

    DX::ThrowIfFailed(
        m_reader->ReadSample(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, nullptr, reinterpret_cast<DWORD*>(&flags), nullptr, &sample)
        );

    // End of stream
    if (flags & MF_SOURCE_READERF_ENDOFSTREAM)
    {
        return resultData;
    }
    if (sample == nullptr)
    {
        throw ref new Platform::FailureException();
    }

    DX::ThrowIfFailed(
        sample->ConvertToContiguousBuffer(&mediaBuffer)
        );

    DX::ThrowIfFailed(
        mediaBuffer->Lock(&audioData, nullptr, reinterpret_cast<DWORD*>(&sampleBufferLength))
        );

    //
    // Prepare and return the resultant array of data
    //
    resultData.resize(sampleBufferLength);
    CopyMemory(&resultData[0], audioData, sampleBufferLength);

    // Release the lock
    DX::ThrowIfFailed(
        mediaBuffer->Unlock()
        );

    return resultData;
}

std::vector<byte> MediaStreamer::ReadAll()
{
    std::vector<byte> resultData;

    // Make sure stream is set to start
    // Restart();

    for (;;)
    {
        std::vector<byte> nextBuffer = GetNextBuffer();
        if (nextBuffer.size() == 0)
        {
            break;
        }

        // Append the new buffer to the running total
        size_t lastBufferSize = resultData.size();
        resultData.resize(lastBufferSize + nextBuffer.size());
        CopyMemory(&resultData[lastBufferSize], &nextBuffer[0], nextBuffer.size());
    }

    return resultData;
}

void MediaStreamer::Restart()
{
    PROPVARIANT var;
    ZeroMemory(&var, sizeof(var));
    var.vt = VT_I8;

    DX::ThrowIfFailed(
        m_reader->SetCurrentPosition(GUID_NULL, var)
    );

}