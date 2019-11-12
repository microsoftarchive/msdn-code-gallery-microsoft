/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#pragma once
#include "windows.h"
#include <synchapi.h>
#include <audioclient.h>
#include <phoneaudioclient.h>
#include "BackEndTransport.h"

namespace PhoneVoIPApp
{
    namespace BackEnd
    {
         public ref class BackEndAudioRender sealed
        {
        public:
            BackEndAudioRender(void);
            virtual ~BackEndAudioRender(void);
            void Start();
            void Stop();
            void SetTransport(BackEndTransport^ transport);
        private:
            HRESULT InitRender();
            
            IAudioRenderClient* m_pRenderClient;
            IAudioClient2* m_pDefaultRenderDevice;
            // Misc interfaces
            IAudioClock* m_pClock; 
            ISimpleAudioVolume* m_pVolume;
            void OnTransportMessageReceived(Windows::Storage::Streams::IBuffer^ stream, UINT64, UINT64);
            // Audio buffer size
            UINT32 m_nMaxFrameCount;

            int m_sourceFrameSizeInBytes;

            WAVEFORMATEX* m_pwfx;
            BackEndTransport^ transportController;
            PhoneVoIPApp::BackEnd::MessageReceivedEventHandler^ onTransportMessageReceivedHandler;
            Windows::Foundation::EventRegistrationToken onTransportMessageReceivedHandlerToken;

            // Has audio started?
            bool started;
        };
    }
}
