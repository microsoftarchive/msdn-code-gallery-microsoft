//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include <InitGuid.h>
#include "StspSchemeHandler.h"
#include "StspMediaStream.h"

#ifdef GetObject
#undef GetObject
#endif

using namespace Stsp;

CSchemeHandler::CSchemeHandler(void)
#pragma warning(push)
#pragma warning(disable: 4355)
    : _OnSourceOpenCB(this, &CSchemeHandler::OnSourceOpen)
#pragma warning(pop)
{
}

CSchemeHandler::~CSchemeHandler(void)
{
}

// IMediaExtension methods
IFACEMETHODIMP CSchemeHandler::SetProperties (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
    return S_OK;
}

// IMFSchemeHandler methods
IFACEMETHODIMP CSchemeHandler::BeginCreateObject( 
        _In_ LPCWSTR pwszURL,
        _In_ DWORD dwFlags,
        _In_ IPropertyStore *pProps,
        _Out_opt_  IUnknown **ppIUnknownCancelCookie,
        _In_ IMFAsyncCallback *pCallback,
        _In_ IUnknown *punkState)
{
    if (pwszURL == nullptr || pCallback == nullptr)
    {
        return E_INVALIDARG;
    }

    if ((dwFlags & MF_RESOLUTION_MEDIASOURCE) == 0)
    {
        return E_INVALIDARG;
    }

    ComPtr<CMediaSource> spSource;
    ComPtr<IMFAsyncResult> spResult;
    HRESULT hr = CMediaSource::CreateInstance(&spSource);

    if (SUCCEEDED(hr))
    {
        ComPtr<IUnknown> spunkSource;
        hr = spSource.As(&spunkSource);
        if (SUCCEEDED(hr))
        {
            hr = MFCreateAsyncResult(spunkSource.Get(), pCallback, punkState, &spResult);
        }
    }

    if (SUCCEEDED(hr))
    {            
        hr = spSource->BeginOpen(pwszURL, &_OnSourceOpenCB, spResult.Get());
    }

    if (SUCCEEDED(hr) && ppIUnknownCancelCookie != nullptr)
    {
        *ppIUnknownCancelCookie = nullptr;
    }

    if (FAILED(hr))
    {
        spSource->Shutdown();
    }

    TRACEHR_RET(hr);
}
        
IFACEMETHODIMP CSchemeHandler::EndCreateObject( 
        _In_ IMFAsyncResult *pResult,
        _Out_  MF_OBJECT_TYPE *pObjectType,
        _Out_  IUnknown **ppObject)
{
    if (pResult == nullptr || pObjectType == nullptr || ppObject == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = pResult->GetStatus();
    *pObjectType = MF_OBJECT_INVALID;
    *ppObject = nullptr;

    if (SUCCEEDED(hr))
    {
        ComPtr<IUnknown> punkSource;
        hr = pResult->GetObject(&punkSource);
        if (SUCCEEDED(hr))
        {
            *pObjectType = MF_OBJECT_MEDIASOURCE;
            *ppObject = punkSource.Detach();
        }
    }

    TRACEHR_RET(hr);
}
        
IFACEMETHODIMP CSchemeHandler::CancelObjectCreation( 
            _In_ IUnknown *pIUnknownCancelCookie)
{
    return E_NOTIMPL;
}

HRESULT CSchemeHandler::OnSourceOpen(_In_ IMFAsyncResult *pResult)
{
    ComPtr<IUnknown> spState = pResult->GetStateNoAddRef();
    if (!spState)
    {
        return MF_E_UNEXPECTED;
    }
    ComPtr<IMFAsyncResult> spSavedResult;
    HRESULT hr = S_OK;

    hr = spState.As(&spSavedResult);
    if (FAILED(hr))
    {
        TRACEHR_RET(hr);
    }

    ComPtr<IUnknown> spunkSource;
    ComPtr<IMFMediaSource> spSource;
    hr = spSavedResult->GetObject(&spunkSource);

    if (SUCCEEDED(hr))
    {
        hr = spunkSource.As(&spSource);
        if (SUCCEEDED(hr))
        {
            CMediaSource *pSource = static_cast<CMediaSource *>(spSource.Get());
            hr = pSource->EndOpen(pResult);
        }
    }

    spSavedResult->SetStatus(hr);    
    hr = MFInvokeCallback(spSavedResult.Get());

    TRACEHR_RET(hr);
}
