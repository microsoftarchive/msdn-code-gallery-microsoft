//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include "NetworkChannel.h"
#include "NetworkOperations.h"

using namespace Stsp::Net;

CNetworkChannel::CNetworkChannel()
    : _IsClosed(false)
{
}

CNetworkChannel::~CNetworkChannel(void)
{
    assert(FAILED(CheckClosed()));
}

IFACEMETHODIMP CNetworkChannel::BeginSend (_In_ IBufferPacket *pPacket, IMFAsyncCallback *pCallback, _In_opt_ IUnknown *pState)
{
    if (pPacket == nullptr || pCallback == nullptr)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);
    HRESULT hr = CheckClosed();
    ComPtr<IStreamSocket> spSocket = GetSocket();
    ComPtr<IOutputStream> spOutputStream;
    ComPtr<IAsyncOperationWithProgressCompletedHandler<UINT32,UINT32> > spSendOp;

    if (SUCCEEDED(hr) && !spSocket)
    {
        hr = MF_E_NET_NOCONNECTION;
    }

    if (SUCCEEDED(hr))
    {
        hr = spSocket->get_OutputStream(&spOutputStream);
    }

    if (SUCCEEDED(hr))
    {
        hr = CSendOperation::CreateInstance(pPacket->GetBufferCount(), pCallback, pState, &spSendOp);
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<IAsyncOperationWithProgress<UINT32,UINT32> > spOperation;
        ComPtr<IBufferEnumerator> spEn;
        hr = pPacket->GetEnumerator(&spEn);

        if (SUCCEEDED(hr) && !spEn->IsValid())
        {
            // packet is empty
            hr = E_INVALIDARG; 
        }

        for(; SUCCEEDED(hr) && spEn->IsValid(); spEn->MoveNext())
        {
            ComPtr<IMediaBufferWrapper> spBuffer;
            ComPtr<IBuffer> spWinRtBuffer;
            DWORD dwLen = 0;

            hr  = spEn->GetCurrent(&spBuffer);
            if (FAILED(hr))
            {
                break;
            }

            hr = spBuffer.As(&spWinRtBuffer);
            if (FAILED(hr))
            {
                break;
            }

            hr = spBuffer->GetCurrentLength(&dwLen);
            if (FAILED(hr))
            {
                break;
            }

            if (SUCCEEDED(hr))
            {
                spOutputStream->WriteAsync(spWinRtBuffer.Get(), spOperation.ReleaseAndGetAddressOf());
            }

            if (SUCCEEDED(hr))
            {
                // Set completion handler
                hr = spOperation->put_Completed(spSendOp.Get());
            }
        }
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CNetworkChannel::EndSend (_In_ IMFAsyncResult * pResult )
{
    if (pResult == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = pResult->GetStatus();
    ComPtr<ISendOperationContext> spContext;

    if (SUCCEEDED(hr))
    {
        ComPtr<IUnknown> spunkContext;

        hr = pResult->GetObject(&spunkContext);
        if (SUCCEEDED(hr))
        {
            hr = spunkContext.As(&spContext);
        }
    }

    if (SUCCEEDED(hr))
    {
        TRACE(TRACE_LEVEL_LOW, L"Sent data %d bytes\n", spContext->GetBytesWritten()); 
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CNetworkChannel::BeginReceive (_In_ IMediaBufferWrapper *pBuffer, IMFAsyncCallback *pCallback, _In_opt_ IUnknown *pState)
{
    if (pBuffer == nullptr || pCallback == nullptr)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);
    HRESULT hr = CheckClosed();
    ComPtr<IStreamSocket> spSocket = GetSocket();
    ComPtr<IInputStream> spInputStream;
    ComPtr<IBuffer> spWinRtBuffer;
    ComPtr<IAsyncOperationWithProgressCompletedHandler<IBuffer*,UINT32> > spContext;
    ComPtr<IAsyncOperationWithProgress<IBuffer*,UINT32> > spOperation;
    DWORD cbBufferLen = 0;

    if (SUCCEEDED(hr) && !spSocket)
    {
        hr = MF_E_NET_NOCONNECTION;
    }

    if (SUCCEEDED(hr))
    {
        hr = spSocket->get_InputStream(&spInputStream);
    }

    if (SUCCEEDED(hr))
    {
        hr = pBuffer->GetMediaBuffer()->GetMaxLength(&cbBufferLen);
    }

    if (SUCCEEDED(hr))
    {
        hr = pBuffer->QueryInterface(IID_PPV_ARGS(spWinRtBuffer.ReleaseAndGetAddressOf()));
    }

    if (SUCCEEDED(hr))
    {
        hr = CReceiveOperation::CreateInstance(pCallback, pState, &spContext);
    }

    if (SUCCEEDED(hr))
    {
        hr = spInputStream->ReadAsync(spWinRtBuffer.Get(), cbBufferLen, InputStreamOptions::InputStreamOptions_Partial, spOperation.ReleaseAndGetAddressOf());
    }

    if (SUCCEEDED(hr))
    {
        hr = spOperation->put_Completed(spContext.Get());
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CNetworkChannel::EndReceive (_In_ IMFAsyncResult * pResult)
{
    if (pResult == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = pResult->GetStatus();

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CNetworkChannel::Close ()
{
    AutoLock lock(_critSec);

    Disconnect();

    OnClose();

    _IsClosed = true;

    return S_OK;
}

IFACEMETHODIMP CNetworkChannel::Disconnect ()
{
    AutoLock lock(_critSec);
    ComPtr<IClosable> spClosable;

    if (_spSocket)
    {
        if (SUCCEEDED(_spSocket.As(&spClosable)))
        {
            spClosable->Close();
        }
        _spSocket.Reset();
    }

    return S_OK;
}
