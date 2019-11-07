/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Queue.h

Abstract:

--*/
#pragma once
#include "resource.h"       // main symbols

class ATL_NO_VTABLE CQueue :
    public CComObjectRootEx<CComMultiThreadModel>,
    public IQueueCallbackDeviceIoControl,
    public IQueueCallbackCreate,
    public IObjectCleanup
{
public:
    CQueue()
    {

    }

    DECLARE_NOT_AGGREGATABLE(CQueue)

    BEGIN_COM_MAP(CQueue)
        COM_INTERFACE_ENTRY(IQueueCallbackDeviceIoControl)
        COM_INTERFACE_ENTRY(IQueueCallbackCreate)
    END_COM_MAP()

public:
    static
    HRESULT CreateInstance(
        _Outptr_ IUnknown        **ppUkwn)
    {
        CComObject< CQueue> *pMyQueue = NULL;
        HRESULT hr = S_OK;
        
        if ( NULL == ppUkwn )
        {
            hr = E_POINTER;
        }
        else
        {
            *ppUkwn = NULL;
        }

        if ( SUCCEEDED (hr) )
        {
            hr = CComObject<CQueue>::CreateInstance( &pMyQueue );
        }

        if ( SUCCEEDED (hr) && NULL == pMyQueue )
        {
            hr = E_UNEXPECTED;
        }

        if( SUCCEEDED (hr) )
        {
            pMyQueue->AddRef();
            hr = pMyQueue->QueryInterface( __uuidof(IUnknown), (void **) ppUkwn );
            pMyQueue->Release();
            pMyQueue = NULL;
        }

        return hr;
    }

    //
    // Wdf Callbacks
    //

    // IQueueCallbackCreateClose
    //
    STDMETHOD_ (void, OnCreateFile)(
        _In_  IWDFIoQueue *pQueue,
        _In_  IWDFIoRequest* pRequest,
        _In_  IWDFFile*      pFileObject
        );

    //
    // IQueueCallbackDeviceIoControl
    //
    STDMETHOD_ (void, OnDeviceIoControl)(
        _In_    IWDFIoQueue*    pQueue,
        _In_    IWDFIoRequest*  pRequest,
        _In_    ULONG           ControlCode,
        _In_    SIZE_T          InputBufferSizeInBytes,
        _In_    SIZE_T          OutputBufferSizeInBytes
        );

    //
    // IObjectCleanup
    //
    STDMETHOD_ (void, OnCleanup)(
        _In_    IWDFObject* pWdfObject
        );

private:
    HRESULT ProcessWpdMessage(
                ULONG       ControlCode,
        _In_    ContextMap* pClientContextMap,
        _In_    LPCWSTR     pszFileName,
        _In_    IWDFDevice* pDevice,
        _In_    PVOID       pInBuffer,
                ULONG       ulInputBufferLength,
        _Inout_ PVOID       pOutBuffer,
                ULONG       ulOutputBufferLength,
        _Out_   DWORD*      pdwBytesWritten);

    HRESULT GetWpdBaseDriver(
        _In_    IWDFDevice*     pDevice,
        _Out_   WpdBaseDriver** ppWpdBaseDriver);

    _Success_(return == S_OK)
    HRESULT GetFileName(
        _In_            IWDFFile*   pFileObject,
        _Outptr_result_maybenull_ LPWSTR*     ppszFilename);

    CComPtr<IWpdSerializer>         m_pWpdSerializer;
    CComAutoCriticalSection         m_CriticalSection;
};


