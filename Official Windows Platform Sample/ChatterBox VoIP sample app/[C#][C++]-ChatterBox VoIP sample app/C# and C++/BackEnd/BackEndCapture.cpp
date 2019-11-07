/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#include "BackEndCapture.h"
#include "ApiLock.h"
#include <ppltasks.h>

using namespace PhoneVoIPApp::BackEnd;
using namespace Windows::System::Threading;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::Phone::Media::Capture;
using namespace Windows::Storage::Streams;
using namespace Concurrency;

BackEndCapture::BackEndCapture() :
    started(false),
    videoOnlyDevice(nullptr),
    pVideoSink(NULL),
    pVideoDevice(NULL),
    cameraLocation(CameraSensorLocation::Front)
{
    hStopCompleted = CreateEventEx(NULL, NULL, CREATE_EVENT_MANUAL_RESET, EVENT_ALL_ACCESS);
    if (!hStopCompleted)
    {
        throw ref new Platform::Exception(HRESULT_FROM_WIN32(GetLastError()), L"Could not create shutdown event");
    }

    hStartCompleted = CreateEventEx(NULL, NULL, CREATE_EVENT_MANUAL_RESET, EVENT_ALL_ACCESS);
    if (!hStartCompleted)
    {
        throw ref new Platform::Exception(HRESULT_FROM_WIN32(GetLastError()), L"Could not create start event");
    }
}

void BackEndCapture::Start(CameraLocation newCameraLocation)
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if (started)
        return;
    if(newCameraLocation == CameraLocation::Front)
    {
        cameraLocation = Windows::Phone::Media::Capture::CameraSensorLocation::Front;
    }
    else if(newCameraLocation == CameraLocation::Back)
    {
        cameraLocation = Windows::Phone::Media::Capture::CameraSensorLocation::Back;
    }

    InitCapture();

    started = true;
}

void BackEndCapture::Stop()
{
    // Make sure only one API call is in progress at a time
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);
    ::OutputDebugString(L"+[BackendCapture::Stop] => Trying to stop capture\n");
    if (!started)
    {
        ::OutputDebugString(L"-[BackendCapture::Stop] => finished stopping capture\n");
        return;
    }
    if (videoOnlyDevice)
    {
        OutputDebugString(L"Destroying VideoCaptureDevice\n");

        try
        {
            videoOnlyDevice->StopRecordingAsync()->Completed = ref new AsyncActionCompletedHandler([this] (IAsyncAction ^action, Windows::Foundation::AsyncStatus status){
                if(status == Windows::Foundation::AsyncStatus::Completed)
                {
                    OutputDebugString(L"[BackendCapture::StopRecordingAsync]  Video successfully stopped.\n");
                }
                else
                {
                    OutputDebugString(L"[BackEndCapture::StopRecordingAsync] Error occurred while stopping recording.\n");
                }
                this->videoCaptureAction = nullptr;
                this->videoOnlyDevice = nullptr;
                started = false;
                SetEvent(hStopCompleted);
            });
        }
        catch(...)
        {
            // A Platform::ObjectDisposedException can be raised if the app has had its access
            // to video revoked (most commonly when the app is going out of the foreground)
            OutputDebugString(L"Exception caught while destroying video capture\n");
            this->videoCaptureAction = nullptr;
            this->videoOnlyDevice = nullptr;
            started = false;
            SetEvent(hStopCompleted);
        }

        if (pVideoDevice)
        {
            pVideoDevice->Release();
            pVideoDevice = NULL;
        }

        if (pVideoSink)
        {
            pVideoSink->Release();
            pVideoSink = NULL;
        }
    }
    ::OutputDebugString(L"-[BackendCapture::Stop] => finished stopping capture\n");
}

BackEndCapture::~BackEndCapture()
{
    if(m_ToggleThread)
    {
        m_ToggleThread->Cancel();
        m_ToggleThread->Close();
        m_ToggleThread = nullptr;
    }
}

void BackEndCapture::InitCapture()
{
    ::OutputDebugString(L"+[BackendCapture::InitCapture] => Initializing Capture\n");
    Windows::Foundation::Size dimensions;
    dimensions.Width = 640;
    dimensions.Height = 480;
    Collections::IVectorView<Size> ^availableSizes = AudioVideoCaptureDevice::GetAvailableCaptureResolutions(this->cameraLocation);
    Collections::IIterator<Windows::Foundation::Size> ^availableSizesIterator = availableSizes->First();

    IAsyncOperation<AudioVideoCaptureDevice^> ^openOperation = nullptr;
    while(!openOperation && availableSizesIterator->HasCurrent)
    {
        // TODO: You should select the appropriate resolution that's supported here,
        // TODO: and then setup your renderer with that selected res.
        // TODO: This shows how to iterate through all supported resolutions.  We're assuming 640x480 support
        if(availableSizesIterator->Current.Height == 480 && availableSizesIterator->Current.Width == 640)
        {
            openOperation = AudioVideoCaptureDevice::OpenForVideoOnlyAsync(this->cameraLocation, dimensions);
        }
        availableSizesIterator->MoveNext();
    }

    openOperation->Completed = ref new AsyncOperationCompletedHandler<AudioVideoCaptureDevice^>([this] (IAsyncOperation<AudioVideoCaptureDevice^> ^operation, Windows::Foundation::AsyncStatus status)
    {
        if(status == Windows::Foundation::AsyncStatus::Completed)
        {
            std::lock_guard<std::recursive_mutex> lock(g_apiLock);
            
            ::OutputDebugString(L"+[BackendCapture::InitCapture] => OpenAsyncOperation started\n");
            
            auto videoDevice = operation->GetResults();

            this->videoOnlyDevice = videoDevice;
            IAudioVideoCaptureDeviceNative *pNativeDevice = NULL; 
            HRESULT hr = reinterpret_cast<IUnknown*>(videoDevice)->QueryInterface(__uuidof(IAudioVideoCaptureDeviceNative), (void**) &pNativeDevice);
            
            if (NULL == pNativeDevice || FAILED(hr))
            {
                throw ref new FailureException("Unable to QI IAudioVideoCaptureDeviceNative");
            }

            // Save off the native device
            this->pVideoDevice = pNativeDevice;

            // Create the sink
            MakeAndInitialize<CaptureSampleSink>(&(this->pVideoSink), transportController);
            this->pVideoSink->SetTransport(this->transportController);
            pNativeDevice->SetVideoSampleSink(this->pVideoSink);

            // Use the same encoding format as in VideoMediaStreamSource.cs
            videoDevice->VideoEncodingFormat = CameraCaptureVideoFormat::H264;

            SetEvent(hStartCompleted);

            // Start recording to our sink
            this->videoCaptureAction = videoDevice->StartRecordingToSinkAsync();
            videoCaptureAction->Completed = ref new AsyncActionCompletedHandler([this] (IAsyncAction ^asyncInfo, Windows::Foundation::AsyncStatus status)
            {
                if(status == Windows::Foundation::AsyncStatus::Completed)
                {
                    ::OutputDebugString(L"[BackendCapture::InitCapture] => StartRecordingToSinkAsync completed\n");
                }
                else if(status == Windows::Foundation::AsyncStatus::Error || status == Windows::Foundation::AsyncStatus::Canceled)
                {
                    ::OutputDebugString(L"[BackendCapture::InitCapture] => StartRecordingToSinkAsync did not complete\n");
                } 
            });

            ::OutputDebugString(L"-[BackendCapture::InitCapture] => OpenAsyncOperation Completed\n");
        }
        else if(status == Windows::Foundation::AsyncStatus::Canceled)
        {
            ::OutputDebugString(L"[BackendCapture::InitCapture] => OpenAsyncOperation Canceled\n");
        }
        else if(status == Windows::Foundation::AsyncStatus::Error)
        {
            ::OutputDebugString(L"[BackendCapture::InitCapture] => OpenAsyncOperation encountered an error.\n");
        }
    });
    ::OutputDebugString(L"-[BackendCapture::InitCapture] => Initializing Capture\n");
}

void BackEndCapture::ToggleCamera()
{
    std::lock_guard<std::recursive_mutex> lock(g_apiLock);

    if(m_ToggleThread)
    {
        m_ToggleThread->Cancel();
        m_ToggleThread->Close();
        m_ToggleThread = nullptr;
    }

    m_ToggleThread = ThreadPool::RunAsync(ref new WorkItemHandler(this, &BackEndCapture::ToggleCameraThread), WorkItemPriority::High, WorkItemOptions::TimeSliced);
}


void BackEndCapture::ToggleCameraThread(Windows::Foundation::IAsyncAction^ operation)
{
    ::OutputDebugString(L"+[BackendCapture::ToggleCamera] => Toggling camera \n");
    CameraLocation newCameraLocation;
    ResetEvent(hStopCompleted);
    Stop();
    DWORD waitResult = WaitForSingleObjectEx(hStopCompleted, INFINITE, FALSE);
    if(waitResult == WAIT_OBJECT_0)
    {
        ResetEvent(hStartCompleted);
        if(cameraLocation == Windows::Phone::Media::Capture::CameraSensorLocation::Back)
        {
            newCameraLocation = CameraLocation::Front;
        }
        else
        {
            newCameraLocation = CameraLocation::Back;
        }
        Start(newCameraLocation);
    }
    else
    {
        throw ref new Platform::Exception(HRESULT_FROM_WIN32(waitResult), L"Error waiting for capture to stop when toggling cameras");
    }

    waitResult = WaitForSingleObjectEx(hStartCompleted, INFINITE, FALSE);
    if(waitResult == WAIT_OBJECT_0)
    {
        CameraLocationChanged(newCameraLocation);
    }
    else
    {
        throw ref new Platform::Exception(HRESULT_FROM_WIN32(waitResult), L"Error waiting for capture to start when toggling cameras");
    }
    ::OutputDebugString(L"-[BackendCapture::ToggleCamera] => Toggling camera \n");
}

void BackEndCapture::SetTransport(BackEndTransport^ transport)
{
    transportController = transport;
}
