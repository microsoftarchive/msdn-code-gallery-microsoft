//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <roapi.h>
#include <mmreg.h>
#include <mfidl.h>
#include <mfapi.h>
#include <mfreadwrite.h>

class MediaStreamer
{
public:
    MediaStreamer(_In_ Platform::String^ url);

    void Restart();
    std::vector<byte> ReadAll();
    std::vector<byte> GetNextBuffer();

    const WAVEFORMATEX& GetOutputWaveFormatEx() const
    {
        return m_waveFormat;
    }

    uint32 GetMaxStreamLengthInBytes() const
    {
        return m_maxStreamLengthInBytes;
    }

private:
    Microsoft::WRL::ComPtr<IMFSourceReader> m_reader;
    WAVEFORMATEX                            m_waveFormat;
    uint32                                  m_maxStreamLengthInBytes;
};
