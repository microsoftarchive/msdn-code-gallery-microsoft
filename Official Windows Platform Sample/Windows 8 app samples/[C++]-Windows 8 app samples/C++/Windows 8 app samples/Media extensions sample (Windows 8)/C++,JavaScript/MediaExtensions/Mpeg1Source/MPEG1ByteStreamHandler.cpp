//////////////////////////////////////////////////////////////////////////
//
// MPEG1ByteStreamHandler.cpp
// Implements the byte-stream handler for the MPEG1 source.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//////////////////////////////////////////////////////////////////////////


#include "MPEG1Src.h"
#include "MPEG1ByteStreamHandler.h"


//-------------------------------------------------------------------
// CMPEG1ByteStreamHandler  class
//-------------------------------------------------------------------
//-------------------------------------------------------------------
// Constructor
//-------------------------------------------------------------------

CMPEG1ByteStreamHandler::CMPEG1ByteStreamHandler()
    : m_pSource(nullptr), m_pResult(nullptr)
{
}

//-------------------------------------------------------------------
// Destructor
//-------------------------------------------------------------------

CMPEG1ByteStreamHandler::~CMPEG1ByteStreamHandler()
{
    SafeRelease(&m_pSource);
    SafeRelease(&m_pResult);
}

//-------------------------------------------------------------------
// IMediaExtension methods
//-------------------------------------------------------------------

//-------------------------------------------------------------------
// SetProperties
// Sets the configuration of the media byte stream handler
//-------------------------------------------------------------------
IFACEMETHODIMP CMPEG1ByteStreamHandler::SetProperties (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
    return S_OK;
}

//-------------------------------------------------------------------
// IMFByteStreamHandler methods
//-------------------------------------------------------------------

//-------------------------------------------------------------------
// BeginCreateObject
// Starts creating the media source.
//-------------------------------------------------------------------

HRESULT CMPEG1ByteStreamHandler::BeginCreateObject(
        /* [in] */ IMFByteStream *pByteStream,
        /* [in] */ LPCWSTR pwszURL,
        /* [in] */ DWORD dwFlags,
        /* [in] */ IPropertyStore *pProps,
        /* [out] */ IUnknown **ppIUnknownCancelCookie,  // Can be NULL
        /* [in] */ IMFAsyncCallback *pCallback,
        /* [in] */ IUnknown *punkState                  // Can be NULL
        )
{
    if (pByteStream == NULL)
    {
        return E_POINTER;
    }

    if (pCallback == NULL)
    {
        return E_POINTER;
    }

    if ((dwFlags & MF_RESOLUTION_MEDIASOURCE) == 0)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;

    IMFAsyncResult *pResult = NULL;
    CMPEG1Source   *pSource = NULL;

    // Create an instance of the media source.
    hr = CMPEG1Source::CreateInstance(&pSource);

    // Create a result object for the caller's async callback.
    if (SUCCEEDED(hr))
    {
        hr = MFCreateAsyncResult(NULL, pCallback, punkState, &pResult);
    }

    // Start opening the source. This is an async operation.
    // When it completes, the source will invoke our callback
    // and then we will invoke the caller's callback.
    if (SUCCEEDED(hr))
    {
        hr = pSource->BeginOpen(pByteStream, this, NULL);
    }

    if (SUCCEEDED(hr))
    {
        if (ppIUnknownCancelCookie)
        {
            *ppIUnknownCancelCookie = NULL;
        }

        m_pSource = pSource;
        m_pSource->AddRef();

        m_pResult = pResult;
        m_pResult->AddRef();
    }

// cleanup
    SafeRelease(&pSource);
    SafeRelease(&pResult);
    return hr;
}

//-------------------------------------------------------------------
// EndCreateObject
// Completes the BeginCreateObject operation.
//-------------------------------------------------------------------

HRESULT CMPEG1ByteStreamHandler::EndCreateObject(
        /* [in] */ IMFAsyncResult *pResult,
        /* [out] */ MF_OBJECT_TYPE *pObjectType,
        /* [out] */ IUnknown **ppObject)
{
    if (pResult == NULL || pObjectType == NULL || ppObject == NULL)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    *pObjectType = MF_OBJECT_INVALID;
    *ppObject = NULL;

    hr = pResult->GetStatus();

    if (SUCCEEDED(hr))
    {
        *pObjectType = MF_OBJECT_MEDIASOURCE;

        assert(m_pSource != NULL);

        hr = m_pSource->QueryInterface(IID_PPV_ARGS(ppObject));
    }

    SafeRelease(&m_pSource);
    SafeRelease(&m_pResult);
    return hr;
}


HRESULT CMPEG1ByteStreamHandler::CancelObjectCreation(IUnknown *pIUnknownCancelCookie)
{
    return E_NOTIMPL;
}

HRESULT CMPEG1ByteStreamHandler::GetMaxNumberOfBytesRequiredForResolution(QWORD* pqwBytes)
{
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// Invoke
// Callback for the media source's BeginOpen method.
//-------------------------------------------------------------------

HRESULT CMPEG1ByteStreamHandler::Invoke(IMFAsyncResult* pResult)
{
    HRESULT hr = S_OK;

    if (m_pSource)
    {
        hr = m_pSource->EndOpen(pResult);
    }
    else
    {
        hr = E_UNEXPECTED;
    }

    m_pResult->SetStatus(hr);

    hr = MFInvokeCallback(m_pResult);

    return hr;
}

