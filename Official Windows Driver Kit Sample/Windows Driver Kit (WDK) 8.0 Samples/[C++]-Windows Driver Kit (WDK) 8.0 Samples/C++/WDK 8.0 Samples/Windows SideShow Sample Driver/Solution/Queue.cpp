//-----------------------------------------------------------------------
// <copyright file="Queue.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
// </copyright>
//
// Module:
//      Queue.cpp
//
// Description:
//      This file implements the basic I/O queue for the UMDF driver.
//
//-----------------------------------------------------------------------

#include "Common.h"
#include "Queue.h"
#include <DevIoctl.h>
#include <PortableDevice.h>


/////////////////////////////////////////////////////////////////////////
//
// CQueue::CreateInstance
//
// This function supports the COM factory creation system
//
// Parameters:
//      parentDevice    - A pointer to the CWSSDevice object
//      ppUkwn          - pointer to a pointer to the queue to be returned
//
// Return Values:
//      S_OK: The queue was created successfully
//
/////////////////////////////////////////////////////////////////////////
HRESULT CQueue::CreateInstance(WSSDevicePtr    parentDevice,
                               IUnknown**      ppUkwn)
{
    CComObject<CQueue>* pMyQueue = NULL;

    if (parentDevice == NULL)
    {
        // ERROR: Cannot create a Queue with a NULL device object
        return E_INVALIDARG;
    }

    HRESULT hr = CComObject<CQueue>::CreateInstance(&pMyQueue);
    if (SUCCEEDED(hr))
    {
        //
        // AddRef the object
        //
        pMyQueue->AddRef();

        //
        // Store the device object
        //
        if (SUCCEEDED(hr))
        {
            pMyQueue->m_parentDevice = parentDevice;

            //
            // Increment the reference for the lifetime of the CQueue object.
            //
            pMyQueue->m_parentDevice->AddRef();
        }

        //
        // Finally, QI for the IUnknown to return
        //
        if(SUCCEEDED(hr))
        {
            hr = pMyQueue->QueryInterface(__uuidof(IUnknown), (void **)ppUkwn);
        }

        //
        // Release the object, as we've returned the reference in ppUkwn and no longer need pMyQueue
        //
        pMyQueue->Release();
        pMyQueue = NULL;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CQueue::CQueue
//
// Object constructor function
//
// Initialize member variables
//
/////////////////////////////////////////////////////////////////////////
CQueue::CQueue() :
    m_parentDevice(NULL)
{
}


/////////////////////////////////////////////////////////////////////////
//
// CQueue::~CQueue
//
// Object destructor function
//
// Free up the buffer, wait for thread to terminate and
// delete critical section.
//
/////////////////////////////////////////////////////////////////////////
CQueue::~CQueue()
{
    //
    // Release the reference that was incremented in CreateInstance().
    //
    m_parentDevice->Release();
    m_parentDevice = NULL;
}


/////////////////////////////////////////////////////////////////////////
//
// CQueue::OnCreateFile
//
// This method is called when CreateFile is called on the device.
//
// Parameters:
//      pQueue      - [in] The IO queue handling the request
//      pRequest    - [in] The IO request containing the data
//      pFileObject - pointer to a file object
//
/////////////////////////////////////////////////////////////////////////
STDMETHODIMP_ (void)
CQueue::OnCreateFile(
    /*[in]*/ IWDFIoQueue*       pQueue,
    /*[in]*/ IWDFIoRequest*     pRequest,
    /*[in]*/ IWDFFile*          pFileObject
    )
{
    (pQueue);
    (pFileObject);
    pRequest->Complete(S_OK);
    return;
}


/////////////////////////////////////////////////////////////////////////
//
// CQueue::OnRead
//
// This method is called when a client tries to read from the device.
//
// Parameters:
//      pQueue           - pointer to an IO queue
//      pRequest         - pointer to an IO request with the data
//      NumOfBytesToRead - number of bytes the caller wants to read
//
//
/////////////////////////////////////////////////////////////////////////
STDMETHODIMP_ (void)
CQueue::OnRead(
        IWDFIoQueue*    pQueue,
        IWDFIoRequest*  pRequest,
        SIZE_T          NumOfBytesToRead
        )
{
    (pQueue);
    (pRequest);
    (NumOfBytesToRead);

    HRESULT hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);
    pRequest->CompleteWithInformation(hr, 0);
}



/////////////////////////////////////////////////////////////////////////
//
// CQueue::OnWrite
//
// This method is called when the caller tries to write to the device
//
// Parameters:
//      pQueue            - pointer to an IO queue
//      pRequest          - pointer to an IO request
//      NumOfBytesToWrite - number of bytes to write to the device
//
/////////////////////////////////////////////////////////////////////////
STDMETHODIMP_ (void)
CQueue::OnWrite(
        IWDFIoQueue*    pQueue,
        IWDFIoRequest*  pRequest,
        SIZE_T          NumOfBytesToWrite
        )
{
    (pQueue);
    (pRequest);
    (NumOfBytesToWrite);

    HRESULT hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);
    pRequest->CompleteWithInformation(hr, 0);
}


/////////////////////////////////////////////////////////////////////////
//
// CQueue::OnDeviceIoControl
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
STDMETHODIMP_ (void)
CQueue::OnDeviceIoControl(
        /* [in] */ IWDFIoQueue*     pQueue,
        /* [in] */ IWDFIoRequest*   pRequest,
        /* [in] */ ULONG            ControlCode,
        /* [in] */ SIZE_T           InputBufferSizeInBytes,
        /* [in] */ SIZE_T           OutputBufferSizeInBytes
        )
{
    //
    // Handle the WPD IOCTL
    //
    if (IS_WPD_IOCTL(ControlCode))
    {
        HRESULT hr = S_OK;
        DWORD dwWritten = 0;
        BOOL fCompleteRequest = FALSE;

        hr = m_parentDevice->ProcessIoControl(pQueue,
                                              pRequest,
                                              ControlCode,
                                              InputBufferSizeInBytes,
                                              OutputBufferSizeInBytes,
                                              &dwWritten,
                                              &fCompleteRequest);

        if (TRUE == fCompleteRequest)
        {
            // Complete the request only if Class Extension is ISideShowClassExtension and not ISideShowClassExtension2.
            pRequest->CompleteWithInformation(hr, dwWritten);
        }
    }
    else
    {
        //
        // We don't handle any other types of IOCTLs.
        //
        pRequest->CompleteWithInformation(HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED), 0);
    }
}
