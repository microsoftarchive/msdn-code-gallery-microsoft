//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include "StspMediaSource.h"
#include "StspMediaStream.h"
#include <IntSafe.h>

using namespace Stsp;

namespace
{
    const DWORD c_cbReceiveBufferSize = 2 * 1024;
    const DWORD c_cbMaxPacketSize = 1024 * 1024;
};

CSourceOperation::CSourceOperation(CSourceOperation::Type opType)
: _cRef(1)
, _opType(opType)
{
    ZeroMemory(&_data, sizeof(_data));
}

CSourceOperation::~CSourceOperation()
{
    PropVariantClear(&_data);
}

// IUnknown methods
IFACEMETHODIMP CSourceOperation::QueryInterface(REFIID riid, void** ppv)
{
    return E_NOINTERFACE;
}

IFACEMETHODIMP_(ULONG) CSourceOperation::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CSourceOperation::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

HRESULT CSourceOperation::SetData(const PROPVARIANT& varData)
{
    return PropVariantCopy(&_data, &varData);
}

CStartOperation::CStartOperation(IMFPresentationDescriptor *pPD)
: CSourceOperation(CSourceOperation::Operation_Start)
, _spPD(pPD)
{
}

CStartOperation::~CStartOperation()
{
}

CSetRateOperation::CSetRateOperation(BOOL fThin, float flRate)
: CSourceOperation(CSourceOperation::Operation_SetRate)
, _fThin(fThin)
, _flRate(flRate)
{
}

CSetRateOperation::~CSetRateOperation()
{
}

CMediaSource::CMediaSource(void)
: OpQueue<CMediaSource, CSourceOperation>(_critSec.m_criticalSection)
, _cRef(1)
, _eSourceState(SourceState_Invalid)
, _serverPort(0)
, _flRate(1.0f)
#pragma warning(push)
#pragma warning(disable: 4355)
, _OnConnectedCB(this, &CMediaSource::OnConnected)
, _OnDescriptionRequestSentCB(this, &CMediaSource::OnDescriptionRequestSent)
, _OnStartRequestSentCB(this, &CMediaSource::OnStartRequestSent)
, _OnDataReceivedCB(this, &CMediaSource::OnDataReceived)
#pragma warning(pop)
{
    ZeroMemory(&_CurrentReceivedOperationHeader, sizeof(_CurrentReceivedOperationHeader));
}

CMediaSource::~CMediaSource(void)
{
}

HRESULT CMediaSource::CreateInstance(CMediaSource **ppNetSource)
{
    HRESULT hr = S_OK;

    if (ppNetSource == nullptr)
    {
        return E_INVALIDARG;
    }

    ComPtr<CMediaSource> spSource;
    spSource.Attach(new CMediaSource());
    if (!spSource)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = spSource->Initialize();
    }

    if (SUCCEEDED(hr))
    {
        *ppNetSource = spSource.Detach();
    }

    TRACEHR_RET(hr);
}

// IUnknown methods

IFACEMETHODIMP CMediaSource::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || 
        riid == IID_IMFMediaSource ||
        riid == IID_IMFMediaEventGenerator) 
    {
        (*ppv) = static_cast<IMFMediaSource*>(this);
        AddRef();
    }
    else if (riid == IID_IMFAttributes)
    {
        (*ppv) = static_cast<IMFAttributes*>(this);
        AddRef();
    }
    else if (riid == IID_IMFGetService)
    {
        (*ppv) = static_cast<IMFGetService*>(this);
        AddRef();
    }
    else if (riid == IID_IMFRateControl)
    {
        (*ppv) = static_cast<IMFRateControl*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CMediaSource::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CMediaSource::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

// IMFMediaEventGenerator methods.
// Note: These methods call through to the event queue helper object.
IFACEMETHODIMP CMediaSource::BeginGetEvent(IMFAsyncCallback* pCallback, IUnknown* punkState)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->BeginGetEvent(pCallback, punkState);
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSource::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->EndGetEvent(pResult, ppEvent);
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSource::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
{
    // NOTE:
    // GetEvent can block indefinitely, so we don't hold the lock.
    // This requires some juggling with the event queue pointer.

    HRESULT hr = S_OK;

    IMFMediaEventQueue *pQueue = NULL;

    {
        AutoLock lock(_critSec);

        // Check shutdown
        hr = CheckShutdown();

        // Get the pointer to the event queue.
        if (SUCCEEDED(hr))
        {
            pQueue = _spEventQueue.Get();
            pQueue->AddRef();
        }
    }

    // Now get the event.
    if (SUCCEEDED(hr))
    {
        hr = pQueue->GetEvent(dwFlags, ppEvent);
    }

    SafeRelease(&pQueue);

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSource::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, PROPVARIANT const *pvValue)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
    }

    TRACEHR_RET(hr);
}

// IMFMediaSource methods
IFACEMETHODIMP CMediaSource::CreatePresentationDescriptor(
IMFPresentationDescriptor** ppPresentationDescriptor
)
{
    if (ppPresentationDescriptor == NULL)
    {
        return E_POINTER;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr) &&
        (_eSourceState == SourceState_Opening || _eSourceState == SourceState_Invalid || !_spPresentationDescriptor))
    {
        hr = MF_E_NOT_INITIALIZED;
    }

    if (SUCCEEDED(hr))
    {
        hr = _spPresentationDescriptor->Clone(ppPresentationDescriptor);
    }

    return hr;
}

IFACEMETHODIMP CMediaSource::GetCharacteristics(DWORD* pdwCharacteristics)
{
    if (pdwCharacteristics == NULL)
    {
        return E_POINTER;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *pdwCharacteristics = MFMEDIASOURCE_IS_LIVE;
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSource::Pause()
{
    return MF_E_INVALID_STATE_TRANSITION;
}

IFACEMETHODIMP CMediaSource::Shutdown()
{
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_spEventQueue)
        {
            _spEventQueue->Shutdown();
        }
        if (_spNetworkSender)
        {
            _spNetworkSender->Close();
        }

        StreamContainer::POSITION pos = _streams.FrontPosition();
        while (pos != _streams.EndPosition())
        {
            ComPtr<IMFMediaStream> spStream;
            hr = _streams.GetItemPos(pos, &spStream);
            pos = _streams.Next(pos);
            if (SUCCEEDED(hr))
            {
                static_cast<CMediaStream*>(spStream.Get())->Shutdown();
            }
        }

        _eSourceState = SourceState_Shutdown;

        _streams.Clear();

        _spEventQueue.ReleaseAndGetAddressOf();
        _spNetworkSender.ReleaseAndGetAddressOf();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSource::Start(
                                   IMFPresentationDescriptor* pPresentationDescriptor,
                                   const GUID* pguidTimeFormat,
                                   const PROPVARIANT* pvarStartPos
                                   )
{
    HRESULT hr = S_OK;

    // Check parameters.

    // Start position and presentation descriptor cannot be NULL.
    if (pvarStartPos == nullptr || pPresentationDescriptor == nullptr)
    {
        return E_INVALIDARG;
    }

    // Check the time format.
    if ((pguidTimeFormat != nullptr) && (*pguidTimeFormat != GUID_NULL))
    {
        // Unrecognized time format GUID.
        return MF_E_UNSUPPORTED_TIME_FORMAT;
    }

    // Check the data type of the start position.
    if (pvarStartPos->vt != VT_EMPTY && pvarStartPos->vt != VT_I8)
    {
        return MF_E_UNSUPPORTED_TIME_FORMAT;
    }

    AutoLock lock(_critSec);
    ComPtr<CStartOperation> spStartOp;

    if (_eSourceState != SourceState_Stopped &&
        _eSourceState != SourceState_Started)
    {
        hr = MF_E_INVALIDREQUEST;
    }

    if (SUCCEEDED(hr))
    {
        // Check if the presentation description is valid.
        hr = ValidatePresentationDescriptor(pPresentationDescriptor);
    }

    if (SUCCEEDED(hr))
    {
        // Prepare asynchronous operation attributes
        spStartOp.Attach(new CStartOperation(pPresentationDescriptor));
        if (!spStartOp)
        {
            hr = E_OUTOFMEMORY;
        }

        if (SUCCEEDED(hr))
        {
            hr = spStartOp->SetData(*pvarStartPos);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Queue asynchronous operation
        hr = QueueOperation(spStartOp.Get());
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSource::Stop()
{
    HRESULT hr = S_OK;
    ComPtr<CSourceOperation> spStopOp;
    spStopOp.Attach(new CSourceOperation(CSourceOperation::Operation_Stop));
    if (!spStopOp)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        // Queue asynchronous stop
        hr = QueueOperation(spStopOp.Get());
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSource::GetService( _In_ REFGUID guidService, _In_ REFIID riid, _Out_opt_ LPVOID *ppvObject)
{
    HRESULT hr = MF_E_UNSUPPORTED_SERVICE;

    if (ppvObject == nullptr)
    {
        return E_POINTER;
    }

    if (guidService == MF_RATE_CONTROL_SERVICE)
    {
        hr = QueryInterface(riid, ppvObject);
    }

    return hr;
}

IFACEMETHODIMP CMediaSource::SetRate(BOOL fThin, float flRate)
{
    if (fThin)
    {
        return MF_E_THINNING_UNSUPPORTED;
    }
    if (!IsRateSupported(flRate, &flRate))
    {
        return MF_E_UNSUPPORTED_RATE;
    }

    AutoLock lock(_critSec);
    HRESULT hr = S_OK;

    if (flRate == _flRate)
    {
        return S_OK;
    }

    ComPtr<CSourceOperation> spSetRateOp;
    spSetRateOp.Attach(new CSetRateOperation(fThin, flRate));
    if (!spSetRateOp)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        // Queue asynchronous stop
        hr = QueueOperation(spSetRateOp.Get());
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSource::GetRate(_Inout_opt_ BOOL *pfThin, _Inout_opt_ float *pflRate)
{
    AutoLock lock(_critSec);
    if (pfThin == nullptr || pflRate == nullptr)
    {
        return E_INVALIDARG;
    }

    *pfThin = FALSE;
    *pflRate = _flRate;

    return S_OK;
}

__override HRESULT CMediaSource::DispatchOperation(_In_ CSourceOperation *pOp)
{
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        switch(pOp->GetOperationType())
        {
        case CSourceOperation::Operation_Start:
            hr = DoStart(static_cast<CStartOperation *>(pOp));
            break;
        case CSourceOperation::Operation_Stop:
            hr = DoStop(pOp);
            break;
        case CSourceOperation::Operation_SetRate:
            hr = DoSetRate(static_cast<CSetRateOperation *>(pOp));
            break;
        default:
            hr = E_UNEXPECTED;
            break;
        }
    }

    TRACEHR_RET(hr);
}

__override HRESULT CMediaSource::ValidateOperation(_In_ CSourceOperation *pOp)
{
    return S_OK;
}

HRESULT CMediaSource::BeginOpen(LPCWSTR pszUrl, IMFAsyncCallback *pCB, IUnknown *pUnkState)
{
    if (pszUrl == nullptr || pCB == nullptr)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);
    HRESULT hr = S_OK;
    ComPtr<INetworkClient> spNetworkClient;

    if (_eSourceState != SourceState_Invalid)
    {
        return MF_E_INVALIDREQUEST;
    }

    // Get network client
    hr = _spNetworkSender.As(&spNetworkClient);

    if (SUCCEEDED(hr))
    {
        // Parse url (to obtain host name and port)
        hr = ParseServerUrl(pszUrl);
    }

    if (SUCCEEDED(hr))
    {
        hr = MFCreateAsyncResult(nullptr, pCB, pUnkState, &_spOpenResult);
    }

    if (SUCCEEDED(hr))
    {
        LPCWSTR pszServerAddress = nullptr;

        if (SUCCEEDED(hr))
        {
            pszServerAddress = WindowsGetStringRawBuffer(_strServerAddress.Get(), nullptr);

            // Begin connection
            hr = spNetworkClient->BeginConnect(pszServerAddress, _serverPort, &_OnConnectedCB, nullptr);
        }
    }

    if (SUCCEEDED(hr))
    {
        // If everything is ok now we are waiting for network client to connect. 
        // Change state to opening.
        _eSourceState = SourceState_Opening;
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::EndOpen(IMFAsyncResult *pResult)
{
    HRESULT hr = pResult->GetStatus();

    if (FAILED(hr))
    {
        Shutdown();
    }

    TRACEHR_RET(hr);
}

_Acquires_lock_(_critSec)
HRESULT CMediaSource::Lock()
{
    _critSec.Lock();
    return S_OK;
}

_Releases_lock_(_critSec)
HRESULT CMediaSource::Unlock()
{
    _critSec.Unlock();
    return S_OK;
}

HRESULT CMediaSource::Initialize()
{
    HRESULT hr = S_OK;
    ComPtr<INetworkClient> spNetworkClient;

    // Create the event queue helper.
    hr = MFCreateEventQueue(&_spEventQueue);

    if (SUCCEEDED(hr))
    {
        hr = CBaseAttributes<>::Initialize();
    }

    if (SUCCEEDED(hr))
    {
        // Create network client
        hr = CreateNetworkClient(&spNetworkClient);
    }

    if (SUCCEEDED(hr))
    {
        hr = spNetworkClient.As(&_spNetworkSender);
    }

    if (FAILED(hr))
    {
        Shutdown();
    }

    TRACEHR_RET(hr);
}

// Handle errors
void CMediaSource::HandleError(HRESULT hResult)
{
    if (_eSourceState == SourceState_Opening)
    {
        // If we have an error during opening operation complete it and pass the error to client.
        CompleteOpen(hResult);
    }
    else if (_eSourceState != SourceState_Shutdown)
    {
        // If we received an error at any other time (except shutdown) send MEError event.
        QueueEvent(MEError, GUID_NULL, hResult, nullptr);
    }
}

// Returns stream object associated with given identifier
HRESULT CMediaSource::GetStreamById(DWORD dwId, CMediaStream **ppStream)
{
    assert(ppStream);
    HRESULT hr = MF_E_NOT_FOUND;
    StreamContainer::POSITION pos = _streams.FrontPosition();
    StreamContainer::POSITION posEnd = _streams.EndPosition();

    for (; pos != posEnd; pos = _streams.Next(pos))
    {
        ComPtr<IMFMediaStream> spStream;
        HRESULT hErrorCode = _streams.GetItemPos(pos, &spStream);
        if (FAILED(hErrorCode))
        {
            hr = hErrorCode;
            break;
        }
        CMediaStream* pStream = static_cast<CMediaStream*>(spStream.Get());
        if (pStream->GetId() == dwId)
        {
            *ppStream = pStream;
            (*ppStream)->AddRef();
            hr = S_OK;
        }
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::ParseServerUrl(LPCWSTR pszUrl)
{
    if (pszUrl == nullptr)
    {
        return E_INVALIDARG;
    }

    ComPtr<IUriRuntimeClassFactory> spUriFactory;
    ComPtr<IUriRuntimeClass> spUri;
    HString strUrl;
    HRESULT hr = strUrl.Set((wchar_t*)pszUrl);

    if (SUCCEEDED(hr))
    {
        hr = ABI::Windows::Foundation::GetActivationFactory(
            HStringReference(RuntimeClass_Windows_Foundation_Uri).Get(), 
            &spUriFactory);
    }

    if (SUCCEEDED(hr))
    {
        hr = spUriFactory->CreateUri(strUrl.Get(), &spUri);
    }

    if (SUCCEEDED(hr))
    {
        HString strScheme;
        LPCWSTR pszScheme = nullptr;
        hr = spUri->get_SchemeName(strScheme.GetAddressOf());

        if (SUCCEEDED(hr))
        {
            pszScheme = WindowsGetStringRawBuffer(strScheme.Get(), nullptr);
        }

        // It is not a scheme we support
        if (SUCCEEDED(hr) && wcscmp(pszScheme, c_szStspScheme) != 0)
        {
            hr = MF_E_UNSUPPORTED_SCHEME;
        }
    }

    if (SUCCEEDED(hr))
    {
        _strServerAddress.Release();
        hr = spUri->get_Host(_strServerAddress.GetAddressOf());
    }

    if (SUCCEEDED(hr))
    {
        int port = 0;
        hr = spUri->get_Port(&port);

        if (port == 0 || FAILED(hr))
        {
            _serverPort = c_wStspDefaultPort;
            hr = S_OK;
        }
        else if (port < 0 || port > WORD_MAX)
        {
            hr = E_UNEXPECTED;
        }
        else
        {
            _serverPort = (WORD)port;
        }
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::CompleteOpen(HRESULT hResult)
{
    assert(_spOpenResult);
    HRESULT hr = _spOpenResult->SetStatus(hResult);
    if (SUCCEEDED(hr))
    {
        // Invoke the user's callback 
        hr = MFInvokeCallback(_spOpenResult.Get());
    }

    _spOpenResult.ReleaseAndGetAddressOf();

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::SendRequest(StspOperation eOperation, IMFAsyncCallback *pCallback)
{
    ComPtr<IMediaBufferWrapper> spBuffer;
    ComPtr<IBufferPacket> spPacket;
    // We just send an operation header this operation contains no payload.
    HRESULT hr = CreateMediaBufferWrapper(sizeof(StspOperationHeader), &spBuffer);

    if (SUCCEEDED(hr))
    {
        hr = CreateBufferPacket(&spPacket);
    }

    if (SUCCEEDED(hr))
    {
        // Prepare operation header.
        StspOperationHeader *pOpHeader = reinterpret_cast<StspOperationHeader *>(spBuffer->GetBuffer());
        pOpHeader->cbDataSize = 0;
        pOpHeader->eOperation = eOperation;
        hr = spBuffer->SetCurrentLength(sizeof(StspOperationHeader));

        if (SUCCEEDED(hr))
        {
            hr = spPacket->AddBuffer(spBuffer.Get());
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = _spNetworkSender->BeginSend(spPacket.Get(), pCallback, nullptr);
    }

    TRACEHR_RET(hr);
}

// Sending request for media description to the server
HRESULT CMediaSource::SendDescribeRequest()
{
    _CurrentReceivedOperationHeader.eOperation = StspOperation_Unknown;
    return SendRequest(StspOperation_ClientRequestDescription, &_OnDescriptionRequestSentCB);
}

HRESULT CMediaSource::SendStartRequest()
{
    _CurrentReceivedOperationHeader.eOperation = StspOperation_Unknown;
    return SendRequest(StspOperation_ClientRequestStart, &_OnStartRequestSentCB);
}

// Trigger receiving operation
HRESULT CMediaSource::Receive()
{
    // We already during receive operation
    if (_spCurrentReceiveBuffer)
    {
        return MF_E_INVALIDREQUEST;
    }

    ComPtr<IMediaBufferWrapper> spBuffer;
    HRESULT hr = CreateMediaBufferWrapper(c_cbReceiveBufferSize, &spBuffer);

    if (SUCCEEDED(hr))
    {
        hr = _spNetworkSender->BeginReceive(spBuffer.Get(), &_OnDataReceivedCB, nullptr);
    }

    if (SUCCEEDED(hr))
    {
        _spCurrentReceiveBuffer = spBuffer;
    }

    TRACEHR_RET(hr);
}

// Parse data stored in current receive buffer
HRESULT CMediaSource::ParseCurrentBuffer()
{
    HRESULT hr = S_OK;
    bool fCompletePackage = false;
    DWORD cbTotalLength = 0;
    // Remember the buffer
    ComPtr<IMediaBufferWrapper> spLastBuffer = _spCurrentReceiveBuffer;

    if (SUCCEEDED(hr) && !_spCurrentReceivePacket)
    {
        // If packet object storing current operation data is not created yet - create it.
        hr = CreateBufferPacket(&_spCurrentReceivePacket);
    }

    if (SUCCEEDED(hr))
    {
        // Add current receive buffer to the packet.
        hr = _spCurrentReceivePacket->AddBuffer(_spCurrentReceiveBuffer.Get());
        _spCurrentReceiveBuffer.ReleaseAndGetAddressOf();
    }

    // Parsing new package, we have't read operation just yet
    if (SUCCEEDED(hr) && _CurrentReceivedOperationHeader.eOperation == StspOperation_Unknown)
    {
        DWORD cbCurrentLength;
        hr = _spCurrentReceivePacket->GetTotalLength(&cbCurrentLength);

        if (SUCCEEDED(hr))
        {
            // Check if we have enough data to retrieve operation header
            if(cbCurrentLength >= sizeof(StspOperationHeader))
            {
                hr = _spCurrentReceivePacket->MoveLeft(sizeof(_CurrentReceivedOperationHeader), &_CurrentReceivedOperationHeader);
            }
            else
            {
                // We don't have enough data just yet so set data size to maximum packet size (then we will pass validation performed later on)
                _CurrentReceivedOperationHeader.cbDataSize = c_cbMaxPacketSize;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        // Get length of the packet (without operation header which was removed from the packet).
        hr = _spCurrentReceivePacket->GetTotalLength(&cbTotalLength); 
    }

    if (SUCCEEDED(hr) && 
        // packet size is too large
            (c_cbMaxPacketSize < _CurrentReceivedOperationHeader.cbDataSize ||
            // Unrecognised operation
            _CurrentReceivedOperationHeader.eOperation >= StspOperation_Last))
    {
        hr = MF_E_UNSUPPORTED_FORMAT;
    }

    if (SUCCEEDED(hr) &&
        // Size of the package already exceeded value described in operation header
            cbTotalLength > _CurrentReceivedOperationHeader.cbDataSize)
    {
        // Trim the right edge of the packet and get buffer representing trimmed data
        hr = spLastBuffer->TrimRight(cbTotalLength - _CurrentReceivedOperationHeader.cbDataSize, &_spCurrentReceiveBuffer);
        // Fix total length
        cbTotalLength = _CurrentReceivedOperationHeader.cbDataSize;
    }

    // We've got a whole packet
    if (SUCCEEDED(hr) &&
        cbTotalLength == _CurrentReceivedOperationHeader.cbDataSize)
    {
        // Process packet payload
        hr = ProcessPacket(&_CurrentReceivedOperationHeader, _spCurrentReceivePacket.Get());
        _spCurrentReceivePacket.ReleaseAndGetAddressOf();
        _CurrentReceivedOperationHeader.eOperation =  StspOperation_Unknown;
    }

    if (SUCCEEDED(hr) && _spCurrentReceiveBuffer)
    {
        // We've got a buffer which was left from TrimRight operation above, we can keep parsing.
        hr = ParseCurrentBuffer();
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::ProcessPacket(StspOperationHeader *pOpHeader, IBufferPacket *pPacket)
{
    assert(pOpHeader);
    assert(pPacket);
    HRESULT hr = S_OK;

    switch(pOpHeader->eOperation)
    {
        // We received server description
    case StspOperation_ServerDescription:
        hr = ProcessServerDescription(pPacket);
        break;
        // We received a media sample
    case StspOperation_ServerSample:
        hr = ProcessServerSample(pPacket);
        break;
        // No supported operation
    default:
        hr = MF_E_UNSUPPORTED_FORMAT;
        break;
    }

    TRACEHR_RET(hr);
}

// Process server description packet
HRESULT CMediaSource::ProcessServerDescription(IBufferPacket *pPacket)
{
    HRESULT hr = S_OK;
    DWORD cbTotalLen = 0;
    DWORD cbConstantSize = 0;
    StspDescription desc = {};
    StspDescription *pDescription = nullptr;

    if (_eSourceState != SourceState_Opening)
    {
        // Server description should only be sent during opening state
        hr = MF_E_UNEXPECTED;
    }

    if (SUCCEEDED(hr))
    {
        hr = pPacket->GetTotalLength(&cbTotalLen);
    }

    // Minimum size of the operation payload is size of Description structure
    if (SUCCEEDED(hr) && cbTotalLen < sizeof(StspDescription))
    {
        hr = MF_E_UNSUPPORTED_FORMAT;
    }

    if (SUCCEEDED(hr))
    {
        // Copy description.
        hr = pPacket->MoveLeft(sizeof(desc), &desc);
    }


    // Size of the packet should match size described in the packet (size of Description structure
    // plus size of attribute blob)
    if (SUCCEEDED(hr))
    {
        cbConstantSize = sizeof(desc) + (desc.cNumStreams - 1) * sizeof(StspStreamDescription);
        // Check if the input parameters are valid. We only support 2 streams.
        if (cbConstantSize < sizeof(desc) || desc.cNumStreams == 0 || 
            desc.cNumStreams > 2 || cbTotalLen < cbConstantSize)
        {
            hr = MF_E_UNSUPPORTED_FORMAT;
        }
    }

    if (SUCCEEDED(hr))
    {
        BYTE *pPtr = new BYTE[cbConstantSize];
        if (pPtr == nullptr)
        {
            hr = E_OUTOFMEMORY;
        }

        if (SUCCEEDED(hr))
        {
            // Copy what we've already read
            CopyMemory(pPtr, &desc, sizeof(desc));

            // Add more stream data if necessary at the end
            hr = pPacket->MoveLeft(cbConstantSize - sizeof(desc), pPtr + sizeof(desc));
        }

        if (SUCCEEDED(hr))
        {
            DWORD cbAttributeSize = 0;

            // Data is ready
            pDescription = reinterpret_cast<StspDescription *>(pPtr);
            pPtr = nullptr;

            for (DWORD nStream = 0; nStream < desc.cNumStreams; ++nStream)
            {
                hr = DWordAdd(cbAttributeSize, pDescription->aStreams[nStream].cbAttributesSize, &cbAttributeSize);
                if (FAILED(hr))
                {
                    break;
                }
            }

            // Validate the parameters. Limit the total size of attributes to 64kB.
            if (FAILED(hr) || (cbTotalLen != (cbConstantSize + cbAttributeSize)) || (cbAttributeSize > 0x10000))
            {
                hr = MF_E_UNSUPPORTED_FORMAT;
            }
        }

        if (pPtr != nullptr)
        {
            delete[] pPtr;
        }
    }

    if (SUCCEEDED(hr))
    {
        // Create stream for every stream description sent by the server.
        for(DWORD nStream = 0; nStream < pDescription->cNumStreams; ++nStream)
        {
            ComPtr<CMediaStream> spStream;

            hr = CMediaStream::CreateInstance(&pDescription->aStreams[nStream], pPacket, this, &spStream);
            if (FAILED(hr))
            {
                break;
            }

            hr = _streams.InsertBack(spStream.Get());
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = InitPresentationDescription();
    }

    if (SUCCEEDED(hr))
    {
        // Everything succeeded we are in stopped state now
        _eSourceState = SourceState_Stopped;
        hr = CompleteOpen(S_OK);
    }

    if (pDescription != nullptr)
    {
        delete pDescription;
    }

    TRACEHR_RET(hr);
}

// Process a media sample reveived from the server.
HRESULT CMediaSource::ProcessServerSample(IBufferPacket *pPacket)
{
    HRESULT hr = S_OK;

    if (_eSourceState == SourceState_Started)
    {
        // Only process samples when we are in started state
        StspSampleHeader sampleHead = {};
        DWORD cbTotalSize;

        // Copy the header object
        hr = pPacket->MoveLeft(sizeof(sampleHead), &sampleHead);
        if (SUCCEEDED(hr))
        {
            hr = pPacket->GetTotalLength(&cbTotalSize);
        }

        if (SUCCEEDED(hr))
        {
            // Convert packet to MF sample
            ComPtr<IMFSample> spSample;
            ComPtr<CMediaStream> spStream;

            hr = GetStreamById(sampleHead.dwStreamId, &spStream);

            if (SUCCEEDED(hr) && spStream->IsActive())
            {
                hr = pPacket->ToMFSample(&spSample);

                if (SUCCEEDED(hr))
                {
                    // Forward sample to a proper stream.
                    hr = spStream->ProcessSample(&sampleHead, spSample.Get());
                }
            }
        }
    }
    else
    {
        hr = MF_E_UNEXPECTED;
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::InitPresentationDescription()
{
    ComPtr<IMFPresentationDescriptor> spPresentationDescriptor;
    IMFStreamDescriptor **aStreams = nullptr;
    HRESULT hr = S_OK;

    aStreams = new IMFStreamDescriptor*[_streams.GetCount()];
    if (aStreams == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    ZeroMemory(aStreams, sizeof(aStreams[0]) * _streams.GetCount());

    if (SUCCEEDED(hr))
    {
        StreamContainer::POSITION pos = _streams.FrontPosition();
        StreamContainer::POSITION posEnd = _streams.EndPosition();

        for (DWORD nStream = 0; pos != posEnd; ++nStream, pos = pos = _streams.Next(pos))
        {
            ComPtr<IMFMediaStream> spStream;
            hr = _streams.GetItemPos(pos, &spStream);
            if (FAILED(hr))
            {
                break;
            }
            hr = spStream->GetStreamDescriptor(&aStreams[nStream]);
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = MFCreatePresentationDescriptor(_streams.GetCount(), aStreams, &spPresentationDescriptor);
    }

    if (SUCCEEDED(hr))
    {
        for (DWORD nStream = 0; nStream < _streams.GetCount(); ++nStream)
        {
            hr = spPresentationDescriptor->SelectStream(nStream);
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        _spPresentationDescriptor = spPresentationDescriptor;
    }

    if (aStreams != nullptr)
    {
        for (DWORD nStream = 0; nStream < _streams.GetCount(); ++nStream)
        {
            if (aStreams[nStream] != nullptr)
            {
                aStreams[nStream]->Release();
            }
        }

        delete[] aStreams;
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::ValidatePresentationDescriptor(IMFPresentationDescriptor *pPD)
{
    HRESULT hr = S_OK;
    BOOL fSelected = FALSE;
    DWORD cStreams = 0;

    if (_streams.GetCount() == 0)
    {
        return E_UNEXPECTED;
    }

    // The caller's PD must have the same number of streams as ours.
    hr = pPD->GetStreamDescriptorCount(&cStreams);

    if (SUCCEEDED(hr))
    {
        if (cStreams != _streams.GetCount())
        {
            hr = E_INVALIDARG;
        }
    }

    // The caller must select at least one stream.
    if (SUCCEEDED(hr))
    {
        for (DWORD i = 0; i < cStreams; i++)
        {
            ComPtr<IMFStreamDescriptor> spSD;
            hr = pPD->GetStreamDescriptorByIndex(i, &fSelected, &spSD);
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::SelectStreams(IMFPresentationDescriptor *pPD)
{
    HRESULT hr = S_OK;

    for (DWORD nStream = 0; nStream < _streams.GetCount(); ++nStream)
    {
        ComPtr<IMFStreamDescriptor> spSD;
        ComPtr<CMediaStream> spStream;
        DWORD nStreamId;
        BOOL fSelected;

        // Get next stream descriptor
        hr = pPD->GetStreamDescriptorByIndex(nStream, &fSelected, &spSD);
        if (FAILED(hr))
        {
            break;
        }

        // Get stream id
        hr = spSD->GetStreamIdentifier(&nStreamId);
        if (FAILED(hr))
        {
            break;
        }

        // Get simple net media stream
        hr = GetStreamById(nStreamId, &spStream);
        if (FAILED(hr))
        {
            break;
        }

        // Remember if stream was selected
        bool fWasSelected = spStream->IsActive();
        hr = spStream->SetActive(!!fSelected);
        if (FAILED(hr))
        {
            break;
        }

        if (fSelected)
        {
            // Choose event type to send
            MediaEventType met = (fWasSelected) ? MEUpdatedStream : MENewStream;
            ComPtr<IUnknown> spStreamUnk;

            spStream.As(&spStreamUnk);

            hr = _spEventQueue->QueueEventParamUnk(met, GUID_NULL, hr, spStreamUnk.Get());
            if (FAILED(hr))
            {
                break;
            }

            // Start the stream. The stream will send the appropriate event.
            hr = spStream->Start();
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::DoStart(CStartOperation *pOp)
{
    assert(pOp->GetOperationType() == CSourceOperation::Operation_Start);
    ComPtr<IMFPresentationDescriptor> spPD = pOp->GetPresentationDescriptor();

    HRESULT hr = SelectStreams(spPD.Get());

    if (SUCCEEDED(hr))
    {
        _eSourceState = SourceState_Starting;
        hr = SendStartRequest();

        if (SUCCEEDED(hr))
        {
            _eSourceState = SourceState_Started;
            hr = _spEventQueue->QueueEventParamVar(MESourceStarted, GUID_NULL, hr, &pOp->GetData());
        }
    }

    if (FAILED(hr))
    {
        _eSourceState = SourceState_Stopped;
        hr = _spEventQueue->QueueEventParamVar(MESourceStarted, GUID_NULL, hr, nullptr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::DoStop(CSourceOperation *pOp)
{
    assert(pOp->GetOperationType() == CSourceOperation::Operation_Stop);

    HRESULT hr = S_OK;
    StreamContainer::POSITION pos = _streams.FrontPosition();
    StreamContainer::POSITION posEnd = _streams.EndPosition();

    for (; pos != posEnd; pos = _streams.Next(pos))
    {
        ComPtr<IMFMediaStream> spStream;
        hr = _streams.GetItemPos(pos, &spStream);
        if (FAILED(hr))
        {
            break;
        }
        CMediaStream *pStream = static_cast<CMediaStream*>(spStream.Get());
        if (pStream->IsActive())
        {
            hr = pStream->Flush();
            if (FAILED(hr))
            {
                break;
            }
            hr = pStream->Stop();
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    // Send the "stopped" event. This might include a failure code.
    (void)_spEventQueue->QueueEventParamVar(MESourceStopped, GUID_NULL, hr, NULL);

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::DoSetRate(CSetRateOperation *pOp)
{
    assert(pOp->GetOperationType() == CSourceOperation::Operation_SetRate);

    HRESULT hr = S_OK;
    StreamContainer::POSITION pos = _streams.FrontPosition();
    StreamContainer::POSITION posEnd = _streams.EndPosition();

    for (; pos != posEnd; pos = _streams.Next(pos))
    {
        ComPtr<IMFMediaStream> spStream;
        hr = _streams.GetItemPos(pos, &spStream);
        if (FAILED(hr))
        {
            break;
        }

        CMediaStream *pStream = static_cast<CMediaStream*>(spStream.Get());

        if (pStream->IsActive())
        {
            hr = pStream->Flush();
            if (FAILED(hr))
            {
                break;
            }
            hr = pStream->SetRate(pOp->GetRate());
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        _flRate = pOp->GetRate();
    }

    // Send the "rate changed" event. This might include a failure code.
    (void)_spEventQueue->QueueEventParamVar(MESourceRateChanged, GUID_NULL, hr, NULL);

    TRACEHR_RET(hr);
}


HRESULT CMediaSource::OnConnected(IMFAsyncResult *pResult)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);
    hr = pResult->GetStatus();

    if (SUCCEEDED(hr))
    {
        hr = CheckShutdown();
    }
    if (SUCCEEDED(hr) && _eSourceState != SourceState_Opening)
    {
        hr = MF_E_UNEXPECTED;
    }

    if (SUCCEEDED(hr))
    {
        hr = SendDescribeRequest();
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::OnDescriptionRequestSent(IMFAsyncResult *pResult)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);
    hr = pResult->GetStatus();

    if (SUCCEEDED(hr))
    {
        hr = CheckShutdown();
    }
    if (SUCCEEDED(hr) && _eSourceState != SourceState_Opening)
    {
        hr = MF_E_UNEXPECTED;
    }

    if (SUCCEEDED(hr))
    {
        // Start receiving
        hr = Receive();
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::OnStartRequestSent(IMFAsyncResult *pResult)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);
    hr = pResult->GetStatus();

    if (SUCCEEDED(hr))
    {
        hr = CheckShutdown();
    }
    if (SUCCEEDED(hr) && _eSourceState != SourceState_Starting && _eSourceState != SourceState_Started)
    {
        hr = MF_E_UNEXPECTED;
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSource::OnDataReceived(IMFAsyncResult *pResult)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);
    hr = pResult->GetStatus();

    if (SUCCEEDED(hr))
    {
        hr = CheckShutdown();
    }

    if (SUCCEEDED(hr))
    {
        hr = _spNetworkSender->EndReceive(pResult);
    }

    if (SUCCEEDED(hr))
    {
        hr = ParseCurrentBuffer();
    }

    if (SUCCEEDED(hr))
    {
        // Start receiving
        hr = Receive();
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

BOOL CMediaSource::IsRateSupported(float flRate, float *pflAdjustedRate)
{
    if (flRate < 0.00001f && flRate > -0.00001f)
    {
        *pflAdjustedRate = 0.0f;
        return TRUE;
    }
    else if(flRate < 1.0001f && flRate > 0.9999f)
    {
        *pflAdjustedRate = 1.0f;
        return TRUE;
    }
    return FALSE;
}
