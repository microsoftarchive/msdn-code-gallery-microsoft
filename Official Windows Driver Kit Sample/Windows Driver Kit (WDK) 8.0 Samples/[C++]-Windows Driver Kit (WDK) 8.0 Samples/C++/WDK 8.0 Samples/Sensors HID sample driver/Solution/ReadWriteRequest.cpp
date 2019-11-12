/*++
 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


Module Name:

    ReadWriteRequest.cpp

Abstract:

    This module contains the implementation of the HID Read/Write request
    completion callback object.
--*/

#include "internal.h"
#include "SensorDDI.h"
#include "ReadWriteRequest.h"

#include "ReadWriteRequest.tmh"

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::CReadWriteRequest
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CReadWriteRequest::CReadWriteRequest() : 
    m_spHidWdfReadRequest(NULL),
    m_spHidWdfDevice(NULL),
    m_spHidWdfDriver(NULL),
    m_spHidWdfFile(NULL),
    m_spHidWdfIoTarget(NULL),
    m_pParentCallback(NULL),
    m_bytesRead(0),
    m_fResend(FALSE)
{

}

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::~CReadWriteRequest
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CReadWriteRequest::~CReadWriteRequest()
{

}


/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::InitializeRequest
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CReadWriteRequest::InitializeRequest(IWDFDevice* pWdfDevice,
                                             CSensorDDI* pCallback)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = (nullptr != pWdfDevice) ? S_OK : E_UNEXPECTED;

    // Store the callback
    m_pParentCallback = pCallback;
    if(SUCCEEDED(hr))
    {
        m_fResend = TRUE;

        // Store the IWDFDevice pointer
        m_spHidWdfDevice = pWdfDevice;
        
        // Step 1: Try CreateWdfFile here (target to next driver in the driver stack)
        hr = m_spHidWdfDevice->CreateWdfFile( NULL, &m_spHidWdfFile );

        if(SUCCEEDED(hr))
        {
            if (nullptr != m_spHidWdfFile)
            {
                if (nullptr != pCallback)
                {
                    if (nullptr == pCallback->m_spHidWdfFile)
                    {
                        pCallback->m_spHidWdfFile = m_spHidWdfFile;
                    }
                    else
                    {
                        Trace(TRACE_LEVEL_ERROR, "pCallback->m_pHidWdfFile is not NULL, hr = %!HRESULT!", hr); 
                    }
                }
                else
                {
                    //there is no callback
                }

                // Step 2: Get the parent driver object
                m_spHidWdfDevice->GetDriver( &m_spHidWdfDriver );

                // Step 3: Get the IO target i.e. default Target
                if (nullptr != m_spHidWdfDriver)
                {
                    m_spHidWdfDevice->GetDefaultIoTarget( &m_spHidWdfIoTarget );
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed during GetDriver(), hr = %!HRESULT!", hr); 
                }

                if (FAILED(hr))
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed during GetDefaultIoTarget(), hr = %!HRESULT!", hr); 
                }
            }
            else
            {
                Trace(TRACE_LEVEL_ERROR, "m_pHidWdfFile is NULL, hr = %!HRESULT!", hr); 
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed during CreateWdfFile(), hr = %!HRESULT!", hr); 
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::UninitializeRequest
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CReadWriteRequest::UninitializeRequest()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    m_bytesRead = 0;
    ZeroMemory(m_buffer, READ_BUFFER_SIZE);

    if (nullptr != m_spHidWdfFile)
    {
        m_spHidWdfFile->Close();

        if (nullptr != m_pParentCallback)
        {
            if (m_pParentCallback->m_spHidWdfFile == m_spHidWdfFile)
            {
                m_pParentCallback->m_spHidWdfFile = nullptr;
            }
        }

        m_spHidWdfFile = nullptr;
    }

    return S_OK;
}

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::OnCompletion
//
//  This method is called by Framework when read request is completed
//  The result is saved in the m_buffer
//
/////////////////////////////////////////////////////////////////////////
void CReadWriteRequest::OnCompletion(
        _In_ IWDFIoRequest*                 pRequest,
        _In_ IWDFIoTarget*                  pIoTarget,
        _In_ IWDFRequestCompletionParams*   CompletionParams,
        _In_ PVOID                          Context
        )
{
    UNREFERENCED_PARAMETER(pRequest);
    UNREFERENCED_PARAMETER(pIoTarget);
    UNREFERENCED_PARAMETER(Context);

    BYTE            bufferCopy[READ_BUFFER_SIZE];
    ULONG           bytesCopied = 0;
    CSensorDDI*     pParentCallbackCopy = nullptr;
    BOOL            fTryResending = FALSE;
    
    {
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

        if (NULL != CompletionParams)
        {
            HRESULT hr = CompletionParams->GetCompletionStatus();

            //Process the read buffer
            if ( SUCCEEDED( hr ))
            {
                WDF_REQUEST_TYPE reqType = CompletionParams->GetCompletedRequestType();

                if (WdfRequestRead == reqType)
                {
                    m_bytesRead = (ULONG) CompletionParams->GetInformation();

                    // Process the input report
                    if ((m_bytesRead > 0) && (m_bytesRead <= sizeof(bufferCopy)))
                    {
                        if (FAILED(EnterProcessing(PROCESSING_IREQUESTCALLBACKREQUESTCOMPLETION)))
                        {
                            // hr = HRESULT_FROM_WIN32(ERROR_SHUTDOWN_IN_PROGRESS);

                            if (nullptr != m_spHidWdfReadRequest)
                            {
                                 //Delete the request object
                                m_spHidWdfReadRequest->DeleteWdfObject();
                                m_spHidWdfReadRequest = nullptr;
                            }
                        }
                        else
                        {
                            if (nullptr != m_pParentCallback)
                            {
                                // Make a copy of the read result for the callback out of the critical section.
                                __analysis_assume(m_bytesRead <= sizeof(bufferCopy));
                                memcpy(bufferCopy, m_buffer, m_bytesRead);
                                bytesCopied = m_bytesRead;
                                pParentCallbackCopy = m_pParentCallback;
                            }

                            if (nullptr != m_spHidWdfReadRequest)
                            {
                                //Delete the request object
                                m_spHidWdfReadRequest->DeleteWdfObject();
                                m_spHidWdfReadRequest = nullptr;
                            }

                            fTryResending = TRUE;

                        } // processing in progress

                        ExitProcessing(PROCESSING_IREQUESTCALLBACKREQUESTCOMPLETION);
                    }
                    else if (nullptr != m_spHidWdfReadRequest)
                    {
                        //Delete the request object
                        m_spHidWdfReadRequest->DeleteWdfObject();
                        m_spHidWdfReadRequest = nullptr;
                    }
                }
                else if (WdfRequestWrite == reqType)
                {
                    Trace(TRACE_LEVEL_INFORMATION, "WdfRequestWrite");
                }
                else if (WdfRequestDeviceIoControl == reqType)
                {
                    Trace(TRACE_LEVEL_INFORMATION, "WdfRequestDeviceIoControl");
                }
            }
        }
    }

    if (0 != bytesCopied)
    {
        pParentCallbackCopy->OnAsyncReadCallback( bufferCopy, bytesCopied);

        if (TRUE == fTryResending)
        {
            CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

            // Queue up another pending Read request if still connected
            // nullptr == m_pHidWdfFile indicates device is disconnected
            if ((nullptr != m_spHidWdfFile) && (nullptr != m_pParentCallback))
            {
                if ((TRUE == m_fResend) && (FALSE == m_pParentCallback->m_fReleasingDevice ))
                {
                    CreateAndSendReadRequest();
                }
            }
        }
    }

    return;
}

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::CreateAndSendReadRequest
//
//  This helper method creates and sends the read request down the stack
//  
//  Remarks:

// In this routine we:
//      1. Create a request and format it for read
//      2. Asynchronously send the request without any timeout
// 
//  In case of failure this routine deletes the request*/     
//
/////////////////////////////////////////////////////////////////////////
HRESULT CReadWriteRequest::CreateAndSendReadRequest()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = E_UNEXPECTED;
    CComPtr<IWDFMemory>      spOutputMemory = nullptr;
    CComPtr<IRequestCallbackRequestCompletion> spICallback = nullptr;

    if ( (nullptr != m_spHidWdfDevice) &&
            (nullptr != m_spHidWdfIoTarget) &&
            (nullptr != m_spHidWdfFile) )
    {
        //Reset internal buffer
        m_bytesRead = 0;
        ZeroMemory(m_buffer, READ_BUFFER_SIZE);

        //Creates a new request
        hr = m_spHidWdfDevice->CreateRequest(NULL, m_spHidWdfDevice, &m_spHidWdfReadRequest);

        //Get callback interface
        if ( SUCCEEDED( hr ) )
        {
            if (nullptr != m_spHidWdfReadRequest)
            {
                hr = this->QueryInterface( __uuidof(IRequestCallbackRequestCompletion) , (PVOID*)&spICallback );
 
                //Set completion callback
                if ( SUCCEEDED( hr ) )
                {
                    m_spHidWdfReadRequest->SetCompletionCallback(spICallback, NULL);
                } 
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed during QueryInterface(), hr = %!HRESULT!", hr); 
                }

                //Create output memory
                if ( SUCCEEDED( hr ) )
                {
                    hr = m_spHidWdfDriver->CreatePreallocatedWdfMemory(  (BYTE*)m_buffer, 
                                                                            READ_BUFFER_SIZE, 
                                                                            NULL,                  // no object event callback
                                                                            m_spHidWdfReadRequest,    // request object as parent
                                                                            &spOutputMemory); 
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed during SetCompletionCallback(), hr = %!HRESULT!", hr); 
                }

                //Format request
                if ( SUCCEEDED( hr ) && (nullptr != m_spHidWdfFile))
                {
                    hr = m_spHidWdfIoTarget->FormatRequestForRead( m_spHidWdfReadRequest, 
                                                                    m_spHidWdfFile,
                                                                    spOutputMemory, 
                                                                    NULL, 
                                                                    NULL );
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed during CreatePreallocatedWdfMemory(), hr = %!HRESULT!", hr); 
                }

                //Send down the request
                if ( SUCCEEDED( hr ) )
                {
                    hr = m_spHidWdfReadRequest->Send(  m_spHidWdfIoTarget,
                                                    0,              //No flag
                                                    0               //No timeout
                                                    );
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed during FormatRequestForRead(), hr = %!HRESULT!", hr); 
                }
        
                if( FAILED( hr ) )
                {    
                    m_spHidWdfReadRequest->DeleteWdfObject();
                    m_spHidWdfReadRequest = NULL;
                    Trace(TRACE_LEVEL_ERROR, "Failed during Send(), hr = %!HRESULT!", hr); 
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "m_pHidWdfReadRequest is NULL, hr = %!HRESULT!", hr); 
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed during CreateRequest() is NULL, hr = %!HRESULT!", hr); 
        }
    }

    if (FAILED(hr))
    {
        if (nullptr == m_spHidWdfDevice)
        {
            Trace(TRACE_LEVEL_ERROR, "m_pHidWdfDevice is NULL, hr = %!HRESULT!", hr); 
        }
        if (nullptr == m_spHidWdfIoTarget)
        {
            Trace(TRACE_LEVEL_ERROR, "m_pHidWdfIoTarget is NULL, hr = %!HRESULT!", hr); 
        }
        if (nullptr == m_spHidWdfFile)
        {
            Trace(TRACE_LEVEL_ERROR, "m_pHidWdfFile is NULL, hr = %!HRESULT!", hr); 
        }

        Trace(TRACE_LEVEL_ERROR, "Failed during read request, hr = %!HRESULT!", hr); 
    }

    return hr;        
}

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::CreateAndSendWriteRequest
//
//  This helper method creates and sends the write request down the stack
//  
//  Remarks:

// In this routine we:
//      1. Create a request and format it for write
//      2. Synchronously send the request without any timeout
// 
//  In case of failure this routine deletes the request*/     
//
/////////////////////////////////////////////////////////////////////////
HRESULT CReadWriteRequest::CreateAndSendWriteRequest(
        BYTE* buffer,
        ULONG bufferSize
        )
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = E_UNEXPECTED;

    CComPtr<IWDFMemory>                         spInputMemory = nullptr;
    CComPtr<IRequestCallbackRequestCompletion>  spICallback = nullptr;

    CComPtr<IWDFIoRequest>                      spHidWdfWriteRequest; // UMDF Framework Request object

    if ( (nullptr != m_spHidWdfDevice) &&
            (nullptr != m_spHidWdfIoTarget) &&
            (nullptr != m_spHidWdfFile) )
    {
        hr = m_spHidWdfDevice->CreateRequest(NULL, m_spHidWdfDevice, &spHidWdfWriteRequest);

        if ( SUCCEEDED( hr ) )
        {
#if 0 //we do not do any processing in the callback, so no need to register it
            hr = this->QueryInterface( __uuidof(IRequestCallbackRequestCompletion) , (PVOID*)&pICallback );
 
            //Set completion callback
            if ( SUCCEEDED( hr ) )
            {
                spHidWdfWriteRequest->SetCompletionCallback(pICallback, NULL);
            } 
#endif

            //Create input memory
            if ( SUCCEEDED( hr ) )
            {
                hr = m_spHidWdfDriver->CreatePreallocatedWdfMemory(buffer, 
                                                                    bufferSize, 
                                                                    NULL,                  // no object event callback
                                                                    spHidWdfWriteRequest,    // request object as parent
                                                                    &spInputMemory); 
            }

            //Format request
            if ( SUCCEEDED( hr ) )
            {
                hr = m_spHidWdfIoTarget->FormatRequestForWrite( spHidWdfWriteRequest, 
                                                                m_spHidWdfFile,
                                                                spInputMemory, 
                                                                NULL, 
                                                                NULL );
            }

            //
            //Send down the request
            //    
            if ( SUCCEEDED( hr ) )
            {
                hr = spHidWdfWriteRequest->Send(
                                            m_spHidWdfIoTarget,
                                            DEVICE_SYNCHRONOUS_REQUEST_FLAGS,   
                                            DEVICE_SYNCHRONOUS_REQUEST_TIMEOUT
                                            );
                if ( FAILED( hr ) )
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed sending the request, hr = %!HRESULT!", hr); 
                }
                else
                {
                    CComPtr<IWDFRequestCompletionParams>  spFxComplParams = nullptr;

                    spHidWdfWriteRequest->GetCompletionParams(&spFxComplParams);
                    if ( nullptr == spFxComplParams )
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Failed getting CompletionParams, hr = %!HRESULT!", hr); 
                    }
                    else
                    {
                        hr = spFxComplParams->GetCompletionStatus();

                        if ( FAILED( hr ) )
                        {
                            Trace(TRACE_LEVEL_ERROR, "Request completed with failure status, hr = %!HRESULT!", hr); 
                        }
                    }
                }
            }
        
            //this is a synchronous call, so we delete the request object
            //whether it succeeded or failed
            spHidWdfWriteRequest->DeleteWdfObject();
            spHidWdfWriteRequest = nullptr;
        }
    }

    if (FAILED(hr))
    {
        Trace(TRACE_LEVEL_ERROR, "Failed during request, hr = %!HRESULT!", hr); 
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::CreateAndSendIoctlRequest
//
//  This helper method creates and sends the ioctl request down the stack
//  
//  Remarks:

// In this routine we:
//      1. Create a request and format it for write
//      2. Synchronously send the request without any timeout
// 
//  In case of failure this routine deletes the request*/     
//
/////////////////////////////////////////////////////////////////////////
HRESULT CReadWriteRequest::CreateAndSendIoctlRequest(
        BYTE* buffer,
        ULONG bufferSize,
        ULONG ioctlCode,
        BYTE* pInBuffer
        )
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = E_POINTER;

    CComPtr<IWDFMemory>                         spOutputMemory = nullptr;
    CComPtr<IWDFMemory>                         spInputMemory = nullptr;
    CComPtr<IRequestCallbackRequestCompletion>  spICallback = nullptr;
    CComPtr<IWDFIoRequest>                      spHidWdfIoctlRequest; // UMDF Framework Request object

    if ( (nullptr != m_spHidWdfDevice) &&
            (nullptr != m_spHidWdfIoTarget) &&
            (nullptr != m_spHidWdfFile) )
    {
        //Create a new request
        hr = m_spHidWdfDevice->CreateRequest(NULL, m_spHidWdfDevice, &spHidWdfIoctlRequest);

        if ( SUCCEEDED( hr ) )
        {
#if 0 //we do not do any processing in the callback, so no need to register it
            hr = this->QueryInterface( __uuidof(IRequestCallbackRequestCompletion) , (PVOID*)&pICallback );
 
            //Set completion callback
            if ( SUCCEEDED( hr ) )
            {
                spHidWdfIoctlRequest->SetCompletionCallback(pICallback, NULL);
            } 
#endif

            //Create input memory
            if (( SUCCEEDED( hr ) ) && ( nullptr != pInBuffer))
            {
                hr = m_spHidWdfDriver->CreatePreallocatedWdfMemory(pInBuffer, 
                                                                    bufferSize, 
                                                                    NULL,                  // no object event callback
                                                                    spHidWdfIoctlRequest,    // request object as parent
                                                                    &spInputMemory); 
            }

            //Create output memory
            if ( SUCCEEDED( hr ) )
            {
                hr = m_spHidWdfDriver->CreatePreallocatedWdfMemory(buffer, 
                                                                    bufferSize, 
                                                                    NULL,                  // no object event callback
                                                                    spHidWdfIoctlRequest,    // request object as parent
                                                                    &spOutputMemory); 
            }

            //Format request
            if (NULL == pInBuffer)
            {
                if ( SUCCEEDED( hr ) )
                {
                    hr = m_spHidWdfIoTarget->FormatRequestForIoctl( spHidWdfIoctlRequest,
                                                                    ioctlCode,
                                                                    m_spHidWdfFile,
                                                                    NULL, 
                                                                    NULL, 
                                                                    spOutputMemory,
                                                                    NULL);
                }
            }
            else
            {
                if ( SUCCEEDED( hr ) )
                {
                    hr = m_spHidWdfIoTarget->FormatRequestForIoctl( spHidWdfIoctlRequest,
                                                                    ioctlCode,
                                                                    m_spHidWdfFile,
                                                                    spInputMemory, 
                                                                    NULL, 
                                                                    spOutputMemory,
                                                                    NULL);
                }
            }

            //
            //Send down the request
            //    
            if ( SUCCEEDED( hr ) )
            {
                hr = spHidWdfIoctlRequest->Send(
                                            m_spHidWdfIoTarget,
                                            DEVICE_SYNCHRONOUS_REQUEST_FLAGS,   
                                            DEVICE_SYNCHRONOUS_REQUEST_TIMEOUT
                                            );
                if ( FAILED( hr ) )
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed sending the request, hr = %!HRESULT!", hr); 
                }
                else
                {
                    CComPtr<IWDFRequestCompletionParams>  spFxComplParams = nullptr;

                    spHidWdfIoctlRequest->GetCompletionParams(&spFxComplParams);
                    if ( nullptr == spFxComplParams )
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Failed getting CompletionParams, hr = %!HRESULT!", hr); 
                    }
                    else
                    {
                        hr = spFxComplParams->GetCompletionStatus();

                        if ( FAILED( hr ) )
                        {
                            Trace(TRACE_LEVEL_ERROR, "Request completed with failure status, hr = %!HRESULT!", hr); 
                        }
                    }
                }
            }
        
            //this is a synchronous call, so we delete the request object
            //whether it succeeded or failed
            spHidWdfIoctlRequest->DeleteWdfObject();
            spHidWdfIoctlRequest = nullptr;
        }
    }

    if (FAILED(hr))
    {
        Trace(TRACE_LEVEL_ERROR, "Failed during request, hr = %!HRESULT!", hr); 
    }

    return hr;        
}

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::CancelAndStopPendingRequest
//
//   
//
/////////////////////////////////////////////////////////////////////////
HRESULT CReadWriteRequest::CancelAndStopPendingRequest()
{
    HRESULT hr = S_OK;

    if ( NULL != m_spHidWdfReadRequest )
    {
        m_spHidWdfReadRequest->CancelSentRequest();
    }

    return hr;
}


inline HRESULT CReadWriteRequest::EnterProcessing(DWORD64 dwControlFlag)
{
    HRESULT hr = E_UNEXPECTED;

    if ( nullptr != m_pParentCallback )
    {
        hr = m_pParentCallback->EnterProcessing(dwControlFlag);
    }

    return hr;
}

inline void CReadWriteRequest::ExitProcessing(DWORD64 dwControlFlag)
{
    if ( nullptr != m_pParentCallback )
    {
        m_pParentCallback->ExitProcessing(dwControlFlag);
    }
}
