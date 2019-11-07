//
//    Copyright (C) Microsoft.  All rights reserved.
//
/*++
 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Module Name:

    Queue.cpp

Abstract:

    This module contains the implementation of the sensors service driver
    queue callback object.

--*/

#include "internal.h"
#include "Device.h"
#include "Queue.h"

#include "Queue.tmh"

/////////////////////////////////////////////////////////////////////////
//
// CMyQueue::CMyQueue
//
// Object constructor function
//
// Initialize member variables
//
/////////////////////////////////////////////////////////////////////////
CMyQueue::CMyQueue() :
    m_pParentDevice(NULL)
{
}

/////////////////////////////////////////////////////////////////////////
//
// CMyQueue::~CMyQueue
//
// Object destructor function
//
//
/////////////////////////////////////////////////////////////////////////
CMyQueue::~CMyQueue()
{
    // Release the reference that was incremented in CreateInstance()
    SAFE_RELEASE(m_pParentDevice);   
}

/////////////////////////////////////////////////////////////////////////
//
// CMyQueue::CreateInstance
//
// This function supports the COM factory creation system
//
// Parameters:
//      parentDevice    - A pointer to the CWSSDevice object
//      ppUkwn          - pointer to a pointer to the queue to be returned
//      ppQueue         - The queue that is created
//
// Return Values:
//      S_OK: The queue was created successfully
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyQueue::CreateInstance(_In_ IWDFDevice*  pWdfDevice, CMyDevice* pMyDevice, IWDFIoQueue** ppQueue)
{
    HRESULT hr = (NULL != pMyDevice) ? S_OK : E_UNEXPECTED;
    
    if(NULL == pMyDevice)
    {
        return E_INVALIDARG;
    }
    else if (nullptr == ppQueue)
    {
        return E_INVALIDARG;
    }

    *ppQueue = nullptr;

    CComObject<CMyQueue>* pMyQueue = NULL;
    if(SUCCEEDED(hr))
    {
        hr = CComObject<CMyQueue>::CreateInstance(&pMyQueue);
    }

    if(SUCCEEDED(hr) && (nullptr != pMyQueue))
    {
        // AddRef the object
        pMyQueue->AddRef();

        // Store the parent device object
        pMyQueue->m_pParentDevice = pMyDevice;

        // Increment the reference for the lifetime of the CMyQueue object.
        pMyQueue->m_pParentDevice->AddRef();

        CComPtr<IUnknown> spIUnknown;
        hr = pMyQueue->QueryInterface(IID_IUnknown, (void**)&spIUnknown);

        if(SUCCEEDED(hr))
        {
            // Create the framework queue
            hr = pWdfDevice->CreateIoQueue( spIUnknown,
                                            TRUE,                        // DefaultQueue
                                            WdfIoQueueDispatchParallel,  // Parallel queue handling 
                                            FALSE,                       // Non PowerManaged so I/O is self managed and
                                                                         // requests can be received while in Dx
                                            TRUE,                        // AllowZeroLengthRequests
                                            ppQueue
                                            );
            if (FAILED(hr))
            {
                Trace( TRACE_LEVEL_ERROR, "%!FUNC!: Could not create default I/O queue, %!hresult!", hr);
            }
        }

        // Release the pMyQueue pointer when done. Note: UMDF holds a reference to it above
        SAFE_RELEASE(pMyQueue);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyQueue::OnDeviceIoControl
//
// This method is called when an IOCTL is sent to the device
//
// Parameters:
//      pQueue            - pointer to an IO queue
//      pRequest          - pointer to an IO request
//      ControlCode       - The IOCTL to process
//      InputBufferSizeInBytes - the size of the input buffer
//      OutputBufferSizeInBytes - the size of the output buffer
//
/////////////////////////////////////////////////////////////////////////
STDMETHODIMP_ (void) CMyQueue::OnDeviceIoControl(
    _In_ IWDFIoQueue*     pQueue,
    _In_ IWDFIoRequest*   pRequest,
    _In_ ULONG            ControlCode,
         SIZE_T           InputBufferSizeInBytes,
         SIZE_T           OutputBufferSizeInBytes
    )
{
    UNREFERENCED_PARAMETER(pQueue);
    UNREFERENCED_PARAMETER(InputBufferSizeInBytes);
    UNREFERENCED_PARAMETER(OutputBufferSizeInBytes);

    //Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");
    
    DWORD dwWritten = 0;

    if (IOCTL_GPS_RADIO_MANAGEMENT_GET_RADIO_STATE == ControlCode ||
        IOCTL_GPS_RADIO_MANAGEMENT_GET_PREVIOUS_RADIO_STATE == ControlCode ||
        IOCTL_GPS_RADIO_MANAGEMENT_SET_RADIO_STATE == ControlCode ||
        IOCTL_GPS_RADIO_MANAGEMENT_SET_PREVIOUS_RADIO_STATE == ControlCode
        )
    {
#if (NTDDI_VERSION >= NTDDI_WIN8)
        if (FAILED(EnterProcessing(PROCESSING_IQUEUECALLBACKDEVICEIOCONTROL)))
        {
            // Unsupported request
            pRequest->CompleteWithInformation(HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED), 0);
        }
        else
        {
            HRESULT hr = m_pParentDevice->ProcessIoControlRadioManagement(pRequest, ControlCode);
            pRequest->CompleteWithInformation(hr, sizeof(DEVICE_RADIO_STATE));
        } // processing in progress

        ExitProcessing(PROCESSING_IQUEUECALLBACKDEVICEIOCONTROL);
#endif
    }
    else if( IS_WPD_IOCTL( ControlCode ) )
    {
        if (FAILED(EnterProcessing(PROCESSING_IQUEUECALLBACKDEVICEIOCONTROL)))
        {
            // Unsupported request
            pRequest->CompleteWithInformation(HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED), 0);
        }
        else
        {
            m_pParentDevice->ProcessIoControl(  pQueue,
                                                pRequest,
                                                ControlCode,
                                                InputBufferSizeInBytes,
                                                OutputBufferSizeInBytes,
                                                &dwWritten);
        } // processing in progress

        ExitProcessing(PROCESSING_IQUEUECALLBACKDEVICEIOCONTROL);
    }
    else
    {
        // Unsupported request
        pRequest->CompleteWithInformation(HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED), 0);
    }

}

inline HRESULT CMyQueue::EnterProcessing(DWORD64 dwControlFlag)
{
    return m_pParentDevice->EnterProcessing(dwControlFlag);
}

inline void CMyQueue::ExitProcessing(DWORD64 dwControlFlag)
{
    m_pParentDevice->ExitProcessing(dwControlFlag);
}
