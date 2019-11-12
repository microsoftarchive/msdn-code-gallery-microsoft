/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    SpbRequest.cpp

Abstract:

    This module contains the implementation of the SPB accelerometer's
    SPB request class.

--*/


#include "Internal.h"
#include "Spb.h"

#include "SpbRequest.h"
#include "SpbRequest.tmh"


/////////////////////////////////////////////////////////////////////////
//
//  CSpbRequest::CSpbRequest()
//
//  Constructor.
//
/////////////////////////////////////////////////////////////////////////
CSpbRequest::CSpbRequest() :
    m_spWdfDriver(nullptr),
    m_spWdfDevice(nullptr),
    m_spRemoteTarget(nullptr),
    m_spWdfIoRequest(nullptr),
    m_spWdfMemory(nullptr),
    m_fInitialized(FALSE)
{

}

/////////////////////////////////////////////////////////////////////////
//
//  CSpbRequest::~CSpbRequest()
//
//  Destructor
//
/////////////////////////////////////////////////////////////////////////
CSpbRequest::~CSpbRequest()
{
    m_fInitialized = FALSE;

    // Close the handle to the SPB controller
    if (m_spRemoteTarget != nullptr)
    {
        m_spRemoteTarget->Close();
        m_spRemoteTarget->DeleteWdfObject();
    }
}

/////////////////////////////////////////////////////////////////////////
//
//  CSpbRequest::Initialize
//
//  This method is used to initialize the SPB request object.  A file handle
//  is opened to the underlying SPB controller.
//
//  Parameters:
//      pDevice - pointer to a device object
//      pszTargetPath - device path used to send requests
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSpbRequest::Initialize(
    _In_  IWDFDevice*        pWdfDevice,
    _In_  PCWSTR             pszTargetPath
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (m_fInitialized == FALSE)
    {
        if ((pWdfDevice == nullptr) ||
            (pszTargetPath == nullptr))
        {
            hr = E_INVALIDARG;
        }
        else
        {
            // Save interface pointers
            m_spWdfDevice = pWdfDevice;
            m_spWdfDevice->GetDriver(&m_spWdfDriver);
        }

        if (SUCCEEDED(hr))
        {
            // Open remote target to SPB controller

            CComPtr<IWDFDevice2> spDevice2;

            hr = m_spWdfDevice->QueryInterface(
                IID_PPV_ARGS(&spDevice2));

            if (SUCCEEDED(hr))
            {
                hr = spDevice2->CreateRemoteTarget(
                    nullptr,
                    nullptr,
                    &m_spRemoteTarget);

                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to create remote target, %!HRESULT!",
                        hr);
                }
            }

            if (SUCCEEDED(hr))
            {
                UMDF_IO_TARGET_OPEN_PARAMS openParams;

                openParams.dwShareMode = 0;
                openParams.dwCreationDisposition = OPEN_EXISTING;
                openParams.dwFlagsAndAttributes = FILE_FLAG_OVERLAPPED;

                hr = m_spRemoteTarget->OpenFileByName(
                    pszTargetPath,
                    (GENERIC_READ | GENERIC_WRITE),
                    &openParams);

                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to open remote target to SPB controller, "
                        "%!HRESULT!",
                        hr);
                }
            }
        }

        // Create a new request
        if (SUCCEEDED(hr))
        {
            hr = m_spWdfDevice->CreateRequest(
                nullptr, 
                m_spRemoteTarget, 
                &m_spWdfIoRequest);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to create the WDF request object, %!HRESULT!",
                    hr);
            }
        }

        if (SUCCEEDED(hr))
        {
            m_fInitialized = TRUE;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSpbRequest::CreateAndSendWrite
//
//  This method is used to create and send a new SPB write request.
//
//  Parameters:
//      pInBuffer - pointer to the input buffer
//      inBufferSize - size of the input buffer
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSpbRequest::CreateAndSendWrite(
    _In_reads_(inBufferSize)  BYTE*   pInBuffer,
    _In_                      SIZE_T  inBufferSize
    )
{
    FuncEntry();

    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    CComQIPtr<IWDFIoRequest2> spWdfIoRequest2(m_spWdfIoRequest);

    if (SUCCEEDED(hr))
    {
        if ((pInBuffer == nullptr) ||
            (inBufferSize == 0))
        {
            hr = E_INVALIDARG;
        }
    }

    if (SUCCEEDED(hr))
    {
        if (m_spWdfMemory == nullptr)
        {
            // Create input memory the first time around,
            // and for subsequent requests just reuse it
            hr = m_spWdfDriver->CreatePreallocatedWdfMemory(
                pInBuffer, 
                inBufferSize, 
                nullptr,            // no object event callback
                m_spRemoteTarget,   // remote target object as parent
                &m_spWdfMemory);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to create the WDF memory, %!HRESULT!",
                    hr);
            }
        }
        else
        {
            m_spWdfMemory->SetBuffer(pInBuffer, inBufferSize);
        }
        
        if (SUCCEEDED(hr))
        {
            spWdfIoRequest2->Reuse(S_OK);

            hr = m_spRemoteTarget->FormatRequestForWrite( 
                m_spWdfIoRequest,
                nullptr,
                m_spWdfMemory, 
                nullptr, 
                0);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to format the WDF request for write, %!HRESULT!",
                    hr);
            }
        }

        // Send down the request
        if (SUCCEEDED(hr))
        {
            hr = m_spWdfIoRequest->Send(
                m_spRemoteTarget,
                (WDF_REQUEST_SEND_OPTION_SYNCHRONOUS | 
                    WDF_REQUEST_SEND_OPTION_TIMEOUT),
                SPB_REQUEST_TIMEOUT
                );

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to send the WDF request, %!HRESULT!",
                    hr);
            }
        }

        if (SUCCEEDED(hr))
        {
            CComPtr<IWDFRequestCompletionParams> spParams = nullptr;
            m_spWdfIoRequest->GetCompletionParams(&spParams);

            hr = spParams->GetCompletionStatus();

            if (SUCCEEDED(hr))
            {
                if (spParams->GetInformation() != inBufferSize)
                {
                    hr = HRESULT_FROM_WIN32(ERROR_BAD_LENGTH);
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Request completed with %lu bytes, expected %lu, "
                        "%!HRESULT!",
                        (ULONG)spParams->GetInformation(),
                        (ULONG)inBufferSize,
                        hr);
                }
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSpbRequest::CreateAndSendWriteReadSequence
//
//  This method is used to create and send a new SPB sequence request.
//
//  Parameters:
//      pInBuffer - pointer to the input buffer
//      inBufferSize - size of the input buffer
//      pOutBuffer - pointer to the output buffer
//      outBufferSize - size of the output buffer
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
#pragma warning(suppress: 6001 6101) // PREFast cannot understand the use of the IOCTL to write to pOutBuffer
HRESULT CSpbRequest::CreateAndSendWriteReadSequence(
    _In_reads_(inBufferSize)     BYTE*   pInBuffer,
    _In_                         SIZE_T  inBufferSize,
    _Out_writes_(outBufferSize)  BYTE*   pOutBuffer,
    _In_                         SIZE_T  outBufferSize,
    _In_                         ULONG   delayInUs
    )
{
    FuncEntry();

    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr))
    {
        if ((pInBuffer == nullptr) ||
            (inBufferSize == 0) ||
            (pOutBuffer == nullptr) ||
            (outBufferSize == 0))
        {
            hr = E_INVALIDARG;
        }
    }

    if (SUCCEEDED(hr))
    {
        const ULONG transfers = 2;

        SPB_TRANSFER_LIST_AND_ENTRIES(transfers) seq;
        SPB_TRANSFER_LIST_INIT(&(seq.List), transfers);

        // Build the write-read sequence from the provided
        // buffer parameters.
        {
            //
            // PreFAST cannot figure out the SPB_TRANSFER_LIST_ENTRY
            // "struct hack" size but using an index variable quiets 
            // the warning. This is a false positive from OACR.
            // 

            ULONG index = 0;
            seq.List.Transfers[index] = SPB_TRANSFER_LIST_ENTRY_INIT_SIMPLE(
                SpbTransferDirectionToDevice,
                0,
                pInBuffer,
                (ULONG)inBufferSize);

#pragma warning(suppress: 6001 6101 ) // PREFast cannot understand the use of the IOCTL to write to pOutBuffer
            seq.List.Transfers[index + 1] = SPB_TRANSFER_LIST_ENTRY_INIT_SIMPLE(
                SpbTransferDirectionFromDevice,
                delayInUs,
                pOutBuffer,
                (ULONG)outBufferSize);
        }

        ULONG_PTR bytesTransferred;

        // Send the sequence in the form of an IOCTL request
        hr = CreateAndSendIoctl(
            IOCTL_SPB_EXECUTE_SEQUENCE,
            (BYTE*)&seq,
            sizeof(seq),
            &bytesTransferred);

        if (SUCCEEDED(hr))
        {
            if (bytesTransferred != (inBufferSize + outBufferSize))
            {
                hr = HRESULT_FROM_WIN32(ERROR_BAD_LENGTH);
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Request completed with %lu bytes, expected %lu, "
                    "%!HRESULT!",
                    (ULONG)bytesTransferred,
                    (ULONG)(inBufferSize + outBufferSize),
                    hr);
            }
        }

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to send the write-read sequence, %!HRESULT!",
                hr);
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSpbRequest::CreateAndSendIoctl
//
//  This helper method is used to create and send a new ioctl request.
//
//  Parameters:
//      ioctlCode - IO control code to use
//      pInBuffer - pointer to the input buffer
//      inBufferSize - size of the input buffer
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSpbRequest::CreateAndSendIoctl(
    _In_                      ULONG       ioctlCode,
    _In_reads_(inBufferSize)  BYTE*       pInBuffer,
    _In_                      SIZE_T      inBufferSize,
    _Out_                     ULONG_PTR*  pbytesTransferred
    )
{
    FuncEntry();

    HRESULT hr = (m_fInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    CComQIPtr<IWDFIoRequest2> spWdfIoRequest2(m_spWdfIoRequest);

    if (SUCCEEDED(hr))
    {
        if ((pInBuffer == nullptr) ||
            (inBufferSize == 0))
        {
            hr = E_INVALIDARG;
        }
    }

    if (SUCCEEDED(hr))
    {
        if (m_spWdfMemory == nullptr)
        {
            // Create input memory the first time around,
            // and for subsequent requests just reuse it
            hr = m_spWdfDriver->CreatePreallocatedWdfMemory(
                pInBuffer, 
                inBufferSize, 
                nullptr,            // no object event callback
                m_spWdfIoRequest,   // remote target object as parent
                &m_spWdfMemory); 

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to create the WDF memory, %!HRESULT!",
                    hr);
            }
        }
        else
        {
            m_spWdfMemory->SetBuffer(pInBuffer, inBufferSize);
        }

        // Format request
        if (SUCCEEDED(hr))
        {
            spWdfIoRequest2->Reuse(S_OK);

            hr = m_spRemoteTarget->FormatRequestForIoctl( 
                m_spWdfIoRequest,
                ioctlCode,
                nullptr,
                m_spWdfMemory, 
                nullptr, 
                nullptr,
                nullptr);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to format the WDF request for IOCTL, %!HRESULT!",
                    hr);
            }
        }

        // Send down the request
        if (SUCCEEDED(hr))
        {
            hr = m_spWdfIoRequest->Send(
                m_spRemoteTarget,
                (WDF_REQUEST_SEND_OPTION_SYNCHRONOUS | 
                    WDF_REQUEST_SEND_OPTION_TIMEOUT),
                SPB_REQUEST_TIMEOUT
                );

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to send the WDF request, %!HRESULT!",
                    hr);
            }
        }

        if (SUCCEEDED(hr))
        {
            CComPtr<IWDFRequestCompletionParams> spParams = nullptr;
            m_spWdfIoRequest->GetCompletionParams(&spParams);

            hr = spParams->GetCompletionStatus();

            if (SUCCEEDED(hr))
            {
                *pbytesTransferred = spParams->GetInformation();
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSpbRequest::Cancel
//
//  This method is used to cancel an outstanding SPB request.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSpbRequest::Cancel()
{
    FuncEntry();

    HRESULT hr = S_FALSE;

    if (m_spWdfIoRequest != nullptr)
    {
        // Attempt to cancel the request
        BOOL fCancelled = m_spWdfIoRequest->CancelSentRequest();

        if (fCancelled)
        {
            hr = S_OK;
        }
    }

    FuncExit();

    return hr;
}
