//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "stdafx.h"
#include <InitGuid.h>
#include "StspMediaSink.h"
#include <ks.h>
#include <Codecapi.h>
#include "StspIncomingConnectionEventArgs.h"

using namespace Stsp;

namespace
{
    const DWORD c_cbReceiveBuffer = 8 * 1024;

    class ShutdownFunc
    {
    public:
        HRESULT operator()(IMFStreamSink *pStream) const
        {
            static_cast<CStreamSink *>(pStream)->Shutdown();
            return S_OK;
        }
    };

    class SetConnectedFunc
    {
    public:
        SetConnectedFunc(bool fConnected, LONGLONG llStartTime)
            : _fConnected(fConnected)
            , _llStartTime(llStartTime)
        {
        }

        HRESULT operator()(IMFStreamSink *pStream) const
        {
            return static_cast<CStreamSink *>(pStream)->SetConnected(_fConnected, _llStartTime);
        }

        bool _fConnected;
        LONGLONG _llStartTime;
    };

    class StartFunc
    {
    public:
        StartFunc(LONGLONG llStartTime)
            : _llStartTime(llStartTime)
        {
        }

        HRESULT operator()(IMFStreamSink *pStream) const
        {
            return static_cast<CStreamSink *>(pStream)->Start(_llStartTime);
        }

        LONGLONG _llStartTime;
    };

    class StopFunc
    {
    public:
        HRESULT operator()(IMFStreamSink *pStream) const
        {
            return static_cast<CStreamSink *>(pStream)->Stop();
        }
    };

    template <class T, class TFunc>
    HRESULT ForEach(ComPtrList<T> &col, TFunc fn)
    {
        ComPtrList<T>::POSITION pos = col.FrontPosition();
        ComPtrList<T>::POSITION endPos = col.EndPosition();
        HRESULT hr = S_OK;

        for (;pos != endPos; pos = col.Next(pos))
        {
            ComPtr<T> spStream;

            hr = col.GetItemPos(pos, &spStream);
            if (FAILED(hr))
            {
                break;
            }

            hr = fn(spStream.Get());
        }

        return hr;
    }

    static HRESULT AddAttribute( _In_ GUID guidKey, _In_ IPropertyValue *pValue, _In_ IMFAttributes* pAttr )
    {
        HRESULT hr = S_OK;
        PROPVARIANT var;
        PropertyType type;
        hr = pValue->get_Type(&type);
        ZeroMemory(&var, sizeof(var));

        if (SUCCEEDED(hr))
        {
            switch( type )
            {
            case PropertyType_UInt8Array:
                {
                    UINT32 cbBlob;
                    BYTE *pbBlog = nullptr;
                    hr = pValue->GetUInt8Array( &cbBlob, &pbBlog );
                    if (SUCCEEDED(hr))
                    {
                        if (pbBlog == nullptr)
                        {
                            hr = E_INVALIDARG;
                        }
                        else
                        {
                            hr = pAttr->SetBlob( guidKey, pbBlog, cbBlob );
                        }
                    }
                    CoTaskMemFree( pbBlog );
                }
                break;

            case PropertyType_Double:
                {
                    DOUBLE value;
                    hr = pValue->GetDouble(&value);
                    if (SUCCEEDED(hr))
                    {
                        hr = pAttr->SetDouble(guidKey, value);
                    }
                }
                break;

            case PropertyType_Guid:
                {
                    GUID value;
                    hr = pValue->GetGuid( &value );
                    if (SUCCEEDED(hr))
                    {
                        hr = pAttr->SetGUID(guidKey, value);
                    }
                }
                break;

            case PropertyType_String:
                {
                    HString value;
                    hr = pValue->GetString(value.GetAddressOf());
                    if (SUCCEEDED(hr))
                    {
                        UINT32 len = 0;
                        LPCWSTR szValue = WindowsGetStringRawBuffer(value.Get(), &len);
                        hr = pAttr->SetString( guidKey, szValue);
                    }
                }
                break;

            case PropertyType_UInt32:
                {
                    UINT32 value;
                    hr = pValue->GetUInt32(&value);
                    if (SUCCEEDED(hr))
                    {
                        pAttr->SetUINT32(guidKey, value);
                    }
                }
                break;

            case PropertyType_UInt64:
                {
                    UINT64 value;
                    hr = pValue->GetUInt64(&value);
                    if (SUCCEEDED(hr))
                    {
                        hr = pAttr->SetUINT64(guidKey, value);
                    }
                }
                break;

            case PropertyType_Inspectable:
                {
                    ComPtr<IInspectable> value;
                    hr = TYPE_E_TYPEMISMATCH;
                    if (SUCCEEDED(hr))
                    {
                        pAttr->SetUnknown(guidKey, value.Get());
                    }
                }
                break;

                // ignore unknown values
            }
        }

        return hr;
    }

    HRESULT ConvertPropertiesToMediaType(_In_ IMediaEncodingProperties *pMEP, _Outptr_ IMFMediaType **ppMT)
    {
        HRESULT hr = S_OK;
        ComPtr<IMFMediaType> spMT;
        ComPtr<IMap<GUID, IInspectable*>> spMap;
        ComPtr<IIterable<IKeyValuePair<GUID, IInspectable*>*>> spIterable;
        ComPtr<IIterator<IKeyValuePair<GUID, IInspectable*>*>> spIterator;

        if (pMEP == nullptr || ppMT == nullptr)
        {
            return E_INVALIDARG;
        }
        *ppMT = nullptr;

        hr = pMEP->get_Properties(&spMap);

        if (SUCCEEDED(hr))
        {
            hr = spMap.As(&spIterable);
        }
        if (SUCCEEDED(hr))
        {
            hr = spIterable->First(&spIterator);
        }
        if (SUCCEEDED(hr))
        {
            MFCreateMediaType(&spMT);
        }

        boolean hasCurrent = false;
        if (SUCCEEDED(hr))
        {
            hr = spIterator->get_HasCurrent(&hasCurrent);
        }

        while (hasCurrent)
        {
            ComPtr<IKeyValuePair<GUID, IInspectable*> > spKeyValuePair;
            ComPtr<IInspectable> spValue;
            ComPtr<IPropertyValue> spPropValue;
            GUID guidKey;

            hr = spIterator->get_Current(&spKeyValuePair);
            if (FAILED(hr))
            {
                break;
            }
            hr = spKeyValuePair->get_Key(&guidKey);
            if (FAILED(hr))
            {
                break;
            }
            hr = spKeyValuePair->get_Value(&spValue);
            if (FAILED(hr))
            {
                break;
            }
            hr = spValue.As(&spPropValue);
            if (FAILED(hr))
            {
                break;
            }
            hr = AddAttribute(guidKey, spPropValue.Get(), spMT.Get());
            if (FAILED(hr))
            {
                break;
            }

            hr = spIterator->MoveNext(&hasCurrent);
            if (FAILED(hr))
            {
                break;
            }
        }


        if (SUCCEEDED(hr))
        {
            ComPtr<IInspectable> spValue;
            ComPtr<IPropertyValue> spPropValue;
            GUID guiMajorType;

            hr = spMap->Lookup(MF_MT_MAJOR_TYPE, spValue.GetAddressOf());

            if (SUCCEEDED(hr))
            {
                hr = spValue.As(&spPropValue);
            }
            if (SUCCEEDED(hr))
            {
                hr = spPropValue->GetGuid(&guiMajorType);
            }
            if (SUCCEEDED(hr))
            {
                if (guiMajorType != MFMediaType_Video && guiMajorType != MFMediaType_Audio)
                {
                    hr = E_UNEXPECTED;
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            *ppMT = spMT.Detach();
        }

        return hr;
    }

    DWORD GetStreamId(ABI::Windows::Media::Capture::MediaStreamType mediaStreamType)
    {
        return 3-mediaStreamType;
    }
}

CMediaSink::CMediaSink() 
: _cRef(1)
, _IsShutdown(false)
, _IsConnected(false)
, _llStartTime(0)
, _cStreamsEnded(0)
#pragma warning(push)
#pragma warning(disable:4355)
, _OnInitializeCB(this, &CMediaSink::OnIntialize)
, _OnAcceptCB(this, &CMediaSink::OnAccept)
, _OnReceivedCB(this, &CMediaSink::OnReceived)
, _OnDesciptionSentCB(this, &CMediaSink::OnDescriptionSent)
, _OnFireIncomingConnectionCB(this, &CMediaSink::OnFireIncomingConnection)
, _OnAcceptConnectionCB(this, &CMediaSink::OnAcceptConnection)
, _OnRefuseConnectionCB(this, &CMediaSink::OnRefuseConnection)
, _dwWaitingConnectionId(0)
#pragma warning(pop)
{
}

CMediaSink::~CMediaSink()
{
    assert(_IsShutdown);
}

///  IStspMediaSink
IFACEMETHODIMP CMediaSink::Close()
{
    Shutdown();

    return S_OK;
}

IFACEMETHODIMP CMediaSink::InitializeAsync(
IMediaEncodingProperties *audioEncodingProperties, 
IMediaEncodingProperties *videoEncodingProperties, 
IInitializeOperation **asyncInfo)
{
    HRESULT hr = S_OK;
    AutoLock lock(_critSec);

    if ((audioEncodingProperties == nullptr && videoEncodingProperties == nullptr) || asyncInfo == nullptr)
    {
        return E_INVALIDARG;
    }
    *asyncInfo = nullptr;
    if (_spInitializeOperation != nullptr)
    {
        return MF_E_INVALIDREQUEST;
    }

    hr = MakeAndInitialize<CInitializeOperation>(&_spInitializeOperation, this, audioEncodingProperties, videoEncodingProperties);

    if (SUCCEEDED(hr))
    {
        *asyncInfo = _spInitializeOperation.Get();
        (*asyncInfo)->AddRef();
    }  

    return hr;
}

IFACEMETHODIMP CMediaSink::add_IncomingConnectionEvent( 
_In_opt_ ABI::Microsoft::Samples::SimpleCommunication::IIncomingConnectionHandler *handler,
_Out_ EventRegistrationToken *cookie)
{
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        _evtStspSourceCreated.Add(handler, cookie);
    }

    return hr;
}

IFACEMETHODIMP CMediaSink::remove_IncomingConnectionEvent(EventRegistrationToken cookie)
{
    AutoLock lock(_critSec);

    _evtStspSourceCreated.Remove(cookie);

    return S_OK;
}


HRESULT CMediaSink::SetMediaStreamProperties( 
MediaStreamType MediaStreamType,
_In_opt_ IMediaEncodingProperties *mediaEncodingProperties)
{
    HRESULT hr = S_OK;
    ComPtr<IMFMediaType> spMediaType;

    if (MediaStreamType != MediaStreamType_VideoRecord && MediaStreamType != MediaStreamType_Audio)
    {
        return E_INVALIDARG;
    }

    RemoveStreamSink(GetStreamId(MediaStreamType));

    if (mediaEncodingProperties != nullptr)
    {
        ComPtr<IMFStreamSink> spStreamSink;
        hr = ConvertPropertiesToMediaType(mediaEncodingProperties, &spMediaType);
        if (SUCCEEDED(hr))
        {
            hr = AddStreamSink(GetStreamId(MediaStreamType), spMediaType.Get(), spStreamSink.GetAddressOf());
        }
    }

    return hr;
}

///  IMFMediaSink
IFACEMETHODIMP CMediaSink::GetCharacteristics(DWORD *pdwCharacteristics)
{
    if (pdwCharacteristics == NULL)
    {
        return E_INVALIDARG;
    }
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        // Rateless sink.
        *pdwCharacteristics = MEDIASINK_RATELESS;
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::AddStreamSink(
DWORD dwStreamSinkIdentifier,
IMFMediaType *pMediaType,
IMFStreamSink **ppStreamSink)
{
    CStreamSink *pStream = nullptr;
    ComPtr<IMFStreamSink> spMFStream;
    AutoLock lock(_critSec);
    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = GetStreamSinkById(dwStreamSinkIdentifier, &spMFStream);
    }

    if (SUCCEEDED(hr))
    {
        hr = MF_E_STREAMSINK_EXISTS;
    }
    else
    {
        hr = S_OK;
    }

    if (SUCCEEDED(hr))
    {
        pStream = new CStreamSink(dwStreamSinkIdentifier);
        if (pStream == nullptr)
        {
            hr = E_OUTOFMEMORY;
        }
        spMFStream.Attach(pStream);
    }

    // Initialize the stream.
    if (SUCCEEDED(hr))
    {
        hr = pStream->Initialize(this, _spNetworkSender.Get());
    }

    if (SUCCEEDED(hr) && pMediaType != nullptr)
    {
        hr = pStream->SetCurrentMediaType(pMediaType);
    }

    if (SUCCEEDED(hr))
    {
        StreamContainer::POSITION pos = _streams.FrontPosition();
        StreamContainer::POSITION posEnd = _streams.EndPosition();

        // Insert in proper position
        for (; pos != posEnd; pos = _streams.Next(pos))
        {
            DWORD dwCurrId;
            ComPtr<IMFStreamSink> spCurr;
            hr = _streams.GetItemPos(pos, &spCurr);
            if (FAILED(hr))
            {
                break;
            }
            hr = spCurr->GetIdentifier(&dwCurrId);
            if (FAILED(hr))
            {
                break;
            }

            if (dwCurrId > dwStreamSinkIdentifier)
            {
                break;
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = _streams.InsertPos(pos, pStream);
        }
    }

    if (SUCCEEDED(hr))
    {
        *ppStreamSink = spMFStream.Detach();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::RemoveStreamSink(DWORD dwStreamSinkIdentifier)
{
    AutoLock lock(_critSec);
    HRESULT hr = CheckShutdown();
    StreamContainer::POSITION pos = _streams.FrontPosition();
    StreamContainer::POSITION endPos = _streams.EndPosition();
    ComPtr<IMFStreamSink> spStream;

    if (SUCCEEDED(hr))
    {
        for (;pos != endPos; pos = _streams.Next(pos))
        {
            hr = _streams.GetItemPos(pos, &spStream);
            DWORD dwId;

            if (FAILED(hr))
            {
                break;
            }

            hr = spStream->GetIdentifier(&dwId);
            if (FAILED(hr) || dwId == dwStreamSinkIdentifier)
            {
                break;
            }
        }

        if (pos == endPos)
        {
            hr = MF_E_INVALIDSTREAMNUMBER;
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = _streams.Remove(pos, nullptr);
        static_cast<CStreamSink *>(spStream.Get())->Shutdown();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::GetStreamSinkCount(_Out_ DWORD *pcStreamSinkCount)
{
    if (pcStreamSinkCount == NULL)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *pcStreamSinkCount = _streams.GetCount();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::GetStreamSinkByIndex(
DWORD dwIndex,
_Outptr_ IMFStreamSink **ppStreamSink)
{
    if (ppStreamSink == NULL)
    {
        return E_INVALIDARG;
    }

    ComPtr<IMFStreamSink> spStream;
    AutoLock lock(_critSec);
    DWORD cStreams = _streams.GetCount();

    if (dwIndex >= cStreams)
    {
        return MF_E_INVALIDINDEX;
    }

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        StreamContainer::POSITION pos = _streams.FrontPosition();
        StreamContainer::POSITION endPos = _streams.EndPosition();
        DWORD dwCurrent = 0;

        for (;pos != endPos && dwCurrent < dwIndex; pos = _streams.Next(pos), ++dwCurrent)
        {
            // Just move to proper position
        }

        if (pos == endPos)
        {
            hr = MF_E_UNEXPECTED;
        }
        else
        {
            hr = _streams.GetItemPos(pos, &spStream);
        }
    }

    if (SUCCEEDED(hr))
    {
        *ppStreamSink = spStream.Detach();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::GetStreamSinkById(
DWORD dwStreamSinkIdentifier,
IMFStreamSink **ppStreamSink)
{
    if (ppStreamSink == NULL)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);
    HRESULT hr = CheckShutdown();
    ComPtr<IMFStreamSink> spResult;

    if (SUCCEEDED(hr))
    {
        StreamContainer::POSITION pos = _streams.FrontPosition();
        StreamContainer::POSITION endPos = _streams.EndPosition();

        for (;pos != endPos; pos = _streams.Next(pos))
        {
            ComPtr<IMFStreamSink> spStream;
            hr = _streams.GetItemPos(pos, &spStream);
            DWORD dwId;

            if (FAILED(hr))
            {
                break;
            }

            hr = spStream->GetIdentifier(&dwId);
            if (FAILED(hr))
            {
                break;
            }
            else if (dwId == dwStreamSinkIdentifier)
            {
                spResult = spStream;
                break;
            }
        }

        if (pos == endPos)
        {
            hr = MF_E_INVALIDSTREAMNUMBER;
        }
    }

    if (SUCCEEDED(hr))
    {
        assert(spResult);
        *ppStreamSink = spResult.Detach();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::SetPresentationClock(IMFPresentationClock *pPresentationClock)
{
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    // If we already have a clock, remove ourselves from that clock's
    // state notifications.
    if (SUCCEEDED(hr))
    {
        if (_spClock)
        {
            hr = _spClock->RemoveClockStateSink(this);
        }
    }

    // Register ourselves to get state notifications from the new clock.
    if (SUCCEEDED(hr))
    {
        if (pPresentationClock)
        {
            hr = pPresentationClock->AddClockStateSink(this);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Release the pointer to the old clock.
        // Store the pointer to the new clock.
        _spClock = pPresentationClock;
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::GetPresentationClock(IMFPresentationClock **ppPresentationClock)
{
    if (ppPresentationClock == NULL)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_spClock == NULL)
        {
            hr = MF_E_NO_CLOCK; // There is no presentation clock.
        }
        else
        {
            // Return the pointer to the caller.
            *ppPresentationClock = _spClock.Get();
            (*ppPresentationClock)->AddRef();
        }
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::Shutdown()
{
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        ForEach(_streams, ShutdownFunc());
        _streams.Clear();

        if (_spNetworkSender)
        {
            _spNetworkSender->Close();
        }

        _spNetworkSender.ReleaseAndGetAddressOf();
        _spClock.ReleaseAndGetAddressOf();

        _IsShutdown = true;
    }

    TRACEHR_RET(hr);
}

// IMFClockStateSink
IFACEMETHODIMP CMediaSink::OnClockStart(
                                        MFTIME hnsSystemTime,
                                        LONGLONG llClockStartOffset)
{
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        TRACE(TRACE_LEVEL_LOW, L"OnClockStart ts=%I64d\n", llClockStartOffset);
        // Start each stream.
        _llStartTime = llClockStartOffset;
        hr = ForEach(_streams, StartFunc(llClockStartOffset));
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaSink::OnClockStop(
                                       MFTIME hnsSystemTime)
{
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        // Stop each stream
        hr = ForEach(_streams, StopFunc());
    }

    TRACEHR_RET(hr);
}


IFACEMETHODIMP CMediaSink::OnClockPause(
                                        MFTIME hnsSystemTime)
{
    return MF_E_INVALID_STATE_TRANSITION;
}

IFACEMETHODIMP CMediaSink::OnClockRestart(
MFTIME hnsSystemTime)
{
    return MF_E_INVALID_STATE_TRANSITION;
}

IFACEMETHODIMP CMediaSink::OnClockSetRate(
/* [in] */ MFTIME hnsSystemTime,
/* [in] */ float flRate)
{
    return S_OK;
}

void CMediaSink::ReportEndOfStream()
{
    AutoLock lock(_critSec);
    ++_cStreamsEnded;

    if (_spNetworkSender != nullptr)
    {
        // TODO: Fix now we have only one stream
        _spNetworkSender->Close();
    }
}

HRESULT CMediaSink::_TriggerInitialization()
{
    AutoLock lock(_critSec);
    HRESULT hr = CheckShutdown();

    if (_spNetworkSender != nullptr)
    {
        hr = MF_E_ALREADY_INITIALIZED;
    }

    if (SUCCEEDED(hr))
    {
        hr = MFPutWorkItem2(MFASYNC_CALLBACK_QUEUE_MULTITHREADED, 0, &_OnInitializeCB, nullptr);
    }

    return hr;
}

HRESULT CMediaSink::_TriggerAcceptConnection(DWORD dwConnectionId)
{
    AutoLock lock(_critSec);
    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_dwWaitingConnectionId != 0 && dwConnectionId == _dwWaitingConnectionId)
        {
            _dwWaitingConnectionId = 0;
            hr = MFPutWorkItem2(MFASYNC_CALLBACK_QUEUE_MULTITHREADED, 0, &_OnAcceptConnectionCB, nullptr);
        }
        else
        {
            hr = MF_E_INVALIDREQUEST;
        }
    }

    return hr;
}

HRESULT CMediaSink::_TriggerRefuseConnection(DWORD dwConnectionId)
{
    AutoLock lock(_critSec);
    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_dwWaitingConnectionId != 0 && dwConnectionId == _dwWaitingConnectionId)
        {
            _dwWaitingConnectionId = 0;
            hr = MFPutWorkItem2(MFASYNC_CALLBACK_QUEUE_MULTITHREADED, 0, &_OnRefuseConnectionCB, nullptr);
        }
        else
        {
            hr = MF_E_INVALIDREQUEST;
        }
    }

    return hr;
}
/// Private methods

// Send media description to the client
HRESULT CMediaSink::SendDescription()
{
    // Size of the description buffer
    const DWORD c_cStreams = _streams.GetCount();
    const DWORD c_cbDescriptionSize = sizeof(StspDescription) + (c_cStreams-1) * sizeof(StspStreamDescription);
    const DWORD c_cbPacketSize = sizeof(StspOperationHeader) + c_cbDescriptionSize;

    HRESULT hr = S_OK;
    ComPtr<IMediaBufferWrapper> spBuffer;
    ComPtr<IMediaBufferWrapper> *arrspAttributes = new ComPtr<IMediaBufferWrapper>[c_cStreams];
    if (arrspAttributes == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        // Create send buffer
        hr = CreateMediaBufferWrapper(c_cbPacketSize, &spBuffer);
    }

    if (SUCCEEDED(hr))
    {
        // Prepare operation header
        BYTE *pBuf = spBuffer->GetBuffer();
        StspOperationHeader *pOpHeader = reinterpret_cast<StspOperationHeader *>(pBuf);
        pOpHeader->cbDataSize = c_cbDescriptionSize;
        pOpHeader->eOperation = StspOperation_ServerDescription;

        // Prepare description
        StspDescription *pDescription = reinterpret_cast<StspDescription *>(pBuf + sizeof(StspOperationHeader));
        pDescription->cNumStreams = c_cStreams;

        if (SUCCEEDED(hr))
        {
            StreamContainer::POSITION pos = _streams.FrontPosition();
            StreamContainer::POSITION endPos = _streams.EndPosition();
            DWORD nStream = 0;
            for (;pos != endPos; pos = _streams.Next(pos), ++nStream)
            {
                ComPtr<IMFStreamSink> spStream;

                hr = _streams.GetItemPos(pos, &spStream);
                if (FAILED(hr))
                {
                    break;
                }

                // Fill out stream description
                hr = FillStreamDescription(static_cast<CStreamSink *>(spStream.Get()), &pDescription->aStreams[nStream], &arrspAttributes[nStream]);
                if (FAILED(hr))
                {
                    break;
                }

                // Add size of variable size attribute blob to size of the package.
                pOpHeader->cbDataSize += pDescription->aStreams[nStream].cbAttributesSize;
            }
        }

        if (SUCCEEDED(hr))
        {
            // Set length of the packet
            hr = spBuffer->SetCurrentLength(c_cbPacketSize);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Prepare packet to send
        ComPtr<IBufferPacket> spPacket;

        hr = CreateBufferPacket(&spPacket);
        if (SUCCEEDED(hr))
        {
            // Add fixed size header and description to the packet
            hr = spPacket->AddBuffer(spBuffer.Get());
        }

        if (SUCCEEDED(hr))
        {
            for (DWORD nStream = 0; nStream < c_cStreams; ++nStream)
            {
                // Add variable size attributes.
                hr = spPacket->AddBuffer(arrspAttributes[nStream].Get());
            }
        }

        if (SUCCEEDED(hr))
        {
            // Send the data.
            hr = _spNetworkSender->BeginSend(spPacket.Get(), &_OnDesciptionSentCB, nullptr);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Keep receiving
        hr = _spNetworkSender->BeginReceive(_spReceiveBuffer.Get(), &_OnReceivedCB, nullptr);
    }

    if (arrspAttributes != nullptr)
    {
        delete[] arrspAttributes;
    }

    TRACEHR_RET(hr);
}

// Fill stream description and prepare attributes blob.
HRESULT CMediaSink::FillStreamDescription(CStreamSink *pStream, StspStreamDescription *pStreamDescription, IMediaBufferWrapper **ppAttributes)
{
    assert(pStream != nullptr);
    assert(pStreamDescription != nullptr);
    assert(ppAttributes != nullptr);

    HRESULT hr = S_OK;
    ComPtr<IMFMediaType> spMediaType;
    ComPtr<IMFMediaType> spMediaTypeToSend;

    // Clear the stream descriptor
    ZeroMemory(pStreamDescription, sizeof(*pStreamDescription));

    // Get current media type
    hr = pStream->GetCurrentMediaType(&spMediaType);

    if (SUCCEEDED(hr))
    {
        // Get major type (Audio, Video and so on)
        hr = pStream->GetMajorType(&pStreamDescription->guiMajorType);
    }

    if (SUCCEEDED(hr))
    {
        // Get subtype (format of the stream)
        hr = spMediaType->GetGUID(MF_MT_SUBTYPE, &pStreamDescription->guiSubType);
    }

    if (SUCCEEDED(hr))
    {
        hr = MFCreateMediaType(&spMediaTypeToSend);
    }

    if (SUCCEEDED(hr))
    {
        hr = FilterOutputMediaType(spMediaType.Get(), spMediaTypeToSend.Get());
    }

    if (SUCCEEDED(hr))
    {
        // Get id
        hr = pStream->GetIdentifier(&pStreamDescription->dwStreamId);
    }

    ComPtr<IMediaBufferWrapper> spAttributes;
    if (SUCCEEDED(hr))
    {
        // Get size of attributes blob
        hr = MFGetAttributesAsBlobSize(spMediaTypeToSend.Get(), &pStreamDescription->cbAttributesSize);
    }

    if (SUCCEEDED(hr))
    {
        // Prepare a buffer for attribute blob
        hr = CreateMediaBufferWrapper(pStreamDescription->cbAttributesSize, &spAttributes);
    }

    if (SUCCEEDED(hr))
    {
        // Set length of the buffer
        hr = spAttributes->SetCurrentLength(pStreamDescription->cbAttributesSize);
    }

    if (SUCCEEDED(hr))
    {
        // Copy attributes to the buffer
        hr = MFGetAttributesAsBlob(spMediaTypeToSend.Get(), spAttributes->GetBuffer(), pStreamDescription->cbAttributesSize);
    }

    if (SUCCEEDED(hr))
    {
        *ppAttributes = spAttributes.Detach();
    }

    TRACEHR_RET(hr);
}

void CMediaSink::HandleError(HRESULT hr)
{
    // TODO: Implement
}

// Finish initialization
HRESULT CMediaSink::OnIntialize(IMFAsyncResult* pAsyncResult)
{
    HRESULT hr = S_OK;
    AutoLock lock(_critSec);
    hr = CheckShutdown();
    ComPtr<INetworkServer> spServer;
    ComPtr<IMFStreamSink> spStream1;

    if (SUCCEEDED(hr) && _spNetworkSender != nullptr)
    {
        hr = MF_E_ALREADY_INITIALIZED;
    }

    if (SUCCEEDED(hr) && _spInitializeOperation == nullptr)
    {
        hr = E_UNEXPECTED;
    }

    if (SUCCEEDED(hr))
    {
        hr = CBaseAttributes<>::Initialize();
    }

    if (SUCCEEDED(hr))
    {
        // Create network server object
        hr = CreateNetworkServer(c_wStspDefaultPort, &spServer);
    }

    if (SUCCEEDED(hr))
    {
        // Get INetworkChannel interface
        hr = spServer.As(&_spNetworkSender);
    }

    if (SUCCEEDED(hr))
    {
        hr = SetMediaStreamProperties(MediaStreamType_Audio, _spInitializeOperation->GetAudioEncodingProperties());
    }
    if (SUCCEEDED(hr))
    {
        hr = SetMediaStreamProperties(MediaStreamType_VideoRecord, _spInitializeOperation->GetVideoEncodingProperties());
    }

    if (SUCCEEDED(hr))
    {
        // Start accepting connections
        hr = spServer->BeginAccept(&_OnAcceptCB, nullptr);
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    _spInitializeOperation->HandleAsyncCompletion(hr);
    _spInitializeOperation.ReleaseAndGetAddressOf();

    TRACEHR_RET(hr);
}

// Connection has been accepted
HRESULT CMediaSink::OnAccept(IMFAsyncResult* pAsyncResult)
{
    HRESULT hr = S_OK;
    ComPtr<INetworkServer> spServer;
    ComPtr<IHostDescription> spRemoteHostDescription;
    AutoLock lock(_critSec);
    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spNetworkSender.As(&spServer);
    }

    if (SUCCEEDED(hr))
    {
        hr = spServer->EndAccept(pAsyncResult, spRemoteHostDescription.GetAddressOf());
    }

    if (SUCCEEDED(hr))
    {
        hr = PrepareRemoteUrl(spRemoteHostDescription.Get(), _strRemoteUrl.GetAddressOf());
    }

    if (SUCCEEDED(hr))
    {
        (void) MFPutWorkItem2( MFASYNC_CALLBACK_QUEUE_MULTITHREADED, 0, &_OnFireIncomingConnectionCB, nullptr );
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

// Data has been received from the client
HRESULT CMediaSink::OnReceived(IMFAsyncResult* pAsyncResult)
{
    HRESULT hr = S_OK;
    StspOperation eOp = StspOperation_Unknown;

    AutoLock lock(_critSec);
    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spNetworkSender->EndReceive(pAsyncResult);
    }

    if (SUCCEEDED(hr))
    {
        BYTE *pBuf = _spReceiveBuffer->GetBuffer();
        DWORD cbCurrentLen;
        _spReceiveBuffer->GetMediaBuffer()->GetCurrentLength(&cbCurrentLen);

        // Validate if the data received from the client is sufficient to fit operation header which is the smallest size of message that we can handle.
        if (cbCurrentLen != sizeof(StspOperationHeader))
        {
            hr = MF_E_INVALID_FORMAT;
        }

        if (SUCCEEDED(hr))
        {
            StspOperationHeader *pOpHeader = reinterpret_cast<StspOperationHeader *>(pBuf);
            // We only support client's request for media description
            if (pOpHeader->cbDataSize != 0 ||
                ((pOpHeader->eOperation != StspOperation_ClientRequestDescription) &&
                (pOpHeader->eOperation != StspOperation_ClientRequestStart) &&
                (pOpHeader->eOperation != StspOperation_ClientRequestStop)))
            {
                hr = MF_E_INVALID_FORMAT;
            }
            else
            {
                eOp = pOpHeader->eOperation;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        switch(eOp)
        {
        case StspOperation_ClientRequestDescription:
            // Send description to the client
            hr = SendDescription();
            break;
        case StspOperation_ClientRequestStart:
            {
                _IsConnected = true;
                if (_spClock)
                {
                    hr = _spClock->GetTime(&_llStartTime);
                }


                if (SUCCEEDED(hr))
                {
                    // We are now connected we can start streaming.
                    ForEach(_streams, SetConnectedFunc(true, _llStartTime));
                }
            }
            break;
        default:
            hr = MF_E_INVALID_FORMAT;
            break;
        }
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

// Description has been successfully sent
HRESULT CMediaSink::OnDescriptionSent(IMFAsyncResult* pAsyncResult)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);
    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spNetworkSender->EndSend(pAsyncResult);
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSink::OnFireIncomingConnection(IMFAsyncResult* pAsyncResult)
{
    HRESULT hr = S_OK;
    ComPtr<ABI::Microsoft::Samples::SimpleCommunication::IIncomingConnectionEventArgs> spArgs;

    {
        AutoLock lock(_critSec);
        hr = CheckShutdown();

        _dwWaitingConnectionId = LODWORD(GetTickCount64());
        if (_dwWaitingConnectionId == 0) 
        {
            ++_dwWaitingConnectionId;
        }

        if (SUCCEEDED(hr))
        {
            hr = MakeAndInitialize<CIncomingConnectionEventArgs>(spArgs.GetAddressOf(), _dwWaitingConnectionId, _strRemoteUrl.Get(), this);
        }
    }

    if (SUCCEEDED(hr))
    {
        _evtStspSourceCreated.InvokeAll(this, spArgs.Get());
    }

    {
        AutoLock lock(_critSec);
        if (FAILED(hr))
        {
            HandleError(hr);
        }
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSink::OnAcceptConnection(IMFAsyncResult* pAsyncResult)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);
    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        // Create receive buffer
        hr = CreateMediaBufferWrapper(c_cbReceiveBuffer, &_spReceiveBuffer);
    }

    if (SUCCEEDED(hr))
    {
        // Start receiving
        hr = _spNetworkSender->BeginReceive(_spReceiveBuffer.Get(), &_OnReceivedCB, nullptr);
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSink::OnRefuseConnection(IMFAsyncResult* pAsyncResult)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);
    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        // Connection refused disconnect current client.
        hr = _spNetworkSender->Disconnect();
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<INetworkServer> spServer;

        hr = _spNetworkSender.As(&spServer);
        if (SUCCEEDED(hr))
        {
            // Wait for another connection
            hr = spServer->BeginAccept(&_OnAcceptCB, nullptr);
        }
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaSink::PrepareRemoteUrl(IHostDescription *pRemoteHostDescription, HSTRING *pstrRemoteUrl)
{
    HRESULT hr = S_OK;
    LPCWSTR pszRemoteHostName = pRemoteHostDescription->GetHostName();
    WCHAR szBuffer[MAX_PATH];

    if (pRemoteHostDescription->GetNetworkType() == StspNetworkType_IPv4)
    {
        hr = StringCchPrintf(szBuffer, _countof(szBuffer), L"%s://%s", c_szStspScheme, pszRemoteHostName);
    }
    else if (pRemoteHostDescription->GetNetworkType() == StspNetworkType_IPv6)
    {
        hr = StringCchPrintf(szBuffer, _countof(szBuffer), L"%s://[%s]", c_szStspScheme, pszRemoteHostName);
    }
    else
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        HString strRemoteUrl;
        hr = strRemoteUrl.Set(szBuffer);

        if (SUCCEEDED(hr))
        {
            *pstrRemoteUrl = strRemoteUrl.Detach();
        }
    }

    return hr;
}
