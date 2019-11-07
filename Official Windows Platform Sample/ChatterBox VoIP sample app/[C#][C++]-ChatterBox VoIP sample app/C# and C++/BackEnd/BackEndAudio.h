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

#define MAX_RAW_BUFFER_SIZE 1024*128

#include <synchapi.h>
#include <audioclient.h>
#include <phoneaudioclient.h>

#include "BackEndTransport.h"

namespace PhoneVoIPApp
{
    namespace BackEnd
    {
        public ref class BackEndAudio sealed
        {
        public:
            // Constructor
            BackEndAudio();

            // Destructor
            virtual ~BackEndAudio();

            void SetTransport(BackEndTransport^ transport);
            
            void Start();
            void Stop();

        private:
            HRESULT InitRender();
            HRESULT InitCapture();
            HRESULT StartAudioThreads();
            void CaptureThread(Windows::Foundation::IAsyncAction^ operation);
            void OnTransportMessageReceived(Windows::Storage::Streams::IBuffer^ stream, UINT64, UINT64);
            
            BackEndTransport^ transportController;

            PhoneVoIPApp::BackEnd::MessageReceivedEventHandler^ onTransportMessageReceivedHandler;
            Windows::Foundation::EventRegistrationToken onTransportMessageReceivedHandlerToken;

            int m_sourceFrameSizeInBytes;

            WAVEFORMATEX* m_pwfx;

            // Devices
            IAudioClient2* m_pDefaultRenderDevice;
            IAudioClient2* m_pDefaultCaptureDevice;

            // Actual render and capture objects
            IAudioRenderClient* m_pRenderClient;
            IAudioCaptureClient* m_pCaptureClient;

            // Misc interfaces
            IAudioClock* m_pClock; 
            ISimpleAudioVolume* m_pVolume;

            // Audio buffer size
            UINT32 m_nMaxFrameCount;
            HANDLE hCaptureEvent;

            // Event for stopping audio capture/render
            HANDLE hShutdownEvent;

            Windows::Foundation::IAsyncAction^ m_CaptureThread;

            // Has audio started?
            bool started;
        };
    }
}
