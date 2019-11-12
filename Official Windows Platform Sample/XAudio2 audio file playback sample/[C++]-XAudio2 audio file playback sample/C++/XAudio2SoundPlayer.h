//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once


#include "DirectXBase.h"

#include "mmreg.h"
#include <vector>
#include <memory>

class XAudio2SoundPlayer
{
    struct ImplData;

    public:
        XAudio2SoundPlayer(uint32 sampleRate);
        ~XAudio2SoundPlayer();

        size_t AddSound(_In_ WAVEFORMATEX* format, _In_ Platform::Array<byte>^ data);
        bool   PlaySound(size_t index);
        bool   StopSound(size_t index);
        bool   IsSoundPlaying(size_t index) const;
        size_t GetSoundCount() const;

        void Suspend();
        void Resume();

    private:
        interface IXAudio2*                     m_audioEngine;
        interface IXAudio2MasteringVoice*       m_masteringVoice;
        std::vector<std::shared_ptr<ImplData>>  m_soundList;
};

