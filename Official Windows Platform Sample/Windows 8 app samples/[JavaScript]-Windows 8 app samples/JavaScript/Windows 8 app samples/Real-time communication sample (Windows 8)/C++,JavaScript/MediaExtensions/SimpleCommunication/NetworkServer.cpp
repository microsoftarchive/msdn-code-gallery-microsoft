//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include <strsafe.h>
#include "NetworkServer.h"
#include "NetworkOperations.h"

using namespace Stsp::Net;

HRESULT CreateNetworkServer(unsigned short wPort, _Outptr_ INetworkServer **ppNetworkServer)
{
    return CNetworkServer::CreateInstance(wPort, ppNetworkServer);
}

/* static */ HRESULT CNetworkServer::CreateInstance(unsigned short wPort, INetworkServer **ppNetworkServer)
{
    if (ppNetworkServer == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;
    ComPtr<CNetworkServer> spNetworkServer;
    spNetworkServer.Attach(new CNetworkServer(wPort));   // Created with ref count = 1.

    if (!spNetworkServer)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        *ppNetworkServer = spNetworkServer.Detach();
    }

    TRACEHR_RET(hr);
}

CNetworkServer::CNetworkServer(unsigned short wPort)
: CNetworkChannel()
, _cRef(1)
, _wListeningPort(wPort)
{
}

CNetworkServer::~CNetworkServer()
{
}

// IUnknown methods
IFACEMETHODIMP CNetworkServer::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || riid == __uuidof(INetworkChannel))
    {
        (*ppv) = static_cast<INetworkChannel*>(this);
        AddRef();
    }
    else if(riid == __uuidof(INetworkServer))
    {
        (*ppv) = static_cast<INetworkServer*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CNetworkServer::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CNetworkServer::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

// INetworkServer methods
IFACEMETHODIMP CNetworkServer::BeginAccept (
_In_ IMFAsyncCallback * pCallback,
_In_ IUnknown * pState )
{
    if (pCallback == nullptr)
    {
        return E_INVALIDARG;
    }
    AutoLock lock(_critSec);
    HRESULT hr = CheckClosed();
    ComPtr<IAcceptOperationContext> spContext;
    DWORD bytes_read = 0;

    if (GetAcceptOperation() != nullptr)
    {
        // We are in the middle of accept oparation.
        hr = E_UNEXPECTED;
    }

    if (SUCCEEDED(hr))
    {
        // Disconnect if we are already connected. We can handle only one client connection.
        Disconnect();
    }

    if (SUCCEEDED(hr))
    {
        hr = CAcceptOperation::CreateInstance(pCallback, pState, &spContext);
    }

    if (SUCCEEDED(hr))
    {
        hr = spContext->StartListeningAsync(_wListeningPort);
    }

    if (SUCCEEDED(hr))
    {
        SetAcceptOperation(spContext.Get());
    }
    else if (spContext != nullptr)
    {
        spContext->Close();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CNetworkServer::EndAccept (
_In_ IMFAsyncResult * pResult, _Outptr_ IHostDescription **ppHostDescription)
{
    HRESULT hr = S_OK;
    ComPtr<IAcceptOperationContext> spContext;
    ComPtr<IStreamSocketInformation> spInfo;
    ComPtr<IHostDescription> spHostDescription;

    if (pResult == nullptr || ppHostDescription == nullptr)
    {
        return E_INVALIDARG;
    }

    if (GetAcceptOperation() == nullptr)
    {
        return E_UNEXPECTED;
    }

    hr = pResult->GetStatus();

    if (SUCCEEDED(hr))
    {
        ComPtr<IUnknown> spUnkContext;
        hr = pResult->GetObject(&spUnkContext);
        if (SUCCEEDED(hr))
        {
            hr = spUnkContext.As(&spContext);
        }
    }

    if (SUCCEEDED(hr) &&
        spContext.Get() != GetAcceptOperation())
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        hr = spContext->GetSocket()->get_Information(spInfo.GetAddressOf());
        if (SUCCEEDED(hr))
        {
            hr = CreateHostDescription(spInfo.Get(), spHostDescription.GetAddressOf());
        }
    }

    if (SUCCEEDED(hr))
    {
        this->SetSocket(spContext->GetSocket());

        if (GetSocket() == nullptr)
        {
            hr = E_UNEXPECTED;
        }
    }

    SetAcceptOperation(nullptr);

    if (SUCCEEDED(hr))
    {
        *ppHostDescription = spHostDescription.Detach();
    }

    TRACEHR_RET(hr);
}

__override void CNetworkServer::OnClose ()
{
    if (_spAcceptOperation)
    {
        _spAcceptOperation->Close();
        _spAcceptOperation.ReleaseAndGetAddressOf();
    }
}

HRESULT CNetworkServer::CreateHostDescription(IStreamSocketInformation *pInfo, IHostDescription **ppHostDescription)
{
    ComPtr<IHostName> spRemoteHostName;
    HString strRemoteHostRawName;
    HString strRemoteServiceName;
    HostNameType hnt;

    HRESULT hr = pInfo->get_RemoteHostName(spRemoteHostName.GetAddressOf());

    if (SUCCEEDED(hr))
    {
        hr = spRemoteHostName->get_Type(&hnt);
        if (SUCCEEDED(hr) && hnt != HostNameType::HostNameType_Ipv4 && hnt != HostNameType::HostNameType_Ipv6)
        {
            return E_INVALIDARG;
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = spRemoteHostName->get_RawName(strRemoteHostRawName.GetAddressOf());
    }

    if (SUCCEEDED(hr))
    {
        hr = pInfo->get_RemoteServiceName(strRemoteServiceName.GetAddressOf());
    }

    if (SUCCEEDED(hr))
    {
        UINT32 chRemoteHostRawNameLength = 0;
        UINT32 chRemoteServiceNameLength = 0;
        StspNetworkType eNetworkType = (hnt == HostNameType::HostNameType_Ipv4) ? StspNetworkType_IPv4 : StspNetworkType_IPv6;
        auto szRemoteHostRawName = WindowsGetStringRawBuffer(strRemoteHostRawName.Get(), &chRemoteHostRawNameLength);
        auto szRemoteServiceName = WindowsGetStringRawBuffer(strRemoteServiceName.Get(), &chRemoteServiceNameLength);

        hr = ::CreateHostDescription(szRemoteHostRawName, szRemoteServiceName, eNetworkType, ppHostDescription);
    }

    return hr;
}
