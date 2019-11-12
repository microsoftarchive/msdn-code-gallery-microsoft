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
#include "implements.h"
#include "ICallControllerStatusListener.h"
#include "BackEndTransport.h"
#include <Windows.Phone.Media.Capture.h>
#include <Windows.Phone.Media.Capture.Native.h>

namespace PhoneVoIPApp
{
    namespace BackEnd
    {
        public delegate void CameraLocationChangedEventHandler(PhoneVoIPApp::BackEnd::CameraLocation);

        class CaptureSampleSink :
            public Microsoft::WRL::RuntimeClass<
            Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::RuntimeClassType::ClassicCom>, 
            ICameraCaptureSampleSink>
        {
            DWORD m_dwSampleCount;
            BackEndTransport ^m_transport;

        public:

            STDMETHODIMP RuntimeClassInitialize(BackEndTransport ^transportController)
            {
                m_dwSampleCount = 0;
                m_transport = transportController;
                return S_OK;
            }

            DWORD GetSampleCount()
            {
                return m_dwSampleCount;
            }

            IFACEMETHODIMP_(void)
                OnSampleAvailable(
                ULONGLONG hnsPresentationTime,
                ULONGLONG hnsSampleDuration,
                DWORD cbSample,
                BYTE* pSample)
            {
                m_dwSampleCount++;
                if (m_transport)
                {
                    m_transport->WriteVideo(pSample, cbSample, hnsPresentationTime, hnsSampleDuration);
                }
            }

            void SetTransport(BackEndTransport ^transport)
            {
                m_transport = transport;
            }
        };

        public ref class BackEndCapture sealed
        {
        public:
            // Constructor
            BackEndCapture();

            void SetTransport(BackEndTransport^ transport);

            void Start(CameraLocation cameraLocation);
            void Stop();

            void ToggleCamera();

            event CameraLocationChangedEventHandler^ CameraLocationChanged;

        private:
            // Destructor
            ~BackEndCapture();

            void InitCapture();

            void ToggleCameraThread(Windows::Foundation::IAsyncAction^ operation);

            // Has capture started?
            bool started;

            // Events to signal whether capture has stopped/started
            HANDLE hStopCompleted;
            HANDLE hStartCompleted;

            Windows::Foundation::IAsyncAction^ m_ToggleThread;

            // Transport
            BackEndTransport^ transportController;

            // Native sink and video device
            CaptureSampleSink *pVideoSink;
            IAudioVideoCaptureDeviceNative *pVideoDevice;

            Windows::Phone::Media::Capture::CameraSensorLocation cameraLocation;

            Windows::Phone::Media::Capture::AudioVideoCaptureDevice ^videoOnlyDevice;
            Windows::Foundation::IAsyncAction ^videoCaptureAction;

        };
    }
}
