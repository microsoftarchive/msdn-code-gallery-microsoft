#include "pch.h"
#include "MediaStreamSchemeHandler.h"
#include "MediaStreamSourceService.h"


CMediaStreamSchemeHandler::CMediaStreamSchemeHandler(void)
{
}


CMediaStreamSchemeHandler::~CMediaStreamSchemeHandler(void)
{
}

// IMediaExtension methods
IFACEMETHODIMP CMediaStreamSchemeHandler::SetProperties (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
    HRESULT hr = E_INVALIDARG;

    if(pConfiguration)
    {
        ComPtr<IInspectable> spInsp;
        ComPtr<ABI::Windows::Foundation::Collections::IMap<HSTRING, IInspectable *>> spSetting;

        hr = pConfiguration->QueryInterface(IID_PPV_ARGS(&spSetting));
        if (FAILED(hr))
        {
            hr = E_FAIL;
            goto done;
        }

        HStringReference strKey(L"plugin");
        hr = spSetting->Lookup(strKey.Get(), spInsp.ReleaseAndGetAddressOf());
        if(FAILED(hr))
        {
            hr = E_INVALIDARG;
            goto done;
        }

        hr = spInsp.As(&_spPlugin);
        if(FAILED(hr))
        {
            hr = E_INVALIDARG;
            goto done;
        }

    }

done:
    return hr;
}

// IMFSchemeHandler methods
IFACEMETHODIMP CMediaStreamSchemeHandler::BeginCreateObject( 
        __in LPCWSTR pwszURL,
        __in DWORD dwFlags,
        __in IPropertyStore *pProps,
        __out_opt  IUnknown **ppIUnknownCancelCookie,
        __in IMFAsyncCallback *pCallback,
        __in IUnknown *punkState)
{
    if (pwszURL == nullptr || pCallback == nullptr)
    {
        return E_INVALIDARG;
    }

    if ((dwFlags & MF_RESOLUTION_MEDIASOURCE) == 0)
    {
        return E_INVALIDARG;
    }

    if (_spPlugin == nullptr)
    {
        return MF_E_INVALIDREQUEST;
    }

    ComPtr<CMediaStreamSourceService> spSource;
    ComPtr<IMFAsyncResult> spResult;

    HRESULT hr = MakeAndInitialize<CMediaStreamSourceService>(&spSource, _spPlugin.Get(), pCallback, punkState);

    if (SUCCEEDED(hr))
    {
        hr = _spPlugin->OpenMediaAsync();
    }

    if (SUCCEEDED(hr) && ppIUnknownCancelCookie != nullptr)
    {
        *ppIUnknownCancelCookie = nullptr;
    }

    return hr;
}
        
IFACEMETHODIMP CMediaStreamSchemeHandler::EndCreateObject( 
        __in IMFAsyncResult *pResult,
        __out  MF_OBJECT_TYPE *pObjectType,
        __out  IUnknown **ppObject)
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

    return hr;
}
        
IFACEMETHODIMP CMediaStreamSchemeHandler::CancelObjectCreation( 
            __in IUnknown *pIUnknownCancelCookie)
{
    return E_NOTIMPL;
}
