/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Queue.cpp

Abstract:

--*/

#include "stdafx.h"
#include <devioctl.h>

#include "Queue.tmh"

// Add table used to lookup the Access required for Wpd Commands
BEGIN_WPD_COMMAND_ACCESS_MAP(g_WpdCommandAccessMap)
    DECLARE_WPD_STANDARD_COMMAND_ACCESS_ENTRIES
    // Add any custom commands here e.g.
    // WPD_COMMAND_ACCESS_ENTRY(MyCustomCommand, WPD_COMMAND_ACCESS_READWRITE)
END_WPD_COMMAND_ACCESS_MAP

// This enables use to use VERIFY_WPD_COMMAND_ACCESS to check command access function for us.
DECLARE_VERIFY_WPD_COMMAND_ACCESS;

/******************************************************************************
 * This function calls the WpdBaseDriver to handle the WPD message. In order
 * to do this it does the following:
 *
 *  - Deserializes pBuffer into an IPortableDeviceValues which holds the command
 *    input parameters from the WPD application.
 *  - Creates an IPortableDeviceValues for the results.
 *  - Calls the WpdBaseDriver to handle the message. (The results of this
 *    operation are put into the previously created results IPortableDeviceValues.)
 *  - The results IPortableDeviceValues is then serialized back into pBuffer, making
 *    sure that it does not overrun ulOutputBufferLength.
 *
 *****************************************************************************/
HRESULT CQueue::ProcessWpdMessage(
            ULONG       ControlCode,
    _In_    ContextMap* pClientContextMap,
    _In_    LPCWSTR     pszFileName,
    _In_    IWDFDevice* pDevice,
    _In_    PVOID       pInBuffer,
            ULONG       ulInputBufferLength,
    _Inout_ PVOID       pOutBuffer,
            ULONG       ulOutputBufferLength,
    _Out_   DWORD*      pdwBytesWritten)
{
    HRESULT                        hr = S_OK;
    CComPtr<IPortableDeviceValues> pParams;
    CComPtr<IPortableDeviceValues> pResults;
    CComPtr<WpdBaseDriver>         pWpdBaseDriver;

    *pdwBytesWritten = 0;

    if (SUCCEEDED(hr))
    {
        hr = m_pWpdSerializer->GetIPortableDeviceValuesFromBuffer((BYTE*)pInBuffer,
                                                                  ulInputBufferLength,
                                                                  &pParams);
        CHECK_HR(hr, "Failed to deserialize command parameters from input buffer");
    }

    // Verify that that command was sent with the appropriate access
    if (SUCCEEDED(hr))
    {
        hr = VERIFY_WPD_COMMAND_ACCESS(ControlCode, pParams, g_WpdCommandAccessMap);
        CHECK_HR(hr, "Wpd Command was sent with incorrect access flags");
    }

    // Create the WPD results collection
    if (SUCCEEDED(hr))
    {
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_IPortableDeviceValues,
                              (VOID**)&pResults);
        CHECK_HR(hr, "Failed to CoCreate CLSID_PortableDeviceValues");
    }

    // Insert the client context map as one of this driver's private properties.  This is
    // just a convenient place holder which allows other methods down the chain to
    // access the context map.
    if (SUCCEEDED(hr))
    {
        hr = pParams->SetIUnknownValue(PRIVATE_SAMPLE_DRIVER_CLIENT_CONTEXT_MAP, pClientContextMap);
        CHECK_HR(hr, "Failed to set PRIVATE_SAMPLE_DRIVER_CLIENT_CONTEXT_MAP");
    }

    // Insert the IWDFDevice interface as one of this driver's private properties.  This is
    // just a convenient place holder which allows other methods down the chain to
    // access the WUDF Device object.
    if (SUCCEEDED(hr))
    {
        hr = pParams->SetIUnknownValue(PRIVATE_SAMPLE_DRIVER_WUDF_DEVICE_OBJECT, pDevice);
        CHECK_HR(hr, "Failed to set PRIVATE_SAMPLE_DRIVER_WUDF_DEVICE_OBJECT");
    }

    // Insert the IWpdSerializer interface as one of this driver's private properties.  This is
    // just a convenient place holder which allows other methods down the chain to
    // access the WPD Serializer object.
    if (SUCCEEDED(hr))
    {
        hr = pParams->SetIUnknownValue(PRIVATE_SAMPLE_DRIVER_WPD_SERIALIZER_OBJECT, m_pWpdSerializer);
        CHECK_HR(hr, "Failed to set PRIVATE_SAMPLE_DRIVER_WPD_SERIALIZER_OBJECT");
    }

    // Insert the Service Object ID as one of this driver's private properties.  This is
    // just a convenient place holder which allows other methods down the chain to
    // access the Service Object ID.
    if ((SUCCEEDED(hr)) && (pszFileName != NULL))
    {
        hr = pParams->SetStringValue(PRIVATE_SAMPLE_DRIVER_REQUEST_FILENAME, pszFileName);
        CHECK_HR(hr, "Failed to set PRIVATE_SAMPLE_DRIVER_REQUEST_FILENAME");
    }

    // Get the WpdBaseDriver so we can dispatch the message
    if (SUCCEEDED(hr))
    {
        hr = GetWpdBaseDriver(pDevice, &pWpdBaseDriver);
        CHECK_HR(hr, "Failed to get WpdBaseDriver");
    }

    if (SUCCEEDED(hr))
    {
        hr = pWpdBaseDriver->DispatchWpdMessage(pParams, pResults);
        CHECK_HR(hr, "Failed to handle WPD command");
    }

    if (SUCCEEDED(hr))
    {
        hr = m_pWpdSerializer->WriteIPortableDeviceValuesToBuffer(ulOutputBufferLength,
                                                                  pResults,
                                                                  (BYTE*)pOutBuffer,
                                                                  pdwBytesWritten);
        CHECK_HR(hr, "Failed to serialize results to output buffer");
    }

    return hr;
}

/******************************************************************************
 * This method gets the WpdBaseDriver associated with the UMDF device object.
 * The caller should Release *ppWpdBaseDriver when it is done.
 *
 * When this device was created, we assigned the WpdBaseDriver as the context.
 * So, in order to retrieve the correct WpdBaseDriver for this device, we simply
 * get the device context.
 *****************************************************************************/
HRESULT CQueue::GetWpdBaseDriver(
    _In_    IWDFDevice*     pDevice,
    _Out_   WpdBaseDriver** ppWpdBaseDriver)
{
    HRESULT         hr          = S_OK;
    WpdBaseDriver*  pContext    = NULL;

    if((pDevice == NULL) || (ppWpdBaseDriver == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter for pDevice or ppWpdBaseDriver");
    }

    if(SUCCEEDED(hr))
    {
        *ppWpdBaseDriver = NULL;
        hr = pDevice->RetrieveContext((void**)&pContext);
        if(SUCCEEDED(hr))
        {
            if(pContext != NULL)
            {
                pContext->AddRef();
                *ppWpdBaseDriver = pContext;
            }
            else
            {
                hr = E_UNEXPECTED;
                CHECK_HR(hr, "Device context is NULL");
            }
        }
    }

    return hr;
}


/******************************************************************************
 * This method gets the Filename associated with the Request object
 * The caller should CoTaskMemFree *ppszFileName when it is done.
 *****************************************************************************/
_Success_(return == S_OK)
HRESULT CQueue::GetFileName(
    _In_            IWDFFile* pFileObject,
    _Outptr_result_maybenull_ LPWSTR*  ppszFileName)
{
    HRESULT  hr          = S_OK;
    DWORD    cchFileName = 0;
    LPWSTR   pszFileName = NULL;

    if((pFileObject == NULL) || (ppszFileName == NULL))
    {
        hr = E_POINTER;
        CHECK_HR(hr, "Cannot have NULL parameter for pFileObject or ppszFileName");
        return hr;
    }

    *ppszFileName = NULL;

    if (SUCCEEDED(hr))
    {
        hr = pFileObject->RetrieveFileName(NULL, &cchFileName);
        CHECK_HR(hr, "Failed to get the filename size from WDF File Object");

        if (SUCCEEDED(hr) && cchFileName > 1)
        {
            pszFileName = (LPWSTR)CoTaskMemAlloc(cchFileName * sizeof(WCHAR));
            if (pszFileName == NULL)
            {
                hr = E_OUTOFMEMORY;
                CHECK_HR(hr, "Failed to allocate memory to hold the filename");
            }
            else
            {
                hr = pFileObject->RetrieveFileName(pszFileName, &cchFileName);
                CHECK_HR(hr, "Failed to get the filename from WDF File Object");
            }

            // The expected filename contains '\' + ServiceObjectID
            if (SUCCEEDED(hr) && (pszFileName != NULL) && (cchFileName > 1))
            {
                // Check that the leading character is '\'                
                if (pszFileName[0] == L'\\')
                {
                    // Skip the leading \\ character
                    *ppszFileName = AtlAllocTaskWideString(pszFileName + 1);
                    if (*ppszFileName == NULL)
                    {
                        hr = E_OUTOFMEMORY;
                        CHECK_HR(hr, "Failed to allocate memory to hold the result filename");
                    }
                }
                else
                {
                    hr = HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);
                }
            }
        }
    }

    CoTaskMemFree(pszFileName);
    
    return hr;
}


// CQueue

STDMETHODIMP_ (void)
CQueue::OnCreateFile(
    _In_    IWDFIoQueue*       pQueue,
    _In_    IWDFIoRequest*     pRequest,
    _In_    IWDFFile*          pFileObject
    )
{
    UNREFERENCED_PARAMETER(pQueue);
    //  This critical section protects the section of code where we
    //  Create the serializer and results interfaces used in handling I/O messages.
    //  We only need to create them once, then we hang on to them for the lifetime of this
    //  queue object.
    CComCritSecLock<CComAutoCriticalSection> Lock(m_CriticalSection);
    HRESULT hr = S_OK;
    CComPtr<IWDFDevice> pDevice;
    CComPtr<WpdBaseDriver> pWpdBaseDriver;

    // Get the WpdBaseDriver so we can dispatch the message when we are done
    pQueue->GetDevice(&pDevice);
    hr = GetWpdBaseDriver(pDevice, &pWpdBaseDriver);
    CHECK_HR(hr, "Failed to get WpdBaseDriver");

    // Create the WPD serializer
    if (SUCCEEDED(hr) &&
        (m_pWpdSerializer == NULL))
    {
        hr = CoCreateInstance(CLSID_WpdSerializer,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_IWpdSerializer,
                              (VOID**)&m_pWpdSerializer);

        CHECK_HR(hr, "Failed to CoCreate CLSID_WpdSerializer");
    }

    // Create the client context map and associate it with the File Object
    // so we can obtain it on a per-client basis.
    if (SUCCEEDED(hr))
    {
        ContextMap* pClientContextMap = new ContextMap(pWpdBaseDriver);

        if(pClientContextMap != NULL)
        {
            hr = pFileObject->AssignContext(this, (void*)pClientContextMap);
            CHECK_HR(hr, "Failed to set client context map");

            // Release the client context map if we cannot set it
            // properly
            if(FAILED(hr))
            {
                pClientContextMap->Release();
                pClientContextMap = NULL;
            }
        }
        else
        {
            hr = E_OUTOFMEMORY;
            CHECK_HR(hr, "Failed to create client context map");
        }
    }

    //
    // Notify the service implementation that a new client has arrived
    //
    if (SUCCEEDED(hr))
    {
        hr = pWpdBaseDriver->DispatchClientArrival();
        CHECK_HR(hr, "Failed to dispatch the client arrival");
    }

    pRequest->Complete(hr);
    return;
}

STDMETHODIMP_ (void)
CQueue::OnCleanup(
    _In_    IWDFObject* pWdfObject
    )
{
    // Destroy the client context map
    HRESULT     hr                = S_OK;
    ContextMap* pClientContextMap = NULL;

    hr = pWdfObject->RetrieveContext((void**)&pClientContextMap);
    if((SUCCEEDED(hr)) && (pClientContextMap != NULL))
    {
        pClientContextMap->Release();
        pClientContextMap = NULL;
    }
}


STDMETHODIMP_ (void)
CQueue::OnDeviceIoControl(
    _In_    IWDFIoQueue*    pQueue,
    _In_    IWDFIoRequest*  pRequest,
    _In_    ULONG           ControlCode,
    _In_    SIZE_T          InputBufferSizeInBytes,
    _In_    SIZE_T          OutputBufferSizeInBytes
    )
{
    UNREFERENCED_PARAMETER(InputBufferSizeInBytes);
    UNREFERENCED_PARAMETER(OutputBufferSizeInBytes);

    HRESULT hr              = S_OK;
    DWORD   dwBytesWritten  = 0;

    if(IS_WPD_IOCTL(ControlCode))
    {
        BYTE*       pInputBuffer         = NULL;
        SIZE_T      cbInputBuffer        = 0;
        BYTE*       pOutputBuffer        = NULL;
        SIZE_T      cbOutputBuffer       = 0;
        ContextMap* pClientContextMap    = NULL;
        LPWSTR      pszFileName          = NULL;

        CComPtr<IWDFMemory> pMemoryIn;
        CComPtr<IWDFMemory> pMemoryOut;
        CComPtr<IWDFDevice> pDevice;
        CComPtr<IWDFFile>   pFileObject;

        //
        // Get input memory buffer, the memory object is always returned even if the
        // underlying buffer is NULL
        //
        pRequest->GetInputMemory(&pMemoryIn);
        pInputBuffer = (BYTE*) pMemoryIn->GetDataBuffer(&cbInputBuffer);

        //
        // Get output memory buffer, the memory object is always returned even if the
        // underlying buffer is NULL
        //
        pRequest->GetOutputMemory(&pMemoryOut);
        pOutputBuffer = (BYTE*) pMemoryOut->GetDataBuffer(&cbOutputBuffer);

        // Get the Context map for this client
        pRequest->GetFileObject(&pFileObject);
        if (pFileObject != NULL)
        {
            hr = pFileObject->RetrieveContext((void**)&pClientContextMap);
            CHECK_HR(hr, "Failed to get Contextmap from WDF File Object");

            if (SUCCEEDED(hr))
            {
                hr = GetFileName(pFileObject, &pszFileName);
                CHECK_HR(hr, "Failed to get Filename from WDF File Object");
            }
        }

        if (SUCCEEDED(hr) && NULL == pClientContextMap)
        {
            hr = E_FAIL;
            CHECK_HR(hr, "pClientContextMap is NULL");
        }

        if (SUCCEEDED(hr) && NULL == pszFileName)
        {
            hr = E_FAIL;
            CHECK_HR(hr, "pszFileName is NULL");
        }

        if (SUCCEEDED(hr))
        {
            // Get the device object
            pQueue->GetDevice(&pDevice );

            hr = ProcessWpdMessage(ControlCode,
                                   pClientContextMap,
                                   pszFileName,
                                   pDevice,
                                   pInputBuffer,
                                   (DWORD)cbInputBuffer,
                                   pOutputBuffer,
                                   (DWORD)cbOutputBuffer,
                                   &dwBytesWritten);
        }

        CoTaskMemFree(pszFileName);
    }
    else
    {
        hr = E_UNEXPECTED;
        CHECK_HR(hr, "Received invalid/unsupported IOCTL code '0x%lx'",ControlCode);
    }

    // Complete the request
    if (SUCCEEDED(hr))
    {
        pRequest->CompleteWithInformation(hr, dwBytesWritten);
    }
    else
    {
        pRequest->Complete(hr);
    }

    return;
}


