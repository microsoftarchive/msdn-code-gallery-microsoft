//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include <Strsafe.h>
#include "NetworkOperations.h"

using namespace Stsp::Net;

HRESULT CConnectOperation::CreateInstance(IMFAsyncCallback *pCallback, IUnknown *pState, IAsyncActionCompletedHandler **ppOp)
{
    if (pCallback == nullptr || ppOp == nullptr)
    {
        return E_INVALIDARG;
    }

    ComPtr<CConnectOperation> spOperation;
    spOperation.Attach(new CConnectOperation(pCallback, pState));

    if (!spOperation)
    {
        return E_OUTOFMEMORY;
    }

    *ppOp = spOperation.Detach();

    return S_OK;
}

CConnectOperation::CConnectOperation(IMFAsyncCallback *pCallback, IUnknown *pState)
: _cRef(1)
, _spCallback(pCallback)
, _spState(pState)
{
}

CConnectOperation::~CConnectOperation()
{
}

// IUnknown methods
IFACEMETHODIMP CConnectOperation::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || riid == __uuidof(IAsyncActionCompletedHandler))
    {
        (*ppv) = static_cast<IAsyncActionCompletedHandler*>(this);
        AddRef();
    }
    else if(riid == __uuidof(INetworkOperationContext))
    {
        (*ppv) = static_cast<INetworkOperationContext*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CConnectOperation::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CConnectOperation::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

IFACEMETHODIMP CConnectOperation::Invoke( 
_In_opt_ IAsyncAction *asyncInfo, AsyncStatus status)
{
    if (asyncInfo == nullptr)
    {
        return E_POINTER;
    }
    HRESULT hrResult = asyncInfo->GetResults();
    ComPtr<IMFAsyncResult> spResult;
    ComPtr<IAsyncInfo> spAsyncInfo;
    ComPtr<IUnknown> spThisUnk;

    HRESULT hr = this->QueryInterface(IID_PPV_ARGS(&spThisUnk));

    if (SUCCEEDED(hr))
    {
        hr = MFCreateAsyncResult(spThisUnk.Get(), _spCallback.Get(), _spState.Get(), &spResult);
    }

    if (SUCCEEDED(hr))
    {
        spResult->SetStatus(hrResult);
        hr = MFInvokeCallback(spResult.Get());
    }

    if (SUCCEEDED(asyncInfo->QueryInterface(IID_IAsyncInfo, &spAsyncInfo)))
    {
        spAsyncInfo->Close();
    }

    return hr;
}

HRESULT CAcceptOperation::CreateInstance(IMFAsyncCallback *pCallback, IUnknown *pState, IAcceptOperationContext **ppOp)
{
    if (pCallback == nullptr || ppOp == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;
    ComPtr<CAcceptOperation> spOperation;
    spOperation.Attach(new CAcceptOperation(pCallback, pState));

    if (!spOperation)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = spOperation->Initialize();
    }

    if (SUCCEEDED(hr))
    {
        *ppOp = spOperation.Detach();
    }

    return hr;
}

CAcceptOperation::CAcceptOperation(IMFAsyncCallback *pCallback, IUnknown *pState)
: _cRef(1)
, _spCallback(pCallback)
, _spState(pState)
{
    ZeroMemory(&_token, sizeof(_token));
}

CAcceptOperation::~CAcceptOperation()
{
}

// IUnknown methods
IFACEMETHODIMP CAcceptOperation::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || riid == __uuidof(ITypedEventHandler<StreamSocketListener*,StreamSocketListenerConnectionReceivedEventArgs*>))
    {
        (*ppv) = static_cast<ITypedEventHandler<StreamSocketListener*,StreamSocketListenerConnectionReceivedEventArgs*>*>(this);
        AddRef();
    }
    else if (riid == __uuidof(IAsyncActionCompletedHandler))
    {
        (*ppv) = static_cast<IAsyncActionCompletedHandler*>(this);
        AddRef();
    }
    else if(riid == __uuidof(INetworkOperationContext) || riid == __uuidof(IAcceptOperationContext))
    {
        (*ppv) = static_cast<IAcceptOperationContext*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CAcceptOperation::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CAcceptOperation::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

IFACEMETHODIMP CAcceptOperation::Invoke(
                                        IStreamSocketListener *sender, 
                                        IStreamSocketListenerConnectionReceivedEventArgs *args)
{
    AutoLock lock(_critSec);

    ComPtr<IMFAsyncResult> spResult;
    ComPtr<IUnknown> spThisUnk;

    if (_spListener == nullptr)
    {
        // Already closed
        return S_OK;
    }

    HRESULT hrResult = args->get_Socket(&_spSocket);

    HRESULT hr = this->QueryInterface(IID_PPV_ARGS(&spThisUnk));

    if (SUCCEEDED(hr))
    {
        hr = MFCreateAsyncResult(spThisUnk.Get(), _spCallback.Get(), _spState.Get(), &spResult);
    }

    if (SUCCEEDED(hr))
    {
        spResult->SetStatus(hrResult);
        hr = MFInvokeCallback(spResult.Get());
    }

    // Stop receiving
    Close();

    return hr;
}

IFACEMETHODIMP CAcceptOperation::Invoke( 
                                        _In_opt_ IAsyncAction *asyncInfo, AsyncStatus status)
{
    if (asyncInfo == nullptr)
    {
        return E_POINTER;
    }

    // This method is invoked when BindServiceNameAsync finishes.
    HRESULT hrResult = asyncInfo->GetResults();
    HRESULT hr = S_OK;
    ComPtr<IMFAsyncResult> spResult;
    ComPtr<IAsyncInfo> spAsyncInfo;
    ComPtr<IUnknown> spThisUnk;

    // If we failed already inform the callerk.
    if (FAILED(hrResult))
    {
        Close();

        hr = this->QueryInterface(IID_PPV_ARGS(&spThisUnk));

        if (SUCCEEDED(hr))
        {
            hr = MFCreateAsyncResult(spThisUnk.Get(), _spCallback.Get(), _spState.Get(), &spResult);
        }

        if (SUCCEEDED(hr))
        {
            spResult->SetStatus(hrResult);
            hr = MFInvokeCallback(spResult.Get());
        }
    }

    if (SUCCEEDED(asyncInfo->QueryInterface(IID_IAsyncInfo, &spAsyncInfo)))
    {
        spAsyncInfo->Close();
    }

    return hr;
}

HRESULT CAcceptOperation::StartListeningAsync(unsigned short wPort)
{
    AutoLock lock(_critSec);
    WCHAR szPortNumber[6];
    ComPtr<IAsyncAction> spOperation;

    if (_spListener == nullptr)
    {
        return E_UNEXPECTED;
    }

    if (wPort == 0)
    {
        wPort = c_wStspDefaultPort;
    }

    HRESULT hr = StringCchPrintf(szPortNumber, _countof(szPortNumber), L"%hu", wPort);

    if (SUCCEEDED(hr))
    {
        HString strServiceName;
        hr = strServiceName.Set(szPortNumber);
        if (SUCCEEDED(hr))
        {
            hr = _spListener->BindServiceNameAsync(strServiceName.Get(), &spOperation);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = spOperation->put_Completed(this);
    }

    return hr;
}

void CAcceptOperation::Close()
{
    AutoLock lock(_critSec);

    if (_spListener != nullptr)
    {
        _spListener->remove_ConnectionReceived(_token);

        ComPtr<IClosable> spClosable;
        if (SUCCEEDED(_spListener.As(&spClosable)))
        {
            spClosable->Close();
        }

        _spListener.Reset();
    }
}

HRESULT CAcceptOperation::Initialize()
{
    ComPtr<IStreamSocketListener> spListener;
    HStringReference socketListenerId(RuntimeClass_Windows_Networking_Sockets_StreamSocketListener);

    HRESULT hr = Windows::Foundation::ActivateInstance(socketListenerId.Get(), &_spListener);

    if (SUCCEEDED(hr))
    {
        hr = _spListener->add_ConnectionReceived(this, &_token);
    }

    return hr;
}

HRESULT CSendOperation::CreateInstance(long cSendOperations, IMFAsyncCallback *pCallback, IUnknown *pState, IAsyncOperationWithProgressCompletedHandler<UINT32, UINT32> **ppOp)
{
    if (pCallback == nullptr || ppOp == nullptr)
    {
        return E_INVALIDARG;
    }

    ComPtr<CSendOperation> spOperation;
    spOperation.Attach(new CSendOperation(cSendOperations, pCallback, pState));

    if (!spOperation)
    {
        return E_OUTOFMEMORY;
    }

    *ppOp = spOperation.Detach();

    return S_OK;
}

CSendOperation::CSendOperation(long cSendOperations, IMFAsyncCallback *pCallback, IUnknown *pState)
: _cRef(1)
, _spCallback(pCallback)
, _spState(pState)
, _cSendOperations(cSendOperations)
, _hResult(S_OK)
, _cbWritten(0)
{
}

CSendOperation::~CSendOperation()
{
}

// IUnknown methods
IFACEMETHODIMP CSendOperation::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || riid == __uuidof(IAsyncOperationWithProgressCompletedHandler<UINT32, UINT32>))
    {
        (*ppv) = static_cast<IAsyncOperationWithProgressCompletedHandler<UINT32, UINT32>*>(this);
        AddRef();
    }
    else if(riid == __uuidof(INetworkOperationContext) || riid == __uuidof(ISendOperationContext))
    {
        (*ppv) = static_cast<ISendOperationContext*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CSendOperation::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CSendOperation::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

IFACEMETHODIMP CSendOperation::Invoke( 
    _In_opt_ IAsyncOperationWithProgress<UINT32, UINT32> *asyncInfo, AsyncStatus status )
{
    if (asyncInfo == nullptr)
    {
        return E_POINTER;
    }

    UINT32 cbWritten;
    HRESULT hResult = asyncInfo->GetResults(&cbWritten);
    ComPtr<IMFAsyncResult> spResult;
    ComPtr<IAsyncInfo> spAsyncInfo;
    ComPtr<IUnknown> spThisUnk;

    InterlockedExchangeAdd(&_cbWritten, cbWritten);

    if (FAILED(hResult))
    {
        InterlockedCompareExchange(&_hResult, hResult, S_OK);
    }

    UINT32 cLeft = InterlockedDecrement(&_cSendOperations);
    if (cLeft == 0)
    {
        ReportResult();
    }

    if (SUCCEEDED(asyncInfo->QueryInterface(IID_IAsyncInfo, &spAsyncInfo)))
    {
        spAsyncInfo->Close();
    }

    return S_OK;
}

HRESULT CSendOperation::ReportResult()
{
    ComPtr<IMFAsyncResult> spResult;
    ComPtr<IUnknown> spThisUnk;
    HRESULT hr = QueryInterface(IID_PPV_ARGS(&spThisUnk));

    if (SUCCEEDED(hr))
    {
        hr = MFCreateAsyncResult(spThisUnk.Get(), _spCallback.Get(), _spState.Get(), &spResult);
    }

    if (SUCCEEDED(hr))
    {
        spResult->SetStatus(_hResult);
        hr = MFInvokeCallback(spResult.Get());
    }

    return hr;
}

HRESULT CReceiveOperation::CreateInstance(IMFAsyncCallback *pCallback, IUnknown *pState, IAsyncOperationWithProgressCompletedHandler<IBuffer*,UINT32> **ppOp)
{
    if (pCallback == nullptr || ppOp == nullptr)
    {
        return E_INVALIDARG;
    }

    ComPtr<CReceiveOperation> spOperation;
    spOperation.Attach(new CReceiveOperation(pCallback, pState));

    if (!spOperation)
    {
        return E_OUTOFMEMORY;
    }

    *ppOp = spOperation.Detach();

    return S_OK;
}

CReceiveOperation::CReceiveOperation(IMFAsyncCallback *pCallback, IUnknown *pState)
: _cRef(1)
, _spCallback(pCallback)
, _spState(pState)
{
}

CReceiveOperation::~CReceiveOperation()
{
}

// IUnknown methods
IFACEMETHODIMP CReceiveOperation::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || riid == __uuidof(IAsyncOperationWithProgressCompletedHandler<IBuffer*, UINT32>))
    {
        (*ppv) = static_cast<IAsyncOperationWithProgressCompletedHandler<IBuffer*, UINT32>*>(this);
        AddRef();
    }
    else if(riid == __uuidof(INetworkOperationContext) || riid == __uuidof(IReceiveOperationContext))
    {
        (*ppv) = static_cast<IReceiveOperationContext*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CReceiveOperation::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CReceiveOperation::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

IFACEMETHODIMP CReceiveOperation::Invoke( 
    _In_opt_ IAsyncOperationWithProgress<IBuffer*, UINT32> *asyncInfo, AsyncStatus status )
{
    if (asyncInfo == nullptr)
    {
        return E_POINTER;
    }

    ComPtr<IBuffer> spWinRtBuffer;
    HRESULT hrResult = asyncInfo->GetResults(&spWinRtBuffer);
    ComPtr<IMFAsyncResult> spResult;
    ComPtr<IAsyncInfo> spAsyncInfo;
    ComPtr<IInputStream> spInputStream;
    ComPtr<IDataReader> spDataReader;
    DWORD cbBufferLen = 0;

    if (SUCCEEDED(hrResult))
    {
        hrResult = spWinRtBuffer->get_Length(&_cbRead);
    }

    if (SUCCEEDED(hrResult) && _cbRead == 0)
    {
        hrResult = MF_E_NET_READ;
    }

    ComPtr<IUnknown> spThisUnk;
    HRESULT hr = this->QueryInterface(IID_PPV_ARGS(&spThisUnk));

    if (SUCCEEDED(hr))
    {
        hr = MFCreateAsyncResult(spThisUnk.Get(), _spCallback.Get(), _spState.Get(), &spResult);
    }

    if (SUCCEEDED(hr))
    {
        spResult->SetStatus(hrResult);
        hr = MFInvokeCallback(spResult.Get());
    }

    if (SUCCEEDED(asyncInfo->QueryInterface(IID_IAsyncInfo, &spAsyncInfo)))
    {
        spAsyncInfo->Close();
    }

    return hr;
}
