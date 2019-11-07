//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "GeometricSchemeHandler.h"
#include "GeometricMediaStream.h"

#include <wrl\module.h>

ActivatableClass(CGeometricSchemeHandler);

CGeometricSchemeHandler::CGeometricSchemeHandler(void)
{
}


CGeometricSchemeHandler::~CGeometricSchemeHandler(void)
{
}

// IMediaExtension methods
IFACEMETHODIMP CGeometricSchemeHandler::SetProperties(ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
    return S_OK;
}

// IMFSchemeHandler methods
IFACEMETHODIMP CGeometricSchemeHandler::BeginCreateObject( 
    _In_ LPCWSTR pwszURL,
    _In_ DWORD dwFlags,
    _In_ IPropertyStore *pProps,
    _Out_opt_  IUnknown **ppIUnknownCancelCookie,
    _In_ IMFAsyncCallback *pCallback,
    _In_ IUnknown *punkState)
{
    HRESULT hr = S_OK;
    ComPtr<CGeometricMediaSource> spSource;
    try
    {
        if (pwszURL == nullptr || pCallback == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        if ((dwFlags & MF_RESOLUTION_MEDIASOURCE) == 0)
        {
            return E_INVALIDARG;
        }

        ComPtr<IMFAsyncResult> spResult;
        spSource = CGeometricMediaSource::CreateInstance();

        ComPtr<IUnknown> spSourceUnk;
        ThrowIfError(spSource.As(&spSourceUnk));
        ThrowIfError(MFCreateAsyncResult(spSourceUnk.Get(), pCallback, punkState, &spResult));

        spSource->OpenAsync(ref new String(pwszURL)).then([this, spResult](concurrency::task<void>& openTask)
        {
            try
            {
                if (spResult == nullptr)
                {
                    ThrowIfError(MF_E_UNEXPECTED);
                }

                openTask.get();
            }
            catch (Exception ^exc)
            {
                if (spResult != nullptr)
                {
                    spResult->SetStatus(exc->HResult);
                }
            }

            if (spResult != nullptr)
            {
                MFInvokeCallback(spResult.Get());
            }
        });

        if (ppIUnknownCancelCookie != nullptr)
        {
            *ppIUnknownCancelCookie = nullptr;
        }
    }
    catch (Exception ^exc)
    {
        if (spSource != nullptr)
        {
            spSource->Shutdown();
        }
        hr = exc->HResult;
    }

    return hr;
}

IFACEMETHODIMP CGeometricSchemeHandler::EndCreateObject( 
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
            *ppObject = punkSource.Get();
            (*ppObject)->AddRef();
        }
    }

    return hr;
}

IFACEMETHODIMP CGeometricSchemeHandler::CancelObjectCreation( 
    _In_ IUnknown *pIUnknownCancelCookie)
{
    return E_NOTIMPL;
}
