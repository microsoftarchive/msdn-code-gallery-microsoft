//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include <strsafe.h>
#include "NetworkClient.h"
#include "NetworkOperations.h"

using namespace Stsp::Net;

HRESULT CreateNetworkClient(_Outptr_ INetworkClient **ppNetworkClient)
{
    return CNetworkClient::CreateInstance(ppNetworkClient);
}

/* static */ HRESULT CNetworkClient::CreateInstance(_Outptr_ INetworkClient **ppNetworkClient)
{
    if (ppNetworkClient == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;
    ComPtr<CNetworkClient> spNetworkClient;
    spNetworkClient.Attach(new CNetworkClient());   // Created with ref count = 1.

    if (!spNetworkClient)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = spNetworkClient->Initialize();
    }

    if (SUCCEEDED(hr))
    {
        *ppNetworkClient = spNetworkClient.Detach();
    }

    TRACEHR_RET(hr);
}

CNetworkClient::CNetworkClient()
: _cRef(1)
{
}

CNetworkClient::~CNetworkClient()
{
}

// IUnknown methods
IFACEMETHODIMP CNetworkClient::QueryInterface(REFIID riid, void** ppv)
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
    else if(riid == __uuidof(INetworkClient))
    {
        (*ppv) = static_cast<INetworkClient*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CNetworkClient::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CNetworkClient::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

IFACEMETHODIMP CNetworkClient::BeginConnect(
_In_ LPCWSTR szUrl,
WORD wPort,
_In_ IMFAsyncCallback * pCallback,
_In_opt_ IUnknown * pState )
{
    if (szUrl == nullptr || pCallback == nullptr)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);
    HRESULT hr = CheckClosed();
    WCHAR szPortNumber[6];
    ComPtr<IHostName> spHostName;
    ComPtr<IHostNameFactory> spHostNameFactory;
    ComPtr<IStreamSocket> spSocket = GetSocket();
    ComPtr<IAsyncAction> spOperation;
    ComPtr<IAsyncActionCompletedHandler> spContext;
    HStringReference hostNameId(RuntimeClass_Windows_Networking_HostName);
    HString strUrl;

    strUrl.Set((LPWSTR)szUrl);

    if (SUCCEEDED(hr) && !spSocket)
    {
        hr = MF_E_NOT_INITIALIZED;
    }

    if (SUCCEEDED(hr))
    {
        if (wPort == 0)
        {
            wPort = c_wStspDefaultPort;
        }
        hr = StringCchPrintf(szPortNumber, _countof(szPortNumber), L"%hu", wPort);
    }

    if (SUCCEEDED(hr))
    {
        hr = GetActivationFactory(hostNameId.Get(), &spHostNameFactory);
    }

    if (SUCCEEDED(hr))
    {
        hr = spHostNameFactory->CreateHostName(strUrl.Get(), &spHostName);
    }

    if (SUCCEEDED(hr))
    {
        hr = CConnectOperation::CreateInstance(pCallback, pState, &spContext);
    }

    if (SUCCEEDED(hr))
    {
        HString strServiceName;
        hr = strServiceName.Set(szPortNumber);
        if (SUCCEEDED(hr))
        {
            // Start asynchronous operation
            hr = spSocket->ConnectAsync(spHostName.Get(), strServiceName.Get(), &spOperation);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Set completion handler
        hr = spOperation->put_Completed(spContext.Get());
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CNetworkClient::EndConnect(
_In_ IMFAsyncResult * pResult )
{
    if (pResult == nullptr)
    {
        return E_INVALIDARG;
    }

    return pResult->GetStatus();
}

HRESULT CNetworkClient::Initialize()
{
    if (GetSocket() != nullptr)
    {
        return MF_E_ALREADY_INITIALIZED;
    }

    HStringReference ref(RuntimeClass_Windows_Networking_Sockets_StreamSocket);

    ComPtr<IStreamSocket> spSocket;
    HRESULT hr = Windows::Foundation::ActivateInstance<ComPtr<IStreamSocket>>(ref.Get(), &spSocket);

    if (SUCCEEDED(hr))
    {
        SetSocket(spSocket.Get());
    }
    else
    {
        Close();
    }

    TRACEHR_RET(hr);
}
