/*++

Copyright (c) Microsoft Corporation, All Rights Reserved

Module Name:

    umdf_vdev_sequentialqueue.cpp

Abstract:

    This file implements the I/O queue interface and performs
    the ioctl operations for sequential requests.

Environment:

    Windows User-Mode Driver Framework (WUDF)


--*/

#include <devioctl.h>
#include "umdf_vdev.h"
#include "wdf_common.h"
#include "wdf_vdev_api.h"

#include "umdf_vdev_sequentialqueue.tmh"

//
// This is a false positive in PreFast
// The object (queue) uses the COM model of release
// upon the refcount reaching 0, the object is then freed from 
// memory.
//
#pragma warning (disable: 28197 )



HRESULT
STDMETHODCALLTYPE
CVDevSequentialQueue::QueryInterface (
                                    _In_ REFIID InterfaceId,
                                    _Out_ PVOID *Object)
/*++

Routine Description:


    Query Interface

Aruments:
    
    Follows COM specifications

Return Value:

    HRESULT indication success or failure

--*/
{
    HRESULT hr = S_OK;

    Enter();

    if (IsEqualIID(InterfaceId, __uuidof(IQueueCallbackDeviceIoControl))) 
    {
        hr = S_OK;
        *Object = DeviceIoControl (); 

    } 
    else 
    {
        hr = CUnknown::QueryInterface (InterfaceId, Object);
    }

    ExitHR(hr);
    return hr;

}

HRESULT 
CVDevSequentialQueue::CreateInstance (
                                     _In_ PCVDevDevice Device,
                                     _In_ IWDFDevice * FxDevice,
                                     _Out_ PCVDevSequentialQueue * Queue)
/*++

Routine Description:


    CreateInstance creates an instance of the queue object.

Aruments:
    
    ppUkwn - OUT parameter is an IUnknown interface to the queue object

Return Value:

    HRESULT indication success or failure

--*/
{
    PCVDevSequentialQueue queue = NULL;
    HRESULT hr = S_OK;

    UNREFERENCED_PARAMETER (Device);

    Enter();

    queue = new CVDevSequentialQueue (FxDevice);

    if (NULL == queue)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = queue->Initialize ();
    }

    if (SUCCEEDED(hr)) 
    {
        *Queue = queue;
    }
    else
    {
        SAFE_RELEASE(queue);
    }

    ExitHR(hr);
    return hr;
}

HRESULT
CVDevSequentialQueue::Initialize ()
{
    HRESULT hr;

    Enter();

    {
        IUnknown *callback = QueryIUnknown();

        hr = m_FxDevice->CreateIoQueue (
            callback,
            false,                       
            WdfIoQueueDispatchSequential,
            true,                       
            true,
            &m_FxQueue);

        callback->Release();
    }

    if (FAILED (hr))
    {
        ExitHR(hr);
        return hr;
    }
    else
    {
        m_FxQueue->Release();
    }

    if (!InitializeCriticalSectionAndSpinCount(&m_Crit,0x80000400))
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
    }

    ExitHR(hr);
    return hr;
}

void
STDMETHODCALLTYPE
CVDevSequentialQueue::OnDeviceIoControl (
                                   _In_ IWDFIoQueue *FxQueue,
                                   _In_ IWDFIoRequest *wdfRequest,
                                   _In_ ULONG ControlCode,
                                   _In_ SIZE_T InputBufferSizeInBytes,
                                   _In_ SIZE_T OutputBufferSizeInBytes)
/*++

Routine Description:


    DeviceIoControl dispatch routine

Aruments:
    
    FxQueue - Framework Queue instance
    wdfRequest - Framework Request  instance
    ControlCode - IO Control Code
    InputBufferSizeInBytes - Lenth of input buffer
    OutputBufferSizeInBytes - Lenth of output buffer

    Always succeeds DeviceIoIoctl
Return Value:

    VOID

--*/
{
    HRESULT hrSend = S_OK;
    IWDFMemory * memory = NULL;
    IWDFMemory * outputMemory = NULL;
    SIZE_T bufSize = 0;

    UNREFERENCED_PARAMETER (FxQueue);

    Enter();
    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                "Request %p Code %08x InSize 0x%x OutSize 0x%x\n",
                wdfRequest, ControlCode, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);

    switch (ControlCode)
    {
    case IOCTL_ISOCH_ALLOCATE_BANDWIDTH:
        {

            if ((sizeof(ISOCH_ALLOCATE_BANDWIDTH) > InputBufferSizeInBytes) ||
                    (sizeof(ISOCH_ALLOCATE_BANDWIDTH) > OutputBufferSizeInBytes))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_ALLOCATE_BANDWIDTH request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ISOCH_ALLOCATE_BANDWIDTH));
            }
            else
            {

                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_ALLOCATE_BANDWIDTH request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_ALLOCATE_BANDWIDTH request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
              
                wdfRequest->Complete (hrSend);
            }

        }// case IOCTL_ISOCH_ALLOCATE_BANDWIDTH:
        break;

    case IOCTL_ISOCH_ALLOCATE_CHANNEL:
        {
            if ((sizeof(ISOCH_ALLOCATE_CHANNEL) > InputBufferSizeInBytes) ||
                    (sizeof(ISOCH_ALLOCATE_CHANNEL) > OutputBufferSizeInBytes))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_ALLOCATE_CHANNEL request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ISOCH_ALLOCATE_CHANNEL));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_ALLOCATE_CHANNEL request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_ALLOCATE_CHANNEL request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
              
                wdfRequest->Complete (hrSend);
            }
        } //   case IOCTL_ISOCH_ALLOCATE_CHANNEL
        break;

    case IOCTL_ISOCH_ALLOCATE_RESOURCES:
        { 
            if ((sizeof(ISOCH_ALLOCATE_RESOURCES) > InputBufferSizeInBytes) ||
                    (sizeof(ISOCH_ALLOCATE_RESOURCES) > OutputBufferSizeInBytes)) 
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_ALLOCATE_RESOURCES request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ISOCH_ALLOCATE_RESOURCES));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_ALLOCATE_RESOURCES request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_ALLOCATE_RESOURCES request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }

        } // case IOCTL_ISOCH_ALLOCATE_RESOURCES
        break;
  
    case IOCTL_ISOCH_ATTACH_BUFFERS:
    case IOCTL_ISOCH_DETACH_BUFFERS:
        {
            //
            // There are race conditions present at this time which prevent full implementation 
            // of the attach / detach routines.  The 1394 team is working on them.
            // 
            hrSend = HRESULT_FROM_WIN32(ERROR_NOINTERFACE);

            if (ControlCode == IOCTL_ISOCH_ATTACH_BUFFERS)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_ATTACH_BUFFERS request %p FAILED (%!HRESULT!)\n",
                            wdfRequest, hrSend);
            }
            else
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_DETACH_BUFFERS request %p FAILED (%!HRESULT!)\n",
                            wdfRequest, hrSend);
            }

            wdfRequest->Complete (hrSend);
        }
        break;

    case IOCTL_ISOCH_FREE_BANDWIDTH:
        {
            HANDLE hBandwidth = NULL;

            if (sizeof (HANDLE) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_FREE_BANDWIDTH request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (HANDLE));
            }
            else
            {
                wdfRequest->GetInputMemory (&memory);
                memory->Release ();

                hBandwidth = (HANDLE) memory->GetDataBuffer (&bufSize);

                if (NULL == hBandwidth || INVALID_HANDLE_VALUE == hBandwidth)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_FREE_BANDWIDTH request %p FAILED (Invalid parameter: hBandwidth %p)\n",
                                wdfRequest, hBandwidth);
                    wdfRequest->Complete (
                        HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
                  
                    hrSend = SubmitRequestToLower (wdfRequest);

                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_ISOCH_FREE_BANDWIDTH request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_ISOCH_FREE_BANDWIDTH request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }

                    wdfRequest->Complete (hrSend);
                }
            }
        } // case IOCTL_ISOCH_FREE_BANDWIDTH
        break;

    case IOCTL_ISOCH_FREE_CHANNEL:
        {
            if (sizeof (ULONG) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_FREE_CHANNEL request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ULONG));
            }
            else
            {   
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_FREE_CHANNEL request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_FREE_CHANNEL request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }

        } // case IOCTL_ISOCH_FREE_CHANNEL
        break; 

    case IOCTL_ISOCH_FREE_RESOURCES:
        {
            if (sizeof (HANDLE) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_FREE_RESOURCES request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (HANDLE));
            }
            else
            {
                PHANDLE IsochFree; 

                wdfRequest->GetInputMemory (&memory);
                memory->Release ();

                IsochFree = (PHANDLE) memory->GetDataBuffer (&bufSize);

                if ((NULL == IsochFree) || (INVALID_HANDLE_VALUE == IsochFree))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_FREE_RESOURCES request %p FAILED (Invalid parameter: IsochFree %p)\n",
                                wdfRequest, IsochFree);
                    wdfRequest->Complete (HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
               
                    hrSend = SubmitRequestToLower (wdfRequest);

                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_ISOCH_FREE_RESOURCES request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_ISOCH_FREE_RESOURCES request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }

                    wdfRequest->Complete (hrSend);
                }
            }
        } // case IOCTL_ISOCH_FREE_RESOURCES
        break; 

    case IOCTL_ISOCH_LISTEN:
        {
            if (sizeof (ISOCH_LISTEN) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_LISTEN request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ISOCH_LISTEN));
            }
            else
            {
                PISOCH_LISTEN IsochListen;

                wdfRequest->GetInputMemory (&memory);
                memory->Release ();

                IsochListen = (PISOCH_LISTEN) memory->GetDataBuffer (&bufSize);

                if ((NULL == IsochListen->hResource) || 
                    (INVALID_HANDLE_VALUE == IsochListen->hResource))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_LISTEN request %p FAILED (Invalid parameter: hResource %p)\n",
                                wdfRequest, IsochListen->hResource);
                    wdfRequest->Complete (
                        HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
                  
                    hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_LISTEN request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_LISTEN request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                    wdfRequest->Complete (hrSend);
                }
            }
        } // case IOCTL_ISOCH_LISTEN
        break; 

    case IOCTL_ISOCH_QUERY_CURRENT_CYCLE_TIME:
        {
            if (sizeof (CYCLE_TIME) > OutputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_QUERY_CURRENT_CYCLE_TIME request %p FAILED (Insufficient Buffer: Out 0x%x)\n",
                            wdfRequest, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (CYCLE_TIME));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_QUERY_CURRENT_CYCLE_TIME request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_QUERY_CURRENT_CYCLE_TIME request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }
        } // case IOCTL_ISOCH_QUERY_CURRENT_CYCLE_TIME
        break; 

    case IOCTL_ISOCH_SET_CHANNEL_BANDWIDTH:
        {
            if (sizeof (ISOCH_SET_CHANNEL_BANDWIDTH) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_SET_CHANNEL_BANDWIDTH request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ISOCH_SET_CHANNEL_BANDWIDTH));
            }
            else
            {
                PISOCH_SET_CHANNEL_BANDWIDTH ChannelBand;

                wdfRequest->GetInputMemory (&memory);
                memory->Release ();

                ChannelBand = \
                    (PISOCH_SET_CHANNEL_BANDWIDTH) memory->GetDataBuffer (&bufSize);

                if ((NULL == ChannelBand->hBandwidth) || 
                    (INVALID_HANDLE_VALUE == ChannelBand->hBandwidth))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_SET_CHANNEL_BANDWIDTH request %p FAILED (Invalid parameter: hBandwidth %p)\n",
                                wdfRequest, ChannelBand->hBandwidth);
                    wdfRequest->Complete (HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
                    hrSend = SubmitRequestToLower (wdfRequest);

                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_ISOCH_SET_CHANNEL_BANDWIDTH request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_ISOCH_SET_CHANNEL_BANDWIDTH request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }

                    wdfRequest->Complete (hrSend);
                }
            }
        } // case IOCTL_ISOCH_SET_CHANNEL_BANDWIDTH
        break;

    case IOCTL_ISOCH_STOP:
        {
            PISOCH_STOP IsochStop;
          
            if (sizeof (ISOCH_STOP) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_STOP request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ISOCH_STOP));
            }
            else
            {
                wdfRequest->GetInputMemory (&memory);

                IsochStop = (PISOCH_STOP) memory->GetDataBuffer(&bufSize);

                if ((NULL == IsochStop->hResource) || 
                    (INVALID_HANDLE_VALUE == IsochStop->hResource))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_STOP request %p FAILED (Invalid parameter: hResource %p)\n",
                                wdfRequest, IsochStop->hResource);
                    wdfRequest->Complete (
                        HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {   
                    hrSend = SubmitRequestToLower (wdfRequest);

                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_ISOCH_STOP request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_ISOCH_STOP request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }

                    wdfRequest->Complete (hrSend);
                }
            }
        } // case IOCTL_ISOCH_STOP
        break;

    case IOCTL_ISOCH_TALK:
    {
        PISOCH_TALK IsochTalk;
        
        if (sizeof (ISOCH_TALK) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_TALK request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ISOCH_TALK));
            }
        else
        {

            wdfRequest->GetInputMemory (&memory);
            memory->Release ();

            IsochTalk = (PISOCH_TALK) memory->GetDataBuffer (&bufSize);

            if ((NULL == IsochTalk->hResource) || 
                (INVALID_HANDLE_VALUE == IsochTalk->hResource))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ISOCH_TALK request %p FAILED (Invalid parameter: hResource %p)\n",
                            wdfRequest, IsochTalk->hResource);
                wdfRequest->Complete (
                    HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_TALK request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ISOCH_TALK request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }
        }
    } // case IOCTL_ISOCH_TALK
    break;
      
    case IOCTL_GET_1394_ADDRESS_FROM_DEVICE_OBJECT:
        {
            if ((sizeof (GET_1394_ADDRESS) > InputBufferSizeInBytes) ||
                (sizeof (GET_1394_ADDRESS) > OutputBufferSizeInBytes))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_GET_1394_ADDRESS_FROM_DEVICE_OBJECT request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (GET_1394_ADDRESS));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_GET_1394_ADDRESS_FROM_DEVICE_OBJECT request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_GET_1394_ADDRESS_FROM_DEVICE_OBJECT request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }
        } // case IOCTL_GET_1394_ADDRESS_FROM_DEVICE_OBJECT
        break;

    case IOCTL_GET_CONFIGURATION_INFORMATION:
        {
            //
            // This isn't implemented at this time until we can manufacture a 
            // good mechanism for this virtual device to support this request.
            //
            hrSend = HRESULT_FROM_WIN32(ERROR_NOINTERFACE);

            TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                        "IOCTL_GET_CONFIGURATION_INFORMATION request %p FAILED (%!HRESULT!)\n",
                        wdfRequest, hrSend);

            wdfRequest->Complete (hrSend);
        }// case IOCTL_GET_CONFIGURATION_INFORMATION
        break;

    case IOCTL_GET_GENERATION_COUNT:
        {
            if (sizeof (ULONG) > OutputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_GET_GENERATION_COUNT request %p FAILED (Insufficient Buffer: Out 0x%x)\n",
                            wdfRequest, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (ULONG));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_GET_GENERATION_COUNT request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_GET_GENERATION_COUNT request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }
        } // case IOCTL_GET_GENERATION_COUNT
        break;

    case IOCTL_GET_MAX_SPEED_BETWEEN_DEVICES:
        {
            if ((sizeof (GET_MAX_SPEED_BETWEEN_DEVICES) > InputBufferSizeInBytes) ||
                (sizeof (GET_MAX_SPEED_BETWEEN_DEVICES) > OutputBufferSizeInBytes))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_GET_MAX_SPEED_BETWEEN_DEVICES request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (GET_MAX_SPEED_BETWEEN_DEVICES));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_GET_MAX_SPEED_BETWEEN_DEVICES request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_GET_MAX_SPEED_BETWEEN_DEVICES request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }
                                    
        } //   case IOCTL_GET_MAX_SPEED_BETWEEN_DEVICES
        break;
        
    case IOCTL_SEND_PHY_CONFIGURATION_PACKET:
        {
            if (sizeof (PHY_CONFIGURATION_PACKET) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_SEND_PHY_CONFIGURATION_PACKET request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (PHY_CONFIGURATION_PACKET));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_SEND_PHY_CONFIGURATION_PACKET request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_SEND_PHY_CONFIGURATION_PACKET request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }
                                
        } // case IOCTL_SEND_PHY_CONFIGURATION_PACKET
        break;

    case IOCTL_SET_DEVICE_XMIT_PROPERTIES:
        {
            if (sizeof (DEVICE_XMIT_PROPERTIES) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_SET_DEVICE_XMIT_PROPERTIES request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                                    sizeof (DEVICE_XMIT_PROPERTIES));
            }
            else
            {
                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_SET_DEVICE_XMIT_PROPERTIES request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_SET_DEVICE_XMIT_PROPERTIES request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }
        } // case IOCTL_SET_DEVICE_XMIT_PROPERTIES
        break; 

    case IOCTL_GET_LOCAL_HOST_INFORMATION:
        {
            ULONG ulBufferSize = 0;
            PGET_LOCAL_HOST_INFORMATION GetLocalHostInfo, SubmitGetLocalHostInfo;
            DWORD dwRet = 0;

            wdfRequest->GetInputMemory (&memory);
            memory->Release ();

            GetLocalHostInfo = \
                (PGET_LOCAL_HOST_INFORMATION) memory->GetDataBuffer (&bufSize);

            wdfRequest->GetOutputMemory (&outputMemory);
            outputMemory->Release ();

            TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                        "IOCTL_GET_LOCAL_HOST_INFORMATION request %p nLevel 0x%x\n",
                        wdfRequest, GetLocalHostInfo->nLevel);

            switch (GetLocalHostInfo->nLevel)
            {
            case 1:
                ulBufferSize = sizeof(GET_LOCAL_HOST_INFO1);
                break;

            case 2:
                ulBufferSize = sizeof(GET_LOCAL_HOST_INFO2);
                break;

            case 3:
                ulBufferSize = sizeof(GET_LOCAL_HOST_INFO3);
                break;

            case 4:
                ulBufferSize = sizeof(GET_LOCAL_HOST_INFO4);
                break;

            case 7:
                ulBufferSize = sizeof(GET_LOCAL_HOST_INFO7);
                break;

            case 5:
            case 6:
            default:
                {
                    //
                    // We're not supporting LocalHost Information levels 
                    // 5 or 6 at this time.
                    // 
                    hrSend = HRESULT_FROM_WIN32 (ERROR_NOINTERFACE);

                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "Unsupported nLevel 0x%x (%!HRESULT!)\n",
                                GetLocalHostInfo->nLevel, hrSend);

                    wdfRequest->Complete (hrSend);

                    ExitHR(hrSend);
                    return;
                }
            } // switch (GetLocalHostInfo->nLevel)

            ulBufferSize += sizeof(GET_LOCAL_HOST_INFORMATION);

            SubmitGetLocalHostInfo = \
                (PGET_LOCAL_HOST_INFORMATION) LocalAlloc (LPTR, ulBufferSize);
            if (!SubmitGetLocalHostInfo)
            {
                dwRet = GetLastError ();
                hrSend = HRESULT_FROM_WIN32(dwRet);

                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "Buffer alloc %p FAILED (%!HRESULT!) 0x%x BufSize 0x%x\n",
                            wdfRequest, hrSend, dwRet, ulBufferSize);

                wdfRequest->Complete (hrSend);
            }
            else
            {
                SubmitGetLocalHostInfo = GetLocalHostInfo;
                outputMemory->SetBuffer (
                    (PVOID *) SubmitGetLocalHostInfo, 
                    ulBufferSize);

                hrSend = SubmitRequestToLower (wdfRequest);

                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_GET_LOCAL_HOST_INFORMATION request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_GET_LOCAL_HOST_INFORMATION request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }

                wdfRequest->Complete (hrSend);
            }
        } // case IOCTL_GET_LOCAL_HOST_INFORMATION
        break;

    case IOCTL_SET_LOCAL_HOST_INFORMATION:
        {
            PSET_LOCAL_HOST_INFORMATION SetLocalHostInfo;

            if ((sizeof (SET_LOCAL_HOST_INFORMATION) > InputBufferSizeInBytes) ||
                (sizeof (SET_LOCAL_HOST_INFORMATION) > OutputBufferSizeInBytes))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_SET_LOCAL_HOST_INFORMATION request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (SET_LOCAL_HOST_INFORMATION));
            }
            else
            {
                wdfRequest->GetInputMemory(&memory);

                SetLocalHostInfo = \
                    (PSET_LOCAL_HOST_INFORMATION) memory->GetDataBuffer(&bufSize);
                memory->Release ();

                wdfRequest->GetOutputMemory (&outputMemory);
                outputMemory->Release ();

                if ((sizeof (SET_LOCAL_HOST_INFORMATION) + \
                    SetLocalHostInfo->ulBufferSize) > \
                    InputBufferSizeInBytes)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_SET_LOCAL_HOST_INFORMATION request %p FAILED (Insufficient Buffer: In 0x%x Buffer 0x%x)\n",
                                wdfRequest, (UINT)InputBufferSizeInBytes, SetLocalHostInfo->ulBufferSize);
                    wdfRequest->CompleteWithInformation (
                        HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                        (sizeof (SET_LOCAL_HOST_INFORMATION) + 
                        SetLocalHostInfo->ulBufferSize));
                }
                else
                {
                    hrSend = SubmitRequestToLower (wdfRequest);

                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_SET_LOCAL_HOST_INFORMATION request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_SET_LOCAL_HOST_INFORMATION request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }

                    wdfRequest->Complete (hrSend);
                }
            }
        }
        break;

    case IOCTL_CONTROL:
        {
            hrSend = SubmitRequestToLower (wdfRequest);

            if (FAILED (hrSend))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_CONTROL request %p FAILED (%!HRESULT!)\n",
                            wdfRequest, hrSend);
                wdfRequest->Complete (hrSend);
            }
            else
            {
                TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                            "IOCTL_CONTROL request %p succeeded (%!HRESULT!)\n",
                            wdfRequest, hrSend);
            }
                                
        } // case IOCTL_CONTROL
        break;

    default:
        {
            hrSend = HRESULT_FROM_WIN32(ERROR_INVALID_FUNCTION);
            TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                        "Forwarded request %p FAILED (%!HRESULT!)\n",
                        wdfRequest, hrSend);
            wdfRequest->Complete (hrSend);
        }
    break;
    }

    ExitHR(hrSend);
    return;
}

HRESULT
CVDevSequentialQueue::SubmitRequestToLower (
                        _In_ IWDFIoRequest * Request)
/*++

Routine Description:
    A base function to manage serialized request submission to the next 
    lower object on the stack.

Arguments:
    Request - reqeuest to submit to the kernel drivers.

Return Value:
    HRESULT indication of success or failure

--*/
{
    HRESULT hr;

    Enter();

    IWDFIoTarget * kmdfIoTarget = NULL;
    
    this->m_FxDevice->GetDefaultIoTarget (&kmdfIoTarget);

    Request->FormatUsingCurrentType ();

    hr = Request->Send (
        kmdfIoTarget, 
        WDF_REQUEST_SEND_OPTION_SYNCHRONOUS, 
        0);

    ExitHR(hr);
    return (hr);
}



