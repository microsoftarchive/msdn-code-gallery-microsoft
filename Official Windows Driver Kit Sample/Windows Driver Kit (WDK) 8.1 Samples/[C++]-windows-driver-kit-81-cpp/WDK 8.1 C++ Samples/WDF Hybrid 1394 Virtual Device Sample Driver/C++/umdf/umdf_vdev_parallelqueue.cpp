/*++

Copyright (c) Microsoft Corporation, All Rights Reserved

Module Name:

umdf_vdev_parallelqueue.cpp

Abstract:

This file implements the I/O queue interface and performs
the ioctl operations.

Environment:

Windows User-Mode Driver Framework (WUDF)


--*/

#include <devioctl.h>
#include "umdf_vdev.h"
#include "wdf_common.h"
#include "wdf_vdev_api.h"

#include "umdf_vdev_parallelqueue.tmh"

//
// This is a false positive in PreFast
// The object (queue) uses the COM model of release
// upon the refcount reaching 0, the object is then freed from 
// memory.
//
#pragma warning (disable: 28197 )




CVDevParallelQueue::~CVDevParallelQueue()
{
    DeleteCriticalSection (&this->m_Crit);
}

HRESULT
STDMETHODCALLTYPE
CVDevParallelQueue::QueryInterface (
                                    _In_ REFIID InterfaceId,
                                    _Out_ PVOID *Object)
/*++

Routine Description:


    Query Interface

Aruments:

    Follows COM specifications

Return Value:

    HRESULT indicate success or failure

--*/
{
    HRESULT hr;

    Enter();

    if (IsEqualIID (
        InterfaceId, 
        __uuidof(IQueueCallbackDeviceIoControl))) 
    {
        hr = S_OK;
        *Object = DeviceIoControl (); 

    }
    else if (IsEqualIID (
        InterfaceId, 
        __uuidof (IRequestCallbackRequestCompletion)))
    {
        hr = S_OK;
        *Object = RequestCompletion ();    
    }
    else
    {
        hr = CUnknown::QueryInterface (InterfaceId, Object);
    }

    ExitHR(hr);
    return hr;

}

HRESULT 
CVDevParallelQueue::CreateInstance (
                                    _In_ PCVDevDevice Device,
                                    _In_ IWDFDevice * FxDevice,
                                    _Out_ PCVDevParallelQueue *Queue)
/*++

Routine Description:


    CreateInstance creates an instance of the queue object.

Arguments:

    ppUkwn - OUT parameter is an IUnknown interface to the queue object

Return Value:

    HRESULT indication success or failure

--*/
{
    PCVDevParallelQueue queue = NULL;
    HRESULT hr = S_OK;

    Enter();

    queue = new CVDevParallelQueue (Device, FxDevice);

    if (NULL == queue)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = queue->Initialize();
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
CVDevParallelQueue::Initialize()
{
    HRESULT hr;

    Enter();

    {
        IUnknown *callback = QueryIUnknown();

        hr = m_FxDevice->CreateIoQueue (
            callback,
            true,                       // This is the default queue object
            WdfIoQueueDispatchParallel,
            true,                       // Let the Framework handle PowMgmt.
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

VOID
STDMETHODCALLTYPE
CVDevParallelQueue::OnDeviceIoControl (
                                       _In_ IWDFIoQueue *FxQueue,
                                       _In_ IWDFIoRequest *wdfRequest,
                                       _In_ ULONG ControlCode,
                                       _In_ SIZE_T InputBufferSizeInBytes,
                                       _In_ SIZE_T OutputBufferSizeInBytes)
/*++

Routine Description:

    DeviceIoControl dispatch routine

Arguments:

    FxQueue - Framework Queue instance
    wdfRequest - Framework Request  instance
    ControlCode - IO Control Code
    InputBufferSizeInBytes - Length of input buffer
    OutputBufferSizeInBytes - Length of output buffer

Return Value:

    VOID

--*/
{
    HRESULT hr = S_OK;  
    HRESULT hrSend = S_OK;
    SIZE_T bufSize = 0;

    //
    // More often than not these will be the same block of memory
    // but to display support for more diverse scenarios, 
    // we'll loosely treat input and output buffers as different blocks.
    //
    IWDFMemory * memory = NULL;
    IWDFMemory * outputMemory = NULL;

    UNREFERENCED_PARAMETER (FxQueue);

    Enter();
    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                "Request %p Code %08x InSize 0x%x OutSize 0x%x\n",
                wdfRequest, ControlCode, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);

    switch (ControlCode)
    {
    case IOCTL_ASYNC_LOCK:
        {
            if ((sizeof (ASYNC_LOCK) > InputBufferSizeInBytes) ||
                (sizeof (ASYNC_LOCK) > OutputBufferSizeInBytes))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ASYNC_LOCK request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (ASYNC_LOCK));
            }
            else
            {
                IRequestCallbackRequestCompletion * completionRoutine = \
                    RequestCompletion ();

                wdfRequest->SetCompletionCallback (
                    completionRoutine,
                    (PVOID) &ControlCode);

                completionRoutine->Release ();

                hrSend = SubmitAsyncRequestToLower (wdfRequest);
                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ASYNC_LOCK request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                    wdfRequest->Complete (hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ASYNC_LOCK request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
            } // else
        }// case IOCTL_ASYNC_LOCK
        break;

    case IOCTL_ASYNC_READ:
        {
            PASYNC_READ AsyncRead;

            if (sizeof (ASYNC_READ) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ASYNC_READ request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (ASYNC_READ));
            }
            else
            {
                wdfRequest->GetInputMemory (&memory);
                memory->Release ();

                wdfRequest->GetOutputMemory (&outputMemory);
                outputMemory->Release ();

                AsyncRead = (PASYNC_READ) outputMemory->GetDataBuffer (&bufSize);

                if ((sizeof (ASYNC_READ) > OutputBufferSizeInBytes) ||
                    ((OutputBufferSizeInBytes - sizeof (ASYNC_READ)) > 
                    AsyncRead->nNumberOfBytesToRead))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ASYNC_READ request %p FAILED (Insufficient Buffer: Out 0x%x NumBytes 0x%x)\n",
                                wdfRequest, (UINT)OutputBufferSizeInBytes, AsyncRead->nNumberOfBytesToRead);
                    wdfRequest->CompleteWithInformation (
                        HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                        (sizeof (ASYNC_READ) + AsyncRead->nNumberOfBytesToRead));
                }
                else if (0 == AsyncRead->nNumberOfBytesToRead)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ASYNC_READ request %p FAILED (Invalid Parameter: NumBytes 0x%x)\n",
                                wdfRequest, AsyncRead->nNumberOfBytesToRead);
                    wdfRequest->Complete (
                        HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
                    IRequestCallbackRequestCompletion * completionRoutine = \
                        RequestCompletion ();

                    wdfRequest->SetCompletionCallback (
                        completionRoutine,
                        (PVOID) &ControlCode);

                    completionRoutine->Release ();

                    hrSend = SubmitAsyncRequestToLower (wdfRequest);
                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_ASYNC_READ request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                        wdfRequest->Complete (hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_ASYNC_READ request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                } // else
            } // else
        } // case IOCTL_ASYNC_READ
        break;

    case IOCTL_ASYNC_STREAM:
        {
            PASYNC_STREAM AsyncStream;

            if (sizeof (ASYNC_STREAM) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ASYNC_STREAM request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (ASYNC_STREAM));
            }
            else
            {
                wdfRequest->GetInputMemory (&memory);
                memory->Release ();

                AsyncStream = (PASYNC_STREAM) memory->GetDataBuffer (&bufSize);

                if ((OutputBufferSizeInBytes - sizeof (ASYNC_STREAM)) > 
                    AsyncStream->nNumberOfBytesToStream)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ASYNC_STREAM request %p FAILED (Insufficient Buffer: Out 0x%x Bytes 0x%x)\n",
                                wdfRequest, (UINT)OutputBufferSizeInBytes, AsyncStream->nNumberOfBytesToStream);
                    wdfRequest->CompleteWithInformation (
                        HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                        (sizeof (ASYNC_STREAM) + 
                        AsyncStream->nNumberOfBytesToStream));
                }
                else if (0 == AsyncStream->nNumberOfBytesToStream)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ASYNC_STREAM request %p FAILED (Invalid parameter: NumBytes 0x%x)\n",
                                wdfRequest, AsyncStream->nNumberOfBytesToStream);
                    wdfRequest->Complete (
                        HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
                    IRequestCallbackRequestCompletion * completionRoutine = \
                        RequestCompletion ();

                    wdfRequest->SetCompletionCallback (
                        completionRoutine,
                        (PVOID) &ControlCode);

                    completionRoutine->Release ();

                    hrSend = SubmitAsyncRequestToLower (wdfRequest);
                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_ASYNC_STREAM request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                        wdfRequest->Complete (hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_ASYNC_STREAM request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                } // else
            } // else
        } // case IOCTL_ASYNC_STREAM
        break;

    case IOCTL_ASYNC_WRITE:
        {
            PASYNC_WRITE AsyncWrite;

            if (sizeof (ASYNC_WRITE) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ASYNC_WRITE request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (ASYNC_WRITE));
            }
            else
            {
                wdfRequest->GetInputMemory (&memory);
                memory->Release ();

                AsyncWrite = (PASYNC_WRITE) memory->GetDataBuffer(&bufSize);

                if ((InputBufferSizeInBytes - sizeof (ASYNC_WRITE)) > 
                    AsyncWrite->nNumberOfBytesToWrite)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ASYNC_WRITE request %p FAILED (Insufficient Buffer: In 0x%x Bytes 0x%x)\n",
                                wdfRequest, (UINT)InputBufferSizeInBytes, AsyncWrite->nNumberOfBytesToWrite);
                    wdfRequest->CompleteWithInformation (
                        HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                        (sizeof (ASYNC_WRITE) + AsyncWrite->nNumberOfBytesToWrite));
                }
                else if (0 == AsyncWrite->nNumberOfBytesToWrite)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ASYNC_WRITE request %p FAILED (Invalid parameter: NumBytes 0x%x)\n",
                                wdfRequest, AsyncWrite->nNumberOfBytesToWrite);
                    wdfRequest->Complete (
                        HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
                    IRequestCallbackRequestCompletion * completionRoutine = \
                        RequestCompletion ();

                    wdfRequest->SetCompletionCallback (
                        completionRoutine,
                        (PVOID) &ControlCode);

                    completionRoutine->Release();

                    hrSend = SubmitAsyncRequestToLower (wdfRequest);
                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_ASYNC_WRITE request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                        wdfRequest->Complete (hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_ASYNC_WRITE request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                }
            } // else                   
        } // case IOCTL_ASYNC_WRITE
        break;

    case IOCTL_BUS_RESET:
        {
            if (sizeof (ULONG) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_BUS_RESET request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (ULONG));
            }
            else
            {
                IRequestCallbackRequestCompletion * completionRoutine = \
                    RequestCompletion ();

                wdfRequest->SetCompletionCallback (
                    completionRoutine,
                    (PVOID) &ControlCode);

                completionRoutine->Release();

                hrSend = SubmitAsyncRequestToLower (wdfRequest);             
                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_BUS_RESET request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                    wdfRequest->Complete (hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_BUS_RESET request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
            }
        } // case IOCTL_BUS_RESET
        break;

    case IOCTL_BUS_RESET_NOTIFICATION:
        {
            if (sizeof (ULONG) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_BUS_RESET_NOTIFICATION request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (ULONG));
            }
            else
            {
                IRequestCallbackRequestCompletion * completionRoutine = \
                    RequestCompletion ();

                wdfRequest->SetCompletionCallback (
                    completionRoutine,
                    (PVOID) &ControlCode);

                completionRoutine->Release();

                hrSend = SubmitAsyncRequestToLower (wdfRequest);
                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_BUS_RESET_NOTIFICATION request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                    wdfRequest->Complete (hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_BUS_RESET_NOTIFICATION request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
            }
        } // case IOCTL_BUS_RESET_NOTIFICATION
        break;

    case IOCTL_GET_ADDRESS_DATA :
        {
            //
            //    
            PGET_ADDRESS_DATA InputGetAddrData, OutputGetAddrData;

            if ((sizeof (GET_ADDRESS_DATA) > InputBufferSizeInBytes) ||
                (sizeof (GET_ADDRESS_DATA) > OutputBufferSizeInBytes))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_GET_ADDRESS_DATA request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (GET_ADDRESS_DATA));
            }
            else
            {
                wdfRequest->GetInputMemory (&memory);
                memory->Release ();

                wdfRequest->GetOutputMemory (&outputMemory);
                outputMemory->Release ();

                InputGetAddrData = \
                    (PGET_ADDRESS_DATA) memory->GetDataBuffer (&bufSize);

                OutputGetAddrData = \
                    (PGET_ADDRESS_DATA) outputMemory->GetDataBuffer (&bufSize);

                if ((InputBufferSizeInBytes - sizeof (GET_ADDRESS_DATA) <  
                    InputGetAddrData->nLength) ||
                    (OutputBufferSizeInBytes - sizeof (GET_ADDRESS_DATA) < 
                    OutputGetAddrData->nLength))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_GET_ADDRESS_DATA request %p FAILED (Insufficient Buffer: In 0x%x InLength 0x%x Out 0x%x OutLength 0x%x)\n",
                                wdfRequest, (UINT)InputBufferSizeInBytes, InputGetAddrData->nLength, 
                                (UINT)OutputBufferSizeInBytes, OutputGetAddrData->nLength);
                    wdfRequest->CompleteWithInformation (
                        HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                        (InputGetAddrData->nLength + sizeof (GET_ADDRESS_DATA)));
                }
                else if (0 == InputGetAddrData->nLength || 
                    0 == OutputGetAddrData->nLength)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_GET_ADDRESS_DATA request %p FAILED (Invalid Parameter: InLength 0x%x OutLength 0x%x)\n",
                                wdfRequest, InputGetAddrData->nLength, OutputGetAddrData->nLength);
                    wdfRequest->Complete (
                        HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
                    IRequestCallbackRequestCompletion * completionRoutine = \
                        RequestCompletion ();

                    wdfRequest->SetCompletionCallback (
                        completionRoutine,
                        (PVOID) &ControlCode);

                    completionRoutine->Release();

                    hrSend = SubmitAsyncRequestToLower (wdfRequest);
                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_GET_ADDRESS_DATA request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                        wdfRequest->Complete (hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_GET_ADDRESS_DATA request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                }
            } // else               
        } // case IOCTL_GET_ADDRESS_DATA 
        break;

    case IOCTL_SET_ADDRESS_DATA:
        {
            PSET_ADDRESS_DATA SetAddrData; 

            if (sizeof (SET_ADDRESS_DATA) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_SET_ADDRESS_DATA request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (SET_ADDRESS_DATA));
            }
            else
            {
                wdfRequest->GetInputMemory (&memory);
                memory->Release();

                SetAddrData = \
                    (PSET_ADDRESS_DATA) memory->GetDataBuffer (&bufSize);

                if ((InputBufferSizeInBytes - sizeof (SET_ADDRESS_DATA)) < 
                    SetAddrData->nLength)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_SET_ADDRESS_DATA request %p FAILED (Insufficient Buffer: In 0x%x nLength 0x%x)\n",
                                wdfRequest, (UINT)InputBufferSizeInBytes, SetAddrData->nLength);
                    wdfRequest->CompleteWithInformation (
                        HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                        (sizeof (SET_ADDRESS_DATA) + SetAddrData->nLength));
                }
                else if (0 == SetAddrData->nLength)
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_SET_ADDRESS_DATA request %p FAILED (Invalid Parameter: nLength 0x%x)\n",
                                wdfRequest, SetAddrData->nLength);
                    wdfRequest->Complete (
                        HRESULT_FROM_WIN32 (ERROR_INVALID_PARAMETER));
                }
                else
                {
                    IRequestCallbackRequestCompletion * completionRoutine = \
                        RequestCompletion ();

                    wdfRequest->SetCompletionCallback (
                        completionRoutine,
                        (PVOID) &ControlCode);

                    completionRoutine->Release ();

                    hrSend = SubmitAsyncRequestToLower (wdfRequest);
                    if (FAILED (hrSend))
                    {
                        TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                    "IOCTL_SET_ADDRESS_DATA request %p FAILED (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                        wdfRequest->Complete (hrSend);
                    }
                    else
                    {
                        TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                    "IOCTL_SET_ADDRESS_DATA request %p succeeded (%!HRESULT!)\n",
                                    wdfRequest, hrSend);
                    }
                }
            } // else
        } // case IOCTL_SET_ADDRESS_DATA
        break;

    case IOCTL_FREE_ADDRESS_RANGE:
        {
            if (sizeof (HANDLE) > InputBufferSizeInBytes)
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_FREE_ADDRESS_RANGE request %p FAILED (Insufficient Buffer: In 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (HANDLE));
            }
            else
            {                               
                IRequestCallbackRequestCompletion * completionRoutine = \
                    RequestCompletion ();

                wdfRequest->SetCompletionCallback (
                    completionRoutine,
                    (PVOID) &ControlCode);

                completionRoutine->Release ();

                hrSend = SubmitAsyncRequestToLower (wdfRequest);
                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_FREE_ADDRESS_RANGE request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                    wdfRequest->Complete (hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_FREE_ADDRESS_RANGE request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
            } 
        } // case IOCTL_FREE_ADDRESS_RANGE
        break;

    case IOCTL_ALLOCATE_ADDRESS_RANGE:
        {

            if ((sizeof (ALLOCATE_ADDRESS_RANGE) > InputBufferSizeInBytes) ||
                (sizeof (ALLOCATE_ADDRESS_RANGE) > OutputBufferSizeInBytes))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "IOCTL_ALLOCATE_ADDRESS_RANGE request %p FAILED (Insufficient Buffer: In 0x%x Out 0x%x)\n",
                            wdfRequest, (UINT)InputBufferSizeInBytes, (UINT)OutputBufferSizeInBytes);
                wdfRequest->CompleteWithInformation (
                    HRESULT_FROM_WIN32 (ERROR_INSUFFICIENT_BUFFER),
                    sizeof (ALLOCATE_ADDRESS_RANGE));
            }
            else
            {
                IRequestCallbackRequestCompletion * completionRoutine = \
                    RequestCompletion ();
                
                wdfRequest->SetCompletionCallback (
                    completionRoutine,
                    (PVOID) &ControlCode);

                completionRoutine->Release ();
                
                hrSend = SubmitAsyncRequestToLower (wdfRequest);
                if (FAILED (hrSend))
                {
                    TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                                "IOCTL_ALLOCATE_ADDRESS_RANGE request %p FAILED (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                    wdfRequest->Complete (hrSend);
                }
                else
                {
                    TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                                "IOCTL_ALLOCATE_ADDRESS_RANGE request %p succeeded (%!HRESULT!)\n",
                                wdfRequest, hrSend);
                }
            }
        } // case IOCTL_ALLOCATE_ADDRESS_RANGE
        break;

    default:
        {
            // 
            // This isn't a request for this queue to handle, 
            // so we'll forward it on to the Serialzed queue
            //
            wdfRequest->ForwardToIoQueue (
                m_VdevDevice->GetSequentialQueue()->GetFxQueue());
            if (FAILED (hrSend))
            {
                TraceEvents(TRACE_LEVEL_ERROR, TEST_TRACE_QUEUE,
                            "Forwarded request %p FAILED (%!HRESULT!)\n",
                            wdfRequest, hrSend);
                wdfRequest->Complete (hr);
            }
            else
            {
                TraceEvents(TRACE_LEVEL_INFORMATION, TEST_TRACE_QUEUE,
                            "Forwarded request %p succeeded (%!HRESULT!)\n",
                            wdfRequest, hrSend);
            }
        }
        break;
    } // switch

    ExitHR(hr);
    return;
}

void
CVDevParallelQueue::OnCompletion (
                                  _In_ IWDFIoRequest*  pWdfRequest,
                                  _In_ IWDFIoTarget*  pIoTarget,
                                  _In_ IWDFRequestCompletionParams*  pParams,
                                  _In_ PVOID  pContext)
/*++

Routine Description:
    Base request completion and dispatch routine.

Arguments:
    pWdfRequest - current reqeust.
    pIoTarget - current I/O Target.
    pParams - Completion params.
    pContext - completion context.

Return Value:
    VOID

--*/
{
    ULONG ControlCode;
    HRESULT hr = pParams->GetCompletionStatus ();

    UNREFERENCED_PARAMETER (pIoTarget);
    
    Enter();

    if (FAILED (hr))
    {
        pWdfRequest->Complete (hr);
        ExitHR(hr);
        return;
    }

    if (NULL != pContext)
    {
        ControlCode = PtrToUlong (pContext);

        switch (ControlCode)
        {
            //
            // These are place holder functions to demonstrate
            // a method for dispatching to specific completion routines
            //
        case IOCTL_ALLOCATE_ADDRESS_RANGE:
            {
                this->OnAllocateAddrRangeCompletion (pWdfRequest, pParams);
            } 
            break;

        case IOCTL_FREE_ADDRESS_RANGE:
            {
                this->OnFreeAddrRangeCompletion (pWdfRequest, pParams);
            } 
            break; 

        default:
            {
                pWdfRequest->Complete (pParams->GetCompletionStatus());
            }
            break;
        } // switch
    }
    else
    {

        //
        // if we get here, just complete the request
        //

        pWdfRequest->Complete (hr);
    }
    ExitHR(hr);
    return;
}

void
CVDevParallelQueue::OnAllocateAddrRangeCompletion (
    _In_ IWDFIoRequest *  pWdfRequest,
    _In_ IWDFRequestCompletionParams*  pParams)
/*++

Routine Description:
    A support function to manage the completion of the 
    IOCTL_ALLOCATE_ADDRESS_RANGE request.

Arguments:
    pWdfRequest - current reqeust.
    pParams - Completion params.

Return Value:
    HRESULT indication of success or failure

    --*/
{
    HRESULT hr = pParams->GetCompletionStatus ();

    Enter();

    if (FAILED (hr))
    {
        pWdfRequest->Complete (hr);
        ExitHR(hr);
        return;
    }

    //
    // TODO: Implement any code necessary to manage the address range
    // structure sent back to us.
    //

    pWdfRequest->ReleaseLock ();
    pWdfRequest->Complete (hr);

    ExitHR(hr);
    return;

}

void
CVDevParallelQueue::OnFreeAddrRangeCompletion (
    _In_ IWDFIoRequest *  pWdfRequest,
    _In_ IWDFRequestCompletionParams*  pParams)
/*++

Routine Description:
    A support function to manage the completion of the 
    IOCTL_FREE_ADDRESS_RANGE request.

Arguments:
    pWdfRequest - current reqeust.
    pParams - Completion params.

Return Value:
    VOID

    --*/
{
    HRESULT hr = pParams->GetCompletionStatus ();

    Enter();

    //
    // If the request failed below us, we should not 
    // proceed w/ the release of resources
    // until the driver is torn down or the request is 
    // submitted again and succeeded.
    //
    if (FAILED (hr))
    {
        pWdfRequest->Complete (hr);
        ExitHR(hr);
        return;
    }

    //
    // TODO: Implement any code necessary to manage the release of address range
    // structure.
    //

    pWdfRequest->ReleaseLock ();
    pWdfRequest->Complete (hr);

    ExitHR(hr);
    return;
}

HRESULT
CVDevParallelQueue::SubmitAsyncRequestToLower (
    _In_ IWDFIoRequest * Request)
    
/*++

Routine Description:
    A base function to manage parallel request submission 
    to the next lower object on the stack.

Arguments:
    Request - the current request.

Return Value:
    HRESULT indication of success or failure

    --*/
{
    HRESULT hr;

    IWDFIoTarget * kmdfIoTarget = NULL;
    
    Enter();

    this->GetFxDevice()->GetDefaultIoTarget (&kmdfIoTarget);

    Request->FormatUsingCurrentType();


    hr = Request->Send (
        kmdfIoTarget, 
        0,  // Submits Asynchronous
        0);

    ExitHR(hr);
    return (hr);
}


