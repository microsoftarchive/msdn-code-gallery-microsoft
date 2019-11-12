//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "stdafx.h"
#include "HostDescription.h"

using namespace Stsp::Net;

HRESULT CreateHostDescription(_In_ LPCWSTR pszHostName, _In_ LPCWSTR pszHostService, StspNetworkType eNetworkType, _Outptr_ IHostDescription **ppHostDesc)
{
    return CHostDescription::CreateInstance(pszHostName, pszHostService, eNetworkType, ppHostDesc);
}

HRESULT CHostDescription::CreateInstance(_In_ LPCWSTR pszHostName, _In_ LPCWSTR pszHostService, StspNetworkType eNetworkType, _Outptr_ IHostDescription **ppHostDesc)
{
    HRESULT hr = S_OK;

    if (pszHostName == nullptr || pszHostService == nullptr || ppHostDesc == nullptr)
    {
        return E_INVALIDARG;
    }

    ComPtr<CHostDescription> spResult;
    spResult.Attach(new CHostDescription());
    if (spResult == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = spResult->Initialize(pszHostName, pszHostService, eNetworkType);
    }

    if (SUCCEEDED(hr))
    {
        *ppHostDesc = spResult.Detach();
    }

    return hr;
}

// IUnknown
// IUnknown methods
IFACEMETHODIMP CHostDescription::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || riid == __uuidof(IHostDescription))
    {
        (*ppv) = static_cast<IHostDescription*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CHostDescription::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CHostDescription::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

// IHostDescription
IFACEMETHODIMP_(LPCWSTR) CHostDescription::GetHostName()
{
    return _pszHostName;
}

IFACEMETHODIMP_(LPCWSTR) CHostDescription::GetHostService()
{
    return _pszHostService;
}

IFACEMETHODIMP_(StspNetworkType) CHostDescription::GetNetworkType()
{
    return _eNetworkType;
}

CHostDescription::CHostDescription()
    : _cRef(1)
    , _pszHostName(nullptr)
    , _pszHostService(nullptr)
    , _eNetworkType(StspNetworkType_IPv4)
{
}

CHostDescription::~CHostDescription()
{
    delete[] _pszHostName;
    _pszHostName = nullptr;
    delete[] _pszHostService;
    _pszHostService = nullptr;
}

HRESULT CHostDescription::Initialize(_In_ LPCWSTR pszHostName, _In_ LPCWSTR pszHostService, StspNetworkType eNetworkType)
{
    if (pszHostName == nullptr || pszHostService == nullptr )
    {
        return E_INVALIDARG;
    }

    size_t len;
    HRESULT hr = StringCchLength(pszHostName, STRSAFE_MAX_CCH, &len);
    if (SUCCEEDED(hr))
    {
        _pszHostName = new WCHAR[len+1];
        if (_pszHostName == nullptr)
        {
            hr = E_OUTOFMEMORY;
        }
        else
        {
            hr = StringCchCopy(_pszHostName, len+1, pszHostName);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = StringCchLength(pszHostService, STRSAFE_MAX_CCH, &len);
    }
    if (SUCCEEDED(hr))
    {
        _pszHostService = new WCHAR[len+1];
        if (_pszHostService == nullptr)
        {
            hr = E_OUTOFMEMORY;
        }
        else
        {
            hr = StringCchCopy(_pszHostService, len+1, pszHostService);
        }
    }

    if (SUCCEEDED(hr))
    {
        _eNetworkType = eNetworkType;
    }

    return hr;
}
